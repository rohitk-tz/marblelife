(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("MarketingClassLeadersService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/marketingLead";

        function getClassReport(query) {
            return httpWrapper.get({
                url: baseUrl + "/marketingClassLeader?filter.startDate=" + query.startDate + "&filter.endDate=" + query.endDate
                    + "&filter.typeIds=" + query.typeIds + "&filter.franchiseeId=" + query.franchiseeId
            });
        }
        function getmarketingClassCollection() {
            return httpWrapper.get({ url: "/application/dropdown/GetClassForDropdown" });
        }

        return {
            getClassReport: getClassReport,
            getmarketingClassCollection: getmarketingClassCollection
        };
    }]);
})();
