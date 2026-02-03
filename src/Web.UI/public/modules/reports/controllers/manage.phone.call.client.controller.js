(function () {
    'use strict';
    var SortColumns = {
        Id: 'Id',
        GeneratedOn: 'GeneratedOn',
        InvoiceId: 'InvoiceId'
    };
    angular.module(SchedulerConfiguration.moduleName).controller("ManagePhoneCallController",
        ["$scope", "$rootScope", "$state", "$q", "APP_CONFIG", "$uibModal", "FranchiseeService", "GeoCodeService", "FileService", "Notification", "Toaster", "$filter", "SalesService", "MarketingLeadService",
            function ($scope, $rootScope, $state, $q, config, $uibModal, franchiseeService, geoCodeService, fileService, notification, toaster, $filter, salesService, marketingLeadService) {

                var vm = this;
                vm.viewCalls = viewCalls;
                vm.changeInPhoneCount = changeInPhoneCount;
                vm.isBulkPossible = false;
                vm.bulkUpdatePopUp = bulkUpdatePopUp;
                vm.resetSearch = resetSearch;
                vm.viewHistry = viewHistry;
                vm.addingSelectedIdInList = addingSelectedIdInList;
                vm.idList = [];
                vm.activeAll = false;
                vm.changeFranchiseeActive = changeFranchiseeActive;
                vm.viewHistry = viewHistry;
                vm.save = save;
                vm.generateInvoice = generateInvoice;
                vm.editValue = editValue;
                vm.addMultiplePhoneCalls = addMultiplePhoneCalls;
                vm.selectedFranchisee = [];
                vm.query = {
                    text: '',
                    uploadedBy: "",
                    pageNumber: 1,
                    statusId: 0,
                    pageSize: config.defaultPageSize,
                    year: '2021',
                    periodStartDate: null,
                    periodEndDate: null,
                    sortingColumn: '',
                    SortingOrder: 0,
                    franchiseeId: 0

                };

                function getFranchiseePhoneCallsList() {

                    return marketingLeadService.getFranchiseePhoneCallsList(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.franchiseePhoneCall = result.data.collection;
                        }
                    });
                }

                function save(serviceInfo) {
                    var datetime = new Date(serviceInfo.phoneCallViewModel.dateOfChange)
                    var year = (datetime.getFullYear());

                    serviceInfo.phoneCallViewModel.totalCost = serviceInfo.phoneCallViewModel.callCount * serviceInfo.phoneCallViewModel.chargesForPhone;
                    serviceInfo.phoneCallViewModel.dateOfChange = new Date(serviceInfo.phoneCallViewModel.dateOfChange);
                    return marketingLeadService.editFranchiseePhoneCallsList(serviceInfo.phoneCallViewModel).then(function (result) {
                        if (result != null && result.data != null) {
                            if (result.data) {
                                toaster.show("Changes done successfully!!");
                                getFranchiseePhoneCallsList();
                            }
                            else {
                                toaster.error("Error in Saving Changes!!");
                            }
                        }
                    });
                }

                function viewHistry(histry, franchiseeName) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.phone.call.client.view.html',
                        controller: 'ModalPhoneCallController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Histry: histry,
                                    FranchiseeName: franchiseeName
                                };
                            }
                        }
                    });
                }

                function editValue(item) {
                    item.phoneCallViewModel.dateOfChange = moment(item.phoneCallViewModel.dateOfChange).toDate();
                }


                function generateInvoice(item) {
                    return marketingLeadService.generateInvoice(item.phoneCallViewModel).then(function (result) {
                        if (result != null && result.data != null) {
                            if (result.data) {
                                toaster.show("Invoice will be generated in next Weekly Upload!!");
                                getFranchiseePhoneCallsList();
                            }
                            else {
                                toaster.error("Error in Generating Invoice!!");
                            }
                        }
                    });
                }

                function changeFranchiseeActive() {
                    vm.idList = [];

                    angular.forEach(vm.franchiseePhoneCall, function (value, key) {
                        if (vm.activeAll) {
                            vm.idList.push(value.phoneCallViewModel.id);
                            value.isActive = true;
                            value.isAllFranchiseeSelected = true;
                            vm.isBulkPossible = true;
                        }
                        else {
                            value.isActive = false;
                            value.isAllFranchiseeSelected = false;
                            vm.isBulkPossible = false;

                        }
                    });

                }
                function addingSelectedIdInList(phoneCallViewModel, isAddedOrRemove) {

                    if (isAddedOrRemove) {
                        vm.selectedFranchisee = [];
                        vm.idList.push(phoneCallViewModel.id);
                        vm.selectedFranchisee.push(phoneCallViewModel.franchiseeId);
                    }
                    else {
                        var index = vm.idList.indexOf(phoneCallViewModel.id);
                        vm.activeAll = false;
                        vm.idList.splice(index, 1);
                    }
                    if (vm.idList.length > 1) {
                        vm.isBulkPossible = true;
                    }
                    else {
                        vm.isBulkPossible = false;
                    }

                }

                function bulkUpdatePopUp() {
                    vm.activeAll = false;
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.bulk.phone.call.client.view.html',
                        controller: 'ModalBulkPhoneCallController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    IdList: vm.idList,
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getFranchiseePhoneCallsList();
                        vm.activeAll = false;
                        vm.isBulkPossible = false;
                    }, function () {
                        getFranchiseePhoneCallsList();
                        vm.activeAll = false;
                        vm.isBulkPossible = false;
                    });
                }

                function viewHistry() {
                    vm.activeAll = false;
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.bulk.histry.phone.call.client.view.html',
                        controller: 'ModalBulkHistryPhoneCallController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    SelectedFranchiseeId: vm.selectedFranchisee,
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getFranchiseePhoneCallsList();
                        vm.activeAll = false;
                    }, function () {
                        vm.selectedFranchisee = [];
                        getFranchiseePhoneCallsList();
                        vm.activeAll = false;
                    });

                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function resetSearch() {
                    vm.query.franchiseeId = '0';
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    $scope.$broadcast("reset-dates");
                    getFranchiseePhoneCallsList();
                }


                function addMultiplePhoneCalls(phoneCallViewModel, franchiseeName) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.multiple.phone.call.client.view.html',
                        controller: 'ModalMultiplePhoneCallController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Histry: phoneCallViewModel.histry,
                                    FranchiseeName: franchiseeName,
                                    FranchiseeId: phoneCallViewModel.franchiseeId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getFranchiseePhoneCallsList();
                    }, function () {
                        getFranchiseePhoneCallsList();
                    })
                }
                $scope.$watch('vm.query.franchiseeId', function (nv, ov) {
                    if (nv == ov) return;
                    //else if (nv == 0 && ov != 0) { nv = ov; vm.query.franchiseeId = ov; return };
                    //vm.query.date = null;
                    getFranchiseePhoneCallsList();
                });

                function changeInPhoneCount(phoneCallItem) {
                    //if (phoneCallItem.phoneCallViewModel.callCount > 0) {
                    //    return;
                    //}
                    vm.queryDate = {};
                    vm.queryDate.startDate = phoneCallItem.phoneCallViewModel.dateOfChange;
                    vm.queryDate.franchiseeId = phoneCallItem.phoneCallViewModel.franchiseeId;

                    return marketingLeadService.getFranchiseePhoneCallsListForFranchisee(vm.queryDate).then(function (result) {
                        if (result != null && result.data != null) {
                            if (result.data.chargesOfCalls != undefined && result.data.chargesOfCalls > 0) {
                                phoneCallItem.phoneCallViewModel.chargesForPhone = result.data.chargesOfCalls;
                                phoneCallItem.phoneCallViewModel.callCount = result.data.callCount;
                            }
                            else {
                                phoneCallItem.phoneCallViewModel.callCount = result.data.totalCount;
                            }

                            phoneCallItem.phoneCallViewModel.totalCost = phoneCallItem.phoneCallViewModel.callCount * phoneCallItem.phoneCallViewModel.chargesForPhone;
                            phoneCallItem.phoneCallViewModel.listOfCallIVR = result.data.listOfCallIVR
                        }
                    });
                }

                function viewCalls(phoneCallViewModel) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.phone.call.details.client.view.html',
                        controller: 'ModalPhoneCallDetailsController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    CallsDetails: phoneCallViewModel.listOfCallIVR,
                                    FranchiseeName: phoneCallViewModel.franchiseeName
                                };
                            }
                        }
                    });
                }
                $scope.$emit("update-title", "Manage Phone Call Charges");

                $q.all([getFranchiseePhoneCallsList(), getFranchiseeCollection()]);
            }]);
}());