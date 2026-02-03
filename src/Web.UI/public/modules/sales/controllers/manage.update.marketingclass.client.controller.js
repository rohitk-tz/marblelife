(function () {
    'use strict';
    var SortColumns = {
        Id: 'Id',
        GeneratedOn: 'GeneratedOn',
        InvoiceId: 'InvoiceId'
    };
    angular.module(SchedulerConfiguration.moduleName).controller("UpdateMarketingClassController",
        ["$scope", "$rootScope", "$state", "$q", "APP_CONFIG", "$uibModal", "FranchiseeService", "GeoCodeService", "FileService", "Notification", "Toaster", "$filter", "SalesService",
            function ($scope, $rootScope, $state, $q, config, $uibModal, franchiseeService, geoCodeService, fileService, notification, toaster, $filter, salesService) {

                var vm = this;
                vm.getLogs = getLogs;
                vm.getInvoiceList = getInvoiceList;
                vm.downloadSalesData = downloadSalesData;
                vm.query2 = {
                    text: '',
                    uploadedBy: "",
                    pageNumber: 1,
                    statusId: 0,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };
                vm.changeTab = changeTab;
                vm.isInvoiceListActive = true;
                vm.isFileListActive = false;
                vm.isCustomRange = false;
                vm.changeYearOption = changeYearOption;
                vm.query = {
                    text: '',
                    uploadedBy: "",
                    pageNumber: 1,
                    statusId: 0,
                    pageSize: config.defaultPageSize,
                    year: new Date().getFullYear().toString(),
                    periodStartDate: null,
                    periodEndDate: null,
                    sortingColumn: '',
                    SortingOrder: 0

                };
                vm.reparse = reparse;
                vm.BatchStatus = DataHelper.SalesDataUploadStatus;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;

                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.downloadInvoiceList = downloadInvoiceList;
                vm.getLogs = getLogs;
                vm.pagingOptions = config.pagingOptions;
                vm.uploadNewFile = uploadNewFile;
                vm.pageChange = pageChange;
                vm.getUpdateSalesDataList = getUpdateSalesDataList;
                vm.SortColumns = SortColumns;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.currencyCode = $rootScope.identity.currencyCode;
                vm.searchOptions = []
                vm.searchOption = '';
                vm.resetSeachOption = resetSeachOption;
                vm.currentRole = $rootScope.identity.roleId;
                
                vm.updateBatchEditable = updateBatchEditable;
                vm.deleteRecord = deleteRecord;
                vm.saveZipCodeNote = saveZipCodeNote;
                vm.getNotes = getNotes;
                vm.searchOptionsForYear = searchOptionsForYear;
                vm.searchOptionsList = [];

                function searchOptionsForYear() {
                    vm.searchOptionsList = [];

                    const startYear = 2017;
                    const currentYear = new Date().getFullYear();

                    for (var year = startYear; year <= currentYear; year++) {
                        vm.searchOptionsList.push({ display: year.toString(), value: year.toString() });
                    }

                    // Add "Custom Range" at the end
                    vm.searchOptionsList.push({ display: 'Custom Range', value: 'Custom Range' });

                    // Set default selected year to current year
                    vm.searchYearOption = currentYear.toString();
                }

                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Status', value: '2' }, { display: 'Others', value: '3' });

                }
                function refresh() {
                    getUpdateSalesDataList();
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
                    vm.query.statusId = '0';
                    vm.searchOption = '';
                    $scope.$broadcast("reset-dates");
                    vm.query.pageNumber = 1;
                    vm.query.year = new Date().getFullYear().toString();
                    vm.isCustomRange = false;
                    getUpdateSalesDataList();
                }

                $scope.$on('clearDates', function (event) {
                    vm.isCustomRange = false;
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    getUpdateSalesDataList();
                });
                function getZipDataUploadStatus() {
                    return geoCodeService.getZipDataUploadStatus().then(function (result) {
                        vm.uploadStatus = result.data;
                    });
                }

                vm.sorting = sorting;


                function downloadFile() {
                    return fileService.getExcelByName().then(function (result) {
                        var fileName = "Sample_salesData.xlsx";
                        fileService.downloadFile(result.data, fileName);
                    });
                }


                function uploadNewFile() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/upload-invoice.client.view.html',
                        controller: 'UploadInvoiceController',
                        controllerAs: 'vm',
                        backdrop: 'static'
                    });

                    modalInstance.result.then(function () {
                        vm.getInvoiceList();
                    }, function () {

                    });
                }

                function pageChange() {
                    vm.getUpdateSalesDataList();
                }

                function getUpdateSalesDataList() {

                    return salesService.getUpdationMarketingClass(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.invoiceList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.sortingOrder = result.data.filter.sortingOrder;
                        }
                    });
                }

                function sorting(propName) {
                    vm.query.sortingColumn = propName;
                    vm.query.sortingOrder = (vm.query.sort.order == 0) ? 1 : 0;
                    getUpdateSalesDataList();
                };

                function downloadInvoiceList() {
                    vm.downloading = true;
                    return salesService.downloadInvoiceList(vm.query).then(function (result) {
                        var date = $filter('date')(new Date(), "dd_MM_yyyy");
                        var fileName = "InvoiceList_" + date + ".xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                        vm.selectedDownload = false;
                        vm.downloadAll = true;
                    }, function () { vm.downloading = false; });
                }

                function updateBatchEditable(batch) {
                    batch.notes = batch.tempNotes;
                    batch.isEditable = !batch.isEditable;
                }
                function saveZipCodeNote(geoCodeData) {
                    return geoCodeService.saveGeoCodeNotes(geoCodeData).then(function (result) {
                        if (result != null && result.data != null) {
                            toaster.show(result.message.message);
                            getUpdateSalesDataList();
                        }
                    });
                }
                function getNotes(description) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/show-description.client.view.html',
                        controller: 'ShowDescriptionController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Description: description
                                };
                            }
                        }
                    });
                }

                function getLogs(logFileId) {
                    if (logFileId == null) return;

                    return fileService.getFile(logFileId).then(function (result) {
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

                function deleteRecord(id) {
                    vm.isProcessing = true;
                    return geoCodeService.deleteBatch(id).then(function (result) {
                        toaster.show(result.message.message);
                        vm.isProcessing = false;
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                function reparse(id) {
                    vm.isProcessing = true;
                    return geoCodeService.reparseFile(id).then(function (result) {
                        toaster.show(result.message.message);
                        getUpdateSalesDataList();
                        vm.isProcessing = false;
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                function changeYearOption() {
                    if (vm.query.year == 'Custom Range') {
                        vm.query.year = new Date().getFullYear().toString();
                        vm.isCustomRange = true;
                    }
                    else {
                        vm.query.periodStartDate = null;
                        vm.query.periodEndDate = null;
                        vm.isCustomRange = false;
                        getUpdateSalesDataList();
                    }
                }

                function changeTab(tabNo) {
                    if (tabNo == 1) {
                        vm.isInvoiceListActive = true;
                        vm.isFileListActive = false;
                        getUpdateSalesDataList();
                        prepareSearchOptions();
                        searchOptionsForYear();
                    }
                    else if (tabNo == 2) {
                        vm.isFileListActive = true;
                        vm.isInvoiceListActive = false;
                        getInvoiceList();
                    }
                }

                function getInvoiceList() {
                    return salesService.getFileParse(vm.query2).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.batchList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            //vm.query.sort.order = result.data.filter.sortingOrder;
                        }
                    });
                }

                function downloadSalesData(fileId) {
                    return fileService.getExcel(fileId).then(function (result) {
                        var date = $filter('date')(new Date(), "dd_MM_yyyy");
                        var fileName = "UpdateSalesData" + ".xlsx";
                        fileService.downloadFile(result.data, fileName);
                    });
                }

                function getLogs(logFileId) {
                    if (logFileId == null) return;

                    return fileService.getFile(logFileId).then(function (result) {
                        // console.log(result.data);
                        showLogs(result.data);
                    });
                }
                $scope.$emit("update-title", "Update Sales Data");

                $q.all([getUpdateSalesDataList(), prepareSearchOptions(), searchOptionsForYear()]);

            }]);
}());