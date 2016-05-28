'use strict';
mainApp.controller('userDefaultController', ['$scope', 'userService', '$uibModal', function ($scope, userService, $uibModal) {
    $scope.updateCurrentUserProfile = function (profile) {
        return userService.updateCurrentUserProfile(profile).$promise;
    }

    $scope.updateProfile = function () {
        if (!$scope.userForm.$valid) return;

        $scope.updateCurrentUserProfile($scope.currentUser).then(function (res) {

        });
    }

    $scope.changePassword = function () {
        var modalInstance = $uibModal.open({
            animation: false,
            templateUrl: 'app/main/user/dlg-change-password.html',
            controller: 'dlgChangePasswordController',
            size: 'md',
            windowClass: 'passswordDialog',
        });

        modalInstance.result.then(function (res) {
        }, function () {
        });
    }
}]);