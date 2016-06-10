'use strict';
mainApp.controller('tradePunishmentController',
[
    '$scope', '$uibModal', 'tradeService', 'Notification', 'AccountState', 'ConfigData',
    function($scope, $uibModal, tradeService, Notification, AccountState, ConfigData) {

        $scope.loadData = function() {
            $scope.data = [];
            tradeService.queryPunishment({},
                function(response) {
                    $scope.data = response;
                });
        }

        $scope.viewDefails = function(target) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/main/transaction/transactionHistory.html',
                size: 'lg',
                controller: 'transactionPunishmentController',
                resolve: {
                    targetData: function() {
                        return target;
                    }
                }
            });

            modalInstance.result.then(function (updatedData) {
                $scope.loadData();
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