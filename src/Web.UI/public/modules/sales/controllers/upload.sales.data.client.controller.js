(function () {

    angular.module(SalesConfiguration.moduleName).controller("UploadSalesDataController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "FranchiseeService",
            "SalesInvoiceService", "FileService", "Toaster", "Clock", "Notification",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, franchiseeService,
            salesInvoiceService, fileService, toaster, clock, notification) {

            var vm = this;
            vm.batch = {};

            vm.batch.periodEndDate = clock.now();
            vm.batch.periodStartDate = clock.now();

            vm.Roles = DataHelper.Role;
            vm.SalesDataUploadStatus = DataHelper.SalesDataUploadStatus;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin; //To do replace with lookup
            vm.save = save;

            function save() {

                if (vm.batch.file == null) {
                    toaster.show("Please upload a file");
                    return;
                }

                vm.batch.statusId = vm.SalesDataUploadStatus.Uploaded;

                return salesInvoiceService.uploadSalesFile(vm.batch).then(function (result) {
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

            vm.uploadFile = function (file) {
                if (file != null) {
                    return fileService.upload(file).then(function (result) {
                        toaster.show("File uploaded.");
                        vm.batch.file = result.data;
                    });
                }
            }

            vm.close = function () {
                $uibModalInstance.dismiss();
            };
        }]);
}());