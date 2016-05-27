var commonApp = angular.module('commonApp', ['ngResource']);

var serviceBase = 'http://localhost:2736/';
commonApp.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'SHARE'
});
