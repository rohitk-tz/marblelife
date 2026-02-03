(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive("maskFormat", function () {

        return {
            restrict: 'A',
            require: "ngModel",
            link: function ($scope, element, $attrs, controller) {
                var text = $attrs["maskFormat"];
                switch (text) {
                    case 'date':
                        $(element).inputmask({ 'alias': 'mm/dd/yyyy' });
                        break;
                    case 'phone':
                        $(element).inputmask("999-999-9999");
                        break;
                    case 'phone-ext':
                        $(element).inputmask("(999) 999-9999? x99999");
                        break;
                    case 'phone-int':
                        $(element).inputmask("+33 999 999 999");
                        break;
                    case 'credit-card':
                        $(element).inputmask("9999 9999 9999 9999");
                        break;
                    case 'decimal':
                        $(element).inputmask({ 'alias': 'numeric', 'groupSeparator': '', 'autoGroup': true, 'digits': 2, rightAlign: false, 'placeholder': '0' });
                        break;
                    case 'decimal-single':
                        $(element).inputmask({ 'mask': '9{1,6}.9{1}', 'alias': 'decimal', 'autoGroup': true, 'digits': 1, 'digitsOptional': false, rightAlign: false, 'placeholder': '0' });
                        break;
                    case 'currency-special':
                        $(element).inputmask({ 'mask': '9{1,4}.9{2}', 'alias': 'decimal', 'autoGroup': true, 'digits': 2, 'digitsOptional': false, rightAlign: false, 'placeholder': '0' });
                        break;
                    case 'currency':
                        $(element).inputmask({ 'alias': 'numeric', 'groupSeparator': '', 'autoGroup': true, 'digits': 2, 'digitsOptional': false, 'placeholder': '0' });
                        break;
                    case 'zip':
                        $(element).inputmask("99999");
                        break;
                    case 'mexico-phone':
                        $(element).inputmask("999-99-999-999-9999");
                        break;
                    default:
                        break;
                }

                $(element).change(function () {
                    $scope.$apply(function () {
                        var value = element.val().replace(/[()-]+/g, "");
                        controller.$setViewValue(value);
                    });
                });
            }
        };
    });


}());
