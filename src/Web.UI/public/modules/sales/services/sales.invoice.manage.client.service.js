(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).service("SalesInvoiceService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/sales/salesInvoice"


        function downloadInvoiceData(query) {
            return httpWrapper.getFileByPost({ url: baseUrl + "/download", data: query });
        }

        function uploadSalesFile(model) {
            return httpWrapper.post({ url: baseUrl + "/upload", data: model });
        }

        return {
            downloadInvoiceData: downloadInvoiceData,
            uploadSalesFile: uploadSalesFile
        };
    }]);
})();
