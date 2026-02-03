(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ShowDeactivationNoteController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService) {

                var vm = this;
                vm.franchiseeId = modalParam.FranchiseeId;
                vm.getFranchiseeDeactivationNote = getFranchiseeDeactivationNote;

                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                function getFranchiseeDeactivationNote() {
                    return franchiseeService.getFranchiseeDeactivationNote(vm.franchiseeId).then(function (result) {
                        vm.deactivationNote = result.data;
                    });
                }
                getFranchiseeDeactivationNote();

            }]);
}());