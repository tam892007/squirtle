mainApp.factory('userinfoService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            queryInfo: { method: 'POST', params: { path: 'api/userinfo/queryInfo' } },
            waitingInfomations: { method: 'POST', params: { path: 'api/userinfo/waitingInfomations', userPrefix: 'userPrefix' } },
            queryTransaction: { method: 'POST', params: { path: 'api/userinfo/queryTransaction' } },
            updateUserInfo: { method: 'POST', params: { path: 'api/userinfo/updateUserInfo' } },
            checkBankNumber: { method: 'POST', params: { path: 'api/userinfo/checkBankNumber', number: '@number', userid: '@userid' } },
        });
    }
]);