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

    angular.module(OrganizationsConfiguration.moduleName).controller("ListFranchiseeForFranchiseeAdminController",
        ["$state", "$scope", "$rootScope", "FranchiseeService", "$q", "APP_CONFIG", "$uibModal",
            "Notification", "Toaster", "LocalStorageService", "FileService", "URLAuthenticationServiceForEncryption",
            function ($state, $scope, $rootScope, franchiseeService, $q, config, $uibModal,
                notification, toaster, LocalStorageService, fileService, URLAuthenticationServiceForEncryption) {
                var vm = this;
                vm.franchiseeList = [];
                vm.franchiseeListForPrint = [];
                vm.query = {
                    text: '',
                    pageNumber: 1,
                    franchiseeId: 0,
                    franchisee: '',
                    email: '',
                    status: null,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };
                vm.Roles = DataHelper.Role;
                vm.currentPage = vm.query.pageNumber;
                vm.maxSize = 5;
                vm.count = 0;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin; //To do replace with lookup

                vm.pagingOptions = config.pagingOptions;
                vm.addNew = addNew;
                vm.getFranchiseeList = getFranchiseeList;
                vm.pageChange = pageChange;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.resetSearch = resetSearch;
                vm.managePayment = managePayment;
                vm.deleteFranchisee = deleteFranchisee;
                vm.searchOptions = []
                vm.searchOption = '';
                vm.activeInActiveOptions = [];
                vm.activeInActiveOption = '';
                vm.resetSeachOption = resetSeachOption;
                vm.currentRole = $rootScope.identity.roleId;
                vm.accountCredit = accountCredit;
                vm.getNotes = getNotes;
                vm.deactivateFranchisee = deactivateFranchisee;
                vm.activateFranchisee = activateFranchisee;
                vm.oneTimeProjectFee = oneTimeProjectFee;
                vm.editFranchisee = editFranchisee;
                vm.createLoan = createLoan;
                vm.download = download;
                vm.editFranchisee = editFranchisee;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }

                var storedQuery = LocalStorageService.getFranchiseeStorageValue();
                if (storedQuery != null) {
                    vm.query = storedQuery;
                    vm.searchOption = storedQuery.searchOption;
                    if (storedQuery.franchiseeId != null) {
                        vm.query.franchiseeId = storedQuery.franchiseeId;
                    }
                }
                function createLoan(franchisee) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/loan-service.client.view.html',
                        controller: 'LoanServiceController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Franchisee: franchisee,
                                    FranchiseeId: franchisee.id
                                };
                            }
                        }
                    });
                }

                function oneTimeProjectFee(franchisee) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/oneTimeProject-fee.client.view.html',
                        controller: 'OneTimeProjectFeeController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Franchisee: franchisee,
                                    FranchiseeId: franchisee.id
                                };
                            }
                        }
                    });
                }

                function getNotes(franchiseeId) {
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
                                    FranchiseeId: franchiseeId
                                };
                            }
                        }
                    });
                }

                function getFranchiseeDeactivation(franchiseeId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/deactivate-note.client.view.html',
                        controller: 'FranchiseeDeactivationNoteController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: franchiseeId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getFranchiseeList();
                    }, function () {
                    });
                }

                function editFranchisee(franchiseeId) {
                    vm.query.searchOption = vm.searchOption;
                    LocalStorageService.setFranchiseeStorageValue(vm.query);
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }

                function deactivateFranchisee(franchiseeId) {
                    notification.showConfirm("Do you really want to Deactivate the Franchisee? All the users associated with this franchisee will be Deactivated automatically.", "Deactivate", function () {
                        getFranchiseeDeactivation(franchiseeId);

                        //return franchiseeService.deactivateFranchisee(franchiseeId).then(function (result) {
                        //    if (result.data == true) {
                        //        toaster.show("Franchisee has been Deactivated succesfully.");
                        //        getFranchiseeList();
                        //    }
                        //    else
                        //        toaster.error("Unable to Deactivate franchisee");
                        //});
                    });
                }

                function activateFranchisee(franchiseeId) {
                    return franchiseeService.activateFranchisee(franchiseeId).then(function (result) {
                        if (result.data == true) {
                            toaster.show("Franchisee has been Activated succesfully.");
                            getFranchiseeList();
                        }
                        else
                            toaster.error("Unable to Activate franchisee");
                    });
                }

                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin) {
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' })
                    }
                    vm.searchOptions.push({ display: 'Email', value: '2' }, { display: 'Other', value: '3' });
                    vm.activeInActiveOptions.push({ display: 'Active', value: '1' }, { display: 'Inactive', value: '2' });
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
                    getFranchiseeList();
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function getFranchiseeList() {
                    return franchiseeService.getFranchiseeCollection(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.franchiseeList = result.data.collection;
                            vm.franchiseeListForPrint = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.sort.order = result.data.filter.sortingOrder;
                        }
                    });
                }

                function addNew() {
                    $state.go("core.layout.franchisee.create");
                }
                function managePayment(franchiseeId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/manage-payment.client.view.html',
                        controller: 'ManagePaymentController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: franchiseeId
                                };
                            }
                        }
                    });
                }

                function accountCredit(franchiseeId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/franchisee-account-credit.client.view.html',
                        controller: 'FranchiseeAccountCreditController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: franchiseeId
                                };
                            }
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

                function deleteFranchisee(franchiseeId) {
                    notification.showConfirm("All users associated with the franchisee will be deleted. Are you sure about deleting the record?", "Delete Franchisee", function () {
                        return franchiseeService.deleteFranchisee(franchiseeId).then(function (result) {
                            if (result.data != true)
                                toaster.error(result.message.message);
                            else
                                toaster.show(result.message.message);
                            getFranchiseeList();
                        });
                    });
                }
                function download() {
                    return franchiseeService.download(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            var fileName = "Download_franchisee.xlsx";
                            fileService.downloadFile(result.data, fileName);
                        }
                    });
                }

                $scope.$emit("update-title", "Franchisee List");
                $q.all([getFranchiseeList(), prepareSearchOptions(), getFranchiseeCollection()]);
            }]);
}());