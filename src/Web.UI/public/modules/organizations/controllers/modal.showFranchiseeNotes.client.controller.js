(function () {
    angular.module(OrganizationsConfiguration.moduleName).controller("ShowFranchiseeNotesController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "Toaster", "$filter", "FranchiseeService", "$q",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, toaster, $filter, franchiseeService, $q) {
                var vm = this;
                vm.franchiseeId = modalParam.FranchiseeId;
                vm.franchiseeName = modalParam.FranchiseeName;
                vm.franchiseeNotes = modalParam.FranchiseeNotes;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                vm.getNotes = getNotes;
                function getNotes() {
                    return franchiseeService.getFranchiseeNotes(vm.franchiseeId).then(function (result) {
                        vm.franchiseeNotes = result.data;
                    });
                }
                $q.all([getNotes()]);
            }           
        ]
    )
}());