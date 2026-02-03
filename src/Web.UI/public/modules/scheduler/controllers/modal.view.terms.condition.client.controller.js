(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalTermsAndConditionController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "modalParam",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, modalParam) {

                var vm = this;
                vm.close = close;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
            }]);
}());