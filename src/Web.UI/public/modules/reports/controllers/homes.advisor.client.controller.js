(function () {
    'use strict';
    angular.module(ReportsConfiguration.moduleName).controller("HomeAdvisorController",
        ["$state", "$stateParams", "$q", "$scope", "MarketingLeadService", "WebLeadService", "APP_CONFIG", "$rootScope", "FileService", "FranchiseeService", "$uibModal", "Clock",
            function ($state, $stateParams, $q, $scope, marketingLeadService, webLeadService, config, $rootScope, fileService, franchiseeService, $uibModal, clock
                ) {
                var vm = this;
                vm.currentDate = clock.now();
                vm.currentDateForMonth = new Date();
                vm.ifFromTop = true;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
                vm.isSalesRep = $rootScope.identity.roleId == vm.Roles.SalesRep;
                vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.isOpsMgr = $rootScope.identity.roleId == vm.Roles.OperationsManager;
                vm.franchiseeId = 0;
                vm.monthOptions = [];
                vm.yearOptions = [];
                vm.viewTypes = [];

                vm.query = {
                    pageNumber: 1,
                    franchiseeId: 0,
                    pageSize: config.defaultPageSize,
                    text: '',
                    url: '',
                    sort: { order: 0, propName: '' },
                    isTimeChange: false,
                };


                if (!vm.isSuperAdmin) {
                    vm.franchiseeId = $rootScope.identity.organizationId;
                }
                if (vm.isExecutive) {
                    if ($stateParams != null && $stateParams.franchiseeId > 1)
                        vm.franchiseeId = $stateParams.franchiseeId;
                    else {
                        vm.franchiseeId = $rootScope.identity.loggedInOrganizationId;
                    }
                    getList();
                }
                vm.isFromTop = true;
                vm.getReport = getReport;
                vm.pagingOptions = config.pagingOptions;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.leadCount = 0;
                vm.adjuststedclass = true;
                vm.isGraphsActive = true;
                vm.franchiseeName = "National";
                vm.isRawVsAdjustedActive = false;
                vm.isManagementVsLocalDataActive = false;
                vm.isManagementDataActive = false;
                $scope.adjuctedclass = false;
                vm.adjustedSum = 0;
                vm.pageChange = pageChange;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.currentRole = $rootScope.identity.roleId;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.franchiseeList = [];
                vm.id = [];
                vm.Items = null;
                vm.managementDetailReportList = null;


                function resetSearch() {
                    vm.franchiseeName = 'National';
                    vm.query.text = '';
                    vm.query.url = '';
                    vm.query.viewTypeId = null;
                    vm.query.franchiseeId = 0;
                    vm.query.pageNumber = 1;
                    vm.query.webPageNumber = 1;
                    vm.query.startDate = vm.currentDate;
                    vm.query.isTimeChange = false;
                    vm.query.franchiseeId = "0";
                    vm.query.text = '';
                    vm.query.month = vm.currentDateForMonth.getMonth(),
                        vm.query.year = vm.currentDateForMonth.getFullYear().toString()
                    getReport(false);

                }


                function refresh() {
                    vm.query.isTimeChange = true;
                    getReport();
                }

                function prepareSearchOptions() {
                    vm.viewTypes.push({ display: 'Month', value: '1' })
                    vm.viewTypes.push({ display: 'Week', value: '2' });
                    vm.viewTypes.push({ display: 'Day', value: '3' });
                    vm.viewTypes.push({ display: 'Year', value: '4' });
                }




                function getReport() {
                    vm.query.sortingColumn = vm.query.sort.propName;
                    vm.query.sortingOrder = vm.query.sort.order;


                    return marketingLeadService.getHomeAdvisorReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.homeAdvisorList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                        }
                    });
                }


                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {

                        vm.franchiseeCollection = result.data;
                        vm.franchiseeCollection.push({ alias: 'National', display: 'National', id: '0', value: '0' });
                        vm.query.franchiseeId = "0";
                    });
                }

                function sorting(propName) {
                    if (vm.query.viewTypeId == null)
                        vm.query.viewTypeId = '1';
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getReport();
                };



                function pageChange() {
                    getReport();
                };



                $scope.$emit("update-title", "Home Advisor Report");
                $q.all([getFranchiseeCollection(), getReport(), prepareSearchOptions()]);
            }]);
}());