mainApp.factory('tradeService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            status: { method: 'POST', params: { path: 'api/trade/accountStatus' } },
            queueGive: { method: 'POST', params: { path: 'api/trade/queueGive' } },
            queueReceive: { method: 'POST', params: { path: 'api/trade/queueReceive' } },
            claimBonus: { method: 'POST', params: { path: 'api/trade/claimBonus' } },
            queryHistory: { method: 'POST', params: { path: 'api/trade/queryHistory' }, isArray: true },
            queryPunishment: { method: 'POST', params: { path: 'api/transaction/queryPunishment' }, isArray: true },
            queryBonus: { method: 'POST', params: { path: 'api/transaction/queryBonus' }, isArray: true },
        });
    }
]);