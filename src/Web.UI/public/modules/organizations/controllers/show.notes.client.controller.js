(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ShowNotesController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService) {

                var vm = this;
                vm.franchiseeId = modalParam.FranchiseeId;
                vm.title = modalParam.Title;
                vm.description = modalParam.Description;
                vm.getFranchiseeNotes = getFranchiseeNotes;
                vm.isFromNotes = modalParam.IsFromNotes != null ? modalParam.IsFromNotes : false;

                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                function getFranchiseeNotes() {
                    return franchiseeService.getFranchiseeNotes(vm.franchiseeId).then(function (result) {
                        vm.notes = result.data;
                    });
                }

                if (!modalParam.IsFromNotes) {
                    getFranchiseeNotes();
                }

            }]);
}());