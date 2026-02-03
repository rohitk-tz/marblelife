(function () {

    angular.module(SalesConfiguration.moduleName).controller("ShowGeoCodeLogsController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "$stateParams", "$q", "modalParam", "FileService",
            function ($scope, $rootScope, $state, $uibModalInstance, $stateParams, $q, modalParam, fileService) {

                var vm = this;
                vm.getGeneralLog = getGeneralLog;
                vm.getLogForCounty = getLogForCounty
                vm.getLogForZip = getLogForZip;
                vm.logFileId = modalParam.LogFileId == null ? 0 : modalParam.LogFileId;
                vm.logForCountyFileId = modalParam.LogForCountyFileId == null ? 0 : modalParam.LogForCountyFileId;
                vm.logForZipFileId = modalParam.LogForZipFileId == null ? 0 : modalParam.LogForZipFileId;
                vm.changeTab = changeTab;
                vm.isGenernalTab = true;
                vm.isCountyTab = false;
                vm.isZipCodeTab = false;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                function getLogForCounty() {
                    if (vm.logForCountyFileId == null || vm.logForCountyFileId == 0) {
                        vm.logs = "No Records Found."
                        return;
                    }

                    return fileService.getFile(vm.logForCountyFileId).then(function (result) {
                        vm.logs = result.data;
                    });
                }
                function getLogForZip() {
                    if (vm.logForZipFileId == null || vm.logForZipFileId == 0) {
                        vm.logs="No Records Found."
                        return;
                    }
                    return fileService.getFile(vm.logForZipFileId).then(function (result) {
                        vm.logs = result.data;
                    });
                }
                function getGeneralLog() {
                    if (vm.logFileId == null || vm.logFileId == 0) {
                        vm.logs = "No Records Found."
                        return;
                    }
                    return fileService.getFile(vm.logFileId).then(function (result) {
                        vm.logs = result.data;
                    });

                }

                function changeTab(tabIndex) {
                    if (tabIndex == 1) {
                        vm.isGenernalTab = true;
                        vm.isCountyTab = false;
                        vm.isZipCodeTab = false;
                        getGeneralLog();
                    }
                    else if (tabIndex == 2) {
                        vm.isGenernalTab = false;
                        vm.isCountyTab = true;
                        vm.isZipCodeTab = false;
                        getLogForCounty();
                    }
                    else if (tabIndex == 3) {
                        vm.isGenernalTab = false;
                        vm.isCountyTab = false;
                        vm.isZipCodeTab = true;
                        getLogForZip();
                    }
                }

                $q.all([getGeneralLog()]);
            }]);
}());