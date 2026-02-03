(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("CreateJobController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "modalParam", "FileService", "$filter", "CustomerService",
            "$uibModal", "APP_CONFIG", "GeoCodeService", "$window",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, addressService, modalParam, fileService, $filter, customerService, $uibModal,
                config, geoCodeService, $window) {

                var vm = this;
                vm.changeZipCode = changeZipCode;
                vm.isMexico = false;
                vm.isRepeated = false;
                vm.isFromOpen = false;
                vm.isFromCheckBox = false;
                vm.techCount = 0;
                vm.isVisible = false;
                vm.isTechIdsChanges = false;
                vm.croseVisible = false;
                vm.isFromLoad = true;
                vm.isFromLoadCountry = true;
                vm.isDateTimeChanged = false;
                vm.isDescriptionChanged = false;
                vm.startDateClick = startDateClick;
                vm.descriptionChange = descriptionChange;
                vm.getCustomerInfo = getCustomerInfo;
                vm.openDropDOwn = openDropDOwn;
                vm.endDateClick = endDateClick;
                vm.closePopUp = closePopUp
                var defaultClassTypeId = angular.copy(config.defaultClassTypeId);
                vm.jobStartDateDate = modalParam.Date;
                vm.query = modalParam.Query;
                vm.jobId = modalParam.JobId != null ? modalParam.JobId : 0;
                vm.isCopy = modalParam.IsCopy != null ? modalParam.IsCopy : false;
                vm.estimateId = modalParam.EstimateId != null ? modalParam.EstimateId : 0;
                vm.estimateData = modalParam.EstimateInfo != null ? modalParam.EstimateInfo : null;
                vm.estimateInvoiceInfo = modalParam.EstimateInvoiceInfo != null ? modalParam.EstimateInvoiceInfo : null;
                vm.convertToJob = modalParam.ConvertToJob != null ? modalParam.ConvertToJob : false;
                vm.isFromConvertToJob = modalParam.IsFromConvertToJob != null ? modalParam.IsFromConvertToJob : false;
                vm.editJob = modalParam.EditJob != null ? modalParam.EditJob : false;
                if (vm.convertToJob || vm.isFromConvertToJob) {
                    vm.repeatButtonName = "Configure Time & Invoice(s)*";
                    vm.repeatNote = "Click to update time & invoices for Technician(s).";
                }
                else {
                    vm.repeatButtonName = "Repeat Time";
                    vm.repeatNote = "Click to update time for Technician(s).";
                }
                vm.stateCode = null;
                vm.isMobile = navigator.userAgent.match(/(iPhone|iPod|iPad|Android|webOS|BlackBerry|IEMobile|Opera Mini)/i);
                var val = 0;
                vm.isOnLoad = true;
                $scope.dateOptions = {
                    showWeeks: false
                };
                
                vm.validTech = true;
                vm.jobInfo = {};
                var currentDate = moment(clock.now());
                vm.validateModel = validateModel;
                vm.calendarView = true;
                vm.techIds = [];
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isSalesRep = $rootScope.identity.roleId == vm.Roles.SalesRep;
                vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.repeatJob = repeatJob;
                vm.Hours = [{ display: "1", value: "1" }, { display: "2", value: "2" }, { display: "3", value: "3" }, { display: "4", value: "4" },
                { display: "5", value: "5" }, { display: "6", value: "6" }, { display: "7", value: "7" }, { display: "8", value: "8" },
                { display: "9", value: "9" }, { display: "10", value: "10" }, { display: "11", value: "11" }, { display: "12", value: "12" }];
                vm.Minutes = [{ display: "00", value: "001" }, { display: "15", value: "15" }, { display: "30", value: "30" }, { display: "45", value: "45" }];
                vm.Time = [{ display: "PM", value: "PM" }, { display: "AM", value: "AM" }];
                vm.openZipCodeModal = openZipCodeModal;
                vm.isEstimateSave = false;
                vm.openValidationModel = openValidationModel;
                vm.close = function () {
                    openValidationModel();
                    $rootScope.$on("IsEstimateSaved", function (evt, data) {
                        vm.isEstimateSave = data;
                        if (vm.isEstimateSave == true) {
                            vm.isEstimateSave = false;
                            $uibModalInstance.dismiss();
                        }
                        else {
                            return;
                        }
                    });

                };

                function repeatJob() {
                    var startDateMintues = vm.jobInfo.startDateMintues;
                    var endDateMintues = vm.jobInfo.endDateMintues;
                    if (vm.isCopy) {
                        vm.jobInfo.jobId = 0;
                    }
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/job-repeat.client.view.html',
                        controller: 'JobRepeatController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    JobInfo: vm.jobInfo,
                                    TechIds: vm.techIds,
                                    InvoiceInfo: vm.estimateInvoiceInfo,
                                    ConvertToJob: modalParam.ConvertToJob != null ? modalParam.ConvertToJob : false,
                                    IsFromConvertToJob: vm.isFromConvertToJob,
                                    EditJob: vm.editJob
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function (result) {
                        if (result.length > 0) {
                            vm.techIds = [];
                            angular.forEach(result, function (value) {
                                if (value.assigneeId > 0) {
                                    angular.forEach(vm.techIds, function (techId) {
                                        if (techId.id == value.assigneeId) {
                                            var index = vm.techIds.indexOf(techId.id);
                                            vm.techIds.splice(index, 1);
                                        }
                                    })
                                    vm.techIds.push({ id: value.assigneeId, invoiceNumber: value.invoiceNumber });
                                }
                            });
                            vm.jobInfo.jobOccurence = {};
                            vm.jobInfo.jobOccurence.franchiseeId = vm.jobInfo.franchiseeId;
                            vm.jobInfo.jobOccurence.collection = result;
                        }
                    });
                }

                if (!vm.isSuperAdmin & !vm.isExecutive) {
                    if (vm.query != null) {
                        vm.query.franchiseeId = $rootScope.identity.organizationId.toString();
                    }
                    vm.jobInfo.franchiseeId = $rootScope.identity.organizationId.toString();
                    vm.franchisee = $rootScope.identity.organizationName;
                }
                if (vm.isExecutive) {
                    vm.jobInfo.franchiseeId = vm.query != null ? vm.query.franchiseeId.toString() : 0;
                }
                vm.getList = getList;
                function getList() {
                    vm.techIds = [];
                    getTechList();
                    getSalesRepList();
                    getInvoiceDetails();
                };
                $scope.settings = {
                    scrollable: true,
                    enableSearch: true,
                    selectedToTop: true,
                    buttonClasses: 'btn btn-primary leader_btn',
                };
                $scope.translationTexts = {
                    checkAll: 'Select All',
                    uncheckAll: 'Deselect All',
                    selectGroup: 'Select All',
                    buttonDefaultText: "Assign Technician(s)",
                    dynamicButtonTextSuffix: 'Selected'
                };
                $scope.selectEvents = {
                    onSelectionChanged: function () {
                        vm.isFromCheckBox = true;
                        if (vm.techIds.length > 0) {
                            angular.forEach(vm.techIds, function (value) {
                                if (vm.alreadyExistingTechIds != undefined) {
                                    var index = vm.alreadyExistingTechIds.indexOf($filter('filter')(vm.alreadyExistingTechIds, value.id, true)[0]);
                                    if (index == -1 && vm.isFromConvertToJob) {
                                        vm.isShowRepeatChangeTime = true;
                                    }
                                    else {
                                        vm.isShowRepeatChangeTime = false;
                                    }
                                }
                                validateSchedule(value);
                            });
                        }
                    }
                }

                function validateSchedule(item) {
                    vm.error = '';
                    if (item == null || item.id <= 0)
                        return;
                    vm.tech = $filter('filter')(vm.techList, { id: item.id }, true);
                    vm.techName = vm.tech.length > 0 ? vm.tech[0].label : null;
                    var startDate = $filter('date')(vm.jobInfo.startDate, 'yyyy-MM-dd');
                    var startDateTimeFormat = new Date((startDate));
                    var endDate = $filter('date')(vm.jobInfo.endDate, 'yyyy-MM-dd');
                    var endDateTimeFormat = new Date((endDate));
                    var model = { jobId: vm.jobInfo.jobId, assigneeId: item.id, startDate: startDateTimeFormat, endDate: endDateTimeFormat };
                    schedulerService.checkAvailability(model).then(function (result) {
                        if (!result.data) {
                            vm.isNotAvailable = true;
                            vm.error = vm.techName + " is not available , try scheduling after 15 mins.";
                            if (vm.techIds.length > 1) {
                                var index = vm.techIds.indexOf(item.id);
                                vm.techIds.splice(index, 1);
                            }
                            else
                                vm.techIds = [];
                        }
                    });
                }

                function fillModel() {
                    if (vm.jobInfo.jobId <= 0 && vm.convertToJob && vm.estimateId > 0 && vm.estimateData != null) {
                        vm.jobInfo.title = vm.estimateData.title;
                        vm.jobInfo.franchiseeId = vm.estimateData.franchiseeId.toString();
                        vm.jobInfo.startDate = moment(clock.now()).toDate();
                        vm.jobInfo.endDate = moment(moment(clock.now()).format("L")).add(17, 'hours');
                        vm.jobInfo.salesRepId = vm.estimateData.salesRepId.toString();
                        vm.jobInfo.jobCustomer = vm.estimateData.jobCustomer;
                        vm.jobInfo.jobCustomer.address = [vm.estimateData.jobCustomer.address];
                        vm.jobInfo.estimateId = vm.estimateId;
                        vm.jobInfo.description = vm.estimateData.description;
                        vm.jobInfo.jobTypeId = vm.estimateData.jobTypeId != null ? vm.estimateData.jobTypeId.toString() : null;
                        vm.jobInfo.geoCode = vm.estimateData.geoCode;
                        vm.jobInfo.startDate = vm.estimateData.startDate;
                        vm.jobInfo.endDate = vm.estimateData.endDate;
                        vm.convertToJob = false;

                        getList();

                        var startDate = moment((vm.jobInfo.startDate)).format("MM/DD/YYYY h:mm A");
                        getStartDateTimeInDropDownFormat(startDate);

                        var endDate = moment((vm.jobInfo.endDate)).format("MM/DD/YYYY h:mm A");
                        getEndDateTimeInDropDownFormat(endDate);

                    }
                    else {
                        if (vm.jobInfo.jobId <= 0 && vm.query != null) {
                            vm.jobInfo.fileList = [];
                            vm.jobInfo.techIds = [];
                            if (vm.query.franchiseeId > 0)
                                vm.jobInfo.franchiseeId = vm.query.franchiseeId.toString();
                            if (vm.query.jobTypeId > 0)
                                vm.jobInfo.jobTypeId = vm.query.jobTypeId;
                            if (vm.jobInfo.jobTypeId <= 0)
                                vm.jobInfo.jobTypeId = defaultClassTypeId.toString();
                            if (vm.jobStartDateDate != null && vm.query.view == "month") {
                                vm.jobInfo.startDate = moment(moment(vm.jobStartDateDate).format("L")).add(8, 'hours');
                            } else {
                                vm.jobInfo.startDate = moment(vm.jobStartDateDate).toDate();
                            }
                            vm.jobInfo.endDate = moment(moment(vm.jobStartDateDate).format("L")).add(17, 'hours');

                            var startDate = moment((vm.jobInfo.startDate)).format("MM/DD/YYYY h:mm A");
                            getStartDateTimeInDropDownFormat(startDate);

                            var endDate = moment((vm.jobInfo.endDate)).format("MM/DD/YYYY h:mm A");
                            getEndDateTimeInDropDownFormat(endDate);
                        }

                        if (vm.jobInfo.jobId > 0) {
                            vm.techIds = [];
                            vm.jobInfo.jobCustomer.address = [vm.jobInfo.jobCustomer.address];

                            vm.jobInfo.startDate = moment(vm.jobInfo.startDate).toDate();
                            vm.jobInfo.endDate = moment(vm.jobInfo.endDate).toDate();
                        }
                        if (!vm.isSuperAdmin && !vm.isExecutive) {
                            vm.jobInfo.franchiseeId = $rootScope.identity.organizationId.toString();
                        }
                        getList();
                        if (vm.jobInfo.jobId <= 0 && vm.query != null) {
                            if (vm.query.techId > 0)
                                vm.techIds.push({ id: vm.query.techId });
                        }
                        if (vm.jobInfo.jobId > 0) {
                            if (vm.jobInfo.techIds != null && vm.jobInfo.techIds.length > 0) {
                                angular.forEach(vm.jobInfo.techIds, function (value) {
                                    var index = vm.jobInfo.jobSchedulerList.indexOf($filter('filter')(vm.jobInfo.jobSchedulerList, value, true)[0]);
                                    if (index != -1) {
                                        vm.techIds.push({ id: value, invoiceNumber: vm.jobInfo.jobSchedulerList[index].invoiceNumbers });
                                    }
                                });
                            }
                        }
                        if (vm.isSalesRep) {
                            vm.jobInfo.salesRepId = $rootScope.identity.organizationRoleUserId.toString();
                        }
                        vm.alreadyExistingTechIds = angular.copy(vm.techIds);
                    }
                }

                $scope.$on('clearDates', function (event) {
                    vm.jobInfo.startDate = moment(vm.jobStartDateDate).toDate();
                    vm.jobInfo.endDate = moment(vm.jobStartDateDate).toDate();
                });

                function validateModel(form) {
                    vm.isProcessing = true;
                    vm.jobInfo.techIds = [];
                    vm.jobInfo.invoiceAssignedIds = [];
                    var isHoliday = false;
                    var currentDates = new Date();
                    var dateBeforeTwoMonth = new Date(currentDates.setMonth(currentDates.getMonth() - 2));
                    if (vm.jobInfo.setGeoCode && (vm.jobInfo.geoCode == null || vm.jobInfo.geoCode == '')) {
                        notification.showAlert("Please Provide Geo Code!");
                        vm.isProcessing = false;
                        return;
                    }
                    //if (vm.jobInfo.jobCustomer.email == null || vm.jobInfo.jobCustomer.email == "") {
                    //    notification.showAlert("Please Enter Email");
                    //    vm.isProcessing = false;
                    //    return;
                    //}
                    if (vm.jobInfo.jobCustomer.phoneNumber == null || vm.jobInfo.jobCustomer.phoneNumber == "") {
                        notification.showAlert("Please Enter Phone Number");
                        vm.isProcessing = false;
                        return;
                    }
                    if (vm.jobInfo.jobCustomer.phoneNumber != null && vm.jobInfo.jobCustomer.phoneNumber != "") {
                        var phoneN = vm.jobInfo.jobCustomer.phoneNumber.replace(/_/g, "");
                        if (phoneN == null || phoneN.trim().length < 1 || phoneN.length < 10) {
                            notification.showAlert("Please Enter a valid Phone Number");
                            vm.isProcessing = false;
                            return;
                        }
                    }
                    if (vm.jobInfo.jobCustomer.address != null && vm.jobInfo.jobCustomer.address[0] != null) {
                        if (vm.jobInfo.jobCustomer.address[0].addressLine1 == null || vm.jobInfo.jobCustomer.address[0].addressLine1 == ""
                            || vm.jobInfo.jobCustomer.address[0].zipCode == null || vm.jobInfo.jobCustomer.address[0].zipCode == ""
                            || vm.jobInfo.jobCustomer.address[0].countryId == null) {
                            notification.showAlert("Please fill complete address!");
                            vm.isProcessing = false;
                            return;
                        }
                    }
                    vm.address = addressService.sanitizeAddress(vm.jobInfo.jobCustomer.address);
                    if (addressService.checkAddressInComplete(vm.jobInfo.jobCustomer.address)) {
                        notification.showAlert("Please fill complete address!");
                        vm.isProcessing = false;
                        return;
                    }

                    //var startDate = $filter('date')(vm.jobInfo.startDate, 'yyyy-MM-dd h:mm A');
                    var startDateTimeFormat = moment((vm.jobInfo.startDate));

                    var endTime = vm.jobInfo.endDateHours + ": " + vm.jobInfo.endDateMintues + " " + vm.jobInfo.endDateTime;
                    var endDate = $filter('date')(vm.jobInfo.endDate, 'yyyy-MM-dd');
                    var endDateTime = endDate + ' ' + endTime;
                    var endDateTimeFormat = moment((vm.jobInfo.endDate));
                    var currentDateLocal = moment(clock.now());
                    var currentDateToCompare = moment(currentDateLocal);
                    var startDateToCompare = moment(vm.jobInfo.startDate);
                    var endDateToCompare = new Date(vm.jobInfo.endDate);
                    if (vm.jobInfo.startDate == null || vm.jobInfo.endDate == null) {
                        notification.showAlert("Please enter Start/End Time!");
                        vm.isProcessing = false;
                        return;
                    }
                    if ((startDateTimeFormat < currentDateToCompare || endDateTimeFormat < currentDateToCompare) &&
                        ((dateBeforeTwoMonth >= startDateTimeFormat) && dateBeforeTwoMonth >= endDateToCompare)) {
                        if (currentDateToCompare != startDateToCompare || currentDateToCompare != endDateToCompare) {
                            if (vm.jobId <= 0) {
                                notification.showAlert("Job should not be of past Time!");
                                var startDate = moment((vm.jobInfo.startDate)).format("MM/DD/YYYY h:mm A");
                                var endDate = moment((vm.jobInfo.endDate)).format("MM/DD/YYYY h:mm A");
                                getStartDateTimeInDropDownFormat(startDate);
                                getEndDateTimeInDropDownFormat(endDate);
                                vm.jobInfo.startDate == null;
                                vm.jobInfo.endDate == null;
                                $scope.$broadcast("reset-dates");
                                vm.isProcessing = false;
                                return;
                            }
                            else {
                                if (!vm.isDateTimeChanged) {
                                    var daysDiff = moment(vm.jobInfo.actualEndDateString).diff(moment(currentDate), 'days');
                                    if (daysDiff < -2 && vm.isDescriptionChanged) {
                                        notification.showAlert("Job should not be of past Time!");
                                        var startDate = moment((vm.jobInfo.startDate)).format("MM/DD/YYYY h:mm A");
                                        var endDate = moment((vm.jobInfo.endDate)).format("MM/DD/YYYY h:mm A");
                                        getStartDateTimeInDropDownFormat(startDate);
                                        getEndDateTimeInDropDownFormat(endDate);
                                        vm.jobInfo.startDate == null;
                                        vm.jobInfo.endDate == null;
                                        $scope.$broadcast("reset-dates");
                                        vm.isProcessing = false;
                                        return;
                                    }
                                }
                                else {
                                    notification.showAlert("Job should not be of past Time!");
                                    var startDate = moment((vm.jobInfo.startDate)).format("MM/DD/YYYY h:mm A");
                                    var endDate = moment((vm.jobInfo.endDate)).format("MM/DD/YYYY h:mm A");
                                    getStartDateTimeInDropDownFormat(startDate);
                                    getEndDateTimeInDropDownFormat(endDate);
                                    vm.jobInfo.startDate == null;
                                    vm.jobInfo.endDate == null;
                                    $scope.$broadcast("reset-dates");
                                    vm.isProcessing = false;
                                    return;
                                }
                            }
                        }
                    }
                    if (endDateTimeFormat <= startDateTimeFormat) {
                        notification.showAlert("End Time should be greater Than Start Time!");
                        vm.jobInfo.startDate == null;
                        vm.jobInfo.endDate == null;
                        $scope.$broadcast("reset-dates");
                        vm.isProcessing = false;
                        return;
                    }
                    if (vm.techIds.length > 0 && (vm.convertToJob || vm.isShowRepeatChangeTime)) {
                        var invoiceNotAssigned = 0;
                        angular.forEach(vm.techIds, function (value) {
                            vm.jobInfo.techIds.push(value.id);
                            if ((value.invoiceNumber == undefined && value.invoiceNumber == null) || value.invoiceNumber.length == 0) {
                                invoiceNotAssigned += 1;
                            }
                        });
                        if (invoiceNotAssigned != 0) {
                            notification.showAlert("Please select invoice(s) by clicking on the Configure Time & Invoice(s) button.");
                            vm.isProcessing = false;
                            return;
                        }
                        else {
                            angular.forEach(vm.techIds, function (value) {
                                vm.jobInfo.invoiceAssignedIds.push({ assigneeId: value.id, invoiceIds: value.invoiceNumber });
                            });
                        }
                    }
                    else if (vm.convertToJob || vm.isShowRepeatChangeTime) {
                        notification.showAlert("Please select Technician(s) and then Configure Time & Invoice(s) for different Technician(s).");
                        vm.isProcessing = false;
                        return;
                    }
                    
                    if (vm.techIds.length > 0 && !vm.convertToJob) {
                        angular.forEach(vm.techIds, function (value) {
                            vm.jobInfo.techIds.push(value.id);
                        });
                    }
                    else {
                        notification.showAlert("Please select Technician(s).");
                        vm.isProcessing = false;
                        return;
                    }
                    vm.jobInfo.jobCustomer.address = vm.address[0];
                    save(form);
                }

                function save(form) {
                    var startDate = $filter('date')(vm.jobInfo.startDate, 'yyyy-MM-dd');
                    var startDateTimeFormat = moment((startDate));

                    var endDate = $filter('date')(vm.jobInfo.endDate, 'yyyy-MM-dd');
                    var endDateTimeFormat = moment((endDate));


                    if (startDateTimeFormat == 'invalidDate') {
                        notification.showAlert("Invalid Start Date!");
                        return;
                    }

                    if (endDateTimeFormat == 'invalidDate') {
                        notification.showAlert("Invalid End Date!");
                        return;
                    }

                    vm.jobInfo.actualStartDateString = moment((vm.jobInfo.startDate)).format("MM/DD/YYYY HH:mm");
                    vm.jobInfo.actualEndDateString = moment((vm.jobInfo.endDate)).format("MM/DD/YYYY HH:mm");
                    if (vm.isCopy) {
                        vm.jobInfo.id = 0;
                        vm.jobInfo.jobId = 0;
                        vm.jobInfo.jobSchedulerList = null;
                        vm.jobInfo.jobCustomer.customerId = 0;
                    }

                    return schedulerService.saveJob(vm.jobInfo).then(function (result) {
                        if (result.data != null) {
                            if (!result.data) {
                                var startDate = moment((vm.jobInfo.startDate)).format("MM/DD/YYYY h:mm A");
                                var endDate = moment((vm.jobInfo.endDate)).format("MM/DD/YYYY h:mm A");
                                getStartDateTimeInDropDownFormat(startDate);
                                getEndDateTimeInDropDownFormat(endDate);
                                toaster.error(result.message.message);
                                vm.jobInfo.jobCustomer.address = [vm.jobInfo.jobCustomer.address];
                            }
                            else {
                                toaster.show(result.message.message);
                                $rootScope.$broadcast('navigationDate', vm.jobInfo.startDate);
                                $uibModalInstance.close();
                            }
                            vm.isProcessing = false;
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                function resetModel(form) {
                    vm.jobInfo = {};
                    form.$setUntouched();
                    form.$setPristine();
                }

                function getmarketingClassCollection() {
                    return customerService.getmarketingClassCollection().then(function (result) {
                        vm.marketingClass = result.data;
                    });
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                        vm.franchisee = $filter('filter')(vm.franchiseeCollection, { value: vm.jobInfo.franchiseeId }, true);
                        vm.franchiseeName = vm.franchisee.length > 0 ? vm.franchisee[0].display : null;
                        vm.franchiseeId = vm.franchisee[0].id;

                    });
                }

                vm.uploadFile = function (file) {
                    if (file != null) {
                        return fileService.upload(file).then(function (result) {
                            toaster.show("File uploaded.");
                            vm.jobInfo.fileList.push(result.data);
                        });
                    }
                }

                function getJobInfo() {
                    return schedulerService.getJobInfo(vm.jobId).then(function (result) {
                        vm.jobInfo = result.data;
                        vm.isRepeated = vm.jobInfo.isHavingMoreThanOneDay;
                        vm.jobInfo.id = result.data.id;
                        if (vm.jobInfo.jobId > 0) {
                            vm.jobInfo.franchiseeId = result.data.franchiseeId.toString();
                            vm.jobInfo.jobTypeId = result.data.jobTypeId.toString();
                            if (vm.isCopy) {
                                var currentDate = moment();
                                currentDate.add(1, 'days');
                                vm.jobInfo.actualStartDateString = moment(moment(currentDate).format("L")).add(8, 'hours');
                                vm.jobInfo.actualEndDateString = moment(moment(currentDate).format("L")).add(17, 'hours');
                            }

                            var startDate = moment((vm.jobInfo.actualStartDateString)).format("MM/DD/YYYY h:mm A");
                            getStartDateTimeInDropDownFormat(startDate);

                            var endDate = moment((vm.jobInfo.actualEndDateString)).format("MM/DD/YYYY h:mm A");
                            getEndDateTimeInDropDownFormat(endDate);

                            //vm.jobInfo.startDate = moment(new Date(vm.jobInfo.startDate + "Z")).format("MM/DD/YYYY HH:mm");
                            //vm.jobInfo.endDate = moment(new Date(vm.jobInfo.endDate + "Z")).format("MM/DD/YYYY HH:mm");

                            if (vm.jobInfo.jobCustomer.address.countryId == 8) {
                                vm.isMexico = true;
                            }
                            else {
                                vm.isMexico = false;
                            }
                            vm.jobInfo.startDate = moment((vm.jobInfo.actualStartDateString)).format("MM/DD/YYYY HH:mm");
                            vm.jobInfo.endDate = moment((vm.jobInfo.actualEndDateString)).format("MM/DD/YYYY HH:mm");
                            vm.jobInfo.salesRepId = result.data.salesRepId != null ? result.data.salesRepId.toString() : 0;
                            if (vm.jobInfo.jobCustomer.address[0] != null && (vm.jobInfo.jobCustomer.address[0] != undefined)) {
                                vm.stateCode = vm.jobInfo.jobCustomer.address[0].state;
                            }
                            else {
                                vm.stateCode = vm.jobInfo.jobCustomer.address.state;
                            }
                        }
                        fillModel();
                        getFranchiseeCollection();
                        getHolidayList();
                        if (vm.jobId <= 0) {
                            getFrachiseeInfo();
                        }
                    });
                }

                function getTechList() {
                    return schedulerService.getTechList(vm.jobInfo.franchiseeId).then(function (result) {
                        vm.techList = result.data;
                    });
                }

                function getInvoiceDetails() {
                    if (vm.estimateData != null && vm.estimateData.estimateInvoiceId != null) {
                        var beforeCompletetionValue = 289;
                        vm.estimateModel = {};
                        vm.estimateModel.id = vm.estimateData.estimateInvoiceId;
                        vm.estimateModel.typeId = beforeCompletetionValue;
                        return schedulerService.getInvoiceInfo(vm.estimateModel).then(function (result) {
                            if (result != null && result.data != null) {
                                vm.estimateInvoiceInfo = result.data;
                                vm.estimateInvoiceInfo.customerName = vm.estimateData.jobCustomer.customerName;
                            }
                            else {
                                vm.estimateInvoiceInfo = null;
                            }
                        });
                    }
                    else if (vm.estimateInvoiceInfo != null) {
                        vm.estimateInvoiceInfo = vm.estimateInvoiceInfo;
                    }
                    else {
                        vm.estimateInvoiceInfo = null;
                    }
                }

                function getSalesRepList() {
                    return schedulerService.getSalesRep(vm.jobInfo.franchiseeId).then(function (result) {
                        vm.salesRepList = result.data;
                        vm.salesRepList.push({ display: "Select Sales Representative", value: null });
                        vm.salesRepList = $filter('orderBy')(vm.salesRepList, '-value');
                        if (vm.jobInfo.salesRepId == null || vm.jobInfo.salesRepId == 0) {
                            vm.jobInfo.salesRepId = null;
                        }

                    });
                }

                function getGeoinfo() {
                    return geoCodeService.getGeoInfo(vm.jobInfo.franchiseeId).then(function (result) {
                        if (result.data.zipcode != undefined) {
                            vm.jobInfo.jobCustomer.address[0].zipCode = result.data.zipcode;
                        }
                        if (result.data.directionCode != undefined) {
                            vm.jobInfo.geoCode = result.data.directionCode;
                        }
                    });
                }

                function getGeoinfoByZipCode(zipCode, stateCode) {
                    var franchiseeId = 0;
                    if (vm.query != undefined) {
                        franchiseeId = vm.query.franchiseeId;
                    }
                    else {
                        franchiseeId = vm.jobInfo.franchiseeId
                    }

                    var countryId = vm.jobInfo.jobCustomer.address[0].countryId;
                    return geoCodeService.getGeoinfoByZipCode(zipCode, stateCode, franchiseeId, countryId).then(function (result) {
                        if (vm.isFromLoad && vm.jobId > 0) {
                            vm.isFromLoad = false;
                            return;
                        }
                        if (result.data != null) {
                            if (result.data.geoCode != "")
                                vm.jobInfo.geoCode = result.data.geoCode;
                            else
                                vm.jobInfo.geoCode = '';
                            if (result.data.cityName != "")
                                vm.jobInfo.jobCustomer.address[0].city = result.data.cityName;
                            else
                                vm.jobInfo.jobCustomer.address[0].city = '';

                            if (result.data.stateName != "")
                                vm.jobInfo.jobCustomer.address[0].state = result.data.stateName;
                            else
                                vm.jobInfo.jobCustomer.address[0].state = '';
                            if (result.data.stateId != "" || result.data.stateId != 0)
                                vm.jobInfo.jobCustomer.address[0].stateId = result.data.stateId;
                            if (result.data.stateId == 0)
                                vm.jobInfo.jobCustomer.address[0].stateId = 0;
                        }
                        else {
                            vm.jobInfo.geoCode = "";
                            vm.jobInfo.jobCustomer.address[0].city = "";
                            vm.jobInfo.jobCustomer.address[0].state = "";
                        }
                    });
                }
                function getHolidayList() {
                    schedulerService.getHolidayList(vm.jobInfo.franchiseeId).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.holidayList = result.data.collection;
                        }
                    });
                }

                $scope.$watch('vm.jobInfo.jobCustomer.address[0].zipCode', function (nv, ov) {
                    var statecode;

                    if (nv == ov || nv == undefined || ov == undefined) return;
                    if ((vm.jobInfo.jobCustomer.address[0].state != undefined && vm.jobInfo.jobCustomer.address[0].state != "")
                        && (vm.jobInfo.jobCustomer.address[0].zipCode != undefined || vm.jobInfo.jobCustomer.address[0].zipCode != "")
                        && (vm.jobInfo.jobCustomer.address[0].city != undefined || vm.jobInfo.jobCustomer.address[0].city != "")) {
                        getGeoinfoByZipCode(vm.jobInfo.jobCustomer.address[0].zipCode, vm.jobInfo.jobCustomer.address[0].state);
                    }


                });
                $scope.$watch('vm.jobInfo.jobCustomer.address.zipCode', function (nv, ov) {
                    var statecode;


                    if (nv == ov || nv == undefined || ov == undefined) return;
                    if ((vm.jobInfo.jobCustomer.address[0].state != undefined && vm.jobInfo.jobCustomer.address[0].state != "")
                        && (vm.jobInfo.jobCustomer.address[0].zipCode != undefined || vm.jobInfo.jobCustomer.address[0].zipCode != "")
                        && (vm.jobInfo.jobCustomer.address[0].city != undefined || vm.jobInfo.jobCustomer.address[0].city != "")) {
                        getGeoinfoByZipCode(vm.jobInfo.jobCustomer.address[0].zipCode, vm.jobInfo.jobCustomer.address[0].state);

                    }
                });
                $scope.$watch('vm.jobInfo.jobCustomer.address[0].state', function (nv, ov) {
                    if (nv == ov || nv == undefined || ov == undefined) return;
                    vm.stateCode = nv;
                    if (vm.jobInfo.jobCustomer.address != null && vm.jobInfo.jobCustomer.address[0] != undefined) {
                        if (vm.jobInfo.jobCustomer.address[0].zipCode == null || vm.jobInfo.jobCustomer.address[0].zipCode == "") {
                            return;
                        }
                        if ((vm.jobInfo.jobCustomer.address[0].state != undefined && vm.jobInfo.jobCustomer.address[0].state != "")
                            && (vm.jobInfo.jobCustomer.address[0].zipCode != undefined || vm.jobInfo.jobCustomer.address[0].zipCode != "")
                            && (vm.jobInfo.jobCustomer.address[0].city != undefined || vm.jobInfo.jobCustomer.address[0].city != "")) {
                            //getGeoinfoByZipCode(vm.jobInfo.jobCustomer.address[0].zipCode, vm.jobInfo.jobCustomer.address[0].state);
                        }
                    }

                });
                $scope.$watch('vm.jobInfo.jobCustomer.address[0].countryId', function (nv, ov) {

                    if (nv == ov || nv == undefined || ov == undefined) return;

                    //if (vm.isFromLoadCountry && vm.jobId > 0) {
                    //    vm.isFromLoadCountry = false;
                    //    return;
                    //}
                    vm.stateCode = nv;
                    if (vm.jobInfo.jobCustomer.address[0].countryId == 8) {
                        vm.isMexico = true;
                    }
                    else {
                        vm.isMexico = false;
                    }
                    if (vm.jobInfo.jobCustomer.address != null && vm.jobInfo.jobCustomer.address[0] != undefined) {
                        if (vm.jobInfo.jobCustomer.address[0].zipCode == null || vm.jobInfo.jobCustomer.address[0].zipCode == "") {
                            return;
                        }
                        if ((vm.jobInfo.jobCustomer.address[0].state != undefined && vm.jobInfo.jobCustomer.address[0].state != "")
                            && (vm.jobInfo.jobCustomer.address[0].zipCode != undefined || vm.jobInfo.jobCustomer.address[0].zipCode != "")
                            && (vm.jobInfo.jobCustomer.address[0].city != undefined || vm.jobInfo.jobCustomer.address[0].city != "")) {
                            getGeoinfoByZipCode(vm.jobInfo.jobCustomer.address[0].zipCode, vm.jobInfo.jobCustomer.address[0].state);
                        }
                        if ((vm.jobInfo.jobCustomer.address[0].zipCode != undefined || vm.jobInfo.jobCustomer.address[0].zipCode != "")) {
                            getGeoinfoByZipCode(vm.jobInfo.jobCustomer.address[0].zipCode, "default");
                        }
                    }

                });

                function getFrachiseeInfo() {
                    if (vm.query != undefined)
                        return schedulerService.getFranchiseeInfo(vm.query.franchiseeId).then(function (result) {
                            if (result.data != null) {
                                vm.jobInfo.jobCustomer.address = [result.data];
                                if (result.data.countryId == 8) {
                                    vm.isMexico = true;
                                }
                                else {
                                    vm.isMexico = false;
                                }
                            }
                        });
                }

                function getStartDateTimeInDropDownFormat(startDate) {

                    vm.jobInfo.startDateTime = startDate;
                }

                function getEndDateTimeInDropDownFormat(endDate) {

                    vm.jobInfo.endDateTime = endDate;

                }

                function openZipCodeModal() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/zip-code-modal.client.view.html',
                        controller: 'ZipCodeController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Franchisee: vm.franchiseeName,
                                    FranchiseeId: vm.query.franchiseeId,
                                    loggedInUserId: vm.loggedInUserId,
                                    isSchedulerHidden: true
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getList();
                    }, function () {

                    });
                }

                $scope.$watch('vm.jobInfo.endDate', function (nv, ov) {
                    if (!vm.isOnLoad) {
                        vm.isOnLoad = false;
                        vm.isDateTimeChanged = true;
                    }

                });

                $scope.$watch('vm.jobInfo.startDate', function (nv, ov) {
                    if (!vm.isOnLoad) {
                        vm.isOnLoad = false;
                        vm.isDateTimeChanged = true;
                    }

                });

                $scope.$watch('vm.techIds', function (nv, ov) {
                    vm.isTechIdsChanges = true;
                    vm.techCount = ov;
                })

                function startDateClick() {
                    vm.isOnLoad = false;
                    var dateFormat = moment(vm.jobInfo.startDate).format("MM/DD/YYYY");
                    var enddateFormat = moment(vm.jobInfo.endDate).format("MM/DD/YYYY");
                    var timeFormat = moment(vm.jobInfo.startDate).format("h:mm A");
                    var endDateTimes = moment(vm.jobInfo.endDate).format("h:mm A");
                    var endDateString = enddateFormat + ' ' + endDateTimes;
                    var startDateString = dateFormat + ' ' + timeFormat;
                    vm.jobInfo.startDate = moment(startDateString);
                    vm.jobInfo.endDate = moment(endDateString);
                }

                function endDateClick() {
                    vm.isOnLoad = false;
                    var dateFormat = moment(vm.jobInfo.startDate).format("MM/DD/YYYY");
                    var endDateFormat = moment(vm.jobInfo.endDate).format("MM/DD/YYYY");
                    var timeFormat = moment(vm.jobInfo.startDate).format("h:mm A");
                    var endDateTimes = moment(vm.jobInfo.endDate).format("h:mm A");
                    var endDateString = endDateFormat + ' ' + endDateTimes;
                    var startDateString = dateFormat + ' ' + timeFormat;
                    vm.jobInfo.startDate = moment(startDateString);
                    vm.jobInfo.endDate = moment(endDateString);
                }

                function closePopUp() {
                    vm.croseVisible = false;
                    $('multiselect-parent btn-group dropdown-multiselect').removeClass("open");
                }

                function descriptionChange() {

                    vm.isDescriptionChanged = true;
                }

                function getCustomerInfo(customerName) {
                    if (vm.jobId > 0) return;
                    //return schedulerService.getCustomerInfo(customerName).then(function (result) {
                    //    if (result.data != null) {
                    //        vm.jobInfo.jobCustomer.address = [result.data];
                    //        vm.jobInfo.jobCustomer.phoneNumber = result.data.phoneNumber;
                    //        vm.jobInfo.jobCustomer.email = result.data.email;
                    //    }
                    //    else {
                    //        vm.jobInfo.jobCustomer.address = null;
                    //        vm.jobInfo.jobCustomer.phoneNumber = null;
                    //        vm.jobInfo.jobCustomer.email = null;
                    //        vm.jobInfo.geoCode = null;
                    //    }
                    //});
                }
                function openDropDOwn() {
                    vm.isFromOpen = true;
                    if (!vm.isFromCheckBox)
                        vm.croseVisible = !vm.croseVisible;
                    else {
                        vm.isFromCheckBox = false;
                    }
                }
                $(document).click(function (event) {
                    if (vm.isMobile) {
                        var elements = event.target.innerHTML;
                        var isPresent = angular.element(elements).find('div');
                        if (isPresent.length != 0) {
                            vm.croseVisible = !vm.croseVisible;
                        }
                        if (vm.isFromOpen) {
                            vm.croseVisible = true;
                            vm.isFromOpen = !vm.isFromOpen;
                        }
                    }
                });

                function getFranchiseeInfo() {
                    if ((vm.query != undefined) && (vm.query.franchiseeId != undefined || vm.query.franchiseeId != null)) {
                        return schedulerService.getFranchiseeInfo(vm.query.franchiseeId).then(function (result) {
                            if (result.data != null && vm.jobInfo != null) {
                                vm.jobInfo.jobCustomer.address = [result.data];
                            }
                        });
                    }
                }

                function changeZipCode() {
                    var stateCode = "default";

                    if (vm.jobInfo.jobCustomer.address != null && vm.jobInfo.jobCustomer.address[0] != undefined) {
                        if (vm.jobInfo.jobCustomer.address[0].state != null && vm.jobInfo.jobCustomer.address[0].state != "") {
                            stateCode = vm.jobInfo.jobCustomer.address[0].state;
                        }
                        if (vm.jobInfo.jobCustomer.address[0].zipCode != null && vm.jobInfo.jobCustomer.address[0].zipCode != "") {
                            getGeoinfoByZipCode(vm.jobInfo.jobCustomer.address[0].zipCode, stateCode);
                        }
                    }
                }

                function openValidationModel() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/modal.validate.view.html',
                        controller: 'ModalValidationController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        //size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    IsEstimate: false,
                                    IsEstimateSaved: false
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {

                    }, function () {

                    });
                }

                if (vm.jobId > 0) {
                    $q.all([getJobInfo(), getmarketingClassCollection()]);
                }
                else if (vm.jobId <= 0) {
                    $q.all([getJobInfo(), getmarketingClassCollection(), getFranchiseeInfo()]);
                }
            }]);
}());