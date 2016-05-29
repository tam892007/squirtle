mainApp.factory('userService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {
            getCurrentUserContext: { method: 'GET', params: { path: 'api/user/getCurrentUserContext' } },
            getCurrentUserProfile: { method: 'GET', params: { path: 'api/user/getCurrent' } },
            getCurrentUserPinInfo: { method: 'GET', params: { path: 'api/user/getCurrentPin' } },
            getChildren: { method: 'GET', params: { path: 'api/user/getChildren', id: 'id' }, isArray : true },
            register: { method: 'POST', params: { path: 'api/account/register', registerVM: 'registerVM' }, isArray: true },
            updateCurrentUserProfile: { method: 'POST', params: { path: 'api/user/updateCurrent', userProfileVM: 'userProfileVM' } },
            updateAvatar: { method: 'POST', params: { path: 'api/user/updateAvatar', avatar: 'avatar' } },
            changePassword: { method: 'POST', params: { path: 'api/user/changePassword', model: 'model' } },
            checkName: { method: 'GET', params: { path: 'api/user/checkName', name: '@name' } },
        });
    }
]);