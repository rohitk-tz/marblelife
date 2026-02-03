(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ShowDocumentHistryController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService) {

                var vm = this;
                vm.franchiseeName = modalParam.FranchiseeName;
                vm.list = modalParam.List;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };


            }]);
}());