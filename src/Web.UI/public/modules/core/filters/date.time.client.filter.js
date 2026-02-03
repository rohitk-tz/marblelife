(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).filter('dateTimeFilter', ['$filter', function ($filter) {
        return function (input, arg) {
            if (!input)
                return '';

            if (typeof input == "string") {
                if (input.endsWith("Z") || input.endsWith("z"))
                    input = new Date(input);
                else {
                    input = new Date(input + "Z");
                }
            }

            switch (arg) {
                case 'd':
                    return $filter('date')(input, 'MM/dd/yyyy', 'local');
                case 'dt':
                    return $filter('date')(input, 'MM/dd/yyyy hh:mm a');
                case 'udt':
                    return $filter('date')(input, 'MM/dd/yyyy hh:mm:ss a', 'UTC');
                default:
                    return $filter('date')(input, 'MM/dd/yyyy', 'UTC');
            }
        };
    }]);

}());
