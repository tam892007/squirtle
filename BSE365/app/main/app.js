var mainApp = angular.module('mainApp', ["ui.router"]);

mainApp.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {

    $urlRouterProvider.otherwise("/")

    $stateProvider
        .state('home', {
            url: "/",
            templateUrl: 'app/main/home/home.html',
        })
}]);