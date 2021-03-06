﻿'use strict';
mainApp.controller('userRegisterController', ['$scope', 'userService', 'Notification', '$state', '$q', function ($scope, userService, Notification, $state, $q) {
    $scope.init = function () {
        $scope.newUser = { userInfo: {} };
        $scope.submitted = false;
        $scope.step = 1;
    }

    $scope.init();

    $scope.registerUser = function () {
        $scope.submitted = true;
        if (!$scope.regForm.$valid) return;
        $scope.newUser.userInfo.parentId = $scope.currentUser.userName;
        userService.register($scope.newUser, function (res) {
            $scope.users = res;
            Notification.success('Register successfully');
            $scope.step = 2;
        }, function (err) {
            Notification.error('Register failed');
        });
    }

    $scope.endRegister = function () {
        $state.reload();
    }

    $scope.interacted = function (field) {
        return $scope.submitted || field.$dirty;
    };

    $scope.validateBankNumber = function (number) {
        var deferred = $q.defer();

        userService.checkBankNumber({ number: number, userName: '*' }, function (res) {
            if (res.result) {
                deferred.resolve(res);
            }
            else {
                deferred.reject(res);
            }
        });

        return deferred.promise;
    }

    $scope.canIntroduce = function () {        
        return $scope.currentUser == null || $scope.currentUser.userName.endsWith('A') || !$scope.currentUser.parentId;
    }
}]);