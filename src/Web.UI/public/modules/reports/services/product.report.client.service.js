
(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("ProductReportService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/reports";

        function getReportList(query) {
            return httpWrapper.get({
                url: baseUrl + "/productReport?filter.franchiseeId=" + query.franchiseeId + "&filter.paymentDateEnd=" + query.paymentDateEnd
                    + "&filter.paymentDateStart=" + query.paymentDateStart + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.typeIds=" + query.typeIds + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order
            });
        }
        function downloadReport(query) {
            return httpWrapper.getFileByPost({ url: "/prodcut/report/download", data: query });
        }

        function getProductChannels() {
            return httpWrapper.get({ url: "/application/dropdown/GetProductChannel" });
        }

        function getLineChartOptions() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 0,
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv1",
                    useGraphSettings: true,
                    valueWidth: 80,

                },
                pathToImages: 'https://www.amcharts.com/lib/3/images/',
                data: [],
                synchronizeGrid: true,
                valueAxes: [{
                    id: "v1",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    labelOffset: 10
                },
                {
                    id: "v2",
                    stackType: "regular",
                    axisColor: "#0000FF",
                    position: "right",
                    axisAlpha: 0.3,
                    gridAlpha: 0,
                    axisThickness: 2,
                }],
                graphs: [
                ],
                chartScrollbar: {},
                chartCursor: {
                    cursorPosition: "pointer",
                },
                categoryField: "date",
                categoryAxis: {
                    parseDates: true,
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month",
                    minPeriod: "MM",
                },
                export: {
                    enabled: false,
                    //divId: "printdiv",
                    //position: "top-right",
                }
            }
        }

        function getChartData(query) {
            return httpWrapper.post({ url: "/product/channel/report", data: query });
        }

        return {
            getReportList: getReportList,
            downloadReport: downloadReport,
            getProductChannels: getProductChannels,
            getLineChartOptions: getLineChartOptions,
            getChartData: getChartData
        };
    }]);
})();

