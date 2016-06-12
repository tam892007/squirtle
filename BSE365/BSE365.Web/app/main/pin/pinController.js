'use strict';
mainApp.controller('pinController', ['$scope', 'userService', 'pinService', '$q', 'Notification', '$state', '$window',
    function ($scope, userService, pinService, $q, Notification, $state, $window) {
    $scope.getCurrentUserPinInfo = function () {
        return userService.getCurrentUserPinInfo().$promise;
    }

    $scope.getCurrentUserPinTransactionHistory = function () {
        return pinService.getCurrentUserHistory().$promise;
    }

    $scope.loadData = function (tableState) {
        $scope.transactionHistories = [];
        pinService.getCurrentUserHistory(JSON.stringify(tableState),
            function (response) {
                $scope.transactionHistories = response.data;
                tableState.pagination.numberOfPages = Math.ceil(response.totalItems / tableState.pagination.number);
            });
    }

    $scope.init = function () {
        $scope.failed = 0;

        $scope.submitted = false;

        $scope.currentPinBalance = {};
        
        $scope.getCurrentUserPinInfo().then(function (res) {
            $scope.currentPinBalance = res;
            $scope.$emit('user:getPinBalance', $scope.currentPinBalance.pinBalance);
        });

        $scope.transaction = { step: 1 };
    }

    $scope.init();

    $scope.processToConfirm = function() {
        $scope.submitted = true;        
        if (!$scope.frmTransfer.$valid) return;
        $scope.transaction.step = 2;
    }

    $scope.goBack = function () {
        $scope.transaction.step = 1;
    }

    $scope.transferPIN = function () {        
        pinService.transfer($scope.transaction, function (res) {
            ////Reload when save successfully
            Notification.success('Transaction has been completed successfully');
            $state.reload();
        },
        function (err) {
            Notification.error('Some errors happenned');
            $scope.failed = 1; ////default
            if (err.data.message == "invalid_captcha") {
                $scope.failed = 2; ////Invalid captcha
                $window.Recaptcha.reload();
            }
        });
    }

    $scope.interacted = function (field) {
        return $scope.submitted || field.$dirty;
    };

    $scope.validateUser = function (userName) {
        var deferred = $q.defer();

        userService.checkName({ name: userName }, function (res) {
            if (res.isSuccessful) {
                $scope.toUser = res.result;
                deferred.resolve(res);
            }
            else {
                $scope.toUser = {};
                deferred.reject(res);
            }
        });

        return deferred.promise;
    }
}]);