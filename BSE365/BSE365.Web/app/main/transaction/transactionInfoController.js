'use strict';
mainApp.controller('transactionInfoController',
[
    '$scope', 'tradeService', 'Notification', 'AccountState', 'PriorityLevel', 'UserState',
    function($scope, tradeService, Notification, AccountState, PriorityLevel, UserState) {

        $scope.updateStatus = function() {
            tradeService.status({},
                function(response) {
                    $scope.info = response;
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