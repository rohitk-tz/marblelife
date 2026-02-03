(function () {

    angular.module(CoreConfiguration.moduleName).controller("ConfirmDialogController",
        ["data", "$uibModalInstance", "$rootScope", function (data, $uibModalInstance, $rootScope) {
            var vm = this;
            vm.title = data.title;
            vm.message = data.message;

            vm.ok = function () {
                $uibModalInstance.close();
            };

            vm.cancel = function () {
                $rootScope.$broadcast('clickCancle', count=1);
                $uibModalInstance.dismiss('cancel');
            };
        }]);

}());
