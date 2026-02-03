(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalListInvoiceReconciliationNotesController",
        ["$uibModalInstance", "modalParam", "Toaster", "$rootScope", "$q", "InvoiceService",
            function ($uibModalInstance, modalParam, toaster, $rootScope, $q, invoiceService) {
                var vm = this;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                vm.Roles = DataHelper.Role;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.reconciliationNotes = modalParam.ReconciliationNotes;
                vm.id = modalParam.InvoiceId;
                vm.query = {
                    id: null,
                    reconciliationNotes: ''
                }

                vm.save = save;

                function save() {
                    vm.query.reconciliationNotes = vm.reconciliationNotes;
                    vm.query.id = vm.id;
                    return invoiceService.saveReconciliationNotes(vm.query).then(function (result) {
                        //if (vm.item.note != vm.note) {
                        //    if (vm.item.note != null) {
                        //        toaster.show("Notes Updated Successfully!!");
                        //    }
                        //    else if (vm.note != null) {
                        //        toaster.show("Notes Added Successfully!!");
                        //    }
                        //}
                        $uibModalInstance.dismiss();
                    });
                }

                $q.all([]);
            }
        ]
    );
}());