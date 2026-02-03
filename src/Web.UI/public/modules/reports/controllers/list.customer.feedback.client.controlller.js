(function () {
    'use strict';

    var SortColumns = {
        Franchisee: 'Franchisee',
        Customer: 'Customer',
        CustomerEmail: 'CustomerEmail',
        DateReceived: 'DateReceived',
        DateSend: 'DateSend',
        CustomerId: 'CustomerId',
        Rating: 'Rating',
        ContactName: 'ContactName',
        CustomerName: 'CustomerName'
    };

    angular.module(ReportsConfiguration.moduleName).controller("CustomerFeedbackController",
        ["$state", "$stateParams", "$q", "$scope", "APP_CONFIG", "$rootScope", "FranchiseeService", "$uibModal", "CustomerFeedbackReportService", "FileService",
            function ($state, $stateParams, $q, $scope, config, $rootScope, franchiseeService, $uibModal, customerFeedbackReportService, fileService) {
                var vm = this;

                vm.query = {
                    pageNumber: 1,
                    franchiseeId: 0,
                    startDate: null,
                    endDate: null,
                    receivedStartDate: null,
                    receivedEndDate: null,
                    pageSize: config.defaultPageSize,
                    text: '',
                    response: null,
                    responseFrom: 0,
                    sort: { order: 0, propName: '' }
                };

                vm.getFeedbackList = getFeedbackList;
                vm.pagingOptions = config.pagingOptions;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.pageChange = pageChange;
                vm.searchOptions = [];
                vm.receivedFrom = [];
                vm.searchOption = '';
                vm.resetSeachOption = resetSeachOption;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.currentRole = $rootScope.identity.roleId;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.viewFeedback = viewFeedback;
                vm.response = [];
                vm.downloadFeedbackReport = downloadFeedbackReport;
                vm.customerIds = [];
                vm.manageFeedbackStatus = manageFeedbackStatus;
                vm.auditStatus = DataHelper.AuditStatus;

                function downloadFeedbackReport() {
                    vm.downloading = true;
                    return customerFeedbackReportService.downloadFeedbackReport(vm.query).then(function (result) {
                        var fileName = "feedbackReport.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;

                    }, function () { vm.downloading = false; });
                }

                function PrepareResponse() {
                    vm.response.push({ display: 'Yes', value: 1 }),
                        vm.response.push({ display: 'No', value: 0 });
                };

                function PrepareRecieveFrom() {
                    //vm.receivedFrom.push({ display: 'Review Push', value: 1 }),
                    //    vm.receivedFrom.push({ display: 'Gather Up', value: 2 });
                    vm.receivedFrom.push({ display: 'Google API', value: 1 });
                    vm.receivedFrom.push({ display: 'Old Review', value: 2 });
                    /*vm.receivedFrom.push({ display: 'System Review', value: 4 });*/
                };

                function viewFeedback(responseId, isFromNewReviewSystem, isFromCustomerReviewTable, id) {
                    //if (responseId == 0) {
                    //    responseId = id;
                    //}
                    responseId = id;
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/view-feedback-detail.client.view.html',
                        controller: 'ViewFeedbackDetailController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    ResponseId: responseId,
                                    IsFromNewReviewSystem: isFromNewReviewSystem,
                                    IsFromCustomerReviewTable: isFromCustomerReviewTable
                                };
                            }
                        }
                    });
                }

                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' })

                    vm.searchOptions.push({ display: 'Response Date', value: '2' }),
                        vm.searchOptions.push({ display: 'Request date', value: '3' }),
                        vm.searchOptions.push({ display: 'Response Received', value: '5' }),
                        vm.searchOptions.push({ display: 'Received From', value: '6' }),
                        vm.searchOptions.push({ display: 'Other', value: '4' });

                }

                function refresh() {
                    getFeedbackList();
                }

                function resetSeachOption() {
                    if (vm.seachOption == '1') {
                        vm.query.startDate = null;
                        vm.query.endDate = null;
                        vm.query.receivedStartDate = null;
                        vm.query.receivedEndDate = null;
                        vm.query.text = '';
                        vm.query.response = null;
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '2') {
                        vm.query.franchiseeId = 0;
                        vm.query.text = '';
                        vm.query.startDate = null;
                        vm.query.endDate = null;
                        vm.query.response = null;
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '3') {
                        vm.query.franchiseeId = 0;
                        vm.query.text = '';
                        vm.query.receivedStartDate = null;
                        vm.query.receivedEndDate = null;
                        vm.query.response = null;
                        vm.query.pageNumber = 1;
                    }
                    else if (vm.seachOption == '4') {
                        vm.query.franchiseeId = 0;
                        vm.query.text = '';
                        vm.query.startDate = null;
                        vm.query.endDate = null;
                        vm.query.receivedStartDate = null;
                        vm.query.receivedEndDate = null;
                        vm.query.response = null;
                        vm.query.pageNumber = 1
                    }
                    else if (vm.seachOption == '5') {
                        vm.query.franchiseeId = 0;
                        vm.query.text = '';

                        vm.query.startDate = null;
                        vm.query.endDate = null;
                        vm.query.receivedStartDate = null;
                        vm.query.receivedEndDate = null;
                        vm.query.pageNumber = 1
                    }
                    else {
                        vm.query.franchiseeId = 0;
                        vm.query.startDate = null;
                        vm.query.endDate = null;
                        vm.query.receivedStartDate = null;
                        vm.query.receivedEndDate = null;
                        vm.query.response = null;
                        vm.query.pageNumber = 1;
                        vm.query.responseFrom = 0;
                    }
                }

                function resetSearch() {
                    vm.query.text = '';
                    vm.query.franchiseeId = 0;
                    vm.query.startDate = null;
                    vm.query.endDate = null;
                    vm.query.response = null;
                    vm.searchOption = '';
                    $rootScope.$broadcast("reset-dates");
                    vm.query.pageNumber = 1;
                    vm.query.receivedStartDate = null;
                    vm.query.receivedEndDate = null;
                    vm.query.responseFrom = 0;
                    getFeedbackList();
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.startDate = null;
                    vm.query.endDate = null;
                    vm.query.receivedStartDate = null;
                    vm.query.receivedEndDate = null;
                    getFeedbackList();
                });

                function getFeedbackList() {
                    if (vm.query.responseFrom == 0) {
                        vm.query.responseFrom = 1;
                    }
                    return customerFeedbackReportService.getCustomerFeedbackReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.customerIds = [];
                            vm.feedbackList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            addResultToList(vm.feedbackList);
                        }
                    });
                }
                function addResultToList() {
                    angular.forEach(vm.feedbackList, function (value, key) {
                        vm.customerIds.push(value.customerId);
                    })
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getFeedbackList();
                };

                function pageChange() {
                    getFeedbackList();
                };

                function manageFeedbackStatus(customerId, id, fromTable) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/customer-feedback-status-option.client.view.html',
                        controller: 'CustomerFeedbackStatusOptionController',
                        controllerAs: 'vm',
                        size: 'sm',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    CustomerId: customerId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function (result) {
                        if (result == 1) {
                            var isAccept = false;
                        }
                        else if (result == 2) {
                            var isAccept = true;
                        }
                        setCustomerFeedbackAction(isAccept, customerId, id, fromTable);
                    }, function () {

                    });
                }

                function setCustomerFeedbackAction(isAccept, customerId, id, fromTable) {
                    return customerFeedbackReportService.manageCustomerFeedbackStatus(isAccept, customerId, id, fromTable).then(function (res) {
                        vm.isProcessing = false;
                        getFeedbackList();
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                $scope.$emit("update-title", "Customer Feedback Report");
                $q.all([getFranchiseeCollection(), getFeedbackList(), prepareSearchOptions(), PrepareResponse(), PrepareRecieveFrom()]);

            }]);
}());