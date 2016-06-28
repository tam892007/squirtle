'use strict';
mainApp.controller('userTreeController', ['$scope', 'userService', '_', function ($scope, userService, _) {
    $scope.getCurrentUserProfile = function () {
        return userService.getCurrentUserProfile().$promise;
    }

    $scope.getUserChildren = function (id) {
        return userService.getChildren({ id: id }).$promise;
    }

    $scope.init = function () {
        $scope.data = [];
        $scope.loadData();
    }

    $scope.loadData = function () {
        $scope.getCurrentUserProfile().then(function (res) {
            $scope.data.push({ userId: res.id, displayName: res.displayName, userName: res.userName });
            $scope.loadDataForSingleNode($scope.data[0]);
        });
    }

    $scope.loadDataForSingleNode = function(node) {        
        if (node.isLoaded) return;
        node.isLoaded = true;  ////Load only one time

        $scope.getUserChildren(node.userId).then(function (res) {        
            node.nodes = res;

            ////for root node
            if (!node.numberOfChildren) {
                node.numberOfChildren = _.reduce(node.nodes, function (memo, child) { return memo + child.numberOfChildren; }, 0);
            }
        });
    }

    $scope.init();    
}]);