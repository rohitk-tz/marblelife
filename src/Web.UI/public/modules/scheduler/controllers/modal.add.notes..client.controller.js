(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalAddNotesController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "modalParam",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, modalParam) {

                var vm = this;
                vm.close = close;
                vm.isValid = true;
                vm.addBreak = addBreak;
                vm.imgs = modalParam.Imgs;
                vm.isEditable = modalParam.IsEditable;
                vm.serviceList = modalParam.ServiceList;
                vm.oldValue = angular.copy(modalParam.ServiceList);
                vm.from = modalParam.From;
                vm.save = save;
                vm.info = {};
                vm.emailList = [];
                vm.emailList.push({ email: '' });
                vm.close = function () {
                    vm.serviceList.notes = vm.oldValue.notes;
                    vm.serviceList.priceNotes = vm.oldValue.priceNotes;
                    vm.serviceList.option1 = vm.oldValue.option1;
                    vm.serviceList.option2 = vm.oldValue.option2;
                    vm.serviceList.option3 = vm.oldValue.option3;
                    
                    $uibModalInstance.dismiss();
                };
                function save() {
                    var htmlContentForNotes = document.getElementById('notes');
                    if (htmlContentForNotes != null) {
                        vm.serviceList.notes = htmlContentForNotes.value;
                        vm.serviceList.notes.replace(/(?:\r\n|\r|\n)/g, "<br>");
                    }
                    var htmlContentForInvoiceNotes = document.getElementById('invoiceNotes');
                    if (htmlContentForInvoiceNotes != null) {
                        vm.serviceList.priceNotes = htmlContentForInvoiceNotes.value;
                        vm.serviceList.priceNotes.replace(/(?:\r\n|\r|\n)/g, "<br>");
                    }
                    var htmlContentForInvoicePriceNotes = document.getElementById('priceNotes');
                    if (htmlContentForInvoicePriceNotes != null) {
                        vm.serviceList.priceNotes = htmlContentForInvoicePriceNotes.value;
                        vm.serviceList.priceNotes.replace(/(?:\r\n|\r|\n)/g, "<br>");
                    }
                    $uibModalInstance.dismiss();
				}
                function addBreak(text) {

                }
                $q.all();
            }]);
}());