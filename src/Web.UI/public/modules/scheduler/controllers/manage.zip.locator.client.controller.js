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
    angular.module(SchedulerConfiguration.moduleName).controller("ZipLocatorController",
        ["$scope", "$rootScope", "$state", "$q", "APP_CONFIG", "$uibModal", "FranchiseeService", "GeoCodeService", "FileService", "Notification", "Toaster", "$filter",
            function ($scope, $rootScope, $state, $q, config, $uibModal, franchiseeService, geoCodeService, fileService, notification, toaster, $filter) {

                var vm = this;
                vm.downloadFiles = downloadFiles;
                vm.query = {
                    text: '',
                    uploadedBy: "",
                    pageNumber: 1,
                    statusId: 0,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };
                vm.closeModal = closeModal;
                vm.reparse = reparse;
                vm.BatchStatus = DataHelper.SalesDataUploadStatus;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;

                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.downloadSalesData = downloadSalesData;
                vm.getLogs = getLogs;
                vm.pagingOptions = config.pagingOptions;
                vm.uploadNewFile = uploadNewFile;
                vm.pageChange = pageChange;
                vm.getGeoList = getGeoList;
                vm.SortColumns = SortColumns;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.currencyCode = $rootScope.identity.currencyCode;
                vm.searchOptions = []
                vm.searchOption = '';
                vm.resetSeachOption = resetSeachOption;
                vm.currentRole = $rootScope.identity.roleId;
                vm.downloadAllGeoCode = downloadAllGeoCode;
                vm.updateBatchEditable = updateBatchEditable;
                vm.deleteRecord = deleteRecord;
                vm.saveZipCodeNote = saveZipCodeNote;
                vm.getNotes = getNotes;
                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Status', value: '2' }, { display: 'Others', value: '3' });
                }
                function refresh() {
                    getGeoList();
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
                    getGeoList();
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    getGeoList();
                });
                function getZipDataUploadStatus() {
                    return geoCodeService.getZipDataUploadStatus().then(function (result) {
                        vm.uploadStatus = result.data;
                    });
                }

                vm.sorting = sorting;

                //function getLogs(logFileId) {
                //    if (logFileId == null) return;

                //    return fileService.getFile(logFileId).then(function (result) {
                //        console.log(result.data);
                //        showLogs(result.data);
                //    });
                //}


                function downloadFile() {
                    return fileService.getExcelByName().then(function (result) {
                        var fileName = "Sample_salesData.xlsx";
                        fileService.downloadFile(result.data, fileName);
                    });
                }


                function uploadNewFile() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/upload-zip.client.view.html',
                        controller: 'UploadZipController',
                        controllerAs: 'vm',
                        backdrop: 'static'
                    });
                    modalInstance.result.then(function () {
                        vm.getGeoList();
                    }, function () {
                    });
                }

                function pageChange() {
                    vm.getGeoList();
                }

                function getGeoList() {
                    return geoCodeService.getGeoList(vm.query).then(function (result) {
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
                    getGeoList();
                };

                function downloadAllGeoCode() {
                    vm.downloading = true;
                    return geoCodeService.downloadAllGeoCode(vm.query).then(function (result) {
                        var date = $filter('date')(new Date(), "dd_MM_yyyy");
                        var fileName = "Geo_Code_" + date + ".xlsx";
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
                            getGeoList();
                        }
                    });
                }
                function getNotes(zipCodeNotes) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/show-zip-code-notes.client.view.html',
                        controller: 'ShowGoeCodeNotesController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    GeoCodeNotes: zipCodeNotes
                                };
                            }
                        }
                    });
                }
                function downloadSalesData(fileId) {
                    return fileService.getExcel(fileId).then(function (result) {
                        var date = $filter('date')(new Date(), "dd_MM_yyyy");
                        var fileName = "Geo_Code" + ".xlsx";
                        fileService.downloadFile(result.data, fileName);
                    });
                }

                //function getLogs(logFileId) {
                //    if (logFileId == null) return;

                //    return fileService.getFile(logFileId).then(function (result) {
                //        // console.log(result.data);
                //        showLogs(result.data);
                //    });
                //}
                function getLogs(logFileId, logForCountyFileId, logForZipFileId) {
                    if (logFileId == null) return;
                    showLogs(logFileId, logForCountyFileId, logForZipFileId);

                }
                function showLogs(logFileId, logForCountyFileId, logForZipFileId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/show-geo-code-logs.client.view.html',
                        controller: 'ShowGeoCodeLogsController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    LogFileId: logFileId,
                                    LogForCountyFileId: logForCountyFileId,
                                    LogForZipFileId: logForZipFileId,
                                    franchiseeId: 0,
                                    logFileId: logFileId,
                                };
                            }
                        },
                        backdrop: 'static',
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
                        getGeoList();
                        vm.isProcessing = false;
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                function closeModal() {
                    $('#exampleModalCenter').modal("hide");
                }

                function downloadFiles() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/modal-add-country.client.view.html',
                        controller: 'ModalAddCountryController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Query:vm.query
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                }
                $scope.$emit("update-title", "Manage Geo Code");

                $q.all([getGeoList(), prepareSearchOptions(), getZipDataUploadStatus()]);

            }]);
}());