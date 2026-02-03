(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("SchedulerOptionController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "Clock", "modalParam", "$uibModal",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, clock, modalParam, $uibModal) {

            var vm = this;
            vm.jobStartDateDate = modalParam.Date;
            vm.isFromPast = modalParam.isFromPast;
            vm.query = modalParam.Query;
            vm.createEstimate = createEstimate;
            vm.createJob = createJob;
            vm.markVacation = markVacation;
            vm.markMeeting = markMeeting;
            vm.ScheduleType = DataHelper.ScheduleType;
            vm.Roles = DataHelper.Role;
            vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
            vm.isSales = $rootScope.identity.roleId == vm.Roles.SalesRep;
            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function createEstimate() {
                $uibModalInstance.close(vm.ScheduleType.Estimate);
            }

            function createJob() {
                $uibModalInstance.close(vm.ScheduleType.Job);
            }

            function markVacation() {
                $uibModalInstance.close(vm.ScheduleType.Vacation);
            }
            function markMeeting() {
                $uibModalInstance.close(vm.ScheduleType.Meeting);
            }

            $q.all([]);
        }]);
}());