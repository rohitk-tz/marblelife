(function () {
    angular.module(SchedulerConfiguration.moduleName).controller("BulkPriceEstimatesModal",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "Toaster","ManagePriceEstimateService",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, toaster, managePriceEstimateService) {
                var vm = this;
                vm.isBulkCorporatePrice = modalParam.IsBulkCorporatePrice;
                vm.selectedFranchisees = modalParam.SelectedFranchisees;
                vm.selectedServices = modalParam.SelectedServices;
                vm.hasTwoPriceColumns = modalParam.HasTwoPriceColumns;
                vm.disableCoorporatePrice = modalParam.DisableCoorporatePrice;
                vm.disableFranchiseePrice = modalParam.DisableFranchiseePrice;
                vm.priceEstimateServices = [];
                vm.close = function() {
                    $uibModalInstance.dismiss();
                };
                vm.save = save;
                vm.query = {
                    corporatePrice: 0,
                    corporateAdditionalPrice: 0,
                    franchiseePrice: 0,
                    franchiseeAdditionalPrice: 0,
                    alternativeSoution: '',
                    serviceTagId: vm.selectedServices,
                    franchiseeId: vm.selectedFranchisees
                }
                vm.saveBulkCorporatePrice = {
                    serviceTagId: [],
                    bulkCorporatePrice: 0
                }
                function save() {
                    if (vm.isBulkCorporatePrice) {
                        if (vm.bulkCorporatePrice != undefined) {
                            vm.saveBulkCorporatePrice = {
                                serviceTagId: [],
                                bulkCorporatePrice: 0,
                                bulkCorporateAdditionalPrice: 0
                            }
                            vm.saveBulkCorporatePrice.bulkCorporatePrice = vm.bulkCorporatePrice;
                            vm.saveBulkCorporatePrice.bulkCorporateAdditionalPrice = vm.bulkCorporateAdditionalPrice;
                            vm.saveBulkCorporatePrice.serviceTagId = vm.selectedServices;
                            return managePriceEstimateService.bulkUpdateCorporatePrice(vm.saveBulkCorporatePrice).then(function (result) {
                                if (result != null && result.data != null) {
                                    if (result.data) {
                                        toaster.show("Changes done successfully!!");
                                        $uibModalInstance.dismiss();
                                    }
                                    else {
                                        toaster.error("Error in Saving Changes!!");
                                    }
                                }
                            });
                        }
                        else {
                            toaster.error("Please Enter Price!!");
                            return;
                        }
                    }
                    else {
                        if (vm.franchiseePrice != undefined
                            || vm.franchiseeAdditionalPrice != undefined
                            || vm.corporatePrice != undefined
                            || vm.corporateAdditionalPrice != undefined
                        ) {

                            vm.query.corporatePrice = vm.corporatePrice;
                            vm.query.corporateAdditionalPrice = vm.corporateAdditionalPrice;
                            vm.query.franchiseePrice = vm.franchiseePrice;
                            vm.query.franchiseeAdditionalPrice = vm.franchiseeAdditionalPrice;
                            vm.query.alternativeSolution = vm.alternativeSolution;
                            return managePriceEstimateService.bulkUpdatePriceEstimate(vm.query).then(function (result) {
                                if (result != null && result.data != null) {
                                    if (result.data) {
                                        toaster.show("Changes done successfully!!");
                                        $uibModalInstance.dismiss();
                                    }
                                    else {
                                        toaster.error("Error in Saving Changes!!");
                                    }
                                }
                            });
                        }
                        else {
                            toaster.error("Please Enter Price!!");
                            return;
                        }
                    }
                    
                }
            }
        ]
    );
}());