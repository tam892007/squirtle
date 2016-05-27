var commonApp = angular.module('commonApp', []);

var serviceBase = 'http://localhost:2736/';
commonApp.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: ''
});