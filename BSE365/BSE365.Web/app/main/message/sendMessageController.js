'use strict';
mainApp.controller('sendMessageController', ['$scope', 'userService', 'messageService', '$q', 'Notification', '$state', function ($scope, userService, messageService, $q, Notification, $state) {
    $scope.getCurrentUserPinInfo = function () {
        return userService.getCurrentUserPinInfo().$promise;
    }

    $scope.init = function () {
        $scope.failed = 0;

        $scope.submitted = false;

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
        $scope.submitted = true;
        if (!$scope.frmTransfer.$valid) return;

        transactionService.transfer($scope.transaction, function (res) {
            ////Reload when save successfully
            Notification.success('Transaction has been completed successfully');
            $state.reload();
        },
        function (err) {
            $scope.failed = 1; ////default

            if (err.data.message == "invalid_captcha") {
                $scope.failed = 2; ////Invalid captcha
            }
        });
    }

    $scope.interacted = function (field) {
        return $scope.submitted || field.$dirty;
    };

    $scope.validateUser = function (userName) {
        console.log(userName);
        var deferred = $q.defer();

        userService.checkName({ name: userName }, function (res) {
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