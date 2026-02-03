(function () {
    'use strict';
    angular.module(SalesConfiguration.moduleName).controller("ManagePaymentController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "PaymentService", "modalParam", "$templateCache", "Notification", "Toaster",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, paymentService, modalParam, $templateCache, notification, toaster) {

            var vm = this;
            vm.franchiseeId = modalParam.FranchiseeId;
            vm.saveChargeCard = saveChargeCard;
            vm.getInstrumentList = getInstrumentList;
            vm.manageCard = manageCard;
            vm.deleteCard = deleteCard;
            vm.setPrimary = setPrimary;
            vm.checkExpiry = checkExpiry;
            vm.inValidExpirydate = false;
            vm.isProcessing = false;
            vm.model = {};
            vm.echeckModel = {};
            vm.saveECheck = saveECheck;
            vm.InstrumetType = DataHelper.InstrumentType;
            vm.paymentType = vm.InstrumetType.ChargeCard; // 41;
            vm.save = save;
            vm.Roles = DataHelper.Role;

            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function save(form) {
                if (vm.paymentType == vm.InstrumetType.ECheck) {
                    if (vm.eCheckModel == null) return;
                    saveECheck(vm.eCheckModel, form);
                }
                else if (vm.paymentType == vm.InstrumetType.ChargeCard) {
                    saveChargeCard(vm.model, form);
                }
            }

            function getInstrumentList() {
                return paymentService.getFranchiseeInstrumentList(vm.franchiseeId).then(function (result) {
                    if (result) {
                        vm.cardDetails = result.data;
                    }
                })
            };

            function resetModel(form) {
                vm.model = {};
                form.$setUntouched();
                form.$setPristine();
            }

            function saveChargeCard(chargeCardDetails, form) {
                vm.isProcessing = true;

                return paymentService.saveChargeCard(chargeCardDetails, vm.franchiseeId).then(function (result) {
                    if (result.data != null && result.data.processorResult == 133)
                        toaster.error(result.data.message);
                    else
                        toaster.show(result.data.message);
                    vm.isProcessing = false;
                    if (result.Message == null) {
                        resetModel(form);
                        getInstrumentList();
                    } else {
                        //logger.showToasterSuccess(result.Message);
                    }
                }).catch(function (err) {
                    vm.isProcessing = false;
                });;
            }

            function getInstrumentType() {
                return paymentService.getInstrumentType().then(function (result) {
                    if (result) {
                        vm.instrumentTypes = result.data;
                        var index = vm.instrumentTypes.map(function (e) { return e.display; }).indexOf('Check');
                        if (vm.currentRole != vm.Roles.SuperAdmin)
                            vm.instrumentTypes.splice(index, 1);

                        var cardonfileindex = vm.instrumentTypes.map(function (e) { return e.value; }).indexOf('45');
                        vm.instrumentTypes.splice(cardonfileindex, 1);
                        var echeckonfileindex = vm.instrumentTypes.map(function (e) { return e.value; }).indexOf('46');
                        vm.instrumentTypes.splice(echeckonfileindex, 1);
                        var accountCredit = vm.instrumentTypes.map(function (e) { return e.value; }).indexOf('47');
                        vm.instrumentTypes.splice(accountCredit, 1);
                    }
                });
            }
            function resetECheckModel(form) {
                vm.eCheckModel = {};
                form.$setUntouched();
                form.$setPristine();
            }

            function saveECheck(echeckDetails, form) {
                vm.isProcessing = true;

                return paymentService.saveECheck(echeckDetails, vm.franchiseeId).then(function (result) {
                    if (result.data != null && result.data.processorResult == 133)
                        toaster.error(result.data.message);
                    else
                        toaster.show(result.data.message);
                    vm.isProcessing = false;
                    if (result.Message == null) {
                        resetECheckModel(form);
                        getInstrumentList();
                    } else {
                        //logger.showToasterSuccess(result.Message);
                    }
                }).catch(function (err) {
                    vm.isProcessing = false;
                });;
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

            function getAccountType() {
                return paymentService.getAccountType().then(function (result) {
                    if (result) {
                        vm.accountType = result.data;
                    }
                });
            }

            function manageCard(instrumentIds, isActive) {
                if (instrumentIds.indexOf(',') == -1) {
                    instrumentIds = instrumentIds + ',0';
                }
                return paymentService.manageCard(instrumentIds, isActive).then(function (result) {
                    if (result.data != true)
                        toaster.error(result.message.message);
                    else
                        toaster.show(result.message.message);
                    getInstrumentList();
                });
            }

            function deleteCard(instrumentIds) {
                if (instrumentIds.indexOf(',') == -1) {
                    instrumentIds = instrumentIds + ',0';
                }
                notification.showConfirm("Are you sure about deleting the card? ", "Delete Card", function () {
                    return paymentService.deleteCard(instrumentIds).then(function (result) {
                        if (result.data != true)
                            toaster.error(result.message.message);
                        else
                            toaster.show(result.message.message);
                        getInstrumentList();
                    });
                });
            }



            function setPrimary(instrumentIds) {
                if (instrumentIds.indexOf(',') == -1) {
                    instrumentIds = instrumentIds + ',0';
                }
                return paymentService.setPrimary(instrumentIds, vm.franchiseeId).then(function (result) {
                    if (result.data.processorResult == 0)
                        toaster.show(result.data.message);
                    else if (result.data != true)
                        toaster.error(result.data.message);
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

            $q.all([getInstrumentList(), getYears(), getMonths(), getCardType(), getInstrumentType(), getAccountType()]);

        }]);
}());