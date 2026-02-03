(function () {
    'use strict';

    var SortColumns = {
        Url: 'Url',
        Count: "Count",
        PhoneLabel: 'PhoneLabel',
        GrandTotal: 'GrandTotal',
    };

    angular.module(ReportsConfiguration.moduleName).controller("LeadPerformanceReportController",
        ["$state", "$stateParams", "$q", "$scope", "MarketingLeadService", "WebLeadService", "APP_CONFIG", "$rootScope", "FileService", "FranchiseeService", "$uibModal", "Clock",
            "DashboardService", "FranchiseeGrowthReportService", "MarketingLeadGraphService", "$filter",
            function ($state, $stateParams, $q, $scope, marketingLeadService, webLeadService, config, $rootScope, fileService, franchiseeService, $uibModal, clock,
                dashboardService, franchiseeGrowthReportService, marketingLeadGraphService, $filter) {
                var vm = this;
                vm.Roles = DataHelper.Role;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isTabChangeData = isTabChangeData;
                vm.isNationalActive = true;
                vm.isLocalActive = false;
                vm.currentDate = clock.now();
                vm.emptyResult = false;
                vm.query = {
                    franchiseeId: 0,
                    startDate: null,
                }

                function isTabChangeData(tabSelected) {
                    if (tabSelected == 1) {
                        vm.isNationalActive = true;
                        vm.isLocalActive = false;
                    }
                    else {
                        vm.isNationalActive = false;
                        vm.isLocalActive = true;
                        getFranchiseeCollection();
                    }
                }
                function getLeadPerformanceNationalWise() {
                    return marketingLeadService.getLeadPerformanceNationalWise(vm.query).then(function (result) {
                        if (result.data != null) {
                            vm.totalOfLeadCount = result.data.totalOfLeadCount;
                            vm.leadPerformanceData = result.data.leadPerformanceFranchiseeNationalViewModel;
                            if (vm.isFranchiseeAdmin) {
                                vm.franchiseeName = vm.leadPerformanceData[0].franchiseeName;
                                $scope.$emit("update-title", "Lead Performance Report ( " + vm.franchiseeName+" )");
                            }
                            vm.month = result.data.months;
                            vm.franchiseeCollection = result.data.franchiseeList;
                        }
                    });
                }

                function getLeadPerformanceFranchiseeWise() {
                    vm.emptyResult = false;
                    return marketingLeadService.getLeadPerformanceFranchiseeWise(vm.query).then(function (result) {
                        if (result.data != null) {

                        }
                        else {
                            vm.emptyResult = true;
                        }
                    });
                };
                function getPPCAndSeoValueLocal() {
                    return marketingLeadService.getPPCAndSeoLocal(vm.query).then(function (result) {
                    });
                };
                function getPPCAndSeoValueNational() {
                    return marketingLeadService.getPPCAndSeoNational(vm.query).then(function (result) {
                        if (result.data != null) {
                            vm.seoValue = result.data.totalCount;
                        }
                    });
                };

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {

                        vm.franchiseeCollection = result.data;
                        vm.query.franchiseeId = "0";
                    });
                }

                $scope.$emit("update-title", "Lead Performance Report");

                    $q.all([getLeadPerformanceNationalWise()]);
            }]);
}());