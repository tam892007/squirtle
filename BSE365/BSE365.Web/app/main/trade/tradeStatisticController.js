'use strict';
mainApp.controller('tradeStatisticController',
[
    '$scope', 'tradeService', 'Notification', 'AccountState', 'PriorityLevel',
    function($scope, tradeService, Notification, AccountState, PriorityLevel) {

        $scope.updateStatus = function() {
            tradeService.status({},
                function(response) {
                    $scope.info = response;
                });
        }

        $scope.queueGive = function() {
            info.isAllowGive = false;
            tradeService.queueGive({},
                function(response) {
                    Notification.success('Queue give successful!');
                    $scope.updateStatus();
                },
                function(response) {
                    info.isAllowGive = true;
                    Notification.success(response);
                });
        }

        $scope.queueReceive = function() {
            info.isAllowReceive = false;
            tradeService.queueReceive({},
                function(response) {
                    Notification.success('Queue receive successful!');
                    $scope.updateStatus();
                },
                function(response) {
                    info.isAllowReceive = true;
                    Notification.success(response);
                });
        }


        $scope.init = function() {
            Notification.success('Hello');

            $scope.accountStates = AccountState;
            $scope.priorityLevels = PriorityLevel;

            $scope.updateStatus();
        }

        $scope.init();
    }
]);