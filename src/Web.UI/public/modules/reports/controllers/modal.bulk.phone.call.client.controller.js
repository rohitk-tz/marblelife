(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalBulkPhoneCallController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "modalParam", "MarketingLeadService",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, modalParam, marketingLeadService) {

                var vm = this;
                var mlist = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

                vm.editValue = editValue;
                var monthName = month_name(new Date());

                $scope.dateOptions = {
                    showWeeks: false
                };
                var monthFormt = (month_name(new Date())) + ',' + new Date().getFullYear().toString();
                vm.close = close;
                vm.month_name = month_name;
                vm.isValid = true;
                vm.franchiseePhoneCall = {
                    chargesForPhone: 15,
                    chargesForPhoneOld: 15,
                    callCount: 0,
                    callCountOld: 0,
                    totalCost: 0,
                    isEditCall: true,
                    isEdit: true,
                    month: monthFormt,
                    isEditDate: false,
                    dateOfChange: moment(new Date()).toDate(),
                    dateOfChangeOld: moment(new Date()).toDate()
                };

                function month_name(dt) {
                    mlist = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
                    return mlist[dt.getMonth()];
                };
                //vm.addBreak = addBreak;
                vm.imgs = modalParam.Imgs;
                vm.isEditable = modalParam.IsEditable;
                vm.idList = modalParam.IdList;
                vm.save = save;
                vm.info = {};
                vm.emailList = [];
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                function editValue() {
                    vm.franchiseePhoneCall.dateOfChange = moment(vm.franchiseePhoneCall.dateOfChange).toDate();
                    var monthFormt = (month_name(vm.franchiseePhoneCall.dateOfChange)) + ',' + (vm.franchiseePhoneCall.dateOfChange).getFullYear().toString();
                    vm.franchiseePhoneCall.month = monthFormt;
                }
                function save() {
                    var datetime = new Date(vm.franchiseePhoneCall.dateOfChange)
                    var year = (datetime.getFullYear());

                    vm.franchiseePhoneCall.totalCost = vm.franchiseePhoneCall.callCount * vm.franchiseePhoneCall.chargesForPhone;
                    vm.franchiseePhoneCall.dateOfChange = new Date(vm.franchiseePhoneCall.dateOfChange);
                    vm.franchiseePhoneCall.phoneIdList = vm.idList;

                    return marketingLeadService.editFranchiseePhoneCallsListByBulk(vm.franchiseePhoneCall).then(function (result) {
                        if (result != null && result.data != null) {
                            if (result.data) {
                                toaster.show("Changes done successfully!!");
                                $uibModalInstance.dismiss();
                            }
                            else {
                                toaster.error("Error in Saving Changes!!");
                            }
                        }
                    });
                }

                $q.all();
            }]);
}());