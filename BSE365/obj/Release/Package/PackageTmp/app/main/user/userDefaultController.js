'use strict';
mainApp.controller('userDefaultController', ['$scope', 'userService', 'FileUploader', function ($scope, userService, FileUploader) {
    $scope.updateCurrentUserProfile = function (profile) {
        return userService.updateCurrentUserProfile(profile).$promise;
    }

    $scope.updateProfile = function () {
        $scope.updateCurrentUserProfile($scope.currentUser).then(function (res) {

        });
    }
}]);