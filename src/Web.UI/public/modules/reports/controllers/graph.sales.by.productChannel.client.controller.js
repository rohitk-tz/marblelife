(function () {
    'use strict';
    angular.module(ReportsConfiguration.moduleName).controller("ProductChannelChartController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "ProductReportService", "modalParam", "Clock", "DashboardService","$filter",
    function ($scope, $rootScope, $state, $q, $uibModalInstance, productReportService, modalParam, clock, dashboardService,$filter) {

            var vm = this;
            vm.currentDate = clock.now();
            vm.franchiseeId = modalParam.FranchiseeId;
            vm.currencyCode = $rootScope.identity.currencyCode;
            vm.generateChartData = generateChartData;
            vm.year = clock.getYear(clock.now()),
            vm.query = {
                franchiseeId: vm.franchiseeId,
                startDate: null,
                endDate: null
            };

            vm.close = function () {
                $uibModalInstance.dismiss();
            };
            vm.report = [];
            $scope.amChartOptions = productReportService.getLineChartOptions();
            //$scope.amChartOptions.data = vm.chartData;

            function generateChartData() {
                var currentYear = clock.getYear(clock.now());
                if (vm.query.startDate == null || vm.query.endDate == null) {
                    vm.query.endDate = clock.now();
                    vm.query.startDate = clock.getStartDateOfYear();
                    vm.query.endDate = moment(vm.query.endDate).format("MM/DD/YYYY");
                    vm.query.startDate = moment(vm.query.startDate).format("MM/DD/YYYY");
                }

                return productReportService.getChartData(vm.query).then(function (result) {
                    if (result != null && result.data != null) {
                        $scope.amChartOptions.graphs = result.data.graphs;
                        $scope.amChartOptions.data = result.data.chartData;
                        $scope.$broadcast('amCharts.renderChart', $scope.amChartOptions);
                        $scope.$broadcast('amCharts.updateData', $scope.amChartOptions.data);
                    }
                });
            }
            function getLastTwentyYearCollection() {
                return dashboardService.getLastTwentyYearCollection().then(function (result) {
                    vm.yearCollection = result.data;
                })
            }

            $q.all([generateChartData(), getLastTwentyYearCollection()]);

        }]);
}());