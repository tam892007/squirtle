'use strict';
mainApp.controller('tradeHistoryController',
[
    '$scope', '$uibModal', 'tradeService', 'Notification', 'AccountState', 'ConfigData',
    function($scope, $uibModal, tradeService, Notification, AccountState, ConfigData) {

        $scope.loadData = function() {
            $scope.data = [];
            tradeService.queryHistory({},
                function(response) {
                    $scope.data = response;
                });
        }

        $scope.viewDefails = function(target) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/main/transaction/transactionHistory.html',
                size: 'lg',
                controller: 'transactionHistoryController',
                resolve: {
                    targetData: function() {
                        return target;
                    }
                }
            });

            modalInstance.result.then(function(updatedData) {
                },
                function() {
                });
        }

        $scope.init = function() {
            $scope.AccountState = AccountState;
            $scope.ConfigData = ConfigData;
            $scope.data = [];
            $scope.loadData();
        }

        $scope.init();
    }
]);