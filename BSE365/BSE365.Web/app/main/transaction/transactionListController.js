'use strict';
mainApp.controller('transactionListController',
[
    '$scope', '$uibModal', '$state', 'transactionService', 'Notification', 'TransactionType', 'TransactionState',
    'ConfigData',
    function($scope,
        $uibModal,
        $state,
        transactionService,
        Notification,
        TransactionType,
        TransactionState,
        ConfigData) {

        $scope.loadData = function(tableState) {
            if (tableState) {
                $scope.tableState = tableState;
            } else {
                tableState = $scope.tableState;
            }
            $scope.data = [];
            transactionService.queryTransaction(JSON.stringify(tableState),
                function(response) {
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

        $scope.viewParentInfo = function(parentId) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/main/account/account-info-parent.html',
                size: 'lg',
                controller: 'accountInfoParentController',
                resolve: {
                    targetData: function() {
                        return { parentId: parentId };
                    }
                },
            });
        }

        $scope.init = function() {
            $scope.ConfigData = ConfigData;
            $scope.TransactionType = TransactionType;
            $scope.TransactionState = TransactionState;

            $scope.data = [];
            $scope.target = {};
            $scope.selected = false;
        }

        $scope.init();
    }
]);