(function () {
    'use strict';
    var SortColumns = {
        Id: 'Id',
        Name: 'Name',
        Email: 'Email',
        Title: 'Title',
        Country: 'Country',
        StartDate: 'StartDate',
        EndDate: 'EndDate',
        JobType: 'JobType',
        SalesRep: 'SalesRep',
        Franchisee: 'Franchisee',
        Desc: 'Desc',
        QBInvoiceNumber: 'QBInvoiceNumber',
        Technician: 'Technician',
        QBinvoiceNumber: 'QBinvoiceNumber',
        PhoneNumber: 'PhoneNumber'
    };
    angular.module(SchedulerConfiguration.moduleName).controller("ListSchedulerController", ["$state", "$stateParams", "$scope", "$q", "APP_CONFIG",
        "$rootScope", "$uibModal", "Toaster", "Clock", "FranchiseeService", "SchedulerService", "Notification", "EstimateService", "CustomerService",
        "JobService", "$window",
    function ($state, $stateParams, $scope, $q, config, $rootScope, $uibModal, toaster, clock, franchiseeService, schedulerService, notification, estimateService,
        customerService, jobService, $window) {
        var vm = this;
        var currenctdate = moment(clock.now()).format("MM/DD/YYYY");
        vm.jobs = [];
        vm.query = {
            franchiseeId: 0,
            text: '',
            pageNumber: 1,
            jobTypeId: 0,
            pageSize: config.defaultPageSize,
            order: 0,
            propName: '',
            dateCreated: null,
            option: null,
            isCalendar: false,
            dateModified: null,
            imported: null,
            customerName: '',
            email: '',
            phoneNumber: null,
            isLock: false,
            firstName: null,
            lastName: null
        };
        vm.model = {
            Id: 0,
            TechId:0
        };
        vm.isParent = false;
        $window.sessionStorage.setItem("IsBeforeAfterTabAtive", false);
        vm.query.franchiseeId = $stateParams.franchiseeId == null ? 0 : $stateParams.franchiseeId;
        vm.previousView = $stateParams.previousView != null ? $stateParams.previousView : null;
        vm.query.startDate = $stateParams.startDate == null ? null : $stateParams.startDate;
        vm.query.endDate = $stateParams.endDate == null ? null : $stateParams.endDate;
        vm.SortColumns = SortColumns;
        vm.currentRole = $rootScope.identity.roleId;
        vm.Roles = DataHelper.Role;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
        vm.currentPage = vm.query.pageNumber;
        vm.count = 0;
        vm.pagingOptions = config.pagingOptions;
        vm.pageChange = pageChange;
        vm.sorting = sorting;
        vm.resetSearch = resetSearch;
        vm.currencyCode = $rootScope.identity.currencyCode;
        vm.searchOptions = [];
        vm.searchOption = '';
        vm.resetSeachOption = resetSeachOption;
        vm.refresh = refresh;
        vm.getJobList = getJobList;
        vm.deleteJob = deleteJob;
        vm.editJob = editJob;
        vm.editEstimate = editEstimate;
        vm.editVacation = editVacation;
        vm.editMeeting = editMeeting;
        vm.createEstimate = createEstimate;
        vm.createJob = createJob;
        vm.deleteEstimate = deleteEstimate;
        vm.deleteMeeting = deleteMeeting;
        vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
        vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
        vm.viewDesc = viewDesc;
        if (!vm.isSuperAdmin && !vm.isExecutive) {
            vm.query.franchiseeId = $rootScope.identity.organizationId;
        }
        vm.options = [];
        function prepareOptions() {
            vm.options.push({ display: 'Job', value: 1 }),
            vm.options.push({ display: 'Estimate', value: 0 }),
            vm.options.push({ display: 'Personal', value: 2 });
        };
        vm.deleteVacation = deleteVacation;
        vm.importOptions = [];
        function prepareImportOptions() {
            vm.importOptions.push({ display: 'Imported', value: 1 }),
            vm.importOptions.push({ display: 'Created', value: 0 });
        };
        vm.CustomerTypeOptions = [];
        function CustomerTypeOptions() {
            vm.CustomerTypeOptions.push({ display: 'Name', value: 10 }),
            vm.CustomerTypeOptions.push({ display: 'Email', value: 11 }),
            vm.CustomerTypeOptions.push({ display: 'Phone Number', value: 12 });
        };
        vm.SalesRepNameTypeOptions = [];
        function SalesRepNameTypeOptions() {
            vm.SalesRepNameTypeOptions.push({ display: 'First Name', value: 13 }),
            vm.SalesRepNameTypeOptions.push({ display: 'Last Name', value: 14 });
        }
        function createEstimate(query) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/scheduler/views/create-estimate.client.view.html',
                controller: 'CreateEstimateController',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    modalParam: function () {
                        return {
                            Query: query
                        };
                    }
                },
                backdrop: 'static',
            });
            modalInstance.result.then(function () {
                getJobList();
            }, function () {
            });
        }
        function createJob(query) {
            var currentDate = moment(clock.now()).format("MM/DD/YYYY");
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/scheduler/views/create-job.client.view.html',
                controller: 'CreateJobController',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    modalParam: function () {
                        return {
                            Date: currentDate,
                            Query: query
                        };
                    }
                },
                backdrop: 'static',
            });
            modalInstance.result.then(function () {
                getJobList();
            }, function () {
            });
        }
        function editEstimate(id) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/scheduler/views/create-estimate.client.view.html',
                controller: 'CreateEstimateController',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    modalParam: function () {
                        return {
                            EstimateId: id,
                        };
                    }
                },
                backdrop: 'static',
            });
            modalInstance.result.then(function () {
                getJobList();
            });
        }
        function editMeeting(id) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/scheduler/views/create-meeting.client.view.html',
                controller: 'CreateMeetingController',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    modalParam: function () {
                        return {
                            MeetingId: id,
                        };
                    }
                },
                backdrop: 'static',
            });
            modalInstance.result.then(function () {
                getJobList();
            });
        }
        function editVacation(id) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/scheduler/views/create-vacation.client.view.html',
                controller: 'CreateVacationController',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    modalParam: function () {
                        return {
                            VacationId: id,
                        };
                    }
                },
                backdrop: 'static',
            });
            modalInstance.result.then(function () {
                getJobList();
            });
        }
        function editJob(id) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/scheduler/views/edit-job-info.client.view.html',
                controller: 'EditJobInfoController',
                controllerAs: 'vm',
                size: 'lg',
                resolve: {
                    modalParam: function () {
                        return {
                            Id: id,
                        };
                    }
                },
                backdrop: 'static',
            });
            modalInstance.result.then(function () {
                getJobList();
            });
        }
        function viewDesc(desc) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/scheduler/views/notes-description.client.view.html',
                controller: 'NotesDescriptionController',
                controllerAs: 'vm',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    modalParam: function () {
                        return {
                            Description: desc
                        };
                    }
                }
            });
            modalInstance.result.then(function () {
            }, function () { });
        }
        function deleteJob(id) {
            notification.showConfirm("Do you really want to delete the Job?", "Delete Job", function () {
                return jobService.deleteJob(id).then(function (result) {
                    if (!result.data)
                        toaster.error(result.message.message);
                    else
                        toaster.show(result.message.message);
                    getJobList();
                });
            });
        }
        function deleteEstimate(id) {
            notification.showConfirm("Do you really want to delete the Estimate?", "Delete Estimate", function () {
                return estimateService.deleteEstimate(id).then(function (result) {
                    if (!result.data)
                        toaster.error(result.message.message);
                    else
                        toaster.show(result.message.message);
                    getJobList();
                });
            });
        }
        function deleteVacation(id) {
            notification.showConfirm("Do you really want to delete the Personal Time?", "Delete Personal", function () {
                return estimateService.deleteVacation(id).then(function (result) {
                    if (!result.data)
                        toaster.error(result.message.message);
                    else
                        toaster.show(result.message.message);
                    getJobList();
                });
            });
        }

        function prepareSearchOptions() {
            vm.searchOptions.push({ display: 'Customer Class', value: '2' },
                { display: 'Type', value: '4' },
                { display: 'Assignee', value: '5' },
                { display: 'Customer', value: '7' },
                { display: 'Source', value: '6' },
                { display: 'Title', value: '8' },
                { display: 'SalesRep', value: '9' },
                { display: 'Others', value: '3' });
        }
        function resetSeachOption() {
            if (vm.searchOption == '1') {
                vm.query.text = '';
                vm.query.jobTypeId = 0;
                vm.query.techId = 0;
                vm.query.pageNumber = 1;
                vm.query.option = null;
                vm.query.imported = null;
                vm.SalesRepNameTypeOption = null;
                vm.CustomerTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.searchOption == '2') {
                vm.query.text = '';
                vm.query.techId = 0;
                vm.query.pageNumber = 1;
                vm.query.option = null;
                vm.query.imported = null;
                vm.SalesRepNameTypeOption = null;
                vm.CustomerTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.searchOption == '3') {
                vm.query.text = '';
                vm.query.techId = 0;
                vm.query.pageNumber = 1;
                vm.query.option = null;
                vm.query.imported = null;
                vm.SalesRepNameTypeOption = null;
                vm.CustomerTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.searchOption == '4') {
                vm.query.text = '';
                vm.query.jobTypeId = 0;
                vm.query.techId = 0;
                vm.query.pageNumber = 1;
                vm.query.imported = null;
                vm.SalesRepNameTypeOption = null;
                vm.CustomerTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.searchOption == '5') {
                vm.query.text = '';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.option = null;
                vm.query.imported = null;
                vm.SalesRepNameTypeOption = null;
                vm.CustomerTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.searchOption == '6') {
                vm.query.text = '';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.option = null;
                vm.query.techId = 0;
                vm.SalesRepNameTypeOption = null;
                vm.CustomerTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.searchOption == '7' && vm.CustomerTypeOption != 10 && vm.CustomerTypeOption != 11 && vm.CustomerTypeOption != 12) {
                vm.query.customerName = '';
                vm.query.techId = '0';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.imported = null;
                vm.query.option = null;
                vm.query.email = '';
                vm.query.phoneNumber = null;
                vm.SalesRepNameTypeOption = null;
                vm.CustomerTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.searchOption == '8') {
                vm.query.text = '';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.option = null;
                vm.query.techId = 0;
                vm.query.customerName = null;
                vm.query.email = null;
                vm.query.phoneNumber = null;
                vm.query.text = null;
                vm.CustomerTypeOption = null;
                vm.SalesRepNameTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.searchOption == '9') {
                vm.query.text = '';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.option = null;
                vm.query.techId = 0;
                vm.query.customerName = null;
                vm.query.email = null;
                vm.query.phoneNumber = null;
                vm.query.text = null;
                vm.CustomerTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.CustomerTypeOption == '10') {
                vm.query.customerName = '';
                vm.query.techId = '0';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.imported = null;
                vm.query.option = null;
                vm.query.email = '';
                vm.query.phoneNumber = null;
                vm.query.text = '';
                vm.CustomerTypeOption.Name = '';
                vm.CustomerTypeOption = null;
                vm.SalesRepNameTypeOptions = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.CustomerTypeOption == '11') {
                vm.query.customerName = '';
                vm.query.techId = '0';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.imported = null;
                vm.query.option = null;
                vm.query.email = '';
                vm.query.phoneNumber = null;
                vm.query.text = '';
                vm.SalesRepNameTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.CustomerTypeOption == '12') {
                vm.query.customerName = '';
                vm.query.techId = '0';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.imported = null;
                vm.query.option = null;
                vm.query.email = '';
                vm.query.phoneNumber = null;
                vm.query.text = '';
                vm.SalesRepNameTypeOption = null;
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.CustomerTypeOption == '13') {
                vm.query.customerName = '';
                vm.query.techId = '0';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.imported = null;
                vm.query.option = null;
                vm.query.email = '';
                vm.query.phoneNumber = null;
                vm.query.text = '';
                vm.query.firstName = null;
                vm.query.lastName = null;
            }
            else if (vm.CustomerTypeOption == '14') {
                vm.query.customerName = '';
                vm.query.techId = '0';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.imported = null;
                vm.query.option = null;
                vm.query.email = '';
                vm.query.phoneNumber = null;
                vm.query.text = '';
            }
            else {
                vm.query.customerName = '';
                vm.query.techId = '0';
                vm.query.jobTypeId = 0;
                vm.query.pageNumber = 1;
                vm.query.imported = null;
                vm.query.option = null;
                vm.SalesRepNameTypeOption = null;
                vm.CustomerTypeOption = null;
            }
        }
        function resetSearch() {
            vm.searchOption = '';
            vm.query.techId = 0;
            vm.query.pageNumber = 1;
            vm.query.text = '';
            vm.query.jobTypeId = 0;
            vm.query.option = null;
            vm.query.dateCreated = null;
            vm.query.dateModified = null;
            vm.query.imported = null;
            $scope.$broadcast("reset-dates");
            vm.query.customerName = null;
            vm.query.email = null;
            vm.query.phoneNumber = null;
            vm.query.text = null;
            vm.CustomerTypeOption = null;
            vm.SalesRepNameTypeOption = null;
            vm.query.firstName = null;
            vm.query.lastName = null;
        }

        function getJobList() {
            return schedulerService.getJobList(vm.query).then(function (result) {
                if (result != null && result.data != null) {
                    vm.jobs = result.data.collection;
                    vm.count = result.data.pagingModel.totalRecords;
                    vm.query.order = result.data.filter.order;
                }
            });
        }

        function getJobListFirstLoad() {
            return schedulerService.getJobList(vm.query).then(function (result) {
                vm.isProcessing = true;
                if (result != null && result.data != null) {
                    vm.jobs = result.data.collection;
                    vm.count = result.data.pagingModel.totalRecords;
                    vm.query.order = result.data.filter.order;
                }
                vm.isProcessing = false;
            });
        }

        function refresh() {
            getJobList();
        }

        $scope.$on('clearDates', function (event) {
            vm.query.dateCreated = null;
            vm.query.dateModified = null;
            getJobList();
        });
        
        function pageChange() {
            getJobList();
        }

        function sorting(propName) {
            vm.query.propName = propName;
            vm.query.order = (vm.query.order == 0) ? 1 : 0;
            getJobList();
        };

        function getFranchiseeCollection() {
            return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                vm.franchiseeCollection = result.data;
            });
        }
        function getmarketingClassCollection() {
            return customerService.getmarketingClassCollection().then(function (result) {
                vm.marketingClass = result.data;
            });
        }

        function getTechList() {
            return schedulerService.getTechList(vm.query.franchiseeId).then(function (result) {
                vm.techList = result.data;
            });
        }
        function deleteMeeting(id, techId) {
            vm.model.id = id;
            vm.model.techId = techId;

            notification.showConfirm("Do you really want to delete the Meeting?", "Delete Meeting", function () {
                return estimateService.deleteMeeting(vm.model).then(function (result) {
                    if (!result.data)
                        toaster.error(result.message.message);
                    else
                        toaster.show(result.message.message);
                    getJobList();
                });
            });
        }
        
        $scope.$emit("update-title", "Manage Scheduler");
        $q.all([getJobListFirstLoad(), getFranchiseeCollection(), prepareSearchOptions(), getmarketingClassCollection(), getTechList(), prepareOptions(),
            prepareImportOptions(), CustomerTypeOptions(), SalesRepNameTypeOptions()]);
    }]);
}());