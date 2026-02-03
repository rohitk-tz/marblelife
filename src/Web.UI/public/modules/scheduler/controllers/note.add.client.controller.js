(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("SchedulerNoteController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "modalParam",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
            notification, clock, toaster, modalParam) {

            var vm = this;
            vm.jobInfo = modalParam.JobInfo;
            vm.estimateInfo = modalParam.EstimateInfo;
            vm.vacationInfo = modalParam.VacationInfo;
            vm.meetingInfo = modalParam.MeetingInfo;
            vm.save = save;
            vm.info = {};

            if (vm.jobInfo != null)
                vm.info.franchisee = vm.jobInfo.franchisee;
            else if (vm.estimateInfo != null)
                vm.info.franchisee = vm.estimateInfo.franchisee;
            else if (vm.vacationInfo != null)
                vm.info.franchisee = vm.vacationInfo.franchisee;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function save() {
                vm.info.jobId = vm.jobInfo != null ? vm.jobInfo.jobId : null;
                vm.info.estimateId = vm.estimateInfo != null ? vm.estimateInfo.id : null;
                vm.info.vacationId = vm.vacationInfo != null ? vm.vacationInfo.id : null;
                vm.info.meetingId = vm.meetingInfo != null ? vm.meetingInfo.id : null;
                vm.info.statusId = vm.jobInfo != null ? vm.jobInfo.statusId : null;
                vm.isProcessing = true;

                return schedulerService.saveNote(vm.info).then(function (result) {
                    if (result.data != null) {
                        toaster.show(result.message.message);
                        $uibModalInstance.close();
                        vm.isProcessing = false;
                    }
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }
            $q.all();
        }]);
}());