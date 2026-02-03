(function () {
    'use strict';

    var SortColumns = {
        Url: 'Url',
        Count: "Count",
        PhoneLabel: 'PhoneLabel',
        GrandTotal: 'GrandTotal',
    };

    angular.module(ReportsConfiguration.moduleName).controller("CallDetailAnalysisController",
        ["$state", "$stateParams", "$q", "$scope", "MarketingLeadService", "WebLeadService", "APP_CONFIG", "$rootScope", "FileService", "FranchiseeService", "$uibModal", "Clock",
            "DashboardService", "FranchiseeGrowthReportService", "MarketingLeadGraphService", "$filter",
            function ($state, $stateParams, $q, $scope, marketingLeadService, webLeadService, config, $rootScope, fileService, franchiseeService, $uibModal, clock,
                dashboardService, franchiseeGrowthReportService, marketingLeadGraphService, $filter) {
                var vm = this;
                vm.currentDate = clock.now();
                vm.currentDateForMonth = new Date();
                vm.ifFromTop = true;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
                vm.isSalesRep = $rootScope.identity.roleId == vm.Roles.SalesRep;
                vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.isOpsMgr = $rootScope.identity.roleId == vm.Roles.OperationsManager;
                vm.franchiseeId = 0;
                vm.monthOptions = [];
                vm.yearOptions = [];
              
                vm.monthOptions.push({ display: 'January', value: 1 }, { display: 'February', value: 2 }, { display: 'March', value: 3 }, { display: 'April', value: 4 }, { display: 'May', value: 5 }
                                              , { display: 'June', value: 6 }, { display: 'July', value: 7 }, { display: 'August', value: 8 }, { display: 'September', value: 9 }, { display: 'October', value: 10 }
                                                    , { display: 'November', value: 11 }, { display: 'December', value: 12 });
                vm.yearOptions.push({ display: '2019', value: '2019' }, { display: '2018', value: '2018' }, { display: '2017', value: '2017' }, { display: '2016', value: '2016' }, { display: '2015', value: '2015' }
                                              , { display: '2014', value: '2014' }, { display: '2013', value: '2013' }, { display: '2012', value: '2012' });

                vm.query = {
                    pageNumber: 1,
                    webPageNumber: 1,
                    startDate: vm.currentDate,
                    viewTypeId: "1",
                    franchiseeId: 0,
                    pageSize: config.defaultPageSize,
                    text: '',
                    url: '',
                    sort: { order: 0, propName: '' },
                    isTimeChange: false,
                    month: vm.currentDateForMonth.getMonth(),
                    year: vm.currentDateForMonth.getFullYear().toString()
                };

               
                if (!vm.isSuperAdmin) {
                    vm.franchiseeId = $rootScope.identity.organizationId;
                }
                if (vm.isExecutive) {
                    if ($stateParams != null && $stateParams.franchiseeId > 1)
                        vm.franchiseeId = $stateParams.franchiseeId;
                    else {
                        vm.franchiseeId = $rootScope.identity.loggedInOrganizationId;
                    }
                    getList();
                }
                vm.isFromTop = true;
                vm.startDateFormat = new Date(vm.query.startDate);
                vm.endDateFormat = new Date(vm.query.startDate);
                vm.endDateFormat.setDate(vm.endDateFormat.getDate() - 1);
                vm.startDateFormat.setDate(vm.endDateFormat.getDate() - 6);
                vm.startDateFormat = (vm.startDateFormat.getMonth() + 1) + "/" + (vm.startDateFormat.getDate()) + "/" + (vm.startDateFormat.getFullYear());
                vm.endDateFormat = (vm.endDateFormat.getMonth() + 1) + "/" + (vm.endDateFormat.getDate()) + "/" + (vm.endDateFormat.getFullYear());
                vm.getReport = getReport;
                vm.pagingOptions = config.pagingOptions;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.leadCount = 0;
                vm.sorting = sorting;
                vm.sortingForAdjustedData = sortingForAdjustedData;
                vm.adjuststedclass = true;
                vm.isGraphsActive = true;
                vm.franchiseeName = "National";
                vm.isRawVsAdjustedActive = false;
                vm.isManagementVsLocalDataActive = false;
                vm.isManagementDataActive = false;
                $scope.adjuctedclass = false;
                vm.adjustedSum = 0;
                vm.sortingLead = sortingLead;
                vm.SortColumns = SortColumns;
                vm.getFromTimeReport = getFromTimeReport;
                vm.pageChange = pageChange;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.currentRole = $rootScope.identity.roleId;
                vm.resetSearch = resetSearch;
                vm.isGraphData = isGraphData
                vm.refresh = refresh;
                vm.getPhoneLabelList = getPhoneLabelList;
                vm.getUrlList = getUrlList;
                vm.downloadReport = downloadReport;
                vm.ids = [];
                vm.viewTypes = [];
                vm.franchiseeList = [];
                vm.id = [];
                vm.getSummary = getSummary;
                vm.adjuctedData = adjuctedData;
                vm.rawData = rawData;
                vm.phoneVsWebReport = [];
                vm.average = [];
                vm.Items = null;
                vm.managementDetailReportList = null;
                $scope.amChartOptions = marketingLeadGraphService.getChartOptions();
                $scope.amChartOptions.data = vm.phoneVsWebReport;
                vm.generateChartData = generateChartData;
                vm.ManagementGraphValueNull = false;
                vm.managementDetailReportListForInits = null;
                vm.busVsPhoneReport = [];
                vm.frontOfficeAverage = [];
                vm.officePersonAverage = [];
                vm.responseWhenAvailableAverage = [];
                vm.responseNextDayAverage = [];
                vm.frontOfficeCount = 0;
                vm.officePersonCount = 0;
                vm.responseWhenAvailableCount = 0;
                vm.responseNextDayCount = 0;
                $scope.amChartOptionsforBusReview = marketingLeadGraphService.getChartOptionsForBusReview();
                $scope.amChartOptionsforBusReview.data = vm.busVsPhoneReport;
                vm.generateChartDataforBusReview = generateChartDataforBusReview;

                vm.webLocalVsNationalReport = [];
                $scope.amChartOptionsforWebReview = marketingLeadGraphService.getChartOptionsForWebReview();
                $scope.amChartOptionsforWebReview.data = vm.webLocalVsNationalReport;
                vm.generateChartDataforWebReview = generateChartDataforWebReview;

                vm.spamVsPhoneReport = [];
                $scope.amChartOptionsforSpam = marketingLeadGraphService.getChartOptionsForSpam();
                $scope.amChartOptionsforSpam.data = vm.spamVsPhoneReport;
                vm.generateChartDataforSpam = generateChartDataforSpam;

                vm.weeklyCallReport = [];
                $scope.amChartOptionsforWeeklyReport = marketingLeadGraphService.getWeeklyLeadChartData();
                $scope.amChartOptionsforWeeklyReport.data = vm.weeklyCallReport;
                vm.generateWeeklyChartData = generateWeeklyChartData;

                vm.dailyCallReport = [];
                $scope.amChartOptionsforDailyReport = marketingLeadGraphService.getDailyLeadChartData();
                $scope.amChartOptionsforDailyReport.data = vm.dailyCallReport;
                vm.generateDailyChartData = generateDailyChartData;

                vm.seasonalLeadReport = [];
                $scope.amChartOptionsforSeason = marketingLeadGraphService.getSeasonalLeadChartData();
                $scope.amChartOptionsforSeason.data = vm.seasonalLeadReport;
                vm.generateSeasonalChartData = generateSeasonalChartData;


                vm.callReport = [];
                $scope.amChartOptionsforCall = marketingLeadGraphService.getChartOptionsForCall();
                $scope.amChartOptionsforCall.data = vm.callReport;
                vm.generateChartDataforCall = generateChartDataforCall;


                vm.localPerformanceLeadReport = [];
                $scope.amChartOptionsforLocalPerformance = marketingLeadGraphService.getLocalPerformanceLeadChartData();
                $scope.amChartOptionsforLocalPerformance.data = vm.localPerformanceLeadReport;
                vm.generateLocalPerformanceChartData = generateLocalPerformanceChartData;
                vm.chartData = null;


                vm.managementChartData = [{ month: "", frontOfficeCount: 0, officePersonCount: 0, nationalCount: 0, localCount: 0, responseNextDayCount: 0, responseWhenAvailableCount: 0 }];


                vm.managementDataLeadReport = [];
                if (vm.query.franchiseeId == 0) {
                    $scope.amChartOptionsforManagementChart = marketingLeadGraphService.getManagementDataChartDataWithoutFranchisee(true);
                }
                else
                {
                    $scope.amChartOptionsforManagementChart = marketingLeadGraphService.getManagementDataChartDataWithoutFranchisee(false);
                }
                $scope.amChartOptionsforManagementChart.data = vm.data;
                vm.generateLocalManagementChartData = generateManagementChartData;


                vm.averageOfAverageCalls = 0;
                vm.averageOfSum = 0;
                vm.averageOfCallsPerDay = 0;
                vm.averageOfAnsweredCalls = 0;
                vm.averageOfPerCalls = 0;
                vm.averageOfPerCallsAnswered = 0;
                vm.averageOfPerMissedCalls = 0;

                function generateManagementChartData() {
                    return marketingLeadGraphService.getLocalSitePerformanceReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            //vm.localPerformanceLeadReport = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.data, 'myFirstChart11');
                        }
                    });
                }

                function generateLocalPerformanceChartData() {
                    return marketingLeadGraphService.getLocalSitePerformanceReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.localPerformanceLeadReport = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.localPerformanceLeadReport, 'myFirstChart10');
                        }
                    });
                }

                function generateChartDataforCall() {
                    return marketingLeadGraphService.getChartOptionsForCallGraph(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.callReport = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.callReport, 'myFirstChart7');
                        }
                    });
                }

                function rawData() {
                    $scope.adjuctedclass = false;
                    vm.adjuststedclass = false;
                    getCallDetailReport();
                    getWebLeadReport();
                }
                function adjuctedData() {
                    $scope.adjuctedclass = true;
                    vm.adjuststedclass = true;
                    getCallDetailReportRawData();
                }
                function getCallDetailReportRawData() {
                    vm.query.sortingColumn = vm.query.sort.propName;
                    vm.query.sortingOrder = vm.query.sort.order;

                    if (vm.query.startDate == null) {
                        vm.query.startDate = vm.currentDate;
                    }

                    if (vm.query.viewTypeId == null) {
                        vm.query.viewTypeId = '1';
                    }

                    return marketingLeadService.getcallDetailReportRawData(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.ids = [];
                            vm.headerList = [];
                            vm.callDetailReportList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            for (var i = 0; i < result.data.collection.length; i++) {
                                vm.headerList = result.data.collection[i].lstHeader;
                            }
                            if (result.data.adjuctedTotal != null) {
                                vm.summaryHeaderListAdjustedData = result.data.adjuctedTotal.lstHeader;
                                vm.adjustedSum = result.data.adjuctedTotal.grandTotal;
                            }
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            addResultToList(vm.callDetailReportList);
                        }
                    });
                }
                function generateSeasonalChartData() {
                    return marketingLeadGraphService.getSeasonalLeadReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.seasonalLeadReport = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.seasonalLeadReport, 'myFirstChart6');
                        }
                    });
                }

                function generateDailyChartData() {
                    return marketingLeadGraphService.getDailyPhoneReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.dailyCallReport = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.dailyCallReport, 'myFirstChart5');
                        }
                    });
                }

                function generateWeeklyChartData() {
                    return marketingLeadGraphService.getWeeklyPhoneReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.weeklyCallReport = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.weeklyCallReport, 'myFirstChart4');
                        }
                    });
                }

                function generateChartDataforSpam() {
                    return marketingLeadGraphService.getSpamVsPhoneReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.spamVsPhoneReport = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.spamVsPhoneReport, 'myFirstChart3');
                        }
                    });
                }

                function generateChartDataforWebReview() {
                    return marketingLeadGraphService.getWebLocalVsNationalReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.webLocalVsNationalReport = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.webLocalVsNationalReport, 'myFirstChart2');
                        }
                    });
                }

                function generateChartDataforBusReview() {
                    return marketingLeadGraphService.getBusVsPhoneReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.busVsPhoneReport = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.busVsPhoneReport, 'myFirstChart1');
                        }
                    });
                }

                function generateChartData() {
                    return marketingLeadGraphService.getPhoneVsWebReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.phoneVsWebReport = result.data.chartData;
                            $scope.$broadcast('amCharts.updateData', vm.phoneVsWebReport, 'myFirstChart');
                        }
                    });
                }

                function resetSearch() {
                    vm.franchiseeName = 'National';
                    vm.query.text = '';
                    vm.query.url = '';
                    vm.query.viewTypeId = null;
                    vm.query.franchiseeId = 0;
                    vm.query.pageNumber = 1;
                    vm.query.webPageNumber = 1;
                    vm.query.startDate = vm.currentDate;
                    vm.query.isTimeChange = false;
                    vm.query.franchiseeId = "0";
                    vm.query.text = '';
                    vm.query.month = vm.currentDateForMonth.getMonth(),
                   vm.query.year = vm.currentDateForMonth.getFullYear().toString()
                    getReport(false);
                   
                }
                function downloadReport() {
                    downloadcallDetailReport();
                    downloadwebLeadReport();
                }
                function downloadcallDetailReport() {
                    vm.downloading = true;
                    return marketingLeadService.downloadcallDetailReport(vm.query).then(function (result) {
                        var fileName = "callDetailReport.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                    }, function () { vm.downloading = false; });
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
                    vm.query.isTimeChange = true;
                    getReport();
                }

                function prepareSearchOptions() {
                    vm.viewTypes.push({ display: 'Month', value: '1' })
                    vm.viewTypes.push({ display: 'Week', value: '2' });
                    vm.viewTypes.push({ display: 'Day', value: '3' });
                    vm.viewTypes.push({ display: 'Year', value: '4' });
                }

                function prepareFranchiseeSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' })
                    vm.searchOptions.push({ display: 'Service', value: '2' }, { display: 'Class', value: '3' });
                }


                function getFromTimeReport() {
                    vm.isFromTop = false;
                    vm.query.isTimeChange = true;
                    getReport();
                }

                function getReport(isNotChange) {

                    if (isNotChange != null && isNotChange == true)
                    {
                        vm.query.isTimeChange = true;
                    }
                    else
                    {
                        vm.query.isTimeChange = false;
                    }
                    
                    var currentDate = new Date();
                    var testDate = new Date(vm.query.startDate);
                    currentDate = (currentDate.getMonth() + 1) + "/" + (currentDate.getDate()) + "/" + (currentDate.getFullYear());
                    testDate = (testDate.getMonth() + 1) + "/" + (testDate.getDate()) + "/" + (testDate.getFullYear());
                    if (testDate != currentDate) {
                        vm.startDateFormat = new Date(vm.query.startDate);
                        vm.endDateFormat = new Date(vm.query.startDate);
                        vm.endDateFormat.setDate(vm.endDateFormat.getDate());
                        vm.startDateFormat.setDate(vm.endDateFormat.getDate() - 6);
                        vm.startDateFormat = (vm.startDateFormat.getMonth() + 1) + "/" + (vm.startDateFormat.getDate()) + "/" + (vm.startDateFormat.getFullYear());
                        vm.endDateFormat = (vm.endDateFormat.getMonth() + 1) + "/" + (vm.endDateFormat.getDate()) + "/" + (vm.endDateFormat.getFullYear());
                    }
                    if (vm.adjuststedclass && vm.isRawVsAdjustedActive) {
                        getCallDetailReportRawData();
                    }
                    else if (vm.isRawVsAdjustedActive && !vm.adjuststedclass) {
                        getCallDetailReport();
                        getWebLeadReport();
                    }

                    if (vm.isGraphsActive) {
                        vm.adjuststedclass = false;
                        generateChartData();
                        generateChartDataforBusReview();
                        generateChartDataforWebReview();
                        generateChartDataforSpam();
                        getSummary();
                        generateWeeklyChartData();
                        generateDailyChartData();
                        generateSeasonalChartData();
                        generateChartDataforCall();
                        generateLocalPerformanceChartData();
                    }
                    if (vm.isManagementVsLocalDataActive) {
                        vm.averageOfAverageCalls = 0;
                        vm.averageOfSum = 0;
                        vm.averageOfCallsPerDay = 0;
                        vm.averageOfAnsweredCalls = 0;
                        vm.averageOfPerCalls = 0;
                        vm.averageOfPerCallsAnswered = 0;
                        vm.averageOfPerMissedCalls = 0;
                        getManagementVsLocalReport();

                    }
                    if (vm.isManagementDataActive) {
                        getManagementReport();
                    }
                }


                function getCallDetailReport() {
                    vm.query.sortingColumn = vm.query.sort.propName;
                    vm.query.sortingOrder = vm.query.sort.order;

                    if (vm.query.startDate == null) {
                        vm.query.startDate = vm.currentDate;
                    }

                    if (vm.query.viewTypeId == null) {
                        vm.query.viewTypeId = '1';
                    }
                    vm.callDetailReportList = null;
                    return marketingLeadService.getcallDetailReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.ids = [];
                            vm.headerList = [];
                            vm.callDetailReportList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            for (var i = 0; i < result.data.collection.length; i++) {
                                vm.headerList = result.data.collection[i].lstHeader;
                            }
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            addResultToList(vm.callDetailReportList);
                        }
                    });
                }


                function getSummary() {
                    if (vm.query.startDate == null) {
                        vm.query.startDate = vm.currentDate;
                    }

                    if (vm.query.viewTypeId == null) {
                        vm.query.viewTypeId = '1';
                    }

                    return marketingLeadGraphService.getSummary(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.summaryHeaderList = [];
                            vm.summary = result.data.summary;
                            if (result.data.summary != null)
                                vm.summaryHeaderList = result.data.summary.lstHeader;

                        }
                    });
                }

                function getWebLeadReport() {
                    vm.query.sortingColumn = vm.query.sort.propName;
                    vm.query.sortingOrder = vm.query.sort.order;

                    if (vm.query.startDate == null) {
                        vm.query.startDate = vm.currentDate;
                    }

                    if (vm.query.viewTypeId == null) {
                        vm.query.viewTypeId = '1';
                    }

                    return webLeadService.getwebLeadReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.id = [];
                            vm.headerWebLeadList = [];
                            vm.webLeadReportList = result.data.collection;
                            vm.leadCount = result.data.pagingModel.totalRecords;

                            for (var i = 0; i < result.data.collection.length; i++) {
                                vm.headerWebLeadList = result.data.collection[i].lstHeader;
                            }
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            addResultToWebLeadList(vm.webLeadReportList);
                        }
                    });
                }


                function addResultToList() {
                    angular.forEach(vm.callDetailReportList, function (value, key) {
                        vm.ids.push(value.id);
                    })
                }

                function addResultToWebLeadList() {
                    angular.forEach(vm.webLeadReportList, function (value, key) {
                        vm.id.push(value.id);
                    })
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {

                        vm.franchiseeCollection = result.data;
                        vm.franchiseeCollection.push({ alias: 'National', display: 'National', id: '0', value: '0' });
                        vm.query.franchiseeId = "0";
                    });
                }
                function sorting(propName) {
                    if (vm.query.viewTypeId == null)
                        vm.query.viewTypeId = '1';
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getCallDetailReport();
                };

                function sortingForAdjustedData(propName) {
                    if (vm.query.viewTypeId == null)
                        vm.query.viewTypeId = '1';
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getCallDetailReportRawData();
                };

                function sortingLead(propName) {
                    if (vm.query.viewTypeId == null)
                        vm.query.viewTypeId = '1';
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getWebLeadReport();
                };

                function pageChange() {
                    getReport();
                };

                function getPhoneLabelList() {
                    return marketingLeadService.getRoutingNumberList().then(function (result) {
                        vm.phoneLabelList = result.data;
                    });
                }

                function getUrlList() {
                    return webLeadService.getUrlList().then(function (result) {
                        vm.urlList = result.data;
                    });
                }

                function isGraphData(isGraphData) {
                    if (isGraphData == 1) {
                        vm.isGraphsActive = true;
                        vm.isRawVsAdjustedActive = false;
                        $scope.adjuctedclass = false;
                        vm.adjuctedclass = false;
                        vm.isManagementVsLocalDataActive = false;
                        vm.isManagementDataActive = false;
                        getReport(false, true);
                    }
                    else if (isGraphData == 2) {
                        vm.isGraphsActive = false;
                        vm.isRawVsAdjustedActive = true;
                        vm.adjuctedclass = true;
                        $scope.adjuctedclass = true;
                        vm.isManagementVsLocalDataActive = false;
                        vm.isManagementDataActive = false;
                        getReport(false, true);
                    }
                    else if (isGraphData == 3) {
                        vm.isGraphsActive = false;
                        vm.isRawVsAdjustedActive = false;
                        vm.adjuctedclass = false;
                        $scope.adjuctedclass = false;
                        vm.isManagementVsLocalDataActive = true;
                        vm.isManagementDataActive = false;
                        getManagementVsLocalReport();
                    }
                    else if (isGraphData == 4) {
                        vm.isGraphsActive = false;
                        vm.isRawVsAdjustedActive = false;
                        vm.adjuctedclass = false;
                        $scope.adjuctedclass = false;
                        vm.isManagementVsLocalDataActive = false;
                        vm.isManagementDataActive = true;
                        getManagementReport();
                        //generateManagementChartData();
                    }
                }

                function getManagementReport() {
                    $scope.amChartOptionsforManagementChart = null;
                    vm.frontOfficeAverage = [];
                    vm.officePersonAverage = [];
                    vm.responseWhenAvailableAverage = [];
                    vm.frontOfficeCount = [];
                    vm.officePersonCount = [];
                    vm.responseWhenAvailableCount = [];
                    vm.responseNextDayCount = [];
                    vm.responseNextDayAverage = [];

                    if (vm.query.franchiseeId != 0) {
                        $scope.currentFranchisee = $filter('filter')(vm.franchiseeCollection, { value: vm.query.franchiseeId }, true)[0];
                        vm.franchiseeName = $scope.currentFranchisee.display;
                    }
                    if (vm.franchiseeId != 0) {
                        $scope.currentFranchisee = $filter('filter')(vm.franchiseeCollection, { value: vm.franchiseeId.toString() }, true)[0];
                        vm.franchiseeName = $scope.currentFranchisee.display;
                    }

                    if (vm.query.franchiseeId == 0) {
                        $scope.amChartOptionsforManagementChart = marketingLeadGraphService.getManagementDataChartDataWithoutFranchisee(true);
                    }
                    else {
                        $scope.amChartOptionsforManagementChart = marketingLeadGraphService.getManagementDataChartDataWithoutFranchisee(false);
                    }
                    return marketingLeadService.getManagementReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            $scope.amChartOptionsforManagementChart = null;
                            vm.frontOfficeAverage = [];
                            vm.officePersonAverage = [];
                            vm.responseWhenAvailableAverage = [];
                            vm.frontOfficeCount = [];
                            vm.officePersonCount = [];
                            vm.responseWhenAvailableCount = [];
                            vm.responseNextDayCount = [];
                            vm.responseNextDayAverage = [];
                            vm.managementDetailReportList = result.data.managementVsLocalAverageData;
                            vm.monthCollection = result.data.monthCollection;
                            vm.mangementChartData = result.data.managementChartListModelForChart;
                            $scope.$broadcast('amCharts.updateData', vm.mangementChartData, 'myFirstChart11');

                            vm.officePerson = $filter('filter')(vm.managementDetailReportList, { categoryName: "OFFICE PERSON" }, true);
                            vm.respondWhenAvailable = $filter('filter')(vm.managementDetailReportList, { categoryName: "RESPOND WHEN AVAILABLE" }, true);
                            vm.respondsNextDay = $filter('filter')(vm.managementDetailReportList, { categoryName: "RESPONDS NEXT DAY" }, true);
                            vm.frontOffice = $filter('filter')(vm.managementDetailReportList, { categoryName: "FRONT OFFICE(MULTI LEVEL COVERAGE)" }, true);
                            if (vm.officePerson.length > 0) {
                                vm.officePerson = vm.officePerson[0].managementVsLocalAverageData;
                            }
                            if (vm.respondWhenAvailable.length > 0) {
                                vm.respondWhenAvailable = vm.respondWhenAvailable[0].managementVsLocalAverageData;
                            }
                            if (vm.respondsNextDay.length > 0) {
                                vm.respondsNextDay = vm.respondsNextDay[0].managementVsLocalAverageData;
                            }
                            if (vm.frontOffice.length > 0) {
                                vm.frontOffice = vm.frontOffice[0].managementVsLocalAverageData;
                            }
                        }
                    });
                }


                function getManagementVsLocalReport() {
                    vm.averageOfAverageCalls = 0;
                    vm.averageOfSum = 0;
                    vm.averageOfCallsPerDay = 0;
                    vm.averageOfAnsweredCalls = 0;
                    vm.averageOfPerCalls = 0;
                    vm.averageOfPerCallsAnswered = 0;
                    vm.averageOfPerMissedCalls = 0;
                    vm.frontOfficeCount = [];
                    vm.officePersonCount = [];
                    vm.responseWhenAvailableCount = [];
                    vm.responseNextDayCount = [];
                    return marketingLeadService.getManagementVsLocalReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.averageOfAverageCalls = 0;
                            vm.averageOfSum = 0;
                            vm.averageOfCallsPerDay = 0;
                            vm.averageOfAnsweredCalls = 0;
                            vm.averageOfPerCalls = 0;
                            vm.averageOfPerCallsAnswered = 0;
                            vm.averageOfPerMissedCalls = 0;
                            vm.frontOfficeCount = [];
                            vm.officePersonCount = [];
                            vm.responseWhenAvailableCount = [];
                            vm.responseNextDayCount = [];
                            vm.managementAndLocalDetailReportList = result.data.managementVsLocalTableData;
                            vm.managementDetailReportForAverageData = result.data.managementVsLocalAverageData;
                            vm.totalCallsForDayForZeros = result.data.totalCallsForDayForZeros;
                            vm.totalMissedCallsForZeros = result.data.totalMissedCallsForZeros;
                            vm.totalMissedCallsForTheDayForZeros = result.data.totalMissedCallsForTheDayForZeros;
                            vm.totalCallsReceivedForDayForZeros = result.data.totalCallsReceivedForDayForZeros;

                            if (vm.managementDetailReportForAverageData.length > 0) {
                                vm.totalCallsForMonth = result.data.totalCalls;
                                vm.totalMissedCallsForMonth = result.data.totalMissedCalls;
                                vm.ManagementGraphValueNull = false;
                            }
                            else {
                                vm.ManagementGraphValueNull = true;
                            }
                        }
                    });
                }


                $scope.$emit("update-title", "Call Detail Analysis");
                $q.all([getFranchiseeCollection(), getReport(), prepareSearchOptions(), getPhoneLabelList(), getUrlList()]);
            }]);
}());