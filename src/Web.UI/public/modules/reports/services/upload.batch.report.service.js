(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("UploadBatchReportService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/reports";

        function getUploadBatchReport(query) {
            return httpWrapper.get({
                url: baseUrl + "/uploadBatchReport?filter.franchiseeId=" + query.franchiseeId + "&pageNumber=" + query.pageNumber
                    + "&pageSize=" + query.pageSize + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order
                    + "&filter.year=" + query.year + "&filter.month=" + query.month + "&filter.statusId=" + query.statusId + "&filter.periodStartDate=" + query.periodStartDate + "&filter.periodEndDate=" + query.periodEndDate
            });
        }

        function getYears() {
            return httpWrapper.get({ url: "/application/dropdown/GetYearsForBatch" });
        }

        function downloadMissingUploadReport(query) {
            return httpWrapper.getFileByPost({ url: "/upload/report/download", data: query });
        }

        return {
            getUploadBatchReport: getUploadBatchReport,
            getYears: getYears,
            downloadMissingUploadReport: downloadMissingUploadReport
        };

    }]);
})();