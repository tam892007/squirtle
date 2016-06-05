'use strict';
mainApp.controller('accountInfoController',
[
    '$scope', '$stateParams', 'accountService', 'Notification', 'AccountState', 'PriorityLevel',
    function($scope, $stateParams, accountService, Notification, AccountState, PriorityLevel) {

        $scope.getData = function() {
            accountService.status({ key: $stateParams.key }, { key: $stateParams.key },
                function (response) {
                    $scope.info = response;
                });
        }

        $scope.queueGive = function() {
            info.isAllowGive = false;
            accountService.queueGive({},
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
            accountService.queueReceive({},
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
            $scope.AccountState = AccountState;
            $scope.PriorityLevel = PriorityLevel;

            $scope.getData();
        }

        $scope.init();
    }
]);