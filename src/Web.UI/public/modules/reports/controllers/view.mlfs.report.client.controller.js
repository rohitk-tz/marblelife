(function () {
    'use strict';
    angular.module(ReportsConfiguration.moduleName).controller("MLFSReportController",
        ["$state", "$stateParams", "$q", "$scope", "MarketingLeadService", "WebLeadService", "APP_CONFIG", "$rootScope", "FileService", "FranchiseeService", "$uibModal", "Clock",
            "DashboardService", "FranchiseeGrowthReportService", "MarketingLeadGraphService", "$filter", "MlfsReportService", "LateFeeReportService", "$window",
            function ($state, $stateParams, $q, $scope, marketingLeadService, webLeadService, config, $rootScope, fileService, franchiseeService, $uibModal, clock,
                dashboardService, franchiseeGrowthReportService, marketingLeadGraphService, $filter, mlfsReportService, lateFeeReportService, $window) {
                var vm = this;
                vm.endDate = null;
                vm.yearList = [];
                vm.isFromLoad = true;
                vm.sideMenuText = "Purchase Data";
                vm.getMlfsData = getMlfsData;
                vm.getMlfsReport = getMlfsReport;
                vm.resetSearch = resetSearch;
                vm.moveToTop = moveToTop;
                vm.isFromLoadDataStartDate = true;
                vm.isFromLoadDataEndDate = true;
                vm.isStartEndDateChanged = false;
                var d = new Date();
                var year = d.getFullYear();
                vm.startYear = new Date((year - 2) + '/01/01');
                vm.endYear = new Date((year) + '/12/31');
                vm.purchaseData = [];
                vm.sales = false;
                vm.purchase = true;
                vm.isVisible = false;
                vm.perData = false;
                vm.tabDisabled = true;
                $scope.loop = -1;
                vm.yearList.push({ display: '2011', value: 2011 }, { display: '2012', value: 2012 }, { display: '2013', value: 2013 }, { display: '2014', value: 2014 }, { display: '2015', value: 2015 }
                    , { display: '2016', value: 2016 }, { display: '2017', value: 2017 }, { display: '2018', value: 2018 }, { display: '2019', value: 2019 }, { display: '2020', value: 2020 },
                    { display: '2021', value: 2021 }, { display: '2022', value: 2022 });
                vm.serviceList = ["ENDURACRETE", "GROUTLIFE", "STONELIFE", "VINYLGUARD", "WOODLIFE"];
                vm.query = {
                    franchiseeId: 0,
                    text: 'null',
                    startDate: 2019,
                    endDate: 2021
                };

                function resetSearch() {
                    vm.query.startDate = 2019;
                    vm.query.endDate = 2021;
                    getMlfsReport();
                }

                function getMlfsReport() {
                    vm.purchaseData = [];
                    vm.salesData = [];
                    if (vm.isStartEndDateChanged) {
                        vm.isFromLoad = false;
                        vm.startYear = new Date((vm.query.startDate) + '/01/01');
                        vm.endYear = new Date((vm.query.endDate) + '/12/31');
                    }
                    if (vm.sales) {
                        vm.sideMenuText = "Sales Data";
                        getMLFSReportForSales();
                    }
                    else if (vm.purchase) {
                        vm.sideMenuText = "PurchaseData";
                        getMLFSReportForPurchase();
                    }
                    else if (vm.perData) {
                        vm.sideMenuText = "Calculated Value";
                        getMLFSReportForPer();
                    }
                }

                function getMLFSReportForPurchase() {
                    return lateFeeReportService.getReportForPurchase(vm.query).then(function (result) {
                        vm.purchaseData = result.data.franchiseeGroupModel;
                        vm.totalSumForServicesForPurchase = result.data.totalServiceSum;
                        vm.endDate = result.data.endDate;
                        vm.statusList = result.data.statusList;
                        vm.quarterData = result.data.quarterList;
                        vm.yearData = result.data.yearList;
                        vm.colorStatusWithYearList = result.data.colorStatusList;
                        vm.totalSumForPurchase = result.data.totalSum;
                        vm.totalSumForInternaltionlForPurchase = result.data.internationalFranchiseeSum;
                        vm.totalSumForLocalForPurchase = result.data.localFranchiseeSum;
                        vm.activeFranchiseeSumWithNewForPurchase = result.data.activeFranchiseeSum;
                        vm.activeFranchiseeSumWithOutsNewForPurchase = result.data.activeFranchiseeSumWithoutNew;
                        vm.activeLocalFranchiseeSumWithNewForPurchase = result.data.activeLocalFranchiseeSumWithNew;
                        vm.activeLocalFranchiseeSumWithOutsNewForPurchase = result.data.activeLocalFranchiseeSumWithoutNew;
                        vm.tabDisabled = false;
                    });
                }

                function getMLFSReportForSales() {
                    return lateFeeReportService.getReportForSales(vm.query).then(function (result) {
                        vm.salesData = result.data.franchiseeGroupModel;
                        vm.totalSumForServicesForSales = result.data.totalServiceSum;
                        vm.endDate = result.data.endDate;
                        vm.statusList = result.data.statusList;
                        vm.quarterData = result.data.quarterList;
                        vm.yearData = result.data.yearList;
                        vm.colorStatusWithYearList = result.data.colorStatusList;
                        vm.totalSumForSales = result.data.totalSum;
                        vm.totalSumForInternaltionlForSales = result.data.internationalFranchiseeSum;
                        vm.totalSumForLocalForSales = result.data.localFranchiseeSum;
                        vm.activeFranchiseeSumWithNewForSales = result.data.activeFranchiseeSum;
                        vm.activeFranchiseeSumWithOutNewForSales = result.data.activeFranchiseeSum;
                        vm.activeLocalFranchiseeSumWithNewForSales = result.data.activeLocalFranchiseeSumWithNew;
                        vm.activeLocalFranchiseeSumWithOutsNewForSales = result.data.activeLocalFranchiseeSumWithoutNew;
                        vm.activeFranchiseeSumWithOutsNewForSales = result.data.activeFranchiseeSumWithoutNew;
                        vm.tabDisabled = false;
                    });
                }

                function getMLFSReportForPer() {
                    getMLFSReportForPurchase();
                    getMLFSReportForSales();
                }

                function getMlfsData(isGraphDatas) {
                    if (isGraphDatas == 1) {
                        vm.sideMenuText = "Purchase Data";
                        vm.sales = false;
                        vm.purchase = true;
                        vm.perData = false;
                        vm.purchaseData = [];
                        vm.salesData = [];
                        getMLFSReportForPurchase();
                    }
                    else if (isGraphDatas == 2) {
                        vm.sideMenuText = "Sales Data";
                        vm.sales = true;
                        vm.purchase = false;
                        vm.perData = false;
                        vm.purchaseData = [];
                        vm.salesData = [];
                        getMLFSReportForSales();
                    }
                    else if (isGraphDatas == 3) {
                        vm.sideMenuText = "Calculated Data";
                        vm.sales = false;
                        vm.purchase = false;
                        vm.perData = true;
                        vm.purchaseData = [];
                        vm.salesData = [];
                        getMLFSReportForPer();
                    }
                }
                function moveToTop() {

                    $window.scrollTo(0, 0);
                }
                window.onscroll = function (e) {
                    var elem = $(e.currentTarget);
                    if (elem[0].scrollY > 100) {
                        changeValue(true);
                    }
                    else {
                        changeValue(false);
                    }
                }
                function changeValue(value) {
                    vm.isVisible = value;
                }


                $scope.$watch('vm.query.startDate', function (nv, ov) {
                    if (!vm.isFromLoadDataStartDate) {
                        vm.isFromLoad = false;
                        vm.isStartEndDateChanged = true;
                    }
                    else {
                        vm.isFromLoadDataStartDate = false;
                    }



                });

                $scope.$watch('vm.query.endDate', function (nv, ov) {

                    if (!vm.isFromLoadDataEndDate) {
                        
                        vm.isStartEndDateChanged = true;
                    }
                    else {
                        vm.isFromLoadDataEndDate = false;
                    }



                });
                $scope.$emit("update-title", "MLFS REPORT");
                $q.all([getMLFSReportForPurchase()]);
            }]);
}());