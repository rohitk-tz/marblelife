(function () {

    angular.module(SalesConfiguration.moduleName).controller("ShowLogsController",
        ["$scope", "$rootScope", "$state", "logs", "$uibModalInstance",
        function ($scope, $rootScope, $state, logs, $uibModalInstance) {

            var vm = this;
            vm.logs = logs;
            vm.close = function () {
                $uibModalInstance.dismiss();
            };

        }]);
}());