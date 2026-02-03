(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).service("AnnualBatchService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/sales/annualBatch"

        function saveBatch(model) {
            return httpWrapper.post({ url: baseUrl + "/upload", data: model });
        }

        function getBatchList(query) {
            return httpWrapper.get({
                url: baseUrl + "?filter.franchiseeId=" + query.franchiseeId + "&filter.statusId=" + query.statusId
                 + "&filter.year=" + query.year + "&filter.reviewStatusId=" + query.reviewStatusId
                 + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.sortingColumn=" + query.sort.propName
            + "&filter.sortingOrder=" + query.sort.order
            });
        }

        function getAnnualSalesList(query) {
            return httpWrapper.post({ url: baseUrl + "/detail", data: query });
        }

        function getInvoiceDetail(invoiceId, auditInvoiceId) {
            return httpWrapper.post({ url: baseUrl + "/" + invoiceId + "/compare/" + auditInvoiceId });
        }

        function getYears() {
            return httpWrapper.get({ url: "/application/dropdown/GetYearsForBatch" });
        }

        function getReviewStatus() {
            return httpWrapper.get({ url: "/application/dropdown/GetReviewStatus" });
        }

        function deleteBatch(id) {
            return httpWrapper.delete({ url: baseUrl + "/delete/" + id });
        }

        function manageBatch(isAccept, batchId) {
            return httpWrapper.post({ url: baseUrl + "/" + isAccept + "/action/" + batchId });
        }

        function getSystemAuditRecords(query) {
            return httpWrapper.post({ url: baseUrl + "/audit/records", data: query });
        }

        function downloadAnnualData(query) {
            return httpWrapper.getFileByPost({ url: "/sales/annualBatch/download", data: query });
        }

        function getFranchiseeForMissingAudit() {
            return httpWrapper.get({ url: "/application/dropdown/GetFranchiseeForMissingAudit" });
        }

        function getAnnualSalesAddressList(query) {
            return httpWrapper.post({ url: baseUrl + "/detail/address", data: query });
        }
        function getAnnualCustomersAddressList(query) {
            return httpWrapper.post({ url: baseUrl + "/detail/address/customer", data: query });
        }
        function updateCustomerInvoice(query) {
            return httpWrapper.post({ url: baseUrl + "/update/customer/adress", data: query });
        }
        function reparseBatch(id) {
            return httpWrapper.post({ url: baseUrl + "/audit/reparse/" + id });
        }
        return {
            saveBatch: saveBatch,
            getBatchList: getBatchList,
            getAnnualSalesList: getAnnualSalesList,
            getInvoiceDetail: getInvoiceDetail,
            getYears: getYears,
            deleteBatch: deleteBatch,
            manageBatch: manageBatch,
            getReviewStatus: getReviewStatus,
            getSystemAuditRecords: getSystemAuditRecords,
            downloadAnnualData: downloadAnnualData,
            getFranchiseeForMissingAudit: getFranchiseeForMissingAudit,
            getAnnualSalesAddressList: getAnnualSalesAddressList,
            getAnnualCustomersAddressList: getAnnualCustomersAddressList,
            updateCustomerInvoice: updateCustomerInvoice,
            reparseBatch: reparseBatch
        };
    }]);
})();