(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).controller("AutomationBackUpCallController",
        ["$state", "$stateParams", "$q", "$scope", "FranchiseeGrowthReportService", "APP_CONFIG", "$rootScope", "FileService", "DashboardService",
            "$uibModal", "Clock", "FranchiseeService", "CustomerService", "MarketingLeadService", "Toaster",
            function ($state, $stateParams, $q, $scope, franchiseeGrowthReportService, config, $rootScope, fileService, dashboardService,
                $uibModal, clock, franchiseeService, customerService, marketingLeadService, toaster) {

                var vm = this;

                vm.Roles = DataHelper.Role;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;

                vm.pageChange = pageChange;
                vm.pagingOptions = config.pagingOptions;
                vm.isBasicDataActive = true;
                vm.isNoDataActive = false;
                vm.changeTab = changeTab;

                vm.query = {
                    FranchiseeId: 0,
                    startDate: null,
                    endDate: null,
                    pageNumber: 1,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };

                var date = new Date();
                var firstDay = new Date(date.getFullYear(), date.getMonth()-1, 1);
                var lastDay = new Date(date.getFullYear(), date.getMonth(), 2);
                vm.query.startDate = firstDay;
                vm.query.endDate = lastDay;
                $rootScope.$broadcast("reset-dates");

                vm.getAutomatedBackUpCalls = getAutomatedBackUpCalls;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function getAutomatedBackUpCalls() {
                    return marketingLeadService.getAutomationBackUpReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.getAutomatedBackUpCalls = result.data.collection;
                            if (vm.getAutomatedBackUpCalls.length == 1 && vm.isFranchiseeAdmin) {
                                vm.getAutomatedBackUpCalls[0].isExpand = true;
                                vm.getAutomatedBackUpCalls[0].isIVRCallClick = true;
                            }
                            vm.noCallMatch = result.data.noCallMatch;
                            vm.count = result.data.totalCount;
                        }
                    });
                }

                function resetSearch() {
                    vm.query = {
                        FranchiseeId: 0,
                        startDate: null,
                        endDate: null,
                        pageNumber: 1,
                        pageSize: config.defaultPageSize,
                        sort: { order: 0, propName: '' }
                    };
                    $scope.$broadcast("reset-dates");
                    var firstDay = new Date(date.getFullYear(), date.getMonth() - 1, 1);
                    var lastDay = new Date(date.getFullYear(), date.getMonth(), 2);
                    vm.query.startDate = firstDay;
                    vm.query.endDate = lastDay;
                    vm.isNoDataActive = false;
                    vm.isBasicDataActive = true;
                    
                    getAutomatedBackUpCalls();
                }

                function refresh() {
                    getAutomatedBackUpCalls();
                }

                function changeTab(tabNo) {
                    if (tabNo == 1) {
                        vm.isNoDataActive = false;
                        vm.isBasicDataActive = true;
                    }
                    else {
                        vm.isNoDataActive = true;
                        vm.isBasicDataActive = false;
                    }
                }

                function pageChange() {
                    getAutomatedBackUpCalls();
                };

                $scope.$emit("update-title", "Back-Up Call Report");

                $q.all([getFranchiseeCollection(),getAutomatedBackUpCalls()]);
            }]);
}());