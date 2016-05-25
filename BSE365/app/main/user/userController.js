'use strict';
mainApp.controller('userController', ['$scope', 'userService', 'imageService', function ($scope, userService, imageService) {

    $scope.getCurrentUserProfile = function () {
        return userService.getCurrentUserProfile().$promise;
    }

    $scope.init = function () {
        $scope.currentUser = $scope.currentUser || null;
        ////Get User Profile if needed
        if ($scope.currentUser == null) {
            $scope.getCurrentUserProfile().then(function (res) {
                $scope.currentUser = res;                
                $scope.currentUser.avatar.url = '/image/getUserPicture/' + $scope.currentUser.avatar.id;
            });
        }
    }

    $scope.init();
}]);