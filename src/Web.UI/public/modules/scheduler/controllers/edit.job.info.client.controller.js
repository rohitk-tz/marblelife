(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("EditJobInfoController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "JobService", "Toaster", "modalParam", "$uibModal", "SchedulerService", "Clock",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, jobService, toaster, modalParam, $uibModal, schedulerService, clock) {

            var vm = this;
            vm.Id = modalParam.Id;
            vm.close = function () {
                $uibModalInstance.dismiss();
            };
            $scope.dateOptions = {
                showWeeks: false
            };
            vm.validTech = true;
            vm.validate = validate;
            var currentDate = moment(clock.now());

            function getJobInfo() {
                return jobService.getJobInfo(vm.Id).then(function (result) {
                    vm.jobInfo = result.data;
                    vm.jobInfo.startDate = moment((vm.jobInfo.startDate + "Z")).format("MM/DD/YYYY HH:mm");
                    vm.jobInfo.endDate = moment((vm.jobInfo.endDate + "Z")).format("MM/DD/YYYY HH:mm");
                    getHolidayList();
                });
            }

            function getHolidayList() {
                schedulerService.getHolidayList(vm.jobInfo.franchiseeId).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.holidayList = result.data.collection;
                    }
                });
            }

            function validate() {
                vm.isProcessing = true;
                var isHoliday = false;

                var currentDateToCompare = moment(currentDate).format('MM/DD/YYYY');
                var startToCompare = moment(vm.jobInfo.start).format('MM/DD/YYYY');
                var endToCompare = moment(vm.jobInfo.end).format('MM/DD/YYYY');

                angular.forEach(vm.holidayList, function (item) {
                    var holidayDate = moment(item.actualstart).format('MM/DD/YYYY');
                    if (!isHoliday) {
                        if (holidayDate == startToCompare || holidayDate == endToCompare) {
                            notification.showAlert("Can't create Job On holiday(s)!");
                            isHoliday = true;
                            vm.isProcessing = false;
                            return;
                        }
                    }
                    return;
                });

                if (isHoliday)
                    return;

                if (vm.jobInfo.start == null || vm.jobInfo.end == null) {
                    notification.showAlert("Please enter Start/End Time!");
                    vm.isProcessing = false;
                    return;
                }
                if (vm.jobInfo.jobId <= 0) {

                    if (vm.jobInfo.start < currentDate || vm.jobInfo.end < currentDate) {
                        if (currentDateToCompare != startToCompare || currentDateToCompare != endToCompare) {
                            notification.showAlert("Job should not be of past Time!");
                            vm.jobInfo.start == null;
                            vm.jobInfo.end == null;
                            $scope.$broadcast("reset-dates");
                            vm.isProcessing = false;
                            return;
                        }
                    }
                }

                if (vm.jobInfo.end <= vm.jobInfo.start) {
                    notification.showAlert("End Time should be greater Than Start Time!");
                    vm.jobInfo.start == null;
                    vm.jobInfo.end == null;
                    $scope.$broadcast("reset-dates");
                    vm.isProcessing = false;
                    return;
                }
                save();
            }

            function save() {
                return jobService.save(vm.jobInfo).then(function (result) {
                    if (result.data != null) {
                        if (!result.data) {
                            toaster.error(result.message.message);
                        }
                        else {
                            toaster.show(result.message.message);
                            $uibModalInstance.close();
                        }
                        vm.isProcessing = false;
                    }
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }

            $q.all([getJobInfo()]);
        }]);
}());