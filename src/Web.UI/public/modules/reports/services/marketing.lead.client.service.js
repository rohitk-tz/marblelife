(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("MarketingLeadService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/marketingLead";

        function getCallDetails(query) {
            return httpWrapper.get({
                url: baseUrl + "/marketingLead?filter.text=" + query.text + "&filter.franchiseeId=" + query.franchiseeId
                    + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.convertedLead=" + query.convertedLead
                    + "&filter.startDate=" + query.startDate + "&filter.endDate=" + query.endDate + "&filter.tagId=" + query.tagId
                    + "&filter.callTypeId=" + query.callTypeId + "&filter.mappedFranchisee=" + query.mappedFranchisee
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order
                    + "&filter.categoryIds=" + query.categoryIds + "&filter.callerId=" + query.callerId + "&filter.marketingLeadId=" + query.marketingLeadId
                    + "&filter.transferToNumber=" + query.transferToNumber + "&filter.office=" + query.office
            });
        }

        function getCategoryList() {
            return httpWrapper.get({ url: "/application/dropdown/GetPhoneLabelCategory" });
        }

        function getCallTypes() {
            return httpWrapper.get({ url: "/application/dropdown/GetCallType" });
        }

        function downloadCallDetails(query) {
            return httpWrapper.getFileByPost({ url: "/marketingLead/download", data: query });
        }

        function getcallDetailReport(query) {
            return httpWrapper.post({ url: baseUrl + "/report", data: query });
        }

        function downloadcallDetailReport(query) {
            return httpWrapper.getFileByPost({ url: "/marketingLead/report/download", data: query });
        }

        function getRoutingNumberList() {
            return httpWrapper.get({ url: "/application/dropdown/GetRoutingNumberList" });
        }

        function getPhoneVsWebReport(query) {
            return httpWrapper.post({ url: baseUrl + "/report/phone/vs/web", data: query });
        }

        function getBusVsPhoneReport(query) {
            return httpWrapper.post({ url: baseUrl + "/report/bus/vs/phone", data: query });
        }

        function getcallDetailReportAdjustedData(query) {
            return httpWrapper.post({ url: baseUrl + "/report/rawdata", data: query });
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
                    axisColor: "#FF0000",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                }, {
                    id: "v2",
                    axisColor: "#63ba2d",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "right",
                },
                {
                    id: "v3",
                    axisColor: "#0000ff",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
                    offset: 50,
                }],
                graphs: [{
                    valueAxis: "v1",
                    lineColor: "#FF0000",
                    bullet: "round",
                    bulletBorderThickness: 1,
                    hideBulletsCount: 20,
                    title: "Phone Lead",
                    valueField: "phone",
                    fillAlphas: 0
                }, {
                    valueAxis: "v2",
                    lineColor: "#63ba2d",
                    bullet: "square",
                    bulletBorderThickness: 1,
                    hideBulletsCount: 20,
                    title: "Web Lead",
                    valueField: "web",
                    fillAlphas: 0
                },
                {
                    valueAxis: "v3",
                    lineColor: "#0000ff",
                    bullet: "triangleUp",
                    bulletBorderThickness: 1,
                    hideBulletsCount: 20,
                    title: "Total",
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
                    axisColor: "#FF0000",
                    axisThickness: 2,
                    axisAlpha: 1,
                    position: "left",
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
                    title: "Phone Lead",
                    valueField: "phone",
                    fillAlphas: 0
                }, {
                    valueAxis: "v2",
                    lineColor: "#63ba2d",
                    bullet: "square",
                    bulletBorderThickness: 1,
                    hideBulletsCount: 20,
                    title: "Bus Review",
                    valueField: "busReview",
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
        function getcallDetailReportRawData(query) {
            return httpWrapper.post({ url: baseUrl + "/report/rawdata", data: query });
        }


        function getManagementVsLocalReport(query) {
            return httpWrapper.post({ url: baseUrl + "/management/vs/local/report", data: query });
        }

        function getManagementReport(query) {
            return httpWrapper.post({ url: baseUrl + "/management/report", data: query });
        }
        function getPerformanceHistryCollection(model) {
            return httpWrapper.post({ url: baseUrl + "/performance/histry", data: model });
        }
        function getLeadPerformanceNationalWise(model) {
            return httpWrapper.post({ url: baseUrl + "/performance/report/nationalLevel", data: model });
        }
        function getLeadPerformanceFranchiseeWise(model) {
            return httpWrapper.post({ url: baseUrl + "/performance/report/localLevel", data: model });
        }
        function getPPCAndSeoNational(model) {
            return httpWrapper.post({ url: baseUrl + "/seo/ppr/report/national", data: model });
        }
        function getPPCAndSeoLocal(model) {
            return httpWrapper.post({ url: baseUrl + "/seo/ppr/report/local", data: model });
        }
        function getHomeAdvisorReport(model) {
            return httpWrapper.post({ url: baseUrl + "/home/Advisor", data: model });
        }
        function getLeadFlowDetails(filter) {
            return httpWrapper.post({ url: baseUrl + "/LeadFlow", data: filter });
        }

        function downloadLeadFlow(query) {
            return httpWrapper.getFileByPost({ url: "/marketingLead/downloadLeadFlow", data: query });
        }
        function getFranchiseePhoneCallsList(query) {
            return httpWrapper.post({ url: "/marketingLead/getFranchiseePhoneCalls", data: query });
        }

        function editFranchiseePhoneCallsList(query) {
            return httpWrapper.post({ url: "/marketingLead/editFranchiseePhoneCalls", data: query });
        }

        function generateInvoice(query) {
            return httpWrapper.post({ url: "/marketingLead/generatePhoneCallInvoice", data: query });
        }

        function editFranchiseePhoneCallsListByBulk(query) {
            return httpWrapper.post({ url: "/marketingLead/editFranchiseePhoneCallsByBulk", data: query });
        }

        function getFranchiseePhoneCallsBulkList(query) {
            return httpWrapper.post({ url: "/marketingLead/getFranchiseePhoneCallsBulkList", data: query });
        }

        function saveFranchiseePhoneCallsByBulk(query) {
            return httpWrapper.post({ url: "/marketingLead/saveFranchiseePhoneCallsByBulk", data: query });
        }

        function saveCallDetailsReportNotes(query) {
            return httpWrapper.post({ url: "/marketingLead/saveCallDetailsReportNotes", data: query });
        }

        function getCallDetailsReportNotes(query) {
            return httpWrapper.post({ url: "/marketingLead/getCallDetailsReportNotes", data: query });
        }

        function getAutomationBackUpReport(query) {
            return httpWrapper.post({ url: "/marketingLead/getAutomationBackUpReport", data: query });
        }

        function getFranchiseePhoneCallsListForFranchisee(query) {
            return httpWrapper.post({ url: "/marketingLead/getFranchiseePhoneCallsListForFranchisee", data: query });
        }

        function getOfficeCollection() {
            return httpWrapper.get({ url: "/marketingLead/getOfficeCollection" });
        }

        function getFranchiseeNameValuePairCollection() {
            return httpWrapper.get({ url: "/marketingLead/getFranchiseeNameValuePairCollection" });
        }

        function downloadCallNotesHistoryDetails(query) {
            return httpWrapper.getFileByPost({ url: "/marketingLead/downloadCallNotesHistoryDetails", data: query });
        }

        function editCallDetailsReportNotes(query) {
            return httpWrapper.post({ url: "/marketingLead/editCallDetailsReportNotes", data: query });
        }

        return {
            getCallDetails: getCallDetails,
            getCallTypes: getCallTypes,
            downloadCallDetails: downloadCallDetails,
            getcallDetailReport: getcallDetailReport,
            downloadcallDetailReport: downloadcallDetailReport,
            getRoutingNumberList: getRoutingNumberList,
            getChartOptions: getChartOptions,
            getPhoneVsWebReport: getPhoneVsWebReport,
            getChartOptionsForBusReview: getChartOptionsForBusReview,
            getBusVsPhoneReport: getBusVsPhoneReport,
            getCategoryList: getCategoryList,
            getcallDetailReportAdjustedData: getcallDetailReportAdjustedData,
            getcallDetailReportRawData: getcallDetailReportRawData,
            getManagementVsLocalReport: getManagementVsLocalReport,
            getManagementReport: getManagementReport,
            getPerformanceHistryCollection: getPerformanceHistryCollection,
            getLeadPerformanceNationalWise: getLeadPerformanceNationalWise,
            getLeadPerformanceFranchiseeWise: getLeadPerformanceFranchiseeWise,
            getPPCAndSeoLocal: getPPCAndSeoLocal,
            getPPCAndSeoNational: getPPCAndSeoNational,
            getHomeAdvisorReport: getHomeAdvisorReport,
            getLeadFlowDetails: getLeadFlowDetails,
            downloadLeadFlow: downloadLeadFlow,
            getFranchiseePhoneCallsList: getFranchiseePhoneCallsList,
            editFranchiseePhoneCallsList: editFranchiseePhoneCallsList,
            generateInvoice: generateInvoice,
            editFranchiseePhoneCallsListByBulk: editFranchiseePhoneCallsListByBulk,
            getFranchiseePhoneCallsBulkList: getFranchiseePhoneCallsBulkList,
            saveFranchiseePhoneCallsByBulk: saveFranchiseePhoneCallsByBulk,
            getAutomationBackUpReport: getAutomationBackUpReport,
            getFranchiseePhoneCallsListForFranchisee: getFranchiseePhoneCallsListForFranchisee,
            saveCallDetailsReportNotes: saveCallDetailsReportNotes,
            getCallDetailsReportNotes: getCallDetailsReportNotes,
            getOfficeCollection: getOfficeCollection,
            getFranchiseeNameValuePairCollection: getFranchiseeNameValuePairCollection,
            downloadCallNotesHistoryDetails: downloadCallNotesHistoryDetails,
            editCallDetailsReportNotes: editCallDetailsReportNotes
        };
    }]);
})();
