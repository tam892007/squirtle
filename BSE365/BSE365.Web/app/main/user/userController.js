'use strict';
mainApp.controller('userController', ['$scope', 'userService', function ($scope, userService) {

    $scope.getCurrentUserProfile = function () {
        return userService.getCurrentUserProfile().$promise;
    }

    $scope.init = function () {
        $scope.currentUser = $scope.currentUser || null;
        ////Get User Profile if needed
        if ($scope.currentUser == null) {
            $scope.getCurrentUserProfile().then(function (res) {
                $scope.currentUser = res;
            });
        }
    }

    $scope.init();
}]);