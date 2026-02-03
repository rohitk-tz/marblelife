(function () {
    'use strict';

    angular.module(AuthenticationConfiguration.moduleName).controller("ConfirmationController", ["$scope", "$stateParams", "SchedulerService", "$window",
        function ($scope, $stateParams, SchedulerService, $window) {
            var vm = this;
            vm.ConfirmationEnum = DataHelper.Confirmation;
            vm.responseDataFromApi = {};
            vm.responseData = {
                customerName: '',
                startDateTime: '',
                endDate: '',
                schedulerType: '',
                confirmationEnum: '',
                confirmationText:''
            }
            vm.query = {
                encryptedData: '',
                schedulerId: 0,
                status:true
            }
            vm.query.encryptedData = $stateParams.id;
            vm.query.status = $stateParams.status =="confirmed"?true:false;
            SchedulerService.confirmationScheduler(vm.query).then(function (response) {
                if (response.data != null) {
                    vm.responseData = response.data;
                    vm.responseData.confirmationText = vm.ConfirmationEnum.getValue(vm.responseData.confirmationEnum);
                }
            });
        }]);
}());