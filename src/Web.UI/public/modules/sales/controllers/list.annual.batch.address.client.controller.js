(function () {
    'use strict';

    var SortColumns = {
        Id: 'Id',
        InvoiceId: 'InvoiceId',
        InvoiceDate: 'InvoiceDate',
        QBInvoice: 'QBInvoice',
        FranchiseeName: 'FranchiseeName',
        MarketingClass: 'MarketingClass'
    };

    angular.module(SalesConfiguration.moduleName).controller("ListAnnualBatchAddressController",
        ["$scope", "$rootScope", "$state", "$stateParams", "$q", "APP_CONFIG", "FranchiseeService", "SalesService", "$uibModal", "FileService", "AnnualBatchService","CustomerService",
    function ($scope, $rootScope, $state, $stateParams, $q, config, franchiseeService, salesService, $uibModal, fileService, annualBatchService, customerService) {

        var vm = this;
        vm.query = {
            text: '',
            annualDataUploadId: 0,
            InvoiceId: 0,
            qbInvoiceNumber: '',
            franchiseeId: 0,
            pageSize: config.defaultPageSize,
            sort: { order: 0, propName: '' },
            pageNumber: 1,
            marketingClassId:0

        };
        vm.address = {
            text: '',
            addressLine1: '',
            addressLine2: '',
            city: '',
            state: '',
            zipcode: '',
            country: '',
            invoiceId: '',
            qbInvoiceId: ''
        };
        vm.currencyCode = $rootScope.identity.currencyCode;
        vm.query.annualDataUploadId = $stateParams.annualDataUploadId == null ? 0 : $stateParams.annualDataUploadId;
        vm.query.franchiseeId = $stateParams.franchiseeId == null ? 0 : $stateParams.franchiseeId;
        vm.Roles = DataHelper.Role;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
        vm.count = 0;
        vm.pagingOptions = config.pagingOptions;
        vm.sorting = sorting;
        vm.SortColumns = SortColumns;
        vm.resetSearch = resetSearch;
        vm.refresh = refresh;
        vm.searchOptions = [];
        vm.statusOptions = []
        vm.searchOption = '';
        vm.resetSeachOption = resetSeachOption;
        vm.currentRole = $rootScope.identity.roleId;
        vm.getRecords = getRecords;
        vm.invoiceIds = [];
        vm.query.statusId = '1';
        vm.downloadAnnualData = downloadAnnualData;
        vm.pageChange = pageChange;
        vm.updateInvoiceAddress = updateInvoiceAddress;
        //vm.updateInvoiceAddress(item.invoiceId, item.qbInvoiceId, item.oldAddressLine1, item.oldAddressLine2, item.oldCountry, item.oldState, item.oldCity, item.oldZip, item.oldoldphoneNumber);
        function updateInvoiceAddress(item) {
            var items = angular.copy(item)
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/sales/views/update-customer-invoice-address.client.view.html',
                controller: 'UpdateCustomerInvoiceController',
                controllerAs: 'vm',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    modalParam: function () {
                        return {
                            AddressModel: items,
                            StatusId: vm.query.statusId
                        };
                    }
                }
            });
            modalInstance.result.then(function () {
                getRecords();
            }, function () {

            });
        }
        function pageChange() {
            getRecords();
        };
        function prepareSearchOptions() {
            //if (vm.currentRole == vm.Roles.SuperAdmin)
            //    vm.searchOptions.push({ display: 'Franchisee', value: '1' })

            vm.statusOptions.push({ display: 'Address Update History', value: '2' });
            vm.statusOptions.push({ display: 'Audit address Descrepency', value: '1' });
            vm.searchOptions.push({ display: 'Franchisee', value: '1' });
            vm.searchOptions.push({ display: 'Qb Invoice Id', value: '2' });
            vm.searchOptions.push({ display: 'MarketingClass', value: '3' });

        }

        function downloadAnnualData() {
            vm.downloading = true;
            return annualBatchService.downloadAnnualData(vm.query).then(function (result) {
                var fileName = "annualSalesData.xlsx";
                fileService.downloadFile(result.data, fileName);
                vm.downloading = false;

            }, function () { vm.downloading = false; });
        }

        $scope.$on('clearDates', function (event) {
            vm.query.periodStartDate = null;
            vm.query.periodEndDate = null;
            getRecords();
        });

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
                vm.query.marketingClassId = 0;
            }
        }

        function resetSearch() {
            vm.query.franchiseeId = 0;
            vm.query.qbInvoiceNumber = '';
            vm.searchOption = '';
            vm.query.marketingClassId = 0;
            vm.query.periodStartDate = null;
            vm.query.periodEndDate = null;
            $scope.$broadcast("reset-dates");
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
            if (vm.query.statusId == '1') {
                $scope.$emit("update-title", "Audit Customer Address Discripency ");
            }
            if (vm.query.statusId == '2') {
                $scope.$emit("update-title", "Audit Customer Update History ");
            }
            vm.mergedList = [];
            if (!vm.isSuperAdmin)
                vm.query.franchiseeId = $rootScope.identity.organizationId;
            getSalesCustomerCollection();
        }

        function getSalesCustomerCollection() {
            return annualBatchService.getAnnualCustomersAddressList(vm.query).then(function (result) {
                if (result != null && result.data != null) {
                    vm.invoiceIds = [];
                    vm.salesDataCustomerList = result.data.collection;
                    vm.salesDataCustomerList.statusId = vm.query.statusId;
                    vm.query.sort.order = result.data.filter.sortingOrder;
                    vm.count = result.data.pagingModel.totalRecords;
                    vm.query.sort.order = result.data.filter.sortingOrder;
                    vm.pagingModel = result.data.pagingModel;
                }
            });
        }


        function getmarketingClassCollection() {
            return customerService.getmarketingClassCollection().then(function (result) {
                vm.marketingClass = result.data;
            });
        }
        function sorting(propName) {
            vm.query.sort.propName = propName;
            vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
            getRecords();
        };

        if (vm.query.statusId == '1') {
            $scope.$emit("update-title", "Audit Customer Address Discripency ");
        }
        if (vm.query.statusId == '2') {
            $scope.$emit("update-title", "Audit Customer Update History ");
        }

        $q.all([getFranchiseeCollection(), getRecords(), prepareSearchOptions(), getmarketingClassCollection()]);

    }]);
}());