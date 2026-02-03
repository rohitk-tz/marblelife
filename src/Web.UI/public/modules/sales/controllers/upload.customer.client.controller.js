(function () {
    angular.module(SalesConfiguration.moduleName).controller("UploadCustomerController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "FileService", "Toaster", "Clock", "Notification", "CustomerService",
    function ($scope, $rootScope, $state, $q, $uibModalInstance, fileService, toaster, clock, notification, customerService) {

        var vm = this;
        vm.customer = {};
        vm.Roles = DataHelper.Role;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
        vm.SalesDataUploadStatus = DataHelper.SalesDataUploadStatus;

        vm.close = function () {
            $uibModalInstance.dismiss();
        };

        vm.save = function () {
            if (vm.customer.file == null) {
                toaster.show("Please upload a file");
                return;
            }
            vm.customer.statusId = vm.SalesDataUploadStatus.Uploaded;// 71; //To do replace with with lookup
            saveCustomerFile();
        };

        function saveCustomerFile() {
            return customerService.saveCustomerFile(vm.customer).then(function (result) {
                if (result.data != null && result.data == false)
                    toaster.error(result.message.message);
                if (result.data == true)
                    toaster.show(result.message.message);
                $uibModalInstance.close(result);
            });
        }

        vm.uploadFile = function (file) {
            if (file != null) {
                return fileService.upload(file).then(function (result) {
                    toaster.show("File uploaded.");
                    vm.customer.file = result.data;
                });
            }
        }

    }]);
}());