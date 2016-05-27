'use strict';
mainApp.controller('userRegisterController', ['$scope', 'userService', function ($scope, userService) {
    $scope.init = function () {
        $scope.newUser = { userInfo: {} };
    }

    $scope.init();

    $scope.registerUser = function () {
        $scope.newUser.userInfo.parentId = $scope.currentUser.userName;
        userService.register($scope.newUser, function (res) {
        });
    }
}]);