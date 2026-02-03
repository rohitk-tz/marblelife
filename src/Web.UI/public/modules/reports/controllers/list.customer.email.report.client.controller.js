(function () {
    'use strict';

    //SchedulerConfiguration
    angular.module(ReportsConfiguration.moduleName).controller("FranchiseeMailsController",
        ["$scope", "$rootScope", "$state", "$q", "APP_CONFIG", "$uibModal", "FileService", "Notification", "Toaster", "ReportService", "FranchiseeService","Clock",
            function ($scope, $rootScope, $state, $q, config, $uibModal, fileService, notification, toaster, reportService, franchiseeService, clock) {

                var vm = this;
                vm.query = {
                    Id: 0,
                    isActive: 1,
                    FranchiseeId: 0,
                    pageNumber: 1,
                    pageSize: config.defaultPageSize,
                    Order: 0,
                    PropName: '',
                    text: '',
                    periodStartDate: null,
                    periodEndDate: null,
                    sort: { order: 0, propName: '' },
                    emailFrom: '',
                    emailTo: '',
                    emailCc: ''
                };
                vm.SortColumns = {
                    EmailTitle: 'EmailTitle',
                    FranchiseeName: 'FranchiseeName',
                    SendDate: 'SendDate',
                    FromEmail: 'FromEmail',
                    RecipientEmail: 'RecipientEmail',
                    RecipientEmailCC: 'RecipientEmailCC'
                };
                vm.pageChange = pageChange;
                vm.resetSearch = resetSearch;
                vm.preview = preview;
                vm.refresh = refresh;
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
                vm.emailOptions = [];
                vm.resetSeachOption = resetSeachOption;
                vm.resetSeachOption = resetSeachOption;

                $scope.$on('clearDates', function (event) {
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    getMailList();
                });


                function pageChange() {
                    getMailList();
                }
                function refresh() {
                    getMailList();
                }
                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' })
                    vm.searchOptions.push({ display: 'Email Id', value: '4' })
                    vm.searchOptions.push({ display: 'Others', value: '3' });
                }

                function emailSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.emailOptions.push({ display: 'From Email', value: '11' })
                    vm.emailOptions.push({ display: 'To Email', value: '12' })
                    vm.emailOptions.push({ display: 'CC' + "'" + 'd Email', value: '13' });
                }

                function preview(id, body) {
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
                                    Body: body,
                                    FromFranchiseeMail: true
                                };
                            }
                        }
                    });
                }

                function resetSeachOption() {
                    if (vm.searchOption == '1') {
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.emailFrom = '';
                        vm.query.emailTo = '';
                        vm.query.emailCc = '';
                        vm.emailOption = '';
                    }
                    else if (vm.searchOption == '4') {
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.emailFrom = '';
                        vm.query.emailTo = '';
                        vm.query.emailCc = '';
                    }
                    else if (vm.searchOption == '3') {
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.emailFrom = '';
                        vm.query.emailTo = '';
                        vm.query.emailCc = '';
                        vm.emailOption = '';
                    }
                    else if (vm.emailOption == '11') {
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.emailTo = '';
                        vm.query.emailCc = '';
                    }
                    else if (vm.emailOption == '12') {
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.emailFrom = '';
                        vm.query.emailCc = '';
                    }
                    else if (vm.emailOption == '13') {
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.emailFrom = '';
                        vm.query.emailTo = '';
                    }
                    else {
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                    }
                }
                function resetSearch() {
                    vm.query.text = '';
                    vm.query.franchiseeId = 0;
                    vm.query.statusId = '';
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    vm.searchOption = '';
                    $scope.$broadcast("reset-dates");
                    vm.query.pageNumber = 1;
                    vm.query.emailFrom = "";
                    vm.query.emailTo = "";
                    vm.query.emailCc = "";
                    vm.emailOption = '';
                    getMailList();
                }
                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function getMailList() {
                    vm.startDate = moment(clock.now()).format("MM/DD/YYYY");
                    vm.endDate = moment(clock.now()).subtract(2, "months").format("MM/DD/YYYY");

                    return reportService.getFranchiseeMailList(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.mailList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.index = ((vm.query.pageNumber - 1) * vm.query.pageSize) + 1;
                            vm.query.periodStartDate = moment(result.data.endDate).format("MM/DD/YYYY");
                            vm.query.periodEndDate = moment(result.data.startDate).format("MM/DD/YYYY");
                            $scope.$broadcast("apply.daterangepicker");
                        }
                    });
                }

                function sorting(propName) {
                    vm.query.PropName = propName;
                    vm.query.Order = (vm.query.Order == 0) ? 1 : 0;
                    getMailList();
                };


                $scope.$emit("update-title", "Franchisee Mails List");

                $q.all([getMailList(), getFranchiseeCollection(), prepareSearchOptions(), emailSearchOptions()]);

            }]);
}());