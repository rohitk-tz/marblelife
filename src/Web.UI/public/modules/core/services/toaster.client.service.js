(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).factory("Toaster", [function () {

        var showMessage = function (message, delay, cb) {

            if (delay == null) delay = 5000;
            
            toastr.options.timeOut = delay;
            toastr.options.extendedTimeOut = delay * 2;

            if (cb == null)
                cb = toastr.info;

            cb(message);
        };

        return {
            show: function (message, delay) {
                showMessage(message, delay, toastr.success);
            },
            error: function (message, delay) {
                showMessage(message, delay, toastr.error);
            }
        };
    }]);

}());