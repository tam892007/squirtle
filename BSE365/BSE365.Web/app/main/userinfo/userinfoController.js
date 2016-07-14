'use strict';
mainApp.controller('userinfoController',
[
    '$scope', '$state', '$uibModal', '$q', 'Notification', 'userinfoService',
    'AccountState', 'TransactionState', 'TransactionType', 'PriorityLevel', 'UserState',
    'ConfigData',
    function($scope,
        $state,
        $uibModal,
        $q,
        Notification,
        userinfoService,
        AccountState,
        TransactionState,
        TransactionType,
        PriorityLevel,
        UserState,
        ConfigData) {

        $scope.loadData = function(tableState) {
            if (!$scope.firstLoad) {
                $scope.firstLoad = true;
                $scope.tableState = tableState;
                return;
            }

            if (tableState) {
                $scope.tableState = tableState;
            } else {
                tableState = $scope.tableState;
            }
            $scope.data = [];
            userinfoService.queryInfo(JSON.stringify(tableState),
                function(response) {
                    $scope.data = response.data;
                    tableState.pagination.numberOfPages = Math.ceil(response.totalItems / tableState.pagination.number);
                });
        }

        $scope.reload = function() {
            $scope.loadData();
        }

        $scope.toggleView = function(value) {
            if (value == null) {
                $scope.showUserInfo = !$scope.showUserInfo;
            } else {
                $scope.showUserInfo = value;
            }
        }

        $scope.viewDefails = function(target) {
            if ($scope.info) {
                $scope.info.selected = false;
            }
            target.selected = true;
            $scope.toggleView(true);

            $scope.info = target;

            $scope.showTransactions = false;
            $scope.showWaitings = false;
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

        $scope.loadTransactions = function() {
            $scope.showTransactions = true;
            $scope.reloadTransaction();
        }

        $scope.loadWaitings = function() {
            userinfoService.waitingInfomations({ userPrefix: $scope.info.userPrefix },
                { userPrefix: $scope.info.userPrefix },
                function(response) {
                    $scope.info.giverData = response.giverData;
                    $scope.info.receiverData = response.receiverData;
                });
            $scope.showWaitings = true;
        }


        $scope.loadTransactionData = function(tableState) {

            if (!$scope.info.firstLoad) {
                $scope.info.firstLoad = true;
                $scope.info.tableState = tableState;
                return;
            }

            if (tableState) {
                $scope.info.tableState = tableState;
            } else {
                tableState = $scope.info.tableState;
            }
            tableState.search = tableState.search || {};
            tableState.search.predicateObject = tableState.search.predicateObject || {};
            tableState.search.predicateObject.userPrefix = $scope.info.userPrefix;
            $scope.info.transactionData = [];
            userinfoService.queryTransaction(JSON.stringify(tableState),
                function(response) {
                    $scope.info.transactionData = response.data;
                    tableState.pagination.numberOfPages = Math.ceil(response.totalItems / tableState.pagination.number);
                });
        }

        $scope.reloadTransaction = function() {
            $scope.info.targetTransaction = {};
            $scope.info.transactionSelected = false;
            $scope.loadTransactionData();
        }

        $scope.viewTransactionDefails = function(target) {
            if ($scope.info.targetTransaction) {
                $scope.info.targetTransaction.selected = false;
            }
            target.selected = true;

            $scope.info.targetTransaction = target;
            $scope.info.transactionSelected = true;
        }

        $scope.update = function() {
            userinfoService.updateUserInfo($scope.info, function(response) { console.log(response); });
        }

        $scope.validateBankNumber = function(number) {
            var deferred = $q.defer();

            userinfoService.checkBankNumber({ number: number, userid: $scope.info.id },
                function(res) {
                    if (res) {
                        deferred.resolve(res);
                    } else {
                        deferred.reject(res);
                    }
                });

            return deferred.promise;
        }

        $scope.init = function() {
            $scope.AccountState = AccountState;
            $scope.PriorityLevel = PriorityLevel;
            $scope.UserState = UserState;
            $scope.ConfigData = ConfigData;
            $scope.TransactionState = TransactionState;
            $scope.TransactionType = TransactionType;

            $scope.data = [];
            $scope.info = null;

            $scope.showUserInfo = false;
            $scope.showTransactions = false;
            $scope.showWaitings = false;
        }

        $scope.init();
    }
]);