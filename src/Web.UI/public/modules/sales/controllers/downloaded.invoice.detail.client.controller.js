(function () {
    'use strict';
    angular.module(SalesConfiguration.moduleName).controller("DownloadedInvoiceDetailController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "InvoiceService", "modalParam", "$templateCache", "Notification",
            "Toaster", "FranchiseeService", "APP_CONFIG", "FileService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, invoiceService, modalParam, $templateCache, notification,
            toaster, franchiseeService, config, fileService) {

            var vm = this;
            vm.invoiceIds = modalParam.InvoiceIds;
            vm.Roles = DataHelper.Role;
            vm.query = modalParam.Query;
            vm.getDownloadedInvoiceList = getDownloadedInvoiceList;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.count = 0;
            vm.currentRole = $rootScope.identity.roleId;
            vm.pageChange = pageChange;
            vm.downloadedInvoiceIds = [];
            vm.markAsDownloaded = markAsDownloaded;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function pageChange() {
                getDownloadedInvoiceList();
            };

            function getDownloadedInvoiceList() {
                vm.query.isDownloaded = true;
                if (vm.invoiceIds.length < 1) {
                    return invoiceService.getInvoiceList(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.downloadedInvoiceIds = [];
                            vm.downloadedInvoices = result.data.collection;
                            addInvoicesToList(vm.downloadedInvoices);
                        }
                    });
                }
                else {
                    return invoiceService.getDownloadedInvoiceList(vm.invoiceIds).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.downloadedInvoiceIds = [];
                            vm.downloadedInvoices = result.data.collection;
                            addInvoicesToList(vm.downloadedInvoices);
                        }
                    });
                }
            }

            function addInvoicesToList() {
                angular.forEach(vm.downloadedInvoices, function (value, key) {
                    if (!value.isDownloaded) {
                        vm.downloadedInvoiceIds.push(value.invoiceId);
                    }
                });
            }

            function markAsDownloaded() {
                vm.isProcessing = true;
                return invoiceService.markAsDownloaded(vm.downloadedInvoiceIds).then(function (result) {
                    if (result) {
                        vm.isProcessing = false;
                        $uibModalInstance.close();
                        toaster.show("Invoice(s) has been marked as Uploaded.");
                    }
                });
            }

            $q.all([getDownloadedInvoiceList()]);

        }]);
}());