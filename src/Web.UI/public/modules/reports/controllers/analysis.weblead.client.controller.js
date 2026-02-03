(function () {
    'use strict';

    var SortColumns = {
        ID: 'ID',
        Franchisee: 'Franchisee',
        URL: 'URL',
        Count: "Count"
    };

    angular.module(ReportsConfiguration.moduleName).controller("WebLeadAnalysisController",
     ["$state", "$stateParams", "$q", "$scope", "WebLeadService", "APP_CONFIG", "$rootScope", "FileService", "FranchiseeService", "$uibModal", "Clock",
         "DashboardService", "FranchiseeGrowthReportService", "URLAuthenticationServiceForEncryption",
    function ($state, $stateParams, $q, $scope, webLeadService, config, $rootScope, fileService, franchiseeService, $uibModal, clock,
        dashboardService, franchiseeGrowthReportService, URLAuthenticationServiceForEncryption) {

        var vm = this;
        vm.query = {
            pageNumber: 1,
            franchiseeId: 0,
            year: 0,
            month: "0",
            pageSize: config.defaultPageSize,
            text: '',
            sort: { order: 0, propName: '' }
        };
        vm.currentDate = clock.now();
        vm.getwebLeadReport = getwebLeadReport;
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
        vm.downloadwebLeadReport = downloadwebLeadReport;
        vm.ids = [];

        vm.editFranchisee = editFranchisee;
        function editFranchisee(franchiseeId) {
            franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
            $state.go("core.layout.franchisee.edit", { id: franchiseeId });
        }

        function resetSearch() {
            vm.query.text = '';
            vm.query.franchiseeId = 0;
            vm.query.year = 0;
            vm.query.month = "0";
            vm.query.pageNumber = 1;
            getwebLeadReport();
        }

        function downloadwebLeadReport() {
            vm.downloading = true;
            return webLeadService.downloadWebLeadReport(vm.query).then(function (result) {
                var fileName = "webLeadReport.xlsx";
                fileService.downloadFile(result.data, fileName);
                vm.downloading = false;
            }, function () { vm.downloading = false; });
        }
        function refresh() {
            getwebLeadReport();
        }

        function getwebLeadReport() {
            vm.query.sortingColumn = vm.query.sort.propName;
            vm.query.sortingOrder = vm.query.sort.order;

            if (vm.query.year <= 0) {
                vm.query.year = clock.getYear(vm.currentDate);
            }
            if (vm.query.month == null) {
                if (vm.query.year == clock.getYear(vm.currentDate)) {
                    vm.query.month = clock.getMonth(vm.currentDate) + "";
                }
            }

            return webLeadService.getwebLeadReport(vm.query).then(function (result) {
                if (result != null && result.data != null) {
                    vm.ids = [];
                    vm.webLeadReportList = result.data.collection;
                    vm.count = result.data.pagingModel.totalRecords;
                    vm.query.sort.order = result.data.filter.sortingOrder;
                    addResultToList(vm.webLeadReportList);
                }
            });
        }

        function addResultToList() {
            angular.forEach(vm.webLeadReportList, function (value, key) {
                vm.ids.push(value.id);
            })
        }

        function sorting(propName) {
            vm.query.sort.propName = propName;
            vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
            getwebLeadReport();
        };

        function pageChange() {
            getwebLeadReport();
        };

        function getFranchiseeCollection() {
            return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                vm.franchiseeCollection = result.data;
                vm.franchiseeCollection.push({ display: "All", value: "0" });
                vm.franchiseeCollection.push({ display: "UnAssigned", value: null });
            });
        }

        function getYearsForGrowthReport() {
            return franchiseeGrowthReportService.getYearsForGrowthReport().then(function (result) {
                vm.yearCollection = result.data;
            })
        }

        function getMonthCollection() {
            return dashboardService.getMonthNames().then(function (result) {
                vm.monthCollection = result.data;
                vm.monthCollection.push({ display: "All", value: "0" });
            })
        }

        $scope.$emit("update-title", "Web Lead Analysis");
        $q.all([getwebLeadReport(), getFranchiseeCollection(), getYearsForGrowthReport(), getMonthCollection()]);

    }]);
}());