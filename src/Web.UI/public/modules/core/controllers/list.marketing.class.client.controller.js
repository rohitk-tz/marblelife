(function () {
    'use strict';
    angular.module(CoreConfiguration.moduleName).controller("MarketingClassController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SalesService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, salesService) {

            var vm = this;
            vm.getmarketingClassCollection = getmarketingClassCollection;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getmarketingClassCollection() {
                return salesService.getmarketingClassCollectionNewList().then(function (result) {
                    vm.marketingClassList = result.data;
                });
            }

            $q.all([getmarketingClassCollection()]);
        }]);
}());