(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).controller('adminController',
        ["$state", "$scope", "$rootScope", "$q", "DashboardService", "Clock", "FranchiseeService",
            "APP_CONFIG", "CustomerEmailReportService", "$filter", "$compile", "URLAuthenticationServiceForEncryption",
            function ($state, $scope, $rootScope, $q, dashboardService, clock, franchiseeService, config, customerEmailReportService, $filter, $compile
                , URLAuthenticationServiceForEncryption) {
                var vm = this;
                vm.searchText = "";
                vm.query = {
                    franchiseeId: 0,
                    startDate: null,
                    endDate: null,
                    rangeStartDate: null,
                    rangeEndDate: null,
                    year: clock.getYear(clock.now()),
                    month: null
                };
                vm.searchTextDuplicate = "";
                vm.isFromLoad = true;
                var totalIncome = 0;

                vm.salesSummary = salesSummary;
                vm.invoiceList = invoiceList;

                vm.franchisees = [];
                vm.salesSummaryList = [];
                vm.recentInvoiceList = [];
                vm.franchiseeLeaderboardList = [];
                vm.revenue = [];
                vm.serviceRevenue = [];

                vm.currencyRate = '';
                vm.currencyCode = '';
                $scope.amChartOptions = dashboardService.getPieChartOptions();
                $scope.data = vm.revenue;

                vm.franchiseeId = $rootScope.identity.organizationId;
                vm.getRevenue = getRevenue;
                vm.getFranchiseeDirectoryList = getFranchiseeDirectoryList;
                vm.getSalesSummary = getSalesSummary;
                vm.getRecentInvoices = getRecentInvoices;
                vm.getFilteredData = getFilteredData;
                vm.getCustomerCount = getCustomerCount;
                vm.customerCount = {};
                vm.resetFranchisee = resetFranchisee;
                vm.getFilteredList = getFilteredList;
                vm.resetSearch = resetSearch;
                vm.showClassChart = true;
                vm.showServiceChart = false;
                vm.getClassChart = getClassChart;
                vm.getServiceChart = getServiceChart;
                vm.getRevenueForService = getRevenueForService;
                vm.getFranchiseeDirectoryListForSuperAdmin = getFranchiseeDirectoryListForSuperAdmin;
                vm.revenueLength = 1;
                vm.serviceRadioButton = 1;
                vm.editFranchisee = editFranchisee;
                vm.classRadioButtion = true;

                vm.generateChartData = generateChartData;
                vm.report = [];
                $scope.amChartOptionsforList = customerEmailReportService.getLineChartOptions();
                $scope.amChartOptionsforList.data = vm.report;
                vm.franchiseesIdforEmailChart = null;
                vm.franchiseesNameforEmailChart = "";

                vm.generateChartDataForreview = generateChartDataForreview;
                vm.reportForreview = [];
                $scope.amChartOptionsforListForreview = customerEmailReportService.getLineChartOptionsForReview();
                $scope.amChartOptionsforListForreview.data = vm.reportForreview;
                vm.franchiseesIdforEmailChartForreview = null;
                vm.franchiseesNameforEmailChartForreview = "";

                vm.getReviewCount = getReviewCount;

                function getClassChart() {
                    if (!vm.showClassChart) {
                        vm.showClassChart = true;
                        vm.showServiceChart = false;
                        getRevenue();
                    }
                }
                function getServiceChart() {
                    if (!vm.showServiceChart) {
                        vm.showClassChart = false;
                        vm.showServiceChart = true;
                        getRevenueForService();
                    }
                }

                function resetSearch() {
                    vm.query.year = clock.getYear(clock.now());
                    vm.query.month = null;
                    vm.franchiseeId = 0;
                    vm.query.franchiseeId = 0;
                    vm.query.startDate = null;
                    vm.query.endDate = null;
                    vm.query.rangeStartDate = null;
                    vm.query.rangeEndDate = null;
                    $scope.$broadcast("reset-dates");
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.startDate = null;
                    vm.query.endDate = null;
                    vm.query.rangeStartDate = null;
                    vm.query.rangeEndDate = null;
                    if (!vm.ClearRangeOnly) {
                        getCustomerCount();
                        getSalesSummary();
                        getRecentInvoices();
                        getFranchiseeLeaderboard();
                        if (vm.showServiceChart) {
                            getRevenueForService();
                        }
                        else
                            getRevenue();
                    }
                    vm.ClearRangeOnly = false;
                });

                function resetFranchisee(franchiseeId) {
                    vm.franchiseeId = franchiseeId;
                    vm.query.franchiseeId = franchiseeId;

                    getCustomerCount();
                    getSalesSummary();
                    getRecentInvoices();
                    getFranchiseeLeaderboard();
                    if (vm.showServiceChart) {
                        getRevenueForService();
                    }
                    else
                        getRevenue();
                }

                function getCustomerCount() {
                    if (vm.query.startDate === null || vm.query.endDate == null) {
                        vm.query.endDate = clock.now();
                        vm.query.startDate = clock.getStartDateOfYear();
                    }
                    return dashboardService.getCustomerCount(vm.query.startDate, vm.query.endDate, vm.query.franchiseeId).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.customerCount = result.data;
                        }
                    });
                }

                function getFilteredList() {
                    vm.query.startDate = null;
                    vm.query.endDate = null;
                    vm.query.month = null;
                    vm.query.year = clock.getYear(clock.now());

                    vm.query.startDate = vm.query.rangeStartDate;
                    vm.query.endDate = vm.query.rangeEndDate;
                    getCustomerCount();
                    getFranchiseeLeaderboard();
                    if (vm.showServiceChart) {
                        getRevenueForService();
                    }
                    else
                        getRevenue();
                }
                function getFilteredData() {
                    vm.ClearRangeOnly = true;
                    $scope.$broadcast("reset-dates");
                    var currentYear = clock.getYear(clock.now());
                    if (vm.query.year != null && vm.query.month == null) {
                        vm.query.startDate = "01/01/" + vm.query.year;
                        vm.query.endDate = "12/31/" + vm.query.year;
                    }
                    if (vm.query.year == null && vm.query.month == null) {
                        vm.query.endDate = clock.now();
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

                    getCustomerCount();
                    getFranchiseeLeaderboard();
                    if (vm.showServiceChart) {
                        getRevenueForService();
                    }
                    else
                        getRevenue();
                }

                function getStartDate(option) {
                    if (option == "month") {
                        return clock.getStartDateOfMonth();
                    }
                    else if (option == "ytd") {
                        return clock.getStartDateOfYear();
                    }
                    else {
                        return clock.getStartDateOfWeek();
                    }
                }

                function getFranchiseeDirectoryList() {
                    return dashboardService.getFranchiseeDirectoryList().then(function (result) {
                        if (result != null && result.data != null) {
                            vm.franchisees = result.data.collection;
                        }
                    });
                }

                function getSalesSummary() {
                    return dashboardService.getSalesSummary(vm.franchiseeId).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.salesSummaryList = result.data.collection;
                        }
                    });
                }
                //var chart=
                function getRecentInvoices() {
                    return dashboardService.getRecentInvoices(vm.franchiseeId).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.recentInvoiceList = result.data.collection;
                        }
                    });
                }

                function getFranchiseeLeaderboard() {
                    if (vm.query.startDate == null || vm.query.endDate == null) {
                        vm.query.endDate = clock.now();
                        vm.query.startDate = clock.getStartDateOfYear();
                    }
                    return dashboardService.getFranchiseeLeaderboard(vm.query.startDate, vm.query.endDate, vm.franchiseeId).then(function (result) {
                        if (result != null && result.data != null)
                            vm.franchiseeLeaderboardList = result.data;
                        if (vm.franchiseeLeaderboardList.length > 0) {
                            vm.currencyRate = vm.franchiseeLeaderboardList[0].currencyRate;
                            vm.currencyCode = vm.franchiseeLeaderboardList[0].currencyCode;
                        }
                    });
                }

                function getRevenue() {
                    if (vm.query.startDate == null || vm.query.endDate == null) {
                        vm.query.endDate = clock.now();
                        vm.query.startDate = clock.getStartDateOfYear();
                    }

                    return dashboardService.getRevenueDetails(vm.query).then(function (result) {

                        var totalIncome = 0;
                        if (result != null && result.data != null) {
                            vm.revenue = result.data;
                            if (vm.revenue.length > 0) {
                                vm.revenueLength = vm.revenue.length;
                                var totalValue = $filter('filter')(vm.revenue, { category: "Total" }, true);
                                var index = vm.revenue.indexOf(totalValue);
                                vm.revenue.splice(index, 1);
                                var el = document.getElementById('legenddiv2');
                                totalIncome = totalValue[0].income;
                            }
                            else {
                                vm.revenueLength = 0;
                            }

                        }
                        angular.element(el).html('');
                        $scope.$broadcast('amCharts.updateData', vm.revenue, 'myFirstChart');
                        var html = '<g cursor="pointer" transform="translate(0,0)"><path cs="100,100" d="M-7.5,8.5 L8.5,8.5 L8.5,-7.5 L-7.5,-7.5 Z" fill="#e35b5a" stroke="#e35b5a" fill-opacity="1" stroke-width="1" stroke-opacity="1" transform="translate(8,8)" class="amcharts-legend-marker"></path><g transform="translate(8,8)" visibility="hidden" class="amcharts-legend-switch"><path cs="100,100" d="M-5.5,-5.5 L6.5,6.5" fill="none" stroke="#FFFFFF" stroke-width="3"></path><path cs="100,100" d="M-5.5,6.5 L6.5,-5.5" fill="none" stroke="#FFFFFF" stroke-width="3"></path></g><text y="6" fill="#000000" font-family="Verdana" font-size="11" opacity="1" text-anchor="start" class="amcharts-legend-label" transform="translate(21,7)"><tspan y="6" x="0" style="font-size: 16px;font-family:Verdana"><b style="margin-left:32%">    Total:</b>     </tspan></text><text y="6" fill="#000000" font-family="Verdana" font-size="16" opacity="1" text-anchor="end" class="amcharts-legend-value" transform="translate(269,7)" style=font-size:16px;> <span ng-if="vm.currencyRate != null" currency-Exchange conversion-Rate="' + vm.currencyRate + '" input="' + totalIncome + '" from-Currency="' + vm.currencyCode + '"> </span></text><rect x="16" y="0" width="252.5" height="18" rx="0" ry="0" stroke-width="0" stroke="none" fill="#fff" fill-opacity="0.005"></rect></g>';
                        angular.element(el).append($compile(html)($scope));
                    });
                }

                function getRevenueForService() {
                    if (vm.query.startDate == null || vm.query.endDate == null) {
                        vm.query.endDate = clock.now();
                        vm.query.startDate = clock.getStartDateOfYear();
                    }

                    return dashboardService.getRevenueForService(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            var totalIncome = 0;
                            vm.serviceRevenue = result.data;
                            if (vm.serviceRevenue.length > 0) {
                                var totalValue = $filter('filter')(vm.serviceRevenue, { category: "Total" }, true);
                                var index = vm.serviceRevenue.indexOf(totalValue);
                                vm.serviceRevenue.splice(index, 1);
                                var el = document.getElementById('legenddiv2');
                                totalIncome = totalValue[0].income;
                            }
                        }
                        angular.element(el).html('');
                        $scope.$broadcast('amCharts.updateData', vm.serviceRevenue, 'myFirstChart');
                        var html = '<g cursor="pointer" transform="translate(0,0)"><path cs="100,100" d="M-7.5,8.5 L8.5,8.5 L8.5,-7.5 L-7.5,-7.5 Z" fill="#e35b5a" stroke="#e35b5a" fill-opacity="1" stroke-width="1" stroke-opacity="1" transform="translate(8,8)" class="amcharts-legend-marker"></path><g transform="translate(8,8)" visibility="hidden" class="amcharts-legend-switch"><path cs="100,100" d="M-5.5,-5.5 L6.5,6.5" fill="none" stroke="#FFFFFF" stroke-width="3"></path><path cs="100,100" d="M-5.5,6.5 L6.5,-5.5" fill="none" stroke="#FFFFFF" stroke-width="3"></path></g><text y="6" fill="#000000" font-family="Verdana" font-size="11" opacity="1" text-anchor="start" class="amcharts-legend-label" transform="translate(21,7)"><tspan y="6" x="0" style="font-size: 16px;font-family:Verdana"><b style="margin-left:32%">    Total:</b>     </tspan></text><text y="6" fill="#000000" font-family="Verdana" font-size="16" opacity="1" text-anchor="end" class="amcharts-legend-value" transform="translate(269,7)" style=font-size:16px;> <span ng-if="vm.currencyRate != null" currency-Exchange conversion-Rate="' + vm.currencyRate + '" input="' + totalIncome + '" from-Currency="' + vm.currencyCode + '"> </span></text><rect x="16" y="0" width="252.5" height="18" rx="0" ry="0" stroke-width="0" stroke="none" fill="#fff" fill-opacity="0.005"></rect></g>';
                        angular.element(el).append($compile(html)($scope));
                    });
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getActiveFranchiseeList().then(function (result) {
                        vm.franchiseeCollection = result.data;
                        vm.franchiseeCollection.push({ display: "All", value: "0" });

                        vm.franchiseeCollectionForEmailChart = vm.franchiseeCollection;
                        vm.franchiseeCollectionForEmailChart.pop();
                        angular.forEach(vm.franchiseeCollectionForEmailChart, function (value1) {
                            if (value1.id == 62) {
                                vm.franchiseesNameforEmailChart = value1.display;
                            }
                            else {
                                vm.franchiseesNameforEmailChart = vm.franchiseeCollectionForEmailChart[43].display;
                                vm.franchiseesNameforEmailChartForreview = vm.franchiseeCollectionForEmailChart[43].display;
                            }
                        });
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
                function getDocuments() {
                    return dashboardService.getDocuments(vm.franchiseeId).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.recentInvoiceLists = result.data.collection;
                        }
                    });
                }
                function salesSummary() {
                    $state.go("core.layout.sales.batch.list");
                }

                function invoiceList() {
                    $state.go("core.layout.sales.invoice");
                }
                function getFranchiseeDirectoryListForSuperAdmin() {

                    if (vm.searchText == "") {
                        vm.searchTextDuplicate = "INVALID";
                    }
                    else {
                        vm.searchTextDuplicate = vm.searchText;
                    }
                    return dashboardService.getFranchiseeDirectoryListForSuperAdmin(vm.searchTextDuplicate).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.franchisees = result.data.collection;
                            if (vm.searchText == "INVALID") {
                                vm.searchText = "";
                            }
                        }
                    });
                }

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }

                function generateChartData(frId) {

                    var currentYear = clock.getYear(clock.now());
                    if (vm.query.year == null || vm.query.year == currentYear) {
                        vm.query.endDate = clock.now();
                        vm.query.startDate = clock.getStartDateOfYear();
                    }
                    else {
                        vm.query.startDate = "01/01/" + vm.query.year;
                        vm.query.endDate = "12/31/" + vm.query.year;
                    }
                    vm.franchiseeId = 62;
                    vm.franchiseesIdforEmailChart = "62";

                    if (frId != null && frId != undefined && vm.franchiseeCollectionForEmailChart != null) {
                        angular.forEach(vm.franchiseeCollectionForEmailChart, function (value1) {
                            vm.franchiseeId = Number(frId);
                            vm.franchiseesIdforEmailChart = frId;
                            if (value1.id == Number(frId)) {
                                vm.franchiseesNameforEmailChart = value1.display
                            }
                        });
                    }

                    return customerEmailReportService.getEmailReportChartData(vm.franchiseeId, vm.query.startDate, vm.query.endDate).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.report = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.report, 'myFirstChart1');
                        }
                    });
                }

                function generateChartDataForreview(frId) {

                    var currentYear = clock.getYear(clock.now());
                    if (vm.query.year == null || vm.query.year == currentYear) {
                        vm.query.endDate = clock.now();
                        vm.query.startDate = clock.getStartDateOfYear();
                    }
                    else {
                        vm.query.startDate = "01/01/" + vm.query.year;
                        vm.query.endDate = "12/31/" + vm.query.year;
                    }
                    vm.franchiseeId = 62;
                    vm.franchiseesIdforEmailChartForreview = "62";

                    if (frId != null && frId != undefined && vm.franchiseeCollectionForEmailChart != null) {
                        angular.forEach(vm.franchiseeCollectionForEmailChart, function (value1) {
                            vm.franchiseeId = Number(frId);
                            vm.franchiseesIdforEmailChartForreview = frId;
                            if (value1.id == Number(frId)) {
                                vm.franchiseesNameforEmailChartForreview = value1.display
                            }
                        });
                    }

                    vm.getReviewCount(vm.franchiseeId);

                    return customerEmailReportService.getReviewReportChartData(vm.franchiseeId, vm.query.startDate, vm.query.endDate).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.reportForreview = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.reportForreview, 'myFirstChart3');
                        }
                    });
                }

                function getReviewCount(franchiseeId) {
                    if (franchiseeId == undefined || franchiseeId == null) {
                        franchiseeId = 62;
                    }
                    return customerEmailReportService.getReviewCount(franchiseeId).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.reviewCount = result.data;
                        }
                    });
                }

                $scope.$emit("update-title", "Dashboard");

                $q.all([getSalesSummary(), getFranchiseeDirectoryListForSuperAdmin(), getRecentInvoices(), getFranchiseeLeaderboard(), getRevenue(),
                    getFranchiseeCollection(), getLastTwentyYearCollection(), getCustomerCount(), getMonthCollection(), getDocuments(), generateChartData(), generateChartDataForreview(), getReviewCount()]);
            }]);
})();