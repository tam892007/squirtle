'use strict';
authApp.controller('loginController', ['$scope', '$location', 'authService', 'ngAuthSettings', function ($scope, $location, authService, ngAuthSettings) {    
    
    $scope.forgotPassword = function () {        
        $scope.init();
        $scope.state = 2;
    }

    $scope.requestNewPassword = function () {
        authService.forgetPassword($scope.forgotUserName).then(function (res) {
            $scope.message = "Successfully. Please check your email.";            
        },
        function (err) {
            $scope.message = err.error_description;
        });
    }

    $scope.init = function () {
        $scope.message = "";
        $scope.state = 1;
        $scope.loginData = {
            userName: "",
            password: "",
            useRefreshTokens: true
        };
        $scope.forgotUserName = '';

        $scope.submitted = false;
    }

    $scope.init();

    $scope.login = function () {
        $scope.submitted = true;

        if (!$scope.frmLogin.$valid) return;        

        authService.login($scope.loginData).then(function (response) {
            $location.path('/');
        },
        function (err) {
            $scope.message = err.error_description;
        });
    };

    $scope.authExternalProvider = function (provider) {

        var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';

        var externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/Account/ExternalLogin?provider=" + provider
                                                                    + "&response_type=token&client_id=" + ngAuthSettings.clientId
                                                                    + "&redirect_uri=" + redirectUri;
        window.$windowScope = $scope;

        var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
    };

    $scope.authCompletedCB = function (fragment) {

        $scope.$apply(function () {

            if (fragment.haslocalaccount == 'False') {

                authService.logOut();

                authService.externalAuthData = {
                    provider: fragment.provider,
                    userName: fragment.external_user_name,
                    externalAccessToken: fragment.external_access_token
                };

                $location.path('/associate');

            }
            else {
                //Obtain access token and redirect to orders
                var externalData = { provider: fragment.provider, externalAccessToken: fragment.external_access_token };
                authService.obtainAccessToken(externalData).then(function (response) {

                    $location.path('/');

                },
             function (err) {
                 $scope.message = err.error_description;
             });
            }

        });
    }

    $scope.interacted = function (field) {    
        return $scope.submitted || field.$dirty;
    };
}]);