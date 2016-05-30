'use strict';
mainApp.controller('rateController', ['$scope', 'userService', '$q', 'Notification', '$state', '$http', function ($scope, userService, $q, Notification, $state, $http) {
    $scope.init = function () {
        //$http.get("https://bitpay.com/api/rates")
        //  .success(function (data) {
        //      console.log(data);
        //      $scope.rates = data;
        //      for (var i = 0; i < data.length; i++) {
        //          if (data[i].code == "USD") {
        //              $scope.currRate = data[i].rate;
        //          }
        //      }
        //  });
        $.getJSON('https://bitpay.com/api/rates', function (data) {
            $scope.rates = data;
            for (var i = 0; i < data.length; i++) {
                if (data[i].code == "USD") {
                    $scope.currRate = data[i].rate;
                }
            }
        });
    }

    $scope.init();

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