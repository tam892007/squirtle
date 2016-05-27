var mainApp = angular.module('mainApp', ['ui.router', 'ui.bootstrap', 'authApp', 'ngResource', 'ui.tree', 'smart-table', 'underscore', 'angularFileUpload', 'ngImgCrop', 'ngMessages']);

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
            abstract: true,
            url: "/user",
            templateUrl: 'app/main/user/user-info.html',
            controller: 'userController'
        })
        .state('user.default', {   
            url: "/",
            templateUrl: 'app/main/user/user-info.default.html',
            controller: 'userDefaultController'
        })
        .state('user.register', {
            url: "/register",
            templateUrl: 'app/main/user/user-info.register.html',
            controller: 'userRegisterController'
        })
        .state('user.tree', {
            url: "/tree",
            templateUrl: 'app/main/user/user-info.tree.html',
            controller: 'userTreeController'
        })
}]);

mainApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
}]);

mainApp.run(['authService', function (authService) {
    authService.fillAuthData();
}])

mainApp.config(['treeConfig', function (treeConfig) {
    treeConfig.defaultCollapsed = true; // collapse nodes by default
}]);