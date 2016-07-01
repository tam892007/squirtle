
mainApp.controller('chatController',
[
    '$scope', 'messageService',
    function($scope, messageService) {
        $scope.messageData = messageService;

        function initData() {
            messageService.promise.done(function () {
                messageService.getUnreadMessages();
            });
        }

        initData();
    }
]);