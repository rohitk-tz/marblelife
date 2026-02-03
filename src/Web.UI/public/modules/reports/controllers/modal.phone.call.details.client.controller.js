(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalPhoneCallDetailsController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "modalParam",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, modalParam) {

                var vm = this;
                vm.close = close;
                vm.isValid = true;
                vm.addBreak = addBreak;
                vm.imgs = modalParam.Imgs;
                vm.franchiseePhoneCall = modalParam.CallsDetails;
                vm.franchiseeName = modalParam.FranchiseeName;
                vm.save = save;
                vm.info = {};
                vm.emailList = [];
                vm.emailList.push({ email: '' });
                vm.close = function () {
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