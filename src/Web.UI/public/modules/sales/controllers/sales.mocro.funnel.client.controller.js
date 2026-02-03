(function () {
    'use strict';

    var SortColumns = {
        FranchiseeName: 'FranchiseeName',
        TechCount: 'TechCount',
        SalesCount: 'SalesCount',
        WebCount: 'WebCount',
        PhoneLeads: 'PhoneLeads',
        PhoneLeadsOverTwoMIn: 'PhoneLeadsOverTwoMIn',
        EstimateCount: 'EstimateCount',
        JobCount: 'JobCount',
        Sales: 'Sales',
        AveTicket: 'AveTicket',
        SalesPerTech: 'SalesPerTech',
        SalesPerTechPermonths: 'SalesPerTechPermonths',
        PhoneAnsweredOverTwoMinPerPhone: 'PhoneAnsweredOverTwoMinPerPhone',
        EstimatePer: 'EstimatePer',
        JobPer: 'JobPer',
        ConvertToInvoice: 'ConvertToInvoice',
        SalesCloseRate: 'SalesCloseRate',
        phoneAnswerPer: 'phoneAnswerPer',
        RoyalityJobs: 'RoyalityJobs'
    };

    angular.module(SalesConfiguration.moduleName).controller("SalesMacroFunnelNationalController",
        ["$scope", "$rootScope", "$state", "$stateParams", "$q", "APP_CONFIG", "FranchiseeService", "SalesService", "$uibModal", "FileService",
    function ($scope, $rootScope, $state, $stateParams, $q, config, franchiseeService, salesService, $uibModal, fileService) {

        var vm = this;
        vm.query = {
            franchiseeId: 0,
            periodStartDate: null,
            periodEndDate: null,
            propName: '',
            order: 0
        };
        vm.currencyCode = $rootScope.identity.currencyCode;
        vm.query.franchiseeId = $stateParams.franchiseeId == null ? 0 : $stateParams.franchiseeId;
        vm.Roles = DataHelper.Role;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
        vm.isFrontOffice = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
        vm.count = 0;
        vm.pagingOptions = config.pagingOptions;
        vm.sorting = sorting;
        vm.SortColumns = SortColumns;
        vm.resetSearch = resetSearch;
        vm.refresh = refresh;
        vm.searchOptions = [];
        vm.statusOptions = []
        vm.searchOption = '';
        vm.resetSeachOption = resetSeachOption;
        vm.currentRole = $rootScope.identity.roleId;
        vm.getRecords = getRecords;
        vm.invoiceIds = [];
        vm.downloadAnnualData = downloadAnnualData;
        vm.isvisible = false;
        function prepareSearchOptions() {
            vm.searchOptions.push({ display: 'Franchisee', value: '1' });
        }

        function downloadAnnualData() {
            vm.downloading = true;
            return salesService.downloadMocroFunnelNational(vm.query).then(function (result) {
                var fileName = "MicroFunnelNational.xlsx";
                fileService.downloadFile(result.data, fileName);
                vm.downloading = false;

            }, function () { vm.downloading = false; });
        }

        $scope.$on('clearDates', function (event) {
            vm.query.periodStartDate = null;
            vm.query.periodEndDate = null;
            getRecords();
        });

        function resetSeachOption() {
            vm.query.franchiseeId = 0;
            vm.query.periodStartDate = null;
            vm.query.periodEndDate = null;
        }

        function resetSearch() {
            vm.query.franchiseeId = 0;
            vm.query.periodStartDate = null;
            vm.query.periodEndDate = null;
            $scope.$broadcast("reset-dates");
            vm.searchOption = '0'
            getRecords();
        }

        function getFranchiseeCollection() {
            return franchiseeService.getActiveFranchiseeListWithOut0ML().then(function (result) {
                vm.franchiseeCollection = result.data;
            });
        }
        function refresh() {
            getRecords();
        }

        function getRecords() {
            vm.isvisible = false;
            vm.isProcessing = true;
            return salesService.getSalesFunnelNationalData(vm.query).then(function (result) {
                if (result != null && result.data != null) {
                    vm.invoiceIds = [];
                    vm.salesFunnelNationalList = result.data.collection;
                    vm.startDate = result.data.startDate;
                    vm.endDate = result.data.endDate;
                    vm.isvisible = true;
                    vm.salesFunnelNationalBestFranchisee = result.data.bestFranchiseeCollection;
                    vm.isProcessing = false;
                }
            }).catch(function (err) {
                vm.isProcessing = false;
            });;
        }


        function sorting(propName) {
            vm.query.propName = propName;
            vm.query.order = (vm.query.order == 0) ? 1 : 0;
            getRecords();
        };

        $scope.$emit("update-title", "Macro Sales Funnel");

        $q.all([getFranchiseeCollection(), getRecords(), prepareSearchOptions()]);

    }]);
}());