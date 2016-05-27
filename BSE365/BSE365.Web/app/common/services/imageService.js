commonApp.factory('imageService', ['$resource',
    function ($resource) {
        return $resource(':path', {}, {            
            getUserPic: { method: 'GET', params: { path: 'api/image/getUserPicture', id: '@id' } },
        });
    }
]);