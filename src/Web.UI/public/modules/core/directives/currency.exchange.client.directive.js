(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive('currencyExchange', ["$rootScope",
    function ($rootScope) {

        return {
            restrict: 'A',
            replace: true,
            scope: {
                conversionRate: "=",
                input: "=",
                fromCurrency: "=",
                isUsClient: "=?"
            },
            template: function (isUsClient) {
                var isUsView = true;
                // getting value of isUsClient attribute as giving out only span attribute with values.
                if (isUsClient[0].attributes != null)
                {
                    isUsView = isUsClient[0].attributes[1].specified;
                }
                
                if (isUsView==true) {
                    return '<span title = "CurrencyExchange Rate : 1 {{currency}} = {{cRate}} {{fromCurrency}}">{{input |  currencyExchangeFilter:currencyCode :cRate}}</span>';
                }
                else {
                    return '<span>{{input |  currencyExchangeFilter:currencyCode :cRate}}</span>';
                }
            },
            link: function ($scope, element, attrs) {
                
                $scope.Roles = DataHelper.Role;
                $scope.isSuperAdmin = $rootScope.identity.roleId == $scope.Roles.SuperAdmin;

                $scope.currencyCode = $rootScope.identity.currencyCode;
                $scope.currency = $scope.currencyCode;
                $scope.isUsClient = $scope.isUsClient == null ? true : $scope.isUsClient ;
                var watch = $scope.$watch('conversionRate', function (newVal, oldVal) {
                    if (newVal) {
                        init();
                        watch();
                    }
                });

                function init() {
                    $scope.cRate = $scope.conversionRate;
                    $scope.cRate = (1 / $scope.cRate).toFixed(3);
                }

                //if (!$scope.isSuperAdmin)
                //    $scope.currency = 'USD';
            },
           
        }
    }]);
}());