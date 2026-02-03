(function () {

    angular.module(ReportsConfiguration.moduleName).controller("ViewFeedbackDetailController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "CustomerFeedbackReportService",
        function ($scope, $rootScope, $state, $uibModalInstance, modalParam, customerFeedbackReportService) {

            var vm = this;
            vm.responseId = modalParam.ResponseId;
            vm.getFeedbackDetail = getFeedbackDetail;
            vm.isFromNewReviewSystem = modalParam.IsFromNewReviewSystem;
            vm.isFromCustomerReviewTable = modalParam.IsFromCustomerReviewTable;
            vm.is = modalParam.IsFromNewReviewSystem;
            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getFeedbackDetail() {
                var isFromView = vm.isFromNewReviewSystem ? 1 : 0;
                var value = vm.responseId + '_' + isFromView + '_' + vm.isFromCustomerReviewTable;
                return customerFeedbackReportService.getFeedbackDetail(value).then(function (result) {
                    vm.feedback = result.data;
                });
            }
            getFeedbackDetail();
        }]);
}());