(function () {

    angular.module(ReportsConfiguration.moduleName).controller("ServiceDescriptionController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam",
        function ($scope, $rootScope, $state, $uibModalInstance, modalParam) {

            var vm = this;
            vm.desc = modalParam.Description;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };
        }]);
}());