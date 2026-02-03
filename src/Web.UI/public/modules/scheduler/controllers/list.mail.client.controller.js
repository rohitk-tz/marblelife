(function () {
    'use strict';
    var SortColumnss = {
        EmailTitle: 'EmailTitle',
    };
    angular.module(SchedulerConfiguration.moduleName).controller("MailController",
        ["$scope", "$rootScope", "$state", "$q", "APP_CONFIG", "$uibModal", "FranchiseeService", "FileService", "Notification", "Toaster", "SchedulerService",
            function ($scope, $rootScope, $state, $q, config, $uibModal, franchiseeService, fileService, notification, toaster, schedulerService) {

                var vm = this;
                vm.query = {
                    Id: 0,
                    isAcitve: 1,
                    PageNumber: 1,
                    pageSize: config.defaultPageSize,
                    Order: 0,
                    PropName: ''
                };

                vm.SortColumns = {
                    EmailTitle: 'EmailTitle'
                };
                vm.editMail = editMail;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.editMailTemplate = editMailTemplate;
                vm.preview = preview;
                vm.BatchStatus = DataHelper.SalesDataUploadStatus;
                vm.Roles = DataHelper.Role;
                $scope.isOperationManager = false;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;

                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;

                vm.pagingOptions = config.pagingOptions;
                vm.getMailList = getMailList;
                vm.currencyCode = $rootScope.identity.currencyCode;
                vm.searchOptions = []
                vm.searchOption = '';
                vm.currentRole = $rootScope.identity.roleId;
                vm.sorting = sorting;

                $scope.$on('clearDates', function (event) {
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    getGeoList();
                });

                function preview(id) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/email-template.client.view.html',
                        controller: 'EmailTemplateController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    EmailTemplate: id,
                                };
                            }
                        }
                    });
                }
                function editMail(id, isActive) {
                    vm.query.Id = id;
                    vm.query.isActive = isActive == true ? 1 : 0;
                    return schedulerService.editTemplate(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            if (result.data) {
                                if (isActive == 0)
                                    toaster.show("Email Deactivated");
                                else
                                    toaster.show("Email Activated");
                            }
                            else {
                                toaster.show("Error in Activating/Deactivating Email");
                            }
                        }
                    });
                }
                function getMailList() {
                    return schedulerService.getMailList(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.mailList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                        }
                    });
                }

                function sorting(propName) {
                    vm.query.PropName = 'EmailTitle';
                    vm.query.Order = (vm.query.Order == 0) ? 1 : 0;
                    getMailList();
                };

                function editMailTemplate(id, languageId, body, subject) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/edit-email-template.client.view.html',
                        controller: 'EditEmailTemplateController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Body: body,
                                    EmailTemplate: id,
                                    LanguageId: languageId,
                                    Subject: subject
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getMailList();
                    }, function () {
                            getMailList();
                    });
                }
                $scope.$emit("update-title", "Mail List");

                $q.all([getMailList()]);

            }]);
}());