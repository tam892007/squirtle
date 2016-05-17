var mainApp = angular.module('mainApp', ['ui.router', 'authApp']);

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

}]);

mainApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
}]);

mainApp.run(['authService', function (authService) {
    authService.fillAuthData();
}])