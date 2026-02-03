(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("ServiceLeadersService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/marketingLead";

        function getServiceReport(query) {
            return httpWrapper.get({
                url: baseUrl + "/serviceLeaders?filter.startDate=" + query.startDate + "&filter.endDate=" + query.endDate
                    + "&filter.typeIds=" + query.typeIds + "&filter.franchiseeId=" + query.franchiseeId
            });
        }

        function getServiceTypes() {
            return httpWrapper.get({ url: "/application/dropdown/GetServicesForDropdown" });
        }

        return {
            getServiceReport: getServiceReport,
            getServiceTypes: getServiceTypes
        };
    }]);
})();
