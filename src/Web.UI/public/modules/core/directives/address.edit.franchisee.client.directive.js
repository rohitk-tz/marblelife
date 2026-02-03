(function () {
    'use strict'
    angular.module(CoreConfiguration.moduleName).directive("addressEditFranchisee", ["GeoService", "$timeout", function (GeoService, $timeout) {

        return {
            restrict: "E",
            replace: true,
            scope: {
                model: '=',
                currency:'='
            },
            templateUrl: '/modules/core/views/address-edit-franchisee.client.view.html',
            link: function ($scope, $element) {

                var a = $scope.currency;
                if ($scope.model == null) $scope.model = {};

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
                        $scope.model.currency = result;
                        $scope.$emit('setCurrency', result);
                    });

                }
                $scope.$on('getCurrency', function (event, data) {
                    $scope.model.currency = data;
                });
                $scope.$watch('model.countryId', function (newValue, oldValue) {
                    if (newValue == undefined) {
                        return;
                    }
                    GeoService.getCountryCurrencyByCountryId(newValue).then(function (result) {
                        $scope.model.currency = result;
                    });
                })
            }
        }

    }]);

}());