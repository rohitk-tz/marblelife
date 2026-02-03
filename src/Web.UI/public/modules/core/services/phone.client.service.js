(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).service("PhoneService", [function () {

        var checkInvalidPhoneNo = function (phoneNumbers) {
            var valid = true;
            angular.forEach(phoneNumbers, function (phone) {
                if (valid) {
                    var phoneN = phone.phoneNumber.replace(/_/g, "");
                    if (phoneN.length < 10)
                        valid = false;
                }
            });
            return valid;
        }

        var isInvalidPhoneNumber = function (phoneNumbers) {
            var valid = false;
            angular.forEach(phoneNumbers, function (phone) {
                if (!valid) {
                    if (phone.phoneNumber == null || phone.phoneNumber == undefined)
                        valid = true;
                    if (!valid) {
                        var phoneN = phone.phoneNumber.replace(/_/g, "");
                        if (phoneN.length < 10)
                            valid = true;
                    }
                }
            });
            return valid;
        }
        var getInvalidPhoneNo = function (phoneNumbers) {
            var indexForPhoneNumber = -1;
            var valid = true;
            angular.forEach(phoneNumbers, function (phone, index) {
                if (valid) {
                    if (phone.phoneNumber == null || phone.phoneNumber == undefined) {
                        indexForPhoneNumber = index;
                        valid = false;
                        return;
                    }
                    var phoneN = phone.phoneNumber.replace(/_/g, "");
                    if (phoneN.length < 10) {
                        indexForPhoneNumber = index;
                        valid = false;
                    }
                }
            });
            return indexForPhoneNumber;
        }

        var checkNumberTypeCombination = function (phoneNumbers) {
            var valid = true;
            if (phoneNumbers == null || phoneNumbers.length < 1) return phoneNumbers;
            angular.forEach(phoneNumbers, function (phone) {
                if (valid) {
                    if (phone.phoneType == null || phone.phoneType < 1)
                        valid = false;
                    else if (phone.phoneType > 0 && phone.phoneNumber == null)
                        valid = false;
                }
            });
            return valid;
        }

        var sanitizePhoneNumbers = function (phoneNumbers) {
            if (phoneNumbers == null || phoneNumbers.length < 1) return phoneNumbers;

            angular.forEach(phoneNumbers, function (phone, index) {
                if (phone.phoneType == null || phone.phoneType < 1 || phone.phoneType.trim().length < 1 || phone.phoneNumber == null || phone.phoneNumber.trim().length < 1) {
                    phoneNumbers.splice(index, 1);
                }
            });
            return phoneNumbers;
        };

        var checkNumberType = function (phoneNumbers) {
            var valid = true;
            if (phoneNumbers == null || phoneNumbers.length < 1) return phoneNumbers;
            angular.forEach(phoneNumbers, function (phone) {
                if (valid) {
                    if (phone.phoneType == null)
                        valid = false;
                    else if (phone.phoneType <= 0)
                        valid = false;
                }
            });
            return valid;
        }

        return {
            sanitizePhoneNumbers: sanitizePhoneNumbers,
            checkInvalidPhoneNo: checkInvalidPhoneNo,
            checkNumberTypeCombination: checkNumberTypeCombination,
            getInvalidPhoneNo: getInvalidPhoneNo,
            isInvalidPhoneNumber: isInvalidPhoneNumber,
            checkNumberType: checkNumberType
        };

    }]);
}());
