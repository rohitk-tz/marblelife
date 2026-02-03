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

    angular.module(SalesConfiguration.moduleName).controller("SalesDataDetailController",
        ["$rootScope", "$uibModalInstance", "APP_CONFIG", "modalParam", "BatchService", "SalesService",
            "URLAuthenticationServiceForEncryption",
            function ($rootScope, $uibModalInstance, config, modalParam, batchService,
                salesService, URLAuthenticationServiceForEncryption) {

                var vm = this;
                vm.query = {
                    text: '',
                    salesDataUploadId: modalParam.BatchId,
                    qbInvoiceNumber: '',
                    franchiseeId: 0,
                    customerId: 0,
                    customerName: '',
                    marketingClass: '',
                    periodStartDate: null,
                    periodEndDate: null,
                    pageNumber: 1,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };
                vm.query.salesDataUploadId = modalParam.BatchId;
                vm.currencyCode = $rootScope.identity.currencyCode;
                vm.getSalesDataDetail = getSalesDataDetail;
                vm.deleteBatch = deleteBatch;
                vm.pageChange = pageChange;
                vm.sorting = sorting;
                vm.isProcessing = false;
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                function pageChange() {
                    vm.getSalesDataDetail();
                }
                function getSalesDataDetail() {
                    return salesService.getSalesDataList(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.salesDataList = result.data.collection;
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            vm.count = result.data.pagingModel.totalRecords;
                        }
                    });
                }

                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getSalesDataCollection();
                };

                function deleteBatch(id) {
                    vm.isProcessing = true;
                    return batchService.deleteBatch(id).then(function (res) {
                        console.log(res);
                        vm.isProcessing = false;
                        $uibModalInstance.close(res);
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                getSalesDataDetail();
            }]);
}());