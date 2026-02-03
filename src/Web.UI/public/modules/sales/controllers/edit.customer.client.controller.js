(function () {
    'use strict';
    angular.module(SalesConfiguration.moduleName).controller("EditCustomerController",
        ["$scope", "$rootScope", "$state", "$q", "CustomerService", "$stateParams", "Notification",
        function ($scope, $rootScope, $state, $q, customerService, $stateParams, notification) {

            var vm = this;
            vm.customerId = $stateParams.id != null ? $stateParams.id : 0;
            vm.customer = {};
            vm.cancel = cancel;
            vm.save = save;
            vm.isProcessing = false;


            function getCustomer() {
                return customerService.getCustomerById(vm.customerId).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.customer = result.data;
                        vm.customer.marketingClassId = vm.customer.marketingClassId.toString();
                    }
                });
            }

            function cancel() {
                $state.go('core.layout.sales.customer');
            }

            function save() {

                vm.isProcessing = true;

                var phoneN = vm.customer.phoneNumber.replace(/_/g, "");
                if (phoneN == null || phoneN.trim().length < 1 || phoneN.length < 10) {
                    notification.showAlert("Please Enter a valid Phone Number");
                    vm.isProcessing = false;
                    return;
                }

                removeBlankFieldsForEmail(vm.customer.emails);

                return customerService.saveCustomer(vm.customer).then(function (result) {
                    vm.isProcessing = false;
                    $state.go('core.layout.sales.customer');
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }

            function removeBlankFieldsForEmail(emails) {
                var newList = [];
                angular.forEach(emails, function (value, key) {
                    if (value.email != null) {
                        newList.push(value);
                    }
                });
                vm.customer.emails = newList;
            }

            function getmarketingClassCollection() {
                return customerService.getmarketingClassCollection().then(function (result) {
                    vm.marketingClass = result.data;
                });
            }

            $scope.$emit("update-title", "EDIT Customer");

            function init() {
                $q.all([getCustomer(), getmarketingClassCollection()]);
            }
            init();

        }]);
}());