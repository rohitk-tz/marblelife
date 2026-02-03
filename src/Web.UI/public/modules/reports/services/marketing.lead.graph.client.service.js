(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("MarketingLeadGraphService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/marketingLead";

        function getPhoneVsWebReport(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/phone/vs/web", data: query });
        }

        function getBusVsPhoneReport(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/bus/vs/phone", data: query });
        }

        function getWebLocalVsNationalReport(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/local/vs/national", data: query });
        }

        function getSpamVsPhoneReport(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/spam/vs/phone", data: query });
        }

        function getWeeklyPhoneReport(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/phone/weekly", data: query });
        }

        function getDailyPhoneReport(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/phone/daily", data: query });
        }
        function getSeasonalLeadReport(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/lead", data: query });
        }

        function getLocalSitePerformanceReport(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/local/performance", data: query });
        }
        function getChartOptions() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 0,
                categoryField: "date",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv",
                    useGraphSettings: true
                },
                pathToImages: 'https://www.amcharts.com/lib/3/images/',
                data: [],
                synchronizeGrid: true,
                valueAxes: [{
                    id: "v1",
                    //axisColor: "#FF0000",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    unit: "%",
                    title: "PHONE CALLS AS A PERCENT OF TOTAL LEADS RECEIVED (PHONE & WEB)"
                }, {
                    id: "v3",
                    axisColor: "#0000FF",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "right",
                    title: "NUMBER OF PHONE CALLS RECEIVED"
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    hideBulletsCount: 20,
                    lineThickness: 2,
                    title: "% Phone (National)",
                    valueField: "national",
                    fillAlphas: 0
                }, {
                    valueAxis: "v1",
                    lineColor: "#63ba2d",
                    bullet: "square",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Phone (Local)",
                    valueField: "local",
                    fillAlphas: 0
                }, {
                    id: "g1",
                    valueAxis: "v3",
                    lineColor: "#0000FF",
                    fillColors: "#0000FF",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    clustered: false,
                    title: "Total Leads(Phone+Web)",
                    valueField: "total",
                    columnWidth: 0.8,
                }],
                chartScrollbar: {},
                chartCursor: {
                    cursorPosition: "pointer"
                },

                categoryAxis: {
                    parseDates: true,
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month",
                    minPeriod: "MM",
                },

                export: {
                    enabled: true,
                    position: "bottom-right"
                }
            }
        }

        function getChartOptionsForBusReview() {
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
                    //axisColor: "#FF0000",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    unit: "%",
                    title: "% OF CALLS GENERATED BY REVIEWS"
                }, {
                    id: "v3",
                    axisColor: "#0000FF",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "right",
                    title: "NUMBER OF REVIEW PAGE GENERATED LEADS"
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Bus Dir. (National)",
                    valueField: "national",
                    fillAlphas: 0
                }, {
                    valueAxis: "v1",
                    lineColor: "#63ba2d",
                    bullet: "square",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Bus Dir. (Local)",
                    valueField: "local",
                    fillAlphas: 0
                },
                {
                    id: "g1",
                    valueAxis: "v3",
                    lineColor: "#0000FF",
                    fillColors: "#0000FF",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total call Leads",
                    valueField: "total",
                    columnWidth: 0.8,
                }],
                chartScrollbar: {},
                chartCursor: {
                    cursorPosition: "pointer"
                },

                categoryAxis: {
                    parseDates: true,
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month",
                    minPeriod: "MM",
                },

                export: {
                    enabled: true,
                    position: "bottom-right"
                }
            }
        }

        function getChartOptionsForWebReview() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 0,
                categoryField: "date",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv2",
                    useGraphSettings: true
                },
                pathToImages: 'https://www.amcharts.com/lib/3/images/',
                data: [],
                synchronizeGrid: true,
                valueAxes: [{
                    id: "v1",
                    //axisColor: "#FF0000",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    unit: "%",
                    title: "LOCAL-WEB LEADS VERSUS TOTAL WEB LEADS (NATIONAL & LOCAL)"
                }, {
                    id: "v3",
                    axisColor: "#0000FF",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "right",
                    title: "NUMBER OF TOTAL WEB LEADS"
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    bullet: "round",
                    title: "% of Web (National)",
                    valueField: "national",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    fillAlphas: 0
                }, {
                    valueAxis: "v1",
                    lineColor: "#63ba2d",
                    bullet: "square",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% of Web (Local)",
                    valueField: "local",
                    fillAlphas: 0
                },
                {
                    id: "g1",
                    valueAxis: "v3",
                    lineColor: "#0000FF",
                    fillColors: "#0000FF",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total Web Leads",
                    valueField: "total",
                    columnWidth: 0.8,
                }],
                chartScrollbar: {},
                chartCursor: {
                    cursorPosition: "pointer"
                },

                categoryAxis: {
                    parseDates: true,
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month",
                    minPeriod: "MM",
                },

                export: {
                    enabled: true,
                    position: "bottom-right"
                }
            }
        }

        function getChartOptionsForSpam() {
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
                    //axisColor: "#FF0000",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    unit: "%",
                    title: " % AUTODIALER REPRESENT OF ALL CALL VOLUME"
                }, {
                    id: "v3",
                    axisColor: "#0000FF",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "right",
                    title: "NUMBER OF AUTODIALER CALLS SCREENED"
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Spam (National)",
                    valueField: "national",
                    fillAlphas: 0
                }, {
                    valueAxis: "v1",
                    lineColor: "#63ba2d",
                    bullet: "square",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Spam (Local)",
                    valueField: "local",
                    fillAlphas: 0
                },
                   {
                       id: "g1",
                       valueAxis: "v3",
                       lineColor: "#0000FF",
                       fillColors: "#0000FF",
                       fillAlphas: 0.8,
                       lineAlpha: 0.2,
                       type: "column",
                       title: "Total AutoDialer",
                       valueField: "total",
                       columnWidth: 0.8,
                   }],
                chartScrollbar: {},
                chartCursor: {
                    cursorPosition: "pointer"
                },

                categoryAxis: {
                    parseDates: true,
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month",
                    minPeriod: "MM",
                },

                export: {
                    enabled: true,
                    position: "bottom-right"
                }
            }
        }

        function getWeeklyLeadChartData() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 1,
                categoryField: "date",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv4",
                    useGraphSettings: true
                },
                pathToImages: 'https://www.amcharts.com/lib/3/images/',
                data: [],
                synchronizeGrid: true,
                valueAxes: [{
                    id: "v1",
                    //axisColor: "#FF0000",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    unit: "%",
                    title: "% OF CALLS COMING RECEIVED EACH DAY OF THE WEEK"
                }, {
                    id: "v3",
                    axisColor: "#0000FF",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "right",
                    title:"NUMBER OF CALLS RECEIVED EACH DAY OF THE WEEK"
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    balloonText: "[[dateString]], [[dayOfWeek]]: [[national]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Phone Lead (National)",
                    valueField: "national",
                    fillAlphas: 0
                }, {
                    valueAxis: "v1",
                    lineColor: "#63ba2d",
                    balloonText: "[[dateString]], [[dayOfWeek]]: [[local]]",
                    bullet: "square",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Phone Lead (Local)",
                    valueField: "local",
                    fillAlphas: 0
                },
                {
                    id: "g1",
                    valueAxis: "v3",
                    balloonText: "[[dateString]], [[dayOfWeek]]: [[total]]",
                    lineColor: "#0000FF",
                    fillColors: "#0000FF",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    clustered: false,
                    title: "Total call Leads",
                    valueField: "total",
                    columnWidth: 0.8,
                    autoColor: true
                }],
                chartScrollbar: {},
                chartCursor: {
                    cursorPosition: "pointer"
                },

                categoryAxis: {
                    parseDates: true,
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Date",
                },
                export: {
                    enabled: true,
                    position: "bottom-right"
                }
            }
        }

        function getDailyLeadChartData() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 0,
                categoryField: "date",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv5",
                    useGraphSettings: true
                },
                pathToImages: 'https://www.amcharts.com/lib/3/images/',
                data: [],
                synchronizeGrid: true,
                valueAxes: [{
                    id: "v1",
                    //axisColor: "#FF0000",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    unit: "%",
                    title: "% CALLS RECEIVED BY HOUR OF DAY"
                }, {
                    id: "v3",
                    axisColor: "#0000FF",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "right",
                    title: "NUMBER OF CALLS RECEIVED BY HOUR OF DAY"
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    balloonText: "<span style='font-size:15px;'>[[lastYearDateString]]: <b>[[national]]</b></span>",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Phone Lead (National)",
                    valueField: "national",
                    fillAlphas: 0,

                }, {
                    valueAxis: "v2",
                    lineColor: "#63ba2d",
                    balloonText: "<span style='font-size:15px;'>[[lastYearDateString]]: <b>[[local]]</b></span>",
                    bullet: "square",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% Phone Lead (Local)",
                    valueField: "local",
                    fillAlphas: 0
                },
                {
                    id: "g4",
                    valueAxis: "v3",
                    lineColor: "#0000FF",
                    fillColors: "#0000FF",
                    fillAlphas: 0.8,
                    type: "column",
                    title: "Total Call Leads",
                    valueField: "total",
                    clustered: false,
                    columnWidth: 0.8,
                    autoColor: true,
                    balloonText: "<span style='font-size:15px;'>[[lastYearDateString]]: <b>[[total]]</b></span>",
                }],
                chartScrollbar: {},
                chartCursor: {
                    cursorPosition: "pointer"
                },

                categoryAxis: {
                    parseDates: true,
                    minPeriod: "hh",
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Time",
                },

                export: {
                    enabled: true,
                    position: "bottom-right"
                }
            }
        }


        function getSeasonalLeadChartData() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 1,
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
                    title: "TOTAL WEB & CALL LEADS"
                }, {
                    id: "v2",
                    stackType: "regular",
                    axisColor: "#0000FF",
                    position: "right",
                    axisAlpha: 0.3,
                    gridAlpha: 0
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    balloonText: "Total : [[dateString]], [[localCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "Total(Last Year)",
                    valueField: "localCount",
                    fillAlphas: 0
                }, {
                    valueAxis: "v1",
                    lineColor: "#63ba2d",
                    balloonText: "Total : ([[lastYearDateString]]), [[totalCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "Average(Last 3 Years)",
                    valueField: "totalCount",
                    fillAlphas: 0
                }, {
                    id: "g1",
                    valueAxis: "v2",
                    lineColor: "#0000FF",
                    fillColors: "#0000FF",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total Web Leads",
                    valueField: "webLeadCount",
                    columnWidth: 0.8,
                }, {
                    id: "g2",
                    valueAxis: "v2",
                    lineColor: "#800000",
                    fillColors: "#800000",
                    fillAlphas: 0.9,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total AutoDialer",
                    valueField: "autoDialerCount",
                    columnWidth: 0.8,
                }, {
                    id: "g3",
                    valueAxis: "v2",
                    lineColor: "#7FFFD4",
                    fillColors: "#7FFFD4",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total Print Media",
                    valueField: "printMediaCount",
                    columnWidth: 0.8,
                }, {
                    id: "g4",
                    valueAxis: "v2",
                    lineColor: "#c1c124",
                    fillColors: "#c1c124",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total Business Directories",
                    valueField: "businessDirectoriesCount",
                    columnWidth: 0.8,
                }, {
                    id: "g5",
                    valueAxis: "v2",
                    lineColor: "#248ec1",
                    fillColors: "#248ec1",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total Phone-Web-Local",
                    valueField: "webLocalCount",
                    columnWidth: 0.8,
                }, {
                    id: "g6",
                    valueAxis: "v2",
                    lineColor: "#a824c1",
                    fillColors: "#a824c1",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total Phone-Web-National",
                    valueField: "webNationalCount",
                    columnWidth: 0.8,
                }, {
                    id: "g7",
                    valueAxis: "v2",
                    lineColor: "#bdb6ac",
                    fillColors: "#bdb6ac",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total WRAP-VAN",
                    valueField: "vanCount",
                    columnWidth: 0.8,
                }],
                categoryAxis: {
                    parseDates: true,
                    gridPosition: "start",
                    minPeriod: "MM",
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month",
                },

                export: {
                    enabled: true,
                    position: "bottom-right"
                }
            }
        }

        function getChartOptionsForCall() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 1,
                categoryField: "date",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv7",
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
                    unit: "%",
                    title: "PERCENT OF CALLS ANSWERED (OVER 2 MIN)"
                }, {
                    id: "v2",
                    stackType: "regular",
                    axisColor: "#0000FF",
                    position: "right",
                    axisAlpha: 0.3,
                    gridAlpha: 0,
                    title: "NUMBER OF CALLS RECEIVED (OVER AND UNDER 2MIN)"
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    balloonText: "Total : ([[dateString]]), [[totalCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: " % of calls over-2-min (National)",
                    valueField: "totalCount",
                    fillAlphas: 0
                }, {
                    valueAxis: "v1",
                    lineColor: "#32CD32",
                    balloonText: "Total : [[dateString]], [[localCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "% of calls over-2-min (Locally)",
                    valueField: "localCount",
                    fillAlphas: 0
                }, {
                    id: "g1",
                    valueAxis: "v2",
                    lineColor: "#0000FF",
                    fillColors: "#0000FF",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Calls over 2 min",
                    valueField: "callOver2min",
                    columnWidth: 0.8,
                }, {
                    id: "g2",
                    valueAxis: "v2",
                    lineColor: "#A9A9A9",
                    fillColors: "#A9A9A9",
                    fillAlphas: 0.9,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Calls under 2 min",
                    valueField: "callUnder2min",
                    columnWidth: 0.8,
                }],
                categoryAxis: {
                    parseDates: true,
                    gridPosition: "start",
                    minPeriod: "MM",
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month",
                },

                export: {
                    enabled: true,
                    position: "bottom-right",

                }
            }
        }
        function getLocalPerformanceLeadChartData() {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 1,
                categoryField: "date",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv10",
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
                    title: "TOTAL WEB EMAIL & WEB CALL LEADS FROM LOCAL SITE ONLY"
                }, {
                    id: "v2",
                    stackType: "regular",
                    axisColor: "#0000FF",
                    position: "right",
                    axisAlpha: 0.3,
                    gridAlpha: 0
                    
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    balloonText: "Total : [[dateString]], [[localCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: "Total(Last Year)",
                    valueField: "localCount",
                    fillAlphas: 0
                }, {
                    valueAxis: "v1",
                    lineColor: "#63ba2d",
                    balloonText: "Total : ([[lastYearDateString]]), [[totalCount]]",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    lineThickness: 2,
                    hideBulletsCount: 20,
                    title: " Average(Last 3 Years)",
                    valueField: "totalCount",
                    fillAlphas: 0
                }, {
                    id: "g1",
                    valueAxis: "v2",
                    lineColor: "#248ec1",
                    fillColors: "#248ec1",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Total Phone-Web-Local",
                    valueField: "webLocalCount",
                    columnWidth: 0.8,
                }, {
                    id: "g2",
                    valueAxis: "v2",
                    lineColor: "#A9A9A9",
                    fillColors: "#A9A9A9",
                    fillAlphas: 0.9,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "Difference(Adjusted and Actual values)",
                    valueField: "phoneWebLocalCount",
                    columnWidth: 0.8,
                }, {
                    id: "g3",
                    valueAxis: "v2",
                    lineColor: "#c1c124",
                    fillColors: "#c1c124",
                    fillAlphas: 0.8,
                    lineAlpha: 0.2,
                    type: "column",
                    title: "MARBLELIFE LOCAL GOOGLE PPC",
                    valueField: "googlePPCCount",
                    columnWidth: 0.8,
                }],
                categoryAxis: {
                    parseDates: true,
                    gridPosition: "start",
                    minPeriod: "MM",
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month",
                },

                export: {
                    enabled: true,
                    position: "bottom-right",

                }
            }
        }
        function getSummary(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/summary", data: query });
        }

        function getAdjustedSummary(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/summary/adjusted", data: query });
        }

        function getManagementDataChartDataWithoutFranchisee(isVisible) {
            return {
                type: "serial",
                theme: 'light',
                startDuration: 0,
                categoryField: "date",
                rotate: false,
                legend: {
                    enabled: true,
                    divId: "legenddiv11",
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
                    unit: "%",
                }, {
                    id: "v2",
                    stackType: "regular",
                    axisColor: "#0000FF",
                    position: "right",
                    axisAlpha: 0.3,
                    gridAlpha: 0,
                }],
                graphs: [{
                    type: "line",
                    title: "MULTI-PERSON",
                    valueField: "frontOfficeCount",
                    lineColor: "darkgreen",
                    dashLength: 4,
                    lineThickness: 4
                },
     {
         type: "line",
         hidden: false,
         title: "Office Person",
         valueField: "officePersonCount",
         lineColor: "lightgreen",
         dashLength: 4,
         lineThickness: 4
     },
     {
         type: "line",
         title: "System Wide",
         valueField: "nationalCount",
         lineThickness: 6,
         lineColor: "blue",
     },
     {
         type: "line",
         hidden: false,
         title: "Responds Next Day",
         valueField: "responseNextDayCount",
         lineColor: "red",
         dashLength: 4,
         lineThickness: 4
     },
     {
         type: "line",
         hidden: false,
         title: "Local Count",
         valueField: "localCount",
         lineThickness: 6,
         lineColor: "#a824c1",
     },
     {
         type: "line",
         title: "Respond When Available",
         valueField: "responseWhenAvailableCount",
         lineColor: "#ffad60",
         dashLength: 4,
         lineThickness: 4
     }],
                categoryAxis: {
                    parseDates: true,
                    gridPosition: "start",
                    minPeriod: "MM",
                    axisColor: "#808080",
                    minorGridEnabled: true,
                    title: "Month",
                },

                export: {
                    enabled: true,
                    position: "bottom-right",

                }
            }
        }



        function getChartOptionsForCallGraph(query) {
            return httpWrapper.post({ url: baseUrl + "/graph/call", data: query });
        }
        return {
            getChartOptions: getChartOptions,
            getPhoneVsWebReport: getPhoneVsWebReport,
            getChartOptionsForBusReview: getChartOptionsForBusReview,
            getBusVsPhoneReport: getBusVsPhoneReport,
            getChartOptionsForWebReview: getChartOptionsForWebReview,
            getWebLocalVsNationalReport: getWebLocalVsNationalReport,
            getChartOptionsForSpam: getChartOptionsForSpam,
            getSpamVsPhoneReport: getSpamVsPhoneReport,
            getSummary: getSummary,
            getWeeklyLeadChartData: getWeeklyLeadChartData,
            getWeeklyPhoneReport: getWeeklyPhoneReport,
            getDailyLeadChartData: getDailyLeadChartData,
            getDailyPhoneReport: getDailyPhoneReport,
            getSeasonalLeadReport: getSeasonalLeadReport,
            getSeasonalLeadChartData: getSeasonalLeadChartData,
            getAdjustedSummary: getAdjustedSummary,
            getChartOptionsForCall: getChartOptionsForCall,
            getChartOptionsForCallGraph: getChartOptionsForCallGraph,
            getLocalPerformanceLeadChartData: getLocalPerformanceLeadChartData,
            getLocalSitePerformanceReport: getLocalSitePerformanceReport,
            getManagementDataChartDataWithoutFranchisee: getManagementDataChartDataWithoutFranchisee,

        };
    }]);
})();
