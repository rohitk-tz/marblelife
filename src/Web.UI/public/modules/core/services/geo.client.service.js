(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).service("GeoService", ["HttpWrapper",
        function (httpWrapper) {

            var baseUrl = "/Geo/Geo"
            var citiesArray = [];
            var zipcodeArray = [];

            var cities = function (model) {
                return httpWrapper.get({ url: baseUrl + "/GetAllCities" }).then(function (result) {
                    var data = result.data;
                    for (var cnt = 0; cnt < data.length; cnt++) {
                        citiesArray.push({ display: data[cnt].name, value: data[cnt].id });
                    }
                });
            };


            //var zip = function (model) {
            //    return httpWrapper.get({ url: "/zip" }).then(function (result) {
            //        var data = result.data;
            //        for (var cnt = 0; cnt < data.length; cnt++) {
            //            zipcodeArray.push({ display: data[cnt].zipcode, value: data[cnt].id });
            //        }
            //    });
            //};

            ////cities();

            var findModel = function (target, keyValue) {
                var array = null;

                if (target === 'city')
                    array = citiesArray;

                if (target === 'state')
                    array = statesArray;

                for (var cnt = 0; cnt < array.length; cnt++) {
                    if (array[cnt]['display'] === keyValue)
                        return array[cnt];
                }

                return null;
            };

            var getStates = function () {
                var statesArray = [];

                return httpWrapper.get({ url: baseUrl + "/GetAllStates", skipFullPageLoader: true }).then(function (result) {
                    var data = result.data;

                    for (var cnt = 0; cnt < data.length; cnt++) {
                        statesArray.push({ display: data[cnt].name, value: data[cnt].id, stateCode: data[cnt].shortName });
                    }

                    return statesArray;
                });
            };

            var getCities = function (text) {
                return httpWrapper.get({ url: baseUrl + '/GetAllCitiesByName?name=' + text , skipFullPageLoader: true }).then(function (result) {
                    return result.data;
                });
            };
            var getStatesByName = function (text) {
                return httpWrapper.get({ url: baseUrl + '/GetAllStatesByName?name=' + text , skipFullPageLoader: true }).then(function (result) {
                    return result.data;
                });
            };
            var getZip = function (key, keyValue) {
                if (key) {
                    var items = $filter('filter')(zipcodeArray, keyValue);
                    return items;
                }
                else
                    return zipcodeArray;
            };

            var getPhoneTypes = function () {
                return httpWrapper.get({ url: "/application/dropdown/GetPhoneTypes" });
            };
            var getCountries = function () {
                var countriesArray = [];

                return httpWrapper.get({ url: baseUrl + "/GetAllCountries", skipFullPageLoader: true }).then(function (result) {
                    var data = result.data;

                    for (var cnt = 0; cnt < data.length; cnt++) {
                        countriesArray.push({ display: data[cnt].name, value: data[cnt].id, stateCode: data[cnt].shortName });
                    }

                    return countriesArray;
                });
            };
            var getCountryCurrencyByCountryId = function (countryId) {
                return httpWrapper.get({ url: baseUrl + "/GetCountryCurrencyByCountryId?countryId=" + countryId, skipFullPageLoader: true }).then(function (result) {
                   return  result.data;

                 
                });
            };
            return {
                getCountries: getCountries,
                getStates: getStates,
                getCities: getCities,
                getZip: getZip,
                getModelObject: findModel,
                getPhoneTypes: getPhoneTypes,
                getCountryCurrencyByCountryId: getCountryCurrencyByCountryId,
                getStatesByName: getStatesByName
            };

        }]);


}());