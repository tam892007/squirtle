var mainApp = angular.module('mainApp',
[
    'ui.router', 'ui.bootstrap', 'authApp', 'ngResource', 'ui.tree', 'smart-table', 'underscore', 'angularFileUpload',
    'ngImgCrop', 'ngMessages', 'angular-loading-bar', 'ui.validate', 'reCAPTCHA', 'commonApp', 'ui-notification',
    'timer'
]);

mainApp.config([
    '$stateProvider', '$urlRouterProvider', function($stateProvider, $urlRouterProvider) {

        $urlRouterProvider.otherwise("/login");

        $stateProvider
            .state('home',
            {
                url: "/",
                templateUrl: 'app/main/home/home.html',
            })
            .state('login',
            {
                url: "/login",
                templateUrl: 'app/authentication/login/login.html',
                controller: 'loginController'
            })
            .state('resetPassword',
            {
                url: "/resetpassword",
                templateUrl: 'app/authentication/password/resetPassword.html',
                controller: 'resetPasswordController'
            })
            .state('refresh',
            {
                url: "/refresh",
                templateUrl: 'app/authentication/refresh/refresh.html',
                controller: 'refreshController'
            })
            .state('pin',
            {
                abstract: true,
                templateUrl: "app/common/templates/empty.html"
            })
            .state('pin.transfer',
            {
                url: "/transfer",
                templateUrl: 'app/main/pin/pin.html',
                controller: 'pinController'
            })
            .state('pin.rate',
            {
                url: "/rate",
                templateUrl: 'app/main/pin/rate.html',
                controller: 'rateController'
            })
            .state('user',
            {
                abstract: true,
                url: "/user",
                templateUrl: 'app/main/user/user-info.html',
                controller: 'userController'
            })
            .state('user.default',
            {
                url: "/",
                templateUrl: 'app/main/user/user-info.default.html',
                controller: 'userDefaultController'
            })
            .state('user.register',
            {
                url: "/register",
                templateUrl: 'app/main/user/user-info.register.html',
                controller: 'userRegisterController'
            })
            .state('user.tree',
            {
                url: "/tree",
                templateUrl: 'app/main/user/user-info.tree.html',
                controller: 'userTreeController'
            })
            .state('trade',
            {
                abstract: true,
                url: "/trade",
                templateUrl: 'app/main/trade/trade-info.html',
                controller: 'tradeInfoController'
            })
            .state('trade.statistic',
            {
                url: "/",
                templateUrl: 'app/main/trade/trade-statistic.html',
                controller: 'tradeStatisticController'
            })
            .state('trade.history',
            {
                url: "/history",
                templateUrl: 'app/main/trade/trade-history.html',
                controller: 'tradeHistoryController'
            })
            .state('waitinggiver',
            {
                url: "/waitinggiver",
                templateUrl: 'app/main/account/waitingGiver.html',
                controller: 'waitingGiverController'
            })
            .state('waitingreceiver',
            {
                url: "/waitingreceiver",
                templateUrl: 'app/main/account/waitingReceiver.html',
                controller: 'waitingReceiverController'
            })
            .state('account',
            {
                abstract: true,
                url: "/account",
                templateUrl: 'app/main/account/account.html',
                controller: 'accountController'
            })
            .state('account.default',
            {
                url: "/",
                templateUrl: 'app/main/account/account-default.html',
                controller: function() {}
            })
            .state('account.details',
            {
                url: "/:key",
                templateUrl: 'app/main/account/account-edit.html',
                controller: 'accountInfoController'
            })
            .state('currentTransaction',
            {
                url: "/trade/current",
                templateUrl: 'app/main/transaction/current.html',
                controller: 'transactionCurrentController'
            })
            .state('reportedTransactions',
            {
                url: "/trade/reported-transactions",
                templateUrl: 'app/main/transaction/transactionReported.html',
                controller: 'transactionReportedController'
            })
            .state('association',
            {
                url: "/association",
                templateUrl: 'app/main/association/association.html',
                controller: 'associationController'
            });
    }
]);

mainApp.config([
    '$httpProvider', function($httpProvider) {
        $httpProvider.interceptors.push('authInterceptorService');
    }
]);

mainApp.run([
    'authService', function(authService) {
        authService.fillAuthData();
    }
]);

mainApp.config([
    'treeConfig', function(treeConfig) {
        treeConfig.defaultCollapsed = true; // collapse nodes by default
    }
]);

mainApp.config([
    'cfpLoadingBarProvider', function(cfpLoadingBarProvider) {
        cfpLoadingBarProvider.includeSpinner = false;
    }
]);

mainApp.config([
    'reCAPTCHAProvider', 'recaptchaSettings', function(reCAPTCHAProvider, recaptchaSettings) {
        // required: please use your own key :)
        reCAPTCHAProvider.setPublicKey(recaptchaSettings.publicKey);

        // optional: gets passed into the Recaptcha.create call
        reCAPTCHAProvider.setOptions({
            theme: 'red'
        });
    }
]);

mainApp.config([
    'NotificationProvider', function(NotificationProvider) {
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
    }
]);

mainApp.factory('AccountState',
    function() {
        var data = {
            /// <summary>
            /// Must give 
            /// </summary>
            Default: 0,
            /// <summary>
            /// Queued in WaitingGive
            /// </summary>
            WaitGive: 1,
            /// <summary>
            /// Gave
            /// Can queue receive
            /// </summary>
            Gave: 2,
            /// <summary>
            /// Queued in WaitingReceive
            /// </summary>
            WaitReceive: 3,

            InGiveTransaction: 11,
            InReceiveTransaction: 12,

            NotGive: 21,
            NotConfirm: 22,
            ReportedNotTransfer: 23,

            AbadonedOne: 31,
        };
        data.display = function(value) {
            switch (value) {
            case data.Default:
                return 'Default';
            case data.WaitGive:
                return 'Wait Give';
            case data.Gave:
                return 'Gave';
            case data.WaitReceive:
                return 'Wait Receive';
            case data.InGiveTransaction:
                return 'In Give Transaction';
            case data.InReceiveTransaction:
                return 'In Receive Transaction';
            case data.NotGive:
                return 'Not Give';
            case data.NotConfirm:
                return 'Not Confirm';
            case data.ReportedNotTransfer:
                return 'Reported Not Transfer';
            case data.AbadonedOne:
                return 'Abadoned One';
            default:
                return '';
            }
        }
        return data;
    });

mainApp.factory('PriorityLevel',
    function() {
        var data = {
            Default: 0,
            Priority: 1,
            High: 2,
            Highest: 10,
        }
        data.display = function(value) {
            switch (value) {
            case data.Default:
                return 'Default';
            case data.Priority:
                return 'Priority';
            case data.High:
                return 'High';
            case data.Highest:
                return 'Highest';
            default:
                return '';
            }
        }
        return data;
    });

mainApp.factory('TransactionState',
    function() {
        var data = {
            Begin: 0,
            Transfered: 1,
            Confirmed: 2,

            NotTransfer: 21,
            NotConfirm: 22,
            ReportedNotTransfer: 23,

            Abadoned: 31,
        }
        data.display = function(value) {
            switch (value) {
            case data.Begin:
                return 'Begin';
            case data.Transfered:
                return 'Transfered';
            case data.Confirmed:
                return 'Confirmed';
            case data.NotTransfer:
                return 'Not Transfer';
            case data.NotConfirm:
                return 'Not Confirm';
            case data.ReportedNotTransfer:
                return 'Reported Not Transfer';
            case data.Abadoned:
                return 'Abadoned';
            default:
                return '';
            }
        }
        return data;
    });

mainApp.factory('TransactionType',
    function() {
        var data = {
            Begin: 0,
            Abadoned: 31,
            Replacement: 41,
        }
        data.display = function(value) {
            switch (value) {
            case data.Begin:
                return 'Begin';
            case data.Abadoned:
                return 'Abadoned';
            case data.Replacement:
                return 'Replacement';
            default:
                return '';
            }
        }
        return data;
    });

mainApp.factory('ReportResult',
    function() {
        var data = {
            Default: 0,
            GiverTrue: 1,
            ReceiverTrue: 2,
            BothTrue: 11,
            BothFalse: 12,
        }
        data.display = function(value) {
            switch (value) {
            case data.Default:
                return 'Default';
            case data.GiverTrue:
                return 'Giver True';
            case data.ReceiverTrue:
                return 'Receiver True';
            case data.BothTrue:
                return 'Both True';
            case data.BothFalse:
                return 'Both False';
            default:
                return '';
            }
        }
        return data;
    });

mainApp.factory('UserState',
    function() {
        var data = {
            /// <summary>
            /// Must give 
            /// </summary>
            Default: 0,

            NotGive: 21,
            NotConfirm: 22,
        };
        data.display = function(value) {
            switch (value) {
            case data.Default:
                return 'Default';
            case data.NotGive:
                return 'Not Give';
            case data.NotConfirm:
                return 'Not Confirm';
            default:
                return '';
            }
        }
        return data;
    });

mainApp.factory('ConfigData',
    function() {
        var data = {
            dateFormat: 'yyyy/MM/dd',
            dateTimeFormat: 'yyyy/MM/dd - hh:mm:ss',
        }
        return data;
    });

mainApp.filter('yesno',
[
    function() {
        return function(input) {
            return input ? 'Yes' : 'No';
        };
    }
]);

function StringToXML(oString) {
    //code for IE
    if (window.ActiveXObject) {
        var oXML = new ActiveXObject("Microsoft.XMLDOM");
        oXML.loadXML(oString);
        return oXML;
    }
    // code for Chrome, Safari, Firefox, Opera, etc. 
    else {
        return (new DOMParser()).parseFromString(oString, "text/xml");
    }
}

mainApp.config([
    'stConfig', 'filterSetting', function(stConfig, filterSetting) {
        stConfig.pagination.itemsByPage = filterSetting.pagination.itemsByPage;
        stConfig.pagination.displayedPages = filterSetting.pagination.displayedPages;
    }
]);