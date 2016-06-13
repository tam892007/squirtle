'use strict';
mainApp.controller('transactionHistoryController',
[
    '$scope', '_', '$timeout', '$uibModal', '$uibModalInstance', 'transactionService', 'tradeService', 'Notification',
    'ConfigData',
    'AccountState', 'TransactionState', 'targetData',
    function($scope,
        _,
        $timeout,
        $uibModal,
        $uibModalInstance,
        transactionService,
        tradeService,
        Notification,
        ConfigData,
        AccountState,
        TransactionState,
        targetData) {

        $scope.getCurrentTransactions = function() {
            if ($scope.targetData.type == AccountState.InGiveTransaction) {
                $scope.accountDisplayTemplate = $scope.receiverInfoTemplateUrl;
                $scope.grState = 'giving';
            } else if ($scope.targetData.type == AccountState.InReceiveTransaction) {
                $scope.accountDisplayTemplate = $scope.giverInfoTemplateUrl;
                $scope.grState = 'receiving';
            }
            transactionService.history(targetData,
                function(response) {
                    loadTransaction(response);
                });
        }

        function loadTransaction(transactions) {
            $scope.histories = [];
            _.each(transactions,
                function(item) {
                    item.isAllowConfirmGave = item.state == TransactionState.Begin;
                    item.isAllowConfirmReceived = item.state == TransactionState.Transfered;

                    item.isAllowAbadonTransaction = false;

                    item.isAllowAttachment = item.state != TransactionState.Abadoned;
                    item.isAllowUploadAttachment =
                        item.state == TransactionState.Begin || item.state == TransactionState.Transfered;

                    item.isAbadoned = item.state == TransactionState.Abadoned;

                    // history
                    generateHistory(item);
                });

            $scope.transactions = transactions;
            generateOverviewStateOverTransactions();
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

        function generateOverviewStateOverTransactions() {
            var allEnded = _.every($scope.transactions,
                function(item) { return item.state == TransactionState.Confirmed; });
            if ($scope.grState == 'giving') {
                $scope.overviewState.queued = 1;
                if (allEnded) {
                    $scope.overviewState.give = 1;
                    $scope.overviewState.waitCofirm = 1;
                } else {
                    $scope.overviewState.give = 0;
                }
            } else if ($scope.grState == 'receiving') {
                $scope.overviewState.queued = 1;
                $scope.overviewState.give = 1;
                $scope.overviewState.waitCofirm = 1;
                if (allEnded) {
                    $scope.overviewState.receive = 1;
                    $scope.overviewState.ended = 1;
                } else {
                    $scope.overviewState.receive = 0;
                }

            }
            console.log($scope.overviewState);
        }

        $scope.moneyTransfered = function(target) {
            transactionService.moneyTransfered(target,
                function(response) {
                    Notification.success('Money Transfered');
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
                    $scope.updateStatus();
                    getCurrentTransactions();
                });
        }

        $scope.moneyReceived = function(target) {
            $scope.isProcessing = true;
            transactionService.moneyReceived(target,
                function(response) {
                    Notification.success('Money Received');
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
                    $scope.updateStatus();
                    getCurrentTransactions();
                    $scope.isProcessing = false;
                });
        }

        $scope.reportNotTransfer = function(target) {
            $scope.isProcessing = true;
            transactionService.reportNotTransfer(target,
                function(response) {
                    Notification.success('Transaction Reported');
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
                    $scope.updateStatus();
                    getCurrentTransactions();
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
                    target.attachmentUrl = returnData;
                    $scope.updateImg(target);
                },
                function() {
                    Notification.success('Upload error.');
                });
        };

        $scope.abadon = function(target) {
            $scope.isProcessing = true;
            transactionService.abadonTransaction(target,
                function(response) {
                    Notification.success('Transaction Abadoned');
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
                    $scope.updateStatus();
                    getCurrentTransactions();
                    $scope.isProcessing = false;
                });
        }


        $scope.init = function() {
            $scope.targetData = targetData;
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

            $scope.getCurrentTransactions();
        }

        $scope.init();

    }
]);