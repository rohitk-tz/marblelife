(function () {

    angular.module(ReportsConfiguration.moduleName).controller("NotesDescriptionController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam) {

                var vm = this;
                vm.desc = modalParam.Description;

                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
            }]);
}());