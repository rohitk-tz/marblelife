(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("LoanServiceController",
        ["$scope", "$rootScope", "$uibModalInstance", "modalParam", "FranchiseeService", "Toaster", "$filter", "Notification", "$q", "FileService", "Clock", "$anchorScroll","$location",
            function ($scope, $rootScope, $uibModalInstance, modalParam, franchiseeService, toaster, $filter, notification, $q, fileService, clock, $anchorScroll, $location) {

                var vm = this;
                vm.isOverPay = false;
                vm.overPrePayAmount = overPrePayAmount;
                vm.franchiseeName = modalParam.Franchisee != null ? modalParam.Franchisee.name : null;
                vm.franchiseeId = modalParam.FranchiseeId;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                vm.dateTime = new Date(clock.now());
                vm.minDate = moment(vm.dateTime).format("MM/DD/YYYY");
                vm.getLoanList = getLoanList;
                vm.deleteLoan = deleteLoan;
                vm.deleteLoan = deleteLoan;
                vm.createLoan = false;
                vm.disableOption = false;
                vm.openModel = openModel;
                vm.closeModel = closeModel;
                vm.getReport = getReport;
                vm.loan = { FranchiseeId: vm.franchiseeId, TypeId: DataHelper.ServiceFeeType.Loan, IsAdfund: false, isRoyality: true };
                vm.changeLoanType = { IsRoyality: true, LoanId: DataHelper.ServiceFeeType.Loan };
                vm.isProcessing = false;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.save = save;
                vm.typeId = DataHelper.ServiceFeeType.Loan;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isRoyality = isRoyality;
                vm.changeLoanAdjustment = changeLoanAdjustment;
                vm.changeLoanAdjustmentMethod = changeLoanAdjustmentMethod;
                vm.saveChangeLoanAdjustment = saveChangeLoanAdjustment;
                vm.isEditable = false;
                vm.exportLoan = exportLoan;
                $scope.loanAdjustmentOptions = [{ name: 'Royalty', value: '1' }, { name: 'Adfund', value: '0' }];

                vm.searchOptions = [{ display: 'Other', value: 247 }, { display: 'ISQFT', value: 244 }, { display: 'Surgical Strike', value: 245 }, { display: 'Geofence', value: 246 }];
                if (vm.isFranchiseeAdmin) {
                    vm.franchiseeName = $rootScope.identity.organizationName;
                }

                function getReport() { }
                function save() {
                    /*vm.overPrePay = {};*/
                    if (vm.isOverPay) {
                        if (vm.loan.prePayAmount == 0 || vm.loan.prePayAmount == '') {
                            toaster.error("Pre Paid Amount should be greater than 0");
                            return;
                        }
                        vm.overPrePay.prePayAmount = vm.loan.prePayAmount;
                        vm.overPrePay.id = vm.loan.id;
                        vm.overPrePay.loanTypeId = vm.loan.loanTypeId;
                        vm.isOverPay = false;
                        return franchiseeService.savePrePayAmount(vm.overPrePay).then(function (result) {
                            vm.loan.prePayAmount = 0;
                            toaster.show(result.message.message);
                            getLoanList();
                        })
                        return;
                    }

                    if (vm.loan.startDate == '' || vm.loan.startDate == null || vm.loan.startDate == undefined) {
                        toaster.error("Start Date Should not be empty!");
                        return;
                    }
                    var todayDate = moment(vm.dateTime).format("MM/DD/YYYY");
                    var startDate = moment(vm.loan.startDate).format("MM/DD/YYYY");
                    vm.loan.startDate = moment(vm.loan.startDate).format("MM/DD/YYYY");

                    var startDateOrginal = new Date(startDate);
                    var todayDateOriginal = new Date(todayDate);

                    if (startDateOrginal < todayDateOriginal) {
                        toaster.error("Start Date Should be Greater than Today's Date!");
                        return;
                    }
                    vm.isProcessing = true

                    return franchiseeService.saveServiceFee(vm.loan, vm.franchiseeId).then(function (result) {
                        if (result) {
                            //toaster.show(result.message.message);
                            toaster.show("Loan Saved Successfully.");
                            getLoanList();
                            closeModel();
                            vm.loan = { FranchiseeId: vm.franchiseeId, TypeId: DataHelper.ServiceFeeType.Loan };
                        }
                        else toaster.error(result.message.message);
                        vm.isProcessing = false;
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                function openModel() {
                    getFranchiseeRoyality();
                    vm.createLoan = true;
                    vm.disableOption = true;
                }

                function closeModel() {
                    vm.loan.Description = null;
                    vm.loan.prePayAmount = 0;
                    vm.loan.amount = 0;
                    vm.loan.startDate = null;
                    vm.isOverPay = false;
                    vm.createLoan = false;
                    vm.createLoan = false;
                    vm.disableOption = false;
                    vm.loan = { FranchiseeId: vm.franchiseeId, TypeId: DataHelper.ServiceFeeType.Loan };
                }

                function deleteLoan(id) {
                    notification.showConfirm("Are you sure about deleting the record?", "Delete Loan", function () {
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
                                getLoanList();
                            }
                        });
                    });
                }

                function isRoyality(isRoyality) {
                    if (isRoyality) {
                        vm.loan.IsAdfund = false;
                        vm.loan.isRoyality = true;
                    }
                    else {
                        vm.loan.IsAdfund = true;
                        vm.loan.isRoyality = false;
                    }
                }


                function getLoanList() {
                    return franchiseeService.getLoanList(vm.franchiseeId).then(function (result) {
                        if (result != null) {
                            vm.list = result.data.collection;
                            vm.balanceLeft = result.data.balance;
                            vm.balanceAlreadyPaid = result.data.amountPaid;
                        }
                    });
                }

                function changeLoanAdjustment(item) {
                    item.isEditable = true;
                    item.isEditing = true;
                }

                function changeLoanAdjustmentMethod(item, loanAdjustmentId) {
                    if (loanAdjustmentId == "1")
                        item.loanAdjustment = "Royalty";
                    else
                        item.loanAdjustment = "Adfund"
                    item.loanAdjustmentId = loanAdjustmentId;
                }
                function saveChangeLoanAdjustment(item) {
                    vm.changeLoanType.LoanId = item.id;
                    vm.changeLoanType.IsRoyality = item.isRoyality;
                    if (item.loanAdjustmentId == "1")
                        vm.changeLoanType.IsRoyality = 1;
                    else
                        vm.changeLoanType.IsRoyality = 0;

                    notification.showConfirm("Are you sure about changing  Loan Adjustment?", "Change Loan Adjustment", function () {
                        return franchiseeService.saveChangeFeeAdjustment(vm.changeLoanType).then(function (result) {
                            if (!result.data) {
                                toaster.error("Error in Changing Franchisee Loan Type.");
                            }
                            else {
                                toaster.show("Franchisee Loan Type Changed succesfully.");
                                getLoanList();

                            }

                        });
                    });
                }
                function getFranchiseeRoyality() {
                    return franchiseeService.getFranchiseeRoyality(vm.franchiseeId).then(function (result) {
                        if (result != null)
                            vm.loan.isRoyality = result.data;
                        if (vm.loan.isRoyality) {
                            vm.loan.IsAdfund = false;
                        }
                        else
                            vm.loan.IsAdfund = true;
                        vm.loan.loanTypeId = 247;
                    });
                }
                function exportLoan(loanId) {
                    return franchiseeService.exportLoan(loanId).then(function (result) {
                        var fileName = "FranchiseeLoan.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                    });
                }
                function overPrePayAmount(item) {
                    vm.overPrePay = {};
                    vm.loan.isRoyality = item.isRoyality;
                    vm.loan.IsAfund = item.isAdfund;
                    vm.loan.Amount = item.amount;
                    vm.loan.duration = item.duration;
                    vm.loan.percentage = item.percentage;
                    vm.loan.Description = item.description;
                    vm.loan.startDate = moment((item.startDate)).format("MM/DD/YYYY");
                    vm.loan.id = item.id;
                    vm.loan.loanTypeId = item.loanTypeId;
                    
                    //$window.scrollTo(0, 0);
                    $location.hash('top');
                    $anchorScroll();
                }
                $q.all([getLoanList()]);
            }]);
}());