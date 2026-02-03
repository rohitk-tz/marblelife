(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("EstimateRepeatController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "EstimateService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "modalParam", "FileService", "$filter", "CustomerService", "SchedulerService",
    function ($scope, $rootScope, $state, $q, $uibModalInstance, estimateService, franchiseeService,
        notification, clock, toaster, addressService, modalParam, fileService, $filter, customerService, schedulerService) {

        var vm = this;
        var isValueUpdated = false;
        vm.jobInfo = modalParam.JobInfo;
        vm.techIds = modalParam.TechIds;
        vm.validateModel = validateModel;
        $scope.dateOptions = {
            showWeeks: false
        };
        var currentDate = moment(clock.now());
        vm.close = function () {
            $uibModalInstance.dismiss();
        };
        $scope.$on('isUpdated', function (event, data) {
            if (data == "true") {
                isValueUpdated = true;
            }
        });
        vm.update = function () {
            if (isValueUpdated == true) {
                $uibModalInstance.close(vm.occurenceInfo.collection);
            }
            else {
                notification.showAlert("Estimate not changed!");
            }
        }

        function getOccurenceInfo() {
            return estimateService.getEstimateOccurenceInfo(vm.jobInfo.id).then(function (result) {
                vm.occurenceInfo = result.data;
                angular.forEach(vm.occurenceInfo.collection, function (item) {
                    item.startDate = moment((item.actualStartDateString)).format("MM/DD/YYYY HH:mm");
                    item.endDate = moment((item.actualEndDateString)).format("MM/DD/YYYY HH:mm");
                    item.isNew = false;
                })
                if ((vm.jobInfo.jobId == null || vm.jobInfo.jobId <= 0) && vm.techIds.length > 0) {
                    angular.forEach(vm.techIds, function (item) {
                        vm.occurenceInfo.collection.push({ startDate: vm.jobInfo.startDate, endDate: vm.jobInfo.endDate });
                    })
                }
                if (vm.occurenceInfo.franchiseeId > 0) {
                    getHolidayList();
                }
            });
        }

        function getTechList() {            
            return schedulerService.GetTechAndSalesListForEstimate(vm.jobInfo.franchiseeId).then(function (result) {
                vm.techList = result.data;
            });
        }

        function validateModel(form) {
            vm.isProcessing = true;
            var currentDateToCompare = moment(currentDate).format('MM/DD/YYYY');
            vm.isValid = false;
            if (vm.occurenceInfo.collection <= 0)
                return;

            angular.forEach(vm.occurenceInfo.collection, function (item) {
                vm.isValid = true;
                if (item.startDate == null || item.endDate == null) {
                    notification.showAlert("Please enter Start/End Time!");
                    vm.isProcessing = false;
                    vm.isValid = false;
                    return;
                }

                if (item.assigneeId == null || item.assigneeId <= 0) {
                    notification.showAlert("Please Select Assignee!");
                    vm.isProcessing = false;
                    vm.isValid = false;
                    return;
                }

                var startDateToCompare = moment(item.startDate).format('MM/DD/YYYY');
                var endDateToCompare = moment(item.endDate).format('MM/DD/YYYY');
                var isHoliday = false;

                angular.forEach(vm.holidayList, function (item) {
                    var holidayDate = moment(item.actualStartDate).format('MM/DD/YYYY');
                    if (!isHoliday) {
                        if (holidayDate == startDateToCompare || holidayDate == endDateToCompare) {
                            notification.showAlert("Can't create Job/Estimates On holiday(s)!");
                            isHoliday = true;
                            vm.isValid = false;
                            vm.isProcessing = false;
                            return;
                        }
                    }
                    return;
                });

                if (isHoliday)
                    return;

                //if (item.startDate < currentDate || item.endDate < currentDate) {
                //    if ((currentDateToCompare > startDateToCompare || currentDateToCompare > endDateToCompare)) {
                //        notification.showAlert("Can't assign for of past Time!");
                //        item.startDate == null;
                //        item.endDate == null;
                //        vm.isValid = false;
                //        vm.isProcessing = false;
                //        return;
                //    }
                //}

                if (item.endDate <= item.startDate) {
                    notification.showAlert("End Time should be greater Than Start Time!");
                    item.startDate == null;
                    item.endDate == null;
                    vm.isValid = false;
                    vm.isProcessing = false;
                    return;
                }

                vm.isValid = true;
            });

            if (vm.isValid)
                save(form);
        }

        function save(form) {
            var model = {
                scheduleAvailabilityFilter: []
            };
            angular.forEach(vm.occurenceInfo.collection, function (item) {
                item.actualStartDateString = moment((item.startDate)).format("MM/DD/YYYY HH:mm");
                item.actualEndDateString = moment((item.endDate)).format("MM/DD/YYYY HH:mm");
                vm.isValid = true;
                //if (item.isNew) {
                //    model.scheduleAvailabilityFilter.push({ jobId: vm.jobInfo.id, assigneeId: item.assigneeId, startDate: item.startDate, endDate: item.endDate });
                //}
                model.scheduleAvailabilityFilter.push({ jobId: vm.jobInfo.id, assigneeId: item.assigneeId, startDate: item.startDate, endDate: item.endDate });
            });
            schedulerService.checkAvailabilityList(model).then(function (result) {
                if (!result.data.isAvailable) {
                    vm.isNotAvailable = true;
                    toaster.error(result.data.assigneeNames + " is not available , try scheduling after 15 mins.");
                    vm.isProcessing = false;
                }
                else {
                    return estimateService.repeatEstimate(vm.occurenceInfo).then(function (result) {
                        if (result.data != null) {
                            if (!result.data) {
                                toaster.error(result.message.message);
                            }
                            else {
                                toaster.show(result.message.message);
                                $uibModalInstance.close();
                            }
                            vm.isProcessing = false;
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }
            });           
        }


        function getHolidayList() {
            schedulerService.getHolidayList(vm.occurenceInfo.franchiseeId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.holidayList = result.data.collection;
                }
            });
        }

        $q.all([getOccurenceInfo(), getTechList()]);
    }]);
}());
