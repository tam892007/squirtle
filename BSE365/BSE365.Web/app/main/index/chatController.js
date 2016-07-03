
mainApp.controller('chatController',
[
    '$scope', 'messageService', 'MessageState',
    function($scope, messageService, MessageState) {
        $scope.messageData = messageService;

        $scope.newChat = function(targetAccount) {
            messageService.validAccount(targetAccount)
                .done(function(result) {
                    if (result) {
                        messageService.getChat(targetAccount);
                    }
                });
        }

        $scope.sendMessageTo = function(chat) {
            var newMessage = chat.newMessage;
            chat.newMessage = '';
            return messageService.sendMessageTo(chat.account, newMessage);
        }

        function updateMessageStates(chat) {
            var messageIds = _.map(_.where(chat.messages, { State: MessageState.UnRead }),
                function(item) {
                    return item.Id;
                });
            return messageService.updateMessageStates(messageIds);
        }

        $scope.closeChat = function(chat) {
            updateMessageStates(chat)
                .done(function() {
                    messageService.closeChat(chat.account);
                });
        }

        $scope.onfocus = function(chat) {
            $scope.currentTarget = chat.account;
            updateMessageStates(chat);
        }

        $scope.onblur = function(chat) {
            $scope.currentTarget = null;
        }

        $scope.$on('message:new-message-received',
            function(event, message) {
                if (message.targetAccount == $scope.currentTarget) {
                    messageService.updateMessageStates([message.Id]);
                }
            });

        function initData() {
            $scope.MessageState = MessageState;

            messageService.promise.done(function() {
                messageService.getUnreadMessages();
            });
        }

        initData();
    }
]);