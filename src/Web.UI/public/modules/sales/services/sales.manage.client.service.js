(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).service("SalesService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/sales";
        function getSalesDataList(query) {
            return httpWrapper.get({
                url: baseUrl + "/sales?filter.text=" + query.text + "&filter.franchiseeId=" + query.franchiseeId + "&filter.qbInvoiceNumber=" + query.qbInvoiceNumber +
                    "&filter.customerId=" + query.customerId + "&filter.salesDataUploadId=" + query.salesDataUploadId +
                    "&filter.customerName=" + query.customerName + "&filter.marketingClassId=" + query.marketingClassId +
                    "&filter.periodStartDate=" + query.periodStartDate + "&filter.periodEndDate=" + query.periodEndDate +
                    "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.sortingColumn=" +
                    query.sort.propName + "&filter.sortingOrder=" + query.sort.order
            });
        }




        function getInvoiceDetails(invoiceId) {
            return httpWrapper.get({ url: baseUrl + "/sales?invoiceId=" + invoiceId });
        }

        function getBatchSalesData(salesDataUploadId) {
            return httpWrapper.get({ url: baseUrl + "/royalty/report/" + salesDataUploadId });
        }

        function downloadSalesData(query) {
            return httpWrapper.getFileByPost({ url: "/sales/download", data: query });
        }

        function getAccountCreditList(query) {
            return httpWrapper.get({
                url: baseUrl + "/account/credit?filter.text=" + query.text + "&filter.franchiseeId=" + query.franchiseeId + "&filter.qbInvoiceNumber=" + query.qbInvoiceNumber +
                    "&filter.customerName=" + query.customerName + "&filter.qbInvoiceNumber=" + query.qbInvoiceNumber +
                    "&filter.from=" + query.from + "&filter.to=" + query.to +
                    "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.sortingColumn=" +
                    query.sort.propName + "&filter.sortingOrder=" + query.sort.order
            });
        }

        function getFranchiseeAccountCreditList(query) {
            return httpWrapper.get({
                url: baseUrl + "/account/list/credit?filter.text=" + query.text + "&filter.franchiseeId=" + query.franchiseeId +
                    "&filter.month=" + query.month + "&filter.year=" + query.year +
                    "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.sortingColumn=" +
                    query.sort.propName + "&filter.sortingOrder=" + query.sort.order
            });
        }
        function getYears() {
            return httpWrapper.get({ url: "/application/dropdown/GetLastTwentyYears" });
        };

        function getMonths() {
            return httpWrapper.get({ url: "/application/dropdown/GetMonths" });
        };

        function getmarketingClassCollection() {
            return httpWrapper.get({ url: "/application/dropdown/GetMarketingClass" });
        }


        function getSalesFunnelNationalData(query) {
            return httpWrapper.get({
                url: baseUrl + "/funnel/national?filter.franchiseeId=" + query.franchiseeId +
                    "&filter.periodStartDate=" + query.periodStartDate + "&filter.periodEndDate=" + query.periodEndDate
                    + "&filter.propName=" + query.propName + "&filter.order=" + query.order
            });
        }
        function getSalesFunnelLocalData(query) {
            return httpWrapper.get({
                url: baseUrl + "/funnel/local?filter.franchiseeId=" + query.franchiseeId +
                    "&filter.periodStartDate=" + query.periodStartDate + "&filter.periodEndDate=" + query.periodEndDate
                    + "&filter.propName=" + query.propName + "&filter.order=" + query.order
            });
        }
        function downloadMocroFunnelNational(query) {
            return httpWrapper.getFileByPost({ url: "/sales/funnel/national/download", data: query });
        }
        function downloadMocroFunnelLocal(query) {
            return httpWrapper.getFileByPost({ url: "/sales/funnel/local/download", data: query });
        }

        function getSalesFunnelLocalChartData() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 0,
                categoryField: "date",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv6",
                    useGraphSettings: true,
                },
                pathToImages: 'https://www.amcharts.com/lib/3/images/',
                data: [],
                synchronizeGrid: true,
                valueAxes: [{
                    id: "v1",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    stackType: "regular",

                }, {
                    id: "v2",
                    axisColor: "#0000FF",
                    position: "right",
                    axisAlpha: 0.3,
                    gridAlpha: 0,
                    unit: "%",

                }],
                graphs: [{
                    dashLength: 3,
                    valueAxis: "v2",
                    lineColor: "#FF0000",
                    balloonText: "Total : [[lastYearDateString]], [[phoneAnsweredCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Phone answered (Over 2min/total phone)",
                    valueField: "phoneAnsweredCount",
                    fillAlphas: 0,
                    dashLength: 3
                }, {
                    dashLength: 3,
                    valueAxis: "v2",
                    lineColor: "#63ba2d",
                    balloonText: "Total : ([[lastYearDateString]]), [[convertToEstimateCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% convert to Estimate ",
                    valueField: "convertToEstimateCount",
                    fillAlphas: 0,
                    dashLength: 3
                }, {
                    dashLength: 3,
                    valueAxis: "v2",
                    lineColor: "#191970",
                    balloonText: "Total : ([[lastYearDateString]]), [[convertToJobCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    title: "% Convert to Job",
                    valueField: "convertToJobCount",
                    fillAlphas: 0,
                    dashLength: 3,
                    connect: false
                }, {
                    dashLength: 3,
                    valueAxis: "v2",
                    lineColor: "#7B68EE",
                    balloonText: "Total : ([[lastYearDateString]]), [[convertToInvoiceCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Convert to Invoice",
                    valueField: "convertToInvoiceCount",
                    fillAlphas: 0,
                    dashLength: 3
                }, {
                    valueAxis: "v2",
                    lineColor: "green",
                    balloonText: "Total : ([[lastYearDateString]]), [[salesCloseRateCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 12,
                    hideBulletsCount: 20,
                    title: "Sales close Rate",
                    valueField: "salesCloseRateCount",
                    fillAlphas: 0,
                },
                {
                    id: "g1",
                    valueAxis: "v1",
                    lineColor: "#c1c124",
                    fillColors: "#0000ff",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total Jobs",
                    valueField: "totalJobsCount",
                    columnWidth: 0.8,
                    lineThickness: 3
                },
                {
                    id: "g2",
                    valueAxis: "v1",
                    lineColor: "#c1c124",
                    fillColors: "#c1c124",
                    fillAlphas: 0,
                    lineAlpha: 0.8,
                    type: "column",
                    title: "Missed Calls",
                    valueField: "missedCallsCount",
                    columnWidth: 0.8,
                    lineThickness: 3
                }, {
                    id: "g3",
                    valueAxis: "v1",
                    lineColor: "#800000",
                    fillColors: "#800000",
                    fillAlphas: 0,
                    lineAlpha: 0.8,
                    type: "column",
                    title: "Lost Estimate",
                    valueField: "lostEstimateCount",
                    columnWidth: 0.8,
                    lineThickness: 3
                }, {
                    id: "g4",
                    valueAxis: "v1",
                    lineColor: "#7FFFD4",
                    fillColors: "#7FFFD4",
                    fillAlphas: 0,
                    lineAlpha: 0.8,
                    type: "column",
                    title: "Lost Jobs",
                    valueField: "lostJobsCount",
                    columnWidth: 0.8,
                    lineThickness: 3
                }],
                categoryAxis: {
                    parseDates: true,
                    gridPosition: "start",
                    minPeriod: "MM",
                    axisColor: "#808080",
                    minorGridEnabled: false,
                    title: "Month",
                },

                export: {
                    enabled: true,
                    position: "bottom-right",

                }
            }
        }

        function generateSalesFunnelLocalGraphData(query) {
            return httpWrapper.post({ url: "/sales/funnel/local/graph", data: query });
        }

        function getUpdationMarketingClass(filter) {
            return httpWrapper.post({ url: baseUrl + "/updateSalesData", data: filter });
        }

        function downloadInvoiceList(filter) {
            return httpWrapper.getFileByPost({ url: baseUrl + "/updateSalesData/download", data: filter });
        }

        function uploadInvoiceDetailsList(filter) {
            return httpWrapper.post({ url: baseUrl + "/updateSalesData/upload", data: filter });
        }

        function getFileParse(query) {
            return httpWrapper.get({ url: baseUrl + "/getInvoiceParseList?&filter.statusId=" + query.statusId
                    + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.sortingColumn=" + query.sort.propName
                    + "&filter.sortingOrder=" + query.sort.order + "&filter.text=" + query.text
            });
        }
        function getmarketingClassCollectionNewList() {
            return httpWrapper.get({ url: "/application/dropdown/GetMarketingClassNewOrder" });
        }
        return {
            getSalesDataList: getSalesDataList,
            getInvoiceDetails: getInvoiceDetails,
            getBatchSalesData: getBatchSalesData,
            downloadSalesData: downloadSalesData,
            getAccountCreditList: getAccountCreditList,
            getFranchiseeAccountCreditList: getFranchiseeAccountCreditList,
            getYears: getYears,
            getMonths: getMonths,
            getmarketingClassCollection: getmarketingClassCollection,
            getSalesFunnelNationalData: getSalesFunnelNationalData,
            downloadMocroFunnelNational: downloadMocroFunnelNational,
            getSalesFunnelLocalData: getSalesFunnelLocalData,
            downloadMocroFunnelLocal: downloadMocroFunnelLocal,
            getSalesFunnelLocalChartData: getSalesFunnelLocalChartData,
            generateSalesFunnelLocalGraphData: generateSalesFunnelLocalGraphData,
            getUpdationMarketingClass: getUpdationMarketingClass,
            downloadInvoiceList: downloadInvoiceList,
            uploadInvoiceDetailsList: uploadInvoiceDetailsList,
            getFileParse: getFileParse,
            getmarketingClassCollectionNewList: getmarketingClassCollectionNewList
        };
    }]);
})();