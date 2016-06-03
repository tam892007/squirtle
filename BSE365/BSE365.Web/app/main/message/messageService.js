mainApp.factory('messageService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            send: { method: 'POST', params: { path: 'api/message/send' } },
            //queueGive: { method: 'POST', params: { path: 'api/trade/queueGive' } },
            //queueReceive: { method: 'POST', params: { path: 'api/trade/queueReceive' } },
            //queryWaitingGivers: { method: 'POST', params: { path: 'api/trade/queryWaitingGivers' }, isArray:true },
            //queryWaitingReceivers: { method: 'POST', params: { path: 'api/trade/queryWaitingReceivers' }, isArray: true },
        });
    }
]);