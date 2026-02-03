(function () {

    angular.module(SalesConfiguration.moduleName).controller("UpdateCustomerListController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "FranchiseeService",
            "BatchService", "FileService", "Toaster", "Clock", "Notification",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, franchiseeService,
            batchService, fileService, toaster, clock, notification) {

            var vm = this;
            vm.batch = {};

            vm.batch.periodEndDate = clock.now();
            vm.batch.periodStartDate = clock.now();
            vm.batch.isUpdate = true;

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

                return batchService.uploadCustomerList(vm.batch).then(function (result) {
                    if (result.data != null && result.data == false)
                        toaster.error(result.message.message);
                    $uibModalInstance.close(result);
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

            function getFranchiseeCollection() {
                return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                    vm.franchiseeCollection = result.data;
                });
            }

            vm.close = function () {
                $uibModalInstance.dismiss();
            };
            getFranchiseeCollection();

            $scope.$watch('vm.batch.franchiseeId', function (nv, ov) {
                if (nv == ov) return;

            });
        }]);
}());