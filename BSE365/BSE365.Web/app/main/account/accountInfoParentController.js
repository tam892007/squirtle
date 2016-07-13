
mainApp.controller('accountInfoParentController',
[
    '$scope', '$uibModal', '$uibModalInstance', 'accountService', 'targetData',
    function($scope, $uibModal, $uibModalInstance, accountService, targetData) {
        $scope.getData = function() {
            accountService.parentAccount({ id: targetData.parentId },
                { id: targetData.parentId },
                function(response) {
                    $scope.parent = response;
                });
        }

        $scope.close = function () {
            $uibModalInstance.dismiss('close');
        }

        $scope.init = function() {
            $scope.parent = {};
            $scope.getData();
        }

        $scope.init();
    }
]);