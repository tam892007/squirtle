'use strict';
mainApp.controller('rateController', ['$scope', '$q', '_', '$http', function ($scope, $q, _, $http) {
    $scope.rate = {};
    $scope.bitPrices = [];

    $scope.init = function () {
        $scope.bitPrices[0] = {};
        $scope.bitPrices[1] = {};
        $scope.bitPrices[2] = {};
        $scope.bitPrices[3] = {};
        $scope.bitPrices[4] = {};
        $scope.bitPrices[5] = {};
        $scope.bitPrices[6] = {};

        $scope.bitPrices[0].quantity1 = 1;
        $scope.bitPrices[0].quantity2 = 50;
        $scope.bitPrices[1].quantity1 = 51;
        $scope.bitPrices[1].quantity2 = 100;
        $scope.bitPrices[2].quantity1 = 101;
        $scope.bitPrices[2].quantity2 = 200;
        $scope.bitPrices[3].quantity1 = 201;
        $scope.bitPrices[3].quantity2 = 300;
        $scope.bitPrices[4].quantity1 = 301;
        $scope.bitPrices[4].quantity2 = 400;
        $scope.bitPrices[5].quantity1 = 401;
        $scope.bitPrices[5].quantity2 = 500;
        $scope.bitPrices[6].quantity1 = 501;
        $scope.bitPrices[6].quantity2 = 1000;

        $scope.bitPrices[0].usd = 9.09;
        $scope.bitPrices[1].usd = 8.18;
        $scope.bitPrices[2].usd = 7.27;
        $scope.bitPrices[3].usd = 6.36;
        $scope.bitPrices[4].usd = 5.45;
        $scope.bitPrices[5].usd = 4.55;
        $scope.bitPrices[6].usd = 3.64;

        $http({ method: 'GET', url: 'http://api.coindesk.com/v1/bpi/currentprice.json', skipInterceptor: true })
          .then(function (res) {
              $scope.rate.usd = res.data.bpi.USD.rate_float;
              for (var i = 0; i < $scope.bitPrices.length; i++) {
                  $scope.bitPrices[i].bitCoin = $scope.bitPrices[i].usd / $scope.rate.usd;
                  $scope.bitPrices[i].totalBitCoin = $scope.bitPrices[i].quantity2 * $scope.bitPrices[i].bitCoin;
                  $scope.bitPrices[i].totalUsd = $scope.bitPrices[i].usd * $scope.bitPrices[i].quantity2;
              }
          });
    }

    $scope.init();
}]);