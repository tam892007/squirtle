var mainApp = angular.module('mainApp', ['ui.router', 'ui.bootstrap', 'authApp', 'ngResource', 'ui.tree', 'smart-table', 'underscore', 'angularFileUpload'
    , 'ngImgCrop', 'ngMessages', 'angular-loading-bar', 'ui.validate', 'reCAPTCHA', 'commonApp', 'ui-notification']);

mainApp.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {

    $urlRouterProvider.otherwise("/login")

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
        .state('refresh', {
            url: "/refresh",
            templateUrl: 'app/authentication/refresh/refresh.html',
            controller: 'refreshController'
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
        .state('association', {
            url: "/association",
            templateUrl: 'app/main/association/association.html',
            controller: 'associationController'
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

mainApp.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
}]);

mainApp.config(['reCAPTCHAProvider', 'recaptchaSettings', function (reCAPTCHAProvider, recaptchaSettings) {
    // required: please use your own key :)
    reCAPTCHAProvider.setPublicKey(recaptchaSettings.publicKey);

    // optional: gets passed into the Recaptcha.create call
    reCAPTCHAProvider.setOptions({
        theme: 'red'
    });
}]);

mainApp.config(['NotificationProvider', function (NotificationProvider) {
    NotificationProvider.setOptions({
        delay: 1500,
        startTop: 20,
        startRight: 10,
        verticalSpacing: 20,
        horizontalSpacing: 20,
        positionX: 'right',
        positionY: 'bottom',
        closeOnClick: true,
        maxCount: 3,
    });
}]);