(function () {
    'use strict';

    var SortColumns = {
        Franchisee: 'Franchisee',
        InvoiceId: 'InvoiceId',
        StartDate: 'StartDate',
        EndDate: 'EndDate',
        DueDate: 'DueDate',
        Status: 'Status',
    };

    angular.module(ReportsConfiguration.moduleName).controller("ListLateFeesController",
        ["$state", "$stateParams", "$q", "$scope", "APP_CONFIG", "$rootScope", "LateFeeReportService",
            "FranchiseeService", "$uibModal", "InvoiceService", "FileService", "URLAuthenticationServiceForEncryption",
            function ($state, $stateParams, $q, $scope, config, $rootScope, lateFeeReportService,
                franchiseeService, $uibModal, invoiceService, fileService, URLAuthenticationServiceForEncryption) {
                var vm = this;
                vm.query = {
                    pageNumber: 1,
                    franchiseeId: 0,
                    startDate: null,
                    endDate: null,
                    lateFeeTypeId: 0,
                    dueDateStart: null,
                    dueDateEnd: null,
                    statusId: 0,
                    pageSize: config.defaultPageSize,
                    text: '',
                    sort: { order: 0, propName: '' }
                };
                vm.getLateFeeReportList = getLateFeeReportList;
                vm.pagingOptions = config.pagingOptions;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.pageChange = pageChange;
                vm.searchOptions = [];
                vm.searchOption = '';
                vm.resetSeachOption = resetSeachOption;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.currentRole = $rootScope.identity.roleId;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.viewInvoice = viewInvoice;
                vm.downloadLateFeeReport = downloadLateFeeReport;
                vm.franchiseeIds = [];
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }

                function downloadLateFeeReport() {
                    vm.downloading = true;
                    return lateFeeReportService.downloadLateFeeReport(vm.query).then(function (result) {
                        var fileName = "lateFeeReport.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;

                    }, function () { vm.downloading = false; });
                }
                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' })

                    vm.searchOptions.push({ display: 'Late Fee Type', value: '2' }),
                        vm.searchOptions.push({ display: 'Due Date', value: '3' }),
                        vm.searchOptions.push({ display: 'Status', value: '4' }),
                        vm.searchOptions.push({ display: 'Other', value: '5' });
                }
                function refresh() {
                    getLateFeeReportList();
                }
                function resetSeachOption() {
                    if (vm.seachOption == '1') {
                        vm.query.lateFeeTypeId = 0;
                        vm.query.statusId = 0;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.text = '';
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '2') {
                        vm.query.franchiseeId = 0;
                        vm.query.text = '';
                        vm.query.statusId = 0;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '3') {
                        vm.query.franchiseeId = 0;
                        vm.query.text = '';
                        vm.query.statusId = 0;
                        vm.query.pageNumber = 1;
                        vm.query.lateFeeTypeId = 0;
                    }
                    else if (vm.seachOption == '4') {
                        vm.query.franchiseeId = 0;
                        vm.query.text = '';
                        vm.query.lateFeeTypeId = 0;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '4') {
                        vm.query.statusId = 0;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.pageNumber = 1
                    }
                    else {
                        vm.query.lateFeeTypeId = 0;
                        vm.query.franchiseeId = 0;
                        vm.query.pageNumber = 1,
                            vm.query.statusId = 0;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                    }
                }

                function resetSearch() {
                    vm.query.text = '';
                    vm.query.statusId = 0;
                    vm.query.franchiseeId = 0;
                    vm.query.lateFeeTypeId = 0;
                    vm.query.startDate = null;
                    vm.query.endDate = null;
                    vm.searchOption = '';
                    $scope.$broadcast("reset-dates");
                    vm.query.pageNumber = 1;
                    vm.query.dueDateStart = null;
                    vm.query.dueDateEnd = null;
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.startDate = null;
                    vm.query.endDate = null;
                    vm.query.dueDateStart = null;
                    vm.query.dueDateEnd = null;
                    getLateFeeReportList();
                });

                function getLateFeeReportList() {
                    return lateFeeReportService.lateFeeReportService(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.franchiseeIds = [];
                            vm.lateFeeReportList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            addResultToList(vm.lateFeeReportList);
                        }
                    });
                }
                function addResultToList() {
                    angular.forEach(vm.lateFeeReportList, function (value, key) {
                        vm.franchiseeIds.push(value.franchiseeId);
                    })
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function getInvoiceStatus() {
                    return invoiceService.getInvoiceStatus().then(function (result) {
                        vm.invoiceStatus = result.data;
                    });
                }

                function getLateFeeItemType() {
                    return lateFeeReportService.getLateFeeItemType().then(function (result) {
                        vm.lateFeeType = result.data;
                    });
                }
                function viewInvoice(invoiceId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/franchisee-invoice-detail.client.view.html',
                        controller: 'FranchiseeInvoiceDetailController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    InvoiceId: invoiceId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getLateFeeReportList();
                    }, function () {

                    });
                }

                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getLateFeeReportList();
                };

                function pageChange() {
                    getLateFeeReportList();
                };

                $scope.$emit("update-title", "Late Fee Report");
                $q.all([getLateFeeReportList(), prepareSearchOptions(), getFranchiseeCollection(), getInvoiceStatus(), getLateFeeItemType()]);

            }]);
}());