'use strict';
mainApp.controller('rateController', ['$scope', '$q', '_', '$http', function ($scope, $q, _, $http) {
    $scope.init = function () {
        $scope.rate = {};
        $http({ method: 'GET', url: 'http://api.coindesk.com/v1/bpi/currentprice.json', skipInterceptor: true })
          .then(function (res) {             
              $scope.rate.usd = res.data.bpi.USD.rate_float;
          });
    }

    $scope.init();
}]);