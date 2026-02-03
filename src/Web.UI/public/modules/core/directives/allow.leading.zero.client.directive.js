(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive('allowLeadingZero', function () {
        return {
            require: 'ngModel',
            restrict: 'A',
            scope: {
                dmin: '=',
                dmax: '='
            },
            link: function (scope, element, attr, ctrl) {

                function inputValue(val) {
                    if (val) {
                        var digits = val.replace(/[^0-9]/g, '');

                        if (digits !== val) {
                            ctrl.$setViewValue(digits);
                            ctrl.$render();
                        }
                        var number = digits;

                        var firstDigit = number.charAt(0);
                        if (firstDigit != '0') {
                            number = parseInt(digits, 10);
                        }

                        validateRange(number);

                        return number;
                    }
                    return '';
                }

                function validateRange(num) {

                    if (scope.dmin != undefined && scope.dmin != null && num < Number(scope.dmin)) {
                        ctrl.$setViewValue('');
                        ctrl.$render();
                    }

                    if (scope.dmax != undefined && scope.dmax != null && num > Number(scope.dmax)) {
                        ctrl.$setViewValue('');
                        ctrl.$render();
                    }
                }

                ctrl.$parsers.push(inputValue);
            }
        };
    });
})();
