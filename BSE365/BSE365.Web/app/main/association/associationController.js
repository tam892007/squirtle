'use strict';
mainApp.controller('associationController', ['$scope', 'userService', 'authService', '$state', function ($scope, userService, authService, $state) {
    $scope.init = function () {
        $scope.associatedUsrs = [];
        $scope.selectedUsr = {};
    }

    $scope.init();

    $scope.getAssociation = function () {
        userService.getCurrentAssociation(function (res) {
            $scope.associatedUsrs = res;
        });
    }
    
    $scope.getAssociation();

    $scope.switch = function (usr) {
        if ($scope.selectedUsr.id == usr.id) return;
        $scope.password = '';
        $scope.selectedUsr = usr;
    }

    $scope.login = function () {
        var loginData = {
            userName: $scope.selectedUsr.userName,
            password: $scope.selectedUsr.password,
            useRefreshTokens: false
        };

        authService.login(loginData).then(function (response) {
            $state.go('home');
        },
        function (err) {
            $scope.message = "Wrong password!";
        });
    }

    $scope.isMain = function (usr) {
        return usr == null || usr.userName.endsWith('A') || $scope.associatedUsrs.length == 1;
    }
}]);