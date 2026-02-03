(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalValidationInvoiceLineItemController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "Notification", "Clock", "Toaster", "modalParam",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, notification, clock, toaster, modalParam) {

                var vm = this;
                vm.close = close;
                vm.save = save;
                vm.isInvoiceLineItemRemove = modalParam.IsInvoiceLineItemRemove;

                vm.close = function () {
                    vm.isInvoiceLineItemRemove = false;
                    $rootScope.$emit("IsInvoiceLineItemRemove", vm.isInvoiceLineItemRemove);
                    $uibModalInstance.dismiss();
                };

                function save() {
                    vm.isInvoiceLineItemRemove = true;
                    $rootScope.$emit("IsInvoiceLineItemRemove", vm.isInvoiceLineItemRemove);
                    vm.close();
                }
            }]);
}());