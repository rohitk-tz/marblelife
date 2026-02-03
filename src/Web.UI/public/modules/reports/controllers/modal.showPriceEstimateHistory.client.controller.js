(function () {
    'use strict';
    var SortColumns = {
        Id: 'Id',
        UploadedDate: 'UploadedDate',
        Status: 'Status'
    };
    angular.module(SchedulerConfiguration.moduleName).controller("ShowPriceEstimateHistoryModal",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "Toaster", "$filter", "$q", "ManagePriceEstimateService", "APP_CONFIG", "FileService",
            function ($scope, $rootScope, $state, $uibModalInstance, toaster, $filter, $q, managePriceEstimateService, config, fileService) {
                var vm = this;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                vm.query = {
                    text: '',
                    uploadedBy: "",
                    pageNumber: 1,
                    statusId: 0,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.isFranchiseeAdmin;
                vm.pagingOptions = config.pagingOptions;
                vm.BatchStatus = DataHelper.SalesDataUploadStatus;
                vm.getPriceEstimateUploadList = getPriceEstimateUploadList;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.downloadPriceEstimateData = downloadPriceEstimateData;
                vm.pageChange = pageChange;
                vm.getLogs = getLogs;

                function getPriceEstimateUploadList() {
                    return managePriceEstimateService.getPriceEstimateUploadList(vm.query).then(function (result) {
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
                    getPriceEstimateUploadList();
                };

                function pageChange() {
                    vm.getPriceEstimateUploadList();
                }

                function downloadPriceEstimateData(fileId) {
                    return fileService.getExcel(fileId).then(function (result) {
                        var date = $filter('date')(new Date(), "dd_MM_yyyy");
                        var fileName = "PriceEstimate_"+ date + ".xlsx";
                        fileService.downloadFile(result.data, fileName);
                    });
                }

                function getLogs(logFileId) {
                    if (logFileId == null)
                        return;
                    vm.logFileId = logFileId
                    getGeneralLog();
                }

                function getGeneralLog() {
                    if (vm.logFileId == null || vm.logFileId == 0) {
                        vm.logs = "No Records Found."
                        return;
                    }
                    return fileService.getFile(vm.logFileId).then(function (result) {
                        vm.logs = result.data;
                        vm.showLogs = true;
                    });
                }

                $q.all([getPriceEstimateUploadList()]);
            }
        ]
    )
}());