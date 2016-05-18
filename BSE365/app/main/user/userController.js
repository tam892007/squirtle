'use strict';
mainApp.controller('userController', ['$scope', 'userService', function ($scope, userService) {

    $scope.getCurrentUserProfile = function () {
        return userService.getCurrentUserProfile().$promise;
    }

    $scope.init = function () {
        $scope.user = {};
        $scope.formMode = 0; ////Display Mode 

        ////Get User Profile
        $scope.getCurrentUserProfile().then(function (res) {
            $scope.user = res;
            console.log(res);
        });
    }

    $scope.init();
}]);