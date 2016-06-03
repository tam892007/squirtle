'use strict';
mainApp.controller('inboxController', ['$scope', 'userService', 'messageService', '$q', 'Notification', 'localStorageService', '$state', function ($scope, userService, messageService, $q, Notification, localStorageService, $state) {
    
    $scope.init = function () {
        var currentUserName = localStorageService.get('authorizationData').userName;
        if (angular.isUndefined(userName) || userName === null) return;
        $scope.model = {};
        $scope.model.fromUser = currentUserName;
    }

    $scope.init();

    $scope.submit = function () {
        $scope.submitted = true;
        if (!$scope.frmSendMessage.$valid) return;
        messageService.send($scope.model, function (res) {
            ////Reload when save successfully
            Notification.success('Send message successfully.');
            $state.reload();
        },
        function (err) {
            Notification.error('Some errors happenned. Contact your administrator.');
        });
    }

    $scope.interacted = function (field) {
        return $scope.submitted || field.$dirty;
    };

}]);