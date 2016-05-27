'use strict';
mainApp.controller('dlgChangeAvatarController', ['$scope', 'userService', 'FileUploader', '_', 'localStorageService', '$uibModalInstance', function ($scope, userService, FileUploader, _, localStorageService, $uibModalInstance) {
    $scope.uploadImage = function () {
        var selectedFile = _.last(uploader.queue);
        selectedFile.upload();        
    }

    /**
    * File Uploader
    */
    var uploader = $scope.uploader = new FileUploader({
        autoUpload: false,
        url: 'api/user/updateAvatar',        
    });

    var authData = localStorageService.get('authorizationData');
    if (authData) {
        uploader.headers["Authorization"] = 'Bearer ' + authData.token;
    }

    /**
    * Show preview of cropped image
    */
    uploader.onAfterAddingFile = function (item) {
        $scope.cropped = { image: '' };
        var reader = new FileReader();
        reader.onload = function (event) {
            $scope.$apply(function () {
                $scope.image = event.target.result;
            });
        };
        reader.readAsDataURL(item._file);
    };

    /**
     * Upload Blob (cropped image) instead of file.
     * @see
     *   https://developer.mozilla.org/en-US/docs/Web/API/FormData
     *   https://github.com/nervgh/angular-file-upload/issues/208
     */
    uploader.onBeforeUploadItem = function (item) {
        var blob = dataURItoBlob($scope.cropped.image);
        item._file = blob;
    };

    /**
     * Converts data uri to Blob. Necessary for uploading.
     * @see
     *   http://stackoverflow.com/questions/4998908/convert-data-uri-to-file-then-append-to-formdata
     * @param  {String} dataURI
     * @return {Blob}
     */
    var dataURItoBlob = function (dataURI) {
        // convert base64/URLEncoded data component to raw binary data held in a string
        var byteString;
        if (dataURI.split(',')[0].indexOf('base64') >= 0) {
            byteString = atob(dataURI.split(',')[1]);
        } else {
            byteString = decodeURI(dataURI.split(',')[1]);
        }
        var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];
        var array = [];
        for (var i = 0; i < byteString.length; i++) {
            array.push(byteString.charCodeAt(i));
        }
        return new Blob([new Uint8Array(array)], { type: mimeString });
    };

    uploader.onSuccessItem = function (item, res, status, headers) {
        console.log(res);
        $scope.ok(res)
    }

    $scope.ok = function (res) {
        $uibModalInstance.close(res);
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss();
    };
}]);