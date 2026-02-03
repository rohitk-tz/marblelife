(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalSendMailToCustomerController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "modalParam",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, modalParam) {

                var vm = this;
                vm.checkForValid = checkForValid;
                vm.isValid = true;
                vm.validate = validate;
                vm.remove = remove;
                vm.addMore = addMore;
                vm.imgs = modalParam.Imgs;
                vm.rowId = modalParam.Id;
                vm.save = save;
                vm.info = {};
                vm.emailList = [];
                vm.emailList.push({ email: '' });
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                function save() {
                    vm.isEmailInvalid = false;
                    angular.forEach(vm.emailList, function (value) {
                        if (!value.mailTo.match(/^(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$/)) {
                            toaster.error('Invalid Email Id ' + value);
                            vm.isEmailInvalid = true;
                            return;
                        }
                    });
                    if (vm.isEmailInvalid) {
                        vm.isEmailInvalid = false;
                        return;
                    }
                    vm.isEmailInvalid = false;
                    vm.email = '';
                    angular.forEach(vm.emailList, function (value1) {
                        if (vm.email != '') {
                            vm.email += ',' + value1.mailTo;
                        }
                        else {
                            vm.email = value1.mailTo;
                        }
                    });
                    vm.imgs.mailTo = vm.email;
                    toaster.show('Mail Sending in Progress');
                    $uibModalInstance.dismiss();
                    schedulerService.beforeAfterImageSendMail(vm.imgs, vm.rowId).then(function (result) {
                        if (result.data != null) {
                            if (!result.data) {
                                vm.isProcessing = false;
                                toaster.error(result.message.message);
                                
                            }
                            else {
                                vm.isProcessing = false;
                                toaster.show(result.message.message);
                                $uibModalInstance.dismiss();
                            }
                            vm.isProcessing = false;
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                function addMore() {
                    vm.emailList.push({ mailTo: '' });
                    vm.isValid = true;
                    angular.forEach(vm.emailList, function (value) {
                        if (!value.mailTo.match(/^(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$/)) {
                            vm.isValid = false;
                            return;
                        }
                    });
                }
                function remove(index) {
                    if (vm.emailList.length == 1) {
                        return;
                    }
                    vm.emailList.splice(index, 1);
                    vm.isValid = true;
                    angular.forEach(vm.emailList, function (value) {
                        if (!value.mailTo.match(/^(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$/)) {
                            vm.isValid = false;
                            return;
                        }
                    });
                }
                function validate(form, index) {

                    var email = vm.emailList[index].mailTo;
                    if (email == undefined)
                        return;
                    if (!email.match(/^(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$/)) {
                        form.email.$setValidity("invalid", false);
                        vm.isValid = false;
                        return;
                    }
                    else {
                        vm.isValid = true;
                        form.email.$setValidity("invalid", true);
                        angular.forEach(vm.emailList, function (value) {
                            if (!value.mailTo.match(/^(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$/)) {
                                vm.isValid = false;
                                return;
                            }

                        });
                    }
                }

                function checkForValid() {
                    vm.isValid = true;
                    angular.forEach(vm.emailList, function (value) {
                        if (!value.mailTo.match(/^(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$/)) {
                            vm.isValid = false;
                            return;
                        }


                    });
                }
                $q.all();
            }]);
}());