(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).filter('phonemask', ['$rootScope',
        function ($rootScope) {
            return function (input, countryId) {
                if (!input)
                    return input;

                input = input.replace(/[()-]+/g, "");
                var roles = DataHelper.Role;
                var isSuperAdmin = $rootScope.identity.roleId == roles.SuperAdmin;

                if (isSuperAdmin && input.substring(0, 1) == 0) {

                    var countryCode = '';
                    switch (countryId) {
                        case 5:
                            countryCode = '0027';
                            break;
                        default:
                    }
                    var number = input.substring(0);
                    number = countryCode + number;
                    //return number.substring(0, 4) + '-' + number.substring(4, 6) + '-' + number.substring(6, 9) + '-' + number.substring(9);
                    return number.substring(0, 3) + '-' + number.substring(3, 6) + '-' + number.substring(6);
                }

                return input.substring(0, 3) + '-' + input.substring(3, 6) + '-' + input.substring(6);
            };
        }]);

}());