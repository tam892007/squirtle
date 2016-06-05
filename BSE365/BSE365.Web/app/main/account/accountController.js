'use strict';
mainApp.controller('accountController',
[
    '$scope', '$state', 'accountService', 'Notification', 'AccountState', 'PriorityLevel', 'UserState', 'ConfigData',
    function($scope, $state, accountService, Notification, AccountState, PriorityLevel, UserState, ConfigData) {

        $scope.loadData = function() {
            $scope.data = [];
            accountService.queryAccount({},
                function(response) {
                    $scope.data = response;
                });
        }

        $scope.viewDefails = function (target) {
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
            $scope.data = [];
            $scope.loadData();
        }

        $scope.init();
    }
]);