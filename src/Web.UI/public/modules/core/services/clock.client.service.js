(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).service("Clock", [function () {

        var dateFormat = 'MM/DD/YYYY';
        var timeFormat = 'HH:mm:ss';
        var dateTimeFormat = dateFormat + ' ' + timeFormat;

        var now = function () {
            return moment().format(dateTimeFormat);
        };

        var date = function (dateObj) {
            if (!dateObj)
                return moment().format(dateFormat);

            return moment(dateObj).format(dateFormat);
        };

        var diffInDays = function (dateFrom, dateTo) {
            return moment(dateTo).diff(moment(dateFrom), 'days');
        };

        var addDays = function (dateObj, amountToAdd) {
            var d = moment(dateObj);
            return d.add(amountToAdd, 'd').format(dateTimeFormat);
        };

        var addMonths = function (dateObj, amountToAdd) {
            var d = moment(dateObj);
            return d.add(amountToAdd, 'M').format(dateTimeFormat);
        };

        var subtractDays = function (dateObj, amountToMinus) {
            return moment(dateObj).subtract(amountToMinus, 'd').format(dateTimeFormat);
        };

        var getDay = function (dateObj) {
            var d = moment(dateObj);
            return d.format('D');
        }

        var getMonth = function (dateObj) {
            var d = moment(dateObj);
            return d.format('M');
        }

        var getYear = function (dateObj) {
            var d = moment(dateObj);
            return d.format('YYYY');
        }

        var getDaysInMonth = function (month, year) {
            return new Date(year, month, 0).getDate();
        }


        function getMonthDateRange(year, month) {

            var startDate = moment([year, month]);

            var endDate = moment(startDate).endOf('month');

            return { start: startDate, end: endDate };
        }

        var getStartDateOfWeek = function () {
            return moment().isoWeekday(1).startOf('week').format(dateTimeFormat);
        }

        var getStartDateOfMonth = function () {
            return moment().startOf('month').format(dateTimeFormat);
        }

        var getStartDateOfYear = function () {
            return moment().startOf('year').format(dateTimeFormat);
        }

        var getEndDateOfMonth = function () {
            return moment().endOf('month').format(dateTimeFormat);
        }

        return {
            now: now,
            date: date,
            subtractDays: subtractDays,
            addDays: addDays,
            diffInDays: diffInDays,
            getDay: getDay,
            getMonth: getMonth,
            getYear: getYear,
            getDaysInMonth: getDaysInMonth,
            getMonthDateRange: getMonthDateRange,
            getStartDateOfWeek: getStartDateOfWeek,
            getStartDateOfMonth: getStartDateOfMonth,
            getStartDateOfYear: getStartDateOfYear,
            addMonths: addMonths,
            getEndDateOfMonth: getEndDateOfMonth
        };
    }]);
})();