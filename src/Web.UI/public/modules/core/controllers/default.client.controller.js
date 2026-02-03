(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).controller("DefaultLayoutController",
        ["$scope", "$state", function ($scope, $state) {
            var vm = this;
            vm.state = $state;

            $scope.$on('update-title', function (e, data) {
                vm.title = data;
            });
        }]);

}());