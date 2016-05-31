'use strict';
mainApp.controller('userDefaultController', ['$scope', 'userService', '$uibModal', 'Notification', '$q', function ($scope, userService, $uibModal, Notification, $q) {
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

    $scope.validateBankNumber = function (number) {
        var deferred = $q.defer();

        userService.checkBankNumber({ number: number, userName: $scope.currentUser.userName }, function (res) {
            if (res.result) {
                deferred.resolve(res);
            }
            else {
                deferred.reject(res);
            }
        });

        return deferred.promise;
    }
}]);