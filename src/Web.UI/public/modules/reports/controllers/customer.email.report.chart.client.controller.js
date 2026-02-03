(function () {
    'use strict';
    angular.module(ReportsConfiguration.moduleName).controller("EmailReportChartController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "CustomerEmailReportService", "modalParam", "Clock", "DashboardService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, customerEmailReportService, modalParam, clock, dashboardService) {

            var vm = this;
            vm.franchiseeId = modalParam.FranchiseeId;
            vm.currencyCode = $rootScope.identity.currencyCode;
            vm.generateChartData = generateChartData;

            vm.query = {
                franchiseeId: vm.franchiseeId,
                startDate: null,
                endDate: null,
                year: clock.getYear(clock.now()),
            };

            vm.close = function () {
                $uibModalInstance.dismiss();
            };
            vm.report = [];
            $scope.amChartOptions = customerEmailReportService.getLineChartOptions();
            $scope.amChartOptions.data = vm.report;

            function generateChartData() {

                var currentYear = clock.getYear(clock.now());
                if (vm.query.year == null || vm.query.year == currentYear) {
                    vm.query.endDate = clock.now();
                    vm.query.startDate = clock.getStartDateOfYear();
                }
                else {
                    vm.query.startDate = "01/01/" + vm.query.year;
                    vm.query.endDate = "12/31/" + vm.query.year;
                }

                return customerEmailReportService.getEmailReportChartData(vm.franchiseeId, vm.query.startDate, vm.query.endDate).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.report = result.data.chartData;
                        vm.franchisee = result.data.franchisee;
                        $scope.$broadcast('amCharts.updateData', vm.report);
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