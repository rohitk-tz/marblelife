(function () {
    'use strict';

    var SortColumns = {
        Account: 'Account',
        Name: 'Name',
        Email: 'Email',
        PrimaryContact: 'PrimaryContact',
        StreetAddress: 'StreetAddress',
        City: 'City',
        State: 'State',
        ZipCode: 'ZipCode',
        Country: 'Country',
        AccountCredit: 'AccountCredit',
        BusinessId: 'BusinessId'
    };

    angular.module(OrganizationsConfiguration.moduleName).controller("ListFranchiseeDirectoryController",
        ["$state", "$scope", "$rootScope", "FranchiseeService", "$q", "APP_CONFIG", "$uibModal", "Notification", "Toaster", "LocalStorageService", "FileService",
            function ($state, $scope, $rootScope, franchiseeService, $q, config, $uibModal, notification, toaster, LocalStorageService, fileService) {
                var vm = this;
                vm.durationApproval = durationApproval;
                vm.franchiseeList = [];
                vm.franchiseeCount = 0;
                vm.getDescription = getDescription;
                vm.franchiseeListForPrint = [];
                vm.query = {
                    text: '',
                    pageNumber: 1,
                    franchiseeId: 0,
                    franchisee: '',
                    email: '',
                    status: 1,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };
                vm.getFranchiseeCollection = getFranchiseeCollection;
                vm.addNotes = addNotes;
                vm.Roles = DataHelper.Role;
                vm.FranchiseeNotes = DataHelper.FranchiseeNotes;
                vm.currentPage = vm.query.pageNumber;
                vm.maxSize = 5;
                vm.count = 0;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin; //To do replace with lookup
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.franchiseeId = $rootScope.identity.organizationId;
                vm.pagingOptions = config.pagingOptions;
                vm.getFranchiseeList = getFranchiseeList;
                vm.getFranchiseeListSearchSelected = getFranchiseeListSearchSelected;
                vm.pageChange = pageChange;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.resetSearch = resetSearch;
                vm.searchOptions = []
                vm.searchOption = '';
                vm.activeInActiveOptions = [];
                vm.activeInActiveOption = '';
                vm.resetSeachOption = resetSeachOption;
                vm.currentRole = $rootScope.identity.roleId;
                vm.download = download;
                vm.downloadFile = downloadFile;
                var storedQuery = LocalStorageService.getFranchiseeStorageValue();
                if (storedQuery != null) {
                    vm.query = storedQuery;
                    vm.searchOption = storedQuery.searchOption;
                    if (storedQuery.franchiseeId != null) {
                        vm.query.franchiseeId = storedQuery.franchiseeId;
                    }
                }
                function prepareSearchOptions() {
                    vm.searchOptions.push({ display: 'Franchisee', value: '1' })
                    vm.searchOptions.push({ display: 'Email', value: '2' }, { display: 'Other', value: '3' });
                    //vm.searchOptions.push({ display: 'Status', value: '4' });
                    vm.activeInActiveOptions.push({ display: 'Active', value: 1 }, { display: 'Inactive', value: 0 });
                }
                function resetSeachOption() {
                    if (vm.seachOption == '1') {
                        vm.query.email = '';
                        vm.query.text = '';
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '2') {
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.franchisee = '';
                    }
                    else {
                        vm.query.email = '';
                        vm.query.pageNumber = 1;
                        vm.query.franchisee = '';
                    }
                }
                function resetSearch() {
                    vm.query.text = '';
                    vm.query.email = '';
                    vm.query.franchisee = '';
                    vm.query.franchiseeId = '';
                    vm.searchOption = '';
                    vm.query.pageNumber = 1;
                    vm.query.status = 1;
                    getFranchiseeList();
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePairByRole().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }
                function getFranchiseeList() {
                    vm.franchiseeCount = 0;
                    return franchiseeService.getFranchiseeDesignCollection(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.franchiseeList = result.data.collection;
                            angular.forEach(vm.franchiseeList, function (item) {
                                vm.franchiseeCount += item.franchiseeViewModel.length;
                            });                            
                        }
                    });
                }
                function pageChange() {
                    getFranchiseeList();
                };
                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getFranchiseeList();
                };
                function download() {
                    return franchiseeService.downloadFranchieeAdmin(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            var fileName = "Download_franchisee.xlsx";
                            fileService.downloadFile(result.data, fileName);
                        }
                    });
                }
                function downloadFile() {
                    return franchiseeService.downloadFileFranchieeAdmin(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            var fileName = "Download_franchisee.xlsx";
                            fileService.downloadFile(result.data, fileName);
                        }
                    });
                }
                function getFranchiseeListSearchSelected() {
                    vm.query.status = null;
                    getFranchiseeList();
                }
                function addNotes(franchiseeId, franchiseeName, title, message, isFromDuration, typeId, duration, description) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/franchisee-notes.client.view.html',
                        controller: 'FranchiseeNotesController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeName: franchiseeName,
                                    FranchiseeId: franchiseeId,
                                    PopUpMeesage: title,
                                    TextBoxMessage: message,
                                    IsFromDuration: isFromDuration,
                                    TypeId: typeId,
                                    Duration: duration,
                                    Description: description
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getFranchiseeList();
                    }, function () {
                            getFranchiseeList();
                    });
                }
                function durationApproval(franchiseeId, franchiseeName) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/duration-approval.client.view.html',
                        controller: 'DurationApprovalController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: franchiseeId,
                                    Message: "Duration Approval List For " + franchiseeName
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getFranchiseeList();
                    }, function () {
                            getFranchiseeList();
                    });
                }
                function getDescription(description, title) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/show-notes.client.view.html',
                        controller: 'ShowNotesController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Title: title,
                                    Description: description,
                                    IsFromNotes: true
                                };
                            }
                        }
                    });
                }
                $scope.$emit("update-title", "Franchisee Directory");
                $q.all([getFranchiseeList(), prepareSearchOptions(), getFranchiseeCollection()]);
            }]);
}());