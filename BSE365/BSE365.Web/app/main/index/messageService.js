
mainApp.factory('messageService',
[
    '$rootScope', '$interval', 'Hub', '_', 'MessageState',
    function($rootScope, $interval, Hub, _, MessageState) {
        var data = this;
        data.messages = {};
        data.notifications = [];
        data.currentAccount = null;

        function addMessage(message, account) {
            if (!account) {
                if (message.ToAccount == data.currentAccount) {
                    account = message.FromAccount;
                } else {
                    account = message.ToAccount;
                }
            }
            if (data.messages[account] == null) {
                data.messages[account] = [];
            }
            if (_.where(data.messages[account], { Id: message.Id }).length == 0) {
                data.messages[account].push(message);
            }
        }

        //declaring the hub connection
        var hub = new Hub('Message',
        {
            //client side methods
            listeners: {
                'notify': function(notification) {
                    data.notifications.push(notification);
                    $rootScope.$apply();
                },
                'message': function(message) {
                    addMessage(message, message.fromAccount);
                    $rootScope.$apply();
                },
            },
            //server side methods
            methods: [
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
        });

        data.hub = hub;
        data.promise = hub.promise;

        data.getUnreadMessages = function() {
            hub.getUnreadMessages()
                .done(function(messages) {
                    _.each(messages, addMessage);
                    $rootScope.$apply();
                });
        };
        data.getMessageHistoryWith = function(targetAccount, pointTime) {
            hub.getMessageHistoryWith(targetAccount, pointTime)
                .done(function(messages) {
                    _.each(messages, addMessage);
                });
        };
        data.sendMessageTo = function(targetAccount, message) {
            hub.sendMessageTo(targetAccount, message)
                .done(function(result) {
                    addMessage(result);
                });
        };
        data.updateMessageStates = function(messageIds) {
            hub.updateMessageStates(messageIds)
                .done(function() {
                    for (var key in data.messages) {
                        _.each(messageIds,
                            function(id) {
                                var target = _.findWhere(data.messages[key], { Id: id });
                                if (target) {
                                    target.State = MessageState.Dissmissed;
                                }
                            });
                    }
                });
        };

        data.getUnreadNotifications = function() {
            hub.getUnreadNotifications()
                .done(function (notifications) {
                    console.log(notifications);
                    _.each(notifications,
                        function(item) {
                            if (_.findWhere(data.notifications, { Id: item.Id }) == null) {
                                data.notifications.push(item);
                            }
                        });
                    $rootScope.$apply();
                });
        };
        data.updateNotificationStates = function(notificationIds) {
            hub.updateNotificationStates(notificationIds)
                .done(function() {
                    _.each(notificationIds,
                        function(id) {
                            var target = _.findWhere(data.notifications, { Id: id });
                            if (target) {
                                target.State = MessageState.Dissmissed;
                            }
                        });
                });
        };

        hub.promise.done(function () {
        });

        return data;
    }
]);