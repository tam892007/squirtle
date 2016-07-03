'use strict';
mainApp.controller('transactionCurrentController',
[
    '$scope', '_', '$timeout', '$uibModal', '$window', 'transactionService', 'tradeService', 'Notification',
    'AccountState',
    'TransactionState', 'ConfigData',
    function($scope,
        _,
        $timeout,
        $uibModal,
        $window,
        transactionService,
        tradeService,
        Notification,
        AccountState,
        TransactionState,
        ConfigData) {

        $scope.updateStatus = function() {
            return tradeService.status({},
                    function(response) {
                        $scope.info = response;
                        generateOverviewStateOverCurrentAccountState();
                    })
                .$promise;
        }

        function generateOverviewStateOverCurrentAccountState() {
            switch ($scope.info.state) {
            case AccountState.Default:
                break;
            case AccountState.AbandonOne:
                break;
            case AccountState.WaitGive:
                $scope.overviewState.queued = 1;
                $scope.overviewState.give = 0;
                break;
            case AccountState.InGiveTransaction:
                $scope.overviewState.queued = 1;
                $scope.overviewState.give = 0;
                break;
            case AccountState.Gave:
                $scope.overviewState.queued = 1;
                $scope.overviewState.give = 1;
                $scope.overviewState.waitCofirm = 1;
                $scope.overviewState.receive = -1;
                break;
            case AccountState.WaitReceive:
                $scope.overviewState.queued = 1;
                $scope.overviewState.give = 1;
                $scope.overviewState.waitCofirm = 1;
                $scope.overviewState.receive = 0;
                break;
            case AccountState.InReceiveTransaction:
                $scope.overviewState.queued = 1;
                $scope.overviewState.give = 1;
                $scope.overviewState.waitCofirm = 1;
                $scope.overviewState.receive = 0;
                break;
            case AccountState.NotGive:
                $scope.overviewState.queued = -1;
                break;
            case AccountState.NotConfirm:
                $scope.overviewState.queued = -1;
                break;
            case AccountState.ReportedNotTransfer:
                $scope.overviewState.queued = -1;
                break;
            }
            console.log($scope.overviewState);
        }

        $scope.queueGive = function() {
            $scope.info.isAllowGive = false;
            tradeService.queueGive({},
                function(response) {
                    Notification.success('Queue give successful!');
                    $scope.$emit('user:removePinBalance', 1);
                    $scope.updateStatus();
                },
                function(response) {
                    $scope.info.isAllowGive = true;
                    Notification.success(response);
                });
        }

        $scope.queueReceive = function() {
            $scope.info.isAllowReceive = false;
            tradeService.queueReceive({},
                function(response) {
                    Notification.success('Queue receive successful!');
                    $scope.updateStatus();
                },
                function(response) {
                    $scope.info.isAllowReceive = true;
                    Notification.success(response);
                });
        }

        function getCurrentTransactions() {
            if ($scope.info.state == AccountState.InGiveTransaction) {
                $scope.accountDisplayTemplate = $scope.receiverInfoTemplateUrl;
                $scope.grState = 'giving';
                transactionService.giveRequested({},
                    function(response) {
                        loadTransactions(response);
                    });
            } else if ($scope.info.state == AccountState.InReceiveTransaction) {
                $scope.accountDisplayTemplate = $scope.giverInfoTemplateUrl;
                $scope.grState = 'receiving';
                transactionService.receiveRequested({},
                    function(response) {
                        loadTransactions(response);
                    });
            }
        }

        function loadTransactions(transactions) {
            $scope.histories = [];
            _.each(transactions, generateTransactionData);
            $scope.transactions = transactions;
        }

        function generateTransactionData(item) {
            item.isAllowConfirmGave = item.state == TransactionState.Begin;
            item.isAllowConfirmReceived = item.state == TransactionState.Transfered;

            item.isAllowAbandonTransaction = $scope.info.isAllowAbandonTransaction &&
                item.state == TransactionState.Begin;

            item.isAllowAttachment = item.state != TransactionState.Abandoned;
            item.isAllowUploadAttachment =
                item.state == TransactionState.Begin || item.state == TransactionState.Transfered;

            item.isAbandoned = item.state == TransactionState.Abandoned;

            // history
            generateHistory(item);
        }

        function generateHistory(item) {
            $scope.histories.push({
                userName: "System",
                rating: 6,
                time: item.created,
                action: 'Created',
            });
            var user = '';
            if ($scope.grState == 'giving') {
                user = item.receiverId;
                if (item.receivedDate) {
                    $scope.histories.push({
                        userName: user,
                        rating: item.rating,
                        time: item.receivedDate,
                        action: 'Received',
                    });
                }
            } else {
                user = item.giverId;
                if (item.transferedDate) {
                    $scope.histories.push({
                        userName: user,
                        rating: item.rating,
                        time: item.transferedDate,
                        action: 'Transfered',
                    });
                }
            }
            $scope.histories = _.sortBy($scope.histories, function(item) { return item.time; });

        }

        $scope.moneyTransfered = function(target) {
            $scope.isProcessing = true;
            transactionService.moneyTransfered(target,
                function(response) {
                    Notification.success('Money Transfered');
                    generateTransactionData(response);
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
                    $scope.updateStatus();
                    $scope.isProcessing = false;
                });
        }

        $scope.moneyReceived = function (target) {
            $scope.isProcessing = true;
            transactionService.moneyReceived(target,
                function(response) {
                    Notification.success('Money Received');
                    generateTransactionData(response);
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
                    $scope.updateStatus().then(checkAllTransactionsCompleted);
                    $scope.isProcessing = false;
                });
        }

        function checkAllTransactionsCompleted() {
            var notcompleted = _.filter($scope.transactions,
                function(item) {
                    return item.state != TransactionState.Confirmed;
                });
            if (notcompleted.length == 0) {
                $scope.overviewState.queued = 1;
                $scope.overviewState.give = 1;
                $scope.overviewState.waitCofirm = 1;
                $scope.overviewState.receive = 1;
                $scope.overviewState.ended = 1;
            }
        }

        $scope.reportNotTransfer = function(target) {
            $scope.isProcessing = true;
            transactionService.reportNotTransfer(target,
                function(response) {
                    Notification.success('Transaction Reported');
                    generateTransactionData(response);
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
                    $scope.updateStatus();
                    $scope.isProcessing = false;
                });
        }

        $scope.updateImg = function(target) {
            transactionService.updateImg(target,
                function(response) {
                    Notification.success('Upload saved.');
                });
        }

        $scope.upload = function(target) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/main/transaction/importPopup.html',
                size: 'lg',
                controller: 'importPopupController',
                resolve: {
                    targetData: function() {
                        return { uploadLink: 'api/transaction/upload' };
                    }
                }
            });

            modalInstance.result.then(function(returnData) {
                    Notification.success('Upload successful.');
                    var fileData = $window.StringToXML(returnData);
                    target.attachmentUrl = returnData; //fileData.childNodes[0].innerHTML;
                    target.attachmentUrl = target.attachmentUrl.replace('~/', '');
                    $scope.updateImg(target);
                },
                function() {
                    Notification.success('Upload error.');
                });
        };

        $scope.abandon = function(target) {
            $scope.isProcessing = true;
            transactionService.abandonTransaction(target,
                function(response) {
                    Notification.success('Transaction Abandoned');
                    generateTransactionData(response);
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
                    $scope.updateStatus();
                    $scope.isProcessing = false;
                });
        }


        $scope.init = function() {
            $scope.grState = '';
            $scope.giverInfoTemplateUrl = 'app/main/transaction/info-giver.html';
            $scope.receiverInfoTemplateUrl = 'app/main/transaction/info-receiver.html';
            $scope.TransactionState = TransactionState;
            $scope.AccountState = AccountState;
            $scope.ConfigData = ConfigData;
            $scope.overviewState = {
                queued: 0,
                give: -1,
                waitCofirm: -1,
                receive: -1,
                ended: -1,
            };
            $scope.histories = [];

            $scope.isProcessing = false;

            $scope.updateStatus()
                .then(function(response) {
                    getCurrentTransactions();
                });
        }

        $scope.init();

    }
]);