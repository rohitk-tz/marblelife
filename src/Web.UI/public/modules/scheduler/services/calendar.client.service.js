(function () {
    'use strict';

    angular.module(SchedulerConfiguration.moduleName).service("CalendarService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/scheduler/calendar"

        function saveCalendar(model) {
            return httpWrapper.post({ url: baseUrl + "/upload", data: model });
        }

        function getTimeZoneInfo() {
            return httpWrapper.post({ url: "/application/dropdown/GetTimeZoneList" });
        }

        return {
            saveCalendar: saveCalendar,
            getTimeZoneInfo: getTimeZoneInfo
        };
    }]);
})();