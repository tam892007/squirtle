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

        $scope.reload = function() {
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

        $scope.mapForReceiver = function() {
            var index = $scope.data.indexOf($scope.target);
            accountService.mapForReceiver($scope.target,
                function(response) {
                    console.log(response);
                    if (response.amountLeft > 0) {
                        if (index !== -1) {
                            $scope.data[index] = response;
                            $scope.target = response;
                        }
                        Notification.success('Not enough Givers!');
                    } else {
                        if (index !== -1) {
                            $scope.data.splice(index, 1);
                            $scope.target = {};
                            $scope.selected = false;
                        }
                        Notification.success('Map successful!');
                    }
                });
        }

        $scope.init = function() {
            $scope.PriorityLevel = PriorityLevel;
            $scope.ConfigData = ConfigData;
            $scope.data = [];
            $scope.target = {};
            $scope.selected = false;
            $scope.isGiver = false;
            $scope.isReceiver = true;

            $scope.loadData();
        }

        $scope.init();
    }
]);