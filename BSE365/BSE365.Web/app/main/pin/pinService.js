mainApp.factory('pinService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            transfer: { method: 'POST', params: { path: 'api/pin/transfer', transactionVM: 'transactionVM' } },
            getCurrentUserHistory: { method: 'POST', params: { path: 'api/pin/getAll' } },
        });
    }
]);