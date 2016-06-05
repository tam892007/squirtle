authApp.controller('resetPasswordController', ['$scope', '$location', 'authService', '$state', function ($scope, $location, authService, $state) {
    $scope.init = function () {
        var params = $location.search();
        $scope.message = "";        
        $scope.resetData = {
            userName: params.name,
            code: params.code,
        };

        $scope.submitted = false;

        $scope.step = 1;
    }

    $scope.init();

    $scope.reset = function () {
        $scope.submitted = true;
        if (!$scope.frmReset.$valid) return;

        authService.resetPassword($scope.resetData).then(function (response) {
            $scope.message = "Your password has been updated."
            $scope.step = 2;
        },
        function (err) {
            $scope.step = 2;
            $scope.message = "The reset link is out of date. Please request a new one.";            
        });
    }

    $scope.backToLogin = function () {
        $state.go('login');
    }

    $scope.interacted = function (field) {
        return $scope.submitted || field.$dirty;
    };
}]);