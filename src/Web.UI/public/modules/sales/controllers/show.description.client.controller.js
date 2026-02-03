(function () {

    angular.module(SalesConfiguration.moduleName).controller("ShowDescriptionController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "$stateParams", "$q", "modalParam", "FileService",
            function ($scope, $rootScope, $state, $uibModalInstance, $stateParams, $q, modalParam, fileService) {

                var vm = this;
                vm.description = modalParam.Description;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };


            }]);
}());