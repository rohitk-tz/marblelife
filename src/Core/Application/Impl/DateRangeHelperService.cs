using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Application.Impl
{
    public static class DateRangeHelperService
    {
        public static IEnumerable<DateTime> DayOfWeekCollection(DateTime startDate, DateTime endDate)
        {
            var dates = new List<DateTime>();
            for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                if (dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    dates.Add(dt);
                }
            }
            return dates;
        }

        public static DateTime GetFirstDayOfWeek(DateTime startDate)
        {
            while (startDate.DayOfWeek != DayOfWeek.Monday)
                startDate = startDate.AddDays(-1);
            return startDate;
        }

        public static IEnumerable<Tuple<DateTime>> MonthsStartDate(DateTime startDate, DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            var dateTimeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator <= limit)
            {
                yield return Tuple.Create(iterator);
                iterator = iterator.AddMonths(1);
            }
        }

        public static IEnumerable<DateTime> GetDaysCollection(DateTime startDate, DateTime endDate)
        {
            var list = new List<DateTime>();
            for (DateTime date = startDate; date >= endDate; date = date.AddDays(-1))
            {
                list.Add(date);
            }
            return list;
        }

        public static IEnumerable<DateTime> GetHoursCollection(DateTime startDate, DateTime endDate)
        {
            var list = new List<DateTime>();
            for (DateTime date = endDate; date > startDate; date = date.AddHours(-1))
            {
                list.Add(date);
            }
            return list.OrderBy(x => x.TimeOfDay);
        }

        public static IEnumerable<int> GetYearsBetween(DateTime startDate, DateTime endDate)
        {
            var list = new List<int>();
            for (DateTime date = startDate; date >= endDate; date = date.AddYears(-1))
            {
                list.Add(date.Year);
            }
            return list.Distinct();
        }

        public static int GetNumberOfDaysInBetween(DayOfWeek day,DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;
            int count = (int)Math.Floor(ts.TotalDays / 7);
            int remainder = (int)(ts.TotalDays % 7);
            int sinceLastDay = (int)(end.DayOfWeek - day);
            if (sinceLastDay < 0) sinceLastDay += 7;

            if (remainder >= sinceLastDay) count++;

            return count > 0 ? count : 0;
        }

        public static IEnumerable<Tuple<DateTime,DateTime>> GetQuarterBetweenYears(DateTime startDate, DateTime endDate)
        {
            var list = new List<Tuple<DateTime, DateTime>>();
            var startDateForSearch = new DateTime(startDate.Year, 1, 1);
            var endDateForTheMonth= DateTime.DaysInMonth(endDate.Year, endDate.Month);
            var endDateForSearch= new DateTime(endDate.Year, endDate.Month, endDateForTheMonth);
            for (DateTime date = startDate; date <= endDate; date = date.AddMonths(3))
            {
                var startDateForQuarter= new DateTime(date.Year, date.Month,1);
                var lastDateOfQuarter= DateTime.DaysInMonth(date.Year, date.Month+2);
                var endDateForQuarter = new DateTime(date.Year, date.Month+2, lastDateOfQuarter);
                list.Add(Tuple.Create(startDateForQuarter, endDateForQuarter));
            }
            return list.Distinct();
        }
    }
}
