(function () {
    'use strict';

    angular.module(SchedulerConfiguration.moduleName).controller("SchedulerController", ["$state", "$stateParams", "$scope", "$q", "APP_CONFIG",
        "$rootScope", "$uibModal", "Toaster", "Clock", "FranchiseeService", "SchedulerService", "$filter", "CustomerService", "$compile", "$window", "$timeout", "LocalStorageService", "Notification", "UserService", "ToDoService",
        function ($state, $stateParams, $scope, $q, config, $rootScope, $uibModal, toaster, clock, franchiseeService, schedulerService, $filter, customerService, $compile, $window, $timeout, LocalStorageService, notification, userService, toDoService) {
            var vm = this;
            vm.isVisible = false;
            vm.openViewFollowUpModal = openViewFollowUpModal;
            vm.getToDoList = getToDoList;
            vm.isFromJobEstimate = false;
            vm.todoInfoListForToday = [];
            vm.isLockCalendarClicked = false;
            vm.firstDateOfCalendar = new Date();
            vm.lastDateOfCalendar = new Date();
            vm.dragStatus = 0;
            vm.onLockCalendar = onLockCalendar;
            vm.isTodayDisable = true;
            vm.draggedStartDate = new Date();
            vm.draggedEndDate = new Date();
            vm.ischanged = false;
            vm.isNavigated = false;
            $scope.eventSources = [];
            $scope.currentRole = null;
            vm.previousViewClicked = "";
            vm.isMobileView = false;
            vm.print = print;
            vm.testing = testing;
            vm.today = today;
            vm.printScreen = printScreen;
            vm.previousview = 'month';
            vm.monthSelected = true;
            vm.weekSelected = false;
            vm.daySelected = false;
            vm.searchOptions = [];
            vm.defaultView = "";
            vm.isFromLoadFunction = true;
            vm.isFromLoadFunctionForMobile = true;
            vm.isFirstTime = true;
            vm.isFromNavigationChange = true;
            vm.nagvigateToDoList = nagvigateToDoList;
            var isMobile = navigator.userAgent.match(/(iPhone|iPod|iPad|Android|webOS|BlackBerry|IEMobile|Opera Mini)/i);
            vm.buttomDisabled = false;
            vm.searchByMonth = searchByMonth;
            vm.resetSelectedMonth = resetSelectedMonth;

            vm.searchOptions.push({ display: 'Customer Class', value: '1' })
            vm.searchOptions.push({ display: 'Type', value: '2' }, { display: 'Source', value: '3' }
                , { display: 'Customer Name', value: '4' }, { display: 'Phone Number', value: '5' }, { display: 'Email', value: '7' }, { display: 'Address', value: '8' }, { display: 'Month', value: '6' });
            vm.previousView = $stateParams.previousView != null ? $stateParams.previousView : "month";
            vm.previousViewForMobile = $stateParams.previousView != null ? $stateParams.previousView : "month";
            vm.nativateDate = $stateParams.nativateDate != null ? moment($stateParams.nativateDate) : null;
            if (isMobile && $stateParams.previousView == null) {
                vm.previousView = 'agendaDay';
            }
            if (vm.previousView == '0') {
                vm.previousView = 'month';
            }
            vm.getSchedulerStatusList = getSchedulerStatusList;
            vm.getSchedulerSalesList = getSchedulerSalesList;
            vm.getSchedulerTechList = getSchedulerTechList;
            vm.getSchedulerInactiveList = getSchedulerInactiveList;
            vm.getSchedulerLockList = getSchedulerLockList;
            vm.editSchedulerDragDrop = editSchedulerDragDrop;
            var currentDate = moment(clock.now());
            vm.isFromLoad = true;
            vm.Roles = DataHelper.Role;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
            vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
            vm.isSalesRep = $rootScope.identity.roleId == vm.Roles.SalesRep;
            vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
            vm.isOpsMgr = $rootScope.identity.roleId == vm.Roles.OperationsManager;
            vm.addToList = addToList;
            vm.loggedInUserId = $rootScope.identity.userId;
            vm.openImportModal = openImportModal;
            vm.openZipCodeModal = openZipCodeModal;
            vm.ScheduleType = DataHelper.ScheduleType;
            vm.getHolidayList = getHolidayList;
            vm.getEvents = getEvents;
            vm.manageSchedulerList = manageSchedulerList;
            vm.isFromLoadFunction = true;
            vm.next = next;
            vm.prev = prev;
            vm.month = month;
            vm.isDataLoaded = false;

            if (vm.previousView == "month") {
                var myEl = angular.element(document.querySelector('#month'));
                myEl.addClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#week'));
                myEl.removeClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#day'));
                myEl.removeClass('fc-state-active');
            }
            else if (vm.previousView == "agendaWeek") {
                var myEl = angular.element(document.querySelector('#month'));
                myEl.removeClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#week'));
                myEl.addClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#day'));
                myEl.removeClass('fc-state-active');
            }
            else if (vm.previousView == "agendaDay") {
                var myEl = angular.element(document.querySelector('#month'));
                myEl.removeClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#week'));
                myEl.removeClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#day'));
                myEl.addClass('fc-state-active');
            }

            vm.googleApi = googleApi;
            $scope.salesColorCode = false;
            vm.salesColorCode = "green";
            vm.techColorCode = "green";
            vm.lockTextCode = "UnLock User"
            vm.lockColorCode = "#334499";
            vm.InactivecolorCode = "green";
            $scope.InactivecolorCode = false;
            $scope.techColorCode = false;
            $scope.lockColorCode = false;
            vm.lockName = "Unlock User";
            $scope.selectedRoles = [];
            $scope.active = true;
            vm.StatusName = "Active";
            $scope.selectedRoles.push("SalesRep");
            $scope.selectedRoles.push("Technician");
            $scope.customerAddress = "";
            $scope.isMap = false;
            var start = null;
            $scope.isRecursive = true;
            vm.cleanEvents = cleanEvents;
            vm.month = month;
            vm.week = week;
            vm.day = day;
            vm.getSearch = getSearch;
            vm.dragDropQuery = {
                days: 0,
                jobId: 0,
                estimateId: 0,
                personalId: 0,
                meetingId: 0,
                startDate: clock.getStartDateOfMonth(),
                endDate: clock.getEndDateOfMonth(),
                seconds: 0,
                id: 0
            }
            vm.holidayQuery = {
                franchiseeId: 0,
                startDate: clock.getStartDateOfMonth(),
                endDate: clock.getEndDateOfMonth(),
            };
            vm.query = {
                franchiseeId: 0,
                text: '',
                pageNumber: 1,
                jobTypeId: 0,
                techId: 0,
                pageSize: config.defaultPageSize,
                dateCreated: null,
                resourceIds: [],
                dateModified: null,
                isExecutive: vm.isExecutive,
                option: null,
                isCalendar: true,
                startDate: clock.getStartDateOfMonth(),
                endDate: clock.getEndDateOfMonth(),
                view: 'day',
                imported: null,
                status: true,
                selectedOption: 0,
                Role: null,
                isLock: false,
                roleId: null,
                isEmpty: 0,
                customerName: '',
                isFromScheduler: true,
                year: null,
                month: null,
                email: null,
                addressOptions: null,
                street: null,
                city: null,
                state: null,
                country: null,
                zipCode: null
            };

            var storedQuery = LocalStorageService.getSchedulerStorageValue();

            if (storedQuery != null) {
                vm.query = storedQuery;
                //vm.searchOption = storedQuery.searchOption;
                vm.query.jobTypeId = 0;
                vm.query.option = null;
                if (vm.query.isLock) {
                    vm.query.status = true;
                    vm.query.isLock = true;
                    vm.lockName = "Lock User";
                    vm.lockColorCode = "green";
                    $scope.lockColorCode = true;
                }
                else {
                    vm.query.status = false;
                    vm.query.isLock = false;
                    vm.lockName = "Unlock User";
                    vm.lockColorCode = "#334499";
                    $scope.lockColorCode = false;
                }
            }

            if (vm.isSuperAdmin) {
                vm.query.franchiseeId = $stateParams.franchiseeId;
            }
            if (!vm.isSuperAdmin) {
                vm.query.franchiseeId = $rootScope.identity.organizationId;
            }
            if (vm.isExecutive) {
                if ($stateParams != null && $stateParams.franchiseeId > 1)
                    vm.query.franchiseeId = $stateParams.franchiseeId;
                else {
                    vm.query.franchiseeId = $rootScope.identity.loggedInOrganizationId;
                }
                getList();
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
                                loggedInUserId: vm.loggedInUserId
                            };
                        }
                    }
                });
                modalInstance.result.then(function () {
                    //vm.query.franchiseeId = result;
                    getList();
                }, function () {

                });
            }
            function googleApi(jobId, estimeId) {
                $scope.isMap = true;
                return schedulerService.getCustomerAddress(jobId, estimeId).then(function (result) {
                    var isMobile = navigator.userAgent.match(/(iPhone|iPod|iPad|Android|webOS|BlackBerry|IEMobile|Opera Mini)/i);
                    // console.log(isMobile);
                    $scope.customerAddress = result.data;
                    var value = getOperatingSystem();
                    var isMobile = navigator.userAgent.match(/(iPhone|iPod|iPad|Android|webOS|BlackBerry|IEMobile|Opera Mini|Win32|Win64)/i);
                    if (value.mobile == true) {
                        if (value.os != "Android") {
                            if (value.os == "iOS") {
                                var url = "https://maps.apple.com?address=" + result.data;
                                console.log(url);
                                window.open(url, '_blank');
                                //window.location.href = url;
                                $scope.isMap = false;
                            }
                            else {
                                var url = "http://maps.google.com/maps?daddr=" + result.data;
                                window.open(url, '_blank');
                                $scope.isMap = false;
                            }
                        }

                        else {
                            $scope.isMap = false;
                            var url = "geo:?q=" + result.data + "&z=15";
                            window.location.href = url;
                        }
                    }
                    else {
                        var url = "https://www.google.com/maps/dir/?api=1&destination=" + result.data + "&dir_action=navigate";
                        window.open(url, '_blank');
                        $scope.isMap = false;
                    }
                });
            }

            function myNavFunc(address) {
                // If it's an iPhone..
                if ((navigator.platform.indexOf("iPhone") != -1)
                    || (navigator.platform.indexOf("iPod") != -1)
                    || (navigator.platform.indexOf("iPad") != -1)) {
                    window.open("maps://maps.google.com/maps?daddr=" + address);
                }
                else {
                    // console.log(address);
                    window.open("http://maps.google.com/maps?daddr=" + address);
                }

            }
            vm.options = [];
            var previousView = vm.previousView;
            function getSchedulerInactiveList() {
                if ($scope.InactivecolorCode) {
                    vm.StatusName = "Active";
                    vm.InactivecolorCode = "green";
                    $scope.active = true;
                    $scope.InactivecolorCode = false;

                    if ($scope.selectedRoles.length > 1) {
                        vm.query.status = $scope.active;
                        vm.query.Role = null
                    }
                    if ($scope.selectedRoles.length == 1) {
                        vm.query.status = $scope.active;
                        vm.query.Role = $scope.selectedRoles[0];
                        if (vm.query.Role == "SalesRep")
                            vm.query.roleId = vm.Roles.SalesRep;
                        if (vm.query.Role == "Technician")
                            vm.query.roleId = vm.Roles.Technician;
                    }

                    if ($scope.selectedRoles.length == 0) {
                        //vm.query.franchiseeId = 0
                        vm.query.status = $scope.active;
                        vm.query.Role = null
                    }
                    var status = DataHelper.Status.getValue(vm.query.selectedOption);
                    vm.query.isLock = false;
                    vm.query.isEmpty = 0;
                    getEvents();
                    getAssigneeList();
                }
                else {
                    vm.StatusName = "Inactive";
                    $scope.active = false;
                    vm.InactivecolorCode = "#334499";
                    $scope.InactivecolorCode = true;
                    vm.query.Role = $scope.currentRole;


                    if ($scope.selectedRoles.length > 1) {
                        vm.query.status = $scope.active;
                        vm.query.Role = null;
                        vm.query.isLock = true;
                    }
                    if ($scope.selectedRoles.length == 1) {
                        vm.query.isLock = true;
                        vm.query.roleId = $scope.selectedRoles[0];
                        vm.query.status = false;
                        vm.query.Role = $scope.selectedRoles[0];
                        if (vm.query.Role == "SalesRep")
                            vm.query.roleId = vm.Roles.SalesRep;
                        if (vm.query.Role == "Technician")
                            vm.query.roleId = vm.Roles.Technician;
                    }
                    if ($scope.selectedRoles.length == 0) {
                        vm.query.status = true;
                        vm.query.Role = null;
                        vm.query.isLock = false;
                        //vm.query.franchiseeId = -1;
                        vm.query.isEmpty = 1;
                    }
                    //vm.query.isLock = true;
                    getEvents();
                    getAssigneeList();
                }

            }
            function getSchedulerSalesList() {

                if ($scope.salesColorCode) {
                    vm.salesColorCode = "green";
                    $scope.salesColorCode = false;
                    var status = DataHelper.Status.getValue(vm.query.selectedOption);
                    if ($scope.selectedRoles.length != 0) {
                        vm.query.status = true;
                        vm.query.Role = null;
                        $scope.selectedRoles.push("SalesRep");

                    }
                    else {
                        vm.query.status = true;
                        vm.query.Role = "SalesRep";
                        $scope.currentRole = "SalesRep";
                        $scope.selectedRoles.push("SalesRep");
                    }

                    if ($scope.selectedRoles.length == 1) {
                        vm.query.status = $scope.active;
                        vm.query.Role = vm.query.Role;
                        vm.query.roleId = vm.Roles.SalesRep;
                    }
                    if ($scope.selectedRoles.length > 1) {
                        vm.query.roleId = null;
                        if ($scope.active == true) {
                            vm.query.status = $scope.active;
                            vm.query.Role = vm.query.Role;
                            //vm.query.roleId = vm.Roles.Technician;
                        }
                        else {
                            vm.query.status = $scope.active;
                            vm.query.Role = vm.query.Role;
                        }
                    }

                    if ($scope.selectedRoles.length == 0) {
                        vm.query.roleId = null;
                        if ($scope.active == true) {
                            vm.query.status = $scope.active;
                            vm.query.Role = null;
                        }
                        else {
                            vm.query.franchiseeId = 0;
                            vm.query.status = $scope.active;
                            vm.query.Role = null;
                        }
                    }
                    vm.query.isEmpty = 0;
                    getEvents();
                    getAssigneeList();
                }
                else {
                    var index = $scope.selectedRoles.indexOf("SalesRep");
                    $scope.selectedRoles.splice(index, 1);
                    $scope.salesColorCode = true;
                    vm.query.Role = $scope.currentRole;
                    vm.salesColorCode = "#334499";
                    if ($scope.selectedRoles.length != 0)
                        vm.query.status = $scope.active;
                    else {
                        vm.query.status = $scope.active;
                        //$scope.selectedRoles.push("SalesRep");
                    }

                    var status = DataHelper.Status.getValue(vm.query.selectedOption);
                    if ($scope.selectedRoles.length > 1) {
                        vm.query.roleId = null;
                        if ($scope.active == true) {
                            vm.query.status = $scope.active;
                            vm.query.Role = null;

                        }
                        else {
                            vm.query.status = $scope.active;
                            vm.query.Role = null;
                        }
                    }
                    if ($scope.selectedRoles.length == 1) {
                        vm.query.roleId = vm.Roles.Technician;
                        if ($scope.active == true) {
                            vm.query.status = $scope.active;
                            vm.query.Role = "Technician";
                        }
                        else {
                            vm.query.status = $scope.active;
                            vm.query.Role = "Technician"
                        }
                    }
                    if ($scope.selectedRoles.length == 0) {
                        vm.query.roleId = null;
                        if ($scope.active == true) {
                            vm.query.status = $scope.active;
                            vm.query.Role = null;
                        }
                        else {
                            vm.query.status = true;
                            vm.query.Role = null;
                            vm.query.isLock = false;
                            //vm.query.franchiseeId = -1;
                            vm.query.isEmpty = 1;
                        }
                    }
                    getEvents();
                    getAssigneeList();
                }


            }
            function getSchedulerTechList() {
                //vm.query.franchiseeId = $stateParams.franchiseeId == null ? 0 : $stateParams.franchiseeId;
                if ($scope.techColorCode) {
                    vm.query.roleId = vm.Roles.Technician;
                    vm.techColorCode = "green";
                    $scope.techColorCode = false;

                    //var status = DataHelper.Status.getValue(vm.query.selectedOption);
                    if ($scope.selectedRoles.length != 0) {
                        vm.query.status = $scope.active;
                        vm.query.Role = null;
                        $scope.selectedRoles.push("Technician");
                    }
                    else {
                        vm.query.status = $scope.active;
                        vm.query.Role = "Technician";
                        $scope.currentRole = "Technician";
                        $scope.selectedRoles.push("Technician");
                    }
                    if ($scope.selectedRoles.length > 1) {
                        vm.query.roleId = null;
                        if ($scope.active == true) {
                            vm.query.status = $scope.active;
                            vm.query.Role = null;
                        }
                        else {
                            vm.query.status = $scope.active;
                            vm.query.Role = null;
                        }

                    }
                    if ($scope.selectedRoles.length == 1) {
                        vm.query.roleId = vm.Roles.Technician;
                        if ($scope.active == true) {
                            vm.query.status = $scope.active;
                            vm.query.Role = "Technician";
                            vm.query.isLock = false;
                        }
                        else {
                            //vm.query.franchiseeId = -1;
                            vm.query.status = $scope.active;
                            vm.query.Role = "Technician";
                            vm.query.isLock = true;
                        }
                    }
                    if ($scope.selectedRoles.length == 0) {
                        if ($scope.active == true) {
                            vm.query.status = $scope.active;
                            vm.query.Role = null;
                        }
                        else {
                            //vm.query.franchiseeId = -1
                            vm.query.status = $scope.active;
                            vm.query.Role = null;
                            vm.query.isLock = true;
                        }
                    }
                    vm.query.isEmpty = 0;
                    getEvents();
                    getAssigneeList();
                }
                else {
                    var index = $scope.selectedRoles.indexOf("Technician");
                    $scope.selectedRoles.splice(index, 1);
                    vm.techColorCode = "#334499";
                    $scope.techColorCode = true;
                    if ($scope.selectedRoles.length != 0)
                        vm.query.status = $scope.active;
                    else {
                        vm.query.status = null;
                        //$scope.selectedRoles.push("SalesRep");
                    }

                    //var status = DataHelper.Status.getValue(vm.query.selectedOption);
                    if ($scope.selectedRoles.length > 1) {
                        vm.query.roleId = null;
                        vm.query.status = $scope.active;
                        vm.query.Role = null;
                    }
                    if ($scope.selectedRoles.length == 1) {
                        vm.query.roleId = vm.Roles.SalesRep;
                        vm.query.status = $scope.active;
                        vm.query.Role = $scope.selectedRoles[0];
                    }
                    if ($scope.selectedRoles.length == 0) {
                        vm.query.roleId = null;
                        if ($scope.active == true) {
                            vm.query.status = $scope.active;
                            vm.query.Role = null;
                        }
                        else {
                            vm.query.status = true;
                            vm.query.Role = null;
                            vm.query.isLock = false;
                            //vm.query.franchiseeId = -1;
                            vm.query.isEmpty = 1;
                        }
                    }
                    getEvents();
                    getAssigneeList();
                }
            }

            function prepareOptions() {
                vm.options.push({ display: 'Job', value: 1 }),
                    vm.options.push({ display: 'Estimate', value: 0 });
                vm.options.push({ display: 'Personal', value: 2 });
            };
            vm.importOptions = [];
            vm.statusOptions = [];
            vm.addressOptions = [];
            function prepareStatusOptions() {
                //vm.statusOptions.push({ display: 'All', value: 1 });
                vm.statusOptions.push({ display: 'Active', value: 2 }),
                    vm.statusOptions.push({ display: 'Inactive', value: 3 });

            }

            function prepareImportOptions() {
                vm.importOptions.push({ display: 'Imported', value: 1 }),
                    vm.importOptions.push({ display: 'Created', value: 0 });
            };
            //vm.query.franchiseeId = $stateParams.franchiseeId == null ? 0 : $stateParams.franchiseeId;

            function addressSerachOptions() {
                vm.addressOptions.push({ display: 'Street', value: 1 })
                vm.addressOptions.push({ display: 'City', value: 2 }),
                    vm.addressOptions.push({ display: 'State', value: 3 }),
                    vm.addressOptions.push({ display: 'Country', value: 4 }),
                    vm.addressOptions.push({ display: 'Zip Code', value: 5 });
            }
            function openImportModal() {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/import-calendar.client.view.html',
                    controller: 'CalendarImportController',
                    controllerAs: 'vm',
                    backdrop: 'static',
                    size: 'md',
                    resolve: {
                        modalParam: function () {
                            return {
                                Franchisee: vm.franchiseeName,
                                FranchiseeId: vm.query.franchiseeId
                            };
                        }
                    }
                });
                modalInstance.result.then(function () {
                    //vm.query.franchiseeId = result;
                    getList();
                }, function () {

                });
            }

            function getList() {
                getAssigneeList();
                getEvents();
                getFranchiseeCollection();
            }

            function addToList(item) {
                var index = vm.query.resourceIds.indexOf(item.id);
                if (index >= 0) {
                    item.selected = false;
                    vm.query.resourceIds.splice(index, 1);
                }
                else {
                    item.selected = true
                    vm.query.resourceIds.push(item.id);
                }
                if (vm.query.resourceIds.length > 0) {
                    vm.isVisible = true;
                }
                else {
                    vm.isVisible = false;
                }
                getEvents();
                getToDoList();
            }

            vm.filterByTech = filterByTech;

            vm.resetSearch = resetSearch;
            vm.refresh = refresh;
            vm.openOptionModal = openOptionModal;
            vm.resetSearchOption = resetSearchOption;
            vm.resetSearchOptionForAddress = resetSearchOptionForAddress;

            function resetSearch() {
                vm.query.resourceIds = [];
                vm.query.jobTypeId = 0;
                vm.query.techId = 0;
                vm.query.option = null;
                vm.query.imported = null;
                vm.query.customerName = '';
                vm.query.text = '';
                vm.searchOption = '';
                vm.query.email = null;
                vm.query.street = null;
                vm.query.city = null;
                vm.query.state = null;
                vm.query.country = null;
                vm.query.zipCode = null;
                vm.query.addressOptions = null;
                getSearch();
            }

            function resetSearchOption() {
                vm.query.resourceIds = [];
                vm.query.jobTypeId = 0;
                vm.query.techId = 0;
                vm.query.option = null;
                vm.query.imported = null;
                vm.query.customerName = '';
                vm.query.text = '';
                vm.query.email = null;
                vm.query.street = null;
                vm.query.city = null;
                vm.query.state = null;
                vm.query.country = null;
                vm.query.zipCode = null;
                vm.query.addressOptions = null;
            }

            function resetSearchOptionForAddress() {
                vm.query.street = null;
                vm.query.city = null;
                vm.query.state = null;
                vm.query.country = null;
                vm.query.zipCode = null;
            }

            function filterByTech(id) {
                vm.query.techId = id;
                getEvents();
            }
            $scope.changeView = function (view, element) {
                previousView = $('#calendar_123').fullCalendar('getView').name;
            };
            function refresh() {
                getEvents();
            }

            function getLastTwentyYearCollection() {
                return schedulerService.getLastTwentyYearCollection().then(function (result) {
                    vm.yearCollection = result.data;
                    var currentDate = new Date();
                    var currentYear = currentDate.getFullYear();
                })
            }

            function getMonthCollection() {
                return schedulerService.getMonthNames().then(function (result) {
                    vm.monthCollection = result.data;
                    var currentDate = new Date();
                    var currentMonth = currentDate.getMonth();
                })
            }

            function searchByMonth() {
                var navigatingDateByMonth = new Date(vm.query.year, vm.query.month - 1);
                var navigatingDate = moment(navigatingDateByMonth);
                $('#calendar_123').fullCalendar('gotoDate', navigatingDate);
                testing();
            }

                function resetSelectedMonth() {
                    vm.query.resourceIds = [];
                    //vm.query.franchiseeId = 0;
                    vm.query.jobTypeId = 0;
                    vm.query.techId = 0;
                    vm.query.option = null;
                    vm.query.imported = null;
                    vm.query.customerName = '';
                    vm.query.text = '';
                    vm.searchOption = '';
                    vm.query.year = null;
                    vm.query.month = null;
                    var currentDate = new Date();
                    var currentYear = currentDate.getFullYear();
                    var currentMonth = currentDate.getMonth();
                    var navigatingDateByMonth = new Date(currentYear, currentMonth);
                    var navigatingDate = moment(navigatingDateByMonth);
                    $('#calendar_123').fullCalendar('gotoDate', navigatingDate);
                    testing();
                    //testing();
                }

            $scope.uiConfig = {
                calendar: {
                    height: 750,
                    //editable: true,
                    header: {
                        //left: 'month agendaWeek agendaDay', //basicDay agendaWeek agendaDay
                        center: 'titles',
                        right: 'today' // prev,next

                    },
                    defaultView: previousView,
                    editable: true,
                    timezone: 'local',
                    eventOrder: function (eventA, eventB) {
                        if (eventA.miscProps.actualStartDate == eventB.miscProps.actualStartDate && eventA.miscProps.actualEndDate == eventB.miscProps.actualEndDate)
                            if (eventA.miscProps.actualStartDate == eventB.miscProps.actualStartDate && eventA.miscProps.actualEndDate == eventB.miscProps.actualEndDate)
                                return (angular.uppercase(eventA.miscProps.assignee) < angular.uppercase(eventB.miscProps.assignee)) ? -1 : 1;
                    },
                    //showNonCurrentDates: false,
                    viewRender: function (view, element) {


                        vm.firstDateOfCalendar = new Date(view.intervalStart);
                        vm.lastDateOfCalendar = new Date(view.intervalEnd);

                        vm.query.startDate = new Date(view.intervalStart);
                        vm.query.endDate = new Date(view.intervalEnd);

                        if (vm.isFromLoadFunction && !isMobile && vm.nativateDate == null) {
                            vm.isFromLoadFunction = false;
                            vm.isFromNavigationChange = true;
                            //$('#calendar_123').fullCalendar('changeView', vm.defaultView);
                            if (vm.defaultView == 'month') { month(); }
                            else if (vm.defaultView == 'agendaWeek') { week(); }
                            else if (vm.defaultView == 'agendaDay') { day(); }
                            vm.previousViewClicked = view.name;
                        }
                        else {

                            var render = $('#calendar_123').fullCalendar('getView').name;
                            if (vm.previousview == "month" && !vm.isFromLoadFunction && !isMobile) {
                                $('#calendar_123').fullCalendar('changeView', 'month');
                            }
                            else if (vm.previousview == "agendaWeek" && !vm.isFromLoadFunction && !isMobile) {
                                $('#calendar_123').fullCalendar('changeView', 'agendaWeek');
                            }
                            else if (vm.previousview == "agendaDay" && !vm.isFromLoadFunction && !isMobile) {
                                $('#calendar_123').fullCalendar('changeView', 'agendaDay');
                            }

                            if (vm.previousView == "month" && !vm.isFromLoadFunction && isMobile) {
                                $('#calendar_123').fullCalendar('changeView', 'month');
                            }
                            else if (vm.previousview == "agendaWeek" && !vm.ischanged && isMobile) {
                                $('#calendar_123').fullCalendar('changeView', 'agendaWeek');
                            }
                            else if (vm.previousview == "agendaDay" && !vm.ischanged && isMobile) {
                                $('#calendar_123').fullCalendar('changeView', 'agendaDay');
                            }

                            if (isMobile && vm.isFirstTime && vm.nativateDate == null) {
                                vm.isFirstTime = false;
                                day();
                            }

                            var date = new Date();
                            var currentDates = moment(clock.now()).add(1, 'months');
                            var calenderMonthInSeconds = new Date(view.start._d).setDate(view.start._d.getDate() + 4);
                            var calenderDate = new Date(calenderMonthInSeconds);

                            if (vm.previousViewClicked != view.name && vm.previousViewClicked != "" && !vm.isFromLoadFunction) {
                                $("#calendar_123").fullCalendar('removeEvents');
                                vm.ischanged = true;
                                testing();
                                $timeout(callAtTimeout, 2000);
                            }
                            if (date.getMonth() != calenderDate.getMonth() && !vm.isFromLoadFunction) {
                                $("#calendar_123").fullCalendar('removeEvents');
                            }
                            if (date.getMonth() == calenderDate.getMonth() && !isMobile) {
                                testing();
                                $timeout(callAtTimeout, 2000);
                            }

                            if (date.getMonth() == calenderDate.getMonth() && isMobile) {
                                $("#calendar_123").fullCalendar('removeEvents');
                                testing();
                            }
                            if (date.getYear() - calenderDate.getYear() == 1 && isMobile) {
                                $("#calendar_123").fullCalendar('removeEvents');
                                testing();
                            }
                            if (!vm.ischanged && render == 'agendaDay') {

                                testing();
                                $timeout(callAtTimeout, 2000);
                            }
                            vm.previousViewClicked = view.name;
                            vm.query.startDate = view.start._d;
                            vm.query.endDate = view.end._d;
                            if (vm.ischanged) {
                                vm.ischanged = false;
                                vm.isDataLoaded = true;
                            }
                            else if (!vm.ischanged) {
                                vm.isDataLoaded = true;
                            }
                            if (vm.isFromLoad) {
                                testing();

                            }

                            //if (!vm.isFromLoad && vm.isNavigated && vm.nativateDate != null && vm.defaultView != render) {
                            //  vm.isNavigated = true;
                            //   var navigatingDate = moment(vm.nativateDate);
                            //    $('#calendar_123').fullCalendar('gotoDate', navigatingDate);
                            //    testing();

                            //}
                            if (vm.nativateDate != null && !vm.isNavigated) {
                                vm.isNavigated = true;
                                var navigatingDate = moment(vm.nativateDate);
                                $('#calendar_123').fullCalendar('gotoDate', navigatingDate);
                                $timeout(callAtTimeout, 2000);
                            }
                            if (vm.isFromJobEstimate) {
                                testing();
                            }
                            disableOrEnableTodayButton();
                        }
                    },
                    //viewDestroy:$scope.viewDestroy,
                    eventLimit: true, // for all non-agenda views 
                    slotEventOverlap: false,
                    droppable: true,
                    businessHours: [{
                        dow: [1, 2, 3, 4, 5], // Maybe not 0,6? Sunday,Saturday
                        start: '08:00',
                        end: '17:00'
                    }],
                    views: {
                        month: {
                            eventLimit: 20 // adjust to 6 only for agendaWeek/agendaDay
                        },
                        agendaWeek: {
                            eventLimit: 20 // adjust to 6 only for agendaWeek/agendaDay
                        }
                    },
                    dayClick: function (date, jsEvent, view) {
                        if (vm.isLockCalendarClicked) {
                            return;
                        }
                        //if (!vm.isOpsMgr) {
                        eventDayClick(date, jsEvent, view);
                        //}
                    },
                    eventRender: function (event, element) {
                        renderEvent(event, element);
                    },
                    dayRender: function (date, cell) {
                        if (date < currentDate) {
                            $(cell).addClass('disabled');
                        }
                    },
                    eventMouseover: function (calEvent, jsEvent) {
                        var isLock = calEvent.isLock;
                        var startDateFormat = moment((calEvent.actualStartDate)).format("MM/DD/YYYY HH:mm");// moment.utc(event.actualStartDate).local().format('YYYY-MM-DD HH:mm:ss');// moment(event.actualStartDate).format("YYYY-MM-DD hh:mm:ss A Z");
                        var endDateFormat = moment((calEvent.actualEndDate)).format("MM/DD/YYYY HH:mm");
                        var toolTipBody = null;
                        var phone = null;
                        var status = '';
                        var geoCode = '';
                        var mailSend = '';
                        var customerClass = '';
                        if (!calEvent.isHoliday && !calEvent.isVacation) {
                            if (calEvent.isCustomerMailSend) {
                                mailSend = "<span><b>Estimate Sent To Customer.</b></span>"
                            }
                            var toolTipBody = '';
                            var amount = '';
                            var serviceTypeNames = '';
                            if (calEvent.isInvoicePresent) {
                                amount = "<span><b>Price</b> : $" + calEvent.invoiceAmount + "</span></br>";
                            }
                            else if (!calEvent.isInvoicePresent) {
                                if (calEvent.estimatedAmount <= 0) {
                                    amount = "<span><b>Price</b> : $ ? </span></br>";
                                }
                                else {
                                    amount = "<span><b>Price</b> : $" + calEvent.estimatedAmount + "</span></br>";
                                }
                            }
                            else {
                                amount = "<span><b>Price</b> : $ ? </span></br>";
                            }
                            if (calEvent.serviceTypeName != null) {
                                serviceTypeNames = "<span> <b>Service Types</b> : " + calEvent.serviceTypeName + "</span> <br />";
                            }
                            if (calEvent.jobCustomer != null) {
                                toolTipBody = mailSend + "<br/><span> <b>Assignee</b> : " + calEvent.assignee + "</span> <br />"
                                    + "<span> <b>City</b> : " + calEvent.jobCustomer.address.city + "</span> <br />"
                                    + "<span> <b>Customer</b> : " + calEvent.jobCustomer.customerName + "</span> <br />"
                                    + serviceTypeNames
                                    + amount
                                    + "<span> <b> IsSigned</b> : " + calEvent.isInvoiceSigned + "</span> <br />"
                                    + "<span> <b>Franchisee</b> : " + calEvent.franchisee + "</span> <br />"
                                    + "<span> <b>Created By</b> : " + calEvent.createdBy + "</span> <br />"
                                    + "<span> <b>Date</b> : " + startDateFormat
                                    + " - " + endDateFormat + "</span> <br />"
                                    + "<span> <b>User Lock</b> : " + isLock + "</span>";

                                phone = calEvent.jobCustomer.phoneNumber != null ? "<span> <b>Phone</b> : " + calEvent.jobCustomer.phoneNumber + "</span>" : ''
                                if (calEvent.jobId > 0) {
                                    geoCode = calEvent.geoCode != null ? "<span> <b>Geo Code</b> : " + calEvent.geoCode + "</span>" : '';
                                    status = calEvent.status != null ? "<span> <b>Status</b> : " + calEvent.status + "</span>" : '';
                                    customerClass = calEvent.jobType != null ? "<span> <b>Customer Class</b> : " + calEvent.jobType + "</span>" : '';
                                }


                                var tooltip = '<div class="tooltipevent" style="min-width:200px;min-height:20px;border-radius:0px 4px 4px 4px; padding:5px; position:absolute;z-index:10001;">'
                                    + geoCode + '<br/>' + toolTipBody + '<br/>' + phone + '<br />' + customerClass + '<br/>' + status + '</div>';
                                $("body").append(tooltip);
                            }
                            else {
                                var toolTipBody = mailSend + "<span> <b>Assignee</b> : " + calEvent.assignee + "</span> <br />"
                                    + "<span> <b>Franchisee</b> : " + calEvent.franchisee + "</span> <br />"
                                    + "<span> <b>Created By</b> : " + calEvent.createdBy + "</span> <br />"
                                    + "<span> <b>Date</b> : " + startDateFormat
                                    + " - " + endDateFormat + "</span>"
                                    + "<br/><span> <b>User Lock</b> : " + isLock + "</span> <br />";
                                status = calEvent.status != null ? "<span> <b>Status</b> : " + calEvent.status + "</span>" : '';
                                var status = '';
                                var geoCode = '';
                                var customerClass = '';

                                var tooltip = '<div class="tooltipevent" style="min-width:200px;min-height:20px;border-radius:0px 4px 4px 4px; padding:5px; position:absolute;z-index:10001;">'
                                    + geoCode + '<br/>' + toolTipBody + '<br/>' + status + '</div>';
                                $("body").append(tooltip);
                            }

                        }
                        if (calEvent.isVacation) {
                            var toolTipBody = "<span> <b>" + calEvent.assignee + " </b></span> <br />"
                                + "<span> <b>Franchisee</b> : " + calEvent.franchisee + "</span> <br />"
                                + "<span> <b>Created By</b> : " + calEvent.createdBy + "</span> <br />"
                                + "<span> <b>Date</b> : " + startDateFormat
                                + " - " + endDateFormat + "</span>";

                            var tooltip = '<div class="tooltipevent" style="min-width:200px;min-height:20px;border-radius:0px 4px 4px 4px; padding:5px; position:absolute;z-index:10001;">'
                                + toolTipBody + '</div>';
                            $("body").append(tooltip);
                        }
                        
                        $(this).mouseover(function (e) {
                            $(this).css('z-index', 10000);
                            $('.tooltipevent').fadeIn('500');
                            $('.tooltipevent').fadeTo('10', 1.9);
                            $('.tooltipevent').css('background', calEvent.backgroundColor);
                            $('.tooltipevent').css('color', '#ffffff');
                        }).mouseup(function (e) {
                            $('.tooltipevent').css('top', e.pageY + 10);
                            $('.tooltipevent').css('left', e.pageX + 20);
                            $('.tooltipevent').css('background', calEvent.backgroundColor);
                            $('.tooltipevent').css('color', '#ffffff');
                        }).mousemove(function (e) {
                            $('.tooltipevent').css('top', e.pageY + 10);
                            $('.tooltipevent').css('left', e.pageX + 20);
                            $('.tooltipevent').css('background', calEvent.backgroundColor);
                            $('.tooltipevent').css('color', '#ffffff');
                        });
                    },

                    eventMouseout: function (calEvent, jsEvent) {
                        $(this).css('z-index', 8);
                        $('.tooltipevent').remove();
                    },
                    defaultDate: getdefaultDate(),
                    eventClick: function (calEvent) {
                        if (vm.isLockCalendarClicked) {
                            return;
                        }
                        vm.query.customerName = '';
                        vm.query.text = '';
                        LocalStorageService.setSchedulerStorageValue(vm.query);
                        $window.sessionStorage.setItem("IsBeforeAfterTabAtive", false);
                        if (!$scope.isMap) {
                            if (calEvent.jobId > 0) { manageJob(calEvent.jobId, vm.previousView, calEvent.id); }
                            else if (calEvent.estimateId > 0) { manageEstimate(calEvent.estimateId, vm.previousView, calEvent.id); }
                            else if (calEvent.isVacation) { manageVacation(calEvent.id, vm.previousView); }
                            else if (calEvent.meetingId > 0) { manageMeeting(calEvent.id, vm.previousView); }
                        }
                    },
                    eventDrop: function (event, delta, revertFunc) {
                        if (event.isLock) {
                            toaster.error("Cannot Change Lock Events!!");
                            return revertFunc();
                        }
                        if (vm.isTech || vm.isSalesRep) {
                            toaster.error("Drag Drop Not allowed!!");
                            return revertFunc();
                        }
                        setDraggedStartEndDate(event, delta);
                        var currentDate = moment(new Date());
                        if (currentDate >= vm.draggedStartDate || currentDate >= vm.draggedEndDate) {
                            toaster.error("Cannot Change Past Date Events!!");
                            allowedEvent = false;
                            return revertFunc();
                        }
                        vm.dragDropQuery.id = event.id;
                        vm.dragDropQuery.start = new Date(event.actualStartDate);
                        vm.dragDropQuery.end = new Date(event.actualEndDate);
                        vm.dragDropQuery.jobId = (event.jobId);
                        vm.dragDropQuery.meetingId = event.meetingId;
                        vm.dragDropQuery.days = delta._days;
                        vm.dragDropQuery.personalId = (event.personalId);
                        vm.dragDropQuery.estimateId = (event.estimateId);
                        vm.dragDropQuery.seconds = (delta._milliseconds);
                        editSchedulerDragDrop(revertFunc, event);

                    },
                    eventAllow: function (dropInfo, draggedEvent) {
                        var startDate = new Date(draggedEvent.start);
                        var endDate = new Date(draggedEvent.end);
                        var currentDate = moment(new Date());
                        console.log(currentDate);
                        console.log(startDate);
                        console.log(endDate);
                        if (currentDate > startDate || currentDate > endDate) {
                            return false;
                        }
                        return true;
                    }
                }
            };
            function getdefaultDate() {
                return vm.defaultDate;
            }
            function eventDayClick(date, jsEvent, view) {
                LocalStorageService.setSchedulerStorageValue(vm.query);
                //vm.query.franchiseeId = $stateParams.franchiseeId == null ? 0 : $stateParams.franchiseeId;
                previousView = $('#calendar_123').fullCalendar('getView').name;
                var currentDates = new Date();
                var dateBeforeTwoMonth = new Date(currentDates.setMonth(currentDates.getMonth() - 2));
                vm.query.view = view.name;
                var startDate = moment(date);
                var dateToCompare = startDate.format('MM/DD/YYYY');
                var dateToCompare2 = moment(startDate).format('MM/DD/YYYY h:mm:ss a');
                var currentDateFormat = currentDate.format('MM/DD/YYYY');
                var isHoliday = false;

                angular.forEach(vm.holidayList, function (item) {
                    var holidayDate = moment(item.actualStartDate).format('MM/DD/YYYY');
                    if (!isHoliday) {
                        if (holidayDate == dateToCompare && !item.canSchedule) {
                            isHoliday = true;

                            if (startDate < currentDate) {
                                var startDates = new Date(startDate);
                                if (startDates >= dateBeforeTwoMonth) {
                                    notification.showConfirm("You are about to schedule a past event on a holiday . Do you want to proceed?", "Warning Message:", function () {
                                        vm.openOptionModal(startDate, vm.query, true);
                                    })
                                }
                                else {
                                    if (vm.isTech) {
                                        toaster.error("Can't create Personal/Meeting for Past Date/Time!");
                                    }
                                    else {
                                        toaster.error("Can't create Job/Estimate for Past Date/Time!");
                                    }
                                    return;
                                }
                            }
                            else {
                                notification.showConfirm("You are about to schedule a event on a holiday . Do you want to proceed?", "Warning Message:", function () {
                                    vm.openOptionModal(startDate, vm.query, false);
                                })
                            }
                        }
                        //else if (holidayDate == dateToCompare) {
                        //    notification.showConfirm("You are about to schedule a event on a holiday . Do you want to proceed?", "Warning Message:", function () {
                        //        vm.openOptionModal(startDate, vm.query, true);
                        //    })
                        //}
                    }
                    return;
                });

                if (!isHoliday) {
                    var currentDateInMoment = new Date(currentDateFormat);
                    var dateToCompareInMoment = new Date(dateToCompare);
                    //if (!vm.isTech) {
                    if (vm.query.franchiseeId <= 0) {
                        toaster.error("Please select franchisee First!");
                        return;
                    }
                    if (vm.query.view == "month" && dateToCompare == currentDateFormat) {
                        vm.openOptionModal(startDate, vm.query, false);
                    }

                    else if (currentDateInMoment > dateToCompareInMoment) {
                        var array1 = currentDateFormat.split('/');
                        var array2 = dateToCompare.split('/');

                        if (array1[2] == array2[2]) {
                            var dateToCompare = new Date(dateToCompare);

                            var dateToCompareInSeconds = new Date(dateToCompare);
                            if (dateBeforeTwoMonth < dateToCompareInSeconds && (!vm.isTech)) {

                                notification.showConfirm("You are about to schedule a past event . Do you want to proceed?", "Warning Message:", function () {
                                    vm.openOptionModal(startDate, vm.query, true);
                                });
                            }
                            else {
                                if (vm.isTech) {
                                    toaster.error("Can't create Personal/Meeting for Past Date/Time!");
                                }
                                else {
                                    toaster.error("Can't create Job/Estimate for Past Date/Time!");
                                };
                                return;
                            }
                        }
                        else {
                            var datfeBeforeTwoMonthInMoment = new Date(dateBeforeTwoMonth);
                            var dateToCompareInMoment = new Date(dateToCompare);
                            if (dateToCompareInMoment < datfeBeforeTwoMonthInMoment) {
                                if (vm.isTech) {
                                    toaster.error("Can't create Personal/Meeting for Past Date/Time!");
                                }
                                else {
                                    toaster.error("Can't create Job/Estimate for Past Date/Time!");
                                }
                                return;
                            }
                            else {
                                notification.showConfirm("You are about to schedule a past event . Do you want to proceed?", "Warning Message:", function () {
                                    vm.openOptionModal(startDate, vm.query, true);
                                });
                            }
                        }
                    }
                    else {
                        vm.openOptionModal(startDate, vm.query, false);
                    }
                    //}
                }
            }

            function renderEvent(event, element) {
                var calendarView = $('#calendar_123').fullCalendar('getView').name;
                var tickMark = "<br /><b><i class='fa fa-check-circle' aria-hidden='true' style='color:white;font-size: 18px;'></i></b><br />";
                var crossMark = "<br /><b><i class='fa fa-times-circle-o' aria-hidden='true' style='color:white;font-size: 18px;'></i></b><br />";
                var questionMark = "<br /><b><i class='fa fa-question-circle' aria-hidden='true' style='color:white;font-size: 18px;'></i></b><br />";
                var isLock = "<span class='round'>L</span>";
                var startDateFormat = moment((event.actualStartDate)).format("HH:mm");
                var endDateFormat = moment((event.actualEndDate)).format("HH:mm");
                var sendMailIcons = event.isCustomerMailSend ? " <span style='margin-left:14px;font-size:19px'><i style='font-size: 20px;margin-left: -13px;' class='fa fa-envelope' aria-hidden='true'></i></span>" : "";
                var Tech = event.assignee != null ? "<span class='pull-left'>" + event.assignee + " , </span>  <br />" : event.backgroundColor == "#26327e" ? "<span> Convention , </span> <br />" : "<span> HOLIDAY , </span> <br />";
                var geoCode = event.geoCode != null ? "<span class='pull-left'>" + event.geoCode + "  </span> <span class='pull-right'>" + startDateFormat + " - " + endDateFormat + "</span>  <br />" : '';
                var info = event.jobCustomer != null ? " <span class='pull-left'>" + event.jobCustomer.address.city + "</span> <br />" : '';
                var image = "<button type=" + "button" + " class=" + "img-link" + " id=btn_img" + " data-ng-click=" + "vm.googleApi(" + event.jobId + "," + event.estimateId + "); $event.stopPropagation()" + "><img src=" + "/Content/images/map.png" + " height=" + "40" + " width=" + "40" + " style=" + "float:right;" + "/></button>";
                if (calendarView != "agendaDay") {
                    //var invoiceIcons = event.amount != 0.00 ? " <span style='margin-left:14px;font-size:15px'><i style='font-size: 15px;margin-left: -13px;' class='fa fa-usd' aria-hidden='true'></i>" + priceForEvent.toFixed(1) + "</span>" : "";
                    var invoiceIcons = "";
                    var marketingClassType = "";
                    if (event.isInvoicePresent) {
                        var priceForEvent = (event.invoiceAmount / 1000);
                        invoiceIcons = " <span style='margin-left:14px;font-size:15px'><i style='font-size: 15px;margin-left: -13px;' class='fa fa-usd' aria-hidden='true'></i>" + priceForEvent.toFixed(1) + "</span>"
                    }
                    else if (!event.isInvoicePresent) {
                        if (event.estimatedAmount <= 0) {
                            invoiceIcons = " <span style='margin-left:14px;font-size:15px'><i style='font-size: 15px;margin-left: -13px;' class='fa fa-usd' aria-hidden='true'></i>" + "?" + "</span>"
                        }
                        else {
                            var priceForEvent = (event.estimatedAmount / 1000);
                            invoiceIcons = " <span style='margin-left:14px;font-size:15px'><i style='font-size: 15px;margin-left: -13px;' class='fa fa-usd' aria-hidden='true'></i>" + priceForEvent.toFixed(1) + "</span>"
                        }
                    }
                    else {
                        invoiceIcons = " <span style='margin-left:14px;font-size:15px'><i style='font-size: 15px;margin-left: -13px;' class='fa fa-usd' aria-hidden='true'></i>" + "?" + "</span>"
                    }
                    if (event.estimateType == "RESIDENTIAL" || event.estimateType == "CONDO" || event.estimateType == "RESIDENTIALPROPERTYMGMT"
                        || event.jobType == "RESIDENTIAL" || event.jobType == "CONDO" || event.jobType == "RESIDENTIALPROPERTYMGMT") {
                        marketingClassType = " <span style='font-size:15px; font-weight:bold;'>" + "R" + "</span>"
                    }
                    else if (event.estimateType == "AUTO" || event.estimateType == "BANK" || event.estimateType == "CHURCH" || event.estimateType == "CLUB" || event.estimateType == "EDUCATION" || event.estimateType == "GROCERY" ||
                        event.estimateType == "HOTEL" || event.estimateType == "MUSEUM" || event.estimateType == "RESTAURANT" || event.estimateType == "RETAILSTORE" || event.estimateType == "OTHER" || event.estimateType == "THEATER" ||
                        event.estimateType == "YACHT" || event.estimateType == "COMMERCIAL" || event.estimateType == "CORPORATE" || event.estimateType == "FINANCIAL" || event.estimateType == "GOVERNMENT" || event.estimateType == "INSURANCE" ||
                        event.estimateType == "LEGAL" || event.estimateType == "MEDICAL" || event.estimateType == "PROFESSIONAL" || event.estimateType == "BUILDER" || event.estimateType == "FLOORING(CONTRACTOR&RETAIL)" || event.estimateType == "INTERIORDESIGN" ||
                        event.estimateType == "HOMEMANAGEMENT" || event.estimateType == "JANITORIAL"
                        || event.jobType == "AUTO" || event.jobType == "BANK" || event.jobType == "CHURCH" || event.jobType == "CLUB" || event.jobType == "EDUCATION" || event.jobType == "GROCERY" ||
                        event.jobType == "HOTEL" || event.jobType == "MUSEUM" || event.jobType == "RESTAURANT" || event.jobType == "RETAILSTORE" || event.jobType == "OTHER" || event.jobType == "THEATER" ||
                        event.jobType == "YACHT" || event.jobType == "COMMERCIAL" || event.jobType == "CORPORATE" || event.jobType == "FINANCIAL" || event.jobType == "GOVERNMENT" || event.jobType == "INSURANCE" ||
                        event.jobType == "LEGAL" || event.jobType == "MEDICAL" || event.jobType == "PROFESSIONAL" || event.jobType == "BUILDER" || event.jobType == "FLOORING(CONTRACTOR&RETAIL)" || event.jobType == "INTERIORDESIGN" ||
                        event.jobType == "HOMEMANAGEMENT" || event.jobType == "JANITORIAL") {
                        marketingClassType = " <span style='font-size:15px; font-weight:bold;'>" + "C" + "</span>";
                    }
                    else if (event.estimateType == "0MLD" || event.estimateType == "0MLFS" || event.jobType == "0MLD" || event.jobType == "0MLFS") {
                        marketingClassType = " <span style='font-size:15px; font-weight:bold;'>" + "0" + "</span>";
                    }
                    else if (event.estimateType == "NATIONAL" || event.jobType == "NATIONAL") {
                        marketingClassType = " <span style='font-size:15px; font-weight:bold;'>" + "N" + "</span>";
                    }
                    else if (event.estimateType == "FRONTOFFICE" || event.jobType == "FRONTOFFICE") {
                        marketingClassType = " <span style='font-size:15px; font-weight:bold;'>" + "F" + "</span>";
                    }
                    else {
                        marketingClassType = "";
                    }
                    element.find(".fc-title").prepend(Tech);
                    element.find(".fc-title").prepend(geoCode);
                    element.find(".fc-title").append(info);
                    if (event.isLock) {
                        element.find(".fc-title").append(isLock);
                    }
                    if (event.jobId > 0 || event.estimateId > 0) {
                        var temp = $compile(image)($scope);
                        element.find(".fc-title").append(sendMailIcons);
                        element.find(".fc-title").append(marketingClassType);
                        element.find(".fc-title").append(invoiceIcons);
                        element.find(".fc-title").append(temp);
                    }
                }
                if (calendarView == "agendaDay") {
                    var priceForEvent = event.amount != 0.00 ? (event.amount / 1000) : null;
                    var invoiceIcons = event.amount != 0.00 ? " <span class='pull-right' style='font-size:10px'><i style='font-size: 10px;margin-left: -13px;' class='fa fa-usd' aria-hidden='true'></i>" + priceForEvent.toFixed(1) + "</span>" : "";

                    element.find(".fc-title").prepend(Tech);
                    element.find(".fc-title").prepend(geoCode);
                    element.find(".fc-title").append(info);
                    if (event.jobId > 0 || event.estimateId > 0) {
                        var temp = $compile(image)($scope);
                        element.find(".fc-title").append(sendMailIcons);
                        element.find(".fc-time").append(invoiceIcons);
                        element.find(".fc-title").append(temp);
                    }
                }

                if (event.schedulerStatus == 218) {
                    element.find(".fc-title").append(tickMark);
                }
                if (event.schedulerStatus == 217) {
                    element.find(".fc-title").append(crossMark);
                }
                //if (event.schedulerStatus == 216) {
                //    element.find(".fc-title").append(questionMark);
                //}
            }
            function manageMeeting(id, previousView) {
                vm.query.customerName = '';
                vm.query.text = '';
                var render = $('#calendar_123').fullCalendar('getView').name;
                $state.go('core.layout.scheduler.meeting', { id: id, previousView: render });
            }
            function manageVacation(id, previousView) {
                vm.query.customerName = '';
                vm.query.text = '';
                var render = $('#calendar_123').fullCalendar('getView').name;
                $state.go('core.layout.scheduler.vacation', { id: id, previousView: render });
            }
            function manageSchedulerList() {
                var currentDateSelected = $('#calendar_123').fullCalendar('getDate');
                currentDateSelected = currentDateSelected._d;
                vm.query.endDate = new Date(currentDateSelected.getFullYear(), currentDateSelected.getMonth() + 1, 0);
                var calenderMonthInSeconds = new Date(vm.query.endDate).setDate(vm.query.endDate.getDate() + 4);
                var calenderDate = new Date(calenderMonthInSeconds);
                vm.query.endDate = calenderDate;
                vm.query.startDate = new Date(currentDateSelected.getFullYear(), currentDateSelected.getMonth(), 1);
                $state.go('core.layout.scheduler.list', { franchiseeId: vm.query.franchiseeId, startDate: vm.query.startDate, previousView: vm.previousView });
            }

            function manageJob(id, previousView, rowid) {
                vm.query.customerName = '';
                vm.query.text = '';
                var render = $('#calendar_123').fullCalendar('getView').name;
                $state.go('core.layout.scheduler.job', {
                    id: id,
                    previousView: render,
                    rowId: rowid
                });
            }

            function manageEstimate(id, previousView, rowid) {
                vm.query.customerName = '';
                vm.query.text = '';
                var render = $('#calendar_123').fullCalendar('getView').name;
                $window.sessionStorage.setItem("IsBeforeAfterTabAtive", false);
                $state.go('core.layout.scheduler.estimate', { id: id, previousView: render, rowId: rowid });
            }

            function openOptionModal(date, query, isFromPast) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/scheduler-option-modal.client.view.html',
                    controller: 'SchedulerOptionController',
                    controllerAs: 'vm',
                    size: 'md',
                    resolve: {
                        modalParam: function () {
                            return {
                                Date: date,
                                Query: query,
                                isFromPast: isFromPast
                            };
                        }
                    },

                    backdrop: 'static',
                });
                modalInstance.result.then(function (result) {
                    if (result == vm.ScheduleType.Estimate) {
                        vm.isFromLoadFunction = false;
                        openCreateEstimatepopup(date, query);
                    }
                    else if (result == vm.ScheduleType.Job) {
                        vm.isFromLoadFunction = false;
                        openCreateJobPopup(date, query);
                    }
                    else if (result == vm.ScheduleType.Vacation) {
                        vm.isFromLoadFunction = false;
                        openMarkVacationPopup(date, query);
                    }
                    else if (result == vm.ScheduleType.Meeting) {
                        vm.isFromLoadFunction = false;
                        openMarkMeetingPopup(date, query);
                    }
                }, function () {
                });
            }
            function openMarkMeetingPopup(date, query) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/create-meeting.client.view.html',
                    controller: 'CreateMeetingController',
                    controllerAs: 'vm',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                Date: date,
                                Query: query,
                                Franchisee: vm.franchisee[0].display
                            };
                        }
                    },
                    backdrop: 'static',
                });
                modalInstance.result.then(function () {
                    vm.isFromJobEstimate = true;
                    getEvents();
                }, function () {
                });
            }
            function openMarkVacationPopup(date, query) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/create-vacation.client.view.html',
                    controller: 'CreateVacationController',
                    controllerAs: 'vm',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                Date: date,
                                Query: query
                            };
                        }
                    },
                    backdrop: 'static',
                });
                modalInstance.result.then(function () {
                    getEvents();
                    vm.isFromJobEstimate = true;
                }, function () {
                });
            }

            function openCreateJobPopup(date, query) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/create-job.client.view.html',
                    controller: 'CreateJobController',
                    controllerAs: 'vm',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                Date: date,
                                Query: query
                            };
                        }
                    },
                    backdrop: 'static',
                });
                modalInstance.result.then(function () {
                    vm.isFromJobEstimate = true;
                    getEvents();
                }, function () {
                });
            }

            function openCreateEstimatepopup(date, query) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/create-estimate.client.view.html',
                    controller: 'CreateEstimateController',
                    controllerAs: 'vm',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                Date: date,
                                Query: query
                            };
                        }
                    },
                    backdrop: 'static',
                });
                modalInstance.result.then(function () {
                    vm.isFromJobEstimate = true;
                    getEvents();
                }, function () {
                });
            }

            function getmarketingClassCollection() {
                return customerService.getmarketingClassCollection().then(function (result) {
                    vm.marketingClass = result.data;
                });
            }

            function month() {
                vm.isFromJobEstimate = false;
                vm.previousView = 'month';
                vm.previousview = 'month';
                var myEl = angular.element(document.querySelector('#month'));
                myEl.addClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#week'));
                myEl.removeClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#day'));
                myEl.removeClass('fc-state-active');
                $('#calendar_123').fullCalendar('changeView', 'month');
                SaveDefaultView('month');
                disableOrEnableTodayButton();
            }

            function week() {
                vm.isFromJobEstimate = false;
                vm.previousView = 'agendaWeek';
                vm.previousview = 'agendaWeek';
                var myEl = angular.element(document.querySelector('#month'));
                myEl.removeClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#week'));
                myEl.addClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#day'));
                myEl.removeClass('fc-state-active');
                $('#calendar_123').fullCalendar('changeView', 'agendaWeek');
                SaveDefaultView('agendaWeek');
                disableOrEnableTodayButton();
            }

            function day() {
                vm.isFromJobEstimate = false;
                vm.previousView = 'agendaDay';
                vm.previousview = 'agendaDay';
                var myEl = angular.element(document.querySelector('#month'));
                myEl.removeClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#week'));
                myEl.removeClass('fc-state-active');
                var myEl = angular.element(document.querySelector('#day'));
                myEl.addClass('fc-state-active');
                $('#calendar_123').fullCalendar('changeView', 'agendaDay');
                SaveDefaultView('agendaDay');
                disableOrEnableTodayButton();
            }

            function prev() {
                vm.isFromJobEstimate = false;
                vm.isTodayDisable = false;
                vm.buttomDisabled = true;
                vm.isFromLoad = false;
                vm.ischanged = true;
                $('#calendar_123').fullCalendar('prev');
                testing();
                $timeout(callAtTimeout, 2000);
                disableOrEnableTodayButton();
            }

            function next() {
                vm.isFromJobEstimate = false;
                vm.buttomDisabled = true;
                vm.isFromLoad = false;
                vm.ischanged = true;
                vm.isTodayDisable = false;
                $('#calendar_123').fullCalendar('next');
                testing();
                $timeout(callAtTimeout, 2000);
                disableOrEnableTodayButton();
            }

            function callAtTimeout() {
                vm.buttomDisabled = false;
            }

            function cleanEvents() {
                $scope.eventSources = [];
            }

            function getEvents() {
                vm.isFromLoad = true;
                vm.events = [];
                vm.isDataLoaded = false;
                $scope.eventSources = [];
                $scope.today = clock.getStartDateOfMonth();
                var currentDate = new Date();
                CheckForView();
                vm.query.endDate = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
                vm.query.startDate = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1);
                schedulerService.getJobList(vm.query).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.events = result.data.collection;
                    }
                    $scope.eventSources = [vm.events];
                });
            }

            function getHolidayList() {
                disableOrEnableTodayButton();
                vm.holidayQuery.startDate = vm.query.startDate;
                vm.holidayQuery.endDate = vm.query.endDate;
                vm.holidayQuery.franchiseeId = vm.query.franchiseeId;
                schedulerService.getHolidayListMonthWise(vm.holidayQuery).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.holidayList = result.data.collection;
                        $scope.eventSources = [vm.events];
                    }
                });
            }

            function getFranchiseeCollection() {
                return franchiseeService.getFranchiseeNameValuePair(vm.query).then(function (result) {
                    vm.franchiseeCollection = result.data;
                    vm.franchisee = $filter('filter')(vm.franchiseeCollection, { value: vm.query.franchiseeId }, true);
                    if (vm.franchisee.length <= 0)
                        vm.franchisee = $filter('filter')(vm.franchiseeCollection, { id: vm.query.franchiseeId }, true);
                    vm.franchiseeName = vm.franchisee.length > 0 ? vm.franchisee[0].display : null;
                    $rootScope.title = vm.franchiseeName + ' - MarbleLife ';
                    setTitle();
                });
            }

            function getAssigneeList() {
                return schedulerService.getAssigneeListForScheduler(vm.query).then(function (result) {
                    vm.techList = result.data;
                });
            }

            function getSchedulerStatusList() {
                var status = DataHelper.Status.getValue(vm.query.selectedOption);
                if (status == 'All')
                    vm.query.status = true;
                if (status == 'Active')
                    vm.query.status = true;
                if (status == 'Inactive')
                    vm.query.status = false;
                getAssigneeList();
            }

            function setTitle() {
                if (vm.franchiseeName != null) {
                    $scope.$emit("update-title", "Manage Scheduler - " + vm.franchiseeName);
                    if (vm.isTech || vm.isSalesRep || vm.isExecutive) {
                        $scope.$emit("update-title", "Dashboard - " + vm.franchiseeName);
                    }
                }
            }

            function getOperatingSystem() {
                var unknown = '-';
                var width = '';
                var height = '';
                // screen
                var screenSize = '';
                if (screen.width) {
                    width = (screen.width) ? screen.width : '';
                    height = (screen.height) ? screen.height : '';
                    screenSize += '' + width + " x " + height;
                }

                // browser
                var nVer = navigator.appVersion;
                var nAgt = navigator.userAgent;
                var browser = navigator.appName;
                var version = '' + parseFloat(navigator.appVersion);
                var majorVersion = parseInt(navigator.appVersion, 10);
                var nameOffset, verOffset, ix;

                // Opera
                if ((verOffset = nAgt.indexOf('Opera')) != -1) {
                    browser = 'Opera';
                    version = nAgt.substring(verOffset + 6);
                    if ((verOffset = nAgt.indexOf('Version')) != -1) {
                        version = nAgt.substring(verOffset + 8);
                    }
                }
                // Opera Next
                if ((verOffset = nAgt.indexOf('OPR')) != -1) {
                    browser = 'Opera';
                    version = nAgt.substring(verOffset + 4);
                }
                // Edge
                else if ((verOffset = nAgt.indexOf('Edge')) != -1) {
                    browser = 'Microsoft Edge';
                    version = nAgt.substring(verOffset + 5);
                }
                // MSIE
                else if ((verOffset = nAgt.indexOf('MSIE')) != -1) {
                    browser = 'Microsoft Internet Explorer';
                    version = nAgt.substring(verOffset + 5);
                }
                // Chrome
                else if ((verOffset = nAgt.indexOf('Chrome')) != -1) {
                    browser = 'Chrome';
                    version = nAgt.substring(verOffset + 7);
                }
                // Safari
                else if ((verOffset = nAgt.indexOf('Safari')) != -1) {
                    browser = 'Safari';
                    version = nAgt.substring(verOffset + 7);
                    if ((verOffset = nAgt.indexOf('Version')) != -1) {
                        version = nAgt.substring(verOffset + 8);
                    }
                }
                // Firefox
                else if ((verOffset = nAgt.indexOf('Firefox')) != -1) {
                    browser = 'Firefox';
                    version = nAgt.substring(verOffset + 8);
                }
                // MSIE 11+
                else if (nAgt.indexOf('Trident/') != -1) {
                    browser = 'Microsoft Internet Explorer';
                    version = nAgt.substring(nAgt.indexOf('rv:') + 3);
                }
                // Other browsers
                else if ((nameOffset = nAgt.lastIndexOf(' ') + 1) < (verOffset = nAgt.lastIndexOf('/'))) {
                    browser = nAgt.substring(nameOffset, verOffset);
                    version = nAgt.substring(verOffset + 1);
                    if (browser.toLowerCase() == browser.toUpperCase()) {
                        browser = navigator.appName;
                    }
                }
                // trim the version string
                if ((ix = version.indexOf(';')) != -1) version = version.substring(0, ix);
                if ((ix = version.indexOf(' ')) != -1) version = version.substring(0, ix);
                if ((ix = version.indexOf(')')) != -1) version = version.substring(0, ix);

                majorVersion = parseInt('' + version, 10);
                if (isNaN(majorVersion)) {
                    version = '' + parseFloat(navigator.appVersion);
                    majorVersion = parseInt(navigator.appVersion, 10);
                }

                // mobile version
                var mobile = /Mobile|mini|Fennec|Android|iP(ad|od|hone)/.test(nVer);

                // cookie
                var cookieEnabled = (navigator.cookieEnabled) ? true : false;

                if (typeof navigator.cookieEnabled == 'undefined' && !cookieEnabled) {
                    document.cookie = 'testcookie';
                    cookieEnabled = (document.cookie.indexOf('testcookie') != -1) ? true : false;
                }

                // system
                var os = unknown;
                var clientStrings = [
                    { s: 'Windows 10', r: /(Windows 10.0|Windows NT 10.0)/ },
                    { s: 'Windows 8.1', r: /(Windows 8.1|Windows NT 6.3)/ },
                    { s: 'Windows 8', r: /(Windows 8|Windows NT 6.2)/ },
                    { s: 'Windows 7', r: /(Windows 7|Windows NT 6.1)/ },
                    { s: 'Windows Vista', r: /Windows NT 6.0/ },
                    { s: 'Windows Server 2003', r: /Windows NT 5.2/ },
                    { s: 'Windows XP', r: /(Windows NT 5.1|Windows XP)/ },
                    { s: 'Windows 2000', r: /(Windows NT 5.0|Windows 2000)/ },
                    { s: 'Windows ME', r: /(Win 9x 4.90|Windows ME)/ },
                    { s: 'Windows 98', r: /(Windows 98|Win98)/ },
                    { s: 'Windows 95', r: /(Windows 95|Win95|Windows_95)/ },
                    { s: 'Windows NT 4.0', r: /(Windows NT 4.0|WinNT4.0|WinNT|Windows NT)/ },
                    { s: 'Windows CE', r: /Windows CE/ },
                    { s: 'Windows 3.11', r: /Win16/ },
                    { s: 'Android', r: /Android/ },
                    { s: 'Open BSD', r: /OpenBSD/ },
                    { s: 'Sun OS', r: /SunOS/ },
                    { s: 'Linux', r: /(Linux|X11)/ },
                    { s: 'iOS', r: /(iPhone|iPad|iPod)/ },
                    { s: 'Mac OS X', r: /Mac OS X/ },
                    { s: 'Mac OS', r: /(MacPPC|MacIntel|Mac_PowerPC|Macintosh)/ },
                    { s: 'QNX', r: /QNX/ },
                    { s: 'UNIX', r: /UNIX/ },
                    { s: 'BeOS', r: /BeOS/ },
                    { s: 'OS/2', r: /OS\/2/ },
                    { s: 'Search Bot', r: /(nuhk|Googlebot|Yammybot|Openbot|Slurp|MSNBot|Ask Jeeves\/Teoma|ia_archiver)/ }
                ];
                for (var id in clientStrings) {
                    var cs = clientStrings[id];
                    if (cs.r.test(nAgt)) {
                        os = cs.s;
                        break;
                    }
                }

                var osVersion = unknown;

                if (/Windows/.test(os)) {
                    osVersion = /Windows (.*)/.exec(os)[1];
                    os = 'Windows';
                }

                switch (os) {
                    case 'Mac OS X':
                        osVersion = /Mac OS X (10[\.\_\d]+)/.exec(nAgt)[1];
                        break;

                    case 'Android':
                        osVersion = /Android ([\.\_\d]+)/.exec(nAgt)[1];
                        break;

                    case 'iOS':
                        osVersion = /OS (\d+)_(\d+)_?(\d+)?/.exec(nVer);
                        osVersion = osVersion[1] + '.' + osVersion[2] + '.' + (osVersion[3] | 0);
                        break;
                }

                var flashVersion = 'no check';
                if (typeof swfobject != 'undefined') {
                    var fv = swfobject.getFlashPlayerVersion();
                    if (fv.major > 0) {
                        flashVersion = fv.major + '.' + fv.minor + ' r' + fv.release;
                    }
                    else {
                        flashVersion = unknown;
                    }
                }

                return {
                    screen: screenSize,
                    browser: browser,
                    browserVersion: version,
                    browserMajorVersion: majorVersion,
                    mobile: mobile,
                    os: os,
                    osVersion: osVersion,
                    cookies: cookieEnabled,
                    flashVersion: flashVersion
                }
            }

            function testing() {
                vm.events = [];
                vm.ischanged = true;
                vm.isDataLoaded = false;
                vm.isFromLoadFunction = false;
                var month = 1;
                var length = $scope.eventSources.length;

                    for (var i = 0; i < length; i++) {
                        $scope.eventSources[i] = [];
                    }
                    $scope.eventSources[0] = [];
                    vm.isFromLoad = false;
                    CheckForView();
                    var dates = new Date(vm.query.endDate);
                    if (dates.getMonth() == 0) {
                        month = dates.getMonth() + 2;
                    }
                    else {
                        month = dates.getMonth() + 1;
                    }

                    var dateStart = new Date(vm.query.startDate);
                    var monForCal = "";
                    if (dateStart.getDate() > 1) {
                        monForCal = dateStart.getMonth();
                    }
                    else {
                        monForCal = dateStart.getMonth() - 1;
                    }

                    var calendarView = $('#calendar_123').fullCalendar('getView').name;
                    if (calendarView == 'month') {
                        var d = new Date(new Date(vm.query.endDate).getFullYear(), monForCal + 1, 1);
                        var startDate1 = $filter('date')(d, 'yyyy-MM-dd');
                        var startDateforEndDate = new Date(startDate1);
                        var lastDayOfMonth = new Date(startDateforEndDate.getFullYear(), startDateforEndDate.getMonth() + 1, 0)
                        var endDate1 = $filter('date')(lastDayOfMonth, 'yyyy-MM-dd');
                    }
                    else {
                        var dateStart = new Date(vm.query.startDate);
                        var dateEnd = new Date(vm.query.endDate);
                        var startDate1 = $filter('date')(dateStart, 'yyyy-MM-dd');
                        var endDate1 = $filter('date')(dateEnd, 'yyyy-MM-dd');
                    }



                    vm.query.endDateForCal = endDate1;
                    vm.query.startDateForCal = startDate1;


                    schedulerService.getJobList(vm.query).then(function (result) {
                        vm.totalAmount = 0.00;
                        //vm.totalJobAmount = 0.00;
                        if (result != null && result.data != null) {
                            vm.events = result.data.collection;
                            vm.totalSum = result.data.totalSum;
                        }
                        var calendarView = $('#calendar_123').fullCalendar('getView').name;
                        if (calendarView != 'month') {
                            var startDate1 = $filter('date')(vm.query.startDate, 'yyyy-MM-dd');
                            var endDate1 = $filter('date')(vm.query.endDate, 'yyyy-MM-dd');
                        }
                        else {
                            var startDate1 = $filter('date')($('#calendar_123').fullCalendar('getView').intervalStart._d, 'yyyy-MM-dd');
                            var startDateforEndDate = new Date(startDate1);
                            var lastDayOfMonth = new Date(startDateforEndDate.getFullYear(), startDateforEndDate.getMonth() + 1, 0)
                            var endDate1 = $filter('date')(lastDayOfMonth, 'yyyy-MM-dd');
                        }
                        angular.forEach(vm.events, function (value1) {
                            var eventStartDate = $filter('date')(value1.actualStartDate, 'yyyy-MM-dd');
                            var eventEndDate = $filter('date')(value1.actualEndDate, 'yyyy-MM-dd');
                            if (eventStartDate >= startDate1 && eventEndDate <= endDate1) {
                                if (value1.isInvoicePresent) {
                                    vm.totalAmount = vm.totalAmount + value1.invoiceAmount;
                                }
                                else if (!value1.isInvoicePresent && value1.invoiceAmount <= 0 ) {
                                    vm.totalAmount = vm.totalAmount + value1.estimatedAmount;
                                }
                                else {
                                    vm.totalAmount = vm.totalAmount + value1.invoiceAmount;
                                }
                               // vm.totalJobAmount = vm.totalJobAmount + value1.jobAmount;
                            }
                        });
                        vm.totalAmount = vm.totalAmount.toFixed(2);
                       // vm.totalJobAmount = vm.totalJobAmount.toFixed(2);
                        $scope.eventSources[0] = (vm.events);
                        deleteScheduler();
                        holidayTesting();
                    });
                }

            function holidayTesting() {
                /*$scope.eventSources[1] = [];*/
                vm.holidayQuery.startDate = vm.query.startDate;
                vm.holidayQuery.endDate = vm.query.endDate;
                vm.holidayQuery.franchiseeId = vm.query.franchiseeId;

                schedulerService.getHolidayListMonthWise(vm.holidayQuery).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.holidayList = result.data.collection;
                        vm.isPresent = false;
                        //angular.forEach($scope.eventSources, function (value, key) {
                        //    if (value.length > 0) {
                        //        if (value[0].isHoliday) {
                        //            vm.isPresent = true;
                        //            return true;
                        //        }
                        //    }
                        //})

                        if (!vm.isPresent) {
                            $scope.eventSources[1] = (vm.holidayList);
                            $scope.eventSources[0] = (vm.events);
                            $timeout(callAtTimeout, 900);
                        }

                        //if ($scope.eventSources[0] != undefined && $scope.eventSources[1] != undefined) {
                        //    if ($scope.eventSources[0].length == $scope.eventSources[1].length) {
                        //        $scope.eventSources[0] = [];
                        //    }
                        //}


                    }
                })
            }
            function deleteScheduler() {
                angular.forEach($scope.eventSources, function (value, key) {
                    var length = $scope.eventSources.length;

                    if ((key + 1) != length) {
                        $scope.eventSources.splice(key, 1);
                        $('#calendar_123').fullCalendar("rerenderEvents");
                    }
                });
            }
            function getSchedulerLockList() {
                $scope.lockColorCode = !$scope.lockColorCode;
                if ($scope.lockColorCode) {
                    vm.query.status = true;
                    vm.query.isLock = true;
                    vm.lockName = "Lock User";
                    vm.lockColorCode = "green";
                }
                else {
                    vm.query.status = false;
                    vm.query.isLock = false;
                    vm.lockName = "Unlock User";
                    vm.lockColorCode = "#334499";
                }
                vm.query.isEmpty = 0;
                getEvents();
                getAssigneeList();
            }
            function load() {
                var details = getOperatingSystem();
                // console.log(details);
            }
            $scope.loadEvents = function (view, element, $scope) {
                var month = (view.title);
                var categoryType = (view.type);
                if (vm.ischanged) {
                    getList();
                    vm.ischanged = !vm.ischanged;
                }
            }

            $scope.$on('navigationDate', function (event, data) {
                vm.isNavigated = false;
                vm.nativateDate = data
            });

            function editSchedulerDragDrop(revertFunc, event) {
                return schedulerService.dragDropScheduler(vm.dragDropQuery).then(function (result) {
                    vm.dragStatus = result.data;
                    if (vm.dragStatus == 3) {
                        toaster.error("Technician(s)/Sales(s) are not available!!");
                        return revertFunc();
                    }
                    else if (vm.dragStatus == 1) {
                        var startDateMilliSeconds = new Date(new Date(event.actualStartDate).setDate(new Date(event.actualStartDate).getDate() + vm.dragDropQuery.days)).setMilliseconds(vm.dragDropQuery.seconds);
                        var endDateMilliSeconds = new Date(new Date(event.actualEndDate).setDate(new Date(event.actualEndDate).getDate() + vm.dragDropQuery.days)).setMilliseconds(vm.dragDropQuery.seconds);
                        var startDate = new Date(startDateMilliSeconds);
                        var endDate = new Date(endDateMilliSeconds);
                        event.actualEndDate = endDate;
                        event.actualStartDate = startDate;
                        toaster.show("Date/Time Changed Successfully!!");
                        $('#calendar_123').fullCalendar('updateEvent', event);
                        return true;
                    }
                    else if (vm.dragStatus == 2) {
                        toaster.error("Error in Changing Date/Time !!");
                        return revertFunc();
                    }
                });
            }

            function getCurrentView() {
                var isMobile = navigator.userAgent.match(/(iPhone|iPod|iPad|Android|webOS|BlackBerry|IEMobile|Opera Mini)/i);
                if (isMobile) {
                    vm.isMobileView = true;
                    vm.previousView = "agendaDay";
                    $('#calendar_123').fullCalendar('changeView', 'agendaDay');
                }
                else {
                    vm.isMobileView = false;
                    vm.previousView = "month";
                    $('#calendar_123').fullCalendar('changeView', 'month');

                }
            }
            function printScreen() {
                var isClear = false;
                var currentContent = '';
                var contentLength = [];
                var lengthWidget = 0;
                var lengthUsingLoop = 0;
                $('fc-row fc-week fc-widget-content').show().removeClass("page-break-before").removeClass("page-break-after");
                var widgetContent = document.getElementsByClassName("fc-row fc-week fc-widget-content");
                for (var i = 0; i < widgetContent.length; i++) {
                    currentContent = widgetContent[i].innerHTML;
                    var length1 = $('.fc-row.fc-week.fc-widget-content:nth-child(' + (i + 1) + ')');
                    var length = $('.fc-row.fc-week.fc-widget-content:nth-child(' + (i + 1) + ') >.fc-content-skeleton table tbody tr').length;
                    for (var a = 0; a < length1.length; a++) {
                        var localHtml = length1[a];
                        if (localHtml.offsetHeight != 61) {
                            lengthUsingLoop = length;
                        }
                        else {
                            lengthUsingLoop = 0;
                        }

                    }
                    contentLength[i] = lengthUsingLoop;
                }

                for (var i = 1; i < widgetContent.length; i++) {

                    if (contentLength[i - 1] > 11) {
                        lengthWidget = lengthWidget + (contentLength[i - 1] - 11);
                    }
                    else {
                        lengthWidget = lengthWidget + contentLength[i - 1]
                    }

                    if (isClear) {
                        lengthWidget = 0;
                    }
                    if (lengthWidget > 11) {
                        lengthWidget = 0;
                    }
                    if (contentLength[i] + lengthWidget > 14) {
                        var leng = $('.fc-row.fc-week.fc-widget-content:nth-child(' + (i + 1) + ')');
                        $('.fc-row.fc-week.fc-widget-content:nth-child(' + (i + 1) + ')').addClass(' page-break-after');
                        lengthWidget = 0;
                        isClear = true;
                    }
                    else {
                        isClear = false;
                    }
                }

                var button = document.getElementById("printButton");
                button.click();
            }


            $scope.isCalendarLock = false
            function onLockCalendar() {
                $scope.isCalendarLock = !$scope.isCalendarLock
                vm.isLockCalendarClicked = !vm.isLockCalendarClicked;
            }

            $scope.isExpand = false;
            $scope.expandCalender = function () {
                if ($scope.isExpand) {
                    $scope.isExpand = false;
                } else {
                    $scope.isExpand = true;
                }
            }


            if ($window.innerWidth < 992) {
                $scope.hideBox = true;
            }

            else {
                $scope.hideBox = false;
            }
            $scope.toggleBox = function () {
                $scope.hideBox = !$scope.hideBox;
            }

            if ($window.innerWidth < 767) {
                $scope.isSearchExpand = true;
            }


            $scope.showSearch = function () {
                $scope.isSearchExpand = !$scope.isSearchExpand;
                $window.scrollTo(0, 120);
            }

            $scope.$watch('vm.previousview', function (nv, ov) {
                if (vm.previousvistew == "month") {
                    $('#calendar_123').fullCalendar('changeView', 'month');
                }
                else if (vm.previousview == "agendaweek") {
                    $('#calendar_123').fullCalendar('changeView', 'agendaWeek');
                }
                else if (vm.previousview == "agendaDay") {
                    $('#calendar_123').fullCalendar('changeView', 'agendaDay');
                }
            });


            function CheckForView() {
                vm.previousview = $('#calendar_123').fullCalendar('getView').name;
            }
            function setDraggedStartEndDate(event, delta) {
                var startDate = moment(event.actualStartDate);
                var endDate = moment(event.actualEndDate);
                startDate.add(delta._days, 'days');
                startDate.add(delta._milliseconds, 'milliseconds');
                endDate.add(delta._days, 'days');
                endDate.add(delta._milliseconds, 'milliseconds');
                vm.draggedStartDate = startDate;
                vm.draggedEndDate = endDate;
            }

            function print() {
                var btnImg = document.getElementById('btn_img');
                btnImg.className += " hide-during-print";
            }

            function getSearch() {
                $("#calendar_123").fullCalendar('removeEvents');
                testing();
            }

            function SaveDefaultView(defaultView) {
                return userService.saveCalendarDefaultView(defaultView).then(function (result) {
                });
            }

            function getDefaultView() {
                return userService.getDefaultView().then(function (result) {
                    if (vm.isFromLoadFunction && !isMobile && !vm.isNavigated) {
                        $("#calendar_123").fullCalendar('removeEvents');
                        vm.defaultView = result.data;
                    }
                });
            }


            function today() {
                vm.buttomDisabled = true;
                vm.isFromLoad = false;
                vm.ischanged = true;
                vm.isTodayDisable = true;
                $('#calendar_123').fullCalendar('today');
                testing();
                $timeout(callAtTimeout, 2000);
            }

            function disableOrEnableTodayButton() {
                var calendarDate = new Date($('#calendar_123').fullCalendar('getDate'));
                var calendarView = $('#calendar_123').fullCalendar('getView').name;
                if (calendarView == 'month') {
                    if (calendarDate != 'Invalid Date') {
                        var calendarYear = calendarDate.getMonth();
                        var currentYear = new Date().getMonth();
                        if (calendarYear == currentYear) {
                            vm.isTodayDisable = true;
                        }
                        else {
                            vm.isTodayDisable = false;
                        }
                    }
                }
                else if (calendarView == 'agendaWeek') {

                    var firstDate = $filter('date')(vm.firstDateOfCalendar, "dd/MM/yyyy");
                    var lastDate = $filter('date')(vm.lastDateOfCalendar, "dd/MM/yyyy");
                    var currentDate = new Date();

                    if (vm.firstDateOfCalendar < currentDate && vm.lastDateOfCalendar > currentDate) {
                        vm.isTodayDisable = true;
                    }
                    else {
                        vm.isTodayDisable = false;
                    }

                }
                else if (calendarView == 'agendaDay') {

                    var calendarDateFormated = $filter('date')(calendarDate, "dd/MM/yyyy");
                    var currentDateFormated = $filter('date')(new Date(), "dd/MM/yyyy");
                    if (calendarDateFormated == currentDateFormated) {
                        vm.isTodayDisable = true;
                    }
                    else {
                        vm.isTodayDisable = false;
                    }
                }
            }

            function getToDoList() {
                var arrayForUserId = [];
                angular.forEach(vm.query.resourceIds, function (item) {
                    arrayForUserId.push(item);
                })
                if (vm.isSalesRep || vm.isTech) {

                }
                var myJsonString = JSON.stringify({ CommentList: arrayForUserId });
                return toDoService.getToDoListForScheduler(myJsonString).then(function (result) {
                    if (result != null) {
                        vm.todoInfoListForToday = result.data.commentList;
                        if (vm.isTech && vm.todoInfoListForToday.length <= 0) {
                            $scope.isExpand = true;
                            vm.isVisible = true;
                        }
                    }
                });
            }

            function openViewFollowUpModal(list) {

                var todoInfoListForTodayList = [];
                if (list.length == undefined) {
                    todoInfoListForTodayList.push(list);
                }
                else {
                    todoInfoListForTodayList = list;
                }
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/show.followup.client.view.html',
                    controller: 'ViewFollowUpController',
                    controllerAs: 'vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                FollowUpList: todoInfoListForTodayList,
                                IsFromScheduler: true
                            };
                        }
                    }
                });
                modalInstance.result.then(function () {
                }, function () {

                })
            };

            function nagvigateToDoList() {
                $window.sessionStorage.setItem("IsFromFranchiseeLevel", true);
                $window.sessionStorage.setItem("IsFromFranchiseeId", vm.query.franchiseeId);
                $window.sessionStorage.setItem("IsFromFranchiseeName", vm.franchiseeName);
                $state.go('core.layout.scheduler.todoJobEstimate', { franchiseeId: vm.query.franchiseeId });
            }
            if (vm.isSalesRep || vm.isTech) {
                getToDoList();
            }
            if (!vm.isExecutive) {
                $q.all([getFranchiseeCollection(), testing(), getmarketingClassCollection(), getAssigneeList(), prepareOptions(), prepareImportOptions(), prepareStatusOptions(), getCurrentView(), getDefaultView(), getToDoList(), getMonthCollection(), getLastTwentyYearCollection(), addressSerachOptions()]);
            }
            else {
                $q.all([getCurrentView(), getmarketingClassCollection(), prepareOptions(), prepareImportOptions(), prepareStatusOptions(), getDefaultView(), addressSerachOptions()]);
            }
        }]);
}());