(function () {
    'use strict';
    angular.module(SalesConfiguration.moduleName).controller("FranchiseeAccountCreditController",
        ["$scope", "$rootScope", "$state", "APP_CONFIG", "$q", "$uibModalInstance", "modalParam", "Toaster", "Notification",
            "FranchiseAccountCreditService",
        function ($scope, $rootScope, $state, config, $q, $uibModalInstance, modalParam, toaster, notification,
            franchiseAccountCreditService) {

            var vm = this;
            vm.franchiseeId = modalParam.FranchiseeId;
            vm.createAccountCredit = false;
            vm.openModel = openModel;
            vm.closeModel = closeModel;
            vm.disableCreateAccountCredit = false;
            vm.accountCredit = {};
            vm.save = save;
            vm.isProcessing = false;
            vm.Roles = DataHelper.Role;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.deleteAccountCredit = deleteAccountCredit;
            vm.removeCredit = removeCredit;
            vm.pageSize = 10;
            vm.pageNumber = 1;
            vm.currentPage = vm.pageNumber;
            vm.count = 0;
            vm.pagingOptions = config.pagingOptions;
            vm.pageChange = pageChange;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function removeCredit(accountCreditId) {
                notification.showConfirm("Do you really want to remove remaining Credit Amount?", "Remove Credit", function () {
                    return franchiseAccountCreditService.removeCredit(accountCreditId).then(function (result) {
                        if (result.data != true)
                            toaster.error(result.message.message);
                        else
                            toaster.show(result.message.message);
                        getAccountCredit();
                    });
                });
            }

            function deleteAccountCredit(accountCreditId) {
                notification.showConfirm("Do you really want to delete the AccountCredit?", "Delete AccountCredit", function () {
                    return franchiseAccountCreditService.deleteAccountCredit(accountCreditId).then(function (result) {
                        if (result.data != true)
                            toaster.error(result.message.message);
                        else
                            toaster.show(result.message.message);
                        getAccountCredit();
                    });
                });
            }

            function getAccountCredit() {
                return franchiseAccountCreditService.getAccountCredit(vm.franchiseeId, vm.pageNumber, vm.pageSize).then(function (result) {
                    vm.list = result.data;
                    if (vm.list.collection.length > 0) {
                        vm.currencyRate = vm.list.collection[0].currencyRate;
                    }
                    vm.totalAmountByCategory = result.data.sumByCategory;
                    vm.count = result.data.pagingModel.totalRecords;
                    if (vm.list.collection.length > 0) {
                        vm.currencyRate = vm.list.collection[0].currencyRate;
                    }
                });
            }

            function openModel() {
                vm.createAccountCredit = true;
                vm.disableCreateAccountCredit = true;
            }

            function closeModel() {
                vm.createAccountCredit = false;
                vm.disableCreateAccountCredit = false;
                vm.accountCredit = {};
            }

            function save() {
                vm.isProcessing = true
                return franchiseAccountCreditService.saveAccountCredit(vm.accountCredit, vm.franchiseeId).then(function (result) {
                    if (result) {
                        toaster.show(result.message.message);
                        getAccountCredit();
                        closeModel();
                        vm.accountCredit = {};
                    }
                    else toaster.error(result.message.message);
                    vm.isProcessing = false;
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }

            function pageChange() {
                getAccountCredit();
            }

            function getCreditType() {
                return franchiseAccountCreditService.getCreditType().then(function (result) {
                    vm.creditType = result.data;
                });
            }

            $q.all([getAccountCredit(), getCreditType()]);
        }]);
}());