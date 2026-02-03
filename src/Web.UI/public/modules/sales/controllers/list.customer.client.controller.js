(function () {
    'use strict';

    var SortColumns = {
        Id: 'Id',
        Name: 'Name',
        Email: 'Email',
        Phone: 'Phone',
        StreetAddress: 'StreetAddress',
        City: 'City',
        State: 'State',
        ZipCode: 'ZipCode',
        Country: 'Country',
        LastInvoiceId: 'LastInvoiceId',
        Amount: 'Amount',
        MarketingClass: 'MarketingClass',
        DateCreated: 'DateCreated',
        DateModified: 'DateModified',
        ContactName: 'ContactName',
        TotalSales: 'TotalSales',
        AvgSales: 'AvgSales'
    };

    angular.module(SalesConfiguration.moduleName).controller("ListCustomerController", ["$state", "$stateParams", "$scope", "$q", "APP_CONFIG",
        "CustomerService", "FileService", "$rootScope", "$uibModal", "Toaster", "FranchiseeService", "LocalStorageService",
        function ($state, $stateParams, $scope, $q, config, customerService, fileService, $rootScope, $uibModal, toaster, franchiseeService, LocalStorageService) {
            var vm = this;
            vm.customers = [];
            vm.query = {
                franchiseeId: 0,
                text: '',
                pageNumber: 1,
                pageSize: config.defaultPageSize,
                sort: { order: 0, propName: '' },
                dateCreated: null,
                dateModified: null,
                receiveNotification: '',
                toDate: null,
                fromDate: null,
                advancedText: '',
                advancedSearchBy: ''
            };

            $scope.editMode = false;

            vm.SortColumns = SortColumns;
            vm.currentRole = $rootScope.identity.roleId;
            vm.uploadNewFile = uploadNewFile;
            vm.Roles = DataHelper.Role;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.currentPage = vm.query.pageNumber;
            vm.count = 0;
            vm.pagingOptions = config.pagingOptions;
            vm.getCustomers = getCustomers;
            vm.pageChange = pageChange;
            vm.sorting = sorting;
            vm.resetSearch = resetSearch;
            vm.customerIds = [];
            vm.downloadCustomerList = downloadCustomerList;
            vm.currencyCode = $rootScope.identity.currencyCode;
            vm.searchOptions = [];
            vm.searchAdvancedOptions = [];
            vm.ReceiveNotification = [];
            vm.searchOption = '';
            vm.searchAdvancedOption = '';
            vm.resetSeachOption = resetSeachOption;
            vm.resetAdvanceSearchOption = resetAdvanceSearchOption;
            vm.updateMarketingClass = updateMarketingClass;
            vm.isProcessing = false;
            vm.viewSalesData = viewSalesData;
            vm.editCustomer = editCustomer;


            var storedQuery = LocalStorageService.getCustomerStorageValue();

            function prepareSearchOptions() {
                if (vm.currentRole == vm.Roles.SuperAdmin) {
                    vm.searchOptions.push({ display: 'Franchisee', value: '4' })
                    vm.searchAdvancedOptions.push({ display: 'Name', value: '5' }, { display: 'Phone number', value: '6' },
                        { display: 'Email', value: '7' });
                }

                vm.searchOptions.push({ display: 'Receive Notification', value: '1' }, { display: 'Recent Customers', value: '3' },
                    { display: 'Contact Person', value: '11' },
                    { display: 'Class', value: '9' },
                    { display: 'Customer', value: '12' },
                    { display: 'City', value: '8' },
                    { display: 'State', value: '13' },
                    { display: 'Zip Code', value: '10' },
                    { display: 'Others', value: '2' });
            }
            function prepareReceiveNotificationOptions() {
                vm.ReceiveNotification.push({ display: 'Yes', value: 'true' },
                    { display: 'No', value: 'false' });
            }

            if (storedQuery != null) {
                vm.query = storedQuery;
                vm.searchOption = storedQuery.searchOption;
                vm.searchAdvancedOption = storedQuery.advancedSearchBy;
                //$scope.$broadcast("reset-dates");
                if (vm.query.dateCreated != null) {
                    vm.query.dateCreated = moment(new Date(storedQuery.dateCreated + "Z")).format("MM/DD/YYYY HH:mm");
                    vm.query.dateModified = moment(new Date(storedQuery.dateModified + "Z")).format("MM/DD/YYYY HH:mm");
                    $scope.$broadcast("reset-dates");
                }
                if (vm.query.toDate != null) {
                    vm.query.toDate = moment(new Date(storedQuery.toDate + "Z")).format("MM/DD/YYYY HH:mm");
                    vm.query.fromDate = moment(new Date(storedQuery.fromDate + "Z")).format("MM/DD/YYYY HH:mm");
                    $scope.$broadcast("reset-dates");
                }
                if (storedQuery.franchiseeId != null) {
                    vm.query.franchiseeId = storedQuery.franchiseeId;
                }
                if (storedQuery.advancedSearchBy != '') {
                    if (vm.searchAdvancedOption == 'Name') {
                        vm.searchAdvancedOption = '5'
                    }
                    if (vm.searchAdvancedOption == 'PhoneNumber') {
                        vm.searchAdvancedOption = '6'
                    }
                    if (vm.searchAdvancedOption == 'Email') {
                        vm.searchAdvancedOption = '7'
                    }
                    if (vm.searchAdvancedOption == 'City') {
                        vm.searchAdvancedOption = '8'
                    }
                    if (vm.searchAdvancedOption == 'Class') {
                        vm.searchAdvancedOption = '9'
                    }
                    if (vm.searchAdvancedOption == 'ZipCode') {
                        vm.searchAdvancedOption = '10'
                    }
                    if (vm.searchAdvancedOption == 'ContactPerson') {
                        vm.searchAdvancedOption = '11'
                    }
                }
            }

            function resetSeachOption() {
                if (vm.seachOption == '1') {
                    vm.query.text = '';
                    vm.query.dateCreated = null;
                    vm.query.dateModified = null;
                    vm.query.fromDate = null;
                    vm.query.toDate = null;
                    vm.query.pageNumber = 1;
                    vm.query.franchiseeId = 0;
                }
                else if (vm.seachOption == '2') {
                    vm.query.receiveNotification = '';
                    vm.query.dateCreated = null;
                    vm.query.dateModified = null;
                    vm.query.fromDate = null;
                    vm.query.toDate = null;
                    vm.query.pageNumber = 1;
                    vm.query.franchiseeId = 0;
                }
                else if (vm.seachOption == '3') {
                    vm.query.receiveNotification = '';
                    vm.query.dateCreated = null;
                    vm.query.dateModified = null;
                    vm.query.text = '';
                    vm.query.franchiseeId = 0;
                    vm.query.pageNumber = 1;
                    vm.query.fromDate = null;
                    vm.query.toDate = null;
                }
                //else if (vm.seachOption == '3') {
                //    vm.query.receiveNotification = '';
                //    vm.query.dateCreated = null;
                //    vm.query.dateModified = null;
                //    vm.query.text = '';
                //    vm.query.pageNumber = 1;
                //    vm.query.fromDate = null;
                //    vm.query.toDate = null;
                //}
                else {
                    vm.query.dateCreated = null;
                    vm.query.dateModified = null;
                    vm.query.fromDate = null;
                    vm.query.toDate = null;
                    vm.query.franchiseeId = 0;
                    vm.query.pageNumber = 1;
                    vm.query.advancedText = '';
                    vm.searchAdvancedOption = '';
                }
            }

            function resetAdvanceSearchOption() {
                vm.query.advancedText = '';
            }


            function resetSearch() {
                vm.searchOption = '';
                vm.searchAdvancedOption = '';
                vm.query.franchiseeId = 0;
                vm.query.text = '';
                vm.query.advancedText = '';
                vm.query.advancedSearchBy = '';
                vm.query.receiveNotification = '';
                vm.query.dateCreated = null;
                vm.query.dateModified = null;
                vm.query.fromDate = null;
                vm.query.toDate = null;
                $scope.$broadcast("reset-dates");
                vm.query.pageNumber = 1;
                getCustomers();
            }

            vm.refresh = refresh;

            function getCustomers() {
                $scope.editMode = false;
                if (vm.query.advancedText != '') {
                    getAdvancedSearchOption();
                }
                return customerService.getCustomers(vm.query).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.customerIds = [];
                        vm.customers = result.data.collection;
                        vm.count = result.data.pagingModel.totalRecords;
                        vm.query.sort.order = result.data.filter.sortingOrder;
                        addCustomerToList(vm.customers);
                    }
                });
            }
            function refresh() {
                getCustomers();
            }

            function getAdvancedSearchOption() {
                if (vm.searchAdvancedOption == '5') {
                    vm.query.advancedSearchBy = 'Name'
                }
                if (vm.searchAdvancedOption == '6') {
                    vm.query.advancedSearchBy = 'PhoneNumber'
                }
                if (vm.searchAdvancedOption == '7') {
                    vm.query.advancedSearchBy = 'Email'
                }
                if (vm.searchOption == '8') {
                    vm.query.advancedSearchBy = 'City'
                }
                if (vm.searchOption == '9') {
                    vm.query.advancedSearchBy = 'Class'
                }
                if (vm.searchOption == '10') {
                    vm.query.advancedSearchBy = 'ZipCode'
                }
                if (vm.searchOption == '11') {
                    vm.query.advancedSearchBy = 'ContactPerson'
                }
                if (vm.searchOption == '13') {
                    vm.query.advancedSearchBy = 'State'
                }
            }

            $scope.$on('clearDates', function (event) {
                vm.query.dateCreated = null;
                vm.query.dateModified = null;
                getCustomers();
            });
            function uploadNewFile() {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/sales/views/upload-customer.client.view.html',
                    controller: 'UploadCustomerController',
                    controllerAs: 'vm',
                    backdrop: 'static'
                });
                modalInstance.result.then(function () {
                    vm.getCustomers();
                }, function () {

                });
            }

            function addCustomerToList() {
                angular.forEach(vm.customers, function (value, key) {
                    vm.customerIds.push(value.customerId);
                })
            }

            function sorting(propName) {
                vm.query.sort.propName = propName;
                vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                getCustomers();
            };

            function pageChange() {
                getCustomers();
            };

            function downloadCustomerList() {
                //vm.downloading = true;
                var paramValues = {
                    'franchiseeId': vm.query.franchiseeId,
                    'text': vm.query.text,
                    'pageNumber': vm.query.pageNumber,
                    'pageSize': vm.query.pageSize,
                    'sortOrder': vm.query.sort.order,
                    'sortPropName': vm.query.sort.propName,
                    'dateCreated': vm.query.dateCreated,
                    'dateModified': vm.query.dateModified,
                    'receiveNotification': vm.query.receiveNotification,
                    'toDate': vm.query.toDate,
                    'fromDate': vm.query.fromDate,
                    'advancedText': vm.query.advancedText,
                    'advancedSearchBy': vm.query.advancedSearchBy
                }
                var url = $state.href('core.layout.sales.downloadInProgress', paramValues);
                var groupreportWindow = window.open(url, "myWindow", "toolbar=yes,resizable=no, location=yes,titlebar =0, directories=no, status=no, menubar=no, scrollbars=yes, resizable=no, copyhistory=yes, width=350, height=350");
                groupreportWindow.onunload = function () {
                    if (groupreportWindow.closed) {
                        toastr.success("File Downloaded Successfully");
                    }
                }
            }

            function getmarketingClassCollection() {
                return customerService.getmarketingClassCollection().then(function (result) {
                    vm.marketingClass = result.data;
                });
            }

            function getFranchiseeCollection() {
                return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                    vm.franchiseeCollection = result.data;
                });
            }

            function updateMarketingClass(id, classTypeId) {
                vm.isProcessing = true;
                return customerService.updateMarketingClass(id, classTypeId).then(function (result) {
                    if (result.data != true)
                        toaster.error(result.message.message);
                    else
                        toaster.show(result.message.message);

                    $scope.editMode = !($scope.editMode);
                    getCustomers();
                    vm.isProcessing = false;
                });
            }

            function editCustomer(customerId) {
                vm.query.searchOption = vm.searchOption;
                LocalStorageService.setCustomerStorageValue(vm.query);
                $state.go("core.layout.sales.customerEdit", { id: customerId });
            }

            function viewSalesData(customerId) {
                vm.query.searchOption = vm.searchOption;
                LocalStorageService.setCustomerStorageValue(vm.query);
                $state.go("core.layout.sales.customerSales", { customerId: customerId });
            }

            $scope.$emit("update-title", "Customer List");
            $q.all([getCustomers(), prepareSearchOptions(), prepareReceiveNotificationOptions(), getmarketingClassCollection(), getFranchiseeCollection()]);

        }]);
}());