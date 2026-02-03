(function () {
    'use strict';

    angular.module(OrganizationsConfiguration.moduleName).controller("CreateFranchiseeController",
        ["$state", "$stateParams", "$scope", "FranchiseeService", "FileService", "PhoneService", "AddressService", "Notification", "GeoService",
            "APP_CONFIG", "Clock", "Toaster",
            function ($state, $stateParams, $scope, franchiseeService, fileService, phoneService, addressService, notification, geoService,
                config, clock, toaster) {
                var vm = this;
                vm.isSchedulerSame = true;
                vm.isMexico = false;
                vm.isFranchiseeIdVisible = false;
                vm.isDisabled = false;
                vm.saveValue = saveValue;
                vm.uploadFiles = function (file) {
                    if (file == null) return;
                    vm.file = filesave
                    return fileService.upload(vm.file).then(function (result) {
                        console.log('File uploaded.');
                    });
                }
                vm.isSameForCustomer = true;
                vm.isSameForMarketing = true;
                vm.isSameForMarketing = true;
                vm.optionId = 2;
                vm.optionChoose = [{ display: "Business ID", value: 1 }, { display: "RPID", value: 2 }];
                vm.isRequired = false;
                vm.isSame = true;
                vm.isEdit = false;
                vm.totalSum = totalSum;
                vm.performanceHistry = performanceHistry;
                vm.techCountChanges = techCountChanges;
                vm.franchiseeId = $stateParams.id != null ? $stateParams.id : 0;
                vm.franchisee = {};
                vm.upload = upload;
                vm.franchisee.renewalDate = '';
                
                vm.franchisee.isRoyality = true;
                $scope.regex = "^(http[s]?:\\/\\/){0,1}(www\\.){0,1}[a-zA-Z0-9\\.\\-]+\\.[a-zA-Z]{2,5}[\\.]{0,1}";
                vm.franchisee.IsAdfund = false;
                vm.franchisee.isGeneric = true;
                vm.franchisee.isTechFee = false;
                vm.franchisee.isMinRoyalityFixed = true;
                vm.is0Franchisee = false;
                //To DO : Should be drived from lookup
                vm.paymentFrequency = [{ display: "Weekly", value: 31 }, { display: "Monthly", value: 32 }];
                vm.languageList = [{ display: "English", value: 249 }, { display: "Spanish", value: 250 }];
                vm.languageId = 249;
                vm.payRollFrequency = [{ display: "Monthly", value: 32 }, { display: "Twice A Month", value: 33 }];
                vm.seoCostBillingPeriodId = 1;
                vm.seoCostBillingPeriod = [{ display: "1st week", value: 1 }, { display: "2nd week", value: 2 }];
                vm.serviceTypes = [];
                vm.delete = deleteImage;
                vm.save = save;
                vm.cancel = cancel;
                vm.resetFeeProfileModel = resetFeeProfileModel;
                vm.isRoyalityLateFeeApplicable = isRoyalityLateFeeApplicable;
                vm.isSalesDateLateFeeApplicable = isSalesDateLateFeeApplicable;
                $scope.isImage = false;
                vm.primaryContact = config.primaryContact;
                vm.secondaryContact = config.secondaryContact;
                vm.contactNumber = config.contactNumber;
                vm.serviceFeeType = DataHelper.ServiceFeeType;
                vm.performanceParameter = DataHelper.PerformanceParamter;
                vm.isApplicable = isApplicable;
                vm.isRoyality = isRoyality;
                vm.isCategoryChange = isCategoryChange;
                vm.franchiseeCategoryType = DataHelper.FranchiseeCategories;
                vm.validation = validation;
                vm.IsFrontOffice = false;
                vm.IsOfficePerson = false;
                vm.IsResponseWhenAvailable = false;
                vm.IsResponseNextDay = false;
                vm.ShowCategoryText = false;
                vm.info = {};
                vm.info.fileList = [];
                vm.info.fileUploadModel = [];
                vm.isGeneric = isGeneric;
                vm.isseoInRoyalty = 1;
                vm.seoCostStatus = [{ display: "Adfund", value: 1 }, { display: "Royalty", value: 2 }];
                vm.changeAdFundRoyalityForSEOCharges = changeAdFundRoyalityForSEOCharges;
                vm.queryAdfundOrRoyalty =
                {
                    franchiseeId: vm.franchiseeId,
                    isSEOInRoyality: true
                }

                $scope.tempFile = {
                    attachTempImageFile: ""
                };
                $scope.file = {
                    attachedImageFile: ""
                };

                function saveValue() {
                    if (vm.franchisee.registrationDate == null) {
                        notification.showConfirm("There is no Registration Date Found for the Franchisee. Without registration date the monthly Royalty will not be calculated. Would you like to proceed without registration date??", "Warning Message:", function () {
                            save();
                        })
                    }
                    else {
                        save();
                    }
                }
                function save() {

                    vm.franchisee.languageId = vm.languageId;
                    vm.validationOnFields = [];
                    vm.isDisabled = true;
                    vm.form = $scope.franchiseeForm;
                    var currentDate = moment(clock.now());

                    validation();

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
                    if (vm.franchisee.webSite == undefined) {
                        notification.showAlert('Invalid Franchisee Website Url');
                        document.getElementById('webSite').focus();
                        return;
                    }
                    if (vm.form.webSite.$invalid && vm.franchisee.webSite != "") {
                        notification.showAlert('Invalid Franchisee Website Url');
                        document.getElementById('webSite').focus();
                        return;
                    }
                    if (vm.franchisee.renewalDate != null && vm.franchisee.renewalDate < currentDate) {
                        notification.showAlert("Renewal date can't be a past date!");
                        var getElement = document.getElementById("renewal-date");
                        getElement.classList.add('red-color');
                        vm.isDisabled = false;
                    }
                    if (angular.isUndefined(vm.franchisee.renewalDate)) {
                        notification.showAlert('Please Enter Valid Date. It should in MM/DD/YYYY Format');
                        document.getElementById('renewal-date').focus();
                        var getElement = document.getElementById("renewal-date");
                        getElement.classList.add('red-color');
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
                        else if (addressService.checkCountryInComplete(vm.franchisee.address)) {
                            document.getElementById("country").focus();
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
                        notification.showAlert("Please Enter a valid Phone Number  And Type");
                        vm.isDisabled = false;
                        return;
                    }
                    if (vm.franchisee.franchiseeEmailEditModel != null && vm.franchisee.franchiseeEmailEditModel.isTechMailFees
                        && (vm.franchisee.franchiseeEmailEditModel.technianCount == 0)) {
                        document.getElementById("techCount").focus();
                        notification.showAlert("Technician Count should be greater than 0");
                        vm.isDisabled = false;
                        return;
                    }
                    if (vm.franchisee.franchiseeEmailEditModel != null && vm.franchisee.franchiseeEmailEditModel.isTechMailFees
                        && (vm.franchisee.franchiseeEmailEditModel.multiplacationFactor == 0 || vm.franchisee.franchiseeEmailEditModel.multiplacationFactor == undefined)) {
                        document.getElementById("multiplicationFactor").focus();
                        notification.showAlert("Multiplication Factor of Franchisee Email Fees should be greater than 0");
                        vm.isDisabled = false;
                        return;
                    }

                    if (vm.franchisee.franchiseeEmailEditModel != null && vm.franchisee.franchiseeEmailEditModel.isTechMailFees
                        && (vm.franchisee.franchiseeEmailEditModel.technianCount % 1 != 0)) {
                        document.getElementById("multiplicationFactor").focus();
                        notification.showAlert("Technician Count of Franchisee Email Fees should be Whole Number");
                        vm.isDisabled = false;
                        return;
                    }
                    if (vm.franchisee.franchiseeEmailEditModel != null && (vm.franchisee.franchiseeEmailEditModel.isTechMailFee
                        || vm.franchisee.franchiseeEmailEditModel.isGeneric) && (vm.franchisee.franchiseeEmailEditModel.amount == 0
                            || vm.franchisee.franchiseeEmailEditModel.amount == undefined)) {
                        document.getElementById("franchiseeMailAmount").focus();
                        notification.showAlert("Amount of Franchisee Email Fees should be greater than 0");
                        vm.isDisabled = false;
                        return;
                    }
                    if (!vm.isSchedulerSame) {
                        if (vm.franchisee.schedulerFirstName == "" || vm.franchisee.schedulerFirstName == null) {
                            document.getElementById("scheduler-first-name").focus();
                            return;
                        }
                        else if (vm.franchisee.schedulerLastName == "" || vm.franchisee.schedulerLastName == null) {
                            document.getElementById("scheduler-last-name").focus();
                            return;
                        }
                        else if (vm.franchisee.schedulerEmail == "" || vm.franchisee.schedulerEmail == null) {
                            document.getElementById("scheduler-email").focus();
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
                    vm.franchisee.fileUploadModel = vm.info;
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
                    $state.go('core.layout.franchisee.list');
                }

                function resetFeeProfileModel() {
                    franchiseeService.resetFeeProfile(vm.franchisee.feeProfile);
                }

                function init() {
                    return franchiseeService.getFranchisee(vm.franchiseeId).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.franchisee = result.data;

                            if (vm.franchisee.feeProfile != null) {
                                vm.franchisee.feeProfile.adFundPercentage = vm.franchisee.feeProfile.adFundPercentage.toString();
                                vm.franchisee.feeProfile.minimumRoyaltyPerMonth = vm.franchisee.feeProfile.minimumRoyaltyPerMonth.toString();
                            }

                            if (vm.franchiseeId <= 0) {
                                vm.franchisee.isRoyality = true;
                                vm.franchisee.IsAdfund = false;
                                setDefaultServicefee();
                            }
                            totalSum();
                            if (vm.franchisee.isseoInRoyalty == 0) {
                                vm.franchisee.isseoInRoyalty = 1;
                            }
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
                        fee.percentage = config.defaultNationalChargePercentage;
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
                function isSalesDateLateFeeApplicable() {
                    if (vm.franchisee.lateFee.isSalesDateLateFeeApplicable == false) {
                        vm.franchisee.lateFee.salesDataLateFee = 0;
                        vm.franchisee.lateFee.salesDataWaitPeriodInDays = 0;
                    } else {
                        vm.franchisee.lateFee.salesDataLateFee = 50;
                        vm.franchisee.lateFee.salesDataWaitPeriodInDays = 1;
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
                init();
                getFranchiseeIRPId();

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
                            //if (height >= width) {
                            vm.info.fileList.push(result.data);
                            $scope.isInValidImage = false;

                        });
                    }
                    else {
                        vm.imagesrc = "/Content/images/no_image_thumb.gif";
                    }
                });

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
                            vm.franchisee.franchiseeEmailEditModel.amount = 3;
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
                    vm.performanceFilter.franchiseeId
                }
                $scope.$on('setCurrency', function (event, data) {
                    vm.franchisee.currency = data;
                });


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

                    // vm.franchisee.phoneNumbers = phoneService.sanitizePhoneNumbers(vm.franchisee.phoneNumbers);
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

                        if (vm.franchisee.ownerFirstName == '' || vm.franchisee.ownerFirstName == null) {
                            vm.validationOnFields.push("owner-first-name");
                        }

                        if (vm.franchisee.ownerLastName == '' || vm.franchisee.ownerLastName == null) {
                            vm.validationOnFields.push("owner-last-name");
                        }

                        if (vm.franchisee.email == '' || vm.franchisee.email == null) {
                            vm.validationOnFields.push("owner-email");
                        }
                    }

                    if (!vm.isSchedulerSame) {

                        if (vm.franchisee.schedulerFirstName == '' || vm.franchisee.schedulerFirstName == null) {
                            vm.validationOnFields.push("scheduler-first-name");
                        }

                        if (vm.franchisee.schedulerLastName == '' || vm.franchisee.schedulerLastName == null) {
                            vm.validationOnFields.push("scheduler-last-name");
                        }

                        if (vm.franchisee.schedulerEmail == '' || vm.franchisee.schedulerEmail == null) {
                            vm.validationOnFields.push("scheduler-email");
                        }
                    }
                }
                function upload() {
                    $('#file_input').click();
                }

                function changeAdFundRoyalityForSEOCharges(addfundOrRoyalty, franchiseeId) {
                    //if (addfundOrRoyalty == 1) {
                    //    vm.queryAdfundOrRoyalty.isseoInRoyality = false;
                    //}
                    //else {
                    //    vm.queryAdfundOrRoyalty.isseoInRoyality = true;
                    //}
                    //vm.queryAdfundOrRoyalty.franchiseeId = franchiseeId;
                    //return franchiseeService.changeAdFundRoyalityStatusForSEOCharges(vm.queryAdfundOrRoyalty).then(function (result) {
                    //    if (result.data) {
                    //        toaster.show("Website SEO Cost (Monthly) Status Is Updated");
                    //    }
                    //    else {
                    //        toaster.error("Can't Update Website SEO Cost (Monthly) Status");
                    //    }
                    //});
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

                $scope.$watch('model', function (newValue, oldValue) {
                })
                $scope.$emit("update-title", "CREATE FRANCHISEE");

            }]);
}());