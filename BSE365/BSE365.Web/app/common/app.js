var commonApp = angular.module('commonApp', ['ngResource', 'ui.router']);

var serviceBase = 'http://localhost:2736/';
commonApp.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'SHARE'
});

commonApp.constant('recaptchaSettings', {
    publicKey: '6LcFKCETAAAAAN6JPAwzot2fExNpZUumgKXj Jugq',
});

commonApp.constant('filterSetting', {
    pagination: {
        template: 'template/smart-table/pagination.html',
        itemsByPage: 30,
        displayedPages: 10
    },
    search: {
        delay: 400, // ms
        inputEvent: 'input'
    },
    select: {
        mode: 'single',
        selectedClass: 'st-selected'
    },
    sort: {
        ascentClass: 'st-sort-ascent',
        descentClass: 'st-sort-descent',
        descendingFirst: false,
        skipNatural: false,
        delay: 300
    },
    pipe: {
        delay: 100 //ms
    }
});

