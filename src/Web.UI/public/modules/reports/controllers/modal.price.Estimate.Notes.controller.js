(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalPriceEstimateNotesController",
        ["$uibModalInstance", "modalParam", "ManagePriceEstimateService", "Toaster", "$rootScope", "$q",
            function ($uibModalInstance, modalParam, managePriceEstimateService, toaster, $rootScope, $q) {
                var vm = this;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                vm.Roles = DataHelper.Role;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.item = modalParam.PriceEstimate;
                vm.service = vm.item.service;
                vm.serviceType = vm.item.serviceType;
                vm.serviceTagId = vm.item.serviceTagId;
                vm.category = vm.item.category;
                vm.materialType = vm.item.materialType;
                vm.note = vm.item.note;
                vm.query = {
                    service: '',
                    serviceType: '',
                    serviceTagId: '',
                    category: '',
                    materialType: '',
                    note: ''
                }

                vm.save = save;

                //function noteChanged() {
                //    if (vm.item.note != vm.note) {
                //        vm.isNoteChanged = true;
                //    }
                //}

                function save() {
                    vm.query.service = vm.item.service;
                    vm.query.serviceType = vm.item.serviceType;
                    vm.query.serviceTagId = vm.item.serviceTagId;
                    vm.query.category = vm.item.category;
                    vm.query.materialType = vm.item.materialType;
                    vm.query.note = vm.note;
                    return managePriceEstimateService.saveNotes(vm.query).then(function (result) {
                        if(vm.item.note != vm.note) {
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

                function getNote() {
                    vm.getNotesQuery = {
                        serviceTagId: vm.serviceTagId
                    }
                    return managePriceEstimateService.getNotes(vm.getNotesQuery).then(function (result) {
                        if (result.data != null) {
                            vm.note = result.data.note;
                        }
                    });
                }

                $q.all([getNote()]);
            }
        ]
    );
}());