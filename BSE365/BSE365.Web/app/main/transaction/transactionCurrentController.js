'use strict';
mainApp.controller('transactionCurrentController',
[
    '$scope', '_', 'transactionService', 'tradeService', 'Notification', 'AccountState', 'TransactionState',
    function($scope, _, transactionService, tradeService, Notification, AccountState, TransactionState) {

        $scope.updateStatus = function() {
            return tradeService.status({},
                    function(response) {
                        $scope.info = response;
                    })
                .$promise;
        }

        $scope.getCurrentTransactions = function() {
            if ($scope.info.state == AccountState.InGiveTransaction) {
                transactionService.giveRequested({},
                    function(response) {
                        loadTransaction(response);
                    });
                $scope.accountDisplayTemplate = $scope.receiverInfoTemplateUrl;
            } else if ($scope.info.state == AccountState.InReceiveTransaction) {
                transactionService.receiveRequested({},
                    function(response) {
                        loadTransaction(response);
                    });
                $scope.accountDisplayTemplate = $scope.giverInfoTemplateUrl;
            }
        }

        function loadTransaction(transactions) {
            _.each(transactions,
                function(item) {
                    item.isBegin = item.state == TransactionState.Begin;
                    item.isAllowConfirmGave = item.state == TransactionState.Begin;
                    item.isAllowConfirmReceived = item.state == TransactionState.Transfered;
                    console.log(item);
                });
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
            $scope.giverInfoTemplateUrl = 'app/main/transaction/info-giver.html';
            $scope.receiverInfoTemplateUrl = 'app/main/transaction/info-receiver.html';
            $scope.TransactionState = TransactionState;
            $scope.AccountState = AccountState;
            $scope.updateStatus()
                .then(function(response) {
                    $scope.getCurrentTransactions();
                });
        }

        $scope.init();

    }
]);