(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive('onlyDecimal', function () {
        return {
            require: 'ngModel',
            restrict: 'A',
            scope: {
                dmin: '=',
                dmax: '='
            },
            link: function (scope, element, attr, ctrl) {
                //var dmin = attr.dmin;
                //var dmax = attr.dmax;

                function inputValue(val) {
                    if (val) {
                       // var digits = val.replace(/[^0-9]/g, '');
                        var digits = val;
                        if (!IsNumeric(val)) {
                            if (val.split(".").length > 2) {
                                digits = val.slice(0, -1);
                            } else {
                                digits = val.replace(/[^0-9]/g, '');
                            }
                            ctrl.$setViewValue(digits);
                            ctrl.$render();
                        }

                        var num = parseFloat(digits);

                        validateRange(num);

                        return num;
                    }
                    return '';
                }
                 function IsNumeric(input) {
                     var RE = /^[0-9]{1,14}\.[0-9]{0,2}$/;                   
                    return (RE.test(input));
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
