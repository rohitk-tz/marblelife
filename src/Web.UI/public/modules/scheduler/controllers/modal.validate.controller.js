(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalValidationController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "Notification", "Clock", "Toaster", "modalParam",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, notification, clock, toaster, modalParam) {

                var vm = this;
                vm.close = close;
                vm.save = save;
                vm.isEstimateSaved = modalParam.IsEstimateSaved;

                vm.close = function () {
                    vm.isEstimateSaved = false;
                    $rootScope.$emit("IsEstimateSaved", vm.isEstimateSaved);
                    $uibModalInstance.dismiss();
                };

                function save() {
                    vm.isEstimateSaved = true;
                    $rootScope.$emit("IsEstimateSaved", vm.isEstimateSaved);
                    vm.close();
                }
            }]);
}());