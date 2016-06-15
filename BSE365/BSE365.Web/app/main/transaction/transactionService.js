mainApp.factory('transactionService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            history: { method: 'POST', params: { path: 'api/transaction/history', key: 'key' }, isArray: true },
            giveRequested: { method: 'POST', params: { path: 'api/transaction/giveRequested' }, isArray: true },
            receiveRequested: { method: 'POST', params: { path: 'api/transaction/receiveRequested' }, isArray: true },
            moneyTransfered: { method: 'POST', params: { path: 'api/transaction/moneyTransfered' } },
            moneyReceived: { method: 'POST', params: { path: 'api/transaction/moneyReceived' } },
            reportNotTransfer: { method: 'POST', params: { path: 'api/transaction/reportNotTransfer' } },
            updateImg: { method: 'POST', params: { path: 'api/transaction/updateImg' } },
            abandonTransaction: { method: 'POST', params: { path: 'api/transaction/abandonTransaction' } },
            queryTransaction: { method: 'POST', params: { path: 'api/transaction/queryTransaction' } },
            reportedTransactions: { method: 'POST', params: { path: 'api/transaction/reportedTransactions' } },
            applyReport: { method: 'POST', params: { path: 'api/transaction/applyReport' } },
            queryUserHistory: { method: 'POST', params: { path: 'api/transaction/queryUserHistory' } },
            transactionDetails: { method: 'POST', params: { path: 'api/transaction/transactionDetails', key: 'key' } },
        });
    }
]);