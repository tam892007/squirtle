'use strict';
mainApp.controller('indexController', ['$scope', '$state', 'authService', 'userService', function ($scope, $state, authService, userService) {
    $scope.logOut = function () {
        authService.logOut();
        $state.go('login');
    }
   
    $scope.authentication = authService.authentication;

    $scope.$on('user:authenticated', function (event, data) {
        // you could inspect the data to see if what you care about changed, or just update your own scope
        $scope.getUserContext();        
    });

    $scope.$on('user:updateAvatar', function (event, data) {
        $scope.userContext.avatar.url = data;
    });

   
    $scope.getUserContext = function () {
        userService.getCurrentUserContext(function (res) {
            $scope.userContext = res;
        });
    }

    $scope.getUserContext();
}]);