
mainApp.factory('messageService',
[
    '$rootScope', '$interval', 'Hub', '_', '$rootScope', 'MessageState',
    function($rootScope, $interval, Hub, _, $rootScope, MessageState) {
        var data = this;
        data.chatData = [];
        data.notifications = [];
        data.currentAccount = null;
        data.isInitialed = false;
        //hub options
        var hubOptions = {
            autoConnect: false,
            //client side methods
            listeners: {
                'notify': onServerNotify,
                'message': onServerMessage,
            },
            //server side methods
            methods: [
                'getCurrentAccount', 'validAccount',
                'getUnreadMessages', 'getMessageHistoryWith', 'sendMessageTo', 'updateMessageStates',
                'getUnreadNotifications', 'updateNotificationStates'
            ],
            //query params sent on initial connection
            queryParams: {
                'token': 'nazi-token'
            },
            //handle connection error
            errorHandler: function(error) {
                console.error(error);
            },
            logging: true,
            //specify a non default root
            //rootPath: '/api
        };

        var hub = new Hub('Message', hubOptions);
        data.hub = hub;

        data.initHub = function () {
            return;
            data.promise = data.hub.promise = data.hub.connect();
            data.hub.promise.done(function () {
                data.getCurrentAccount();
                $rootScope.$broadcast('message:hub-initiated');
                data.isInitialed = true;
            });
        }

        data.destroyHub = function() {
            data.hub.disconnect();
            $rootScope.$broadcast('message:hub-destroyed');
        }

        $rootScope.$on('user:authenticated', data.initHub);

        $rootScope.$on('user:signedout', data.destroyHub);

        function getChat(targetAccount) {
            if (!targetAccount) {
                var errorMsg = 'target account is empty';
                console.log(errorMsg);
                throw errorMsg;
            }
            var chat = _.findWhere(data.chatData, { account: targetAccount });
            if (chat == null) {
                chat = { account: targetAccount, messages: [] };
                data.chatData.push(chat);
                $rootScope.$apply();
            }
            return chat;
        }

        function closeChat(targetAccount) {
            if (!targetAccount) {
                var errorMsg = 'target account is empty';
                console.log(errorMsg);
                throw errorMsg;
            }
            var chat = _.findWhere(data.chatData, { account: targetAccount });
            if (chat != null) {
                var index = data.chatData.indexOf(chat);
                if (index > -1) {
                    delete data.chatData[index];
                    data.chatData.splice(index, 1);
                }
                $rootScope.$apply();
            }
            return chat;
        }

        function addMessage(message) {
            if (!message) {
                var errorMsg = 'error when add message!';
                console.log(errorMsg);
                throw errorMsg;
            }
            var isOwner = false;
            var owner = null;
            var targetAccount = null;
            if (message.FromAccount == data.currentAccount) {
                isOwner = true;
                owner = 'You'; //data.currentAccount;
                targetAccount = message.ToAccount;
            } else {
                isOwner = false;
                owner = message.FromAccount;
                targetAccount = message.FromAccount;
            }
            var chat = getChat(targetAccount);
            if (_.findWhere(chat.messages, { Id: message.Id }) == null) {
                message.isOwner = isOwner;
                message.Owner = owner;
                chat.messages.push(message);
            }
        };

        function onServerNotify(notification) {
            data.notifications.push(notification);
            $rootScope.$broadcast('notification:new-notification-received', notification);
            $rootScope.$apply();
        };

        function onServerMessage(message) {
            addMessage(message);
            $rootScope.$broadcast('message:new-message-received', message);
            $rootScope.$apply();
        };

        data.getCurrentAccount = function() {
            var promise = data.hub.getCurrentAccount();
            promise.done(function(result) {
                data.currentAccount = result;
            });
            return promise;
        };

        data.validAccount = function(targetAccount) {
            var promise = data.hub.validAccount(targetAccount);
            return promise;
        };

        data.getUnreadMessages = function() {
            var promise = data.hub.getUnreadMessages();
            promise.done(function(messages) {
                if (messages) {
                    _.each(messages, addMessage);
                    $rootScope.$apply();
                }
            });
            return promise;
        };

        data.getMessageHistoryWith = function(targetAccount, pointTime) {
            var promise = data.hub.getMessageHistoryWith(targetAccount, pointTime);
            promise.done(function(messages) {
                if (messages) {
                    _.each(messages, addMessage);
                    $rootScope.$apply();
                }
            });
            return promise;
        };

        data.sendMessageTo = function(targetAccount, message) {
            var promise = data.hub.sendMessageTo(targetAccount, message);
            promise.done(function(result) {
                addMessage(result);
                $rootScope.$apply();
            });
            return promise;
        };

        data.updateMessageStates = function(messageIds) {
            var promise = data.hub.updateMessageStates(messageIds);
            promise.done(function() {
                var messages = [];
                _.each(data.chatData,
                    function(chat) {
                        messages.push(_.flatten(_.pick(chat, 'messages')));
                    });
                _.each(messageIds,
                    function(mesId) {
                        var target = _.findWhere(messages, { Id: mesId });
                        if (target != null) {
                            target.State = MessageState.Readed;
                        }
                    });
            });
            return promise;
        };

        data.getUnreadNotifications = function() {
            var promise = data.hub.getUnreadNotifications();
            promise.done(function(notifications) {
                _.each(notifications,
                    function(item) {
                        if (_.findWhere(data.notifications, { Id: item.Id }) == null) {
                            data.notifications.push(item);
                        }
                    });
                $rootScope.$apply();
            });
            return promise;
        };

        data.updateNotificationStates = function(notificationIds) {
            var promise = data.hub.updateNotificationStates(notificationIds);
            promise.done(function() {
                _.each(notificationIds,
                    function(id) {
                        var target = _.findWhere(data.notifications, { Id: id });
                        if (target) {
                            target.State = MessageState.Dissmissed;
                        }
                    });
            });
            return promise;
        };

        data.getChat = getChat;
        data.closeChat = closeChat;

        return data;
    }
]);