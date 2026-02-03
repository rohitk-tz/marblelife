(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("FranchsieeRegistrationHistryController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "MarketingLeadService",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, marketingLeadService) {

                var vm = this;
                vm.listValues = [];
                vm.franchiseeName = modalParam.FranchiseeName;
                vm.listValues = modalParam.list;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

            }]);
}());