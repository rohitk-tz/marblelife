(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).controller("HomeController", ["$scope", "$state", "$rootScope", function ($scope, $state, $rootScope) {
        var vm = this;
        vm.dashboardSrc = "";
        vm.Roles = DataHelper.Role;
        vm.isFrontOfficeExe = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;

        $scope.$on("identity", function (e, d) {
            if ($rootScope.identity != null) {
                setDashboard($rootScope.identity.roleAlias);
            }
        });

        if ($scope.identity != null) {
            setDashboard($scope.identity.roleAlias);
        }

        function setDashboard(roleAlias) {
            if (vm.isFrontOfficeExe) {
                $state.go('core.layout.scheduler.manage', { franchiseeId: $rootScope.identity.loggedInOrganizationId });
            }
            else
                vm.dashboardSrc = "/modules/core/views/dashboard-" + roleAlias + ".client.view.html";
        }
    }]);

}());