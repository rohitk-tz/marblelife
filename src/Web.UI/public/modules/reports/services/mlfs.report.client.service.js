(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("MlfsReportService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/reports";

        function getConfiguration(query) {
            return httpWrapper.post({ url: baseUrl + "/mlfsReportConfiguration", data: query });
        }

        function saveMLFSConfiguration(query) {
            return httpWrapper.post({ url: baseUrl + "/mlfsReportSave", data: query });
        }

       

        return {
            getConfiguration: getConfiguration,
            saveMLFSConfiguration: saveMLFSConfiguration
        };
    }]);
})();
