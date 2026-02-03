(function () {
    'use strict';
    angular.module(CoreConfiguration.moduleName).controller("ManageFranchiseeController",
        ["$scope", "$rootScope", "$state", "$stateParams", "$q", "$uibModalInstance", "modalParam", "FranchiseeService", "UserAuthenticationService",
        function ($scope, $rootScope, $state, $stateParams, $q, $uibModalInstance, modalParam, franchiseeService, userAuthenticationService) {

            var vm = this;
            vm.getFranchiseeCollection = getFranchiseeCollection;
            vm.manageFranchisee = manageFranchisee;
            vm.userId = $rootScope.identity.userId;
            vm.Roles = DataHelper.Role;
            vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
            vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getFranchiseeCollection() {
                return franchiseeService.getFranchiseeListForLogin(vm.userId).then(function (result) {
                    vm.franchiseeCollection = result.data.collection;
                });
            }

            function manageFranchisee(franchiseeId) {
                vm.model = { franchiseeId: franchiseeId, userId: vm.userId };
                if (vm.isFranchiseeAdmin)
                    vm.model.currentFranchiseeId = $rootScope.identity.organizationId;
                else if (vm.isExecutive) {
                    if ($stateParams != null && $stateParams.franchiseeId > 1)
                        vm.model.currentFranchiseeId = $stateParams.franchiseeId;
                    else
                        vm.model.currentFranchiseeId = $rootScope.identity.loggedInOrganizationId;
                }

                return franchiseeService.manageFranchisee(vm.model).then(function (result) {
                    if (result.data) {
                        if (vm.isExecutive) {
                            var url = $state.href('core.layout.scheduler.manage', { franchiseeId: franchiseeId });
                            window.open(url, '_blank');
                        }
                        else
                            userAuthenticationService.getUserSessionId();
                    }
                    //getFranchiseeCollection();
                    $uibModalInstance.close();
                });
            }

            $q.all([getFranchiseeCollection()]);
        }]);
}());