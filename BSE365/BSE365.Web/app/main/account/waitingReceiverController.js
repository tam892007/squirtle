'use strict';
mainApp.controller('waitingReceiverController',
[
    '$scope', '$state', 'accountService', 'Notification', 'PriorityLevel', 'ConfigData',
    function($scope, $state, accountService, Notification, PriorityLevel, ConfigData) {

        $scope.loadData = function (tableState) {
            $scope.data = [];
            accountService.queryWaitingReceivers(JSON.stringify(tableState),
                function(response) {
                    $scope.data = response.data;
                    tableState.pagination.numberOfPages = Math.ceil(response.totalItems / tableState.pagination.number);
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
            accountService.mapForReceiver($scope.target,
                function(response) {
                    $scope.reload();
                    if (response.amountLeft > 0) {
                        $scope.target.amount = response.amountLeft;
                        Notification.success('Not enough Givers!');
                    } else {
                        $scope.target = {};
                        $scope.selected = false;
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
        }

        $scope.init();
    }
]);