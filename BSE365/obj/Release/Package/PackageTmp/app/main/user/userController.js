'use strict';
mainApp.controller('userController', ['$scope', 'userService', 'imageService', '$uibModal', function ($scope, userService, imageService, $uibModal) {

    $scope.getCurrentUserProfile = function () {
        return userService.getCurrentUserProfile().$promise;
    }

    $scope.init = function () {
        $scope.currentUser = $scope.currentUser || null;
        ////Get User Profile if needed
        if ($scope.currentUser == null) {
            $scope.getCurrentUserProfile().then(function (res) {
                $scope.currentUser = res;                
                $scope.currentUser.avatar.url = '/image/getUserPicture/' + $scope.currentUser.avatar.id;
            });
        }
    }

    $scope.init();

    $scope.updateAvatar = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: 'app/main/user/dlg-change-avatar.html',
            controller: 'dlgChangeAvatarController',
            size: 'lg',
            windowClass: 'portraitDialog',
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
        }, function () {
            console.info('Modal dismissed at: ' + new Date());
        });
    }
}]);