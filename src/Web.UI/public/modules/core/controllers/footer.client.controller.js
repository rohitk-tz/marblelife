(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).controller("FooterController", ["$scope", function ($scope) {

        $scope.$on('$includeContentLoaded', function () {
            Layout.initFooter(); // init footer
        });

        var vm = this;
        vm.currentYear = (new Date()).getFullYear();

    }]);

}());