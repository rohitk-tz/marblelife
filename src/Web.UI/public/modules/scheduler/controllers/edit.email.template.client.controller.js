(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("EditEmailTemplateController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "modalParam", '$sce', "Toaster",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, modalParam, $sce, toaster) {

                var vm = this;
                vm.query = {
                    body: '',
                    emailTemplateId: 0,
                    languageId: 0,
                    notificationId: 0,
                    subject: ''
                };
                vm.saveEmailTemplate = saveEmailTemplate;
                //$scope.data = {
                //    text: "<p style='font-size:90px'>hello</p>"
                //}

                $scope.orightml = '';
                $scope.htmlcontent = $scope.orightml;
                $scope.disabled = false;

                vm.getEmailTemplate = getEmailTemplate;
                vm.emailTemplate = [];
                vm.query.emailTemplateId = modalParam.EmailTemplate != null ? modalParam.EmailTemplate : 0;
                vm.query.body = modalParam.Body != null ? modalParam.Body : '';
                vm.query.languageId = modalParam.LanguageId != null ? modalParam.LanguageId : 0;
                vm.query.notificationId = modalParam.notificationId != null ? modalParam.notificationId : 0;
                vm.query.subject = modalParam.Subject != null ? modalParam.Subject : '';
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                function getEmailTemplate(emailTemplateId) {
                    vm.emailTemplate.body = "";
                    if (vm.fromFranchiseeMail) {
                        vm.emailTemplate.body = vm.body;
                    }
                    else {
                        return schedulerService.getEmailTemplate(emailTemplateId).then(function (result) {
                            vm.emailTemplate = result.data;
                        });
                    }
                }
                function saveEmailTemplate() {
                    vm.query.body = '<!DOCTYPE html PUBLIC " -//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml">' + vm.query.body + '</html>';

                    return schedulerService.editEmailTemplate(vm.query).then(function (result) {
                        if (result.data) {
                            toaster.show("Template Created/Edited Successfully!!");
                        }
                        else {
                            toaster.error("Error in Creating/Editing Template!!");
                        }

                        $uibModalInstance.dismiss();
                    });
                }

                $q.all([]);
            }]);
}());