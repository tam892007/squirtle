mainApp.factory('userService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            getCurrentUserProfile: { method: 'GET', params: { path: 'api/user/getCurrent' } },
            getCurrentUserPinInfo: { method: 'GET', params: { path: 'api/user/getCurrentPin' } },
            getChildren: { method: 'GET', params: { path: 'api/user/getChildren', id: 'id' }, isArray : true },
            register: { method: 'POST', params: { path: 'api/account/register', registerVM: 'registerVM' } },
            updateCurrentUserProfile: { method: 'POST', params: { path: 'api/user/updateCurrent', userProfileVM: 'userProfileVM' } },
            updateAvatar: { method: 'POST', params: { path: 'api/user/updateAvatar', avatar: 'avatar' } },
        });
    }
]);