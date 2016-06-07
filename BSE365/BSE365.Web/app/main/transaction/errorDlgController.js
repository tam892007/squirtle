'use strict';

mainApp.controller('errorDlgController',
[
    '$scope', '$uibModalInstance', 'rejection',
    function($scope, $uibModalInstance, rejection) {
        $scope.cancel = function() {
            $uibModalInstance.close();
        };

        $scope.rejection = rejection;
        $scope.isCollapsed = false;
        $scope.isHtml = false;
        $scope.isJson = false;

        function init() {
            if (rejection && rejection.data) {
                $scope.isJson = angular.isObject(rejection.data);
            }
        }

        init();
    }
]);