﻿'use strict';
mainApp.controller('transactionInfoHistoryController',
[
    '$scope', '$state', 'transactionService', 'Notification', 'TransactionType', 'TransactionState', 'ConfigData',
    function($scope, $state, transactionService, Notification, TransactionType, TransactionState, ConfigData) {

        $scope.loadData = function(tableState) {
            if (tableState) {
                $scope.tableState = tableState;
            } else {
                tableState = $scope.tableState;
            }
            $scope.data = [];
            transactionService.queryUserHistory(JSON.stringify(tableState),
                function(response) {
                    console.log(response);
                    $scope.data = response.data;
                    tableState.pagination.numberOfPages = Math.ceil(response.totalItems / tableState.pagination.number);
                });
        }

        $scope.reload = function() {
            $scope.loadData();
        }

        $scope.viewDefails = function(target) {
            if ($scope.current) {
                $scope.current.selected = false;
            }
            target.selected = true;
            $scope.current = target;

            $state.go('.details', { key: target.id });
        }

        $scope.init = function() {
            $scope.ConfigData = ConfigData;
            $scope.TransactionType = TransactionType;
            $scope.TransactionState = TransactionState;

            $scope.data = [];
        }

        $scope.init();
    }
]);