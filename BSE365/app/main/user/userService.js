mainApp.factory('userService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            getCurrentUserProfile: { method: 'GET', params: { path: 'api/user/getCurrent' } },
            getCurrentUserPinInfo: { method: 'GET', params: { path: 'api/user/getCurrentPin' } },
            //get: { method: 'GET', params: { path: 'api/user/getCurrent', id: 'id' } },
            register: { method: 'POST', params: { path: 'api/account/register', registerVM: 'registerVM' } },
            //create: { method: 'POST', params: { path: 'api/group/create', groupVM: 'groupVM' } },
            //update: { method: 'POST', params: { path: 'api/group/update', groupVM: 'groupVM' } },
            //remove: { method: 'POST', params: { path: 'api/group/delete', groupVM: 'groupVM' } },
        });
    }
]);