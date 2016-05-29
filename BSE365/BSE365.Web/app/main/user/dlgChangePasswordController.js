mainApp.controller('dlgChangePasswordController', ['$scope', 'userService', '$uibModalInstance', 'Notification', function ($scope, userService, $uibModalInstance, Notification) {
    $scope.interacted = function (field) {
        return $scope.submitted || field.$dirty;
    };
    
    $scope.init = function () {
        $scope.failed = false;
        $scope.submitted = false;
        $scope.model = {};
    }

    $scope.init();

    $scope.changePassword = function (model) {
        return userService.changePassword(model).$promise;
    }

    $scope.submit = function () {
        $scope.submitted = true;
        if (!$scope.frmPassword.$valid) return;
        $scope.changePassword($scope.model).then(function (res) {
            if (res.result) {                
                $scope.ok();
            }
            else {
                $scope.failed = true;
                Notification.error('Failed to update your password');
            }
        });
    }

    $scope.ok = function (res) {
        Notification.success('Password has been changed successfully');
        $uibModalInstance.close(res);
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss();
    };
}]);