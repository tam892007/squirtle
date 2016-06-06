'use strict';
mainApp.controller('accountInfoController',
[
    '$scope', '$stateParams', 'accountService', 'Notification', 'AccountState', 'PriorityLevel', 'UserState',
    function($scope, $stateParams, accountService, Notification, AccountState, PriorityLevel, UserState) {

        $scope.getData = function() {
            accountService.status({ key: $stateParams.key },
                { key: $stateParams.key },
                function(response) {
                    $scope.info = response;
                    $scope.info.newPriority = $scope.info.priority;
                    $scope.info.newState = $scope.info.state;
                });
        }

        $scope.queueGive = function() {
            $scope.info.isAllowGive = false;
            accountService.queueGive({},
                function(response) {
                    Notification.success('Queue give successful!');
                    $scope.updateStatus();
                },
                function(response) {
                    $scope.info.isAllowGive = true;
                    Notification.success(response);
                });
        }

        $scope.queueReceive = function() {
            $scope.info.isAllowReceive = false;
            accountService.queueReceive({},
                function(response) {
                    Notification.success('Queue receive successful!');
                    $scope.updateStatus();
                },
                function(response) {
                    $scope.info.isAllowReceive = true;
                    Notification.success(response);
                });
        }

        $scope.setPriority = function() {
            accountService.setAccountPriority({
                    userName: $scope.info.userName,
                    priority: $scope.info.newPriority
                },
                function(response) {
                    Notification.success('Priority Changed!');
                });
        }

        $scope.setAccountState = function() {
            accountService.setAccountState({
                    userName: $scope.info.userName,
                    state: $scope.info.newState
                },
                function(response) {
                    Notification.success('State Changed!');
                    $scope.getData();
                });
        }

        $scope.init = function() {
            $scope.UserState = UserState;
            $scope.AccountState = AccountState;

            $scope.accountStateToSelect = [
                {
                    value: AccountState.Default,
                    text: AccountState.display(AccountState.Default),
                },
                {
                    value: AccountState.Gave,
                    text: AccountState.display(AccountState.Gave),
                },
            ];
            $scope.PriorityLevel = PriorityLevel;
            $scope.priorityToSelect = [
                {
                    value: PriorityLevel.Default,
                    text: PriorityLevel.display(PriorityLevel.Default),
                },
                {
                    value: PriorityLevel.Priority,
                    text: PriorityLevel.display(PriorityLevel.Priority),
                },
                {
                    value: PriorityLevel.High,
                    text: PriorityLevel.display(PriorityLevel.High),
                },
                {
                    value: PriorityLevel.Highest,
                    text: PriorityLevel.display(PriorityLevel.Highest),
                },
            ];

            $scope.getData();
        }

        $scope.init();
    }
]);