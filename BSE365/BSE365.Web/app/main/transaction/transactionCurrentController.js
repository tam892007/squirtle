'use strict';
mainApp.controller('transactionCurrentController',
[
    '$scope', '_', '$timeout', 'transactionService', 'tradeService', 'Notification', 'AccountState', 'TransactionState', 'ConfigData',
    function ($scope, _, $timeout, transactionService, tradeService, Notification, AccountState, TransactionState, ConfigData) {

        $scope.updateStatus = function() {
            return tradeService.status({},
                    function(response) {
                        $scope.info = response;
                    })
                .$promise;
        }

        $scope.getCurrentTransactions = function() {
            if ($scope.info.state == AccountState.InGiveTransaction) {
                $scope.accountDisplayTemplate = $scope.receiverInfoTemplateUrl;
                $scope.grState = 'giving';
                transactionService.giveRequested({},
                    function(response) {
                        loadTransaction(response);
                    });
            } else if ($scope.info.state == AccountState.InReceiveTransaction) {
                $scope.accountDisplayTemplate = $scope.giverInfoTemplateUrl;
                $scope.grState = 'receiving';
                transactionService.receiveRequested({},
                    function(response) {
                        loadTransaction(response);
                    });
            }
        }

        function loadTransaction(transactions) {
            var minState = -1;
            _.each(transactions,
                function(item) {
                    item.isBegin = item.state == TransactionState.Begin;
                    item.isAllowConfirmGave = item.state == TransactionState.Begin;
                    item.isAllowConfirmReceived = item.state == TransactionState.Transfered;

                    // overview state
                    if (minState < 0) {
                        if (item.state != TransactionState.Abadoned)
                            minState = item.state;
                    } else if (item.state == TransactionState.Abadoned) {
                        // do nothing
                    } else if (minState < TransactionState.NotTransfer) {
                        if (item.state >= TransactionState.NotTransfer) {
                            minState = item.state;
                        } else {
                            minState = minState > item.state ? item.state : minState;
                        }
                    } else {
                        if (item.state >= TransactionState.NotTransfer) {
                            minState = minState > item.state ? item.state : minState;
                        }
                    }
                    console.log(minState);

                    // history
                    $scope.histories.push({
                        userName: "System - Create",
                        rating: 5,
                        time: item.created,
                        isCompleted: true
                    });
                    var user = '';
                    if ($scope.grState == 'giving') {
                        user = item.receiverId;
                        if (item.receivedDate) {
                            $scope.histories.push({
                                userName: user,
                                rating: item.rating,
                                time: item.receivedDate,
                                isCompleted: true
                            });
                        }
                    } else {
                        user = item.giverId;
                        if (item.transferedDate) {
                            $scope.histories.push({
                                userName: user,
                                rating: item.rating,
                                time: item.transferedDate,
                                isCompleted: true
                            });
                        }
                    }
                    $scope.histories = _.sortBy($scope.histories, function(item) { return item.time; });
                });

            // over view state
            switch (minState) {
            case TransactionState.Begin:
                $scope.overviewState.queued = 1;
                $scope.overviewState.giving = 0;
                $scope.overviewState.gave = -1;
                $scope.overviewState.received = -1;
                $scope.overviewState.ended = -1;
                break;
            case TransactionState.Transfered:
                $scope.overviewState.queued = 1;
                $scope.overviewState.giving = 1;
                $scope.overviewState.gave = 1;
                $scope.overviewState.received = 0;
                $scope.overviewState.ended = -1;
                break;
            case TransactionState.Confirmed:
                $scope.overviewState.queued = 1;
                $scope.overviewState.giving = 1;
                $scope.overviewState.gave = 1;
                $scope.overviewState.received = 1;
                $scope.overviewState.ended = 1;
                break;
            case TransactionState.NotTransfer:
                $scope.overviewState.queued = 1;
                $scope.overviewState.giving = -1;
                $scope.overviewState.gave = -1;
                $scope.overviewState.received = -1;
                $scope.overviewState.ended = 1;
                break;
            case TransactionState.NotConfirm:
                $scope.overviewState.queued = 1;
                $scope.overviewState.giving = 1;
                $scope.overviewState.gave = 1;
                $scope.overviewState.received = -1;
                $scope.overviewState.ended = 1;
                break;
            case TransactionState.ReportedNotTransfer:
                $scope.overviewState.queued = 1;
                $scope.overviewState.giving = 1;
                $scope.overviewState.gave = -1;
                $scope.overviewState.received = -1;
                $scope.overviewState.ended = 0;
                break;
            }
            console.log(minState);
            console.log($scope.overviewState);

            $scope.transactions = transactions;
        }

        $scope.moneyTransfered = function(target) {
            transactionService.moneyTransfered(target,
                function(response) {
                    Notification.success('Money Transfered');
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
                });
        }

        $scope.moneyReceived = function(target) {
            transactionService.moneyReceived(target,
                function(response) {
                    Notification.success('Money Received');
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
                });
        }

        $scope.reportNotTransfer = function(target) {
            transactionService.reportNotTransfer(target,
                function(response) {
                    Notification.success('Transaction Reported');
                    var index = $scope.transactions.indexOf(target);
                    if (index !== -1) {
                        $scope.transactions[index] = response;
                    }
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
                queued: 1,
                giving: 0,
                gave: -1,
                received: -1,
                ended: -1,
            };
            $scope.histories = [];

            $scope.updateStatus()
                .then(function(response) {
                    $scope.getCurrentTransactions();
                });
        }

        $scope.init();

    }
]);