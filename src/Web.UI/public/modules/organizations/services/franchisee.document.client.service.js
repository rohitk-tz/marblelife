(function () {
    'use strict';

    angular.module(OrganizationsConfiguration.moduleName).service("FranchiseeDocumentService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {
        var baseUrl = "/organizations";

        function getDocumentList(query) {
            return httpWrapper.get({
                url: baseUrl + "/franchiseeDocument?filter.text=" + query.text + "&filter.franchiseeId=" + query.franchiseeId
                    + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.sortingColumn=" + query.sort.propName
                    + "&filter.sortingOrder=" + query.sort.order + "&filter.isImportant=" + query.isImportant
                    + "&filter.categoryId=" + query.categoryId + "&filter.documentTypeId=" + query.documentTypeId
                    + "&filter.periodStartDate=" + query.periodStartDate + "&filter.periodEndDate=" + query.periodEndDate
            })
        }

        function saveDocuments(model) {
            return httpWrapper.post({ url: baseUrl + "/doc/upload", data: model });
        }

        function deleteDoc(id) {
            return httpWrapper.delete({ url: baseUrl + "/doc/" + id + "/delete" });
        }

        function getDocumentTypeForFranchisee(franchiseeId) {

            return httpWrapper.post({ url: "/application/dropdown/GetDocumentTypeForFranchisee", data: franchiseeId });
        }
        function getDocumentType() {
            return httpWrapper.get({ url: "/application/dropdown/GetDocumentType" });
        }
        function getNationalDocumentType() {
            return httpWrapper.get({ url: "/application/dropdown/GetNationalDocumentType" });
        }
        function GetNationalTypeForFranchisee(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/GetNationalTypeForFranchisee", data: franchiseeId });
        }
        function isExpiryCorrect(model) {
            return httpWrapper.post({ url: baseUrl + "/expiry/check", data: model });
        }
        function editDoc(id) {
            return httpWrapper.get({ url: baseUrl + "/doc/" + id + "/get/info" });
        }
        return {
            saveDocuments: saveDocuments,
            getDocumentList: getDocumentList,
            deleteDoc: deleteDoc,
            getDocumentType: getDocumentType,
            getNationalDocumentType: getNationalDocumentType,
            getDocumentTypeForFranchisee: getDocumentTypeForFranchisee,
            GetNationalTypeForFranchisee: GetNationalTypeForFranchisee,
            isExpiryCorrect: isExpiryCorrect,
            editDoc: editDoc
        };
    }]);
})();