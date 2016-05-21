'use strict';
mainApp.controller('pinController', ['$scope', 'userService', 'pinService', function ($scope, userService, pinService) {
    $scope.getCurrentUserPinInfo = function () {
        return userService.getCurrentUserPinInfo().$promise;
    }

    $scope.init = function () {
        $scope.currentPinBalance = {};
        
        $scope.getCurrentUserPinInfo().then(function (res) {
            $scope.currentPinBalance = res;
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