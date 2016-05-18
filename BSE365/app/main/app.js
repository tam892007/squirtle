var mainApp = angular.module('mainApp', ['ui.router', 'authApp', 'ngResource']);

mainApp.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {

    $urlRouterProvider.otherwise("/")

    $stateProvider
        .state('home', {
                url: "/",
                templateUrl: 'app/main/home/home.html',
            })
        .state('login', {
            url: "/login",
            templateUrl: 'app/authentication/login/login.html',
            controller: 'loginController'
            })
        .state('pin', {
            url: "/pin",
            templateUrl: 'app/main/pin/pin.html',
            controller: 'pinController'
        })
        .state('user', {
            url: "/user",
            templateUrl: 'app/main/user/user-info.html',
            controller: 'userController'
        })
}]);

mainApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
}]);

mainApp.run(['authService', function (authService) {
    authService.fillAuthData();
}])