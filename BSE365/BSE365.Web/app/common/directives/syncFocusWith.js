commonApp.directive('syncFocusWith', ['$timeout', function ($timeout) {
    return {
        restrict: 'A',
        scope: {
            focusValue: "=syncFocusWith"
        },
        link: function ($scope, $element, attrs) {
            $scope.$watch("focusValue", function (currentValue, previousValue) {
                if (currentValue == true) {
                    $element[0].focus();
                } else  {
                    $element[0].blur();
                }
            })
        }
    }
}]);