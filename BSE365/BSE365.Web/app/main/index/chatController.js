
mainApp.controller('chatController',
[
    '$scope', '$location', '$anchorScroll', 'messageService', 'MessageState',
    function($scope, $location, $anchorScroll, messageService, MessageState) {
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
            gotoLastMessage(chat.account);
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

        function gotoLastMessage(withAccount) {
            if (!withAccount) {
                return;
            }
            var hashAccount = 'end-of-' + withAccount;
            console.log(hashAccount);
            if ($location.hash() !== hashAccount) {
                // set the $location.hash to `newHash` and
                // $anchorScroll will automatically scroll to it
                $location.hash(hashAccount);
            } else {
                // call $anchorScroll() explicitly,
                // since $location.hash hasn't changed
                $anchorScroll();
            }
        };

        $scope.$on('message:new-message-received',
            function(event, message) {
                if (message.targetAccount == $scope.currentTarget) {
                    messageService.updateMessageStates([message.Id]);
                    gotoLastMessage($scope.currentTarget);
                }
            });

        $scope.$on('message:hub-initiated',
            function(event, data) {
                messageService.promise.done(function() {
                    messageService.getUnreadMessages();
                });
            });

        function initData() {
            $scope.MessageState = MessageState;
            if (!messageService.isInitialed) {
                messageService.initHub();
            }
        }

        initData();
    }
]);