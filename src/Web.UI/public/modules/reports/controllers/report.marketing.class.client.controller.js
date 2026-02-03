(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).controller("MarketingClassReportController",
   ["$state", "$stateParams", "$q", "$scope", "MarketingClassLeadersService", "APP_CONFIG", "$rootScope", "FileService", "DashboardService",
       "$uibModal", "Clock", "FranchiseeService",
   function ($state, $stateParams, $q, $scope, marketingClassLeadersService, config, $rootScope, fileService, dashboardService,
       $uibModal, clock, franchiseeService) {

       var vm = this;
       var temp = angular.copy(config.defaultClassTypeIds);
       vm.query = {
           year: null,
           month: null,
           idList: temp,
           typeIds: [],
           franchiseeId: '',
           startDate: null,
           endDate: null
       };

       vm.getClassReport = getClassReport;
       vm.Roles = DataHelper.Role;
       vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
       vm.currentRole = $rootScope.identity.roleId;
       vm.loggedInFranchiseeId = $rootScope.identity.organizationId;
       vm.defaultCurrency = config.defaultCurrency;
       vm.resetSearch = resetSearch;
       vm.refresh = refresh;
       vm.loggedInCurrency = $rootScope.identity.currencyCode;

       $scope.settings = {
           scrollable: true,
           enableSearch: true,
           selectedToTop: true,
           buttonClasses: 'btn btn-primary leader_btn',
       };
       $scope.translationTexts = {
           checkAll: 'Select All',
           uncheckAll: 'Deselect All',
           selectGroup: 'Select All',
           dynamicButtonTextSuffix: 'Selected'
       };

       function refresh() {
           var ids = angular.copy(config.defaultClassTypeIds);
           vm.query.idList = angular.copy(config.defaultClassTypeIds);
           getClassReport();
       }

       function resetSearch() {
           var ids = angular.copy(config.defaultClassTypeIds);
           vm.query.year = null;
           vm.query.month = null;
           vm.query.idList = [];
           vm.query.franchiseeId = '',
           vm.query.typeIds = [];
           vm.query.idList = angular.copy(config.defaultClassTypeIds);
           getClassReport();
       }

       function getClassReport() {
           vm.query.typeIds = [];
           if (vm.query.year != null && vm.query.month == null) {
               vm.query.startDate = "01/01/" + vm.query.year;
               if (vm.query.year == clock.getYear(clock.now())) {
                   vm.query.endDate = clock.now();
               }
               else
                   vm.query.endDate = "12/31/" + vm.query.year;
           }
           if (vm.query.year == null && vm.query.month == null) {
               vm.query.endDate = clock.now();
               vm.query.year = clock.getYear(clock.now());
               vm.query.startDate = clock.getStartDateOfYear();
           }
           if (vm.query.month != null) {
               if (vm.query.year != null) {
                   var daysInMonth = clock.getDaysInMonth(vm.query.month, vm.query.year)
                   vm.query.startDate = vm.query.month + "/01/" + vm.query.year;
                   vm.query.endDate = vm.query.month + "/" + daysInMonth + "/" + vm.query.year;
               }
               else {
                   var daysInMonth = clock.getDaysInMonth(vm.query.month, currentYear)
                   vm.query.startDate = vm.query.month + "/01/" + currentYear;
                   vm.query.endDate = vm.query.month + "/" + daysInMonth + "/" + currentYear;
               }
           }

           if (vm.query.idList.length > 0) {
               angular.forEach(vm.query.idList, function (value) {
                   vm.query.typeIds.push(value.id);
               });
           }
           if (vm.query.typeIds.length <= 0) {
               //vm.query.idList = angular.copy(config.defaultClassTypeIds);
               //var ids = angular.copy(config.defaultClassTypeIds);
               var ids=[{ id: 0 }]
               angular.forEach(ids, function (value) {
                   vm.query.typeIds.push(value.id);
               });
           }

           vm.query.startDate = moment(vm.query.startDate).format("MM/DD/YYYY");
           vm.query.endDate = moment(vm.query.endDate).format("MM/DD/YYYY");

           return marketingClassLeadersService.getClassReport(vm.query).then(function (result) {
               if (result != null && result.data != null) {
                   vm.reportList = result.data.collection;
               }
           });
       }

       function getLastTwentyYearCollection() {
           return dashboardService.getLastTwentyYearCollection().then(function (result) {
               vm.yearCollection = result.data;
           })
       }

       function getMonthCollection() {
           return dashboardService.getMonthNames().then(function (result) {
               vm.monthCollection = result.data;
               vm.monthCollection.push({ display: "All", value: null });
           })
       }

       function getClassTypeCollection() {
           return marketingClassLeadersService.getmarketingClassCollection().then(function (result) {
               vm.marketingClass = result.data;
           });
       }

       function getFranchiseeCollection() {
           return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
               vm.franchiseeCollection = result.data;
               vm.franchiseeCollection.push({ display: "All", value: null });
           });
       }

       $scope.$emit("update-title", "Leaderboard - Marketing Class");
       $q.all([getClassReport(), getLastTwentyYearCollection(), getMonthCollection(), getClassTypeCollection(), getFranchiseeCollection()]);

   }]);
}());