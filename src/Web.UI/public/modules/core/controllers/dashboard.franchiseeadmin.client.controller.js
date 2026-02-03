(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).controller('franchiseeAdminController',
        ["$state", "$scope", "$rootScope", "$q", "DashboardService", "Clock", "$uibModal", "FileService",
            "CustomerEmailReportService","$filter", "$compile",
            function DashboardController($state, $scope, $rootScope, $q, dashboardService, clock,
                $uibModal, fileService, customerEmailReportService, $filter, $compile) {
        var vm = this;
        vm.query = {
            franchiseeId: $rootScope.identity.organizationId,
            startDate: null,
            endDate: null,
            rangeStartDate: null,
            rangeEndDate: null,
            year: clock.getYear(clock.now()),
            month: null
        };
        vm.isTeamImageUploaded = $rootScope.identity.teamFileName!=""?true:false;
        vm.recentInvoiceList = [];
        vm.salesSummaryList = [];
        vm.getRecentInvoicesForFranchisee = getRecentInvoicesForFranchisee;

        vm.franchiseeId = $rootScope.identity.organizationId;
        vm.viewInvoice = viewInvoice;
        vm.documentUpload =documentUpload;
        vm.revenue = [];
        vm.serviceRevenue = [];
        vm.salesRepLeaderboardList = [];
        vm.startDate = clock.now();
        vm.endDate = clock.now();
        vm.salesSummary = salesSummary;
        vm.currencyCode = $rootScope.identity.currencyCode;
        vm.franchiseeName = $rootScope.identity.organizationName;
        $scope.amChartOptions = dashboardService.getPieChartOptions();
        $scope.data = vm.revenue;
        $scope.isUsClient = $rootScope.identity.currencyCode == "USD";

        vm.downloadSalesData = downloadSalesData;
        vm.getCustomerCount = getCustomerCount;
        vm.customerCount = {};
        vm.makePayment = makePayment;
        vm.getUnpaidInvoices = getUnpaidInvoices;
        vm.closeNotificationWindow = false;
        vm.closeNotification = closeNotification;
        vm.closeNotificationForDoc = false;
        vm.closeNotificationForDocWindow = closeNotificationForDocWindow;
        vm.getFilteredData = getFilteredData;
        vm.getFilteredList = getFilteredList;
        vm.resetSearch = resetSearch;
        vm.showClassChart = true;
        vm.showServiceChart = false;
        vm.getClassChart = getClassChart;
        vm.getServiceChart = getServiceChart;
        vm.getRevenueForService = getRevenueForService;
        vm.generateChartData = generateChartData;
        vm.getAnnualUploadResponse = getAnnualUploadResponse;
        vm.annualUpload = annualUpload;

        vm.report = [];
        $scope.amChartOptionsforList = customerEmailReportService.getLineChartOptions();
        $scope.amChartOptionsforList.data = vm.report;

        vm.generateChartDataForreview = generateChartDataForreview;
        vm.reportForreview = [];
        $scope.amChartOptionsforListForreview = customerEmailReportService.getLineChartOptionsForReview();
        $scope.amChartOptionsforListForreview.data = vm.reportForreview;

        vm.getReviewCount = getReviewCount;

        function documentUpload() {
            $state.go('core.layout.franchisee.document({ franchiseeId: ' + vm.franchiseeId + ' })');
        }
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
                    $scope.$broadcast('amCharts.updateData', vm.report, 'myFirstChart1');
                }
            });
        }

        function getClassChart() {
            if (!vm.showClassChart) {
                vm.showClassChart = true;
                vm.showServiceChart = false;
                getRevenueDetails();
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
                generateChartData();
                getCustomerCount();
                getSalesSummaryForFranchisee();
                getRecentInvoicesForFranchisee();
                getSalesRepLeaderboard();
                if (vm.showServiceChart) {
                    getRevenueForService();
                }
                else
                    getRevenueDetails();
            }
            vm.ClearRangeOnly = false;
        });

        function getFilteredList() {
            vm.query.startDate = null;
            vm.query.endDate = null;
            vm.query.month = null;
            vm.query.year = clock.getYear(clock.now());
            vm.query.startDate = vm.query.rangeStartDate;
            vm.query.endDate = vm.query.rangeEndDate;
            getCustomerCount();
            getSalesRepLeaderboard();
            if (vm.showServiceChart) {
                getRevenueForService();
            }
            else
                getRevenueDetails();
        }
        
        function closeNotificationForDocWindow() {
            vm.closeNotificationForDoc = true;
        }
        function closeNotification() {
            vm.closeNotificationWindow = true;
            vm.isTeamImageUploaded = true;
        }

        function getCustomerCount() {
            if (vm.query.startDate == null || vm.query.endDate == null) {
                vm.query.endDate = clock.now();
                vm.query.startDate = clock.getStartDateOfYear();
            }
            return dashboardService.getCustomerCount(vm.query.startDate, vm.query.endDate, vm.franchiseeId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.customerCount = result.data;
                }
            });
        }

        function makePayment(franchiseeId, invoiceId, franchiseeName, payableAmount, currencyRate, accountTypeId, currencyCode) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/sales/views/payment.client.view.html',
                controller: 'PaymentController',
                controllerAs: 'vm',
                backdrop: 'static',
                size: 'md',
                resolve: {
                    modalParam: function () {
                        return {
                            FranchiseeId: franchiseeId,
                            InvoiceId: invoiceId,
                            FranchiseeName: franchiseeName,
                            PayableAmount: payableAmount,
                            CurrencyRate: currencyRate,
                            AccountTypeId: accountTypeId,
                            CurrencyCode: currencyCode
                        };
                    }
                }
            });

            modalInstance.result.then(function () {
                getRecentInvoicesForFranchisee();
                getUnpaidInvoices();
            }, function () {

            });
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
            getSalesRepLeaderboard();
            if (vm.showServiceChart) {
                getRevenueForService();
            }
            else
                getRevenueDetails();
        }

        function getSalesRepLeaderboard() {
            if (vm.query.startDate == null || vm.query.endDate == null) {
                vm.query.endDate = clock.now();
                vm.query.startDate = clock.getStartDateOfYear();
            }
            return dashboardService.getSalesRepLeaderboard(vm.franchiseeId, vm.query.startDate, vm.query.endDate).then(function (result) {
                if (result != null)
                    vm.salesRepLeaderboardList = result.data;
            });
        }

        function downloadSalesData(fileId) {
            return fileService.getExcel(fileId).then(function (result) {
                var fileName = "SalesData-" + fileId + ".xlsx";
                fileService.downloadFile(result.data, fileName);
            });
        }

        function getRecentInvoicesForFranchisee() {
            return dashboardService.getRecentInvoices(vm.franchiseeId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.recentInvoiceList = result.data.collection;
                }
            });
        }

        function getUnpaidInvoices() {
            return dashboardService.getUnpaidInvoices(vm.franchiseeId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.unpaidInvoiceList = result.data.collection;
                }
            });
        }

        function getSalesSummaryForFranchisee() {
            return dashboardService.getSalesSummary(vm.franchiseeId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.salesSummaryList = result.data.collection;
                }
            });
        }

        function getPendingUploads() {
            return dashboardService.getPendindUploadList(vm.franchiseeId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.pendingUploads = result.data;
                }
            });
        }

        function viewInvoice(invoiceId) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/sales/views/franchisee-invoice-detail.client.view.html',
                controller: 'FranchiseeInvoiceDetailController',
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
        }

        function salesSummary() {
            $state.go("core.layout.sales.batch.list");
        }

        function getRevenueDetails() {
            if (vm.query.startDate == null || vm.query.endDate == null) {
                vm.query.endDate = clock.now();
                vm.query.startDate = clock.getStartDateOfYear();
            }

            return dashboardService.getRevenueDetails(vm.query).then(function (result) {
                if (result != null && result.data != null) {
                    vm.revenue = result.data;
                    if (vm.revenue.length > 0) {
                        vm.revenueLength = vm.revenue.length;
                        var totalValue = $filter('filter')(vm.revenue, { category: "Total" }, true);
                        var index = vm.revenue.indexOf(totalValue);
                        vm.revenue.splice(index, 1);
                        var el = document.getElementById('legenddiv2');
                        var totalIncome = totalValue[0].income;
                    }
                    else {
                        vm.revenueLength = 0;
                    }
                }
                angular.element(el).html('');
                $scope.$broadcast('amCharts.updateData', vm.revenue, 'myFirstChart');
                var html = '<g cursor="pointer" transform="translate(0,0)"><path cs="100,100" d="M-7.5,8.5 L8.5,8.5 L8.5,-7.5 L-7.5,-7.5 Z" fill="#e35b5a" stroke="#e35b5a" fill-opacity="1" stroke-width="1" stroke-opacity="1" transform="translate(8,8)" class="amcharts-legend-marker"></path><g transform="translate(8,8)" visibility="hidden" class="amcharts-legend-switch"><path cs="100,100" d="M-5.5,-5.5 L6.5,6.5" fill="none" stroke="#FFFFFF" stroke-width="3"></path><path cs="100,100" d="M-5.5,6.5 L6.5,-5.5" fill="none" stroke="#FFFFFF" stroke-width="3"></path></g><text y="6" fill="#000000" font-family="Verdana" font-size="11" opacity="1" text-anchor="start" class="amcharts-legend-label" transform="translate(21,7)"><tspan y="6" x="0" style="font-size: 16px;font-family:Verdana"><b style="margin-left:32%">    Total:</b>     </tspan></text><text y="6" fill="#000000" font-family="Verdana" font-size="16" opacity="1" text-anchor="end" class="amcharts-legend-value" transform="translate(269,7)" style=font-size:16px;> <span ng-if="vm.currencyRate != null" currency-Exchange conversion-Rate="' + vm.currencyRate + '" input="' + totalIncome + '" from-Currency="' + vm.currencyCode + '"> </span></text><rect x="16" y="0" width="252.5" height="18" rx="0" ry="0" stroke-width="0" stroke="none" fill="#fff" fill-opacity="0.005"></rect></g>';
                angular.element(el).append($compile(html)($scope))
            });
        }

        function getRevenueForService() {
            if (vm.query.startDate == null || vm.query.endDate == null) {
                vm.query.endDate = clock.now();
                vm.query.startDate = clock.getStartDateOfYear();
            }

            return dashboardService.getRevenueForService(vm.query).then(function (result) {
                
                if (result != null && result.data != null) {
                    vm.serviceRevenue = result.data;
                    if (vm.serviceRevenue.length > 0) {
                        var totalValue = $filter('filter')(vm.serviceRevenue, { category: "Total" }, true);
                        var index = vm.serviceRevenue.indexOf(totalValue);
                        vm.serviceRevenue.splice(index, 1);
                        var el = document.getElementById('legenddiv2');
                        var totalIncome = totalValue[0].income;
                    }
                }
                angular.element(el).html('');
                $scope.$broadcast('amCharts.updateData', vm.serviceRevenue, 'myFirstChart');
                var html = '<g cursor="pointer" transform="translate(0,0)"><path cs="100,100" d="M-7.5,8.5 L8.5,8.5 L8.5,-7.5 L-7.5,-7.5 Z" fill="#e35b5a" stroke="#e35b5a" fill-opacity="1" stroke-width="1" stroke-opacity="1" transform="translate(8,8)" class="amcharts-legend-marker"></path><g transform="translate(8,8)" visibility="hidden" class="amcharts-legend-switch"><path cs="100,100" d="M-5.5,-5.5 L6.5,6.5" fill="none" stroke="#FFFFFF" stroke-width="3"></path><path cs="100,100" d="M-5.5,6.5 L6.5,-5.5" fill="none" stroke="#FFFFFF" stroke-width="3"></path></g><text y="6" fill="#000000" font-family="Verdana" font-size="11" opacity="1" text-anchor="start" class="amcharts-legend-label" transform="translate(21,7)"><tspan y="6" x="0" style="font-size: 16px;font-family:Verdana"><b style="margin-left:32%">    Total:</b>     </tspan></text><text y="6" fill="#000000" font-family="Verdana" font-size="16" opacity="1" text-anchor="end" class="amcharts-legend-value" transform="translate(269,7)" style=font-size:16px;> <span ng-if="vm.currencyRate != null" currency-Exchange conversion-Rate="' + vm.currencyRate + '" input="' + totalIncome + '" from-Currency="' + vm.currencyCode + '"> </span></text><rect x="16" y="0" width="252.5" height="18" rx="0" ry="0" stroke-width="0" stroke="none" fill="#fff" fill-opacity="0.005"></rect></g>';
                angular.element(el).append($compile(html)($scope));
            });
        }

        function getLastTwentyYearCollection() {
            return dashboardService.getLastTwentyYearCollection().then(function (result) {
                vm.yearCollection = result.data;
            })
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

        function getMonthCollection() {
            return dashboardService.getMonthNames().then(function (result) {
                vm.monthCollection = result.data;
                vm.monthCollection.push({ display: "All", value: null });
            })
        }

        function getAnnualUploadResponse() {
            return dashboardService.getAnnualUploadResponse(vm.franchiseeId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.response = result.data;
                    vm.response.franchiseeId = vm.franchiseeId;
                }
            });
        }
        function getDocuments() {
            return dashboardService.getDocuments(vm.franchiseeId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.expiryingDocuments = result.data.collection;
                }
            });
        }
        function getPendingDocuments() {
            return dashboardService.getPendingDocuments(vm.franchiseeId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.pendingDocuments = result.data.collection;
                }
            });
        }
        function annualUpload() {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'modules/sales/views/upload-annual-batch.client.view.html',
                controller: 'UploadAnnualBatchController',
                controllerAs: 'vm',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    modalParam: function () {
                        return {
                            Response: vm.response
                        };
                    }
                }
            });
            modalInstance.result.then(function () {
                getAnnualUploadResponse();
            }, function () {

            });
        }

        function generateChartDataForreview() {

            var currentYear = clock.getYear(clock.now());
            if (vm.query.year == null || vm.query.year == currentYear) {
                vm.query.endDate = clock.now();
                vm.query.startDate = clock.getStartDateOfYear();
            }
            else {
                vm.query.startDate = "01/01/" + vm.query.year;
                vm.query.endDate = "12/31/" + vm.query.year;
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

        $scope.$emit("update-title", "Dashboard - " + vm.franchiseeName);
        $q.all([getRecentInvoicesForFranchisee(), getSalesSummaryForFranchisee(), getSalesRepLeaderboard(), getRevenueDetails(), getLastTwentyYearCollection(),
            getCustomerCount(), getPendingUploads(), getUnpaidInvoices(), getMonthCollection(), generateChartData(), getAnnualUploadResponse(), getDocuments(), getPendingDocuments(), generateChartDataForreview(), getReviewCount()]);

    }]);
})();