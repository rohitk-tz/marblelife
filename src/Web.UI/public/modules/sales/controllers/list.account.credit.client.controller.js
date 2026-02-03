(function () {
    'use strict';
    var SortColumns = {
        id: 'Id',
        franchiseeName: 'FranchiseeName',
        creditedOn: 'creditedOn',
        qbInvoiceNumber: 'QbInvoiceNumber',
        customerName: 'CustomerName',
        email: 'Email',
        phone: 'Phone',
        streetAddress: 'StreetAddress',
        city: 'City',
        state: 'State',
        zipCode: 'ZipCode',
    };
    angular.module(SalesConfiguration.moduleName).controller("ListAccountCreditController",
        ["$scope", "$rootScope", "$q", "SalesService", "APP_CONFIG", "FranchiseeService", "$uibModal",
            "$stateParams", "URLAuthenticationServiceForEncryption",
            function ($scope, $rootScope, $q, salesService, config, franchiseeService, $uibModal, $stateParams
                , URLAuthenticationServiceForEncryption) {

                var vm = this;
                vm.query = {
                    text: '',
                    franchiseeId: '',
                    customerName: '',
                    qbInvoiceNumber: '',
                    pageNumber: 1,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' },
                    from: null,
                    to: null
                };
                vm.isSuperAdmin = $rootScope.identity.roleId == 1;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.pagingOptions = config.pagingOptions;
                vm.list = [];
                vm.currentRole = $rootScope.identity.roleId;
                vm.query.franchiseeId = $stateParams.franchiseeId != null ? $stateParams.franchiseeId : 0;
                vm.searchOptions = []

                function prepareSearchOptions() {
                    //if (vm.currentRole == 1)
                    //    vm.searchOptions.push({ display: 'Franchisee', value: '1' })

                    vm.searchOptions.push({ display: 'Customer', value: '1' }, { display: 'Qb Invoice#', value: '2' }, { display: 'Other', value: '3' });
                }


                vm.searchOption = '';


                vm.getAccountCreditList = getAccountCreditList;
                vm.pageChange = pageChange;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.resetSearch = resetSearch;

                vm.resetSeachOption = resetSeachOption;
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }
                $scope.$emit("update-title", "Account Credit");

                function resetSeachOption() {
                    if (vm.seachOption == '1') {
                        vm.query.qbInvoiceNumber = '';
                        vm.query.text = '';
                    }
                    else if (vm.seachOption == '2') {
                        vm.query.text = '';
                        vm.query.customerName = '';
                    }
                    else {
                        vm.query.customerName = '';
                        vm.query.qbInvoiceNumber = '';
                    }
                }

                function getAccountCreditList() {
                    if (!vm.isSuperAdmin)
                        vm.query.franchiseeId = $rootScope.identity.organizationId;

                    return salesService.getAccountCreditList(vm.query).then(function (result) {
                        vm.list = result.data.collection;
                        vm.count = result.data.pagingModel.totalRecords;
                        vm.query.sort.order = result.data.filter.sortingOrder;
                    });
                }

                function resetSearch() {
                    vm.query.text = '';
                    vm.query.customerName = '';
                    vm.query.text = '';
                    vm.searchOption = '';
                    vm.query.from = null;
                    vm.query.to = null;
                    $scope.$broadcast("reset-dates");
                    getAccountCreditList();
                }
                $scope.$on('clearDates', function (event) {
                    vm.query.from = null;
                    vm.query.to = null;
                    getAccountCreditList();
                });
                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getAccountCreditList();
                }

                function pageChange() {
                    getAccountCreditList();
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                $q.all([getAccountCreditList(), getFranchiseeCollection(), prepareSearchOptions()]);
            }]);
}());