(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("ReportService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/reports";

        function getReportList(query) {
            return httpWrapper.get({
                url: baseUrl + "/servicereport?filter.franchiseeId=" + query.franchiseeId + "&filter.paymentDateEnd=" + query.paymentDateEnd
                    + "&filter.paymentDateStart=" + query.paymentDateStart + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order
                    + "&filter.classTypeId=" + query.classTypeId + "&filter.serviceTypeId=" + query.serviceTypeId
            });
        }

        function downloadSalesReport(query) {
            return httpWrapper.getFileByPost({ url: "/reports/download", data: query });
        }

        function getFranchiseeMailList(query) {
            return httpWrapper.post({ url: "/franchisee/mail", data: query });
        }
        return {
            getReportList: getReportList,
            downloadSalesReport: downloadSalesReport,
            getFranchiseeMailList: getFranchiseeMailList
        };

    }]);
})();