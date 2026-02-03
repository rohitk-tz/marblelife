(function () {

    angular.module(SalesConfiguration.moduleName).controller("UploadAnnualBatchController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "FranchiseeService",
            "BatchService", "FileService", "Toaster", "Clock", "Notification", "APP_CONFIG", "AnnualBatchService", "modalParam", "DashboardService",
    function ($scope, $rootScope, $state, $q, $uibModalInstance, franchiseeService,
        batchService, fileService, toaster, clock, notification, config, annualBatchService, modalParam, dashboardService) {

        var vm = this;
        vm.response = modalParam.Response;
        vm.batch = {};
        vm.batch.franchiseeId = vm.response != null ? vm.response.franchiseeId : 0;
        vm.batch.annualUploadStartDate = vm.response != null ? vm.response.uploadStartDate : null;
        vm.batch.annualUploadEndDate = vm.response != null ? vm.response.uploadEndDate : null;
        vm.Roles = DataHelper.Role;
        vm.SalesDataUploadStatus = DataHelper.SalesDataUploadStatus;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
        vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;//To do replace with lookup
        vm.getAnnualUploadResponse = getAnnualUploadResponse;
        vm.yearCollection = [];
        
        if (!vm.isSuperAdmin)
            vm.batch.franchiseeId = $rootScope.identity.organizationId;

        vm.save = function () {
            if (vm.batch.annualFile == null) {
                toaster.error("Please upload a file");
                return;
            }
            vm.batch.statusId = vm.SalesDataUploadStatus.Uploaded;
            saveBatch();

        };

        function getAnnualUploadResponse() {
            return dashboardService.getAnnualUploadResponse(vm.batch.franchiseeId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.response = result.data;
                    vm.batch.franchiseeId = vm.batch.franchiseeId;
                    vm.batch.annualUploadStartDate = vm.response.uploadStartDate;
                    vm.batch.annualUploadEndDate = vm.response.uploadEndDate;
                }
            });
        }


        function saveBatch() {
            if (vm.batch.annualUploadStartDate == null || vm.batch.annualUploadEndDate == null || vm.batch.franchiseeId == null) {
                return;
            }
            return annualBatchService.saveBatch(vm.batch).then(function (result) {
                if (result.data != null) {
                    if (!result.data) {
                        toaster.error(result.message.message);
                        //$uibModalInstance.close(result);
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
                    vm.batch.annualFile = result.data;
                });
            }
        }

        vm.close = function () {
            $uibModalInstance.dismiss();
        };


        $scope.$watch('vm.batch.franchiseeId', function (nv, ov) {
            if (nv == ov) return;
        });

        function getFranchiseeCollection() {
            return franchiseeService.getActiveFranchiseeList().then(function (result) {
                vm.franchiseeCollection = result.data;
            });
        }

        function getFranchiseeForMissingAudit() {
            return annualBatchService.getFranchiseeForMissingAudit().then(function (result) {
                vm.franchiseeForAudit = result.data;
            })
        }

        function fillCollection() {
            var index = 0;
            var year = new Date().getFullYear();
            for (var i = year - 1; i >= 2017; i--) {
                vm.yearCollection.push({ alias: '' + i, display: '' + i, id: '' + index, value: '' + i });
                ++index;
            }
        }

        $q.all([getFranchiseeCollection(), getFranchiseeForMissingAudit(), fillCollection()]);

    }]);
}());