(function () {
    'use strict';
    angular.module(SalesConfiguration.moduleName).controller("PaymentController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "PaymentService", "modalParam", "Notification",
            "Toaster", "FranchiseAccountCreditService", "APP_CONFIG",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, paymentService, modalParam, notification, toaster,
            franchiseAccountCreditService, config) {

            var vm = this;
            vm.franchiseeId = modalParam.FranchiseeId;
            vm.invoiceId = modalParam.InvoiceId;
            vm.currencyRate = modalParam.CurrencyRate;
            vm.accountTypeId = modalParam.AccountTypeId;
            vm.adjustAccountCredit = adjustAccountCredit;
            vm.currencyCode = $rootScope.identity.currencyCode;
            vm.defaultCurrency = config.defaultCurrency;
            vm.isLoanOverPayment = false;
            vm.getInstrumentList = getInstrumentList;
            vm.setPrimary = setPrimary;
            vm.checkExpiry = checkExpiry;
            vm.inValidExpirydate = false;
            vm.isChargeCardPaymentProfile = true;
            vm.isCheckPaymentProfile = false;
            vm.resetPaymentProfile = resetPaymentProfile;
            vm.save = save;
            vm.resetCard = resetCard;
            vm.eCheckModel = { saveOnFile: false };
            vm.model = { saveOnFile: false };
            vm.chargeCardPaymentEditModel = { saveOnFile: false };
            vm.checkModel = {};
            vm.isProcessing = false;
            vm.paymentType = DataHelper.InstrumentType.ChargeCard; //41;
            vm.isSaveCard = isSaveCard;
            vm.isSaveECheck = isSaveECheck;
            vm.currentRole = $rootScope.identity.roleId;
            vm.instrumentType = DataHelper.InstrumentType;
            vm.Roles = DataHelper.Role;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.pageSize = 10;
            vm.pageNumber = 1;
            vm.getInvoicePaymentInfo = getInvoicePaymentInfo;

            function getInvoicePaymentInfo() {
                return paymentService.getInvoicePaymentInfo(vm.invoiceId).then(function (result) {
                    if (result.data != null) {
                        vm.invoiceInfo = result.data;
                        vm.franchisee = vm.invoiceInfo.franchiseeName;
                        vm.payableAmount = vm.invoiceInfo.grandTotal;
                        vm.currency = vm.invoiceInfo.currencyCode;
                        vm.loanAmount = vm.invoiceInfo.loanAmount;
                        vm.remainingAmount = vm.invoiceInfo.remainingLoanAmount;
                    }
                })
            }

            function resetCard() {
                vm.model = vm.selectedCard;
            }

            function getAccountCredit() {
                return franchiseAccountCreditService.getAccountCreditforInvoice(vm.franchiseeId, vm.invoiceId).then(function (result) {
                    vm.accountCredit = result.data;
                    vm.totalAmountByCategory = result.data.sumByCategory;
                })
            }

            function adjustAccountCredit(invoiceId) {
                vm.adjustingCredit = true;
                return paymentService.adjustAccountCredit(vm.franchiseeId, invoiceId).then(function (result) {
                    $uibModalInstance.close();
                    if (result.data.processorResult == 131) // to do : replace with lookup value
                    {
                        toaster.show(result.data.message);
                    }
                    else
                        toaster.error(result.data.message);
                    vm.adjustingCredit = false;
                });

            }

            function save(form) {
                if (vm.overPaymentAmount !=undefined)
                {
                    vm.isLoanOverPayment = true;
                }
                if (vm.paymentType == vm.instrumentType.Check) {
                    if (vm.checkModel == null) return;
                    if (vm.isLoanOverPayment && vm.overPaymentAmount > 0) {
                        vm.checkModel.isLoanOverPayment = vm.isLoanOverPayment;
                        vm.checkModel.overPaymentAmount = vm.overPaymentAmount;
                    }
                    saveCheck(vm.checkModel, form);
                }
                else if (vm.paymentType == vm.instrumentType.OnFileChargeCard) {
                    if (vm.selectedCard == null) return;
                    if (vm.isLoanOverPayment && vm.overPaymentAmount > 0) {
                        vm.selectedCard.isLoanOverPayment = vm.isLoanOverPayment;
                        vm.selectedCard.overPaymentAmount = vm.overPaymentAmount;
                    }
                    makePaymentByOnFileCard(vm.selectedCard, form);
                }
                else if (vm.paymentType == vm.instrumentType.ECheck) {
                    if (vm.eCheckModel == null) return;
                    vm.eCheckModel.instrumentTypeId = vm.instrumentType.ECheck;
                    MakePaymentByECheck(vm.eCheckModel, form);
                }
                else if (vm.paymentType == vm.instrumentType.OnFileECheck) {
                    if (vm.eCheckModel == null) return;
                    vm.selectedCard.instrumentTypeId = vm.instrumentType.OnFileECheck;
                    if (vm.isLoanOverPayment && vm.overPaymentAmount > 0) {
                        vm.selectedCard.isLoanOverPayment = vm.isLoanOverPayment;
                        vm.selectedCard.overPaymentAmount = vm.overPaymentAmount;
                    }
                    MakePaymentByECheck(vm.selectedCard, form);
                }
                else {
                    fillPaymentModel(vm.model);
                    if (vm.isLoanOverPayment && vm.overPaymentAmount > 0) {
                        vm.chargeCardPaymentEditModel.isLoanOverPayment = vm.isLoanOverPayment;
                        vm.chargeCardPaymentEditModel.overPaymentAmount = vm.overPaymentAmount;
                    }
                    makePayment(vm.chargeCardPaymentEditModel, form);
                }
            }

            function fillPaymentModel(model) {
                vm.chargeCardPaymentEditModel.chargeCardEditModel = model;
                vm.chargeCardPaymentEditModel.saveOnFile = model.saveOnFile;
            }

            function isSaveCard(saveOnFile) {
                vm.model.saveOnFile = saveOnFile;
            }

            function isSaveECheck(saveOnFile) {
                vm.eCheckModel.saveOnFile = saveOnFile;
            }

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getInstrumentList() {
                paymentService.getInstrumentList(vm.franchiseeId, vm.accountTypeId).then(function (result) {
                    if (result) {
                        vm.cardDetails = result.data;
                    }
                })
            };


            function getAccountType() {
                return paymentService.getAccountType().then(function (result) {
                    if (result) {
                        vm.accountType = result.data;
                    }
                });
            }
            function getInstrumentType() {
                return paymentService.getInstrumentType().then(function (result) {
                    if (result) {
                        vm.instrumentTypes = result.data;
                        var index = vm.instrumentTypes.map(function (e) { return e.display; }).indexOf('Check');
                        if (vm.currentRole != vm.Roles.SuperAdmin)
                            vm.instrumentTypes.splice(index, 1);
                        var accountCredit = vm.instrumentTypes.map(function (e) { return e.value; }).indexOf('47');
                        vm.instrumentTypes.splice(accountCredit, 1);
                    }
                });
            }

            function resetPaymentProfile() {
                if (vm.isChargeCardPaymentProfile == true) {
                    vm.isChargeCardPaymentProfile = false;
                    vm.isCheckPaymentProfile = true;
                }
                else if (vm.isCheckPaymentProfile == true) {
                    vm.isChargeCardPaymentProfile = true;
                    vm.isCheckPaymentProfile = false;
                }
                else {
                    vm.isChargeCardPaymentProfile = true;
                    vm.isCheckPaymentProfile = false;
                }
            }

            function resetCheckModel(form) {
                vm.checkModel = {};
                form.$setUntouched();
                form.$setPristine();
            }
            function saveCheck(checkDetails, form) {
                vm.isProcessing = true;
                checkDetails.profileTypeId = vm.accountTypeId;
                return paymentService.saveCheck(checkDetails, vm.invoiceId, vm.franchiseeId).then(function (result) {
                    if (result.data != null) {
                        toaster.show(result.data.message);
                        $uibModalInstance.close();
                        vm.isProcessing = false;
                    }
                    resetCheckModel(form);
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }

            function resetChrageCardModel(form) {
                vm.model = { saveOnFile: false };
                form.$setUntouched();
                form.$setPristine();
                vm.selectedCard == null;
            }
            function resetSelectedcard(form) {
                vm.selectedCard = null;
                form.$setUntouched();
                form.$setPristine();
            }
            function reseteCheckModel(form) {
                vm.eCheckModel = { saveOnFile: false };
                form.$setUntouched();
                form.$setPristine();
            }

            function makePayment(paymentDetails, form) {
                vm.isProcessing = true;
                paymentDetails.profileTypeId = vm.accountTypeId;
                return paymentService.makePayment(paymentDetails, vm.invoiceId, vm.franchiseeId).then(function (result) {
                    if (result.data != null) {
                        if (result.data.processorResult != 131) {
                            vm.isProcessing = false;
                            toaster.error(result.data.message);
                            resetChrageCardModel(form);
                        }
                        else {
                            toaster.show(result.data.message);
                            $uibModalInstance.close();
                        }
                    }
                    else {
                        resetChrageCardModel(form);
                        getInstrumentList();
                        vm.isProcessing = false;
                    }
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }

            function makePaymentByOnFileCard(paymentDetails, form) {
                vm.isProcessing = true;
                paymentDetails.profileTypeId = vm.accountTypeId;
                return paymentService.makePaymentByOnFileCard(paymentDetails, vm.invoiceId, vm.franchiseeId).then(function (result) {
                    toaster.show(result.data.message);
                    $uibModalInstance.close();
                    if (result.Message == null) {
                        resetSelectedcard(form);
                        getInstrumentList();
                        vm.isProcessing = false;
                    } else {
                        toaster.error(result.data.message);
                        vm.isProcessing = false;
                    }
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }
            function MakePaymentByECheck(paymentDetails, form) {
                vm.isProcessing = true;
                paymentDetails.profileTypeId = vm.accountTypeId;
                return paymentService.MakePaymentByECheck(paymentDetails, vm.invoiceId, vm.franchiseeId).then(function (result) {
                    if (result.data.processorResult != 131) // to do : replace with lookup value
                    {
                        vm.isProcessing = false;
                        toaster.error(result.data.message);
                        reseteCheckModel(form);
                    }
                    else {
                        toaster.show(result.data.message);
                        $uibModalInstance.close();
                        vm.isProcessing = false;
                    }
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }

            function getYears() {
                return paymentService.getYears().then(function (result) {
                    if (result) {
                        vm.years = result.data;
                    }
                });
            }

            function getMonths() {
                return paymentService.getMonths().then(function (result) {
                    if (result) {
                        vm.months = result.data;
                    }
                });
            }

            function getCardType() {
                return paymentService.getCardType().then(function (result) {
                    if (result) {
                        vm.cardType = result.data;
                    }
                });
            }

            function setPrimary(instrumentId, franchiseePaymentProfileId) {
                return paymentService.setPrimary(instrumentId, franchiseePaymentProfileId).then(function (result) {
                    if (result.data != true)
                        toaster.error(result.data.message);
                    else
                        toaster.show(result.data.message);
                    getInstrumentList();
                });
            }

            function checkExpiry(month, year) {
                if (month == null || year == null)
                    return;
                vm.inValidExpirydate = false;
                return paymentService.checkExpiry(month, year).then(function (result) {
                    if (result.data == false) {
                        vm.inValidExpirydate = true;
                    }
                });
            }

            $q.all([getInstrumentList(), getYears(), getMonths(), getCardType(), getAccountType(), getInstrumentType(), getAccountCredit(), getInvoicePaymentInfo()]);

        }]);
}());