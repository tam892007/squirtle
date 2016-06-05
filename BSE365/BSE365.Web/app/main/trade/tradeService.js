mainApp.factory('tradeService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            status: { method: 'POST', params: { path: 'api/trade/accountStatus' } },
            queueGive: { method: 'POST', params: { path: 'api/trade/queueGive' } },
            queueReceive: { method: 'POST', params: { path: 'api/trade/queueReceive' } },
            queryHistory: { method: 'POST', params: { path: 'api/trade/queryHistory' }, isArray: true },
        });
    }
]);