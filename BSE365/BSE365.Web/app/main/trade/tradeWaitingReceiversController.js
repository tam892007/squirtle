'use strict';
mainApp.controller('tradeWaitingReceiversController',
[
    '$scope', 'tradeService', 'Notification', ,
    function ($scope, tradeService, Notification) {

        $scope.loadData = function () {
            $scope.data = [];
            tradeService.queryWaitingReceivers({},
                function (response) {
                    $scope.data = response;
                });
        }

        $scope.init = function () {
            $scope.data = [];
            $scope.loadData();
        }

        $scope.init();
    }
]);