'use strict';
mainApp.controller('tradeInfoController',
[
    '$scope', 'tradeService', 'Notification', 'AccountState', 'PriorityLevel', 'UserState',
    function ($scope, tradeService, Notification, AccountState, PriorityLevel, UserState) {

        $scope.updateStatus = function() {
            tradeService.status({},
                function(response) {
                    $scope.info = response;
                });
        }

        $scope.queueGive = function() {
            $scope.info.isAllowGive = false;
            tradeService.queueGive({},
                function(response) {
                    Notification.success('Queue give successful!');
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


        $scope.init = function() {
            $scope.AccountState = AccountState;
            $scope.UserState = UserState;
            $scope.PriorityLevel = PriorityLevel;

            $scope.updateStatus();
        }

        $scope.init();
    }
]);