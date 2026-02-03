(function () {
    'use strict';

    var SortColumns = {
        Id: 'Id',
        InvoiceId: 'InvoiceId',
        InvoiceDate: 'InvoiceDate',
        QBInvoice: 'QBInvoice',
        FranchiseeName: 'FranchiseeName',
        CustomerName: 'CustomerName',
        AnnualSalesAmount: 'AnnualSalesAmount',
        AnnualPaidAmount: 'AnnualPaidAmount',
        WeeklySalesAmount: 'WeeklySalesAmount',
        WeeklyPaidAmount: 'WeeklyPaidAmount',
        AnnualReportStatus: 'AnnualReportStatus',
        AnnuallyPaidDifferent: 'AnnuallyPaidDifferent',
        AnnuallySalesDifferent: 'AnnuallySalesDifferent',
        WeeklyDifferent: 'WeeklyDifferent',
        AnnuallyDifferent: 'AnnuallyDifferent'
    };
   

    angular.module(SalesConfiguration.moduleName).controller("ListAnnualSalesController",
        ["$scope", "$rootScope", "$state", "$stateParams", "$q", "APP_CONFIG", "FranchiseeService", "SalesService", "$uibModal", "FileService", "AnnualBatchService", '$filter',
    function ($scope, $rootScope, $state, $stateParams, $q, config, franchiseeService, salesService, $uibModal, fileService, annualBatchService, $filter) {

        var vm = this;
        
        vm.query = {
            text: '',
            annualDataUploadId: 0,
            InvoiceId: 0,
            qbInvoiceNumber: '',
            franchiseeId: 0,
            propName: '',
            sort: { order: 0, propName: '' },
            reportTypeId: 0
        };
        vm.currencyCode = $rootScope.identity.currencyCode;
        vm.query.annualDataUploadId = $stateParams.annualDataUploadId == null ? 0 : $stateParams.annualDataUploadId;
        vm.query.franchiseeId = $stateParams.franchiseeId == null ? 0 : $stateParams.franchiseeId;
        vm.Roles = DataHelper.Role;
        vm.ReportTypes = DataHelper.ReportTypes;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
        vm.count = 0;

        vm.sorting = sorting;
        vm.SortColumns = SortColumns;
        vm.resetSearch = resetSearch;
        vm.refresh = refresh;
        vm.searchOptions = []
        vm.searchOption = '';
        
        vm.resetSeachOption = resetSeachOption;
        vm.currentRole = $rootScope.identity.roleId;
        vm.getRecords = getRecords;
        vm.invoiceIds = [];
        vm.downloadAnnualData = downloadAnnualData;
        vm.downloadSortOrder = [];
        function prepareSearchOptions() {

            vm.searchOptions.push({ display: 'Qb Invoice#', value: '2' });
        }

        function downloadAnnualData() {
            vm.downloading = true;
            return annualBatchService.downloadAnnualData(vm.query).then(function (result) {
                var fileName = "Annual Report_" + vm.FranchiseeName + ".xlsx";
                fileService.downloadFile(result.data, fileName);
                vm.downloading = false;

            }, function () { vm.downloading = false; });
        }


        function resetSeachOption() {
            if (vm.searchOption == '1') {
                vm.query.qbInvoiceNumber = '';
            }
            else if (vm.searchOption == '2') {
                vm.query.franchiseeId = 0;
                vm.query.pageNumber = 1;
            }
            else {
                vm.query.franchiseeId = 0;
                vm.query.qbInvoiceNumber = '';
            }
        }

        function resetSearch() {
            vm.query.franchiseeId = 0;
            vm.query.qbInvoiceNumber = '';
            vm.searchOption = '';
            getRecords();
        }

        function getFranchiseeCollection() {
            return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                vm.franchiseeCollection = result.data;
            });
        }
        function refresh() {
            getRecords();
        }

        function getRecords() {
            vm.mergedList = [];
            if (!vm.isSuperAdmin)
                vm.query.franchiseeId = $rootScope.identity.organizationId;

            getSalesDataCollection();
        }

        function getSalesDataCollection() {
            vm.query.isAnnualAudit = true;
            //vm.query.sortingOrderPertable = SortOrder;
            if (vm.query.reportTypeId == 0 && vm.query.qbInvoiceNumber!=null) {
                return annualBatchService.getAnnualSalesList(vm.query).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.invoiceIds = [];
                        vm.salesDataListGroupedValues = [];
                        vm.salesDataListGroupedValues = result.data.groupCollection;

                        if (vm.query.franchiseeId == 0 || vm.query.franchiseeId == undefined) {
                            vm.query.franchiseeId = vm.salesDataListGroupedValues[0].franchiseeId;
                        }
                        vm.query.sort.order = result.data.filter.sortingOrder;
                        vm.count = result.data.filter.count;
                    }
                    var franchisee = $filter('filter')(vm.franchiseeCollection, { value: vm.query.franchiseeId.toString() }, true);
                    vm.FranchiseeName = franchisee[0].display;
                    if (vm.FranchiseeName != null) {
                        $scope.$emit("update-title", "Annual Sales Data :" + vm.FranchiseeName);
                    }
                });
            }
            else {
                return annualBatchService.getAnnualSalesList(vm.query).then(function (result) {
                    var isPresent = true;
                    
                    if (result != null && result.data != null) {
                       
                        angular.forEach(vm.salesDataListGroupedValues, function (value, key) {
                            if (value.reportTypeId == vm.query.reportTypeId && isPresent) {
                                value = result.data.groupCollection;
                                vm.salesDataListGroupedValues[key] = value[0];
                                vm.query.sort.order = result.data.filter.sortingOrder;
                                vm.count = result.data.filter.count;
                                isPresent = false;
                            }
                        })
                    }

                });
            }
        }

        function sorting(propName, reportTypeId) {
            vm.query.sortingColumn = propName;
            vm.query.sortingOrder = (vm.query.sortingOrder == 0) ? 1 : 0;
            vm.downloadSortOrder.push({ propName: propName, order: vm.query.sortingOrder, reportTypeId:reportTypeId, });
            vm.query.reportTypeId = reportTypeId;
            vm.query.downloadSortOrder = vm.downloadSortOrder;
            getRecords();
        };

        $scope.$emit("update-title", "Annual Sales Data");

        $q.all([getFranchiseeCollection(), getRecords(), prepareSearchOptions()]);

    }]);
}());