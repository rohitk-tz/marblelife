(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ShowPeformanceDetailsController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "MarketingLeadService",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, marketingLeadService) {

                var vm = this;
                vm.performanceFilter = {};
                vm.performanceFilter.franchiseeId = modalParam.franchiseeId;
                vm.franchiseeName = modalParam.FranchiseeName;
                vm.performanceFilter.categoryId = modalParam.categoryId;
                
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                function getPeformaniceHistry() {
                    return marketingLeadService.getPerformanceHistryCollection(vm.performanceFilter).then(function (result) {
                        if (result.data != null) {
                            vm.performanceHistry= result.data.leadPerformanceFranchiseeViewData;
                        }
                    });
                }
                getPeformaniceHistry();

            }]);
}());