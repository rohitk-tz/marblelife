(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("CustomerEmailReportService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/reports";

        function getCustomerEmailReport(query) {
            return httpWrapper.get({
                url: baseUrl + "/customerEmailReport?filter.franchiseeId=" + query.franchiseeId + "&filter.year=" + query.year
                    + "&filter.month=" + query.month
            });
        }

        function getEmailReportChartData(franchiseeId, startDate, endDate) {
            return httpWrapper.get({
                url: baseUrl + "/customerEmailReport/email/" + franchiseeId + "/report?startDate=" + startDate + "&endDate=" + endDate
            });
        }

        function getReviewReportChartData(franchiseeId, startDate, endDate) {
            return httpWrapper.get({
                url: baseUrl + "/customerReviewReport/review/" + franchiseeId + "/report?startDate=" + startDate + "&endDate=" + endDate
            });
        }

        function downloadEmailReport(query) {
            return httpWrapper.getFileByPost({ url: "/customerEmail/report/download", data: query });
        }

        function getReviewCount(franchiseeId) {
            return httpWrapper.get({ url: "/customerEmail/report/getReviewCounts/" + franchiseeId });
        }

        function getLineChartOptions() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 0,
                categoryField: "date",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv1",
                    useGraphSettings: true
                },
                pathToImages: 'https://www.amcharts.com/lib/3/images/',
                data: [],
                synchronizeGrid: true,
                valueAxes: [{
                    id: "v1",
                    axisColor: "#FF0000",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    title: "(%age) Email Coverage"
                }, {
                    id: "v2",
                    axisColor: "#63ba2d",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "right",
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    hideBulletsCount: 20,
                    title: "Franchisee",
                    valueField: "current",
                    fillAlphas: 0
                }, {
                    valueAxis: "v2",
                    lineColor: "#63ba2d",
                    bullet: "square",
                    bulletBorderThickness: 1,
                    hideBulletsCount: 20,
                    title: "All Franchisee",
                    valueField: "total",
                    fillAlphas: 0
                }],
                chartScrollbar: {},
                chartCursor: {
                    cursorPosition: "pointer"
                },

                categoryAxis: {
                    parseDates: true,
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month"
                },

                export: {
                    enabled: true,
                    position: "bottom-right"
                }
            }
        }

        function getLineChartOptionsForReview() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 0,
                categoryField: "date",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv3",
                    useGraphSettings: true
                },
                pathToImages: 'https://www.amcharts.com/lib/3/images/',
                data: [],
                synchronizeGrid: true,
                valueAxes: [{
                    id: "v1",
                    axisColor: "#FF0000",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    title: "(%age) Review Coverage"
                }, {
                    id: "v2",
                    axisColor: "#63ba2d",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "right",
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    hideBulletsCount: 20,
                    title: "Review",
                    valueField: "current",
                    fillAlphas: 0
                }, {
                    valueAxis: "v2",
                    lineColor: "#63ba2d",
                    bullet: "square",
                    bulletBorderThickness: 1,
                    hideBulletsCount: 20,
                    title: "All Review",
                    valueField: "total",
                    fillAlphas: 0
                }],
                chartScrollbar: {},
                chartCursor: {
                    cursorPosition: "pointer"
                },

                categoryAxis: {
                    parseDates: true,
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month"
                },

                export: {
                    enabled: true,
                    position: "bottom-right"
                }
            }
        }

        return {
            getCustomerEmailReport: getCustomerEmailReport,
            getLineChartOptions: getLineChartOptions,
            getEmailReportChartData: getEmailReportChartData,
            downloadEmailReport: downloadEmailReport,
            getLineChartOptionsForReview: getLineChartOptionsForReview,
            getReviewReportChartData: getReviewReportChartData,
            getReviewCount: getReviewCount
        };
    }]);
})();