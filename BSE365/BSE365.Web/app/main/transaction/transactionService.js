mainApp.factory('transactionService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            history: { method: 'POST', params: { path: 'api/transaction/history' }, isArray: true },
            giveRequested: { method: 'POST', params: { path: 'api/transaction/giveRequested' }, isArray: true },
            receiveRequested: { method: 'POST', params: { path: 'api/transaction/receiveRequested' }, isArray: true },
            moneyTransfered: { method: 'POST', params: { path: 'api/transaction/moneyTransfered' } },
            moneyReceived: { method: 'POST', params: { path: 'api/transaction/moneyReceived' } },
            reportNotTransfer: { method: 'POST', params: { path: 'api/transaction/reportNotTransfer' } },
        });
    }
]);