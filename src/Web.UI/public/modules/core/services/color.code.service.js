(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).service("ColorCodeService", [function () {

        var checkUserColorCode = function (colorCode) {
            if (colorCode != null) {
                var valid = colorCode.match("^#+([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$");
                if (valid != null) {
                    return true;
                }
            }
            return false;
        };

      

        return {
            checkUserColorCode: checkUserColorCode
        };

    }]);
}());