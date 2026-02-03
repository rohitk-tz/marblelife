(function () {
    'use strict';

    angular.module(OrganizationsConfiguration.moduleName).controller("FranchiseeDeactivationNoteController",
        ["$state", "$scope", "$rootScope", "FranchiseeService", "$q", "APP_CONFIG", "$uibModal", "Notification", "Toaster","modalParam","$uibModalInstance",
            function ($state, $scope, $rootScope, franchiseeService, $q, config, $uibModal, notification, toaster, modalParam, $uibModalInstance) {
                var vm = this;
                vm.franchiseeList = [];
                vm.query = {
                    deactivateNote: '',
                    franchiseeId: 0,
                };
                vm.cancel = cancel;
                vm.save = save;
                vm.query.franchiseeId = modalParam.FranchiseeId != null ? modalParam.FranchiseeId : 0;
                function save()
                {
                    return franchiseeService.deactivateFranchisee(vm.query).then(function (result) {
                        if (result.data == true) {
                            toaster.show("Franchisee has been Deactivated Successfully.");
                            $uibModalInstance.close();
                        }
                        else
                            toaster.error("Unable to Deactivate franchisee");
                    });
                }
                function cancel()
                {
                    $uibModalInstance.close();
                }
            }]);
}());