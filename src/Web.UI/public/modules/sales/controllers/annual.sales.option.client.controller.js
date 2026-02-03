(function () {
    'use strict';
    angular.module(SalesConfiguration.moduleName).controller("AnnualSalesOptionController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "Clock", "modalParam", "$uibModal",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, clock, modalParam, $uibModal) {

            var vm = this;
            vm.batchId = modalParam.BatchId;
            vm.acceptBatch = acceptBatch;
            vm.rejectBatch = rejectBatch;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function rejectBatch() {
                $uibModalInstance.close(1);
            }

            function acceptBatch() {
                $uibModalInstance.close(2);
            }

            $q.all([]);
        }]);
}());