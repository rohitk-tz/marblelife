(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ShowGoeCodeNotesController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService",
        function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService) {

            var vm = this;
            vm.geoCodeNotes = modalParam.GeoCodeNotes;
            vm.getFranchiseeNotes = getFranchiseeNotes;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getFranchiseeNotes() {
                vm.notes = vm.geoCodeNotes;
            }
            getFranchiseeNotes();

        }]);
}());