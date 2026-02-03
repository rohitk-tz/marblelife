(function () {
    'use strict';
    angular.module(CoreConfiguration.moduleName).controller("EmailTemplateController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "modalParam",'$sce',
        function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, modalParam, $sce) {

            var vm = this;
            vm.getEmailTemplate = getEmailTemplate;
            vm.emailTemplate = [];
            vm.emailTemplateId = modalParam.EmailTemplate != null ? modalParam.EmailTemplate : 0;
            vm.body = modalParam.Body != null ? modalParam.Body : 0;
            vm.fromFranchiseeMail = modalParam.FromFranchiseeMail != null ? modalParam.FromFranchiseeMail : 0;
            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getEmailTemplate(emailTemplateId) {
                vm.emailTemplate.body = "";
                if (vm.fromFranchiseeMail) {
                    vm.emailTemplate.body = $sce.trustAsHtml(vm.body);
                }
                else {
                    return schedulerService.getEmailTemplate(emailTemplateId).then(function (result) {
                        vm.emailTemplate = result.data;
                    });
                }
            }

            $q.all([getEmailTemplate(vm.emailTemplateId)]);
        }]);
}());