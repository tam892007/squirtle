'use strict';
authApp.factory('authService',
[
    '$http', '$q', 'localStorageService', 'ngAuthSettings', '$rootScope',
    function($http, $q, localStorageService, ngAuthSettings, $rootScope) {

        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var authServiceFactory = {};

        var _authentication = {
            isAuth: false,
            userName: "",
            useRefreshTokens: false
        };

        var _externalAuthData = {
            provider: "",
            userName: "",
            externalAccessToken: ""
        };

        var _saveRegistration = function(registration) {

            _logOut();

            return $http.post(serviceBase + 'api/account/register', registration)
                .then(function(response) {
                    return response;
                });

        };

        var _login = function(loginData) {

            var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

            if (loginData.useRefreshTokens) {
                data = data + "&client_id=" + ngAuthSettings.clientId;
            }

            var deferred = $q.defer();

            $http.post(serviceBase + 'token',
                    data,
                    { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
                .success(function(response) {

                    if (loginData.useRefreshTokens) {
                        localStorageService.set('authorizationData',
                        {
                            token: response.access_token,
                            userName: loginData.userName,
                            refreshToken: response.refresh_token,
                            useRefreshTokens: true
                        });
                    } else {
                        localStorageService.set('authorizationData',
                        {
                            token: response.access_token,
                            userName: loginData.userName,
                            refreshToken: "",
                            useRefreshTokens: false
                        });
                    }
                    // cookie token for signalR
                    setCookie('BearerToken', response.access_token, 1);
                    _authentication.isAuth = true;
                    _authentication.userName = loginData.userName;
                    _authentication.useRefreshTokens = loginData.useRefreshTokens;
                    $rootScope.$broadcast('user:authenticated'); ////tamld - to update user context;

                    deferred.resolve(response);

                })
                .error(function(err, status) {
                    //_logOut();
                    deferred.reject(err);
                });

            return deferred.promise;

        };

        var _logOut = function() {

            localStorageService.remove('authorizationData');

            _authentication.isAuth = false;
            _authentication.userName = "";
            _authentication.useRefreshTokens = false;
            // cookie token for signalR
            setCookie('BearerToken', '', 0);
            $rootScope.$broadcast('user:signedout');
        };

        var _fillAuthData = function() {

            var authData = localStorageService.get('authorizationData');
            if (authData) {
                _authentication.isAuth = true;
                _authentication.userName = authData.userName;
                _authentication.useRefreshTokens = authData.useRefreshTokens;
            }

        };

        var _refreshToken = function() {
            var deferred = $q.defer();

            var authData = localStorageService.get('authorizationData');

            if (authData && !authServiceFactory.tokenRefreshing) {
                if (authData.useRefreshTokens) {
                    authServiceFactory.tokenRefreshing = true; ////Prevent multiple refresh token requested   -  tamld

                    var data = "grant_type=refresh_token&refresh_token=" +
                        authData.refreshToken +
                        "&client_id=" +
                        ngAuthSettings.clientId;

                    //localStorageService.remove('authorizationData');

                    $http.post(serviceBase + 'token',
                            data,
                            { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
                        .success(function(response) {
                            localStorageService.set('authorizationData',
                            {
                                token: response.access_token,
                                userName: response.userName,
                                refreshToken: response.refresh_token,
                                useRefreshTokens: true
                            });
                            $rootScope.$broadcast('user:authenticated');
                            deferred.resolve(response);

                        })
                        .error(function(err, status) {
                            _logOut();
                            deferred.reject(err);
                        })
                        .finally(function() {
                            authServiceFactory.tokenRefreshing = false;
                        });
                }
            }

            return deferred.promise;
        };

        var _obtainAccessToken = function(externalData) {

            var deferred = $q.defer();

            $http.get(serviceBase + 'api/account/ObtainLocalAccessToken',
                { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } })
                .success(function(response) {

                    localStorageService.set('authorizationData',
                    {
                        token: response.access_token,
                        userName: response.userName,
                        refreshToken: "",
                        useRefreshTokens:
                            false
                    });

                    _authentication.isAuth = true;
                    _authentication.userName = response.userName;
                    _authentication.useRefreshTokens = false;

                    deferred.resolve(response);

                })
                .error(function(err, status) {
                    _logOut();
                    deferred.reject(err);
                });

            return deferred.promise;

        };

        var _registerExternal = function(registerExternalData) {

            var deferred = $q.defer();

            $http.post(serviceBase + 'api/account/registerexternal', registerExternalData)
                .success(function(response) {

                    localStorageService.set('authorizationData',
                    {
                        token: response.access_token,
                        userName: response.userName,
                        refreshToken: "",
                        useRefreshTokens:
                            false
                    });

                    _authentication.isAuth = true;
                    _authentication.userName = response.userName;
                    _authentication.useRefreshTokens = false;

                    deferred.resolve(response);

                })
                .error(function(err, status) {
                    _logOut();
                    deferred.reject(err);
                });

            return deferred.promise;

        };

        var _forgetPassword = function(userName) {
            var deferred = $q.defer();

            $http.get(serviceBase + 'api/account/forgotpassword', { params: { userName: userName } })
                .success(function(res) {
                    deferred.resolve(res);
                })
                .error(function(err, status) {
                    deferred.reject(err);
                });

            return deferred.promise;
        }

        var _resetPassword = function(resetData) {
            var deferred = $q.defer();

            $http.post(serviceBase + 'api/account/resetpassword', resetData)
                .success(function(response) {
                    deferred.resolve(response);
                })
                .error(function(err, status) {
                    deferred.reject(err);
                });

            return deferred.promise;
        }

        function setCookie(cname, cvalue, exdays) {
            var d = new Date();
            d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
            var expires = "expires=" + d.toUTCString();
            document.cookie = cname + "=" + cvalue + "; " + expires;
        }

        authServiceFactory.saveRegistration = _saveRegistration;
        authServiceFactory.login = _login;
        authServiceFactory.logOut = _logOut;
        authServiceFactory.fillAuthData = _fillAuthData;
        authServiceFactory.authentication = _authentication;
        authServiceFactory.refreshToken = _refreshToken;

        authServiceFactory.obtainAccessToken = _obtainAccessToken;
        authServiceFactory.externalAuthData = _externalAuthData;
        authServiceFactory.registerExternal = _registerExternal;

        authServiceFactory.forgetPassword = _forgetPassword;
        authServiceFactory.resetPassword = _resetPassword;

        return authServiceFactory;
    }
]);