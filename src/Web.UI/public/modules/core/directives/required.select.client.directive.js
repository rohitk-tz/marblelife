(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive('requiredSelect', function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function(scope, elm, attr, ctrl) {

                if (!ctrl) return;
                attr.requiredSelect = true; // force truthy in case we are on non input element

                var validator = function(value) {
                    if (attr.requiredSelect && (ctrl.$isEmpty(value) || value <= 0)) {
                        ctrl.$setValidity('requiredSelect', false);
                        return;
                    } else {
                        ctrl.$setValidity('requiredSelect', true);
                        return value;
                    }
                };

                ctrl.$formatters.push(validator);
                ctrl.$parsers.unshift(validator);

                attr.$observe('requiredSelect', function() {
                    validator(ctrl.$viewValue);
                });
            }
        };
    });

})();