'use strict';
mainApp.controller('tradeWaitingGiversController',
[
    '$scope', 'tradeService', 'Notification',
    function($scope, tradeService, Notification) {

        $scope.loadData = function() {
            $scope.data = [];
            tradeService.queryWaitingGivers({},
                function(response) {
                    $scope.data = response;
                });
        }

        $scope.init = function() {
            $scope.data = [];
            $scope.loadData();
        }

        $scope.init();
    }
]);