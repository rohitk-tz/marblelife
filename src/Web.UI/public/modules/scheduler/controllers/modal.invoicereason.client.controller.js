(function () {
    angular.module(SchedulerConfiguration.moduleName).controller("InvoiceReasonModal",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "SchedulerService", "Toaster", "$filter",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, schedulerService, toaster, $filter) {
                var vm = this;
                vm.invoiceModel = {
                };
                vm.invoiceModel.schedulerId = modalParam.JobSchedulerId;
                vm.invoiceModel.IsInvoiceRequired = modalParam.IsInvoiceRequired;
                vm.modalHeader = modalParam.ModalHeader;
                vm.reason = modalParam.Reason;
                vm.cancel = function() {
                    $uibModalInstance.dismiss();
                    vm.invoiceModel.IsInvoiceRequired = true;
                    vm.invoiceModel.InvoiceReason = null;
                    schedulerService.saveInvoiceRequired(vm.invoiceModel).then(function (result) {
                        if (result.data != null) {
                            if (result.data) {
                                toaster.error("Reason for No Is Mandatory!");
                            }
                            else {
                                toaster.error("Error in changing status Of Invoice Required!!");
                            }
                        }
                    });
                };
                vm.saveReason = function () {
                    vm.invoiceModel.InvoiceReason = vm.invoiceReason;
                    schedulerService.saveInvoiceRequired(vm.invoiceModel).then(function (result) {
                        if (result.data != null) {
                            if (result.data) {
                                $uibModalInstance.dismiss();
                                toaster.show("Reason added successfully for No Invoice.");
                            }
                            else {
                                toaster.error("Error in changing status Of Invoice Required!!");
                            }
                        }
                    });
                }
                if (vm.modalHeader == "Add a Reason") {
                    vm.isAddReason = true;
                    vm.isShowReason = false;
                }
                else{
                    vm.isAddReason = false;
                    vm.isShowReason = true;
                }
                vm.close = function () {
                    $uibModalInstance.dismiss();
                }
            }
        ]
    );
}());