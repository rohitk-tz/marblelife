(function () {
    'use strict';
    angular.module(SalesConfiguration.moduleName).controller("AccountCreditDetailController",
        ["$scope", "$rootScope", "$state", "$q", "SalesService", "$uibModalInstance", "modalParam",
        function ($scope, $rootScope, $state, $q, salesService, $uibModalInstance, modalParam) {

            var vm = this;
            vm.accountCreditId = modalParam.accountCreditId;
            vm.currencyCode = $rootScope.identity.currencyCode;
            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getDetails()
            {
                return salesService.getAccountCreditItems(vm.accountCreditId).then(function (result) {
                    vm.list = result.data;
                });
            }

            getDetails();
        }]);
}());