(function () {
    'use strict';
    angular.module(SalesConfiguration.moduleName).controller("FranchiseeInvoiceDetailController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "InvoiceService", "modalParam", "Notification", "Toaster", "APP_CONFIG",'$filter',
        function ($scope, $rootScope, $state, $q, $uibModalInstance, invoiceService, modalParam, notification, toaster, config, $filter) {

            var vm = this;

            vm.InvoiceId = modalParam.InvoiceId;
            vm.currencyCode = $rootScope.identity.currencyCode;
            vm.getFranchiseeInvoiceDetails = getFranchiseeInvoiceDetails;
            vm.invoiceItemTypes = DataHelper.InvoiceItemTypes;
            vm.Roles = DataHelper.Role;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.deleteInvoiceItem = deleteInvoiceItem;
            vm.currencyExchangeReferenceSite = config.currencyExchangeRateReferenceSite;
            vm.goToReferenceSite = goToReferenceSite;
            vm.defaultCurrency = config.defaultCurrency;
            vm.status = DataHelper.InvoiceStatus;
            vm.serviceFeeTypes = DataHelper.ServiceFeeType;
            vm.IsOneTimeProjectFee = false;
            vm.oneTimeprojectDescription="";
            var count = 0;
            function goToReferenceSite() {
                window.open(vm.currencyExchangeReferenceSite, '_blank');
            }

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getFranchiseeInvoiceDetails(InvoiceId) {
                return invoiceService.getFranchiseeInvoiceDetails(InvoiceId).then(function (result) {
                    if (result != null && result.data != null) {
                        vm.invoiceDetails = result.data;
                        vm.currencyRate = vm.invoiceDetails.invoiceItems[0].currencyRate;
                        vm.currencyRateForNote = (1 / vm.currencyRate).toFixed(3);
                        var invoicesWithServiceFee = $filter('filter')(vm.invoiceDetails.invoiceItems, { serviceFeeTypeId: vm.serviceFeeTypes.OneTimeProject }, true);
                        if (invoicesWithServiceFee && invoicesWithServiceFee.length > 0) {
                            vm.IsOneTimeProjectFee = true;
                            vm.description = invoicesWithServiceFee[0].description;
                            vm.oneTimeprojectDescription =invoicesWithServiceFee[0].oneTimeProjectDescription;
                        }
                    }
                });
            }
            $rootScope.$on('clickCancle', function (event, data) {
                count = 0;
            })

            function deleteInvoiceItem(invoiceItemId) {
                if (count == 0) {
                    count++;
                    notification.showConfirm("Do you really want to delete the invoice Item?", "Delete InvoiceItem", function () {
                        return invoiceService.deleteInvoiceItem(invoiceItemId).then(function (result) {
                            if (result.data.isLastItem && result.data.isSuccess) {
                                toaster.show(result.data.response);
                                $uibModalInstance.close(result);
                                count = 0;
                            }
                            else {
                                count = 0;
                                if (!result.data.isSuccess)
                                    toaster.error(result.data.response);

                                else
                                    toaster.show(result.data.response);
                                getFranchiseeInvoiceDetails(vm.InvoiceId);
                            }
                        });
                    });
                }
            }

            getFranchiseeInvoiceDetails(vm.InvoiceId);
        }]);
}());