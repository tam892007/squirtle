var authApp = angular.module('authApp', ['LocalStorageModule', 'commonApp']);

authApp.factory('UserRolesText',
    function () {
        var data = {
            User: "user",

            SuperAdmin: "superadmin",
            ManageUserInfo: "manageuserinfo",

            ManageTransaction: "managetransaction",
            MapTransaction: "maptransaction",
        };
        return data;
    });
