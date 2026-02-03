(function () {
    'use strict';
    var SortColumns = {
        id: 'Id',
        franchiseeName: 'FranchiseeName',
        totalSales: 'TotalSales',
        creditAmount: 'CreditAmount',
        creditMemo: 'CreditMemo'
    };
    angular.module(SalesConfiguration.moduleName).controller("ListFranchiseeAccountCreditController",
        ["$scope", "$rootScope", "$q", "SalesService", "APP_CONFIG", "FranchiseeService",
            "Clock", "URLAuthenticationServiceForEncryption",
            function ($scope, $rootScope, $q, salesService, config, franchiseeService, clock,
                URLAuthenticationServiceForEncryption) {

                var vm = this;
                vm.query = {
                    text: '',
                    franchiseeId: '',
                    pageNumber: 1,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' },
                    month: clock.getMonth(),
                    year: clock.getYear()
                };
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.pagingOptions = config.pagingOptions;
                vm.list = [];
                vm.currentRole = $rootScope.identity.roleId;

                vm.searchOptions = []
                vm.searchOption = '';
                vm.getFranchiseeAccountCreditList = getFranchiseeAccountCreditList;
                vm.pageChange = pageChange;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.resetSearch = resetSearch;
                vm.searchByDates = searchByDates;
                vm.resetSeachOption = resetSeachOption;

                $scope.$emit("update-title", "Account Credit");
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }

                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' })
                    vm.searchOptions.push({ display: 'Date', value: '2' }, { display: 'Other', value: '3' });
                }

                function resetSeachOption() {
                    if (vm.seachOption == '1') {
                        vm.query.text = '';
                        vm.query.month = clock.getMonth();
                        vm.query.year = clock.getYear();
                    }
                    if (vm.seachOption == '2') {
                        vm.query.franchiseeId = '';
                        vm.query.text = '';
                    }
                    else {
                        vm.query.franchiseeId = '';
                        vm.query.month = clock.getMonth();
                        vm.query.year = clock.getYear();
                    }
                }

                function getFranchiseeAccountCreditList() {
                    if (!vm.isSuperAdmin)
                        vm.query.franchiseeId = $rootScope.identity.organizationId;

                    return salesService.getFranchiseeAccountCreditList(vm.query).then(function (result) {
                        vm.list = result.data.collection;
                        vm.count = result.data.pagingModel.totalRecords;
                        vm.query.sort.order = result.data.filter.sortingOrder;
                    });
                }

                function resetSearch() {
                    vm.query.text = '';
                    vm.query.franchiseeId = '';
                    vm.searchOption = '';
                    vm.query.month = clock.getMonth();
                    vm.query.year = clock.getYear();
                    getFranchiseeAccountCreditList();
                }
                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getFranchiseeAccountCreditList();
                }

                function pageChange() {
                    getFranchiseeAccountCreditList();
                }

                function getYears() {
                    return salesService.getYears().then(function (result) {
                        if (result) {
                            vm.years = result.data;
                        }
                    });
                }

                function getMonths() {
                    return salesService.getMonths().then(function (result) {
                        if (result) {
                            vm.months = result.data;
                        }
                    });
                }

                function searchByDates() {
                    if (vm.query.month == null)
                        vm.query.month = clock.getMonth();
                    if (vm.query.year == null)
                        vm.query.year = clock.getYear();
                    getFranchiseeAccountCreditList();
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                $q.all([getFranchiseeAccountCreditList(), getFranchiseeCollection(), prepareSearchOptions(), getYears(), getMonths()]);
            }]);
}());