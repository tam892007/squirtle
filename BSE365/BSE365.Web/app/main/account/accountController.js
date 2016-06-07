'use strict';
mainApp.controller('accountController',
[
    '$scope', '$state', 'accountService', 'Notification', 'AccountState', 'PriorityLevel', 'UserState', 'ConfigData',
    function ($scope, $state, accountService, Notification, AccountState, PriorityLevel, UserState, ConfigData) {

        $scope.loadData = function (tableState) {
            console.log(tableState);
            $scope.data = [];
            accountService.queryAccount(JSON.stringify(tableState),
                function(response) {
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

            $state.go('^.details', { key: target.userName });
        }

        $scope.getAccountState = function(target) {
            var result = (target.UserState == UserState.NotGive || target.UserState == UserState.NotConfirm)
                ? UserState.display(target.userState) + ' (' + target.relatedAccount + ')'
                : AccountState.display(target.state);
            return result;
        }

        $scope.init = function() {
            $scope.AccountState = AccountState;
            $scope.PriorityLevel = PriorityLevel;
            $scope.UserState = UserState;
            $scope.ConfigData = ConfigData;
        }

        $scope.init();
    }
]);