var commonApp = angular.module('commonApp', ['ngResource', 'ui.router']);

var serviceBase = 'http://localhost:2736/';
commonApp.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'SHARE'
});

commonApp.constant('recaptchaSettings', {
    publicKey: '6LcFKCETAAAAAN6JPAwzot2fExNpZUumgKXj Jugq',
});

