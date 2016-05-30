'use strict';
mainApp.controller('tradeCurrentController',
[
    '$scope', 'transactionService', 'tradeService', 'Notification', 'TransactionState',
    function($scope, transactionService, tradeService, Notification, TransactionState) {

        $scope.updateStatus = function() {
            return tradeService.status({},
                function (response) {
                    console.log("2");
                    $scope.info = response;
                }).$promise;
        }

        $scope.getCurrentTransactions = function() {
            
        }

        $scope.init = function() {
            $scope.TransactionState = TransactionState;
            $scope.updateStatus().then(function (response) {
                console.log("2");
            });
            $scope.getCurrentTransactions();
        }

        $scope.init();

    }
]);