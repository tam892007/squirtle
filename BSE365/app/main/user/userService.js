mainApp.factory('userService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            getCurrentUserProfile: { method: 'GET', params: { path: 'api/user/getCurrent' } },
            //get: { method: 'GET', params: { path: 'api/user/getCurrent', id: 'id' } },
            //filter: { method: 'POST', params: { path: 'api/group/filter', filter: 'filter' } },
            //create: { method: 'POST', params: { path: 'api/group/create', groupVM: 'groupVM' } },
            //update: { method: 'POST', params: { path: 'api/group/update', groupVM: 'groupVM' } },
            //remove: { method: 'POST', params: { path: 'api/group/delete', groupVM: 'groupVM' } },
        });
    }
]);