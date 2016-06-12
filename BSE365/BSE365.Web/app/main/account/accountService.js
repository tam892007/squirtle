mainApp.factory('accountService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            status: { method: 'POST', params: { path: 'api/trade/accountStatus', key: 'key' } },

            setAccountPriority: { method: 'POST', params: { path: 'api/trade/setAccountPriority' } },
            setAccountState: { method: 'POST', params: { path: 'api/trade/setAccountState' } },

            queueGive: { method: 'POST', params: { path: 'api/trade/queueGive', key: 'key' } },
            queueReceive: { method: 'POST', params: { path: 'api/trade/queueReceive', key: 'key' } },

            queryAccount: { method: 'POST', params: { path: 'api/trade/queryAccount' } },
            queryHistory: { method: 'POST', params: { path: 'api/trade/queryHistory', key: 'key' }, isArray: true },

            queryWaitingGivers: { method: 'POST', params: { path: 'api/trade/queryWaitingGivers' }, isArray:true },
            queryWaitingReceivers: { method: 'POST', params: { path: 'api/trade/queryWaitingReceivers' }, isArray: true },

            mapForReceiver: { method: 'POST', params: { path: 'api/trade/mapForReceiver' } },
        });
    }
]);