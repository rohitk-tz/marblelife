(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("CalendarImportController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "modalParam", "FileService", "Toaster", "SchedulerService", "CalendarService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, modalParam, fileService, toaster, schedulerService, calendarService) {

            var vm = this;
            vm.franchisee = modalParam.Franchisee;
            vm.franchiseeId = modalParam.FranchiseeId;
            vm.calendarModel = {};
            vm.calendarModel.franchiseeId = modalParam.FranchiseeId;
            vm.isProcessing = false;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            vm.options = [];
            function prepareOptions() {
                vm.options.push({ display: 'Technician', value: 1 }),
                vm.options.push({ display: 'Sales Rep.', value: 0 });
            };

            vm.uploadFile = function (file) {
                vm.isProcessing = true;
                if (file != null) {
                    return fileService.upload(file).then(function (result) {
                        toaster.show("File uploaded.");
                        vm.calendarModel.file = result.data;
                        vm.isProcessing = false;
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }
            }

            function getTechList() {
                return schedulerService.getTechList(vm.franchiseeId).then(function (result) {
                    vm.techList = result.data;
                });
            }

            function getSalesRepList() {
                return schedulerService.getSalesRep(vm.franchiseeId).then(function (result) {
                    vm.salesRepList = result.data;
                });
            }

            function getTimeZoneInfo() {
                return calendarService.getTimeZoneInfo().then(function (result) {
                    vm.timeZoneList = result.data;
                });
            }

            vm.save = function () {
                if (vm.calendarModel.techId == null && vm.calendarModel.salesRepId == null) {
                    toaster.error("Please select an assignee first!");
                    return;
                }
                if (vm.calendarModel.file == null) {
                    toaster.error("Please upload a file!");
                    return;
                }

                if (vm.calendarModel.timeZoneId == null || vm.calendarModel.timeZoneId <= 0) {
                    toaster.error("Please Select appropriate timeZone first!");
                    return;
                }
                vm.isProcessing = true;
                return calendarService.saveCalendar(vm.calendarModel).then(function (result) {
                    if (result.message != null) {
                        vm.isProcessing = false;
                        if (!result.data)
                            toaster.error(result.message.message);
                        else
                            toaster.show(result.message.message);
                    }
                    vm.isProcessing = false;
                    $uibModalInstance.close(result);
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }

            $q.all(prepareOptions(), getTechList(), getSalesRepList(), getTimeZoneInfo());
        }]);
}());