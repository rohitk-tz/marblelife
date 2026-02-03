(function () {
    'use strict';

    var SortColumns = {
        InvoiceId: 'InvoiceId',
        Name: 'Name',
        InvoiceDate: 'InvoiceDate',
        QBInvoice: 'QBInvoice',
        StreetAddress: 'StreetAddress',
        City: 'City',
        State: 'State',
        ZipCode: 'ZipCode',
        Country: 'Country',
        TotalAmount: 'TotalAmount',
        PaidAmount: 'PaidAmount',
        MarketingClass: 'MarketingClass',
        FranchiseeName: 'FranchiseeName'
    };

    angular.module(SalesConfiguration.moduleName).controller("ListSalesController",
        ["$scope", "$rootScope", "$stateParams", "$q", "APP_CONFIG", "FranchiseeService", "SalesService", "$uibModal",
            "FileService", "SalesInvoiceService", "URLAuthenticationServiceForEncryption",
            function ($scope, $rootScope, $stateParams, $q, config, franchiseeService, salesService, $uibModal,
                fileService, salesInvoiceService, URLAuthenticationServiceForEncryption) {
                var vm = this;
                vm.query = {
                    text: '',
                    salesDataUploadId: 0,
                    qbInvoiceNumber: '',
                    franchiseeId: 0,
                    customerId: 0,
                    customerName: '',
                    marketingClassId: 0,
                    periodStartDate: null,
                    periodEndDate: null,
                    pageNumber: 1,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };
                vm.currencyCode = $rootScope.identity.currencyCode;
                vm.query.salesDataUploadId = $stateParams.salesDataUploadId == null ? 0 : $stateParams.salesDataUploadId;
                vm.query.customerId = $stateParams.customerId == null ? 0 : $stateParams.customerId;
                vm.query.franchiseeId = $stateParams.franchiseeId == null ? 0 : $stateParams.franchiseeId;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;

                vm.pagingOptions = config.pagingOptions;
                vm.pageChange = pageChange;
                vm.getSalesDataCollection = getSalesDataCollection;
                vm.viewInvoice = viewInvoice;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.invoiceIds = [];
                vm.downloadSalesData = downloadSalesData;
                vm.searchOptions = []
                vm.searchOption = '';
                vm.resetSeachOption = resetSeachOption;
                vm.currentRole = $rootScope.identity.roleId;
                vm.viewFeedback = viewFeedback;
                vm.getmarketingClassCollection = getmarketingClassCollection;
                vm.downloadInvoiceData = downloadInvoiceData;
                vm.uploadInvoiceData = uploadInvoiceData;
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }
                function viewFeedback(responseId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/view-feedback-detail.client.view.html',
                        controller: 'ViewFeedbackDetailController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    ResponseId: responseId
                                };
                            }
                        }
                    });
                }

                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' })

                    vm.searchOptions.push({ display: 'Customer', value: '2' }, { display: 'Qb Invoice#', value: '3' }, { display: 'Marketing Class', value: '4' },
                        { display: 'Other', value: '5' });
                }

                function resetSeachOption() {
                    if (vm.searchOption == '1') {
                        vm.query.customerName = '';
                        vm.query.qbInvoiceNumber = '';
                        vm.query.text = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pageNumber = 1;
                    }
                    else if (vm.searchOption == '2') {
                        vm.query.text = '';
                        vm.query.franchiseeId = '0';
                        vm.query.qbInvoiceNumber = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pageNumber = 1;
                    }
                    else if (vm.searchOption == '3') {
                        vm.query.text = '';
                        vm.query.franchiseeId = '0';
                        vm.query.customerName = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pageNumber = 1;
                    }
                    else if (vm.searchOption == '3') {
                        vm.query.text = '';
                        vm.query.franchiseeId = '0';
                        vm.query.customerName = '';
                        vm.query.qbInvoiceNumber = '';
                        vm.query.pageNumber = 1;
                    }
                    else {
                        vm.query.customerName = '';
                        vm.query.franchiseeId = '0';
                        vm.query.qbInvoiceNumber = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pageNumber = 1;
                    }
                }

                function resetSearch() {
                    vm.query.text = '';
                    vm.query.franchiseeId = '0';
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    vm.query.customerName = '';
                    vm.query.marketingClassId = 0;
                    vm.query.qbInvoiceNumber = '';
                    vm.searchOption = '';
                    $scope.$broadcast("reset-dates");
                    vm.query.pageNumber = 1;
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    getSalesDataCollection();
                });
                function pageChange() {
                    vm.getSalesDataCollection();
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }
                function refresh() {
                    getSalesDataCollection();
                }

                function getSalesDataCollection() {
                    if (!vm.isSuperAdmin)
                        vm.query.franchiseeId = $rootScope.identity.organizationId;

                    return salesService.getSalesDataList(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.invoiceIds = [];
                            vm.salesDataList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            addSalesDataToList(vm.salesDataList);
                        }
                    });
                }

                function addSalesDataToList() {
                    angular.forEach(vm.salesDataList, function (value, key) {
                        vm.invoiceIds.push(value.invoiceId);
                    })
                }

                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getSalesDataCollection();
                };

                function viewInvoice(invoiceId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/invoice-detail.client.view.html',
                        controller: 'InvoiceDetailController',
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

                    }, function () {

                    });
                }

                function downloadSalesData() {
                    vm.downloading = true;
                    return salesService.downloadSalesData(vm.query).then(function (result) {
                        var fileName = "salesData.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;

                    }, function () { vm.downloading = false; });
                }

                function downloadInvoiceData() {
                    vm.downloading = true;
                    return salesInvoiceService.downloadInvoiceData(vm.query).then(function (result) {
                        var fileName = "sales_InvoiceData.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;

                    }, function () { vm.downloading = false; });
                }

                function uploadInvoiceData() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/upload-sales-data.client.view.html',
                        controller: 'UploadSalesDataController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'md',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                    });
                }

                function getmarketingClassCollection() {
                    return salesService.getmarketingClassCollection().then(function (result) {
                        vm.marketingClass = result.data;
                    });
                }

                $scope.$emit("update-title", "Sales Data");

                $q.all([getFranchiseeCollection(), getSalesDataCollection(), prepareSearchOptions(), getmarketingClassCollection()]);

            }]);
}());