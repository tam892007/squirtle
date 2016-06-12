'use strict';
mainApp.controller('waitingGiverController',
[
    '$scope', '$state', 'accountService', 'Notification', 'PriorityLevel', 'WaitingType', 'ConfigData',
    function ($scope, $state, accountService, Notification, PriorityLevel, WaitingType, ConfigData) {

        $scope.loadData = function (tableState) {
            if (tableState) {
                $scope.tableState = tableState;
            } else {
                tableState = $scope.tableState;
            }
            $scope.data = [];
            accountService.queryWaitingGivers(JSON.stringify(tableState),
                function(response) {
                    $scope.data = response.data;
                    tableState.pagination.numberOfPages = Math.ceil(response.totalItems / tableState.pagination.number);
                });
        }

        $scope.reload = function () {
            $scope.loadData();
        }

        $scope.viewDefails = function (target) {
            if ($scope.target) {
                $scope.target.selected = false;
            }
            target.selected = true;
            $scope.target = target;
            $scope.selected = true;
        }

        $scope.init = function () {
            $scope.PriorityLevel = PriorityLevel;
            $scope.ConfigData = ConfigData;
            $scope.WaitingType = WaitingType;
            $scope.data = [];
            $scope.target = {};
            $scope.selected = false;
            $scope.isGiver = true;
            $scope.isReceiver = false;
        }

        $scope.init();
    }
]);