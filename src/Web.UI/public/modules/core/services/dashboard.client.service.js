(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).service("DashboardService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/dashboard";


        function getFranchiseeDirectoryList() {
            return httpWrapper.get({ url: baseUrl + "/franchisee/Directory"})
        }

        function getRecentInvoices(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/invoice/list" })
        }

        function getSalesSummary(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/sales/summary" })
        }

        function getSalesRepLeaderboard(franchiseeId, startDate, endDate) {
            return httpWrapper.get({ url: baseUrl + "/leaderboard/" + franchiseeId + "/salesrep?startDate=" + startDate + "&endDate=" + endDate });
        }

        function getFranchiseeLeaderboard(startDate, endDate, franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/leaderboard/" + franchiseeId + "/franchisee?startDate=" + startDate + "&endDate=" + endDate });
        }

        function getRevenueDetails(query) {
            return httpWrapper.get({ url: baseUrl + "/revenue/" + query.franchiseeId + "?startDate=" + query.startDate + "&endDate=" + query.endDate });
        }

        function getRevenueForService(query) {
            return httpWrapper.get({ url: baseUrl + "/service/revenue/" + query.franchiseeId + "?startDate=" + query.startDate + "&endDate=" + query.endDate });
        }

        function getLastTwentyYearCollection() {
            return httpWrapper.get({ url: "/application/dropdown/GetLastTwentyYears" });
        }

        function getPendindUploadList(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/pending/upload/list" });
        }

        function getUnpaidInvoices(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/unpaid/invoice/list" })
        }

        function getAnnualUploadResponse(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/annual/upload/response" })
        }
        function getAnnualUploadResponse(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/annual/upload/response" })
        }
        function getPieChartOptions() {
            return {
                type: "pie",
                theme: 'light',
                startDuration: 0,
                categoryField: "year",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv",
                },
                pathToImages: 'https://www.amcharts.com/lib/3/images/',

                innerRadius: "0",
                labelsEnabled: false,
                autoMargins: false,
                marginTop: 0,
                marginBottom: 0,
                marginLeft: 0,
                marginRight: 0,
                pullOutRadius: 0,
                data: [],
                categoryAxis: {
                    gridPosition: "start",
                    parseDates: false
                },
                valueAxes: [{
                    position: "top",
                    title: "Revenue"
                }],

                valueField: "income",
                titleField: "category",
                colorField: "colorCode",
                export: {
                    "enabled": true
                }
            }
        }

        function getCustomerCount(startDate, endDate, franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/customer/count/" + franchiseeId + "?startDate=" + startDate + "&endDate=" + endDate })
        }
        function getMonthCollection() {
            return httpWrapper.get({ url: "/application/dropdown/GetMonths" });
        }
        function getMonthNames() {
            return httpWrapper.get({ url: "/application/dropdown/GetMonthNames" });
        }
        function getDocuments(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/documents" })
        }
        function getPendingDocuments(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/documents/pending" })
        }
        function getFranchiseeDirectoryListForSuperAdmin(franchiseeName)
        {
            return httpWrapper.get({ url: baseUrl + "/franchisee/Directory/"+franchiseeName+"/superAdmin"})

        }
        function redirectionToBulkPhotoUpload() {
            return httpWrapper.get({ url: baseUrl + "/franchisee/RedirectionToBulkPhotoUpload" })
        }

        return {
            getFranchiseeDirectoryList: getFranchiseeDirectoryList,
            getSalesSummary: getSalesSummary,
            getRecentInvoices: getRecentInvoices,
            getSalesRepLeaderboard: getSalesRepLeaderboard,
            getFranchiseeLeaderboard: getFranchiseeLeaderboard,
            getRevenueDetails: getRevenueDetails,
            getPieChartOptions: getPieChartOptions,
            getLastTwentyYearCollection: getLastTwentyYearCollection,
            getCustomerCount: getCustomerCount,
            getPendindUploadList: getPendindUploadList,
            getUnpaidInvoices: getUnpaidInvoices,
            getMonthCollection: getMonthCollection,
            getRevenueForService: getRevenueForService,
            getMonthNames: getMonthNames,
            getAnnualUploadResponse: getAnnualUploadResponse,
            getDocuments: getDocuments,
            getPendingDocuments: getPendingDocuments,
            getFranchiseeDirectoryListForSuperAdmin: getFranchiseeDirectoryListForSuperAdmin,
            redirectionToBulkPhotoUpload: redirectionToBulkPhotoUpload
        };
    }]);
})();