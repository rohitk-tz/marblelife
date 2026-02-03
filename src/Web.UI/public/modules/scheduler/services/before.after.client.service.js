(function () {
    'use strict';

    angular.module(SchedulerConfiguration.moduleName).service("BeforeAfterService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/Scheduler/job"

        function getReviewImages(model) {
            return httpWrapper.post({ url: baseUrl + "/getReviewImages", data: model });
        }

        function saveImagesBestPair(model) {
            return httpWrapper.post({ url: baseUrl + "/saveImagesBestPair", data: model });
        }

        function saveReviewMark(model) {
            return httpWrapper.post({ url: baseUrl + "/saveReviewMarkImage", data: model });
        }

        function getReviewImagesForFA(model) {
            return httpWrapper.post({ url: baseUrl + "/getReviewImagesForFA", data: model });
        }
        function saveReviewImagesForFA(model) {
            return httpWrapper.post({ url: baseUrl + "/saveReviewImagesForFA", data: model });
        }
        function getUserList(franchiseeId) {
            return httpWrapper.get({ url: "/application/dropdown/GetUserList" });
        }
        function getLocalMarketingReview(model) {
            return httpWrapper.post({ url: baseUrl + "/getLocalMarketingReview", data: model });
        }
        function getSalesRepTechnicianList(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/getSalesRepTechnicianList/" + franchiseeId });
        }
        function markImageAsReviwed(model) {
            return httpWrapper.post({ url: baseUrl + "/markImageAsReviwed", data: model });
        }
        function markImageAsBestPair(model) {
            return httpWrapper.post({ url: baseUrl + "/markImageAsBestPair", data: model });
        }
        function markImageAsAddToLocalGallery(model) {
            return httpWrapper.post({ url: baseUrl + "/markImageAsAddToLocalGallery", data: model });
        }
        function bestPairMarkedJobEstimateImagePair(model) {
            return httpWrapper.post({ url: baseUrl + "/bestPairMarkedJobEstimateImagePair", data: model });
        }
        return {
            getReviewImages: getReviewImages,
            saveImagesBestPair: saveImagesBestPair,
            saveReviewMark: saveReviewMark,
            getReviewImagesForFA: getReviewImagesForFA,
            saveReviewImagesForFA: saveReviewImagesForFA,
            getUserList: getUserList,
            getLocalMarketingReview: getLocalMarketingReview,
            getSalesRepTechnicianList: getSalesRepTechnicianList,
            markImageAsReviwed: markImageAsReviwed,
            markImageAsBestPair: markImageAsBestPair,
            markImageAsAddToLocalGallery: markImageAsAddToLocalGallery,
            bestPairMarkedJobEstimateImagePair: bestPairMarkedJobEstimateImagePair
        };
    }]);
})();