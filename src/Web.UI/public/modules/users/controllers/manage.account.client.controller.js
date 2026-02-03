(function () {
    'use strict';
    angular.module(UsersConfiguration.moduleName).controller("ManageAccountController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "UserService", "FranchiseeService", "modalParam", "Notification", "Toaster",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, userService, franchiseeService, modalParam, notification, toaster) {

            var vm = this;
            vm.userId = modalParam.UserId;
            vm.getFranchiseeCollection = getFranchiseeCollection;
            vm.franchiseeIds = [];
            vm.addFranchiseeToList = addFranchiseeToList;
            vm.manageAccount = manageAccount;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getFranchiseeCollection() {
                return franchiseeService.getFranchiseeInfo(vm.userId).then(function (result) {
                    vm.franchiseeCollection = result.data.collection;
                });
            }

            function addFranchiseeToList(franchiseeId) {
                var index = vm.franchiseeIds.indexOf(franchiseeId);
                if (index >= 0)
                    vm.franchiseeIds.splice(index, 1);
                else
                    vm.franchiseeIds.push(franchiseeId);
            }

            function manageAccount() {
                return userService.manageAccount(vm.userId, vm.franchiseeIds).then(function (result) {
                    if (result.data == true) {
                        toaster.show("Account settings has been changed.");
                        getFranchiseeCollection();
                        $uibModalInstance.close(result);
                    }
                });
            }

            $q.all([getFranchiseeCollection()]);
        }]);
}());