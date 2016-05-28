'use strict';
mainApp.controller('userRegisterController', ['$scope', 'userService', 'Notification', function ($scope, userService, Notification) {
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
            Notification.success('Register successfully');
            $scope.init();
        });
    }

    $scope.interacted = function (field) {
        return $scope.submitted || field.$dirty;
    };
}]);