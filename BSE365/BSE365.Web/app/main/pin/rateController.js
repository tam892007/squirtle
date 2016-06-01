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

        $scope.bitPrices[0].bitCoin = 0.0085172497707;
        $scope.bitPrices[1].bitCoin = 0.0076561651785;
        $scope.bitPrices[2].bitCoin = 0.0068137998166;
        $scope.bitPrices[3].bitCoin = 0.0059527152243;
        $scope.bitPrices[4].bitCoin = 0.0051103498624;
        $scope.bitPrices[5].bitCoin = 0.0052788229348;
        $scope.bitPrices[6].bitCoin = 0.0034068999083;
        
        $http({ method: 'GET', url: 'http://api.coindesk.com/v1/bpi/currentprice.json', skipInterceptor: true })
          .then(function (res) {             
              $scope.rate.usd = res.data.bpi.USD.rate_float;
              for (var i = 0; i < $scope.bitPrices.length; i++) {
                  $scope.bitPrices[i].usd = $scope.rate.usd * $scope.bitPrices[i].bitCoin;
                  $scope.bitPrices[i].totalBitCoin = $scope.bitPrices[i].quantity2 * $scope.bitPrices[i].bitCoin;
                  $scope.bitPrices[i].totalUsd = $scope.rate.usd * $scope.bitPrices[i].totalBitCoin;
              }
          });
    }

    $scope.init();

    $scope.initBitPrices = function() {
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

        $scope.bitPrices[0].bitCoin = 0.0085172497707;
        $scope.bitPrices[1].bitCoin = 0.0076561651785;
        $scope.bitPrices[2].bitCoin = 0.0068137998166;
        $scope.bitPrices[3].bitCoin = 0.0059527152243;
        $scope.bitPrices[4].bitCoin = 0.0051103498624;
        $scope.bitPrices[5].bitCoin = 0.0052788229348;
        $scope.bitPrices[6].bitCoin = 0.0034068999083;
    }
}]);