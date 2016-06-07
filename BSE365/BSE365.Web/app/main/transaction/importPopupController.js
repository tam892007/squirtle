
mainApp.controller('importPopupController',
[
    '$scope', '$window', '$uibModal', '$log', '$uibModalInstance', '_', 'FileUploader', 'cfpLoadingBar',
    'localStorageService',
    'targetData',
    function($scope,
        $window,
        $uibModal,
        $log,
        $uibModalInstance,
        _,
        FileUploader,
        cfpLoadingBar,
        localStorageService,
        targetData) {
        $scope.target = {};
        $scope.data = [];

        $scope.uploader = new FileUploader({
            url: targetData.uploadLink,
        });

        $scope.upload = function() {
            $scope.isError = false;
            $scope.uploaded = false;
            $scope.importing = true;
            cfpLoadingBar.start();

            var selectedFile = _.last($scope.uploader.queue);
            selectedFile.upload();
        };

        $scope.ok = function(response) {
            console.log(response);
            var url = response.url;
            url = url.replace('~/', '');
            $uibModalInstance.close(url);
        };

        $scope.cancel = function() {
            $uibModalInstance.dismiss('cancel');
        };

        $scope.uploaded = false;
        $scope.isError = false;
        $scope.importing = false;

        // PROPERTIES

        $scope.uploader.queueLimit = 2;

        // FILTERS

        //// by max size (5MB)
        $scope.uploader.filters.push({
            'name': "size",
            'fn': function(item) {
                return item.size <= 5 * 1048576; // 5* 1024 * 1024 | Math.pow(2,20); | 0x100000
            }
        });

        $scope.uploader.filters.push({
            'name': "image",
            'fn': function(item) {
                return !$scope.uploader.hasHTML5 ? true : /\/(png|jpeg|jpg)$/.test(item.file.type);
            }
        });

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            $scope.uploader.headers["Authorization"] = 'Bearer ' + authData.token;
        }

        // CALLBACKS

        $scope.uploader.onAfterAddingFile = function(fileItem) {
            if ($scope.uploader.getIndexOfItem(fileItem)) {
                $scope.uploader.removeFromQueue(0);
            };
        };

        $scope.uploader.onErrorItem = function(fileItem, response, status, headers) {
            $scope.isError = true;
            $scope.importing = false;
            cfpLoadingBar.complete();
            var modalInstance = $uibModal.open({
                templateUrl: 'app/main/transaction/errorDlgHandled.html',
                size: 'lg',
                controller: 'errorDlgController',
                resolve: {
                    rejection: function() {
                        return {
                            data: response,
                            statusText: 'File is invalid. See techinical details for more information'
                        };
                    },
                },
            });
        };

        $scope.uploader.onSuccessItem = function (fileItem, response, status, headers) {
            $scope.uploaded = true;
            $scope.importing = false;
            cfpLoadingBar.complete();
            $scope.ok(response);
        };

    }
]);