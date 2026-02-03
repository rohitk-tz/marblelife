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
        RoyalityJobs: 'RoyalityJobs',
        MissedCalls: 'MissedCalls',
        LostEstimate: 'LostEstimate',
        LostJobs: 'LostJobs',
        TotalJobs: 'TotalJobs',
        TotalCalls: 'TotalCalls'
    };

    angular.module(SalesConfiguration.moduleName).controller("SalesMacroFunnelLocalController",
        ["$scope", "$rootScope", "$state", "$stateParams", "$q", "APP_CONFIG", "FranchiseeService", "SalesService", "$uibModal", "FileService",'$filter',
    function ($scope, $rootScope, $state, $stateParams, $q, config, franchiseeService, salesService, $uibModal, fileService, $filter) {

        var vm = this;
        vm.query = {
            franchiseeId: 0,
            periodStartDate: null,
            periodEndDate: null,
            propName: '',
            order: 0
        };
        vm.currencyCode = $rootScope.identity.currencyCode;
        vm.query.franchiseeId = "62";
        vm.Roles = DataHelper.Role;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
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
        vm.seasonalLeadReport = [];
        $scope.amChartOptionsforSalesFunnelLocalGraph = salesService.getSalesFunnelLocalChartData();
        $scope.amChartOptionsforSalesFunnelLocalGraph.data = vm.seasonalLeadReport;
        vm.generateSalesFunnelLocalGraphData = generateSalesFunnelLocalGraphData;
        vm.franchiseeSelected = franchiseeSelected;

        function generateSalesFunnelLocalGraphData() {
            vm.isvisible = false;
            vm.isProcessing = true;
            return salesService.generateSalesFunnelLocalGraphData(vm.query).then(function (result) {
                if (result != null && result.data != null) {
                    if (result.data.chartData != null) {
                        vm.seasonalLeadReport = result.data.chartData;
                        $scope.$broadcast('amCharts.updateData', vm.seasonalLeadReport, 'myFirstChart6');
                        vm.isProcessing = false;
                    }
                    else
                    {
                        vm.seasonalLeadReport = null;
                        $scope.$broadcast('amCharts.updateData', vm.seasonalLeadReport, 'myFirstChart6');
                        vm.isProcessing = false;
                    }
                }
            }).catch(function (err) {
                vm.isProcessing = false;
            });;
        }
        function prepareSearchOptions() {
            vm.searchOptions.push({ display: 'Franchisee', value: '1' });
        }

        function downloadAnnualData() {
            vm.downloading = true;
            return salesService.downloadMocroFunnelLocal(vm.query).then(function (result) {
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
            vm.query.franchiseeId = "62";
            vm.query.periodStartDate = null;
            vm.query.periodEndDate = null;
            $scope.$broadcast("reset-dates");
            vm.searchOption = '0'
            getRecords();
            generateSalesFunnelLocalGraphData();
        }

        function getFranchiseeCollections() {
            return franchiseeService.getActiveFranchiseeListWithOut0ML().then(function (result) {
                vm.franchiseeCollection = result.data;
            });
        }
        function franchiseeSelected()
        {
            getRecords();
            generateSalesFunnelLocalGraphData();
        }
        function refresh() {
            getRecords();
            generateSalesFunnelLocalGraphData();
        }

        function getRecords() {
            vm.isvisible = false;
            vm.isProcessing = true;

            return salesService.getSalesFunnelLocalData(vm.query).then(function (result) {
                if (result != null && result.data != null) {
                    vm.invoiceIds = [];
                    vm.salesFunnelNationalList = result.data.localCollection;
                    if (vm.salesFunnelNationalList != null) {
                        vm.query.franchiseeId = vm.salesFunnelNationalList[0].franchiseeId.toString();
                        vm.startDate = result.data.startDate;
                        vm.endDate = result.data.endDate;
                        vm.isvisible = true;
                        vm.salesFunnelNationalBestFranchisee = result.data.bestFranchiseeCollection;
                        var franchisee = $filter('filter')(vm.franchiseeCollection, { value: vm.query.franchiseeId.toString() }, true);
                        vm.FranchiseeName = franchisee[0].display;
                        $scope.$emit("update-title", "Macro Sales Funnel :" + vm.FranchiseeName);
                        vm.isProcessing = false;
                    }
                }
            }).catch(function (err) {
                vm.isProcessing = false;
            });
        }


        function sorting(propName) {
            vm.query.propName = propName;
            vm.query.order = (vm.query.order == 0) ? 1 : 0;
            getRecords();
        };

        $scope.$emit("update-title", "Macro Sales Funnel");

        $q.all([getRecords(), getFranchiseeCollections(), prepareSearchOptions(), generateSalesFunnelLocalGraphData()]);

    }]);
}());