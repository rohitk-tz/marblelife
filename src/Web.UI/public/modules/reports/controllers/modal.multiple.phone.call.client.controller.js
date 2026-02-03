(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalMultiplePhoneCallController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "modalParam", "MarketingLeadService",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, modalParam, marketingLeadService) {

                var vm = this;
                var mlist = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

                vm.addSlab = addSlab;
                vm.deleteSlab = deleteSlab;
                vm.editValue = editValue;
                vm.saveValue = saveValue;
                vm.histry = [];
                vm.isError = false;
                vm.histryCopy = [];
                vm.monthsPresent = [];
                var monthName = month_name(new Date());
                var monthFormt = (month_name(new Date())) + ',' + new Date().getFullYear().toString();
                var message = "Billing has been already entered for the ";
                var monthsString = "";

                vm.franchiseeName = modalParam.FranchiseeName;
                vm.franchiseeId = modalParam.FranchiseeId;
                if (modalParam.Histry.length == 0) {
                    vm.histry = [];
                    vm.histry.push({
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
                        dateOfChangeOld: moment(new Date()).toDate(),
                        IsGenerateInvoice: false,
                        franchiseeId: vm.franchiseeId,
                        isInvoiceGeneratedFromAPI: false
                    })
                }
                else {
                    angular.copy(modalParam.Histry, vm.histryCopy);
                    vm.histry = vm.histryCopy;
                }

                $scope.dateOptions = {
                    showWeeks: false
                };

                vm.close = close;
                vm.month_name = month_name;
                vm.isValid = true;
                vm.franchiseePhoneCall = {
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

                function save(item) {
                    var datetime = new Date(item.dateOfChange)
                    var year = (datetime.getFullYear());

                    item.totalCost = item.callCount * item.chargesForPhone;
                    item.dateOfChange = new Date(item.dateOfChange);
                    item.phoneIdList = vm.idList;

                    var year = datetime.getFullYear();
                    var month = datetime.getMonth();
                    item.month = month_name(datetime) + "," + year;
                }

                function saveValue() {
                    validation();

                    if (vm.isError) {
                        toaster.error(message +' .Please edit the same.');
                        return;
                    }
                    vm.franchiseePhoneCall.collection = [];
                    //vm.franchiseePhoneCall.collection = vm.histry;
                    vm.franchiseePhoneCall.collection = vm.histry;
                    //vm.franchiseePhoneCall = JSON.stringify({ Collection: vm.histry });
                    vm.franchiseePhoneCall.franchiseeId = vm.franchiseeId;
                    return marketingLeadService.saveFranchiseePhoneCallsByBulk(vm.franchiseePhoneCall).then(function (result) {
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
                function validation() {
                    vm.isError = false;
                    var month = "";
                    vm.monthsPresent = []
                    var monthsString = "";
                    message = "Billing has been already entered for the ";
                    angular.forEach(vm.histry, function (value) {

                        var datetime = new Date(value.dateOfChange)
                        var year = (datetime.getFullYear());
                        var year = datetime.getFullYear();
                        var month = datetime.getMonth();
                        value.month = month_name(datetime) + "," + year;

                        if (!value.isInvoiceGeneratedFromAPI) {
                            var month = value.month.split(",");
                            if (vm.monthsPresent.indexOf(month[0]) !== -1) {
                                monthsString += month[0]+", ";
                                vm.isError = true;
                            }
                            else {
                                var month = value.month.split(",");
                                vm.monthsPresent.push(month[0]);
                            }
                        }
                    });
                    message += monthsString;
                }
                function addSlab() {
                    vm.histry.push({
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
                        dateOfChangeOld: moment(new Date()).toDate(),
                        IsGenerateInvoice: false,
                        franchiseeId: vm.franchiseeId,
                        isInvoiceGeneratedFromAPI: false
                    })
                }

                function deleteSlab(index) {
                    vm.histry.splice(index, 1);
                }

                function editValue(item) {
                    item.dateOfChange = moment(item.dateOfChange).toDate();

                }
                $q.all();
            }]);
}());