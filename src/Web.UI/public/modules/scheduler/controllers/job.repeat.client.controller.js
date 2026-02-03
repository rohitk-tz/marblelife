(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("JobRepeatController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "EstimateService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "modalParam", "FileService", "$filter", "CustomerService", "SchedulerService",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, estimateService, franchiseeService,
                notification, clock, toaster, addressService, modalParam, fileService, $filter, customerService, schedulerService) {

                var vm = this;
                var isValueUpdated = false;
                vm.jobInfo = modalParam.JobInfo;
                vm.techIds = modalParam.TechIds;
                vm.invoiceInfo = modalParam.InvoiceInfo;
                vm.convertToJob = modalParam.ConvertToJob;
                vm.editJob = modalParam.EditJob;
                vm.isFromConvertToJob = modalParam.IsFromConvertToJob;
                vm.validateModel = validateModel;
                vm.invoiceList = [];
                vm.assigneeIds = [];
                vm.copiedCollection = [];
                $scope.dateOptions = {
                    showWeeks: false
                };
                var currentDate = moment(clock.now());
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                $scope.$on('isUpdated', function (event, data) {
                    var isTechSelected = true;
                    if (data == "true") {
                        isValueUpdated = true;
                    }
                });

                vm.update = function () {
                    var isTechSelected = true;
                    var assigneeNotSelected = 0;
                    var invoiceNotSelected = 0;
                    var schedulerWithSameTech = 0;
                    angular.forEach(vm.occurenceInfo.collection, function (item) {
                        item.actualStartDateString = moment((item.startDate)).format("MM/DD/YYYY HH:mm");
                        item.actualEndDateString = moment((item.endDate)).format("MM/DD/YYYY HH:mm");
                        vm.isValid = true;
                        if (item.assigneeId == null || item.assigneeId <= 0) {
                            assigneeNotSelected += 1
                        }
                        if (item.invoiceNumber.length <= 0 && vm.invoiceInfo != null) {
                            invoiceNotSelected += 1
                        }
                    })
                    if (assigneeNotSelected == 0 && invoiceNotSelected == 0 && vm.occurenceInfo.collection.length > 0) {
                        $uibModalInstance.close(vm.occurenceInfo.collection);
                    }
                    else if (assigneeNotSelected != 0 && invoiceNotSelected != 0 && vm.invoiceInfo != null) {
                        notification.showAlert("Please Select Assignee and the Invoice!");
                        vm.isProcessing = false;
                        vm.isValid = false;
                        isTechSelected = false;
                        return;
                    }
                    else if (assigneeNotSelected != 0) {
                        notification.showAlert("Please Select Assignee!");
                        vm.isProcessing = false;
                        vm.isValid = false;
                        isTechSelected = false;
                        return;
                    }
                    else if (invoiceNotSelected != 0 && vm.invoiceInfo != null) {
                        notification.showAlert("Please Select Invoice!");
                        vm.isProcessing = false;
                        vm.isValid = false;
                        isTechSelected = false;
                        return;
                    }
                }

                function getOccurenceInfo() {
                    return estimateService.getOccurenceInfo(vm.jobInfo.jobId).then(function (result) {
                        if (vm.jobInfo.jobOccurence != undefined) {
                            var collection = vm.jobInfo.jobOccurence.collection;
                        }
                        vm.occurenceInfo = result.data;
                        if (result.data.collection.length == 0 && collection != undefined) {
                            vm.occurenceInfo.collection = collection;
                        }
                        angular.forEach(vm.occurenceInfo.collection, function (item, index) {
                            item.startDate = moment((item.actualStartDateString)).format("MM/DD/YYYY HH:mm");
                            item.endDate = moment((item.actualEndDateString)).format("MM/DD/YYYY HH:mm");
                            if (item.invoiceNumber.length == 0 && vm.isFromConvertToJob) {
                                item.isNew = true;
                            }
                            else {
                                item.isNew = false;
                            }
                            item.checkSchedule = false;
                            if (vm.techIds != undefined) {
                                var index1 = vm.techIds.indexOf($filter('filter')(vm.techIds, item.assigneeId, true)[0]);
                                if (index1 == -1) {
                                    vm.occurenceInfo.collection.splice(index, 1);
                                }
                            }
                            vm.assigneeIds.push({ id: item.assigneeId });
                        });
                        if (vm.techIds != undefined) {
                            if ((vm.jobInfo.jobId == null || vm.jobInfo.jobId <= 0) && vm.techIds.length > 0) {
                                angular.forEach(vm.techIds, function (item) {
                                    if (item.invoiceNumber == undefined) {
                                        item.invoiceNumber = [];
                                    }
                                    var index = vm.occurenceInfo.collection.indexOf($filter('filter')(vm.occurenceInfo.collection, item.id, true)[0]);
                                    if (index == -1) {
                                        vm.occurenceInfo.collection.push({ startDate: vm.jobInfo.startDate, endDate: vm.jobInfo.endDate, invoiceNumber: item.invoiceNumber, assigneeId: item.id, isNew: true, checkSchedule: true });
                                    }
                                })
                            }
                            if ((vm.jobInfo.jobId > 0) && vm.editJob && vm.techIds.length > 0) {
                                angular.forEach(vm.techIds, function (item) {
                                    var index = vm.occurenceInfo.collection.indexOf($filter('filter')(vm.occurenceInfo.collection, item.id, true)[0]);
                                    if (item.invoiceNumber == undefined) {
                                        item.invoiceNumber = [];
                                    }
                                    if (index == -1) {
                                        vm.occurenceInfo.collection.push({ startDate: vm.jobInfo.startDate, endDate: vm.jobInfo.endDate, invoiceNumber: item.invoiceNumber, assigneeId: item.id, isNew: true, checkSchedule: true });
                                    }
                                })
                            }
                        }
                        if (vm.occurenceInfo.franchiseeId > 0) {
                            getHolidayList();
                        }
                        angular.forEach(vm.occurenceInfo.collection, function (item, index) {
                            if (item.invoiceNumber == undefined) {
                                item.invoiceNumber = [];
                            }
                            if (item.invoiceNumber.length == 0) {
                                angular.forEach(vm.invoiceList, function (invoice, indexInvoice) {
                                    item.invoiceNumber.push({ id: invoice.id, label: invoice.label });
                                });
                            }
                        });
                        vm.copiedCollection = angular.copy(vm.occurenceInfo.collection);
                    });
                }

                function getTechList() {
                    return schedulerService.getTechList(vm.jobInfo.franchiseeId).then(function (result) {
                        vm.techList = result.data;
                    });
                }

                function getInvoiceList() {
                    vm.invoiceNumbers = [];
                    if (vm.invoiceInfo != null) {
                        var startDate = moment((vm.jobInfo.startDate)).format("MM/DD/YYYY HH:mm");
                        angular.forEach(vm.invoiceInfo.serviceList, function (value1) {
                            var index = vm.invoiceNumbers.indexOf($filter('filter')(vm.invoiceNumbers, value1.invoiceNumber, true)[0]);
                            var customerSplittedName = vm.invoiceInfo.customerName.split(' ');
                            var locationJoined = "";
                            var fileName = "";
                            angular.forEach(customerSplittedName.reverse(), function (custName) {
                                fileName += custName + "_";
                            });
                            if (value1.serviceType == "CONCRETE-COATINGS" || value1.serviceType == "ENDURACRETE") {
                                fileName = fileName + "_InternalConcreteOrder";
                            }
                            else {
                                fileName = fileName + "_InternalOrder";
                            }
                            if (value1.locationIds.length > 0) {
                                var locationsName = value1.locationIds;
                                if (locationsName.length >= 2) {
                                    locationJoined = "_" + locationsName[0].id + "_" + locationsName[1].id;
                                }
                                else {
                                    locationJoined = "_" + locationsName[0].id;
                                }
                                fileName = fileName + locationJoined;
                            }
                            fileName = fileName + " (Invoice " + value1.invoiceNumber + "_" + startDate.substr(0, 10) + ")";
                            if (index == -1) {
                                vm.invoiceNumbers.push(value1.invoiceNumber);
                                vm.invoiceList.push({ id: parseInt(value1.invoiceNumber), label: fileName });
                            }
                        });
                    }
                    else {
                        vm.invoiceNumbers = [];
                        vm.invoiceList = [];
                    }
                    vm.invoiceList = $filter('orderBy')(vm.invoiceList, 'id', true);
                }

                function validateModel(form) {
                    vm.isProcessing = true;
                    var assigneeNotSelected = 0;
                    var invoiceNotSelected = 0;
                    var currentDateToCompare = moment(currentDate).format('MM/DD/YYYY');
                    vm.isValid = false;
                    if (vm.occurenceInfo.collection <= 0)
                        return;
                    angular.forEach(vm.occurenceInfo.collection, function (item) {
                        item.actualStartDateString = moment((item.startDate)).format("MM/DD/YYYY HH:mm");
                        item.actualEndDateString = moment((item.endDate)).format("MM/DD/YYYY HH:mm");
                        vm.isValid = true;
                        if (item.assigneeId == null || item.assigneeId <= 0) {
                            assigneeNotSelected += 1
                        }
                        if (item.invoiceNumber.length <= 0 && vm.invoiceInfo != null) {
                            invoiceNotSelected += 1
                        }
                        if (item.startDate == null || item.endDate == null) {
                            notification.showAlert("Please enter Start/End Time!");
                            vm.isProcessing = false;
                            vm.isValid = false;
                            return;
                        }
                        var startDateToCompare = moment(item.startDate).format('MM/DD/YYYY');
                        var endDateToCompare = moment(item.endDate).format('MM/DD/YYYY');
                        var isHoliday = false;
                        if (item.endDate <= item.startDate) {
                            notification.showAlert("End Time should be greater Than Start Time!");
                            item.startDate == null;
                            item.endDate == null;
                            vm.isValid = false;
                            vm.isProcessing = false;
                            return;
                        }
                    })
                    if (assigneeNotSelected != 0 && invoiceNotSelected != 0 && vm.invoiceInfo != null) {
                        notification.showAlert("Please Select Assignee and the Invoice!");
                        vm.isProcessing = false;
                        vm.isValid = false;
                        return;
                    }
                    else if (assigneeNotSelected != 0) {
                        notification.showAlert("Please Select Assignee!");
                        vm.isProcessing = false;
                        vm.isValid = false;
                        return;
                    }
                    else if (invoiceNotSelected != 0 && vm.invoiceInfo != null) {
                        notification.showAlert("Please Select Invoice!");
                        vm.isProcessing = false;
                        vm.isValid = false;
                        return;
                    }
                    else {
                        vm.isValid = true;
                    }
                    if (vm.isValid)
                        save(form);
                }

                function save(form) {
                    var model = {
                        scheduleAvailabilityFilter: []
                    };
                    angular.forEach(vm.occurenceInfo.collection, function (item) {
                        var index = vm.copiedCollection.indexOf($filter('filter')(vm.copiedCollection, item.scheduleId, true)[0]);

                        item.actualStartDateString = moment((item.startDate)).format("MM/DD/YYYY HH:mm");
                        item.actualEndDateString = moment((item.endDate)).format("MM/DD/YYYY HH:mm");
                        if (index > -1) {
                            if (item.actualEndDateString == vm.copiedCollection[index].endDate
                                && item.actualStartDateString == vm.copiedCollection[index].startDate
                                && item.assigneeId == vm.copiedCollection[index].assigneeId) {
                                item.checkSchedule = false;
                            }
                            else {
                                item.checkSchedule = true;
                            }
                        }
                        vm.isValid = true;
                        if (item.checkSchedule) {
                            model.scheduleAvailabilityFilter.push({ jobId: vm.jobInfo.jobId, assigneeId: item.assigneeId, startDate: item.startDate, endDate: item.endDate });
                        }
                    });
                    schedulerService.checkAvailabilityList(model).then(function (result) {
                        if (!result.data.isAvailable) {
                            vm.isNotAvailable = true;
                            toaster.error(result.data.assigneeNames + " is not available , try scheduling after 15 mins.");
                            vm.isProcessing = false;
                        }
                        else {
                            vm.occurenceInfo.schedulerId = vm.jobInfo.id;
                            return estimateService.saveSchedule(vm.occurenceInfo).then(function (result) {
                                if (result.data != null) {
                                    if (!result.data) {
                                        toaster.error(result.message.message);
                                    }
                                    else {
                                        toaster.show(result.message.message);
                                        $uibModalInstance.close(vm.occurenceInfo.collection);
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

                $q.all([getOccurenceInfo(), getTechList(), getInvoiceList()]);
            }]);
}());
