'use strict';
authApp.controller('refreshController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {

    $scope.authentication = authService.authentication;
    $scope.tokenRefreshed = false;
    $scope.tokenResponse = null;

    $scope.refreshToken = function () {        
        authService.refreshToken().then(function (response) {
            $scope.tokenRefreshed = true;
            $scope.tokenResponse = response;
            console.log(response);
        },
         function (err) {
             $location.path('/login');
         });
    };

    $scope.refreshToken();
}]);