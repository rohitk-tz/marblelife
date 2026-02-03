(function () {

    angular.module(SalesConfiguration.moduleName).controller("UpdateCustomerInvoiceController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "AnnualBatchService",
             "Toaster", "Clock", "Notification", "modalParam", "LocalStorageService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, annualBatchService,
            toaster, clock, notification, modalParam, LocalStorageService) {

            var vm = this;
            vm.batch = {};

            vm.batch.periodEndDate = clock.now();
            vm.batch.periodStartDate = clock.now();
            vm.AddressModel = modalParam.AddressModel == null ? 0 : modalParam.AddressModel;
            vm.statusId = modalParam.StatusId == null ? 1 : modalParam.StatusId;
            vm.SalesDataUploadStatus = DataHelper.SalesDataUploadStatus;
            vm.customer = vm.AddressModel;
            vm.customer.statusId = vm.statusId;
            if (vm.statusId == '1')
            {
                vm.AddressModel.newAddressLine1 = vm.AddressModel.oldAddressLine1;
                vm.AddressModel.newAddressLine2 = vm.AddressModel.oldAddressLine2;
                vm.AddressModel.newCity = vm.AddressModel.oldCity;
                vm.AddressModel.newCountry = vm.AddressModel.oldCountry;
                vm.AddressModel.newState = vm.AddressModel.oldState;
                vm.customer.newZip = vm.AddressModel.oldZip;
                vm.customer.newphoneNumber = vm.customer.oldphoneNumber;
                vm.customer.newemailId = vm.customer.oldemailId
            }
            //vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin; //To do replace with lookup
            vm.save = save;

            function save() {
                return annualBatchService.updateCustomerInvoice(vm.customer).then(function (result) {
                    if (result.data != null) {
                        if (!result.data) {
                            toaster.error(result.message.message);
                        }
                        else {
                            toaster.show(result.message.message);
                            $uibModalInstance.close(result);
                        }
                    }
                });
            }

            vm.close = function () {
                $uibModalInstance.dismiss();
            };
        }]);
}());