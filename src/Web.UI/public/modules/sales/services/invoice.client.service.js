(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).service("InvoiceService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/sales/invoice"

        function getInvoiceList(query) {
            return httpWrapper.get({
                url: baseUrl + "?filter.text=" + query.text + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.franchiseeId=" + query.franchiseeId + "&filter.sortingColumn=" + query.sort.propName +
                    "&filter.statusId=" + query.statusId + "&filter.periodStartDate=" + query.periodStartDate + "&filter.salesDataUploadId=" + query.salesDataUploadId +
                    "&filter.periodEndDate=" + query.periodEndDate + "&filter.dueDateStart=" + query.dueDateStart + "&filter.dueDateEnd=" + query.dueDateEnd
                    + "&filter.sortingOrder=" + query.sort.order + "&filter.lateFeeTypeId=" + query.lateFeeTypeId + "&filter.isDownloaded=" + query.isDownloaded
                    + "&filter.paymentDateStart=" + query.paymentDateStart + "&filter.paymentDateEnd=" + query.paymentDateEnd + "&filter.undownloadedInvoice=" + query.undownloadedInvoice
                    + "&filter.accounting=" + query.accounting + "&filter.loanAndLoanInt=" + query.loanAndLoanInt + "&filter.iSQFT=" + query.iSQFT
                    + "&filter.webSEO=" + query.webSEO + "&filter.backUpCharges=" + query.backUpCharges + "&filter.isAdfund=" + query.isAdfund +"&filter.isRoyality=" + query.isRoyality
            })
        }

        function getFranchiseeInvoiceDetails(invoiceId) {
            return httpWrapper.get({ url: baseUrl + "?invoiceId=" + invoiceId });
        }

        function downloadAdfundInvoice(invoiceIds) {
            return httpWrapper.getFileByPost({ url: "/invoice/downloadAdfundInvoice", data: invoiceIds });
        }

        function downloadRoyalityInvoice(invoiceIds) {
            return httpWrapper.getFileByPost({ url: "/invoice/downloadRoyalityInvoice", data: invoiceIds });
        }
        function downloadInvoiceList(invoiceIds) {
            return httpWrapper.getFileByPost({ url: "/invoice/download/list", data: invoiceIds });
        }
        function getDownloadedInvoiceList(invoiceIds, pageNumber, pageSize) {
            return httpWrapper.post({ url: "/invoice/downloaded/list/", data: invoiceIds });
        }
        function downloadAllInvoice(query) {
            return httpWrapper.getFileByPost({ url: "/invoice/download/all", data: query });
        }
        function downloadAllInvoiceList(query) {
            return httpWrapper.getFileByPost({ url: "/invoice/download/list/all", data: query });
        }
        function getInvoiceStatus() {
            return httpWrapper.get({ url: "/application/dropdown/GetInvoiceStatus" });
        }
        function deleteInvoiceItem(invoiceItemId) {
            return httpWrapper.get({ url: "/invoice/invoiceItem/" + invoiceItemId + "/delete" });
        }
        function markAsDownloaded(invoiceIds) {
            return httpWrapper.post({ url: "/invoice/mark/downloaded/", data: invoiceIds });
        }
        function saveReconciliationNotes(query) {
            return httpWrapper.post({ url: "/invoice/SaveReconciliationNotes", data: query });
        }

        return {
            getInvoiceList: getInvoiceList,
            getFranchiseeInvoiceDetails: getFranchiseeInvoiceDetails,
            downloadAdfundInvoice: downloadAdfundInvoice,
            downloadRoyalityInvoice: downloadRoyalityInvoice,
            getInvoiceStatus: getInvoiceStatus,
            downloadInvoiceList: downloadInvoiceList,
            deleteInvoiceItem: deleteInvoiceItem,
            downloadAllInvoice: downloadAllInvoice,
            downloadAllInvoiceList: downloadAllInvoiceList,
            getDownloadedInvoiceList: getDownloadedInvoiceList,
            markAsDownloaded: markAsDownloaded,
            saveReconciliationNotes: saveReconciliationNotes
        };
    }]);
})();