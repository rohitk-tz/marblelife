(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive('onlyAlphabets', function () {
        return {
            require: 'ngModel',
            restrict: 'A',           
            link: function (scope, element, attr, ctrl) {
                //var dmin = attr.dmin;
                //var dmax = attr.dmax;

                function inputValue(val) {
                    if (val) {
                        var alphabets = val.replace(/[^a-zA-Z-]/, '');

                        if (alphabets !== val) {
                            ctrl.$setViewValue(alphabets);
                            ctrl.$render();
                        }
                        return alphabets;
                    }
                    return '';
                }
              

                ctrl.$parsers.push(inputValue);
            }
        };
    });
})();
