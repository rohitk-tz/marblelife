(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("CreateMeetingController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "modalParam", "FileService", "$filter", "CustomerService", "$uibModal", "APP_CONFIG","$stateParams","EstimateService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
            notification, clock, toaster, addressService, modalParam, fileService, $filter, customerService, $uibModal, config, $stateParams, estimateService) {

            
            var vm = this;
            vm.vacationInfo = {};
            vm.id = vm.vacationInfo.id = modalParam.MeetingId != null ? modalParam.MeetingId : 0;
            vm.id = vm.vacationInfo.id = modalParam.MeetingId != null ? modalParam.MeetingId : 0;
            vm.getReportList=getReportList;
            vm.query = modalParam.Query;
            vm.startDate = modalParam.Date;
            vm.franchisee = modalParam.Franchisee;
            vm.Roles = DataHelper.Role;
            vm.franchiseeId = modalParam.franchiseeId != null ? modalParam.franchiseeId.toString() : 0;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
            vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
            vm.isSalesRep = $rootScope.identity.roleId == vm.Roles.SalesRep;
            vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
            vm.getUserList = getUserList;
            vm.createMeeting = createMeeting;
            vm.fillModel = fillModel;
            vm.getVacationInfo = getVacationInfo;
            vm.vacationInfo.idList = [];
            vm.isUser = isUser;
            //vm.query.isForAllMeetings = false;
            vm.query = {
                franchiseeId: 0,
                idList: [],
                idListForUser: [],
                isEquipment: false,
                isUser:true
            }
            if (modalParam.franchiseeId == undefined) {
                vm.query.franchiseeId = $stateParams.franchiseeId == null ? 0 : $stateParams.franchiseeId;
            }
            else
            {
                vm.query.franchiseeId=vm.franchiseeId
            }
            $scope.dateOptions = {
                showWeeks: false
            };
            vm.close = function () {
                $uibModalInstance.dismiss();
            };
            $scope.settings = {
                scrollable: true,
                enableSearch: true,
                selectedToTop: true,
                buttonClasses: 'btn btn-primary leader_btn'
            };
            $scope.translationTexts = {
                checkAll: 'Select All',
                uncheckAll: 'Deselect All',
                selectGroup: 'Select All',
                buttonDefaultText: "Select Equipment(s)",
                dynamicButtonTextSuffix: 'Selected'
            };
            $scope.translationTextsForUsers = {
                checkAll: 'Select All',
                uncheckAll: 'Deselect All',
                selectGroup: 'Select All',
                buttonDefaultText: "Select Users(s)",
                dynamicButtonTextSuffix: 'Selected'
            };
            vm.validTech = true;
            vm.jobInfo = {};
            var currentDate = moment(clock.now());
            //vm.validateModel = validateModel;
            vm.calendarView = true;
            vm.techIds = [];
            vm.Roles = DataHelper.Role;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;


            function isUser(isFromEquipment) {
                if (isFromEquipment)
                {
                    vm.query.idListForUser = [];
                    vm.query.idList = [];
                    vm.query.isUser = !vm.query.isEquipment;
                }
                else {
                    vm.query.idListForUser = [];
                    vm.query.idList = [];
                    vm.query.isEquipment = !vm.query.isUser;
                }
            }
            //vm.repeatJob = repeatJob;
            function getVacationInfo() {
                if (vm.id == null)
                    vm.id = 0;
                return estimateService.getMeetingInfo(vm.id).then(function (result) {
                    vm.vacationInfo = result.data;
                    if (vm.id != 0) {
                        vm.query.isEquipment = vm.vacationInfo.isEquipment;
                        vm.query.isUser = vm.vacationInfo.isUser;
                    }
                    
                    //vm.vacationInfo.startDate = moment(new Date(vm.vacationInfo.startDate + "Z")).format("MM/DD/YYYY HH:mm");
                    //vm.vacationInfo.endDate = moment(new Date(vm.vacationInfo.endDate + "Z")).format("MM/DD/YYYY HH:mm");
                    vm.vacationInfo.startDate = moment((vm.vacationInfo.actualStartDateString)).format("MM/DD/YYYY HH:mm");
                    vm.vacationInfo.endDate = moment((vm.vacationInfo.actualEndDateString)).format("MM/DD/YYYY HH:mm");
                    fillModel();
                });
            }
            function getUserList() {
                return schedulerService.getTechListForMeeting(vm.query.franchiseeId).then(function (result) {
                    vm.techList = result.data;
                });
            }

            function getUserListForUser() {
                return schedulerService.getTechListForMeetingForUser(vm.query.franchiseeId).then(function (result) {
                    vm.techListForUser = result.data;
                });
            }
            function getReportList() {
                vm.query.typeIds = [];
                if (vm.query.idList != null && vm.query.idList.length > 0) {
                    angular.forEach(vm.query.idList, function (value) {
                        vm.query.typeIds.push(value.id);
                    });
                    vm.vacationInfo.idList = vm.query.typeIds;
                }
            }
            function fillModel() {
                if (!vm.isSuperAdmin && !vm.isExecutive) {
                    if (vm.query != null) {
                        vm.query.franchiseeId = $rootScope.identity.organizationId.toString();
                        if (vm.franchiseeId != null && vm.query.franchiseeId == null) {
                            vm.query.franchiseeId = vm.franchiseeId;
                        }
                    }
                    else
                        vm.vacationInfo.franchiseeId = $rootScope.identity.organizationId.toString();
                    vm.franchisee = $rootScope.identity.organizationName;
                }
                if (vm.isExecutive) {
                    vm.vacationInfo.franchiseeId = vm.query != null ? vm.query.franchiseeId.toString() : 0;
                }
                if (vm.vacationInfo.id <= 0) {
                    if (vm.query.franchiseeId > 0)
                        vm.vacationInfo.franchiseeId = vm.query.franchiseeId.toString();

                    if (vm.startDate != null && vm.query.view == "month") {
                        //vm.vacationInfo.startDate = moment(moment(vm.startDate).format("L"));
                        vm.vacationInfo.startDate = moment(moment(vm.startDate).format("L")).add(8, 'hours');
                        vm.vacationInfo.endDate = moment(moment(vm.startDate).format("L")).add(17, 'hours');
                    } else {
                        //vm.vacationInfo.startDate = moment(vm.startDate).toDate();
                        vm.vacationInfo.startDate = moment(moment(vm.startDate).format("L")).add(8, 'hours');
                        vm.vacationInfo.endDate = moment(moment(vm.startDate).format("L")).add(17, 'hours');
                    }
                    vm.vacationInfo.endDate = moment(moment(moment(vm.startDate).format("L")).add(23, 'hours')).add(59, 'minutes');
                }
                if (vm.vacationInfo.jobAssigneeIds != null && vm.vacationInfo.jobAssigneeIds.length > 0 && vm.vacationInfo.isEquipment) {
                    angular.forEach(vm.vacationInfo.jobAssigneeIds, function (value) {
                        vm.query.idList.push({ id: value });
                    });
                }

                if (vm.vacationInfo.jobAssigneeIds != null && vm.vacationInfo.jobAssigneeIds.length > 0 && !vm.vacationInfo.isEquipment) {
                    angular.forEach(vm.vacationInfo.jobAssigneeIds, function (value) {
                        vm.query.idListForUser.push({ id: value });
                    });
                }
                if (vm.vacationInfo.id > 0) {
                    vm.vacationInfo.startDate = moment(vm.vacationInfo.startDate).toDate();
                    vm.vacationInfo.endDate = moment(vm.vacationInfo.endDate).toDate();
                    vm.vacationInfo.assigneeId = vm.query.idList[0].id.toString();
                    if (vm.isSalesRep) {
                        vm.isEdit = (vm.vacationInfo.dataRecorderMetaData.createdBy == $rootScope.identity.organizationRoleUserId);
                    }
                }
                if (!vm.isSuperAdmin && !vm.isExecutive) {
                    vm.vacationInfo.franchiseeId = $rootScope.identity.organizationId.toString();
                }
                if (vm.isTech)
                    vm.vacationInfo.assigneeId = vm.query.idList[0].id.toString();
            }
            function createMeeting(form) {
                vm.isProcessing = true;

                if (vm.query.idList <= 0 && vm.query.isEquipment) {
                    notification.showAlert("Please Select a Equipment!");
                    vm.isProcessing = false;
                    return;
                }
                if (vm.query.idListForUser <= 0 && vm.query.isUser) {
                    notification.showAlert("Please Select a User!");
                    vm.isProcessing = false;
                    return;
                }

                if (vm.vacationInfo.startDate == null || vm.vacationInfo.endDate == null) {
                    notification.showAlert("Please enter Start/End Time!");
                    vm.isProcessing = false;
                    return;
                }

                if (vm.vacationInfo.endDate <= vm.vacationInfo.startDate) {
                    notification.showAlert("End Time should be greater Than Start Time!");
                    vm.vacationInfo.startDate == null;
                    vm.vacationInfo.endDate == null;
                    $scope.$broadcast("reset-dates");
                    vm.isProcessing = false;
                    return;
                }
                save(form);
            }
            function save(form) {
                //validateSchedule();
                vm.vacationInfo.actualStartDateString = moment((vm.vacationInfo.startDate)).format("MM/DD/YYYY HH:mm");
                vm.vacationInfo.actualEndDateString = moment((vm.vacationInfo.endDate)).format("MM/DD/YYYY HH:mm");

                if (vm.vacationInfo.techIds == null)
                {
                    vm.query.typeIds = [];
                    angular.forEach(vm.query.idList, function (value) {
                        vm.query.typeIds.push(value.id);
                    })
                    angular.forEach(vm.query.idListForUser, function (value) {
                        vm.query.typeIds.push(value.id);
                    })
                    vm.vacationInfo.idList = vm.query.typeIds;
                }

                vm.vacationInfo.isEquipment = vm.query.isEquipment;
                return estimateService.saveMeeting(vm.vacationInfo).then(function (result) {
                    if (result.data != null) {
                        if (!result.data) {
                            toaster.error(result.message.message);
                        }
                        else {
                            $rootScope.$broadcast('navigationDate', vm.vacationInfo.startDate);
                            toaster.show(result.message.message);
                            $uibModalInstance.close();
                        }
                        vm.isProcessing = false;
                    }
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }

            //function validateSchedule() {
            //    vm.error = '';
            //    if (vm.vacationInfo.assigneeId <= 0)
            //        return;
            //    vm.salesRep = $filter('filter')(vm.techListForUser, { id: vm.vacationInfo.jobAssigneeIds[0] }, true);
            //    vm.salesRepName = vm.salesRep.length > 0 ? vm.salesRep[0].label : null;
            //    var model = { jobId: vm.vacationInfo.id, assigneeId: vm.vacationInfo.assigneeId, startDate: vm.vacationInfo.startDate, endDate: vm.vacationInfo.endDate };
            //    schedulerService.checkAvailability(model).then(function (result) {
            //        if (!result.data) {
            //            //vm.isNotAvailable = true;
            //            //vm.error = vm.salesRepName + " is not available , try scheduling after 15 mins.";
            //            //vm.vacationInfo.assigneeId = null;
            //            toaster.error("User is not available, try scheduling after 15 mins.");
            //            return;
            //        }
            //    });
            //}

            $q.all([getUserList(), getVacationInfo(), getUserListForUser()]);
        }]);
}());