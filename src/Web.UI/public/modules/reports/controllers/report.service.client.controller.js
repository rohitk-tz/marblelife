(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).controller("ServiceReportController",
   ["$state", "$stateParams", "$q", "$scope", "ServiceLeadersService", "APP_CONFIG", "$rootScope", "FileService", "DashboardService", "$uibModal", "Clock", "FranchiseeService",
   function ($state, $stateParams, $q, $scope, serviceLeadersService, config, $rootScope, fileService, dashboardService, $uibModal, clock, franchiseeService) {
       var vm = this;
       var temp = angular.copy(config.defaultServiceTypeIds);
       vm.query = {
           year: null,
           month: null,
           idList: temp,
           typeIds: [],
           franchiseeId: '',
           startDate: null,
           endDate: null
       };

       vm.getServiceReport = getServiceReport;
       vm.Roles = DataHelper.Role;
       vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
       vm.defaultCurrency = config.defaultCurrency;
       vm.currentRole = $rootScope.identity.roleId;
       vm.resetSearch = resetSearch;
       vm.loggedInFranchiseeId = $rootScope.identity.organizationId;
       vm.refresh = refresh;
       vm.loggedInCurrency = $rootScope.identity.currencyCode;
       $scope.settings = {
           scrollable: true,
           enableSearch: true,
           selectedToTop: true,
           buttonClasses: 'btn btn-primary leader_btn'
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
           getServiceReport();
       }

       function resetSearch() {
           var ids = angular.copy(config.defaultClassTypeIds);
           vm.query.year = null;
           vm.query.month = null;
           vm.query.idList = [];
           vm.query.franchiseeId = '',
           vm.query.typeIds = [];
           vm.query.idList = angular.copy(config.defaultClassTypeIds);
           getServiceReport();
       }


       function getServiceReport() {
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
               //vm.query.idList = angular.copy(config.defaultServiceTypeIds);
               //var ids = angular.copy(config.defaultServiceTypeIds);
               var ids = [{ id: 0 }]
               angular.forEach(ids, function (value) {
                   vm.query.typeIds.push(value.id);
               });
           }
           vm.query.startDate = moment(vm.query.startDate).format("MM/DD/YYYY");
           vm.query.endDate = moment(vm.query.endDate).format("MM/DD/YYYY");

           return serviceLeadersService.getServiceReport(vm.query).then(function (result) {
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

       function getServiceTypeCollection() {
           return serviceLeadersService.getServiceTypes().then(function (result) {
               vm.serviceTypes = result.data;
           });
       }

       function getFranchiseeCollection() {
           return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
               vm.franchiseeCollection = result.data;
               vm.franchiseeCollection.push({ display: "All", value: null });
           });
       }


       $scope.$emit("update-title", "Leaderboard - Service");
       $q.all([getServiceReport(), getLastTwentyYearCollection(), getMonthCollection(), getServiceTypeCollection(), getFranchiseeCollection()]);

   }]);
}());