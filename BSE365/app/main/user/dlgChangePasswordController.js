mainApp.controller('dlgChangePasswordController', ['$scope', 'userService', '$uibModalInstance', function ($scope, userService, $uibModalInstance) {
    $scope.interacted = function (field) {
        return $scope.submitted || field.$dirty;
    };
    
    $scope.init = function () {
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
        });
    }

    $scope.ok = function (res) {
        $uibModalInstance.close(res);
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss();
    };
}]);