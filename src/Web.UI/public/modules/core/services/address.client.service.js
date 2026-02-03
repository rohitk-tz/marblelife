(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).service("AddressService", [function () {

        var checkAddressInComplete = function (address)
        {
            if (address == null || address.length < 1) return true;

            if (isEmpty(address[0].addressLine1) || isEmpty(address[0].city) ||
                isEmpty(address[0].state) || isEmpty(address[0].zipCode) || isEmpty(address[0].countryId))
                return true;
            
            return false;
        };

        var sanitizeAddress = function (address)
        {
            if (address == null || address.length < 1) return address;

            var count = 0; //total number of fields in object
            if (isEmpty(address[0].addressLine1)) {
                address[0].addressLine1 = null;
                count++;
            }

            if (isEmpty(address[0].addressLine2)) {
                address[0].addressLine2 = null;
                count++;
            }

            if (isEmpty(address[0].city)) {
                address[0].city = null;
                count++;
            }

            if (isEmpty(address[0].state)) {
                address[0].state = null;
                count++;
            }
            if (isEmpty(address[0].countryId)) {
                address[0].countryId = null;
                count++;
            }

            if (isEmpty(address[0].zipCode)) {
                address[0].zipCode = null;
                count++;
            }

            if (count == 6) return null;

            return address;
        };

        var isEmpty = function (field) {
            return field == null || field.toString().trim().length < 1;
        };
        var checkIsAddress = function (address) {
            if (address == null || address.length < 1) return true; 
            return false;
        };

        var checkAddressLine1InComplete = function (address) {
            if (address[0].addressLine1 == "" || address[0].addressLine1 == null ) return true;
            return false;
        }
        var checkCityInComplete = function (address) {
            if (address[0].city == "" || address[0].city == null) return true;
            return false;
        }

        var checkStateInComplete = function (address) {
            if (address[0].state == "" || address[0].state == null) return true;
            return false;
        }

        var checkZipCodeInComplete = function (address) {
            if (address[0].zipCode == "" || address[0].zipCode == null) return true;
            return false;
        }
        var checkCountryInComplete = function (address) {
            if (address[0].countryId == "" || address[0].countryId == null) return true;
            return false;
            
        }
        return {
            checkAddressInComplete: checkAddressInComplete,
            sanitizeAddress: sanitizeAddress,
            checkIsAddress: checkIsAddress,
            checkAddressLine1InComplete: checkAddressLine1InComplete,
            checkCityInComplete: checkCityInComplete,
            checkStateInComplete: checkStateInComplete,
            checkZipCodeInComplete: checkZipCodeInComplete,
            checkCountryInComplete: checkCountryInComplete
        };

    }]);
}());
