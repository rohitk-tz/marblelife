(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("CreateVacationController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "$filter", "Clock", "Toaster", "modalParam", "FranchiseeService",
            "EstimateService", "Notification",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, $filter, clock, toaster, modalParam, franchiseeService,
            estimateService, notification) {

            var vm = this;
            vm.close = function () {
                $uibModalInstance.dismiss();
            };
            vm.vacationInfo = {};
            vm.vacationInfo = {};
            vm.id = vm.vacationInfo.id = modalParam.VacationId != null ? modalParam.VacationId : 0;
            vm.franchiseeId = modalParam.FranchiseeId != null ? modalParam.FranchiseeId.toString() : 0;
            vm.query = modalParam.Query;
            vm.startDate = modalParam.Date;
            vm.Roles = DataHelper.Role;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
            vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
            vm.isSalesRep = $rootScope.identity.roleId == vm.Roles.SalesRep;
            vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
            $scope.dateOptions = {
                showWeeks: false
            };
            vm.createVacation = createVacation;


            if (vm.query != null) {
                vm.franchiseeId = vm.query.franchiseeId;
            }

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

            function getFranchiseeCollection() {
                return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                    vm.franchiseeCollection = result.data;
                    vm.franchisee = $filter('filter')(vm.franchiseeCollection, { value: vm.franchiseeId }, true);
                    vm.franchisee = vm.franchisee.length > 0 ? vm.franchisee[0].display : null;
                });
            }

            function getUserList() {
                return schedulerService.getUserList(vm.franchiseeId).then(function (result) {
                    vm.userList = result.data;
                });
            }

            function getVacationInfo() {
                if (vm.id == null)
                    vm.id = 0;
                return estimateService.getVacationInfo(vm.id).then(function (result) {
                    vm.vacationInfo = result.data;
                    //vm.vacationInfo.startDate = moment(new Date(vm.vacationInfo.startDate + "Z")).format("MM/DD/YYYY HH:mm");
                    //vm.vacationInfo.endDate = moment(new Date(vm.vacationInfo.endDate + "Z")).format("MM/DD/YYYY HH:mm");
                    vm.vacationInfo.startDate = moment((vm.vacationInfo.actualStartDateString)).format("MM/DD/YYYY HH:mm");
                    vm.vacationInfo.endDate = moment((vm.vacationInfo.actualEndDateString)).format("MM/DD/YYYY HH:mm");
                    fillModel();
                });
            }

            function fillModel() {
                if (vm.vacationInfo.id <= 0) {
                    if (vm.query.franchiseeId > 0)
                        vm.vacationInfo.franchiseeId = vm.query.franchiseeId.toString();

                    if (vm.startDate != null && vm.query.view == "month") {
                        vm.vacationInfo.startDate = moment(moment(vm.startDate).format("L"));
                    } else {
                        vm.vacationInfo.startDate = moment(vm.startDate).toDate();
                    }
                    vm.vacationInfo.endDate = moment(moment(moment(vm.startDate).format("L")).add(23, 'hours')).add(59, 'minutes');
                }
                if (vm.vacationInfo.id > 0) {
                    vm.vacationInfo.startDate = moment(vm.vacationInfo.startDate).toDate();
                    vm.vacationInfo.endDate = moment(vm.vacationInfo.endDate).toDate();
                    vm.vacationInfo.assigneeId = vm.vacationInfo.userId.toString();
                    if (vm.isSalesRep) {
                        vm.isEdit = (vm.vacationInfo.dataRecorderMetaData.createdBy == $rootScope.identity.organizationRoleUserId);
                    }
                }
                if (!vm.isSuperAdmin && !vm.isExecutive) {
                    vm.vacationInfo.franchiseeId = $rootScope.identity.organizationId.toString();
                }
                if (vm.isTech)
                    vm.vacationInfo.assigneeId = $rootScope.identity.userId.toString();
            }

            function createVacation(form) {
                vm.isProcessing = true;

                if (vm.vacationInfo.assigneeId <= 0) {
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

                vm.vacationInfo.actualStartDateString = moment((vm.vacationInfo.startDate)).format("MM/DD/YYYY HH:mm");
                vm.vacationInfo.actualEndDateString = moment((vm.vacationInfo.endDate)).format("MM/DD/YYYY HH:mm");

                return estimateService.saveVacation(vm.vacationInfo).then(function (result) {
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

            $q.all([getFranchiseeCollection(), getUserList(), getVacationInfo()]);
        }]);
}());