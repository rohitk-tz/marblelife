(function () {
    'use strict';

    angular.module(OrganizationsConfiguration.moduleName).service("FranchiseeService", ["HttpWrapper", "APP_CONFIG", "URLAuthenticationServiceForEncryption",
        function (httpWrapper, config, uRLAuthenticationServiceForEncryption) {
        var baseUrl = "/organizations";
        var maxSlabsCount = config.maxSlabsCount;

        function getFranchisee(id) {
            id = uRLAuthenticationServiceForEncryption.decrypt(id.toString());
            if (id == "") {
                id = 0;
            }
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + id });
        }

        function saveFranchisee(franchisee) {
            franchisee.typeId = 22;  //To Do : Replace hard coded value by Lookup
            return httpWrapper.post({ url: baseUrl + "/franchisee", data: franchisee });
        }

        function getFranchiseeCollection(query) {
            return httpWrapper.get({
                url: baseUrl + "/franchisee?filter.text=" + query.text + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order + "&filter.franchiseeId=" + query.franchiseeId +
                    "&filter.franchisee=" + query.franchisee + "&filter.email=" + query.email + "&filter.franchiseeStatus=" + query.status
            });
        }

        function getServiceTypeCollection() {
            return httpWrapper.get({ url: "/application/dropdown/GetServiceTypes" });
        }

        function getServiceTypeCollectionForInvoice() {
            return httpWrapper.get({ url: "/application/dropdown/GetServiceTypesForInvoice" });
        }
        function getServiceTypeCollectionForInvoiceNew() {
            return httpWrapper.get({ url: "/application/dropdown/GetServiceTypesForInvoiceNew" });
        }
        function getListOfServiceCollection() {
            return httpWrapper.get({ url: "/application/dropdown/GetListOfServices" });
        }

        function getFranchiseeNameValuePair() {
            return httpWrapper.get({ url: "/application/dropdown/GetFranchiseeNameValuePair" });
        }

        function getActiveFranchiseeList() {
            return httpWrapper.get({ url: "/application/dropdown/GetActiveFranchiseeList" });
        }
        function getActiveFranchiseeListWithOut0ML() {
            return httpWrapper.get({ url: "/application/dropdown/GetActiveFranchiseeListWithOut0ML" });
        }
        function getFranchiseeInfo(userId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + userId + "/franchiseeInfo/collection" });
        }

        function getFranchiseeFeeProfile(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/fee/profile" });
        }

        function resetFeeProfile(feeProfile) {
            if (feeProfile.salesBasedRoyalty == true) {
                feeProfile.fixedAmount = null;
            }
            else {
                feeProfile.paymentFrequencyId = null;
                feeProfile.minimumRoyaltyPerMonth = 0;
                feeProfile.adFundPercentage = 0;
                feeProfile.slabs = [];
            }
        }

        function deleteFee(id, typeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + id + "/delete/" + typeId + "/fee" });
        }

        function getOTPFeeList(id) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + id + "/get/otpFee" });
        }
        function getLoanList(id) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + id + "/get/loan" });
        }
        function saveServiceFee(model) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/" + model.franchiseeId + "/save/serviceFee", data: model });
        }

        function deleteFranchisee(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/delete/franchisee" });
        }

        function getFranchiseeListForLogin(userId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + userId + "/list/franchisee" });
        }
        function manageFranchisee(model) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/manage/account", data: model });
        }

        function getFranchiseeNotes(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/list/notes" });
        }

        function deactivateFranchisee(model) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/deactivate/franchisee", data: model });
        }
        function activateFranchisee(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/activate/franchisee" });
        }

        function isUniqueBusinessId(id, businessId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + id + "/businessId/" + businessId + "/verify" });
        }
        function getFranchiseeUserCollection(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/GetUserListForDocument", data: franchiseeId });
        }
        function saveChangeFeeAdjustment(model) {
            return httpWrapper.post({ url: baseUrl + "/save/changeServiceFee", data: model });
        }
        function getFranchiseeRoyality(franchiseeId) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/" + franchiseeId + "/royalty/adfund" });
        }
        function getFranchiseeTeamImage(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/team/image" });
        }
        function saveFranchiseeTeamImage(model) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/save/team/image", data: model });
        }
        function exportLoan(loanId) {
            return httpWrapper.getFileByPost({ url: baseUrl + "/loan/export/", data: loanId });
        }
        function download(query) {
            return httpWrapper.getFileByPost({
                url: baseUrl + "/download/franchisee?filter.text=" + query.text + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order + "&filter.franchiseeId=" + query.franchiseeId +
                    "&filter.franchisee=" + query.franchisee + "&filter.email=" + query.email
            });
        }
        function downloadFranchieeAdmin(query) {
            return httpWrapper.getFileByPost({
                url: baseUrl + "/download/franchiseDirectory?filter.text=" + query.text + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order + "&filter.franchiseeId=" + query.franchiseeId +
                    "&filter.franchisee=" + query.franchisee + "&filter.email=" + query.email
            });
        }

        function getFranchiseeDesignCollection(query) {
            return httpWrapper.get({
                url: baseUrl + "/franchisees/redesign?filter.text=" + query.text + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order + "&filter.franchiseeId=" + query.franchiseeId +
                    "&filter.franchisee=" + query.franchisee + "&filter.email=" + query.email + "&filter.franchiseeStatus=" + query.status
            });
        }
        function downloadFileFranchieeAdmin(query) {
            return httpWrapper.getFileByPost({
                url: baseUrl + "/franchisee/downloadFileFranchiseeDirectory?filter.text=" + query.text + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                    + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order + "&filter.franchiseeId=" + query.franchiseeId +
                    "&filter.franchisee=" + query.franchisee + "&filter.email=" + query.email + "&filter.franchiseeStatus=" + query.status
            });
        }

        function getFranchiseeDeactivationNote(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId + "/list/deactivation/notes" });
        }

        function getFranchiseeRPId(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/" + franchiseeId+"/list/rpId" });
        }

        function getFranchiseeNameValuePairByRole() {
            return httpWrapper.get({ url: "/application/dropdown/GetFranchiseeNameValuePairByRole" });
        }

        function changeAdFundRoyalityStatus(model) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/change/adfund/royality", data: model });
        }
        function getFranchiseeDocumentReport(model) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/document/report", data: model });
        }

        function getFranchiseeDocumentList() {
            return httpWrapper.post({ url: baseUrl + "/franchisee/list/document" });
        }

        function getFranchiseeListWithOut0ML() {
            return httpWrapper.get({ url: "/application/dropdown/GetFranchiseeListWithOut0ML" });
        }
        function getFranchiseeNameValuePairByRoleForFA() {
            return httpWrapper.get({ url: "/application/dropdown/getFranchiseeNameValuePairByRoleForFA" });
        }
        function savePrePayAmount(model) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/prePay/Loan", data: model });
        }
        function saveNotes(model) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/notes", data: model });
        }
        function getDurationApprovalList(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/durationApprovalList/" + franchiseeId + "/list"});
        }
        function changeDurationStatus(model) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/changeStatus", data: model });
        }
        function downloadTaxReport(model) {
            return httpWrapper.getFileByPost({ url: baseUrl + "/franchisee/downloadTaxReport", data: model });
        }
        function getServicesMutiSelectCollection() {
            return httpWrapper.get({ url: "/application/dropdown/getServicesForDropdown" });
        }

        function getServiceTypeCollectionNewOrder() {
            return httpWrapper.get({ url: "/application/dropdown/GetServiceTypesNewOrder" });
        }

        function changeAdFundRoyalityStatusForSEOCharges(model) {
            return httpWrapper.post({ url: baseUrl + "/franchisee/change/adfund/royality/SEOCharges", data: model });
        }

        function getAllServicesListForDropdown() {
            return httpWrapper.get({ url: "/application/dropdown/GetAllServicesList" });
        }

        return {
            getFranchisee: getFranchisee,
            saveFranchisee: saveFranchisee,
            getFranchiseeCollection: getFranchiseeCollection,
            getServiceTypeCollection: getServiceTypeCollection,
            getFranchiseeNameValuePair: getFranchiseeNameValuePair,
            getFranchiseeFeeProfile: getFranchiseeFeeProfile,
            resetFeeProfile: resetFeeProfile,
            deleteFranchisee: deleteFranchisee,
            getFranchiseeInfo: getFranchiseeInfo,
            getFranchiseeListForLogin: getFranchiseeListForLogin,
            manageFranchisee: manageFranchisee,
            getFranchiseeNotes: getFranchiseeNotes,
            deactivateFranchisee: deactivateFranchisee,
            getActiveFranchiseeList: getActiveFranchiseeList,
            activateFranchisee: activateFranchisee,
            isUniqueBusinessId: isUniqueBusinessId,
            deleteFee: deleteFee,
            getOTPFeeList: getOTPFeeList,
            saveServiceFee: saveServiceFee,
            getLoanList: getLoanList,
            getFranchiseeUserCollection: getFranchiseeUserCollection,
            getActiveFranchiseeListWithOut0ML: getActiveFranchiseeListWithOut0ML,
            saveChangeFeeAdjustment: saveChangeFeeAdjustment,
            getFranchiseeRoyality: getFranchiseeRoyality,
            getFranchiseeTeamImage: getFranchiseeTeamImage,
            saveFranchiseeTeamImage: saveFranchiseeTeamImage,
            exportLoan: exportLoan,
            download: download,
            downloadFranchieeAdmin: downloadFranchieeAdmin,
            getFranchiseeDesignCollection: getFranchiseeDesignCollection,
            downloadFileFranchieeAdmin: downloadFileFranchieeAdmin,
            getFranchiseeDeactivationNote: getFranchiseeDeactivationNote,
            getFranchiseeRPId: getFranchiseeRPId,
            getFranchiseeNameValuePairByRole: getFranchiseeNameValuePairByRole,
            changeAdFundRoyalityStatus: changeAdFundRoyalityStatus,
            getFranchiseeDocumentReport: getFranchiseeDocumentReport,
            getFranchiseeDocumentList: getFranchiseeDocumentList,
            getFranchiseeListWithOut0ML: getFranchiseeListWithOut0ML,
            getFranchiseeNameValuePairByRoleForFA: getFranchiseeNameValuePairByRoleForFA,
            savePrePayAmount: savePrePayAmount,
            saveNotes: saveNotes,
            getDurationApprovalList: getDurationApprovalList,
            changeDurationStatus: changeDurationStatus,
            downloadTaxReport: downloadTaxReport,
            getServicesMutiSelectCollection: getServicesMutiSelectCollection,
            getServiceTypeCollectionNewOrder: getServiceTypeCollectionNewOrder,
            getFranchiseeNotes: getFranchiseeNotes,
            getServiceTypeCollectionForInvoice: getServiceTypeCollectionForInvoice,
            getListOfServiceCollection: getListOfServiceCollection,
            getServiceTypeCollectionForInvoiceNew: getServiceTypeCollectionForInvoiceNew,
            changeAdFundRoyalityStatusForSEOCharges: changeAdFundRoyalityStatusForSEOCharges,
            getAllServicesListForDropdown: getAllServicesListForDropdown
        };
    }]);
})();