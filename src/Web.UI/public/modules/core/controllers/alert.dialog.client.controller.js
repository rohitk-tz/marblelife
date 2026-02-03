(function () {

    angular.module(CoreConfiguration.moduleName).controller("AlertDialogController",
        ["message", "$uibModalInstance", function (message, $uibModalInstance) {
            var vm = this;
            vm.message = message;

            vm.cancel = function () {
                $uibModalInstance.close();
            };
        }]);

}());