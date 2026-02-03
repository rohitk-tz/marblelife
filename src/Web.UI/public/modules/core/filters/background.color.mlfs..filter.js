
(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).filter('mlfsColorFilter', ['$filter', '$rootScope', function ($filter, $rootScope) {
        return function (input, toCurrency, conversionRate) {
            //console.log('input-' + input);
            //console.log('toCurrency-' + toCurrency);
            //console.log('conversionRate-' + conversionRate);
            if (!input || typeof (input) === 'undefined' || typeof (conversionRate) === 'undefined') {
                return 0;
            }
            else {
                var currency = '$';
                switch (toCurrency) {
                    case 'ZAR':
                        currency = 'R';
                        break;
                    case 'AED':
                        currency = 'د.إ';//&#x62f;&#x2e;&#x625;
                        break;
                    default:

                }
                var roles = DataHelper.Role;
                var isSuperAdmin = $rootScope.identity.roleId == roles.SuperAdmin;
                //var currency = '(' + toCurrency + ')';
                if (!isSuperAdmin) {
                    if (conversionRate > 0) {
                        //console.log('conversionRate-' + conversionRate + 'input=' + input);
                        // console.log('result-' + $filter('currency')(((input / conversionRate).toFixed(2)), currency, 2));
                        return $filter('currency')(((input * conversionRate).toFixed(2)), currency, 2);
                    }
                    else {
                        return $filter('currency')((input.toFixed(2)), currency, 2);
                    }
                } else {
                    return $filter('currency')((input.toFixed(2)), currency, 2);
                }
            }
            ;
        }
    }]);

}());
