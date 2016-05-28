'use strict';
mainApp.controller('userDefaultController', ['$scope', 'userService', '$uibModal', 'Notification', function ($scope, userService, $uibModal, Notification) {
    $scope.updateCurrentUserProfile = function (profile) {
        return userService.updateCurrentUserProfile(profile).$promise;
    }

    $scope.updateProfile = function () {
        if (!$scope.userForm.$valid) return;

        $scope.updateCurrentUserProfile($scope.currentUser).then(function (res) {
            Notification.success('Update successfully');
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