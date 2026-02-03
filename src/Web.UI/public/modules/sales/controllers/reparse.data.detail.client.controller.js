(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).controller("ReparseDataDetailController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "APP_CONFIG", "modalParam", "BatchService", "SalesService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, config, modalParam, batchService, salesService) {

            var vm = this;

            vm.salesDataUploadId = modalParam.BatchId;
            vm.currencyCode = $rootScope.identity.currencyCode;
            vm.getReparseDataDetail = getReparseDataDetail;
            vm.reparseBatch = reparseBatch;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getReparseDataDetail() {
                return salesService.getpaidInvoices(vm.salesDataUploadId).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.invoiceList = result.data.collection;
                    }
                });
            }

            function reparseBatch() {
                vm.isProcessing = true;
                return batchService.reparseBatch(vm.salesDataUploadId).then(function (result) {
                    if (result.data != true)
                        toaster.error(result.message.message);
                    else
                        toaster.show(result.message.message);
                    getReparseDataDetail();
                    vm.isProcessing = false;

                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }

            getReparseDataDetail();
        }]);
}());