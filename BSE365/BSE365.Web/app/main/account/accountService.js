mainApp.factory('accountService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            status: { method: 'POST', params: { path: 'api/trade/accountStatus', key: 'key' } },
            queueGive: { method: 'POST', params: { path: 'api/trade/queueGive', key: 'key' } },
            queueReceive: { method: 'POST', params: { path: 'api/trade/queueReceive', key: 'key' } },
            queryAccount: { method: 'POST', params: { path: 'api/trade/queryAccount' }, isArray: true },
            queryHistory: { method: 'POST', params: { path: 'api/trade/queryHistory', key: 'key' }, isArray: true },
            queryWaitingGivers: { method: 'POST', params: { path: 'api/trade/queryWaitingGivers' }, isArray:true },
            queryWaitingReceivers: { method: 'POST', params: { path: 'api/trade/queryWaitingReceivers' }, isArray: true },
        });
    }
]);