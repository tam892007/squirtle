'use strict';
mainApp.controller('pinController', ['$scope', 'userService', 'pinService', function ($scope, userService, pinService) {
    $scope.getCurrentUserPinInfo = function () {
        return userService.getCurrentUserPinInfo().$promise;
    }

    $scope.getCurrentUserPinTransactionHistory = function () {
        return pinService.getCurrentUserHistory().$promise;
    }

    $scope.init = function () {
        $scope.currentPinBalance = {};

        $scope.transactionHistories = [];
        
        $scope.getCurrentUserPinInfo().then(function (res) {
            $scope.currentPinBalance = res;
        });

        $scope.getCurrentUserPinTransactionHistory().then(function (res) {
            $scope.transactionHistories = res;
        });

        $scope.transaction = {};
    }

    $scope.init();

    $scope.transferPIN = function () {
        pinService.transfer($scope.transaction, function (res) {
            ////Reload when save successfully
            $scope.init();
        });
    }
}]);