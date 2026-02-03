(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("OneTimeProjectFeeController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService", "Toaster", "Notification", "APP_CONFIG",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService, toaster, notification, config) {

                var vm = this;
                vm.franchiseeName = modalParam.Franchisee != null ? modalParam.Franchisee.name : null;
                vm.franchiseeId = modalParam.FranchiseeId;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                vm.isinRoyality = true;
                vm.isinAdFund = false;
                vm.changeAdFundRoyality = changeAdFundRoyality;
                vm.getOTPFeeList = getOTPFeeList;
                vm.deleteOTPFee = deleteOTPFee;
                vm.createOtp = false;
                vm.disableOption = false;
                vm.openModel = openModel;
                vm.closeModel = closeModel;
                vm.oneTimeProjectFee = { FranchiseeId: vm.franchiseeId, TypeId: DataHelper.ServiceFeeType.OneTimeProject };
                vm.isProcessing = false;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.save = save;
                vm.typeId = DataHelper.ServiceFeeType.OneTimeProject;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;

                vm.query =
                {
                    franchiseeId: vm.franchiseeId,
                    isinRoyality: true

                }

                if (vm.isFranchiseeAdmin) {
                    vm.franchiseeName = $rootScope.identity.organizationName;
                }
                function save() {
                    vm.isProcessing = true
                    return franchiseeService.saveServiceFee(vm.oneTimeProjectFee, vm.franchiseeId).then(function (result) {
                        if (result) {
                            toaster.show(result.message.message);
                            getOTPFeeList();
                            closeModel();
                            vm.oneTimeProjectFee = { FranchiseeId: vm.franchiseeId, TypeId: DataHelper.ServiceFeeType.OneTimeProject };
                        }
                        else toaster.error(result.message.message);
                        vm.isProcessing = false;
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                function openModel() {
                    vm.createOtp = true;
                    vm.disableOption = true;
                }

                function closeModel() {
                    vm.createOtp = false;
                    vm.disableOption = false;
                    vm.oneTimeProjectFee = { FranchiseeId: vm.franchiseeId, TypeId: DataHelper.ServiceFeeType.OneTimeProject };
                }

                function deleteOTPFee(id) {
                    notification.showConfirm("Are you sure about deleting the record?", "Delete OneTime Project Fee", function () {
                        return franchiseeService.deleteFee(id, vm.typeId).then(function (result) {
                            if (!result.data.isSuccess) {
                                toaster.error(result.data.response);
                            }
                            else if (result.data.isLastItem && result.data.isSuccess) {
                                toaster.show(result.data.response);
                                $uibModalInstance.close();
                            }
                            else {
                                toaster.show(result.data.response);
                                getOTPFeeList();
                            }
                        });
                    });
                }


                function changeAdFundRoyality(isFromRoyality) {
                    if (isFromRoyality) {
                        vm.isinAdFund = !vm.isinAdFund;
                    }
                    else {
                        vm.isinRoyality = !vm.isinRoyality;
                    }
                    vm.query.isinRoyality = vm.isinRoyality;
                    return franchiseeService.changeAdFundRoyalityStatus(vm.query).then(function (result) {

                    });
                }
                function getOTPFeeList() {
                    return franchiseeService.getOTPFeeList(vm.franchiseeId).then(function (result) {
                        if (result != null)
                            vm.list = result.data.collection;
                        if (result.data.isRoyality) {
                            vm.isinRoyality = true;
                            vm.isinAdFund = false;
                        }
                        else {
                            vm.isinAdFund = true;
                            vm.isinRoyality = false;
                        }
                    });
                }
                getOTPFeeList();
            }]);
}());