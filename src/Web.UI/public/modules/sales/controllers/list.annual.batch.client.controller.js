(function () {
    'use strict';

    var SortColumns = {
        Id: 'Id',
        Franchisee: 'Franchisee',
        UploadedDate: 'UploadedDate',
        StartDate: 'StartDate',
        EndDate: 'EndDate',
        TotalAmount: 'TotalAmount',
        PaidAmount: 'PaidAmount',
        Status: 'Status',
        ReviewStatus: 'ReviewStatus'
    };

    angular.module(SalesConfiguration.moduleName).controller("ListAnnualBatchController",
        ["$scope", "$rootScope", "$state", "$q", "APP_CONFIG", "$uibModal", "FranchiseeService", "BatchService", "FileService", "Notification", "Toaster",
            "AnnualBatchService", "DashboardService", "URLAuthenticationServiceForEncryption",
            function ($scope, $rootScope, $state, $q, config, $uibModal, franchiseeService, batchService, fileService, notification, toaster, annualBatchService,
                dashboardService, URLAuthenticationServiceForEncryption) {

                var vm = this;
                vm.query = {
                    franchiseeId: '',
                    statusId: '',
                    year: null,
                    pageNumber: 1,
                    reviewStatus: '',
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };

                vm.BatchStatus = DataHelper.SalesDataUploadStatus;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;

                vm.pagingOptions = config.pagingOptions;
                vm.pageChange = pageChange;
                vm.getBatchList = getBatchList;
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
                vm.deleteBatch = deleteBatch;
                vm.manageBatch = manageBatch;
                vm.auditStatus = DataHelper.AuditStatus;
                vm.downloadAnnualSalesData = downloadAnnualSalesData;
                vm.reparseAnnualSalesData = reparseAnnualSalesData;
                vm.annualUpload = annualUpload;
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }
                function reparseAnnualSalesData(id) {
                    return annualBatchService.reparseBatch(id).then(function (res) {
                        vm.isProcessing = false;
                        getBatchList();
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }
                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' })

                    vm.searchOptions.push({ display: 'Status', value: '2' },
                        { display: 'Review Status', value: '3' });
                }
                function refresh() {
                    getBatchList();
                    if (vm.isFranchiseeAdmin) {
                        vm.query.franchiseeId = $rootScope.identity.organizationId;
                        getAnnualUploadResponse();
                    }
                }

                function downloadAnnualSalesData(fileId) {
                    return fileService.getExcel(fileId).then(function (result) {
                        var fileName = "AnnualData-" + fileId + ".xlsx";
                        fileService.downloadFile(result.data, fileName);
                    });
                }

                function annualUpload() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/upload-annual-batch.client.view.html',
                        controller: 'UploadAnnualBatchController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Response: vm.response
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        if (vm.isFranchiseeAdmin) {
                            vm.query.franchiseeId = $rootScope.identity.organizationId;
                            getAnnualUploadResponse();
                        }
                        getBatchList();
                    }, function () {

                    });
                }

                function resetSeachOption() {
                    if (vm.seachOption == '1') {
                        vm.query.statusId = '';
                        vm.query.pageNumber = 1;
                        vm.query.reviewStatusId = '';
                    }
                    else if (vm.seachOption == '2') {
                        vm.query.franchiseeId = '';
                        vm.query.pageNumber = 1;
                        vm.query.reviewStatusId = '';
                    }
                    else if (vm.seachOption == '3') {
                        vm.query.franchiseeId = '';
                        vm.query.statusId = '';
                        vm.query.pageNumber = 1;
                    }
                    else {
                        vm.query.statusId = '';
                        vm.query.franchiseeId = '';
                        vm.query.pageNumber = 1;
                        vm.query.reviewStatusId = '';
                    }
                }

                function resetSearch() {
                    vm.query.franchiseeId = '';
                    vm.query.statusId = '';
                    vm.query.year = null;
                    vm.searchOption = '';
                    vm.query.pageNumber = 1;
                    vm.query.reviewStatusId = '';
                    getBatchList();
                    if (vm.isFranchiseeAdmin) {
                        vm.query.franchiseeId = $rootScope.identity.organizationId;
                        getAnnualUploadResponse();
                    }
                }

                function manageBatch(batchId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/annul-sales-option.client.view.html',
                        controller: 'AnnualSalesOptionController',
                        controllerAs: 'vm',
                        size: 'sm',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    BatchId: batchId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function (result) {
                        if (result == 1) {
                            var isAccept = false;
                        }
                        else if (result == 2) {
                            var isAccept = true;
                        }
                        setBatchAction(isAccept, batchId);
                    }, function () {

                    });
                }

                function setBatchAction(isAccept, batchId) {
                    return annualBatchService.manageBatch(isAccept, batchId).then(function (res) {
                        vm.isProcessing = false;
                        getBatchList();
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                function deleteBatch(id) {
                    notification.showConfirm("Do you really want to Delete the File?", "Delete", function () {
                        vm.isProcessing = true;
                        return annualBatchService.deleteBatch(id).then(function (res) {
                            vm.isProcessing = false;
                            getBatchList();
                        }).catch(function (err) {
                            vm.isProcessing = false;
                        });
                    });
                }

                function getSalesDataUploadStatus() {
                    return batchService.getSalesDataUploadStatus().then(function (result) {
                        vm.uploadStatus = result.data;
                    });
                }

                function getReviewStatus() {
                    return annualBatchService.getReviewStatus().then(function (result) {
                        vm.reviewStatus = result.data;
                    });
                }


                function getSalesData(franchiseeId) {
                    $state.go('core.layout.sales.list', { franchiseeId: franchiseeId });
                }

                vm.sorting = sorting;

                function getLogs(logFileId) {
                    if (logFileId == null) return;

                    return fileService.getFile(logFileId).then(function (result) {
                        //console.log(result.data);
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

                function pageChange() {
                    vm.getBatchList();
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function getBatchList() {
                    return annualBatchService.getBatchList(vm.query).then(function (result) {
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

                function getYears() {
                    return annualBatchService.getYears().then(function (result) {
                        vm.yearCollection = result.data;
                    })
                }

                function getAnnualUploadResponse() {
                    return dashboardService.getAnnualUploadResponse(vm.query.franchiseeId).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.response = result.data;
                            vm.response.franchiseeId = vm.franchiseeId;
                        }
                    });
                }

                if (vm.isFranchiseeAdmin) {
                    vm.query.franchiseeId = $rootScope.identity.organizationId;
                    getAnnualUploadResponse();
                }

                $scope.$emit("update-title", "Annual Uploads");

                $q.all([getFranchiseeCollection(), getBatchList(), getSalesDataUploadStatus(), prepareSearchOptions(), getYears(), getReviewStatus()]);

            }]);

}());