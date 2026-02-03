(function () {
    'use strict';

    var SortColumns = {
        Franchisee: 'Franchisee',
        ID: 'ID'
    };

    angular.module(ReportsConfiguration.moduleName).controller("GrowthReportController",
        ["$state", "$stateParams", "$q", "$scope", "FranchiseeGrowthReportService", "APP_CONFIG", "$rootScope", "FileService", "DashboardService",
            "$uibModal", "Clock", "FranchiseeService", "CustomerService", "URLAuthenticationServiceForEncryption",
            function ($state, $stateParams, $q, $scope, franchiseeGrowthReportService, config, $rootScope, fileService, dashboardService,
                $uibModal, clock, franchiseeService, customerService, URLAuthenticationServiceForEncryption) {

                var vm = this;

                vm.currentDate = clock.now();
                vm.query = {
                    pageNumber: 1,
                    franchiseeId: 0,
                    serviceTypeId: 0,
                    classTypeId: 0,
                    year: 0,
                    month: 0,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };

                vm.getGrowthReport = getGrowthReport;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.pagingOptions = config.pagingOptions;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.pageChange = pageChange;
                vm.searchOptions = [];
                vm.searchOption = '';
                vm.downloadGrowthReport = downloadGrowthReport;
                vm.franchiseeIds = [];
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }
                function downloadGrowthReport() {
                    vm.downloading = true;
                    return franchiseeGrowthReportService.downloadGrowthReport(vm.query).then(function (result) {
                        var fileName = "growthReport.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;

                    }, function () { vm.downloading = false; });
                }

                function resetSearch() {
                    vm.query.franchiseeId = 0;
                    vm.query.classTypeId = 0;
                    vm.query.serviceTypeId = 0;
                    vm.query.year = 0;
                    vm.query.month = 0;
                    vm.searchOption = '';
                    vm.query.pageNumber = 1;
                    getGrowthReport();
                }

                function refresh() {
                    getGrowthReport();
                }

                function getGrowthReport() {
                    if (vm.query.year <= 0) {
                        vm.query.year = clock.getYear(vm.currentDate);
                    }
                    if (vm.query.month == null || vm.query.month <= 0) {
                        if (vm.query.year == clock.getYear(vm.currentDate)) {
                            vm.query.month = clock.getMonth(vm.currentDate) - 1 + "";
                        }
                    }

                    return franchiseeGrowthReportService.getGrowthReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.franchiseeIds = [];
                            vm.reportList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            addResultToList(vm.reportList);
                        }
                    });
                }

                function addResultToList() {
                    angular.forEach(vm.reportList, function (value, key) {
                        vm.franchiseeIds.push(value.franchiseeId);
                    })
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                        vm.franchiseeCollection.push({ display: "All", value: "0" });
                    });
                }

                function getYearsForGrowthReport() {
                    return franchiseeGrowthReportService.getYearsForGrowthReport().then(function (result) {
                        vm.yearCollection = result.data;
                    })
                }

                function getMonthCollection() {
                    return dashboardService.getMonthNames().then(function (result) {
                        vm.monthCollection = result.data;
                        vm.monthCollection.push({ display: "All", value: 0 });
                    })
                }

                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getGrowthReport();
                };

                function getServiceTypeCollection() {
                    return franchiseeService.getServiceTypeCollection().then(function (result) {
                        vm.serviceTypes = result.data;
                    });
                }

                function getmarketingClassCollection() {
                    return customerService.getmarketingClassCollection().then(function (result) {
                        vm.marketingClass = result.data;
                    });
                }


                function pageChange() {
                    getGrowthReport();
                };


                $scope.$emit("update-title", "Franchisee Growth Report");

                $q.all([getGrowthReport(), getFranchiseeCollection(), getMonthCollection(), getYearsForGrowthReport(), getmarketingClassCollection(), getServiceTypeCollection()]);
            }]);
}());