(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalListInvoiceNotesController",
        ["$uibModalInstance", "modalParam", "Toaster", "$rootScope", "$q",
            function ($uibModalInstance, modalParam, toaster, $rootScope, $q) {
                var vm = this;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                vm.Roles = DataHelper.Role;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.oneTimeNote = modalParam.ItemNote;
                //vm.item = modalParam.PriceEstimate;
                //vm.service = vm.item.service;
                //vm.serviceType = vm.item.serviceType;
                //vm.serviceTagId = vm.item.serviceTagId;
                //vm.category = vm.item.category;
                //vm.materialType = vm.item.materialType;
                //vm.note = vm.item.note;
                //vm.query = {
                //    service: '',
                //    serviceType: '',
                //    serviceTagId: '',
                //    category: '',
                //    materialType: '',
                //    note: ''
                //}

                vm.save = save;

                function save() {
                    vm.query.note = vm.note;
                    return managePriceEstimateService.saveNotes(vm.query).then(function (result) {
                        if (vm.item.note != vm.note) {
                            if (vm.item.note != null) {
                                toaster.show("Notes Updated Successfully!!");
                            }
                            else if (vm.note != null) {
                                toaster.show("Notes Added Successfully!!");
                            }
                        }
                        $uibModalInstance.dismiss();
                    });
                }

                //function getNote() {
                //    return invoiceService.getNotes(vm.getNotesQuery).then(function (result) {
                //        if (result.data != null) {
                //            vm.note = result.data.note;
                //        }
                //    });
                //}

                $q.all([]);
            }
        ]
    );
}());