(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("DurationApprovalController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService", "FileService", "Toaster", "FranchiseeDocumentService",
            "Clock", "$q", '$filter',
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService, fileService, toaster, franchiseeDocumentService,
                clock, $q, $filter) {

                var vm = this;
                vm.isEmpty = false;
                vm.info = {};
                vm.changeDurationStatus = changeDurationStatus;
                vm.searchOptions = [];
                vm.getDurationApprovalList = getDurationApprovalList;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isFrontOffice = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.franchiseeId = modalParam.FranchiseeId != null ? modalParam.FranchiseeId : 0;
                vm.message = modalParam.Message != null ? modalParam.Message : '';
                vm.searchOptions.push({ display: 'Pending', value: 153 });
                vm.searchOptions.push({ display: 'Approved', value: 151 });
                vm.searchOption = 153;
                function getDurationApprovalList() {
                    return franchiseeService.getDurationApprovalList(vm.franchiseeId).then(function (result) {
                        if (result.data != null) {
                            vm.durationApprovalList = result.data.collection;
                            var approvalCount = $filter('filter')(vm.durationApprovalList, { statusId: 153 }, true);
                            vm.isEmpty = approvalCount.length > 0 ? false : true;
                            vm.isProcessing = false;
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }
                function changeDurationStatus(item, statusId) {

                    vm.info.statusId = statusId;
                    vm.info.id = item.id;
                    return franchiseeService.changeDurationStatus(vm.info).then(function (result) {
                        if (result.data != null) {
                            if (result.data) {
                                item.statusId = statusId;
                                getDurationApprovalList();

                                toaster.show(result.message.message);
                            }
                            else
                                toaster.error(result.message.message);
                            vm.isProcessing = false;
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                $q.all([getDurationApprovalList()]);

            }]);
}());