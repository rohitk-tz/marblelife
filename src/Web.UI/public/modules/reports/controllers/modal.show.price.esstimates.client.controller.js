(function () {
    angular.module(SchedulerConfiguration.moduleName).controller("ShowPriceEstimatesModal",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "modalParam", "Toaster", "ManagePriceEstimateService",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, modalParam, toaster, managePriceEstimateService) {
                var vm = this;
                vm.serviceTagId = modalParam.ServiceTagId;
                vm.query = {
                    serviceTagId: 0
                }
                vm.close = function() {
                    $uibModalInstance.dismiss();
                };
                
                function getPriceEstimate() {
                    vm.query.serviceTagId = vm.serviceTagId;
                    vm.query.showAllFranchisee = false;
                    return managePriceEstimateService.getPriceEstimate(vm.query).then(function (result) {
                        vm.priceEstimate = result.data;                    
                        vm.hasTwoPriceColumns = vm.priceEstimate.hasTwoPriceColumns;
                        vm.priceEstimateServices = [];
                        angular.forEach(vm.priceEstimate.priceEstimateServices, function (value, key) {
                            if (value.franchiseePrice != null
                                || value.franchiseeAdditionalPrice != null
                                || value.corporatePrice != null
                                || value.corporateAdditionalPrice != null
                            ){
                                vm.priceEstimateServices.push(value);
                            }
                        });
                        vm.scrollClass = vm.priceEstimateServices.length > 10 ? "customScroll-s" : "";
                    });
                }



                $q.all([getPriceEstimate()]);
            }
        ]
    );
}());