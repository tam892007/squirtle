'use strict';
mainApp.controller('userRegisterController', ['$scope', 'userService', function ($scope, userService) {
    $scope.init = function () {
        $scope.newUser = { userInfo: {} };
        $scope.submitted = false;
    }

    $scope.init();

    $scope.registerUser = function () {
        $scope.submitted = true;
        if (!$scope.regForm.$valid) return;
        $scope.newUser.userInfo.parentId = $scope.currentUser.userName;
        userService.register($scope.newUser, function (res) {

        });
    }

    $scope.interacted = function (field) {
        return $scope.submitted || field.$dirty;
    };
}]);