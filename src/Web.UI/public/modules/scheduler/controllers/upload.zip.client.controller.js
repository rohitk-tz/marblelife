(function () {

    angular.module(SchedulerConfiguration.moduleName).controller("UploadZipController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "FranchiseeService",
            "GeoCodeService", "FileService", "Toaster", "Clock", "Notification", "APP_CONFIG",
    function ($scope, $rootScope, $state, $q, $uibModalInstance, franchiseeService,
        geoCodeService, fileService, toaster, clock, notification, config) {

        var vm = this;
        vm.fileInfo = {};
        vm.feeProfile = {};
        vm.feeProfiles = DataHelper.FeeProfile;
        vm.currentYear = moment(clock.now(), "DD/MM/YYYY").year();
        vm.fileInfo.periodEndDate = null;
        vm.fileInfo.periodStartDate = null;
        vm.Roles = DataHelper.Role;
        vm.SalesDataUploadStatus = DataHelper.SalesDataUploadStatus;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin; //To do replace with lookup

        if (!vm.isSuperAdmin)
            vm.fileInfo.franchiseeId = $rootScope.identity.organizationId;

        vm.save = function () {
            if (vm.fileInfo.file == null) {
                toaster.error("Please upload a file");
                return;
            }
            vm.fileInfo.statusId = vm.SalesDataUploadStatus.Uploaded;
            saveFile();
        };

        function saveFile() {
            var tf = document.getElementById('notes_p');
            return geoCodeService.saveFile(vm.fileInfo).then(function (result) {
                if (result.data != null && result.data == false)
                    toaster.error(result.message.message);
                $uibModalInstance.close(result);
            });
        }

        vm.uploadFile = function (file) {
            if (file != null) {
                return fileService.uploadZip(file).then(function (result) {
                    toaster.show("File uploaded.");
                    vm.fileInfo.file = result.data;
                });
            }
        }

        vm.close = function () {
            $uibModalInstance.dismiss();
        };
    }]);
}());