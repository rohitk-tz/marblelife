(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("FranchiseeGrowthReportService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/reports";

        function getGrowthReport(query) {
            return httpWrapper.get({
                url: baseUrl + "/growthReport?filter.franchiseeId=" + query.franchiseeId + "&filter.year=" + query.year
                    + "&filter.month=" + query.month + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.serviceTypeId=" + query.serviceTypeId + "&filter.classTypeId=" + query.classTypeId
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order
            });
        }
        function downloadGrowthReport(query) {
            return httpWrapper.getFileByPost({ url: "/growth/report/download", data: query });
        }

        function getYearsForGrowthReport() {
            return httpWrapper.get({ url: "/application/dropdown/GetYearsForGrowthReport" });
        }
        function getARReport(query) {
            return httpWrapper.post({ url: "/ar/report", data: query });
        }
        return {
            getGrowthReport: getGrowthReport,
            downloadGrowthReport: downloadGrowthReport,
            getYearsForGrowthReport: getYearsForGrowthReport,
            getARReport: getARReport
        };
    }]);
})();