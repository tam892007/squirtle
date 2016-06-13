'use strict';
mainApp.controller('transactionReportedController',
[
    '$scope', '$state', 'transactionService', 'Notification', 'TransactionState', 'TransactionType', 'ReportResult', 'ConfigData',
    function($scope, $state, transactionService, Notification, TransactionState, TransactionType, ReportResult, ConfigData) {

        $scope.loadData = function (tableState) {
            if (tableState) {
                $scope.tableState = tableState;
            } else {
                tableState = $scope.tableState;
            }
            $scope.data = [];
            transactionService.reportedTransactions(JSON.stringify(tableState),
                function (response) {
                    console.log(response);
                    $scope.data = response.data;
                    tableState.pagination.numberOfPages = Math.ceil(response.totalItems / tableState.pagination.number);
                });
        }

        $scope.reload = function() {
            $scope.target = {};
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

        $scope.giverTrue = function() {
            applyReport(ReportResult.GiverTrue);
        }
        $scope.receiverTrue = function() {
            applyReport(ReportResult.ReceiverTrue);
        }
        $scope.bothTrue = function() {
            applyReport(ReportResult.BothTrue);
        }
        $scope.bothFalse = function() {
            applyReport(ReportResult.BothFalse);
        }

        function applyReport(state) {
            $scope.target.result = state;
            transactionService.applyReport($scope.target,
                function(response) {
                    Notification.success('Transaction Applied.');
                    $scope.reload();
                });
        }
        
        $scope.init = function() {
            $scope.ConfigData = ConfigData;
            $scope.TransactionType = TransactionType;
            $scope.TransactionState = TransactionState;
            $scope.ReportResult = ReportResult;

            $scope.data = [];
            $scope.target = {};
            $scope.selected = false;
        }

        $scope.init();
    }
]);