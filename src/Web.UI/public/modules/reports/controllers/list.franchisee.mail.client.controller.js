(function () {
    'use strict';

    var SortColumns = {
        Franchisee: 'Franchisee',
        ID: 'ID'
    };

    angular.module(ReportsConfiguration.moduleName).controller("FranchiseeMailssController",
   ["$state", "$stateParams", "$q", "$scope", "FranchiseeGrowthReportService", "APP_CONFIG", "$rootScope", "FileService", "DashboardService",
       "$uibModal", "Clock", "FranchiseeService",
   function ($state, $stateParams, $q, $scope, franchiseeGrowthReportService, config, $rootScope, fileService, dashboardService,
       $uibModal, clock, franchiseeService) {

       var vm = this;

       vm.currentDate = clock.now();
       vm.query = {
           FranchiseeId: 0,
           ReportDateStart: vm.currentDate,
       };
       vm.getARReport = getARReport;
       vm.Roles = DataHelper.Role;
       vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
       vm.resetSearch = resetSearch;
       vm.refresh = refresh;
       vm.pagingOptions = config.pagingOptions;
       vm.currentPage = vm.query.pageNumber;
       vm.count = 0;
       vm.pageChange = pageChange;
       vm.searchOptions = [];
       vm.searchOption = '';
       vm.franchiseeIds = [];
       vm.total = 0;
       vm.onetothirty = 0;
       vm.thirtytosixty = 0;
       vm.sixtytoninty = 0;
       vm.morethanninty = 0;

       function resetSearch() {
           vm.query.FranchiseeId = 0;
           vm.query.ReportDateStart = vm.currentDate;
           getARReport();
       }

       function refresh() {
           getARReport();
       }

       function getARReport() {
           if (vm.query.year <= 0) {
               vm.query.year = clock.getYear(vm.currentDate);
           }
           if (vm.query.month == null || vm.query.month <= 0) {
               if (vm.query.year == clock.getYear(vm.currentDate)) {
                   vm.query.month = clock.getMonth(vm.currentDate) - 1 + "";
               }
           }

           return franchiseeGrowthReportService.getARReport(vm.query).then(function (result) {
               var localSumForThirsty = '0';
               var localSumForSixty = '0';
               var localSumForNinty = '0';
               var localSumForMorethanNinty = '0';
               if (result != null && result.data != null) {
                   vm.franchiseeIds = [];
                   vm.reportList = result.data.collection;
                   vm.filter = result.data.filter.reportDateStart;
                   vm.total = 0;
                   angular.forEach(vm.reportList, function (item) {
                       vm.total += item.totalInt;
                       localSumForThirsty = parseFloat(item.thirty.split(' ')[1]);
                       vm.onetothirty += (localSumForThirsty);
                       localSumForSixty = parseFloat(item.sixty.split(' ')[1]);
                       vm.thirtytosixty += localSumForSixty;
                       localSumForNinty = parseFloat(item.ninety.split(' ')[1]);
                       vm.sixtytoninty += localSumForNinty;
                       localSumForMorethanNinty = parseFloat(item.moreThanNinety.split(' ')[1]);
                       vm.morethanninty += localSumForMorethanNinty;
                   });
               }
           });
       }

       function getFranchiseeCollection() {
           return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
               vm.franchiseeCollection = result.data;
               vm.franchiseeCollection.push({ display: "All", value: "0" });
           });
       }

       function pageChange() {
           getARReport();
       };


       $scope.$emit("update-title", "AR Report");

       $q.all([getARReport(), getFranchiseeCollection(), ]);
   }]);
}());