'use strict';
mainApp.controller('tradeBonusController',
[
    '$scope', '$uibModal', 'tradeService', 'Notification', 'AccountState', 'ConfigData',
    function($scope, $uibModal, tradeService, Notification, AccountState, ConfigData) {

        $scope.updateStatus = function() {
            tradeService.status({},
                function(response) {
                    $scope.info = response;
                });
        }

        $scope.loadData = function() {
            $scope.data = [];
            tradeService.queryBonus({},
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

            modalInstance.result.then(function(updatedData) {
                $scope.loadData();
            });
        }

        $scope.claimBonus = function() {
            $scope.info.isAllowClaimBonus = false;
            tradeService.claimBonus({ key: $scope.info.userName },
                { key: $scope.info.userName },
                function(response) {
                    Notification.success('Claim bonus successful!');
                    $scope.updateStatus();
                },
                function(response) {
                    $scope.info.isAllowClaimBonus = true;
                    Notification.success(response);
                });
        }

        $scope.init = function() {
            $scope.AccountState = AccountState;
            $scope.ConfigData = ConfigData;
            $scope.data = [];
            $scope.loadData();

            $scope.updateStatus();
        }

        $scope.init();
    }
]);