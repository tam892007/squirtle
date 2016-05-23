'use strict';
mainApp.controller('userDefaultController', ['$scope', 'userService', function ($scope, userService) {
    $scope.updateCurrentUserProfile = function (profile) {
        return userService.updateCurrentUserProfile(profile).$promise;
    }

    $scope.updateProfile = function () {
        $scope.updateCurrentUserProfile($scope.currentUser).then(function (res) {
            console.log(res);
        });
    }
}]);