(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("LateFeeReportService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/reports";

        function lateFeeReportService(query) {
            return httpWrapper.get({
                url: baseUrl + "/latefeeReport?filter.text=" + query.text + "&filter.franchiseeId=" + query.franchiseeId + "&filter.startDate=" + query.startDate
                    + "&filter.endDate=" + query.endDate + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order
                    + "&filter.lateFeeTypeId=" + query.lateFeeTypeId + "&filter.statusId=" + query.statusId
                    + "&filter.dueDateStart=" + query.dueDateStart + "&filter.dueDateEnd=" + query.dueDateEnd
            });
        }
        function getLateFeeItemType() {
            return httpWrapper.get({ url: "/application/dropdown/GetLateFeeItemType" });
        }
        function downloadLateFeeReport(query) {
            return httpWrapper.getFileByPost({ url: "/lateFee/report/download", data: query });
        }
        function getReportForPurchase(query) {
            return httpWrapper.post({
                url: baseUrl + "/mlfsReport", data: query });
        }
        function getReportForSales(query) {
            return httpWrapper.post({
                url: baseUrl + "/mlfsReportsales", data: query
            });
        }
        function getConfiguration(query) {
            return httpWrapper.post({ url: baseUrl + "/mlfsReport/getMLFSConfigurationData", data: query });
        }

        function saveMLFSConfiguration(query) {
            return httpWrapper.post({ url: baseUrl + "/mlfsReport/saveMLFSConfigurationData", data: query });
        }
        return {
            lateFeeReportService: lateFeeReportService,
            getLateFeeItemType: getLateFeeItemType,
            downloadLateFeeReport: downloadLateFeeReport,
            getReportForPurchase: getReportForPurchase,
            getReportForSales: getReportForSales,
            getConfiguration: getConfiguration,
            saveMLFSConfiguration: saveMLFSConfiguration
        };

    }]);
})();