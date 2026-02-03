(function () {
    'use strict';
    angular.module(OrganizationsConfiguration.moduleName).controller("EditFranchiseeController",
        ["$state", "$stateParams", "$scope", "$q", "FranchiseeService", "PhoneService", "AddressService", "Notification", "Toaster",
            "APP_CONFIG", "$uibModal", "Clock", "FileService", "$window", "URLAuthenticationServiceForEncryption",
            function ($state, $stateParams, $scope, $q, franchiseeService, phoneService, addressService, notification, toaster,
                config, $uibModal, clock, fileService, $window, URLAuthenticationServiceForEncryption) {
                var vm = this;
                vm.isMexico = false;
                vm.isSchedulerSame = true;
                vm.saveValue = saveValue;
                vm.timeClicked = timeClicked;
                vm.isSameForMarketing = true;
                vm.isTimeValid = true;
                vm.isFranchiseeIdVisible = true;
                vm.totalSum = totalSum;
                vm.performanceHistry = performanceHistry;
                vm.performanceParameter = DataHelper.PerformanceParamter;
                vm.franchiseeId = $stateParams.id != null ? $stateParams.id : 0;
                vm.languageList = [{ display: "English", value: 249 }, { display: "Spanish", value: 250 }];
                vm.franchisee = {};
                $scope.model = {}
                vm.isEdit = URLAuthenticationServiceForEncryption.decrypt(vm.franchiseeId) > 0;
                vm.isDisabled = false;
                vm.isRpiPresent = false;
                $scope.isImage = false;
                vm.performanceFilter = [];
                vm.optionId = 2;
                vm.optionChoose = [{ display: "Business ID", value: 1 }, { display: "RPID", value: 2 }];
                vm.paymentFrequency = [{ display: "Weekly", value: 31 }, { display: "Monthly", value: 32 }];
                vm.payRollFrequency = [{ display: "Monthly", value: 32 }, { display: "Twice A Month", value: 33 }];
                vm.seoCostBillingPeriodId = 1;
                vm.seoCostBillingPeriod = [{ display: "1st week", value: 1 }, { display: "2nd week", value: 2 }];
                vm.serviceTypes = [];
                vm.cancel = cancel;
                vm.isGeneric = isGeneric;
                vm.save = save;
                vm.isRequired = false;
                vm.imagesrc = "/Content/images/no_image_thumb.gif";
                vm.resetFeeProfileModel = resetFeeProfileModel;
                vm.isRoyalityLateFeeApplicable = isRoyalityLateFeeApplicable;
                vm.isSalesDateLateFeeApplicable = isSalesDateLateFeeApplicable;
                vm.isCategoryChange = isCategoryChange;
                vm.downloadUserImage = downloadUserImage;
                vm.techCountChanges = techCountChanges;
                $scope.regex = "^(http[s]?:\\/\\/){0,1}(www\\.){0,1}[a-zA-Z0-9\\.\\-]+\\.[a-zA-Z]{2,5}[\\.]{0,1}";
                vm.info = {};
                vm.info.fileList = [];
                vm.info.fileUploadModel = [];
                vm.delete = deleteImage;
                vm.primaryContact = config.primaryContact;
                vm.secondaryContact = config.secondaryContact;
                vm.contactNumber = config.contactNumber;
                vm.serviceFeeType = DataHelper.ServiceFeeType;
                vm.franchiseeCategoryType = DataHelper.FranchiseeCategories;
                vm.isApplicable = isApplicable;
                vm.isSame = true;
                vm.isSameForCustomer = true;
                vm.isRoyality = isRoyality;
                vm.upload = upload;
                vm.ShowCategoryText = false;
                vm.isGeneric = isGeneric;
                vm.registrationHistry = registrationHistry;
                vm.seoCostStatusId = 1;
                vm.seoCostStatus = [{ display: "Adfund", value: 1 }, { display: "Royalty", value: 2 }];
                vm.changeAdFundRoyalityForSEOCharges = changeAdFundRoyalityForSEOCharges;
                $scope.tempFile = {
                    attachTempImageFile: ""
                };
                $scope.file = {
                    attachedImageFile: ""
                };

                vm.showNotes = showNotes;

                vm.queryAdfundOrRoyalty =
                {
                    franchiseeId: vm.franchiseeId,
                    isSEOInRoyality: true
                }

                function saveValue() {
                    if (vm.franchisee.registrationDate == null) {
                        notification.showConfirm("There is no Registration Date Found for the Franchisee. Without registration date the monthly Royalty will not be calculated. Would you like to proceed without registration date?", "Warning Message:", function () {
                            save();
                        })
                    } else {
                        save();
                    }
                }
                function save() {
                    vm.franchisee.languageId = vm.languageId;
                    vm.franchisee.seoCostBillingPeriodId = vm.franchisee.leadPerformanceEditModel.seoCostBillingPeriodId;
                    vm.form = $scope.franchiseeForm;
                    vm.isDisabled = true;
                    vm.validationOnFields = [];
                    var currentDate = moment(clock.now());
                    validation();
                    if (vm.validationOnFields.length > 0) {
                        var elementId = vm.validationOnFields[0];
                        var getElement = document.getElementById(vm.validationOnFields[0]);
                        getElement.classList.add('red-color');
                        if (elementId == 'country' || elementId == 'state' || elementId == 'ctity' || elementId == 'zip' || elementId == 'address-line-1') {
                            notification.showAlert("Please fill complete address");
                        }
                        document.getElementById(vm.validationOnFields[0]).focus();
                        return;
                    }
                    if (vm.franchisee.webSite == undefined && vm.franchisee.webSite != null) {
                        notification.showAlert('Invalid Franchisee Website Url');
                        document.getElementById('webSite').focus();
                        return;
                    }
                    if (vm.form.webSite.$invalid && vm.franchisee.webSite == undefined) {
                        notification.showAlert('Invalid Franchisee Website Url');
                        document.getElementById('webSite').focus();
                        return;
                    }
                    if (angular.isUndefined(vm.franchisee.renewalDate)) {
                        notification.showAlert('Please Enter Valid Date. It should in MM/DD/YYYY Format');
                        document.getElementById('renewal-date').focus();
                        var getElement = document.getElementById("renewal-date");
                        getElement.classList.add('red-color');
                        return;
                    }
                    if (vm.franchisee.renewalDate != null && vm.franchisee.renewalDate < currentDate) {
                        if (vm.franchisee.renewalDate != null && vm.franchisee.renewalDate < currentDate) {
                            notification.showAlert("Renewal date can't be a past date!");
                            document.getElementById("renewal-date").focus();
                            var getElement = document.getElementById("renewal-date");
                            getElement.classList.add('red-color');
                            vm.isDisabled = false;
                            vm.isRequired = false;
                            return;
                        }
                    }
                    if (addressService.checkIsAddress(vm.franchisee.address)) {
                        if (addressService.checkAddressLine1InComplete(vm.franchisee.address)) {
                            document.getElementById("address-line-1").focus();
                        }
                        else if (addressService.checkCityInComplete(vm.franchisee.address)) {
                            document.getElementById("city").focus();
                        }
                        else if (addressService.checkStateInComplete(vm.franchisee.address)) {
                            document.getElementById("state").focus();
                        }
                        else if (addressService.checkZipCodeInComplete(vm.franchisee.address)) {
                            document.getElementById("zip").focus();
                        }
                        notification.showAlert("Please fill address");
                        vm.isDisabled = false;
                        return;
                    }
                    vm.franchisee.address = addressService.sanitizeAddress(vm.franchisee.address);

                    if (addressService.checkAddressInComplete(vm.franchisee.address)) {
                        if (addressService.checkAddressLine1InComplete(vm.franchisee.address)) {
                            document.getElementById("address-line-1").focus();
                        }
                        else if (addressService.checkCityInComplete(vm.franchisee.address)) {
                            document.getElementById("city").focus();
                        }
                        else if (addressService.checkStateInComplete(vm.franchisee.address)) {
                            document.getElementById("state").focus();
                        }
                        else if (addressService.checkZipCodeInComplete(vm.franchisee.address)) {
                            document.getElementById("zip").focus();
                        }
                        notification.showAlert("Please fill complete address");
                        vm.isDisabled = false;
                        return;
                    }
                    if (phoneService.checkNumberTypeCombination(vm.franchisee.phoneNumbers) == false) {
                        document.getElementById("phoneType").focus();
                        notification.showAlert("Please Enter a valid Phone Number Type");
                        vm.isDisabled = false;
                        return;
                    }
                    vm.franchisee.phoneNumbers = phoneService.sanitizePhoneNumbers(vm.franchisee.phoneNumbers);
                    if (phoneService.checkInvalidPhoneNo(vm.franchisee.phoneNumbers) == false) {
                        document.getElementById("phoneNumber").focus();
                        notification.showAlert("Please Enter a valid Phone Number");
                        vm.isDisabled = false;
                        return;
                    }
                    if (vm.franchisee.phoneNumbers.length > 0) {
                        var isTransferablePresent = false;
                        angular.forEach(vm.franchisee.phoneNumbers, function (phone) {
                            if (phone.isTransferable) {
                                isTransferablePresent = true;
                            }
                        });
                        if (!isTransferablePresent) {
                            document.getElementById("exampleCheck1").focus();
                            notification.showAlert("Please select transferable phone number");
                            vm.isDisabled = false;
                            $window.scrollTo(document.getElementById('phone').offsetTop, 0);
                            return;
                        }

                    }

                    if (vm.franchisee.taxrate != '' || vm.franchisee.taxrate != undefined) {
                        if (vm.franchisee.taxrate > 10) {
                            document.getElementById("taxrate").focus();
                            notification.showAlert("Sales Tax should Be In The Range Of 0-10%.");
                            return;
                        }
                        if (vm.franchisee.taxrate < 0) {
                            document.getElementById("taxrate").focus();
                            notification.showAlert("Sales Tax should Be In The Range Of 0-10%.");
                            return;
                        }
                    }

                    if (vm.franchisee.franchiseeEmailEditModel != null && vm.franchisee.franchiseeEmailEditModel.isTechMailFees
                        && (vm.franchisee.franchiseeEmailEditModel.technianCount == 0)) {
                        document.getElementById("techCount").focus();
                        notification.showAlert("Technician Count  of Franchisee Email Fees  should be greater than 0");
                        vm.isDisabled = false;
                        return;
                    }

                    if (vm.franchisee.franchiseeEmailEditModel != null && vm.franchisee.franchiseeEmailEditModel.isTechMailFees
                        && (vm.franchisee.franchiseeEmailEditModel.technianCount % 1 != 0)) {
                        document.getElementById("techCount").focus();
                        notification.showAlert("Technician Count of Franchisee Email Fees should be Whole Number");
                        vm.isDisabled = false;
                        return;
                    }

                    if (vm.franchisee.franchiseeEmailEditModel != null && vm.franchisee.franchiseeEmailEditModel.isTechMailFees
                        && (vm.franchisee.franchiseeEmailEditModel.multiplacationFactor == 0 || vm.franchisee.franchiseeEmailEditModel.multiplacationFactor == undefined)) {
                        document.getElementById("multiplicationFactor").focus();
                        notification.showAlert("Multiplication Factor  of Franchisee Email Fees  should be greater than 0");
                        vm.isDisabled = false;
                        return;
                    }


                    if (vm.franchisee.franchiseeEmailEditModel != null && (vm.franchisee.franchiseeEmailEditModel.isTechMailFee
                        || vm.franchisee.franchiseeEmailEditModel.isGeneric) && vm.franchisee.franchiseeEmailEditModel.amount == 0
                        || vm.franchisee.franchiseeEmailEditModel.amount == undefined) {
                        document.getElementById("franchiseeMailAmount").focus();
                        notification.showAlert("Amount Of Franchisee Email Fees should be greater than 0");
                        vm.isDisabled = false;
                        return;
                    }

                    if (!vm.isSchedulerSame) {
                        if (vm.franchisee.schedulerFirstName == "" || vm.franchisee.schedulerFirstName == null) {
                            document.getElementById("schedulerFirstName").focus();
                            return;
                        }
                        else if (vm.franchisee.schedulerLastName == "" || vm.franchisee.schedulerLastName == null) {
                            document.getElementById("schedulerLastName").focus();
                            return;
                        }
                        else if (vm.franchisee.schedulerEmail == "" || vm.franchisee.schedulerEmail == null) {
                            document.getElementById("schedulerEmail").focus();
                            return;
                        }
                    }

                    if (vm.isSame) {
                        vm.franchisee.contactFirstName = vm.franchisee.organizationOwner.ownerFirstName;
                        vm.franchisee.contactLastName = vm.franchisee.organizationOwner.ownerLastName;
                        vm.franchisee.contactEmail = vm.franchisee.email;
                    }
                    if (vm.isSameForCustomer) {
                        vm.franchisee.accountPersonFirstName = vm.franchisee.organizationOwner.ownerFirstName;
                        vm.franchisee.accountPersonLastName = vm.franchisee.organizationOwner.ownerLastName;
                        vm.franchisee.accountPersonEmail = vm.franchisee.contactEmail;
                    }
                    if (vm.isSameForMarketing) {
                        vm.franchisee.marketingPersonFirstName = vm.franchisee.organizationOwner.ownerFirstName;
                        vm.franchisee.marketingPersonLastName = vm.franchisee.organizationOwner.ownerLastName;
                        vm.franchisee.marketingPersonEmail = vm.franchisee.contactEmail;
                    }


                    if (vm.isSchedulerSame) {
                        vm.franchisee.schedulerFirstName = vm.franchisee.organizationOwner.ownerFirstName;
                        vm.franchisee.schedulerLastName = vm.franchisee.organizationOwner.ownerLastName;
                        vm.franchisee.schedulerEamil = vm.franchisee.contactEmail;
                    }

                    vm.franchisee.fileUploadModel = vm.info;
                    vm.franchisee.isImageChanged = $scope.isImageChanged;
                    if (!vm.franchisee.franchiseeEmailEditModel.isGeneric && !vm.franchisee.franchiseeEmailEditModel.isTechMailFees) {
                        vm.franchisee.franchiseeEmailEditModel.technianCount = 0;
                    }
                    if (vm.franchisee.isReviewFeedbackEnabled) {
                        if (vm.franchisee.reviewURL == "" || vm.franchisee.reviewURL == null || vm.franchisee.reviewURL == undefined) {
                            notification.showAlert("Please Enter the Review URL");
                            return;
                        }
                    }
                    if (vm.franchisee.isReviewFeedbackEnabled) {
                        if (vm.franchisee.reviewRpId == null || vm.franchisee.reviewRpId == "") {
                            notification.showConfirm("There is no RPID Found for the Franchisee. Without RPID... . Would you like to proceed without RPID?", "Warning Message:", function () {
                                return franchiseeService.saveFranchisee(vm.franchisee).then(function (result) {
                                    if (result.data) {
                                        toaster.show(result.message.message);
                                        $state.go('core.layout.franchisee.list');
                                    }
                                    else {
                                        toaster.error(result.message.message);
                                    }

                                }).catch(function (err) {
                                    vm.isDisabled = false;
                                });
                            }, function () {
                                document.getElementById("rpIds").focus();
                                vm.isDisabled = false;
                                return;
                            })
                        }
                        else {
                            return franchiseeService.saveFranchisee(vm.franchisee).then(function (result) {
                                if (result.data) {
                                    toaster.show(result.message.message);
                                    $state.go('core.layout.franchisee.list');
                                }
                                else {
                                    toaster.error(result.message.message);
                                }

                            }).catch(function (err) {
                                vm.isDisabled = false;
                            });
                        }
                    }
                    else {
                        return franchiseeService.saveFranchisee(vm.franchisee).then(function (result) {
                            if (result.data) {
                                toaster.show(result.message.message);
                                $state.go('core.layout.franchisee.list');
                            }
                            else {
                                toaster.error(result.message.message);
                            }

                        }).catch(function (err) {
                            vm.isDisabled = false;
                        });
                    }

                }

                function cancel() {
                    $state.go('core.layout.franchisee.list')
                }

                function resetFeeProfileModel() {
                    franchiseeService.resetFeeProfile(vm.franchisee.feeProfile);
                }

                function init() {
                    return franchiseeService.getFranchisee(vm.franchiseeId).then(function (result) {
                        if (result == undefined || result.data == null || result.data.id == 0 || result.data.id == undefined) {
                            $state.go("core.layout.pageNotFound");
                        }
                        else {
                            if (result.data.address[0].countryId == 8) {
                                vm.isMexico = true;
                            }
                            else {
                                vm.isMexico = false;
                            }
                            vm.languageId = result.data.languageId;
                            $scope.$broadcast('getCurrency', result.data.currency);
                            $scope.model.currency = result.data.currency;
                            vm.is0Franchisee = result.data.is0Franchisee;
                            if (result.data.categoryId != null) {
                                isCategoryChange(result.data.categoryId);
                            }
                            if (result.data.fileName != null) {
                                if (result.data.fileName != "") {
                                    vm.fileName = result.data.fileName;
                                    fileService.getFileStreamByUrl(result.data.fileName).then(function (result) {
                                        $scope.imageUrl = fileService.getStreamUrl(result);
                                        vm.imagesrc = $scope.imageUrl;
                                        vm.index = 1;
                                        $scope.isImage = true;
                                    })
                                }
                            }
                            vm.franchisee = result.data;
                            if (vm.franchisee.reviewRpId != null) {
                                vm.franchisee.reviewRpId = vm.franchisee.reviewRpId.toString();
                                vm.isRpiPresent = true;
                            }
                            else {
                                vm.isRpiPresent = false;
                            }
                            totalSum();
                            if (result.data.franchiseeEmailEditModel != null) {
                                var franchiseeEmailEditModel = result.data.franchiseeEmailEditModel;
                                if (vm.franchisee.franchiseeEmailEditModel == undefined
                                    || vm.franchisee.franchiseeEmailEditModel == null) {
                                }
                                vm.franchisee.franchiseeEmailEditModel.isGeneric = franchiseeEmailEditModel.isGeneric;
                                vm.franchisee.franchiseeEmailEditModel.isTechMailFees = franchiseeEmailEditModel.isTechMailFees;
                            }
                            if (vm.franchisee.isRoyality == false) {
                                vm.franchisee.IsAdfund = true;
                            }
                            else {
                                vm.franchisee.IsAdfund = false;
                            }
                            if (vm.franchisee.organizationOwner.ownerFirstName == vm.franchisee.contactFirstName && vm.franchisee.contactLastName == vm.franchisee.organizationOwner.ownerLastName && vm.franchisee.contactEmail == vm.franchisee.email) {
                                vm.isSame = true;
                                vm.franchisee.contactFirstName = "";
                                vm.franchisee.contactLastName = "";
                                vm.franchisee.contactEmail = "";
                            }
                            else {
                                vm.isSame = false;
                            }


                            if (vm.franchisee.organizationOwner.ownerFirstName == vm.franchisee.accountPersonFirstName
                                && vm.franchisee.organizationOwner.ownerLastName == vm.franchisee.accountPersonLastName
                                && vm.franchisee.email == vm.franchisee.accountPersonEmail) {
                                vm.isSameForCustomer = true;
                                vm.franchisee.accountPersonFirstName = "";
                                vm.franchisee.accountPersonLastName = "";
                                vm.franchisee.accountPersonEmail = "";
                            }
                            else {
                                if (vm.franchisee.accountPersonFirstName == ''
                                    && vm.franchisee.accountPersonLastName == ''
                                    && vm.franchisee.accountPersonEmail == '') {
                                    vm.isSameForCustomer = true;
                                }
                                else {
                                    vm.isSameForCustomer = false;
                                }

                            }

                            if (vm.franchisee.organizationOwner.ownerFirstName == vm.franchisee.marketingPersonFirstName
                                && vm.franchisee.organizationOwner.ownerLastName == vm.franchisee.marketingPersonLastName
                                && vm.franchisee.email == vm.franchisee.marketingPersonEmail) {
                                vm.isSameForMarketing = true;
                                vm.franchisee.marketingPersonFirstName = "";
                                vm.franchisee.marketingPersonLastName = "";
                                vm.franchisee.marketingPersonEmail = "";
                            }
                            else {
                                if (vm.franchisee.marketingPersonFirstName == null
                                    && vm.franchisee.marketingPersonLastName == null
                                    && vm.franchisee.marketingPersonEmail == null) {
                                    vm.isSameForMarketing = true;
                                }
                                else {
                                    vm.isSameForMarketing = false;
                                }

                            }


                            //////
                            if (vm.franchisee.organizationOwner.ownerFirstName == vm.franchisee.schedulerFirstName
                                && vm.franchisee.organizationOwner.ownerLastName == vm.franchisee.schedulerLastName
                                && vm.franchisee.email == vm.franchisee.schedulerEamil) {
                                vm.isSchedulerSame = true;
                                vm.franchisee.schedulerLastName = "";
                                vm.franchisee.schedulerLastName = "";
                                vm.franchisee.schedulerLastName = "";
                            }
                            else {
                                if (vm.franchisee.schedulerFirstName == null
                                    && vm.franchisee.schedulerLastName == null
                                    && vm.franchisee.schedulerEamil == null) {
                                    vm.isSchedulerSame = true;
                                }
                                else {
                                    vm.isSchedulerSame = false;
                                }

                            }

                            ////
                            if (vm.franchisee.feeProfile != null) {
                                vm.franchisee.feeProfile.adFundPercentage = vm.franchisee.feeProfile.adFundPercentage.toString();
                                vm.franchisee.feeProfile.minimumRoyaltyPerMonth = vm.franchisee.feeProfile.minimumRoyaltyPerMonth.toString();
                            }

                            if (vm.franchisee.renewalDate != null) {
                                vm.franchisee.renewalDate = moment(vm.franchisee.renewalDate).toDate();
                            }
                            if (vm.franchisee.registrationDate != null) {
                                vm.franchisee.registrationDate = moment(vm.franchisee.registrationDate).toDate();
                            }
                            //convert to string
                            if (vm.franchisee != null && vm.franchisee.phoneNumbers != null) {
                                angular.forEach(vm.franchisee.phoneNumbers, function (phone) {
                                    if (phone.phoneType != null)
                                        phone.phoneType = phone.phoneType.toString();
                                });
                            }
                            if (vm.franchiseeId <= 0)
                                setDefaultServicefee(vm.franchisee.serviceFees);
                        }
                    });
                }


                function getFranchiseeIRPId() {
                    return franchiseeService.getFranchiseeRPId(vm.franchiseeId).then(function (result) {
                        if (result != null) {
                            vm.rpIds = result.data;
                        }
                    });
                }
                function setDefaultServicefee() {
                    angular.forEach(vm.franchisee.serviceFees, function (fee) {
                        fee.franchiseeId = vm.franchiseeId;
                        setValues(fee);
                    });
                }

                function setValues(fee) {
                    if (fee.typeId == vm.serviceFeeType.Bookkeeping) {
                        fee.amount = config.defaultBookkeepingAmount;
                        fee.percentage = config.defaultSalesPercentage;
                        fee.frequencyId = config.defaultFrequency;
                    }
                    else if (fee.typeId == vm.serviceFeeType.PayRollProcessing) {
                        fee.amount = config.defaultPayrollAmount;
                        fee.frequencyId = config.defaultFrequency;
                    }
                    else if (fee.typeId == vm.serviceFeeType.Recruiting) {
                        fee.amount = config.defaultRecruitmentAmount;
                    }
                    else if (fee.typeId == vm.serviceFeeType.NationalCharge) {
                        fee.percentage = defaultNationalChargePercentage;
                    }
                }

                function isApplicable(fee) {
                    if (fee.isApplicable) {
                        setValues(fee);
                    }
                }
                function isRoyalityLateFeeApplicable() {
                    if (vm.franchisee.lateFee.isRoyalityLateFeeApplicable == false) {
                        vm.franchisee.lateFee.royalityLateFee = 0;
                        vm.franchisee.lateFee.royalityWaitPeriodInDays = 0;
                        vm.franchisee.lateFee.royalityInterestRate = 0;
                    } else {
                        vm.franchisee.lateFee.royalityLateFee = 50;
                        vm.franchisee.lateFee.royalityWaitPeriodInDays = 2;
                        vm.franchisee.lateFee.royalityInterestRate = 18;
                    }
                }
                function isRoyality(isRoyality) {
                    if (isRoyality) {
                        vm.franchisee.IsAdfund = false;
                        vm.franchisee.isRoyality = true;
                    }
                    else {
                        vm.franchisee.IsAdfund = true;
                        vm.franchisee.isRoyality = false;
                    }
                }
                function isSalesDateLateFeeApplicable() {
                    if (vm.franchisee.lateFee.isSalesDateLateFeeApplicable == false) {
                        vm.franchisee.lateFee.salesDataLateFee = 0;
                        vm.franchisee.lateFee.salesDataWaitPeriodInDays = 0;
                    } else {
                        vm.franchisee.lateFee.salesDataLateFee = 50;
                        vm.franchisee.lateFee.salesDataWaitPeriodInDays = 1;
                    }
                }
                function downloadUserImage(fileId) {
                    return fileService.getFileForDownload(fileId).then(function (result) {
                        fileService.downloadFileImage(result.data, vm.fileName);
                    });
                }

                function deleteImage() {
                    $scope.isImage = false;
                    vm.franchisee.fileId = null;
                    var imgs = document.getElementsByTagName('img');
                    imgs[3].src = "/Content/images/no_image_thumb.gif";
                    $scope.isImageChanged = true;
                    var id = "logo-image";
                    $scope.file.attachedImageFile = ""
                    var myElem = document.getElementById(id);
                    vm.index = 0;
                }

                $scope.readUrl = fileService.readLocalFile;
                $scope.$watch('tempFile.attachTempImageFile', function (newValue, oldValue) {
                    $scope.file.attachedImageFile = $scope.tempFile.attachTempImageFile;
                    if ($scope.tempFile.attachTempImageFile.type == "image/png" || $scope.tempFile.attachTempImageFile.type == "image/jpg"
                        || $scope.tempFile.attachTempImageFile.type == "image/jpeg" || $scope.tempFile.attachTempImageFile.type == "image/psd"
                        || $scope.tempFile.attachTempImageFile.type != undefined) {
                        $scope.isImage = true;
                        var image = document.getElementById('logo-image');
                        image.style.transform = "rotate(" + -0 + "deg)";
                        fileService.uploadFile($scope.file.attachedImageFile).then(function (result) {
                            var width = image.naturalWidth;
                            var height = image.naturalHeight;
                            vm.info.fileList.push(result.data);
                            $scope.isInValidImage = false;
                        });
                    }
                    else {
                        vm.imagesrc = "/Content/images/no_image_thumb.gif";
                    }
                });

                function upload() {
                    $('#file_input ').click();
                }
                function isCategoryChange(franchiseeCategoryId) {
                    if (franchiseeCategoryId == vm.franchiseeCategoryType.FRONTOFFICE) {
                        vm.franchisee.categoryId = vm.franchiseeCategoryType.FRONTOFFICE;
                        vm.IsFrontOffice = true;
                        vm.IsOfficePerson = false;
                        vm.IsResponseWhenAvailable = false;
                        vm.IsResponseNextDay = false;
                        vm.ShowCategoryText = true;
                        vm.franchisee.categoryNotes = "";
                    }
                    else if (franchiseeCategoryId == vm.franchiseeCategoryType.OFFICEPERSON) {
                        vm.franchisee.categoryId = vm.franchiseeCategoryType.OFFICEPERSON;
                        vm.IsFrontOffice = false;
                        vm.IsOfficePerson = true;
                        vm.IsResponseWhenAvailable = false;
                        vm.IsResponseNextDay = false;
                        vm.ShowCategoryText = true;
                        vm.franchisee.categoryNotes = "";
                    }
                    else if (franchiseeCategoryId == vm.franchiseeCategoryType.RESPONDWHENAVAILABLE) {
                        vm.franchisee.categoryId = vm.franchiseeCategoryType.RESPONDWHENAVAILABLE;
                        vm.IsFrontOffice = false;
                        vm.IsOfficePerson = false;
                        vm.IsResponseWhenAvailable = true;
                        vm.IsResponseNextDay = false;
                        vm.ShowCategoryText = true;
                        vm.franchisee.categoryNotes = "";
                    }
                    else if (franchiseeCategoryId == vm.franchiseeCategoryType.RESPONDSNEXTDAY) {
                        vm.franchisee.categoryId = vm.franchiseeCategoryType.RESPONDSNEXTDAY;
                        vm.IsFrontOffice = false;
                        vm.IsOfficePerson = false;
                        vm.IsResponseWhenAvailable = false;
                        vm.IsResponseNextDay = true;
                        vm.ShowCategoryText = true;
                        vm.franchisee.categoryNotes = "";
                    }
                }
                init();
                getFranchiseeIRPId();
                function isGeneric() {
                    //if (vm.franchisee.franchiseeEmailEditModel.isGeneric) { !vm.franchisee.franchiseeEmailEditModel.isGeneric; }
                    //if (!vm.franchisee.franchiseeEmailEditModel.isGeneric) { !vm.franchisee.franchiseeEmailEditModel.isTechMailFees; }
                }
                $scope.$on('setCurrency', function (event, data) {
                    vm.franchisee.currency = data;
                });

                function isGeneric(isFromGeneric) {
                    if (isFromGeneric) {
                        if (vm.franchisee.franchiseeEmailEditModel.isTechMailFees) {
                            vm.franchisee.franchiseeEmailEditModel.isTechMailFees = false;
                            vm.franchisee.franchiseeEmailEditModel.amount = 0;
                        }
                        else if (!vm.franchisee.franchiseeEmailEditModel.isTechMailFees) {
                            // vm.franchisee.franchiseeEmailEditModel.isTechMailFees = true;
                            vm.franchisee.franchiseeEmailEditModel.amount = 0;
                        }
                    }
                    else {
                        if (vm.franchisee.franchiseeEmailEditModel.isGeneric) {
                            vm.franchisee.franchiseeEmailEditModel.isGeneric = false;
                            vm.franchisee.franchiseeEmailEditModel.amount = 3;
                        }
                        else if (!vm.franchisee.franchiseeEmailEditModel.isGeneric) {
                            vm.franchisee.franchiseeEmailEditModel.amount = 0;
                            //vm.franchisee.franchiseeEmailEditModel.isGeneric = true;
                        }
                    }

                }
                function techCountChanges() {
                    var multiplicationFactor = parseFloat(vm.franchisee.franchiseeEmailEditModel.multiplacationFactor);
                    var techCount = vm.franchisee.franchiseeEmailEditModel.technianCount;
                    if (techCount == undefined) {
                        vm.franchisee.franchiseeEmailEditModel.amount = 0;
                        return;
                    }
                    var amount = multiplicationFactor * techCount;
                    amount += 3;
                    vm.franchisee.franchiseeEmailEditModel.amount = amount;
                }
                function totalSum() {
                    vm.franchisee.leadPerformanceEditModel.totalAmount = parseInt(vm.franchisee.leadPerformanceEditModel.seoCost) +
                        parseInt(vm.franchisee.leadPerformanceEditModel.ppcSpend);
                }
                function performanceHistry(categoryId) {
                    vm.performanceFilter.franchiseeId = URLAuthenticationServiceForEncryption.decrypt(vm.franchiseeId);
                    vm.performanceFilter.categoryId = categoryId;

                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/show-performance-details.view.html',
                        controller: 'ShowPeformanceDetailsController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    franchiseeId: vm.performanceFilter.franchiseeId,
                                    categoryId: vm.performanceFilter.categoryId,
                                    franchiseeName: vm.franchisee.name
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                        getEvents();
                    }, function () {
                    });
                }
                function validation() {
                    if (vm.franchisee.duration != '' || vm.franchisee.duration != null) {
                        if (vm.franchisee.duration < 1 || vm.franchisee.duration > 100) {
                            vm.validationOnFields.push("duration");
                        }
                    }
                    if (vm.franchisee.name == '' || vm.franchisee.name == null) {
                        vm.validationOnFields.push("name");
                    }
                    if (vm.franchisee.organizationOwner.ownerFirstName == '' || vm.franchisee.organizationOwner.ownerFirstName == null) {
                        vm.validationOnFields.push("owner-first-name");
                    }
                    if (vm.franchisee.organizationOwner.ownerLastName == '' || vm.franchisee.organizationOwner.ownerLastName == null) {
                        vm.validationOnFields.push("owner-last-name");
                    }
                    if (vm.franchisee.email == '' || vm.franchisee.email == null) {
                        vm.validationOnFields.push("owner-email");
                    }
                    if (phoneService.checkNumberType(vm.franchisee.phoneNumbers) == false) {
                        vm.validationOnFields.push("phoneType");
                    }
                    if (phoneService.isInvalidPhoneNumber(vm.franchisee.phoneNumbers) == true) {
                        var index = phoneService.getInvalidPhoneNo(vm.franchisee.phoneNumbers);
                        var phoneNumberId = "phoneNumber_" + index;
                        vm.validationOnFields.push(phoneNumberId);
                    }
                    if (addressService.checkIsAddress(vm.franchisee.address)) {

                        if (vm.franchisee.address == null || vm.franchisee.address.length <= 0) {
                            vm.validationOnFields.push("address-line-1");
                            vm.validationOnFields.push("country");
                            vm.validationOnFields.push("state");
                            vm.validationOnFields.push("city");
                            vm.validationOnFields.push("zip");
                        }
                        else {
                            if (addressService.checkAddressLine1InComplete(vm.franchisee.address)) {
                                vm.validationOnFields.push("address-line-1");
                            }
                            if (addressService.checkStateInComplete(vm.franchisee.address)) {
                                vm.validationOnFields.push("state");
                            }
                            if (addressService.checkCityInComplete(vm.franchisee.address)) {
                                vm.validationOnFields.push("city");
                            }
                            if (addressService.checkZipCodeInComplete(vm.franchisee.address)) {
                                vm.validationOnFields.push("zip");
                            }
                        }
                    }
                    if (!vm.franchisee.feeProfile.salesBasedRoyalty) {
                        if (vm.franchisee.feeProfile.fixedAmount == null || (vm.franchisee.feeProfile.fixedAmount == '')) {
                            if (vm.franchisee.feeProfile.fixedAmount == 0 && vm.is0Franchisee) { }
                            else
                                vm.validationOnFields.push("fixed-amount");
                        }
                    }
                    if (vm.franchisee.feeProfile.salesBasedRoyalty) {
                        if (vm.franchisee.feeProfile.paymentFrequencyId == null || vm.franchisee.feeProfile.paymentFrequencyId == '') {
                            vm.validationOnFields.push("service");
                        }
                        if (vm.franchisee.feeProfile.minimumRoyaltyPerMonth == null || vm.franchisee.feeProfile.minimumRoyaltyPerMonth == '') {
                            if (vm.franchisee.feeProfile.minimumRoyaltyPerMonth != 0)
                                vm.validationOnFields.push("min-royalty-sales");
                        }
                        if (vm.franchisee.feeProfile.adFundPercentage == null || vm.franchisee.feeProfile.adFundPercentage == '') {
                            if (vm.franchisee.feeProfile.adFundPercentage != 0)
                                vm.validationOnFields.push("ad-fund-sales");
                        }
                    }
                    if (vm.franchisee.lateFee.isRoyalityLateFeeApplicable) {
                        if (vm.franchisee.lateFee.royalityLateFee == null || vm.franchisee.lateFee.royalityLateFee == '') {
                            vm.validationOnFields.push("late-fees");
                        }
                        if (vm.franchisee.lateFee.royalityWaitPeriodInDays == null || vm.franchisee.lateFee.royalityWaitPeriodInDays == '') {
                            vm.validationOnFields.push("wait-period");
                        }
                        if (vm.franchisee.lateFee.royalityInterestRate == null || vm.franchisee.lateFee.royalityInterestRate == '') {
                            vm.validationOnFields.push("p-a");
                        }
                    }

                    if (vm.franchisee.lateFee.isSalesDateLateFeeApplicable) {
                        if (vm.franchisee.lateFee.salesDataLateFee == null || vm.franchisee.lateFee.salesDataLateFee == '') {
                            vm.validationOnFields.push("sales-data-late-fees");
                        }
                        if (vm.franchisee.lateFee.salesDataWaitPeriodInDays == null || vm.franchisee.lateFee.salesDataWaitPeriodInDays == '') {
                            vm.validationOnFields.push("sales-data-wait-period");
                        }

                    }

                    if (vm.franchisee.leadPerformanceEditModel.seoCost == null || vm.franchisee.leadPerformanceEditModel.seoCost == '') {
                        vm.validationOnFields.push("min-seo");
                    }

                    if (vm.franchisee.leadPerformanceEditModel.ppcSpend == null || vm.franchisee.leadPerformanceEditModel.ppcSpend == '') {
                        vm.validationOnFields.push("min-royalty");
                    }
                    angular.forEach(vm.franchisee.serviceFees, function (fee) {
                        if (fee.isApplicable) {

                            if (fee.typeId == vm.serviceFeeType.Bookkeeping) {
                                var frequencyId = fee.serviceName + "-frequency";
                                if (fee.frequencyId == null || fee.frequencyId == '') {
                                    vm.validationOnFields.push(frequencyId);
                                }

                                var nameId = fee.serviceName + "-name";
                                if (fee.percentage == null || fee.percentage == '') {
                                    vm.validationOnFields.push(nameId);
                                }

                            }

                            if (fee.typeId == vm.serviceFeeType.PayRollProcessing) {
                                var frequencyId = fee.serviceName + "-payrollFrequency";
                                if (fee.frequencyId == null || fee.frequencyId == '') {
                                    vm.validationOnFields.push(frequencyId);
                                }
                            }

                            if (fee.typeId != vm.serviceFeeType.NationalCharge) {
                                var amountId = fee.serviceName + "-amount";
                                if (fee.amount == null || fee.amount == '') {
                                    vm.validationOnFields.push(amountId);
                                }
                            }

                            if (fee.typeId == vm.serviceFeeType.NationalCharge) {
                                var waitPeriodId = fee.serviceName + "-wait-period";
                                if (fee.percentage == null || fee.percentage == '') {
                                    vm.validationOnFields.push(waitPeriodId);
                                }
                            }
                        }
                    });


                    if (!vm.isSame) {

                        if (vm.franchisee.contactFirstName == '' || vm.franchisee.contactFirstName == undefined) {
                            vm.validationOnFields.push("contact-first-name");
                        }
                        if (vm.franchisee.contactLastName == '' || vm.franchisee.contactLastName == undefined) {
                            vm.validationOnFields.push("contact-last-name");
                        }
                        if (vm.franchisee.contactEmail == '' || vm.franchisee.contactEmail == undefined) {
                            vm.validationOnFields.push("contactEmail");
                        }
                    }
                    if (!vm.isSameForCustomer) {

                        if (vm.franchisee.organizationOwner.ownerFirstName == '' || vm.franchisee.organizationOwner.ownerFirstName == null) {
                            vm.validationOnFields.push("owner-first-name");
                        }

                        if (vm.franchisee.organizationOwner.ownerLastName == '' || vm.franchisee.organizationOwner.ownerLastName == null) {
                            vm.validationOnFields.push("owner-last-name");
                        }

                        if (vm.franchisee.email == '' || vm.franchisee.email == null) {
                            vm.validationOnFields.push("owner-email");
                        }
                    }

                    if (!vm.isSameForMarketing) {

                        if (vm.franchisee.marketingPersonFirstName == '' || vm.franchisee.marketingPersonFirstName == null) {
                            vm.validationOnFields.push("marketingPersonFirstName");
                        }

                        if (vm.franchisee.marketingPersonLastName == '' || vm.franchisee.marketingPersonLastName == null) {
                            vm.validationOnFields.push("marketingPersonLastName");
                        }

                        if (vm.franchisee.marketingPersonEmail == '' || vm.franchisee.marketingPersonEmail == null) {
                            vm.validationOnFields.push("marketingPersonEmail");
                        }
                    }



                    if (!vm.isSchedulerSame) {
                        if (vm.franchisee.schedulerFirstName == '' || vm.franchisee.schedulerFirstName == null) {
                            vm.validationOnFields.push("schedulerFirstName");
                        }

                        if (vm.franchisee.schedulerLastName == '' || vm.franchisee.schedulerLastName == null) {
                            vm.validationOnFields.push("schedulerLastName");
                        }

                        if (vm.franchisee.schedulerEmail == '' || vm.franchisee.schedulerEmail == null) {
                            vm.validationOnFields.push("schedulerEmail");
                        }
                    }
                }

                function timeClicked(time) {
                    var dateTime = document.getElementById('renewal-date');
                    if (time == undefined)
                        vm.isTimeValid = false;
                    else if (time == null)
                        vm.isTimeValid == false
                }

                function registrationHistry(value) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/franchisee-registration-histry.view.html',
                        controller: 'FranchsieeRegistrationHistryController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    list: value,
                                    franchiseeName: vm.franchisee.name
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                        getEvents();
                    }, function () {
                    });
                }

                function showNotes() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/modal.showFranchiseeNotes.client.view.html',
                        controller: 'ShowFranchiseeNotesController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: vm.franchisee.id,
                                    FranchiseeName: vm.franchisee.name,
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                    });
                }

                function changeAdFundRoyalityForSEOCharges(addfundOrRoyalty, franchiseeId) {
                    if (addfundOrRoyalty == 1) {
                        vm.queryAdfundOrRoyalty.isseoInRoyality = false;
                    }
                    else {
                        vm.queryAdfundOrRoyalty.isseoInRoyality = true;
                    }
                    vm.queryAdfundOrRoyalty.franchiseeId = franchiseeId;
                    return franchiseeService.changeAdFundRoyalityStatusForSEOCharges(vm.queryAdfundOrRoyalty).then(function (result) {
                        if (result.data) {
                            toaster.show("Website SEO Cost (Monthly) Status Is Updated");
                        }
                        else {
                            toaster.error("Can't Update Website SEO Cost (Monthly) Status");
                        }
                    });
                }

                $scope.$watch('vm.franchisee.address[0].countryId', function (nv, ov) {

                    if (nv == undefined) return;
                    if (nv == 8) {
                        vm.isMexico = true;
                    }
                    else {
                        vm.isMexico = false;
                    }
                });
                $scope.$emit("update-title", "EDIT FRANCHISEE");
            }]);

}());