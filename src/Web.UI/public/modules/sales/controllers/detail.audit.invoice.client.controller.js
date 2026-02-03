(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).controller("AuditInvoiceDetailController",
        ["$scope", "$rootScope", "$state", "$stateParams", "$q", "APP_CONFIG", "FranchiseeService", "SalesService", "$uibModal", "FileService", "AnnualBatchService",
    function ($scope, $rootScope, $state, $stateParams, $q, config, franchiseeService, salesService, $uibModal, fileService, annualBatchService) {

        var vm = this;
        vm.invoiceId = $stateParams.invoiceId == null ? 0 : $stateParams.invoiceId;
        vm.auditInvoiceId = $stateParams.auditInvoiceId == null ? 0 : $stateParams.auditInvoiceId;
        vm.annualUploadId = $stateParams.annualUploadId == null ? 0 : $stateParams.annualUploadId;
        vm.Roles = DataHelper.Role;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;

        vm.getInvoiceDetail = getInvoiceDetail;
        vm.currentRole = $rootScope.identity.roleId;
        vm.goToList = goToList;

        function getInvoiceDetail() {
            return annualBatchService.getInvoiceDetail(vm.invoiceId, vm.auditInvoiceId).then(function (result) {
                if (result != null && result.data != null) {
                    vm.auditInfo = result.data;
                    vm.auditInfo.annualUploadId = vm.annualUploadId;
                    vm.auditInvoice = vm.auditInfo.auditInvoice;
                    vm.systemInvoice = vm.auditInfo.systemInvoice;
                    vm.auditInvoiceType13 = vm.auditInfo.auditInvoices;
                    vm.currencyRate = (vm.auditInvoice != null && vm.auditInvoice.invoiceItems != null)
                                        ? vm.auditInvoice.invoiceItems[0].currencyRate
                                        : vm.systemInvoice.invoiceItems[0].currencyRate;
                }
            });
        }

        function goToList() {
            $state.go('core.layout.sales.details', { annualDataUploadId: vm.annualUploadId });
        }

        $scope.$emit("update-title", "Audits Invoice Detail");

        $q.all([getInvoiceDetail()]);

    }]);
}());