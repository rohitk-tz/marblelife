(function () {
    'use strict';

    var SortColumns = {
        Id: 'Id',
        Name: 'Name',
        Email: 'Email',
        Username: 'Username',
        FranchiseeName: 'FranchiseeName',
        StreetAddress: 'StreetAddress',
        City: 'City',
        State: 'State',
        ZipCode: 'ZipCode',
        Country: 'Country',
        LastLoginAt: 'LastLoginAt',
        Role: 'Role'
    };

    angular.module(UsersConfiguration.moduleName).controller("ListUserController",
        ["$state", "$stateParams", "$rootScope", "$q", "$scope", "APP_CONFIG", "Toaster",
            "UserService", "FranchiseeService", "$uibModal", "LocalStorageService", "URLAuthenticationServiceForEncryption",
            function ($state, $stateParams, $rootScope, $q, $scope, config, toaster, userService, franchiseeService, $uibModal,
                LocalStorageService, uRLAuthenticationServiceForEncryption) {
            var vm = this;
            vm.users = [];
            vm.query = {
                text: '',
                pageNumber: 1,
                name: '',
                email: '',
                userName: '',
                franchiseeId: 0,
                pageSize: config.defaultPageSize,
                sort: { order: 0, propName: '' },
                roleId: 0,
            };


            var storedQuery = LocalStorageService.getStorageValue();

            
            vm.count = 0;
            vm.editUser = editUser;
            vm.pagingOptions = config.pagingOptions;
            vm.addNew = addNew;
            vm.getUsers = getUsers;
            vm.pageChange = pageChange;
            vm.lock = lock;
            vm.sorting = sorting;
            vm.SortColumns = SortColumns;
            vm.resetSearch = resetSearch;
            vm.Roles = DataHelper.Role;
            vm.loggedInUserId = $rootScope.identity.userId;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.searchOptions = [];
            vm.searchStatusOptions = [];
            vm.searchOption = '';
            vm.resetSeachOption = resetSeachOption;
            vm.currentRole = $rootScope.identity.roleId;
            vm.manageAccount = manageAccount;
            vm.isFrontOfficeExe = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
            if (vm.isFrontOfficeExe) {
                if ($stateParams != null && $stateParams.franchiseeId > 1)
                    vm.query.franchiseeId = $stateParams.franchiseeId;
            }
            else
                vm.query.franchiseeId = $rootScope.identity.organizationId;
            if (storedQuery != null) {
                vm.query = storedQuery;
                vm.searchOption = storedQuery.searchOption;
                if (storedQuery.franchiseeId != null) {
                    vm.query.franchiseeId = storedQuery.franchiseeId;
                }
            }
            function prepareSearchOptions() {
                if (vm.currentRole == vm.Roles.SuperAdmin)
                    vm.searchOptions.push({ display: 'Franchisee', value: '1' })
                vm.searchOptions.push({ display: 'Role', value: '2' }, { display: 'Name', value: '3' },
                    { display: 'Email', value: '4' },
                    { display: 'User Name', value: '5' }, { display: 'Other', value: '6' }, { display: 'Status', value: '7' });
                vm.searchStatusOptions.push({ display: 'Active', value: '1' }, { display: 'Inactive', value: '0' });
            }

            function resetSeachOption() {
                if (vm.seachOption == '1') {
                    vm.query.name = '';
                    vm.query.email = '';
                    vm.query.text = '';
                    vm.query.userName = '';
                    vm.query.roleId = '';
                    vm.query.pageNumber = 1;
                    vm.query.statusId = null;
                }
                else if (vm.seachOption == '2') {
                    vm.query.text = '';
                    vm.query.franchiseeId = '';
                    vm.query.email = '';
                    vm.query.userName = '';
                    vm.query.name = '';
                    vm.query.pageNumber = 1;
                    vm.query.statusId = null;
                }
                else if (vm.seachOption == '3') {
                    vm.query.text = '';
                    vm.query.roleId = '';
                    vm.query.email = '';
                    vm.query.franchiseeId = '';
                    vm.query.userName = '';
                    vm.query.pageNumber = 1;
                    vm.query.statusId = null;
                }
                else if (vm.seachOption == '4') {
                    vm.query.text = '';
                    vm.query.roleId = '';
                    vm.query.name = '';
                    vm.query.userName = '';
                    vm.query.franchiseeId = '';
                    vm.query.pageNumber = 1;
                    vm.query.statusId = null;
                }
                else if (vm.seachOption == '5') {
                    vm.query.text = '';
                    vm.query.roleId = '';
                    vm.query.name = '';
                    vm.query.email = '';
                    vm.query.franchiseeId = '';
                    vm.query.pageNumber = 1;
                    vm.query.statusId = null;
                }
                else if (vm.seachOption == '6') {
                    vm.query.text = '';
                    vm.query.roleId = '';
                    vm.query.name = '';
                    vm.query.email = '';
                    vm.query.franchiseeId = '';
                    vm.query.pageNumber = 1;
                    vm.query.statusId = null;
                }
                else {
                    vm.query.roleId = '';
                    vm.query.name = '';
                    vm.query.email = '';
                    vm.query.userName = '';
                    vm.query.franchiseeId = '';
                    vm.query.pageNumber = 1;
                    vm.query.statusId = null;
                }
            }
            function resetSearch() {
                vm.query.text = '';
                vm.query.roleId = '';
                vm.query.name = '';
                vm.query.email = '';
                vm.query.userName = '';
                vm.query.franchiseeId = '';
                vm.query.statusId = null;
                vm.searchOption = '';
                vm.query.pageNumber = 1;
                getUsers();
            }

            function getFranchiseeCollection() {
                return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                    vm.franchiseeCollection = result.data;
                });
            }

                function addNew() {
                    if (vm.query.franchiseeId == null) {
                        vm.query.franchiseeId = 0;
                    }
                    $state.go("core.layout.user.create", { franchiseeId: uRLAuthenticationServiceForEncryption.encrypt(vm.query.franchiseeId.toString()) });
            }

            function editUser(userId, franchiseeId) {
                vm.query.searchOption = vm.searchOption;
                LocalStorageService.setStorageValue(vm.query);
                userId = uRLAuthenticationServiceForEncryption.encrypt(userId.toString());
                franchiseeId = uRLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                $state.go("core.layout.user.edit", { id: userId, franchiseeId: franchiseeId});
            }
            
            function getUsers() {
                if (!vm.isSuperAdmin && !vm.isFrontOfficeExe)
                    vm.query.franchiseeId = $rootScope.identity.organizationId;
                if (vm.isFrontOfficeExe)
                    vm.query.franchiseeId = $stateParams.franchiseeId;

                return userService.getUsers(vm.query).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.users = result.data.collection;
                        vm.count = result.data.pagingModel.totalRecords;
                        vm.query.sort.order = result.data.filter.sortingOrder;
                    }
                });
            }
            
            function sorting(propName) {
                vm.query.sort.propName = propName;
                vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                getUsers();
            };

            function pageChange() {
                getUsers();
            };
            function lock(userId, isLocked) {
                return userService.lock(userId, isLocked).then(function (data) {
                    if (data != null && data.message != null) {
                        if (data.data) {
                            toaster.show(data.message.message);
                        }
                        else
                            toaster.error(data.message.message);
                    }
                    getUsers();
                });
            }

            function getRoles() {
                return userService.getRoles().then(function (result) {
                    vm.roles = result.data;
                    vm.roles.push({ display: 'FrontOffice Executive', value: '5' });
                });
            }

            function manageAccount(userId) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/users/views/manage-account.client.view.html',
                    controller: 'ManageAccountController',
                    controllerAs: 'vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                UserId: userId
                            };
                        }
                    }
                });
                modalInstance.result.then(function () {
                    getUsers();
                }, function () {
                });
            }
            $scope.$emit("update-title", "User List");

            $q.all([getUsers(), getRoles(), prepareSearchOptions(), getFranchiseeCollection()]);

        }]);
}());