
/**
* A generic confirmation for risky actions.
* Usage: Add attributes: ng-confirm-message="Are you sure"? ng-confirm-click="takeAction()" function
*/
commonApp.directive('ngConfirmClick',
[
    function() {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {
                element.bind('click',
                    function() {
                        var message = attrs.ngConfirmMessage;
                        if (message && confirm(message)) {
                            scope.$apply(attrs.ngConfirmClick);
                        }
                    });
            }
        }
    }
]);

commonApp.directive('ngReallyClick',
[
    '$uibModal',
    function($uibModal) {

        var ModalInstanceCtrl = [
            '$scope', '$uibModalInstance', function($scope, $uibModalInstance) {
                $scope.ok = function() {
                    $uibModalInstance.close();
                };

                $scope.cancel = function() {
                    $uibModalInstance.dismiss('cancel');
                };
            }
        ];

        return {
            restrict: 'A',
            scope: {
                ngReallyClick: "&"
            },
            link: function(scope, element, attrs) {
                element.bind('click',
                    function() {
                        var message = attrs.ngReallyMessage || "Are you sure ?";

                        var modalHtml = '<div class="modal-body">' + message + '</div>';
                        modalHtml +=
                            '<div class="modal-footer"><button class="btn btn-primary" ng-click="ok()">OK</button><button class="btn btn-warning" ng-click="cancel()" >Close</button></div>';

                        var modalInstance = $uibModal.open({
                            template: modalHtml,
                            controller: ModalInstanceCtrl
                        });

                        modalInstance.result.then(function() {
                                scope.ngReallyClick();
                            },
                            function() {
                                //Modal dismissed
                            });
                    });
            }
        }
    }
]);