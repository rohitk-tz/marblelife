(function () {
    'use strict';

    var SortColumns = {
        Id: 'Id',
        Franchisee: 'Franchisee',
        UploadedDate: 'UploadedDate',
        StartDate: 'StartDate',
        EndDate: 'EndDate',
        Frequency: 'Frequency',
        TotalAmount: 'TotalAmount',
        PaidAmount: 'PaidAmount',
        Customers: 'Customers',
        Invoices: 'Invoices',
        LastInvoiceId: 'LastInvoiceId',
        Status: 'Status'
    };

    angular.module(SalesConfiguration.moduleName).controller("ListBatchController",
        ["$scope", "$rootScope", "$state", "$q", "APP_CONFIG", "$uibModal",
            "FranchiseeService", "BatchService", "FileService", "Notification", "Toaster", "URLAuthenticationServiceForEncryption", "SalesService",
            function ($scope, $rootScope, $state, $q, config, $uibModal, franchiseeService,
                batchService, fileService, notification, toaster, URLAuthenticationServiceForEncryption, salesService) {

                var vm = this;
                vm.query = {
                    franchiseeId: '',
                    statusId: '',
                    periodStartDate: null,
                    periodEndDate: null,
                    pageNumber: 1,
                    text: '',
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };

                vm.BatchStatus = DataHelper.SalesDataUploadStatus;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;

                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;

                vm.pagingOptions = config.pagingOptions;
                vm.uploadNewFile = uploadNewFile;
                vm.pageChange = pageChange;
                vm.getBatchList = getBatchList;
                vm.downloadSalesData = downloadSalesData;
                vm.getLogs = getLogs;
                vm.getSalesData = getSalesData;
                vm.SortColumns = SortColumns;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.currencyCode = $rootScope.identity.currencyCode;
                vm.searchOptions = []
                vm.searchOption = '';
                vm.resetSeachOption = resetSeachOption;
                vm.currentRole = $rootScope.identity.roleId;
                vm.updateBatch = updateBatch;
                vm.updateCustomerList = updateCustomerList;
                vm.showDetails = showDetails;
                vm.downloadSampleFile = downloadSampleFile;
                vm.reparse = reparse;
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }
                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' })

                    vm.searchOptions.push({ display: 'Status', value: '2' }, { display: 'Others', value: '3' });
                }
                function refresh() {
                    getBatchList();
                }

                function resetSeachOption() {
                    if (vm.seachOption == '1') {
                        vm.query.statusId = '';
                        vm.query.text = '';
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '2') {
                        vm.query.franchiseeId = '';
                        vm.query.text = '';
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '3') {
                        vm.query.franchiseeId = '';
                        vm.query.statusId = '';
                        vm.query.pageNumber = 1
                    }
                    else {
                        vm.query.statusId = '';
                        vm.query.franchiseeId = '';
                        vm.query.text = '';
                        vm.query.pageNumber = 1
                    }
                }

                function resetSearch() {
                    vm.query.text = '';
                    vm.query.franchiseeId = '';
                    vm.query.statusId = '';
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    vm.searchOption = '';
                    $scope.$broadcast("reset-dates");
                    vm.query.pageNumber = 1
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    getBatchList();
                });
                function getSalesDataUploadStatus() {
                    return batchService.getSalesDataUploadStatus().then(function (result) {
                        vm.uploadStatus = result.data;
                    });
                }

                function getSalesData(franchiseeId) {
                    $state.go('core.layout.sales.list', { franchiseeId: franchiseeId });
                }

                vm.sorting = sorting;

                function getLogs(logFileId) {
                    if (logFileId == null) return;

                    return fileService.getFile(logFileId).then(function (result) {
                        // console.log(result.data);
                        showLogs(result.data);
                    });
                }

                function showLogs(logs) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/show-logs.client.view.html',
                        controller: 'ShowLogsController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: { logs: function () { return logs; } }
                    });
                }

                function downloadSalesData(fileId) {
                    return fileService.getExcel(fileId).then(function (result) {
                        var fileName = "SalesData-" + fileId + ".xlsx";
                        fileService.downloadFile(result.data, fileName);
                    });
                }

                function downloadSampleFile() {
                    return fileService.getExcelByName().then(function (result) {
                        var fileName = "Sample_salesData.xlsx";
                        fileService.downloadFile(result.data, fileName);
                    });
                }

                function showDetails(batchId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/sales-data-details.client.view.html',
                        controller: 'SalesDataDetailController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    BatchId: batchId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        vm.getBatchList();
                    }, function () {

                    });
                }

                function reparse(batchId) {
                    notification.showConfirm("Do you really want to Reparse the File?", "Reparse", function () {
                        return batchService.reparseBatch(batchId).then(function (result) {
                            if (result.data != true)
                                toaster.error(result.message.message);
                            else
                                toaster.show(result.message.message);
                            getBatchList();
                        });
                    });
                }

                function uploadNewFile() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/upload-batch.client.view.html',
                        controller: 'UploadBatchController',
                        controllerAs: 'vm',
                        backdrop: 'static'
                    });

                    modalInstance.result.then(function () {
                        vm.getBatchList();
                    }, function () {

                    });
                }

                function updateCustomerList() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/update-customer-list.client.view.html',
                        controller: 'UpdateCustomerListController',
                        controllerAs: 'vm',
                        backdrop: 'static'
                    });

                    modalInstance.result.then(function () {
                        vm.getBatchList();
                    }, function () {

                    });
                }

                function updateBatch(batch) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/update-batch.client.view.html',
                        controller: 'UpdateBatchController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Batch: batch
                                };
                            }
                        }
                    });

                    modalInstance.result.then(function () {
                        vm.getBatchList();
                    }, function () {

                    });
                }

                function pageChange() {
                    vm.getBatchList();
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function getBatchList() {
                    return batchService.getBatchList(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.batchList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.sort.order = result.data.filter.sortingOrder;
                        }
                    });
                }
                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getBatchList();
                };

                function getServiceCollection() {
                    return franchiseeService.getServiceTypeCollectionNewOrder().then(function (result) {
                        vm.serviceTypeList = result.data;
                        vm.flatServiceList = [];

                        angular.forEach(vm.serviceTypeList, function (group, index1) {
                            if (index1 == 0) {
                                vm.flatServiceList.push({
                                    display: "FRANCHISEE SERVICES:-",
                                    isCategory: true
                                });
                            }
                            if (index1 == 1) {
                                vm.flatServiceList.push({
                                    display: "ML- DISTRIBUTION SERVICES:-",
                                    isCategory: true
                                });
                            }
                            if (index1 == 2) {
                                vm.flatServiceList.push({
                                    display: "FRONT OFFICE CALL MANAGEMENT:-",
                                    isCategory: true
                                });
                            }

                            angular.forEach(group.collection, function (item, index) {
                                if (index1 == 1) {
                                    vm.flatServiceList.push({
                                        display: "MLD-" + item.display,
                                        isCategory: false
                                    });
                                } else {
                                    vm.flatServiceList.push({
                                        display: item.display,
                                        isCategory: false
                                    });
                                }
                            });
                        });

                    });
                }

                function getmarketingClassCollection() {
                    return salesService.getmarketingClassCollectionNewList().then(function (result) {
                        vm.marketingClassList = result.data;
                        vm.flatMarketingClassList = [];

                        angular.forEach(vm.marketingClassList, function (group, index1) {
                            vm.flatMarketingClassList.push({
                                display: group.groupName + ":-",
                                isCategory: true
                            });
                            angular.forEach(group.collection, function (item, index) {
                                vm.flatMarketingClassList.push({
                                    display: item.display,
                                    isCategory: false
                                });
                            });
                        });
                    });
                }

                $scope.$emit("update-title", "Manage Uploads");

                $q.all([getFranchiseeCollection(), getBatchList(), getSalesDataUploadStatus(), prepareSearchOptions(), getServiceCollection(), getmarketingClassCollection()]);

            }]);

}());