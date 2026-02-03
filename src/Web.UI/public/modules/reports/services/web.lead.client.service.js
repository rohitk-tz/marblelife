(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("WebLeadService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/marketingLead";

        function getWebLeads(query) {
            return httpWrapper.get({
                url: baseUrl + "/webLead?filter.text=" + query.text + "&filter.franchiseeId=" + query.franchiseeId
                    + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.convertedLead=" + query.convertedLead
                    + "&filter.startDate=" + query.startDate + "&filter.endDate=" + query.endDate + "&filter.url=" + query.url + "&filter.zipCode=" + query.zipCode + "&filter.street=" + query.street
                    + "&filter.name=" + query.name + "&filter.propertyType=" + query.propertyType + "&filter.webLeadId=" + query.webLeadId + "&filter.city=" + query.city + "&filter.state=" + query.state
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order
            });
        }

        function getMarketingClass() {
            return httpWrapper.get({ url: "/application/dropdown/GetMarketingClass" });
        }

        function downloadWebLead(query) {
            return httpWrapper.getFileByPost({ url: "/webLead/download", data: query });
        }

        function getwebLeadReport(query) {
            return httpWrapper.post({ url: baseUrl + "/webLead/report", data: query });
        }

        function downloadWebLeadReport(query) {
            return httpWrapper.getFileByPost({ url: "/webLead/report/download", data: query });
        }

        function getUrlList() {
            return httpWrapper.get({ url: "/application/dropdown/GetUrlList" });
        }

        return {
            getWebLeads: getWebLeads,
            getMarketingClass: getMarketingClass,
            downloadWebLead: downloadWebLead,
            getwebLeadReport: getwebLeadReport,
            downloadWebLeadReport: downloadWebLeadReport,
            getUrlList: getUrlList
        };
    }]);
})();
