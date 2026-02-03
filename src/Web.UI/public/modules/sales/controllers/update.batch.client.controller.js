(function () {

    angular.module(SalesConfiguration.moduleName).controller("UpdateBatchController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "FranchiseeService",
            "BatchService", "FileService", "Toaster", "Clock", "Notification", "modalParam",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, franchiseeService,
            batchService, fileService, toaster, clock, notification, modalParam) {

            var vm = this;
            vm.batch = modalParam.Batch;
            vm.batch.isUpdate = true;
            vm.Roles = DataHelper.Role;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;//1; //To do replace with lookup

            vm.save = function () {

                if (vm.batch.file == null) {
                    toaster.show("Please upload a file");
                    return;
                }

                updateBatch();

            };

            function updateBatch() {
                return batchService.updateBatch(vm.batch).then(function (result) {
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

            vm.close = function () {
                $uibModalInstance.dismiss();
            };


            $scope.$watch('vm.batch.franchiseeId', function (nv, ov) {
                if (nv == ov) return;

                $q.all([]);

            });

        }]);
}());