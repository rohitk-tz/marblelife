(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).service("BatchService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/sales"

        function saveBatch(model) {
            return httpWrapper.post({ url: baseUrl + "/batch/upload", data: model });
        }

        function uploadCustomerList(model) {
            return httpWrapper.post({ url: baseUrl + "/batch/customer/upload", data: model });
        }

        function getBatchList(query) {
            return httpWrapper.get({
                url: baseUrl + "/batch?filter.franchiseeId=" + query.franchiseeId + "&filter.statusId=" + query.statusId
                 + "&filter.periodStartDate=" + query.periodStartDate + "&filter.periodEndDate=" + query.periodEndDate
                 + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.sortingColumn=" + query.sort.propName
            + "&filter.sortingOrder=" + query.sort.order + "&filter.text=" + query.text
            });
        }
        function getSalesDataUploadStatus() {
            return httpWrapper.get({ url: "/application/dropdown/GetSalesDataUploadStatus" });
        }

        function getLastBatchUploaded(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/batch/last/uploaded/" + franchiseeId });
        }

        function deleteBatch(id) {
            return httpWrapper.delete({ url: baseUrl + "/batch/delete/" + id });
        }

        function reparseBatch(id) {
            return httpWrapper.get({ url: baseUrl + "/batch/reparse/" + id });
        }

        function updateBatch(model) {
            return httpWrapper.post({ url: baseUrl + "/batch/update", data: model });
        }

        function getAnnualUploadInfo(model) {
            return httpWrapper.post({ url: baseUrl + "/batch/verify", data: model });
        }

        function uploadAnnualFile(model) {
            return httpWrapper.post({ url: baseUrl + "/batch/annual/upload", data: model });
        }
        return {
            saveBatch: saveBatch,
            getBatchList: getBatchList,
            getLastBatchUploaded: getLastBatchUploaded,
            deleteBatch: deleteBatch,
            getSalesDataUploadStatus: getSalesDataUploadStatus,
            updateBatch: updateBatch,
            uploadCustomerList: uploadCustomerList,
            reparseBatch: reparseBatch,
            getAnnualUploadInfo: getAnnualUploadInfo,
            uploadAnnualFile: uploadAnnualFile,
        };
    }]);
})();