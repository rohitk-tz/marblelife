(function () {
    'use strict'
    angular.module(CoreConfiguration.moduleName).directive('matchInput', [function () {
        return {
            restrict: 'A',
            scope: true,
            require: 'ngModel',
            link: function (scope, elem, attrs, ctrl) {
                var checker = function () {
                    var e1 = scope.$eval(attrs.ngModel);
                    var e2 = scope.$eval(attrs.matchInput);
                    return e1 == e2;
                };
                scope.$watch(checker, function (n) {
                    ctrl.$setValidity("match", n);
                });
            }
        }
    }]);
}());