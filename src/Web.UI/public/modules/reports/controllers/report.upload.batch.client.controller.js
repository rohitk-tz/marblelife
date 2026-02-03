(function () {
    'use strict';

    var SortColumns = {
        Franchisee: 'Franchisee',
        FeeProfile: 'FeeProfile',
        ID: 'ID',
        WaitPeriod: 'WaitPeriod',
        ExpectedDate: 'ExpectedDate',
        ActualDate: 'ActualDate'
    };

    angular.module(ReportsConfiguration.moduleName).controller("UploadBatchReportController",
       ["$state", "$stateParams", "$q", "$scope", "UploadBatchReportService", "APP_CONFIG", "$rootScope", "FileService", "FranchiseeService",
           "$uibModal", "Clock", "DashboardService",
       function ($state, $stateParams, $q, $scope, uploadBatchReportService, config, $rootScope, fileService, franchiseeService,
           $uibModal, clock, dashboardService) {
           var vm = this;
           vm.query = {
               pageNumber: 1,
               year: 0,
               month: 0,
               franchiseeId: 0,
               statusId: null,
               periodStartDate: null,
               periodEndDate: null,
               pageSize: config.defaultPageSize,
               sort: { order: 0, propName: '' }
           };

           vm.getUploadBatchReport = getUploadBatchReport;
           vm.pagingOptions = config.pagingOptions;
           vm.currentPage = vm.query.pageNumber;
           vm.count = 0;
           vm.sorting = sorting;
           vm.SortColumns = SortColumns;
           vm.pageChange = pageChange;
           vm.Roles = DataHelper.Role;
           vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
           vm.currentRole = $rootScope.identity.roleId;
           vm.resetSearch = resetSearch;
           vm.refresh = refresh;
           vm.downloadMissingUploadReport = downloadMissingUploadReport;
           vm.ids = [];

           vm.options = [];
           function prepareOptions() {
               vm.options.push({ display: 'Late Uploaded', value: 1 }),
               vm.options.push({ display: 'Not Uploaded', value: 0 });
           };

           function downloadMissingUploadReport() {
               vm.downloading = true;
               return uploadBatchReportService.downloadMissingUploadReport(vm.query).then(function (result) {
                   var fileName = "missingUploadReport.xlsx";
                   fileService.downloadFile(result.data, fileName);
                   vm.downloading = false;
               }, function () { vm.downloading = false; });
           }

           function refresh() {
               getUploadBatchReport();
           }

           function resetSearch() {
               vm.query.franchiseeId = 0;
               vm.query.year = 0;
               vm.query.month = 0;
               vm.query.periodStartDate = null;
               vm.query.periodEndDate = null;
               vm.searchOption = '';
               vm.query.statusId = null;
               $scope.$broadcast("reset-dates");
               vm.query.pageNumber = 1;
               getUploadBatchReport();
           }

           function getUploadBatchReport() {
               vm.currentDate = clock.now();
               //if (vm.query.year <= 0)
               //    vm.query.year = clock.getYear(vm.currentDate);
               if (vm.query.statusId == null)
               {
                   vm.query.statusId = 0;
               }
               if (vm.query.periodEndDate == null)
               {
                   vm.query.year = 2018;
               }
               else
               {
                   vm.query.year = null;

               }
               return uploadBatchReportService.getUploadBatchReport(vm.query).then(function (result) {
                   if (result != null && result.data != null) {
                       vm.ids = [];
                       vm.reportList = result.data.collection;
                       vm.count = result.data.pagingModel.totalRecords;
                       vm.query.sort.order = result.data.filter.sortingOrder;
                       addResultToList(vm.reportList);
                   }
               });
           }

           function addResultToList() {
               angular.forEach(vm.reportList, function (value, key) {
                   vm.ids.push(value.id);
               })
           }

           function sorting(propName) {
               vm.query.sort.propName = propName;
               vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
               getUploadBatchReport();
           };

           function pageChange() {
               getUploadBatchReport();
           };

           function getFranchiseeCollection() {
               return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                   vm.franchiseeCollection = result.data;
               });
           }

           function getMonthCollection() {
               return dashboardService.getMonthNames().then(function (result) {
                   vm.monthCollection = result.data;
               })
           }

           function getYears() {
               return uploadBatchReportService.getYears().then(function (result) {
                   vm.yearCollection = result.data;
               })
           }

           $scope.$emit("update-title", "Sales Data Upload Report");
           $q.all([getUploadBatchReport(), getFranchiseeCollection(), getMonthCollection(), getYears(), prepareOptions()]);

       }]);
}());