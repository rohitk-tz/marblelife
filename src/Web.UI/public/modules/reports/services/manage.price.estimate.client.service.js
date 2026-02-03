(function () {
    'use strict';
    angular.module(ReportsConfiguration.moduleName).service("ManagePriceEstimateService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/priceEstimate";

        function getPriceEstimateCollection(model) {
            return httpWrapper.post({ url: baseUrl+ "/getPriceEstimateCollection", data: model });
        }
        function getPriceEstimate(model) {
            return httpWrapper.post({ url: baseUrl + "/getPriceEstimate", data: model });
        }
        function saveBulkCorporatePrice(model) {
            return httpWrapper.post({ url: baseUrl + "/saveBulkCorporatePrice", data: model });
        }
        function bulkUpdateCorporatePrice(model) {
            return httpWrapper.post({ url: baseUrl + "/bulkUpdateCorporatePrice", data: model });
        }
        function savePriceEstimateFranchiseeWise(model) {
            return httpWrapper.post({ url: baseUrl + "/savePriceEstimateFranchiseeWise", data: model });
        }
        function bulkUpdatePriceEstimate(model) {
            return httpWrapper.post({ url: baseUrl + "/bulkUpdatePriceEstimate", data: model });
        }
        function getCategoryCollection() {
            return httpWrapper.get({ url: "/application/dropdown/GetServiceTagCategories" });
        }

        function getPriceEstimateForFranchisee(model) {
            return httpWrapper.post({ url: baseUrl + "/getPriceEstimateCollectionPerFranchisee", data: model });
        }
        function getShiftCharges() {
            return httpWrapper.post({ url: baseUrl + "/getShiftCharges"});
        }
        function saveShiftCharges(model) {
            return httpWrapper.post({ url: baseUrl + "/saveShiftCharges", data:model});
        }
        function getReplacementCharges() {
            return httpWrapper.post({ url: baseUrl + "/getReplacementCharges" });
        }
        function saveReplacementCharges(model) {
            return httpWrapper.post({ url: baseUrl + "/saveReplacementCharges", data: model });
        }
        function getMaintenanceCharges() {
            return httpWrapper.post({ url: baseUrl + "/getMaintenanceCharges" });
        }
        function saveMaintenanceCharges(model) {
            return httpWrapper.post({ url: baseUrl + "/saveMaintenanceCharges", data: model });
        }
        function getFloorGrindingAdjustment() {
            return httpWrapper.post({ url: baseUrl + "/getFloorGrindingAdjustment" });
        }
        function saveFloorGrindingAdjustmentNote(model) {
            return httpWrapper.post({ url: baseUrl + "/saveFloorGrindingAdjustmentNote", data: model });
        }
        function getSeoHistry(model) {
            return httpWrapper.post({ url: baseUrl + "/getSeoHistry", data: model });
        }

        function saveSeoNotes(model) {
            return httpWrapper.post({ url: baseUrl + "/saveSeoNotes", data: model });
        }
        function downloadPriceEstimateData(query) {
            return httpWrapper.getFileByPost({ url: "/priceEstimate/download", data: query });
        }
        function saveFile(model) {
            return httpWrapper.post({ url: baseUrl + "/file/upload", data: model });
        }
        function getPriceEstimateUploadList(query) {
            return httpWrapper.get({
                url: baseUrl + "/list?&filter.statusId=" + query.statusId
                    + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.sortingColumn=" + query.sort.propName
                    + "&filter.sortingOrder=" + query.sort.order + "&filter.text=" + query.text
            });
        }
        function getPriceEstimateDataUploadStatus() {
            return httpWrapper.get({ url: "/application/dropdown/GetSalesDataUploadStatus" });
        }
        function saveNotes(query) {
            return httpWrapper.post({ url: baseUrl + "/save/serviceNotes", data: query });
        }
        function getNotes(query) {
            return httpWrapper.post({ url: baseUrl + "/get/serviceNotes", data: query });
        }
        return {
            getPriceEstimateCollection: getPriceEstimateCollection,
            getPriceEstimate: getPriceEstimate,
            saveBulkCorporatePrice: saveBulkCorporatePrice,
            bulkUpdateCorporatePrice: bulkUpdateCorporatePrice,
            savePriceEstimateFranchiseeWise: savePriceEstimateFranchiseeWise,
            getCategoryCollection: getCategoryCollection,
            bulkUpdatePriceEstimate: bulkUpdatePriceEstimate,
            getPriceEstimateForFranchisee: getPriceEstimateForFranchisee,
            getShiftCharges: getShiftCharges,
            saveShiftCharges: saveShiftCharges,
            getReplacementCharges: getReplacementCharges,
            saveReplacementCharges: saveReplacementCharges,
            getMaintenanceCharges: getMaintenanceCharges,
            saveMaintenanceCharges: saveMaintenanceCharges,
            getFloorGrindingAdjustment: getFloorGrindingAdjustment,
            saveFloorGrindingAdjustmentNote: saveFloorGrindingAdjustmentNote,
            getSeoHistry: getSeoHistry,
            saveSeoNotes: saveSeoNotes,
            downloadPriceEstimateData: downloadPriceEstimateData,
            saveFile: saveFile,
            getPriceEstimateUploadList: getPriceEstimateUploadList,
            getPriceEstimateDataUploadStatus: getPriceEstimateDataUploadStatus,
            saveNotes: saveNotes,
            getNotes: getNotes
        };
    }]);
})();
