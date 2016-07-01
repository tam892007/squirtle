
mainApp.controller('notificationController',
[
    '$scope', 'messageService',
    function($scope, messageService) {
        $scope.messageData = messageService;

        $scope.dissmiss = function(target) {
            messageService.updateNotificationStates([target.Id]);
        }

        $scope.clear = function() {
            var ids = messageService.notifications.map(function (item) {
                return item.Id;
            });
            messageService.updateNotificationStates(ids);
            messageService.notifications = [];
        }

        $scope.refresh = function() {
            messageService.notifications = [];
            messageService.getUnreadNotifications();
        }

        function initData() {
            messageService.promise.done(function() {
                messageService.getUnreadNotifications();
            });
        }

        initData();
    }
]);