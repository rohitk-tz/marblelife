(function () {
    'use strict';
    angular.module(SalesConfiguration.moduleName).controller("InvoiceDetailController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SalesService", "modalParam",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, salesService, modalParam) {

            var vm = this;
            vm.InvoiceId = modalParam.InvoiceId;
            vm.getInvoiceDetails = getInvoiceDetails;
            vm.currencyCode = $rootScope.identity.currencyCode;
            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getInvoiceDetails(InvoiceId) {
                return salesService.getInvoiceDetails(InvoiceId).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.invoiceDetails = result.data;
                        vm.currencyRate = vm.invoiceDetails.invoiceItems[0].currencyRate;
                    }
                });
            }

            getInvoiceDetails(vm.InvoiceId);
        }]);
}());