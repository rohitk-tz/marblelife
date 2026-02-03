(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalValidationCallNotesController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "Notification", "Clock", "Toaster", "modalParam",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, notification, clock, toaster, modalParam) {

                var vm = this;
                vm.close = close;
                vm.save = save;
                vm.isCallNoteSaved = modalParam.IsCallNoteSaved;

                vm.close = function () {
                    vm.isCallNoteSaved = false;
                    $rootScope.$emit("IsCallNoteSaved", vm.isCallNoteSaved);
                    $uibModalInstance.dismiss();
                };

                function save() {
                    vm.isCallNoteSaved = true;
                    $rootScope.$emit("IsCallNoteSaved", vm.isCallNoteSaved);
                    vm.close();
                }
            }]);
}());