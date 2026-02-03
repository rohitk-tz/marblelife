(function () {
    'use strict'
    angular.module(CoreConfiguration.moduleName).directive("addressEdit", ["GeoService", "$timeout", function (GeoService, $timeout) {
        
        return {
            restrict: "E",
            replace: true,
            scope: {
                model: '=',
                
            },
            templateUrl: '/modules/core/views/address-edit.client.view.html',
            link: function ($scope, $element) {
                //if ($scope.isFromJobEstimate != undefined) {
                //    var isFromJobEstimate = "paddingClass";
                //}
                //else
                //{
                //    var isFromJobEstimate = "nopaddingClass";
                //}
                if ($scope.model == null) $scope.model = { };

                $scope.getCities = getCities;
                $scope.getStates = getStates;
                $scope.allStates = [];

                function getStates(text) {
                    return GeoService.getStatesByName(text);
                }

                $scope.allCountries = [];

                GeoService.getCountries().then(function (arr) {
                    $scope.allCountries = arr;
                })

                function getCities(text) {
                    return GeoService.getCities(text);
                }
                $scope.getCurrency = function (countryId) {
                    GeoService.getCountryCurrencyByCountryId(countryId).then(function (result) {
                        $scope.$emit('setCurrency', result);
                    });

                }
            }
        }

    }]);

}());