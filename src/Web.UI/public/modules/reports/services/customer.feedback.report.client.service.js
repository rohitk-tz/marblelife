(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("CustomerFeedbackReportService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/reports";

        function getCustomerFeedbackReport(query) {
            return httpWrapper.get({
                url: baseUrl + "/customerFeedbackReport?filter.text=" + query.text + "&filter.franchiseeId=" + query.franchiseeId
                    + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.startDate=" + query.startDate + "&filter.endDate=" + query.endDate
                    + "&filter.response=" + query.response + "&filter.responseFrom=" + query.responseFrom
                    + "&filter.responseStartDate=" + query.receivedStartDate + "&filter.responseEndDate=" + query.receivedEndDate
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order
            });
        }

        function getFeedbackDetail(responseId) {
            return httpWrapper.get({ url: baseUrl + "/customerFeedbackReport/feedback/get/" + responseId });
        }

        function downloadFeedbackReport(query) {
            return httpWrapper.getFileByPost({ url: "/customerFeedbackReport/download", data: query });
        }

        function manageCustomerFeedbackStatus(isAccept, customerId, id, fromTable) {
            return httpWrapper.post({ url: baseUrl + "/customerFeedbackReport/manageCustomerFeedbackStatus/" + isAccept + "/action/" + customerId + "/action/" + id + "/action/" + fromTable});
        }

        return {
            getCustomerFeedbackReport: getCustomerFeedbackReport,
            getFeedbackDetail: getFeedbackDetail,
            downloadFeedbackReport: downloadFeedbackReport,
            manageCustomerFeedbackStatus: manageCustomerFeedbackStatus
        };
    }]);
})();