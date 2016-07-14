mainApp.controller('accountPasswordController',
[
    '$scope', 'accountService', function($scope, accountService) {
        $scope.requestNewPassword = function() {
            $scope.message = '';
            accountService.forceResetPassword({ id: $scope.forgotUserName },
                function(res) {
                    $scope.message = "Reset password successfully!";
                },
                function(err) {
                    console.log(err);
                });
        }

        $scope.changeUserPassword = function() {
            $scope.message = '';
            accountService.forceChangePassword({ userPrefix: $scope.userPrefix, password: $scope.password },
                function(res) {
                    $scope.message = "Change password successfully!";
                },
                function(err) {
                    console.log(err);
                });
        }
    }
]);