
(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("CreateEstimateController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "EstimateService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "modalParam", "SchedulerService", "$filter", "CustomerService", "APP_CONFIG", "GeoCodeService", "$uibModal",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, estimateService, franchiseeService,
                notification, clock, toaster, addressService, modalParam, schedulerService, $filter, customerService, config, geoCodeService, $uibModal) {
                var vm = this;
                vm.changeZipCode = changeZipCode;
                vm.isMexico = false;
                vm.isOnLoad = true;
                vm.isFromLoadCountry = true;
                vm.isDescriptionChanged = false;
                vm.descriptionChange = descriptionChange;
                vm.startDateClick = startDateClick;
                vm.endDateClick = endDateClick;
                vm.isDateTimeChanged = false;
                var defaultClassTypeId = angular.copy(config.defaultClassTypeId);
                vm.startDate = modalParam.Date;
                vm.query = modalParam.Query;
                vm.getCustomerInfo = getCustomerInfo;
                vm.estimateInfo = {};
                vm.isCopy = modalParam.IsCopy != null ? modalParam.IsCopy : false;
                vm.estimateId = modalParam.EstimateId != null ? modalParam.EstimateId : 0;
                vm.isEstimateSave = false;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.validateModel = validateModel;
                vm.getRepList = getRepList;
                vm.openZipCodeModal = openZipCodeModal;
                vm.openValidationModel = openValidationModel;
                vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
                vm.isSalesRep = $rootScope.identity.roleId == vm.Roles.SalesRep;
                vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                var currentDate = moment(clock.now());
                vm.isEdit = true;
                vm.validateSchedule = validateSchedule;
                vm.Hours = [{ display: "1", value: "1" }, { display: "2", value: "2" }, { display: "3", value: "3" }, { display: "4", value: "4" },
                { display: "5", value: "5" }, { display: "6", value: "6" }, { display: "7", value: "7" }, { display: "8", value: "8" },
                { display: "9", value: "9" }, { display: "10", value: "10" }, { display: "11", value: "11" }, { display: "12", value: "12" }];

                vm.Minutes = [{ display: "00", value: "001" }, { display: "15", value: "15" }, { display: "30", value: "30" }, { display: "45", value: "45" }];
                vm.Time = [{ display: "PM", value: "PM" }, { display: "AM", value: "AM" }];
                var val = 0;
                if (!vm.isSuperAdmin && !vm.isExecutive) {
                    if (vm.query != null) {
                        vm.query.franchiseeId = $rootScope.identity.organizationId.toString();
                    }
                    vm.estimateInfo.franchiseeId = $rootScope.identity.organizationId.toString();
                    vm.franchisee = $rootScope.identity.organizationName;
                }
                if (vm.isExecutive) {
                    vm.estimateInfo.franchiseeId = vm.query != null ? vm.query.franchiseeId.toString() : 0;
                }

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
                $scope.dateOptions = {
                    showWeeks: false
                };

                function validateSchedule() {
                    if (vm.estimateInfo.id > 0) {
                        vm.error = '';
                        if (vm.estimateInfo.salesRepId <= 0)
                            return;
                        vm.salesRep = $filter('filter')(vm.salesRepList, { value: vm.estimateInfo.salesRepId }, true);
                        vm.salesRepName = vm.salesRep.length > 0 ? vm.salesRep[0].display : null;
                        var model = { jobId: vm.estimateInfo.id, assigneeId: vm.estimateInfo.salesRepId, startDate: vm.estimateInfo.actualStartDateString, endDate: vm.estimateInfo.actualEndDateString };
                        schedulerService.checkAvailability(model).then(function (result) {
                            if (!result.data) {
                                vm.isNotAvailable = true;
                                vm.error = vm.salesRepName + " is not available , try scheduling after 15 mins.";
                                vm.estimateInfo.salesRepId = null;
                            }
                        });
                    } else {
                        vm.estimateInfo.actualStartDateString = moment(new Date(vm.estimateInfo.startDate)).format("MM/DD/YYYY HH:mm");
                        vm.estimateInfo.actualEndDateString = moment(new Date(vm.estimateInfo.endDate)).format("MM/DD/YYYY HH:mm");
                        vm.error = '';
                        if (vm.estimateInfo.salesRepId <= 0)
                            return;
                        vm.salesRep = $filter('filter')(vm.salesRepList, { value: vm.estimateInfo.salesRepId }, true);
                        vm.salesRepName = vm.salesRep.length > 0 ? vm.salesRep[0].display : null;
                        var model = { jobId: vm.estimateInfo.id, assigneeId: vm.estimateInfo.salesRepId, startDate: vm.estimateInfo.actualStartDateString, endDate: vm.estimateInfo.actualEndDateString };
                        schedulerService.checkAvailability(model).then(function (result) {
                            if (!result.data) {
                                vm.isNotAvailable = true;
                                vm.error = vm.salesRepName + " is not available , try scheduling after 15 mins.";
                                vm.estimateInfo.salesRepId = null;
                            }
                        });
                    }
                    
                }

                function validateModel(form) {
                    validateSchedule();
                    var startDate = $filter('date')(vm.estimateInfo.startDate, 'yyyy-MM-dd');
                    var startDateTimeFormat = moment((startDate));

                    var endDate = $filter('date')(vm.estimateInfo.endDate, 'yyyy-MM-dd');
                    var endDateTimeFormat = moment((endDate));

                    vm.isProcessing = true;
                    var isHoliday = false;
                    var currentDateToCompare = moment(currentDate).format('MM/DD/YYYY');
                    var startDateToCompare = moment(vm.estimateInfo.startDate).format('MM/DD/YYYY');
                    var endDateToCompare = moment(vm.estimateInfo.endDate).format('MM/DD/YYYY');


                    //if (vm.estimateInfo.jobCustomer.email == null || vm.estimateInfo.jobCustomer.email == "") {
                    //    notification.showAlert("Please Enter Email");
                    //    vm.isProcessing = false;
                    //    return;
                    //}

                    if (vm.estimateInfo.jobCustomer.phoneNumber == null || vm.estimateInfo.jobCustomer.phoneNumber == "") {
                        notification.showAlert("Please Enter Phone Number");
                        vm.isProcessing = false;
                        return;
                    }

                    if (vm.estimateInfo.jobCustomer.phoneNumber != null && vm.estimateInfo.jobCustomer.phoneNumber != "") {
                        var phoneN = vm.estimateInfo.jobCustomer.phoneNumber.replace(/_/g, "");
                        if (phoneN == null || phoneN.trim().length < 1 || phoneN.length < 10) {
                            notification.showAlert("Please Enter a valid Phone Number");
                            vm.isProcessing = false;
                            return;
                        }
                    }

                    if (vm.estimateInfo.jobCustomer.address != null && vm.estimateInfo.jobCustomer.address[0] != null) {
                        if (vm.estimateInfo.jobCustomer.address[0].addressLine1 == null || vm.estimateInfo.jobCustomer.address[0].addressLine1 == ""
                            || vm.estimateInfo.jobCustomer.address[0].zipCode == null || vm.estimateInfo.jobCustomer.address[0].zipCode == ""
                            || vm.estimateInfo.jobCustomer.address[0].countryId == null) {
                            notification.showAlert("Please fill complete address!");
                            vm.isProcessing = false;
                            return;
                        }
                    }

                    vm.address = addressService.sanitizeAddress(vm.estimateInfo.jobCustomer.address);
                    if (addressService.checkAddressInComplete(vm.estimateInfo.jobCustomer.address)) {
                        notification.showAlert("Please fill complete address!");
                        vm.isProcessing = false;
                        return;
                    }

                    if (vm.estimateInfo.startDate == null || vm.estimateInfo.endDate == null) {
                        notification.showAlert("Please enter Start/End Time!");
                        vm.isProcessing = false;
                        return;
                    }

                    var currentDates = new Date();
                    var dateBeforeTwoMonth = new Date(currentDates.setMonth(currentDates.getMonth() - 2));

                    var startDateTimeFormatDate = new Date(startDateTimeFormat);
                    var endDateToCompareDate = new Date(endDateToCompare);
                    //if (vm.estimateInfo.id <= 0) {
                    if ((vm.estimateInfo.startDate < currentDate || vm.estimateInfo.endDate < currentDate) &&
                        ((dateBeforeTwoMonth >= startDateTimeFormatDate) && dateBeforeTwoMonth >= endDateToCompareDate)) {
                        if (currentDateToCompare != startDateToCompare || currentDateToCompare != endDateToCompare) {
                            if (vm.estimateId <= 0) {
                                notification.showAlert("Estimate should not be of past Time!");
                                vm.estimateInfo.startDate == null;
                                vm.estimateInfo.endDate == null;
                                $scope.$broadcast("reset-dates");
                                vm.isProcessing = false;
                                return;
                            }
                            else {
                                if (!vm.isDateTimeChanged) {
                                    var daysDiff = moment(vm.estimateInfo.actualEndDateString).diff(moment(currentDate), 'days');
                                    if (daysDiff < -2 && vm.isDescriptionChanged) {
                                        notification.showAlert("Estimate should not be of past Time!");
                                        var startDate = moment((vm.estimateInfo.startDate)).format("MM/DD/YYYY h:mm A");
                                        var endDate = moment((vm.estimateInfo.endDate)).format("MM/DD/YYYY h:mm A");
                                        getStartDateTimeInDropDownFormat(startDate);
                                        getEndDateTimeInDropDownFormat(endDate);
                                        vm.estimateInfo.startDate == null;
                                        vm.estimateInfo.endDate == null;
                                        $scope.$broadcast("reset-dates");
                                        vm.isProcessing = false;
                                        return;
                                    }
                                }
                                else {
                                    notification.showAlert("Estimate should not be of past Time!");
                                    var startDate = moment((vm.estimateInfo.startDate)).format("MM/DD/YYYY h:mm A");
                                    var endDate = moment((vm.estimateInfo.endDate)).format("MM/DD/YYYY h:mm A");
                                    getStartDateTimeInDropDownFormat(startDate);
                                    getEndDateTimeInDropDownFormat(endDate);
                                    vm.estimateInfo.startDate == null;
                                    vm.estimateInfo.endDate == null;
                                    $scope.$broadcast("reset-dates");
                                    vm.isProcessing = false;
                                    return;
                                }
                            }
                        }
                    }
                    //}

                    if (vm.estimateInfo.endDate <= vm.estimateInfo.startDate) {
                        notification.showAlert("End Time should be greater Than Start Time!");
                        vm.estimateInfo.startDate == null;
                        vm.estimateInfo.endDate == null;
                        $scope.$broadcast("reset-dates");
                        vm.isProcessing = false;
                        return;
                    }

                    vm.estimateInfo.jobCustomer.address = vm.address[0];
                    save(form);
                }

                function save(form) {
                    var startDate = $filter('date')(vm.estimateInfo.startDate, 'yyyy-MM-dd');
                    var startDateTimeFormat = moment((startDate));
                    //vm.estimateInfo.startDate = startDateTimeFormat;

                    var endDate = $filter('date')(vm.estimateInfo.endDate, 'yyyy-MM-dd');
                    var endDateTimeFormat = moment((endDate));
                    //vm.estimateInfo.endDate = endDateTimeFormat;

                    if (startDateTimeFormat == 'invalidDate') {
                        notification.showAlert("Invalid Start Date!");
                        return;
                    }

                    if (endDateTimeFormat == 'invalidDate') {
                        notification.showAlert("Invalid End Date!");
                        return;
                    }
                   
                    vm.estimateInfo.actualStartDateString = moment(new Date(vm.estimateInfo.startDate)).format("MM/DD/YYYY HH:mm");
                    vm.estimateInfo.actualEndDateString = moment(new Date(vm.estimateInfo.endDate)).format("MM/DD/YYYY HH:mm");
                    vm.estimateInfo.jobScheduler.actualStartDateString = moment(new Date(vm.estimateInfo.startDate)).format("MM/DD/YYYY HH:mm");
                    vm.estimateInfo.jobScheduler.actualEndDateString = moment(new Date(vm.estimateInfo.endDate)).format("MM/DD/YYYY HH:mm");

                    if (vm.isCopy) {
                        vm.estimateInfo.id = 0;
                        vm.estimateInfo.estimateId = 0;
                        vm.estimateInfo.estimateSchedulerList = null;
                        vm.estimateInfo.jobCustomer.customerId = 0;
                    }
                    return estimateService.saveEstimate(vm.estimateInfo).then(function (result) {
                        if (result.data != null) {
                            if (!result.data) {
                                var startDate = moment(vm.estimateInfo.startDate).format("MM/DD/YYYY h:mm A");
                                var endDate = moment(vm.estimateInfo.endDate).format("MM/DD/YYYY h:mm A");

                                getStartDateTimeInDropDownFormat(startDate);
                                getEndDateTimeInDropDownFormat(endDate);

                                toaster.error(result.message.message);
                                vm.estimateInfo.jobCustomer.address = [vm.estimateInfo.jobCustomer.address];
                            }
                            else {
                                toaster.show(result.message.message);
                                $rootScope.$broadcast('navigationDate', vm.estimateInfo.startDate);
                                $uibModalInstance.close();
                            }
                            vm.isProcessing = false;
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                function getEstimateInfo() {
                    return estimateService.getEstimateInfo(vm.estimateId).then(function (result) {
                        vm.estimateInfo = result.data;
                        if (vm.estimateInfo.id > 0) {
                            if (vm.isCopy) {
                                var currentDate = moment();
                                currentDate.add(1, 'days');
                                vm.estimateInfo.actualStartDateString = moment(moment(currentDate).format("L")).add(8, 'hours');
                                vm.estimateInfo.actualEndDateString = moment(moment(currentDate).format("L")).add(17, 'hours');
                                vm.estimateInfo.amount = 0;
                            }
                            vm.estimateInfo.salesRepId = result.data.salesRepId != null ? result.data.salesRepId.toString() : 0;
                            vm.estimateInfo.franchiseeId = result.data.franchiseeId != null ? result.data.franchiseeId.toString() : 0;
                            vm.estimateInfo.jobTypeId = result.data.jobTypeId != null ? result.data.jobTypeId.toString() : null;
                            vm.estimateInfo.startDate = moment(new Date(vm.estimateInfo.actualStartDateString)).format("MM/DD/YYYY HH:mm");
                            vm.estimateInfo.endDate = moment(new Date(vm.estimateInfo.actualEndDateString)).format("MM/DD/YYYY HH:mm");
                            if (vm.estimateInfo.jobCustomer.address.countryId == 8) {
                                vm.isMexico = true;
                            }
                            else {
                                vm.isMexico = false;
                            }
                        }
                        fillModel();
                        getFranchiseeCollection();
                        getHolidayList();
                        if (vm.estimateId == 0) {
                            getFrachiseeInfo();
                        }
                    });
                }

                function fillModel() {
                    if (vm.estimateInfo.id <= 0 && vm.query != null) {
                        if (vm.query.franchiseeId > 0)
                            vm.estimateInfo.franchiseeId = vm.query.franchiseeId.toString();
                        if (vm.query.jobTypeId > 0)
                            vm.estimateInfo.jobTypeId = vm.query.jobTypeId;
                        if (vm.estimateInfo.jobTypeId <= 0)
                            vm.estimateInfo.jobTypeId = defaultClassTypeId.toString();
                        if (vm.startDate != null && vm.query.view == "month") {
                            vm.estimateInfo.startDate = moment(moment(vm.startDate).format("L")).add(8, 'hours');
                        } else {
                            vm.estimateInfo.startDate = moment(vm.startDate).toDate();
                        }
                        vm.estimateInfo.endDate = moment(moment(vm.estimateInfo.startDate)).add(1, 'hours');
                    }
                    if (vm.estimateInfo.id > 0) {
                        vm.estimateInfo.jobCustomer.address = [vm.estimateInfo.jobCustomer.address];
                        vm.estimateInfo.startDate = moment(vm.estimateInfo.startDate).toDate();
                        vm.estimateInfo.endDate = moment(vm.estimateInfo.endDate).toDate();
                        if (vm.isSalesRep) {
                            vm.isEdit = (vm.estimateInfo.dataRecorderMetaData.createdBy == $rootScope.identity.organizationRoleUserId);
                        }
                    }
                    if (!vm.isSuperAdmin && !vm.isExecutive) {
                        vm.estimateInfo.franchiseeId = $rootScope.identity.organizationId.toString();
                    }
                    getRepList();
                }

                function getRepList() {
                    return schedulerService.GetRepList(vm.estimateInfo.franchiseeId).then(function (result) {
                        vm.salesRepList = result.data;
                        if (vm.isSalesRep) {
                            vm.estimateInfo.salesRepId = $rootScope.identity.organizationRoleUserId.toString();
                        }
                    });
                }

                function getmarketingClassCollection() {
                    return customerService.getmarketingClassCollection().then(function (result) {
                        vm.marketingClass = result.data;
                    });
                }


                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                        vm.franchisee = $filter('filter')(vm.franchiseeCollection, { value: vm.estimateInfo.franchiseeId.toString() }, true);
                        vm.franchiseeName = vm.franchisee.length > 0 ? vm.franchisee[0].display : null;
                    });
                }

                function getHolidayList() {
                    schedulerService.getHolidayList(vm.estimateInfo.franchiseeId).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.holidayList = result.data.collection;
                        }
                    });
                }
                function getGeoinfo() {
                    return geoCodeService.getGeoInfo(vm.estimateInfo.franchiseeId).then(function (result) {
                        if (result.data.zipcode != undefined) {
                            vm.estimateInfo.jobCustomer.address[0].zipCode = result.data.zipcode;
                        }
                        if (result.data.directionCode != undefined) {
                            vm.estimateInfo.geoCode = result.data.directionCode;
                        }
                    });
                }
                function getGeoinfoByZipCode(zipCode, stateCode) {
                    var franchiseeId = 0;
                    if (vm.query != undefined) {
                        franchiseeId = vm.query.franchiseeId;
                    }
                    else {
                        franchiseeId = vm.estimateInfo.franchiseeId
                    }
                    var countryId = vm.estimateInfo.jobCustomer.address[0].countryId;
                    return geoCodeService.getGeoinfoByZipCode(zipCode, stateCode, franchiseeId, countryId).then(function (result) {
                        if (result.data != null) {
                            if (result.data.geoCode != "")
                                vm.estimateInfo.geoCode = result.data.geoCode;
                            else
                                vm.estimateInfo.geoCode = '';
                            if (result.data.cityName != "")
                                vm.estimateInfo.jobCustomer.address[0].city = result.data.cityName;
                            else
                                vm.estimateInfo.jobCustomer.address[0].city = '';

                            if (result.data.stateName != "")
                                vm.estimateInfo.jobCustomer.address[0].state = result.data.stateName;
                            else
                                vm.estimateInfo.jobCustomer.address[0].state = '';
                            if (result.data.stateId != "" || result.data.stateId != 0)
                                vm.estimateInfo.jobCustomer.address[0].stateId = result.data.stateId;
                            if (result.data.stateId == 0)
                                vm.estimateInfo.jobCustomer.address[0].stateId = 0;
                        }
                        else {
                            vm.estimateInfo.geoCode = "";
                            vm.estimateInfo.jobCustomer.address[0].city = "";
                            vm.estimateInfo.jobCustomer.address[0].state = "";
                        }
                    });
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
                                    isSchedulerHidden: false
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getList();
                    }, function () {

                    });
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
                                    IsEstimate: true,
                                    IsEstimateSaved: false
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        
                    }, function () {

                    });
                }

                function getFrachiseeInfo() {
                    if ((vm.query != undefined) && (vm.query.franchiseeId != undefined || vm.query.franchiseeId != null)) {
                        return schedulerService.getFranchiseeInfo(vm.query.franchiseeId).then(function (result) {
                            if (result.data != null && vm.estimateInfo != null) {
                                vm.estimateInfo.jobCustomer.address = [result.data];
                                if (result.data.countryId == 8) {
                                    vm.isMexico = true;
                                }
                                else {
                                    vm.isMexico = false;
                                }
                            }
                        });
                    }
                }


                function getStartDateTimeInDropDownFormat(startDate) {
                    vm.estimateInfo.startDateTime = startDate;
                }

                function getEndDateTimeInDropDownFormat(endDate) {
                    vm.estimateInfo.endDateTime = endDate;

                }

                function startDateClick() {
                    vm.isOnLoad = false;
                    var dateFormat = moment(vm.estimateInfo.startDate).format("MM/DD/YYYY");
                    var endDateFormat = moment(vm.estimateInfo.endDate).format("MM/DD/YYYY");
                    var timeFormat = moment(vm.estimateInfo.startDate).format("h:mm A");
                    var endDateTimes = moment(vm.estimateInfo.endDate).format("h:mm A");
                    var endDateString = endDateFormat + ' ' + endDateTimes;
                    var startDateString = dateFormat + ' ' + timeFormat;
                    vm.estimateInfo.startDate = moment(startDateString);
                    vm.estimateInfo.endDate = moment(endDateString);
                }

                function endDateClick() {
                    vm.isOnLoad = false;
                    var dateFormat = moment(vm.estimateInfo.startDate).format("MM/DD/YYYY");
                    var endDateFormat = moment(vm.estimateInfo.endDate).format("MM/DD/YYYY");
                    var timeFormat = moment(vm.estimateInfo.startDate).format("h:mm A");
                    var endDateTimes = moment(vm.estimateInfo.endDate).format("h:mm A");
                    var endDateString = endDateFormat + ' ' + endDateTimes;
                    var startDateString = dateFormat + ' ' + timeFormat;
                    vm.estimateInfo.startDate = moment(startDateString);
                    vm.estimateInfo.endDate = moment(endDateString);
                }

                $scope.$watch('vm.estimateInfo.endDate', function (nv, ov) {
                    if (!vm.isOnLoad) {
                        vm.isOnLoad = false;
                        vm.isDateTimeChanged = true;
                    }

                });


                $scope.$watch('vm.estimateInfo.startDate', function (nv, ov) {
                    if (!vm.isOnLoad) {
                        vm.isOnLoad = false;
                        vm.isDateTimeChanged = true;
                    }

                });

                function getCustomerInfo(customerName) {
                    if (vm.estimateId > 0) return;
                    //return schedulerService.getCustomerInfo(customerName).then(function (result) {
                    //    if (result.data != null) {
                    //        vm.estimateInfo.jobCustomer.address = [result.data];
                    //        vm.estimateInfo.jobCustomer.phoneNumber = result.data.phoneNumber;
                    //        vm.estimateInfo.jobCustomer.email = result.data.email;
                    //    }
                    //    else {
                    //        vm.estimateInfo.jobCustomer.address = null;
                    //        vm.estimateInfo.jobCustomer.phoneNumber = null;
                    //        vm.estimateInfo.jobCustomer.email = null;
                    //        vm.estimateInfo.geoCode = null;
                    //    }
                    //});
                }

                $scope.$watch('vm.estimateInfo.jobCustomer.address[0].countryId', function (nv, ov) {

                    if (nv == ov || nv == undefined || ov == undefined) return;

                    //if (vm.isFromLoadCountry && vm.jobId > 0) {
                    //    vm.isFromLoadCountry = false;
                    //    return;
                    //}
                    vm.stateCode = nv;
                    if (vm.estimateInfo.jobCustomer.address[0].countryId == 8) {
                        vm.isMexico = true;
                    }
                    else {
                        vm.isMexico = false;
                    }
                    if (vm.estimateInfo.jobCustomer.address != null && vm.estimateInfo.jobCustomer.address[0] != undefined) {
                        if (vm.estimateInfo.jobCustomer.address[0].zipCode == null || vm.estimateInfo.jobCustomer.address[0].zipCode == "") {
                            return;
                        }
                        if ((vm.estimateInfo.jobCustomer.address[0].state != undefined && vm.estimateInfo.jobCustomer.address[0].state != "")
                            && (vm.estimateInfo.jobCustomer.address[0].zipCode != undefined || vm.estimateInfo.jobCustomer.address[0].zipCode != "")
                            && (vm.estimateInfo.jobCustomer.address[0].city != undefined || vm.estimateInfo.jobCustomer.address[0].city != "")) {
                            getGeoinfoByZipCode(vm.estimateInfo.jobCustomer.address[0].zipCode, vm.estimateInfo.jobCustomer.address[0].state);
                        }
                        if ((vm.estimateInfo.jobCustomer.address[0].zipCode != undefined || vm.estimateInfo.jobCustomer.address[0].zipCode != "")) {
                            getGeoinfoByZipCode(vm.estimateInfo.jobCustomer.address[0].zipCode, "default");
                        }
                    }

                });


                function changeZipCode() {
                    var stateCode = "default";

                    if (vm.estimateInfo.jobCustomer.address != null && vm.estimateInfo.jobCustomer.address[0] != undefined) {
                        if (vm.estimateInfo.jobCustomer.address[0].state != null && vm.estimateInfo.jobCustomer.address[0].state != "") {
                            stateCode = vm.estimateInfo.jobCustomer.address[0].state;
                        }
                        if (vm.estimateInfo.jobCustomer.address[0].zipCode != null && vm.estimateInfo.jobCustomer.address[0].zipCode != "") {
                            getGeoinfoByZipCode(vm.estimateInfo.jobCustomer.address[0].zipCode, stateCode);
                        }
                    }
                }
                function descriptionChange() {
                    vm.isDescriptionChanged = true;
                }
                $q.all([getEstimateInfo(), getFrachiseeInfo(), getmarketingClassCollection()]);
            }]);
}());