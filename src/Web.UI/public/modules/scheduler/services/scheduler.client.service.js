(function () {
    'use strict';

    angular.module(SchedulerConfiguration.moduleName).service("SchedulerService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/scheduler/scheduler"

        function saveJob(job) {
            return httpWrapper.post({ url: baseUrl + "/SaveJob", data: job });
        }

        function getJobInfo(estimateId) {
            return httpWrapper.get({ url: baseUrl + "/" + estimateId });
        }

        //function getJobList(query) {
        //    return httpWrapper.get({
        //        url: baseUrl + "?filter.text=" + query.text + "&filter.dateCreated=" + query.dateCreated + "&filter.dateModified=" + query.dateModified
        //            + "&filter.franchiseeId=" + query.franchiseeId + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
        //            + "&filter.techId=" + query.techId + "&filter.resourceIds=" + query.resourceIds
        //            + "&filter.jobTypeId=" + query.jobTypeId + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order
        //    })
        //}

        function getJobList(query) {
            return httpWrapper.post({ url: baseUrl + "/list", data: query });
        }

        function getAssigneeList(query) {
            return httpWrapper.post({ url: "/application/dropdown/GetAssigneeList", data: query });
        }
        function getJobStatus() {
            return httpWrapper.get({ url: "/application/dropdown/GetJobStatus" });
        }

        function getUserList(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/getUserList", data: franchiseeId });
        }

        function changeJobStatus(jobId, statusId) {
            return httpWrapper.post({ url: baseUrl + "/" + jobId + "/change/status", data: statusId });
        }

        function checkQbNumberIsValid(qbInvoice) {
            return httpWrapper.get({ url: baseUrl + "/" + qbInvoice + "/verify" });
        }

        function updateQbInvoice(jobId, qbInvoiceNumber,schedulerId) {
            return httpWrapper.post({ url: baseUrl + "/update/qb/" + schedulerId + "/job", data: qbInvoiceNumber });
        }

        function deleteJob(id) {
            return httpWrapper.delete({ url: baseUrl + "/job/delete/" + id });
        }

        function getSalesRep(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/GetSalesRepList", data: franchiseeId });
        }
        function GetRepList(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/GetRepList", data: franchiseeId });
        }

        function getTechList(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/GetTechList", data: franchiseeId });
        }
        function getTechListForMeeting(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/GetTechListForMeeting", data: franchiseeId });
        }

        function checkAvailability(model) {
            return httpWrapper.post({ url: baseUrl + "/job/verify", data: model });
        }

        function saveMedia(model) {
            return httpWrapper.post({ url: baseUrl + "/media/upload", data: model });
        }
        function saveMediaForUser(model) {
            return httpWrapper.post({ url: baseUrl + "/media/uploadForUser", data: model });
        }

        function getMedia(model) {
            return httpWrapper.post({ url: baseUrl + "/media/get", data: model });
        }

        function saveNote(model) {
            return httpWrapper.post({ url: baseUrl + "/note/save", data: model });
        }

        function getHolidayList(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/" + franchiseeId + "/list/holiday" });
        }
        function GetTechAndSalesList(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/GetTechAndSalesList", data: franchiseeId });
        }
        function GetTechAndSalesListForEstimate(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/GetTechAndSalesListForEstimate", data: franchiseeId });
        }
        function getCustomerAddress(jobId, estimeId) {
            return httpWrapper.post({ url: baseUrl + "/map/" + jobId, data: estimeId });
        }
        function getMailList(query) {
            return httpWrapper.post({ url: baseUrl + "/mail/",data: query });
        }
        function getEmailTemplate(id) {
            return httpWrapper.post({ url: baseUrl + "/mail/" + id + /template/ });
        }
        function editTemplate(query) {
            return httpWrapper.post({ url: baseUrl + "/mail/edit", data: query });
        }
        function saveBeforeAfterImages(query) {
            return httpWrapper.post({ url: baseUrl + "/job/estimate/images", data: query });
        }
        function saveMediaBeforeAfterForUser(query) {
            return httpWrapper.post({ url: baseUrl + "/job/estimate/before/after/upload", data: query });
        }
        function beforeAfterImageSendMail(query,id) {
            return httpWrapper.post({ url: baseUrl + "/job/estimate/before/after/send/mail/"+id, data: query });
        }
        function invoiceSendMail(query, id) {
            return httpWrapper.post({ url: baseUrl + "/job/estimate/invoice/send/mail/" + id, data: query });
        }
        function isEligibleForDeletion(query) {
            return httpWrapper.post({ url: baseUrl + "/job/estimate/isEligible/", data: query });
        }
        function getHolidayListMonthWise(model) {
            return httpWrapper.post({ url: baseUrl + "/list/holidayMonthWise", data: model });
        }
        function getTechListForMeetingForUser(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/GetTechListForMeetingForUser", data: franchiseeId });
        }
        function getFranchiseeInfo(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/franchisee/info/" + franchiseeId });
        }

        function getAssigneeListForScheduler(query) {
            return httpWrapper.post({ url: "/application/dropdown/GetAssigneeListForScheduler", data: query });
        }
        function dragDropScheduler(model) {
            return httpWrapper.post({ url: baseUrl + "/save/SaveDragDropEvent", data: model });
        }
        function confirmationScheduler(model) {
            return httpWrapper.post({ url: baseUrl + "/confirmation", data: model });
        }
        function confirmationSchedulerFromUI(model) {
            return httpWrapper.post({ url: baseUrl + "/confirmationFromUI", data: model });
        }
        function saveJobNotes(model) {
            return httpWrapper.post({ url: baseUrl + "/saveNotes", data: model });
        }
        function deleteJobNotes(id) {
            return httpWrapper.post({ url: baseUrl + "/delete/Notes/" + id});
        }
        function getCustomerInfo(name) {
            return httpWrapper.get({ url: baseUrl + "/customer/info/" + name });
        }

        function getUserListForFA(franchiseeId) {
            return httpWrapper.post({ url: "/application/dropdown/getUserListForFA", data: franchiseeId });
        }
        function saveEstimateWorth(model) {
            return httpWrapper.post({ url: baseUrl + "/saveEstimateWorth", data: model });
        }

        function saveInvoiceInfo(model) {
            return httpWrapper.post({ url: baseUrl + "/saveInvoiceInfo", data: model });
        }
        function getInvoiceInfo(model) {
            return httpWrapper.post({ url: baseUrl + "/getInvoiceInfo", data: model });
        }

        function sendInvoiceToCustomer(model) {
            return httpWrapper.post({ url: baseUrl + "/sendInvoiceToCustomer", data: model });
        }

        function uploadInvoicesZipFile(model) {
            return httpWrapper.post({ url: baseUrl + "/uploadInvoicesZipFile", data:model });
        }

        function saveCustomerSignature(model) {
            return httpWrapper.post({ url: baseUrl + "/SaveCustomerSignature", data: model });
        }
        function uploadSignedInvoicesZipFile(model) {
            return httpWrapper.post({ url: baseUrl + "/uploadSignedInvoicesZipFile", data: model });
        }

        function shiftImagesToInvoiceBuildMaterial(model) {
            return httpWrapper.post({ url: baseUrl + "/ShiftImagesToInvoiceBuildMaterial", data: model });
        }

        function editEmailTemplate(query) {
            return httpWrapper.post({ url: baseUrl + "/editEmailTemplate", data: query });
        }

        function uploadCustomerInvoicesZipFile(model) {
            return httpWrapper.post({ url: baseUrl + "/UploadInvoicesCustomerZipFile", data: model });
        }
        function saveInvoiceRequired(query) {
            return httpWrapper.post({ url: baseUrl + "/SaveInvoiceRequired", data: query });
        }
        function addInvoiceToEstimate(schedulerId) {
            return httpWrapper.post({ url: baseUrl + "/AddInvoiceToEstimate/" + schedulerId });
        }
        function getInvoiceMedia(model) {
            return httpWrapper.post({ url: baseUrl + "/invoicemedia/get", data: model });
        }
        function changeCustomerAvailability(model) {
            return httpWrapper.post({ url: baseUrl + "/customerIsAvailableOrNot", data: model });
        }

        function checkAvailabilityList(model) {
            return httpWrapper.post({ url: baseUrl + "/job/verifyList", data: model });
        }

        function getServiceTypeId(model) {
            return httpWrapper.post({ url: baseUrl + "/getServiceTypeId", data: model });
        }

        function saveInvoiceLineItemMedia(query) {
            return httpWrapper.post({ url: baseUrl + "/job/estimate/invoiceLine/upload", data: query });
        }

        function getLastTwentyYearCollection() {
            return httpWrapper.get({ url: "/application/dropdown/GetLastTwentyYears" });
        }

        function getMonthNames() {
            return httpWrapper.get({ url: "/application/dropdown/GetMonthNames" });
        }
        function saveRotation(query) {
            return httpWrapper.post({ url: baseUrl + "/saveRotation", data: query  });
        }
        function saveCroppedImage(query) {
            return httpWrapper.post({ url: baseUrl + "/saveCroppedImage", data: query });
        }

        return {
            saveJob: saveJob,
            getJobList: getJobList,
            getAssigneeList: getAssigneeList,
            getJobStatus: getJobStatus,
            changeJobStatus: changeJobStatus,
            getJobInfo: getJobInfo,
            checkQbNumberIsValid: checkQbNumberIsValid,
            deleteJob: deleteJob,
            getSalesRep: getSalesRep,
            updateQbInvoice: updateQbInvoice,
            checkAvailability: checkAvailability,
            saveMedia: saveMedia,
            getMedia: getMedia,
            saveNote: saveNote,
            getTechList: getTechList,
            getHolidayList: getHolidayList,
            getUserList: getUserList,
            getTechListForMeeting: getTechListForMeeting,
            saveMediaForUser: saveMediaForUser,
            GetRepList: GetRepList,
            GetTechAndSalesList: GetTechAndSalesList,
            GetTechAndSalesListForEstimate: GetTechAndSalesListForEstimate,
            getCustomerAddress: getCustomerAddress,
            getMailList: getMailList,
            getEmailTemplate: getEmailTemplate,
            editTemplate: editTemplate,
            saveBeforeAfterImages: saveBeforeAfterImages,
            saveMediaBeforeAfterForUser: saveMediaBeforeAfterForUser,
            beforeAfterImageSendMail: beforeAfterImageSendMail,
            invoiceSendMail: invoiceSendMail,
            isEligibleForDeletion: isEligibleForDeletion,
            getHolidayListMonthWise: getHolidayListMonthWise,
            getTechListForMeetingForUser: getTechListForMeetingForUser,
            getFranchiseeInfo: getFranchiseeInfo,
            getAssigneeListForScheduler: getAssigneeListForScheduler,
            dragDropScheduler: dragDropScheduler,
            confirmationScheduler: confirmationScheduler,
            confirmationSchedulerFromUI: confirmationSchedulerFromUI,
            saveJobNotes: saveJobNotes,
            deleteJobNotes: deleteJobNotes,
            getCustomerInfo: getCustomerInfo,
            getUserListForFA: getUserListForFA,
            saveEstimateWorth: saveEstimateWorth,
            saveInvoiceInfo: saveInvoiceInfo,
            getInvoiceInfo: getInvoiceInfo,
            sendInvoiceToCustomer: sendInvoiceToCustomer,
            uploadInvoicesZipFile: uploadInvoicesZipFile,
            saveCustomerSignature: saveCustomerSignature,
            uploadSignedInvoicesZipFile: uploadSignedInvoicesZipFile,
            shiftImagesToInvoiceBuildMaterial: shiftImagesToInvoiceBuildMaterial,
            editEmailTemplate: editEmailTemplate,
            uploadCustomerInvoicesZipFile: uploadCustomerInvoicesZipFile,
            saveInvoiceRequired: saveInvoiceRequired,
            addInvoiceToEstimate: addInvoiceToEstimate,
            getInvoiceMedia: getInvoiceMedia,
            changeCustomerAvailability: changeCustomerAvailability,
            checkAvailabilityList: checkAvailabilityList,
            getServiceTypeId: getServiceTypeId,
            saveInvoiceLineItemMedia: saveInvoiceLineItemMedia,
            getLastTwentyYearCollection: getLastTwentyYearCollection,
            getMonthNames: getMonthNames,
            saveRotation: saveRotation,
            saveCroppedImage: saveCroppedImage
        };
    }]);
})();