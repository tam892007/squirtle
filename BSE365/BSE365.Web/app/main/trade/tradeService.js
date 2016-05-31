mainApp.factory('tradeService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            status: { method: 'POST', params: { path: 'api/trade/accountStatus' } },
            queueGive: { method: 'POST', params: { path: 'api/trade/queueGive' } },
            queueReceive: { method: 'POST', params: { path: 'api/trade/queueReceive' } },
            queryWaitingGivers: { method: 'POST', params: { path: 'api/trade/queryWaitingGivers' }, isArray:true },
            queryWaitingReceivers: { method: 'POST', params: { path: 'api/trade/queryWaitingReceivers' }, isArray: true },
        });
    }
]);