'use strict';

mainApp.controller('indexController',
[
    '$scope', '$state', 'authService', 'userService', '$location', '_', '$timeout', 'localize', 'UserRolesText',
    function($scope, $state, authService, userService, $location, _, $timeout, localize, UserRolesText) {
        $scope.logOut = function() {
            authService.logOut();
            $state.go('login');
        }

        $scope.authentication = authService.authentication;
        $scope.UserRolesText = UserRolesText;

        $scope.$on('user:authenticated',
            function(event, data) {
                // you could inspect the data to see if what you care about changed, or just update your own scope
                $scope.getUserContext();
            });

        $scope.$on('user:updateAvatar',
            function(event, data) {
                $scope.userContext.avatar.url = data;
            });

        $scope.$on('user:getPinBalance',
            function(event, data) {
                $scope.userContext.pinBalance = data;
            });

        $scope.$on('user:removePinBalance',
            function(event, data) {
                if (!isNaN($scope.userContext.pinBalance))
                    $scope.userContext.pinBalance -= data;
            });

        $scope.getUserContext = function() {
            $scope.userContext = {};

            userService.getCurrentUserContext(function(res) {
                $scope.userContext = res;
            });
        }

        var params = $location.search();
        if ($scope.authentication.isAuth && params.anonymous != 'true') {
            ////Only load user context when authenticated (even if it's not refreshed)
            $scope.getUserContext();
        }

        $scope.forAdmin = function() {
            return $scope.authentication.isAuth &&
                $scope.userContext &&
                $scope.userContext.roles &&
                _.contains($scope.userContext.roles, UserRolesText.SuperAdmin);
        }

        $scope.forManager = function() {
            return $scope.authentication.isAuth &&
                $scope.userContext &&
                $scope.userContext.roles &&
                $scope.userContext.roles.length > 0;
        }

        $scope.isInRole = function (allowedRoles) {
            if (!$scope.authentication.isAuth) {
                return false;
            }
            var result = $scope.forAdmin();
            if ($scope.userContext.roles && !result) {
                for (var i = 0; i < allowedRoles.length; i++) {
                    result = _.contains($scope.userContext.roles, allowedRoles[i]);
                    if (result) {
                        return result;
                    }
                }
            }
            return result;
        }

        $scope.changeLanguage = function(language) {
            $timeout(function() {
                localize.setLanguage(language);
            });
        }

        $scope.isCurrentLanguage = function(language) {
            return localize.isCurrentLanguage(language);
        }
    }
]);