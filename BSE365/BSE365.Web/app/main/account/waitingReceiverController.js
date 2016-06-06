'use strict';
mainApp.controller('waitingReceiverController',
[
    '$scope', '$state', 'accountService', 'Notification', 'PriorityLevel', 'ConfigData',
    function($scope, $state, accountService, Notification, PriorityLevel, ConfigData) {

        $scope.loadData = function() {
            $scope.data = [];
            accountService.queryWaitingReceivers({},
                function(response) {
                    $scope.data = response;
                });
        }

        $scope.reload = function () {
            $scope.loadData();
        }

        $scope.viewDefails = function(target) {
            if ($scope.target) {
                $scope.target.selected = false;
            }
            target.selected = true;

            $scope.target = target;
            $scope.selected = true;
        }

        $scope.init = function() {
            $scope.PriorityLevel = PriorityLevel;
            $scope.ConfigData = ConfigData;
            $scope.data = [];
            $scope.target = {};
            $scope.selected = false;

            $scope.loadData();
        }

        $scope.init();
    }
]);