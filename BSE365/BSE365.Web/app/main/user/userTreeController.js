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
            $scope.data.push({ id: res.id, title: res.displayName });
        });
    }

    $scope.loadDataForSingleNode = function(node) {        
        if (node.isLoaded) return;
        node.isLoaded = true;  ////Load only one time

        $scope.getUserChildren(node.id).then(function (res) {
            node.nodes = _.map(res, function (user) { return { id: user.id, title: user.displayName } })
        });
    }

    $scope.init();    
}]);