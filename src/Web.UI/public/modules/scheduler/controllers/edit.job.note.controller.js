(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("EditJobNoteController",
        ["$scope", "$rootScope", "$state", "$q", "SchedulerService", "FranchiseeService", "$uibModal",
            "Notification", "Clock", "Toaster", "$stateParams", "FileService", "EstimateService", "$filter", "CustomerService", "$sce", "$uibModalInstance","modalParam",
            function ($scope, $rootScope, $state, $q, schedulerService, franchiseeService, $uibModal,
                notification, clock, toaster, $stateParams, fileService, estimateService, $filter, customerService, $sce, $uibModalInstance, modalParam) {

                var vm = this;
                vm.jobInfo = {}
                vm.jobInfo.jobNote = modalParam.Note;
                vm.franchiseeName = modalParam.FranchiseeName;
                vm.jobInfo.id = modalParam.Id;
                vm.jobInfo.isJob = modalParam.IsJob;
                vm.close = close;
                vm.save = save;
                $scope.$emit("update-title", "Edit Job Note");

                function save() {
                    return schedulerService.saveJobNotes(vm.jobInfo).then(function (result) {
                        toaster.show(result.message.message);
                        $uibModalInstance.close();
                    })
                }
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                
            }]);
}());
