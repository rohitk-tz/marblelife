(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("VacationRepeatController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "EstimateService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "modalParam", "FileService", "$filter", "CustomerService", "SchedulerService",
    function ($scope, $rootScope, $state, $q, $uibModalInstance, estimateService, franchiseeService,
        notification, clock, toaster, addressService, modalParam, fileService, $filter, customerService, schedulerService) {

        var vm = this;
        vm.vacationInfo = modalParam.VacationInfo;
        vm.repeatModel = {};
        $scope.dateOptions = {
            showWeeks: false
        };
        var currentDate = moment(clock.now());
        vm.close = function () {
            $uibModalInstance.dismiss();
        };
        vm.repeat = DataHelper.RepeatFrequency;
        vm.save = save;

        if (vm.vacationInfo != null) {
            vm.repeatModel.vacationId = vm.vacationInfo.id;
            vm.repeatModel.assigneeId = vm.vacationInfo.userId;
            vm.repeatModel.franchiseeId = vm.vacationInfo.franchiseeId;
        }

        function getRepeatFrequency() {
            return estimateService.getRepeatFrequency().then(function (result) {
                vm.repeatFrequency = result.data;
            });
        }
        $scope.$on('clearDates', function (event) {
            vm.repeatModel.startDate = null;
            vm.repeatModel.endDate = null;
        });


        function save(form) {
            vm.isProcessing = true;

            if (vm.repeatModel.startDate == null && vm.repeatModel.endDate == null) {
                notification.showAlert("Please enter Start/End Time!");
                vm.isProcessing = false;
                return;
            }

            if (vm.repeatModel.endDate <= currentDate) {
                notification.showAlert("End Date Can't be of Past time!");
                vm.isProcessing = false;
                return;
            }

            if (vm.repeatModel.startDate != null && vm.repeatModel.endDate != null) {
                if (vm.repeatModel.endDate <= vm.repeatModel.startDate) {
                    notification.showAlert("End Time should be greater Than Start Time!");
                    vm.repeatModel.startDate == null;
                    vm.repeatModel.endDate == null;
                    $scope.$broadcast("reset-dates");
                    vm.isProcessing = false;
                    return;
                }
            }

            return estimateService.repeatVacation(vm.repeatModel).then(function (result) {
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

        $q.all([getRepeatFrequency()]);
    }]);
}());
