(function () {
    'use strict';

    var SortColumns = {
        ID: 'ID',
        WebLeadId: 'WebLeadId',
        Name: 'Name',
        Email: 'Email',
        Phone: 'Phone',
        Country: 'Country',
        PropertyType: 'PropertyType',
        SurfaceType: 'SurfaceType',
        Contact: 'Contact',
        Franchisee: 'Franchisee',
        FEmail: 'FEmail',
        Description: 'Description',
        Date: 'Date',
        URL: 'URL',
        InvoiceId: 'InvoiceId'
    };

    angular.module(ReportsConfiguration.moduleName).controller("WebLeadController",
       ["$state", "$stateParams", "$q", "$scope", "WebLeadService", "APP_CONFIG", "$rootScope", "FileService", "FranchiseeService", "$uibModal", "Clock",
       function ($state, $stateParams, $q, $scope, webLeadService, config, $rootScope, fileService, franchiseeService, $uibModal, clock) {
           var vm = this;
           vm.query = {
               pageNumber: 1,
               startDate: null,
               endDate: null,
               franchiseeId: 0,
               propertyType: '',
               convertedLead: null,
               url: '',
               pageSize: config.defaultPageSize,
               text: '',
               name: '',
               sort: { order: 0, propName: '' },
               street: '',
               zipCode: '',
               city: '',
               state: ''
           };

           vm.getWebLeads = getWebLeads;
           vm.getPropertyTypes = getPropertyTypes;
           vm.pagingOptions = config.pagingOptions;
           vm.currentPage = vm.query.pageNumber;
           vm.count = 0;
           vm.viewInvoice = viewInvoice;
           vm.sorting = sorting;
           vm.SortColumns = SortColumns;
           vm.pageChange = pageChange;
           vm.searchOptions = [];
           vm.searchOption = '';
           vm.resetSeachOption = resetSeachOption;
           vm.Roles = DataHelper.Role;
           vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
           vm.currentRole = $rootScope.identity.roleId;
           vm.resetSearch = resetSearch;
           vm.refresh = refresh;
           vm.downloadWebLead = downloadWebLead;
           vm.ids = [];
           vm.viewDesc = viewDesc;
           vm.openUrl = openUrl;
           vm.leadOptions = [];

           function prepareLeadOptions() {
               vm.leadOptions.push({ display: 'Yes', value: 1 }),
               vm.leadOptions.push({ display: 'No', value: 0 });
           };
           function viewDesc(desc) {
               var modalInstance = $uibModal.open({
                   animation: true,
                   templateUrl: 'modules/reports/views/service-description.client.view.html',
                   controller: 'ServiceDescriptionController',
                   controllerAs: 'vm',
                   backdrop: 'static',
                   size: 'lg',
                   resolve: {
                       modalParam: function () {
                           return {
                               Description: desc
                           };
                       }
                   }
               });
               modalInstance.result.then(function () {
               }, function () { });
           }

           function openUrl(url) {
               window.open("https://" + url, '_blank');
           }

           function downloadWebLead() {
               vm.downloading = true;
               return webLeadService.downloadWebLead(vm.query).then(function (result) {
                   var fileName = "webLeadReport.xlsx";
                   fileService.downloadFile(result.data, fileName);
                   vm.downloading = false;
               }, function () { vm.downloading = false; });
           }

           function prepareSearchOptions() {
               if (vm.currentRole == vm.Roles.SuperAdmin)
                   vm.searchOptions.push({ display: 'Franchisee', value: '1' })
               vm.searchOptions.push({ display: 'Name', value: '5' });
               vm.searchOptions.push({ display: 'URL', value: '6' });
               vm.searchOptions.push({ display: 'PropertyType', value: '2' });
               vm.searchOptions.push({ display: 'Converted Leads', value: '3' });
               vm.searchOptions.push({ display: 'Street', value: '7' });
               vm.searchOptions.push({ display: 'City', value: '9' });
               vm.searchOptions.push({ display: 'State', value: '10' });
               vm.searchOptions.push({ display: 'Zip Code', value: '8' });
               vm.searchOptions.push({ display: 'Other', value: '4' });
           }

           function refresh() {
               getWebLeads();
           }

           function viewInvoice(invoiceId) {
               var modalInstance = $uibModal.open({
                   animation: true,
                   templateUrl: 'modules/sales/views/invoice-detail.client.view.html',
                   controller: 'InvoiceDetailController',
                   controllerAs: 'vm',
                   backdrop: 'static',
                   size: 'lg',
                   resolve: {
                       modalParam: function () {
                           return {
                               InvoiceId: invoiceId
                           };
                       }
                   }
               });

               modalInstance.result.then(function () {

               }, function () {

               });
           }

           function resetSeachOption() {
               if (vm.searchOption == '1') {
                   vm.query.name = '';
                   vm.query.text = '';
                   vm.query.propertyType = '';
                   vm.query.pageNumber = 1;
                   vm.query.url = '',
                       vm.query.zipCode = '';
                   vm.query.street = '';
                   vm.query.convertedLead = null;
                   vm.query.city = '';
                   vm.query.state = '';
               }
               else if (vm.searchOption == '4') {
                   vm.query.franchiseeId = 0;
                   vm.query.propertyType = '';
                   vm.query.name = '';
                   vm.query.url = '',
                   vm.query.pageNumber = 1;
                   vm.query.zipCode = ''
                   vm.query.street = '';
                   vm.query.convertedLead = null;
                   vm.query.city = '';
                   vm.query.state = '';
               }
               else if (vm.searchOption == '2') {
                   vm.query.franchiseeId = 0;
                   vm.query.text = '';
                   vm.query.convertedLead = null;
                   vm.query.url = '',
                   vm.query.name = '';
                   vm.query.zipCode = '';
                   vm.query.street = '';
                   vm.query.pageNumber = 1;
                   vm.query.city = '';
                   vm.query.state = '';
               }
               else if (vm.searchOption == '3') {
                   vm.query.franchiseeId = 0;
                   vm.query.text = '';
                   vm.query.propertyType = '';
                   vm.query.url = '',
                   vm.query.name = '';
                   vm.query.zipCode = '';
                   vm.query.street = '';
                   vm.query.pageNumber = 1;
                   vm.query.city = '';
                   vm.query.state = '';
               }
               else if (vm.searchOption == '6') {
                   vm.query.franchiseeId = 0;
                   vm.query.convertedLead = null;
                   vm.query.text = '';
                   vm.query.propertyType = '';
                   vm.query.name = '';
                   vm.query.pageNumber = 1;
                   vm.query.zipCode = '';
                   vm.query.street = '';
                   vm.query.city = '';
                   vm.query.state = '';
               }
               else if (vm.searchOption == '7') {
                   vm.query.franchiseeId = 0;
                   vm.query.convertedLead = null;
                   vm.query.text = '';
                   vm.query.propertyType = '';
                   vm.query.name = '';
                   vm.query.pageNumber = 1;
                   vm.query.zipCode = '';
                   vm.query.city = '';
                   vm.query.state = '';
               }
               else if (vm.searchOption == '8') {
                   vm.query.franchiseeId = 0;
                   vm.query.convertedLead = null;
                   vm.query.text = '';
                   vm.query.propertyType = '';
                   vm.query.name = '';
                   vm.query.pageNumber = 1;
                   vm.query.street = '';
                   vm.query.city = '';
                   vm.query.state = '';
               }
               else if (vm.searchOption == '9') {
                   vm.query.franchiseeId = 0;
                   vm.query.convertedLead = null;
                   vm.query.text = '';
                   vm.query.propertyType = '';
                   vm.query.name = '';
                   vm.query.pageNumber = 1;
                   vm.query.street = '';
                   vm.query.zipCode = '';
                   vm.query.state = '';
               }
               else if (vm.searchOption == '10') {
                   vm.query.franchiseeId = 0;
                   vm.query.convertedLead = null;
                   vm.query.text = '';
                   vm.query.propertyType = '';
                   vm.query.name = '';
                   vm.query.pageNumber = 1;
                   vm.query.street = '';
                   vm.query.zipCode = '';
                   vm.query.city = '';
               }
               else {
                   vm.query.text = '';
                   vm.query.url = '',
                   vm.query.franchiseeId = 0;
                   vm.query.propertyType = '';
                   vm.query.convertedLead = null;
                   vm.query.pageNumber = 1;
                   vm.query.zipCode = '';
                   vm.query.street = '';
               }
           }

           function resetSearch() {
               vm.query.text = '';
               vm.query.name = '';
               vm.query.franchiseeId = 0;
               vm.query.convertedLead = null;
               vm.query.propertyType = '';
               vm.query.startDate = null;
               vm.query.endDate = null;
               vm.searchOption = '';
               vm.query.url = '',
               $scope.$broadcast("reset-dates");
               vm.query.pageNumber = 1;
               vm.query.zipCode = '';
               vm.query.street = '';
               vm.query.city = '';
               vm.query.state = '';
           }

           $scope.$on('clearDates', function (event) {
               vm.query.startDate = null;
               vm.query.endDate = null;
               getWebLeads();
           });

           function getWebLeads() {
               return webLeadService.getWebLeads(vm.query).then(function (result) {
                   if (result != null && result.data != null) {
                       vm.ids = [];
                       vm.webLeadList = result.data.collection;
                       angular.forEach(vm.webLeadList, function (value, index) {
                           value.fullAddress1 = [value.streetAddress, value.suiteNumber, value.city, value.province, value.country, value.zip].filter(Boolean).join(", ");
                       })
                       vm.count = result.data.pagingModel.totalRecords;
                       vm.query.sort.order = result.data.filter.sortingOrder;
                       addResultToList(vm.webLeadList);
                   }
               });
           }

           function addResultToList() {
               angular.forEach(vm.webLeadList, function (value, key) {
                   vm.ids.push(value.id);
               })
           }

           function sorting(propName) {
               vm.query.sort.propName = propName;
               vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
               getWebLeads();
           };

           function pageChange() {
               getWebLeads();
           };

           function getFranchiseeCollection() {
               return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                   vm.franchiseeCollection = result.data;
               });
           }

           function getPropertyTypes() {
               return webLeadService.getMarketingClass().then(function (result) {
                   vm.propertyTypeCollection = result.data;
               });
           }

           $scope.$emit("update-title", "Web Leads Report");
           $q.all([getWebLeads(), prepareSearchOptions(), getFranchiseeCollection(), getPropertyTypes(), prepareLeadOptions()]);

       }]);
}());