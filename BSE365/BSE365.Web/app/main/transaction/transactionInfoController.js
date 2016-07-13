'use strict';
mainApp.controller('transactionInfoController',
[
    '$scope', '$uibModal', 'tradeService', 'Notification', 'AccountState', 'PriorityLevel', 'UserState',
    function($scope, $uibModal, tradeService, Notification, AccountState, PriorityLevel, UserState) {

        $scope.updateStatus = function() {
            tradeService.status({},
                function(response) {
                    $scope.info = response;
                });
        }

        $scope.viewParentInfo = function(parentId) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/main/account/account-info-parent.html',
                size: 'lg',
                controller: 'accountInfoParentController',
                resolve: {
                    targetData: function() {
                        return { parentId: parentId };
                    }
                },
            });
        }

        $scope.init = function() {
            $scope.AccountState = AccountState;
            $scope.UserState = UserState;
            $scope.PriorityLevel = PriorityLevel;

            $scope.updateStatus();
        }

        $scope.init();
    }
]);