(function () {
    'use strict';
    angular.module(CoreConfiguration.moduleName).controller("ServiceTypeController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "FranchiseeService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, franchiseeService) {

            var vm = this;
            vm.getServiceCollection = getServiceCollection;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getServiceCollection() {
                return franchiseeService.getServiceTypeCollectionNewOrder().then(function (result) {
                    vm.serviceTypeList = result.data;
                    //vm.serviceTypeGroupedList = result.data;
                });
            }

            $q.all([getServiceCollection()]);
        }]);
}());