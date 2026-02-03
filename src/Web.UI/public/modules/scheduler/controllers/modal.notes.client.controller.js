(function () {
    angular.module(SchedulerConfiguration.moduleName).controller("NotesModal",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "SchedulerService", "FileService", "Toaster", "$filter", "$window",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, schedulerService, fileService, toaster, $filter, $window) {
                var vm = this;
                vm.modalName = modalParam.ModalName;
                vm.content = modalParam.Content;
                //getting the name of invoices
                vm.close = function () {
                    $uibModalInstance.dismiss();
                }
                $window.addEventListener('click', function (e) {
                    $uibModalInstance.dismiss();
                });
            }
        ]
    );
}());