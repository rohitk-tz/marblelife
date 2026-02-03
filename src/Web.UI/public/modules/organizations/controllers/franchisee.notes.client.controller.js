(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("FranchiseeNotesController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService", "FileService", "Toaster", "FranchiseeDocumentService",
            "Clock", "$q", '$filter', 'Notification',
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService, fileService, toaster, franchiseeDocumentService,
                clock, $q, $filter, notification) {

                var vm = this;
                vm.info = {};
                vm.save = save;
                vm.currentDate = moment(clock.now());
                vm.Roles = DataHelper.Role;
                vm.DocumentType = DataHelper.DocumentName;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isFrontOffice = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.categoryId = modalParam.CategoryId;
                vm.info.message = modalParam.PopUpMeesage != null ? modalParam.PopUpMeesage : 0;
                vm.info.textboxMessage = modalParam.TextBoxMessage != null ? modalParam.TextBoxMessage : 0;
                vm.info.isFromDuration = modalParam.IsFromDuration != null ? modalParam.IsFromDuration : 0;
                vm.info.typeid = modalParam.Typeid != null ? modalParam.Typeid : 0;
                vm.info.franchiseeId = modalParam.FranchiseeId != null ? modalParam.FranchiseeId : 2;
                vm.info.typeId = modalParam.TypeId != null ? modalParam.TypeId : 0;
                vm.info.duration = modalParam.Duration != null ? modalParam.Duration : 0;
                vm.info.description = modalParam.Description != null ? modalParam.Description : '';
                vm.info.oldDescription = modalParam.Description != null ? modalParam.Description : '';
                vm.info.oldDuration = modalParam.Duration != null ? modalParam.Duration : 0;
                vm.IsFranchiseeSelected = false;
                var currentDate = moment(clock.now());
                vm.isEdit = false;

                function save() {
                    var mod = vm.info.duration % 1;
                    if (vm.info.isFromDuration) {
                        if (vm.info.duration == null || vm.info.duration == '') {
                            notification.showAlert("Duration Cannot be blank!");
                            return;
                        }
                        else if (vm.info.duration <= 0) {
                            notification.showAlert("Duration Cannot be in less than 0!");
                            return;
                        }
                        else if (vm.info.duration < 1.5) {
                            notification.showAlert("Duration Cannot be in Less Than 1.5 hrs!");
                            return;
                        }
                        if (vm.info.duration == vm.info.oldDuration) {
                            notification.showAlert("No Changes in Durationl!");
                            return;
                        }
                    }
                    else {
                        if (vm.info.description == null || vm.info.description == '') {
                            notification.showAlert("Note Cannot be blank!");
                            return;
                        }
                        if (vm.info.description == vm.info.oldDescription) {
                            notification.showAlert("No Changes in Notes!");
                            return;
                        }
                    }
                    
                    return franchiseeService.saveNotes(vm.info).then(function (result) {
                        if (result.data != null) {
                            if (result.data) {
                                toaster.show(result.message.message);
                                $uibModalInstance.close();
                            }
                            else
                                toaster.error(result.message.message);
                            vm.isProcessing = false;
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };


            }]);
}());