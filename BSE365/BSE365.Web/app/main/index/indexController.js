'use strict';
mainApp.controller('indexController', ['$scope', '$state', 'authService', 'userService', '$location', '_', function ($scope, $state, authService, userService, $location, _) {
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

    $scope.$on('user:getPinBalance', function (event, data) {
        $scope.userContext.pinBalance = data;
    })

    $scope.$on('user:removePinBalance', function (event, data) {
        if (!isNaN($scope.userContext.pinBalance))
        $scope.userContext.pinBalance -= data;
    })
   
    $scope.getUserContext = function () {
        $scope.userContext = {};

        userService.getCurrentUserContext(function (res) {
            $scope.userContext = res;
        });
    }
    
    var params = $location.search();
    if ($scope.authentication.isAuth && params.anonymous != 'true') {   ////Only load user context when authenticated (even if it's not refreshed)
        $scope.getUserContext();
    }

    $scope.forAdmin = function () {
        return $scope.userContext && _.contains($scope.userContext.roles, 'superadmin');
    }
}]);