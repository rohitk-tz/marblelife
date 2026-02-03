(function () {
    'use strict';

    angular.module(SchedulerConfiguration.moduleName).service("GeoCodeService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/scheduler/geoCode"

        function saveFile(model) {
            return httpWrapper.post({ url: baseUrl + "/file/upload", data: model });
        }

        function getGeoList(query) {
            return httpWrapper.get({
                url: baseUrl + "/list?&filter.statusId=" + query.statusId
                 + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.sortingColumn=" + query.sort.propName
            + "&filter.sortingOrder=" + query.sort.order + "&filter.text=" + query.text
            });
        }

        function getZipDataUploadStatus() {
            return httpWrapper.get({ url: "/application/dropdown/GetSalesDataUploadStatus" });
        }

        function downloadAllGeoCode(query) {
            return httpWrapper.getFileByPost({ url: baseUrl + "/DownloadAllGeoCodeFile", data: query });
        }

        function getGeoCode(query) {
            return httpWrapper.post({ url: baseUrl + "/info", data: query });
        }

        function getGeoInfo(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/zip/info/" + franchiseeId });
        }

        function getGeoinfoByZipCode(zipCode, state, franchiseeId, countryId) {
            if (countryId == undefined) countryId = 0;
            return httpWrapper.get({ url: baseUrl + "/get/" + zipCode + "/geoCode/" + state + "/franchiseeId/" + franchiseeId + "/countryId/" + countryId });
        }

        function saveGeoCodeNotes(query) {
            return httpWrapper.post({ url: baseUrl + "/zip/code/notes", data: query });
        }
        function downloadSalesData(fileId) {
            return fileService.getExcel(fileId).then(function (result) {
                var fileName = "SalesData-" + fileId + ".xlsx";
                fileService.downloadFile(result.data, fileName);
            });
        }
        function deleteBatch(batchId) {
            return httpWrapper.post({ url: baseUrl + "/delete/" + batchId });
        }
        function reparseFile(batchId) {
            return httpWrapper.post({ url: baseUrl + "/reparse/geofile/" + batchId });
        }
        return {
            saveFile: saveFile,
            getGeoList: getGeoList,
            getZipDataUploadStatus: getZipDataUploadStatus,
            downloadAllGeoCode: downloadAllGeoCode,
            getGeoCode: getGeoCode,
            getGeoInfo: getGeoInfo,
            getGeoinfoByZipCode: getGeoinfoByZipCode,
            deleteBatch: deleteBatch,
            reparseFile: reparseFile
        };
    }]);
})();