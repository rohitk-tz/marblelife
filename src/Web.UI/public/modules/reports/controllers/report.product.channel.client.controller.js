(function () {
    'use strict';

    var SortColumns = {
        ID: 'ID',
        Franchisee: 'Franchisee',
    };

    angular.module(ReportsConfiguration.moduleName).controller("ProductChannelReportController",
        ["$state", "$stateParams", "$q", "$scope", "APP_CONFIG", "$rootScope", "FileService", "FranchiseeService", "$uibModal", "Clock", "ProductReportService",
            "CustomerService", "URLAuthenticationServiceForEncryption",
            function ($state, $stateParams, $q, $scope, config, $rootScope, fileService, franchiseeService, $uibModal, clock, productReportService,
                customerService, URLAuthenticationServiceForEncryption) {
                var vm = this;
                vm.query = {
                    pageNumber: 1,
                    paymentDateEnd: null,
                    paymentDateStart: null,
                    franchiseeId: 0,
                    idList: [],
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };
                vm.currentDate = clock.now();
                vm.getReportList = getReportList;
                vm.pagingOptions = config.pagingOptions;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.pageChange = pageChange;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.currentRole = $rootScope.identity.roleId;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.downloadReport = downloadReport;
                vm.ids = [];
                vm.viewChart = viewChart;
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }
                $scope.settings = {
                    scrollable: true,
                    enableSearch: true,
                    selectedToTop: true,
                    buttonClasses: 'btn btn-primary leader_btn'
                };
                $scope.translationTexts = {
                    checkAll: 'Select All',
                    uncheckAll: 'Deselect All',
                    selectGroup: 'Select All',
                    buttonDefaultText: "Select Channel(s)",
                    dynamicButtonTextSuffix: 'Selected'
                };

                function viewChart() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/graph-sales-by-productChannel.client.view.html',
                        controller: 'ProductChannelChartController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'xl',
                        resolve: {
                            modalParam: function () {
                                return {
                                    //FranchiseeId: franchiseeId
                                };
                            }
                        }
                    });
                }
                function downloadReport() {
                    vm.query.typeIds = [];
                    if (vm.query.idList != null && vm.query.idList.length > 0) {
                        vm.query.typeIds = vm.query.idList.map(function (elem) {
                            return elem.id;
                        }).join(",");
                    }
                    vm.downloading = true;
                    return productReportService.downloadReport(vm.query).then(function (result) {
                        var fileName = "productChannel_report.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;

                    }, function () { vm.downloading = false; });
                }

                function refresh() {
                    getReportList();
                }

                function resetSearch() {
                    vm.query.text = '';
                    vm.query.franchiseeId = 0;
                    vm.query.idList = [];
                    vm.query.typeIds = [];
                    vm.query.paymentDateEnd = null;
                    vm.query.paymentDateStart = null;
                    vm.searchOption = '';
                    vm.query.pageNumber = 1;
                    $scope.$broadcast("reset-dates");
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.paymentDateEnd = null;
                    vm.query.paymentDateStart = null;
                    getReportList();
                });


                function getReportList() {
                    vm.query.typeIds = [];
                    if (vm.query.idList != null && vm.query.idList.length > 0) {
                        angular.forEach(vm.query.idList, function (value) {
                            vm.query.typeIds.push(value.id);
                        });
                    }

                    return productReportService.getReportList(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.ids = [];
                            vm.reportList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            addResultToList(vm.reportList);
                        }
                    });
                }
                function addResultToList() {
                    angular.forEach(vm.reportList, function (value, key) {
                        vm.ids.push(value.franchiseeId);
                    })
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function getProductChannels() {
                    return productReportService.getProductChannels().then(function (result) {
                        vm.serviceTypes = result.data;
                    });
                }

                function getmarketingClassCollection() {
                    return customerService.getmarketingClassCollection().then(function (result) {
                        vm.marketingClass = result.data;
                    });
                }

                function pageChange() {
                    getReportList();
                };

                $scope.$emit("update-title", "Product Channel Analysis");
                $q.all([getmarketingClassCollection(), getProductChannels(), getReportList(), getFranchiseeCollection()]);

            }]);
}());