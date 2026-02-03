(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).controller("ListReportController",
        ["$state", "$stateParams", "$q", "$scope", "ReportService", "APP_CONFIG", "$rootScope",
            "FranchiseeService", "CustomerService", "FileService", "URLAuthenticationServiceForEncryption",
            function ($state, $stateParams, $q, $scope, reportService, config, $rootScope, franchiseeService,
                customerService, fileService, URLAuthenticationServiceForEncryption) {
                var vm = this;
                vm.query = {
                    pageNumber: 1,
                    franchiseeId: 0,
                    paymentDateEnd: null,
                    paymentDateStart: null,
                    serviceTypeId: 0,
                    classTypeId: 0,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };
                vm.getReportList = getReportList;
                vm.pagingOptions = config.pagingOptions;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.pageChange = pageChange;
                vm.searchOptions = [];
                vm.searchOption = '';
                vm.resetSeachOption = resetSeachOption;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.currentRole = $rootScope.identity.roleId;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.downloadSalesReport = downloadSalesReport;
                vm.franchiseeIds = [];
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }
                function downloadSalesReport() {
                    vm.downloading = true;
                    return reportService.downloadSalesReport(vm.query).then(function (result) {
                        var fileName = "salesReport.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;

                    }, function () { vm.downloading = false; });
                }

                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' })

                    vm.searchOptions.push({ display: 'Service', value: '2' }, { display: 'Class', value: '3' });
                }
                function refresh() {
                    getReportList();
                }
                function resetSeachOption() {
                    if (vm.seachOption == '1') {
                        vm.query.serviceTypeId = 0;
                        vm.query.classTypeId = 0;
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '2') {
                        vm.query.franchiseeId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '3') {
                        vm.query.franchiseeId = 0;
                        vm.query.classTypeId = 0;
                        vm.query.pageNumber = 1
                    }
                    else {
                        vm.query.statusId = 0;
                        vm.query.franchiseeId = 0;
                        vm.query.serviceTypeId = 0;
                        vm.query.pageNumber = 1
                    }
                }

                function resetSearch() {
                    vm.query.text = '';
                    vm.query.franchiseeId = 0;
                    vm.query.serviceTypeId = 0;
                    vm.query.classTypeId = 0;
                    vm.query.paymentDateEnd = null;
                    vm.query.paymentDateStart = null;
                    vm.searchOption = '';
                    $scope.$broadcast("reset-dates");
                    vm.query.pageNumber = 1
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.paymentDateEnd = null;
                    vm.query.paymentDateStart = null;
                    getReportList();
                });

                function getReportList() {
                    return reportService.getReportList(vm.query).then(function (result) {
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
                    });
                }

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
                    getReportList();
                };

                $scope.$emit("update-title", "Sales Report");
                $q.all([getReportList(), prepareSearchOptions(), getFranchiseeCollection(), getServiceTypeCollection(), getmarketingClassCollection()]);
            }]);
}());