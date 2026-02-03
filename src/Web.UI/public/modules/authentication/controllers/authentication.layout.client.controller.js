(function () {
    'use strict';

    angular.module(AuthenticationConfiguration.moduleName).controller("AuthenticationLayoutController", ["$scope", function ($scope)
    {
        var vm = this;
        vm.currentYear = (new Date()).getFullYear();

        //$scope.$on('$includeContentLoaded', function () {
            $("body").removeClass("login").addClass("login");
        //});

        $scope.$on('$destroy', function () {
            $("body").removeClass("login");
        });
        
    }]);

}());