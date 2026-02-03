(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).service("Util", [function () {

        function hookToAdjustLabels() {

            setTimeout(function () {

                $('.form-md-floating-label .form-control, .form-control').each(function () {
                    var val = $(this).val();
                    if (val != null && val.length > 0 && $(this).hasClass('edited') == false) {
                        $(this).addClass('edited');
                    }
                });
            }, 500);

        }

        return {
            hookToAdjustLabels: hookToAdjustLabels
        };

    }]);

}());