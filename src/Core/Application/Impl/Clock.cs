using System;

namespace Core.Application.Impl
{
    public class Clock : IClock
    {
        private TimeZoneInfo _timeZoneInfo;


        private static class TimeZone
        {
            public const string IST = "India Standard Time";
            public const string EST = "Eastern Standard Time";
            public const string CST = "Central Standard Time";
            public const string MST = "Mountain Standard Time";
            public const string PST = "Pacific Standard Time";
            public const string AKST = "Alaskan Standard Time";
            public const string HAST = "Hawaii-Aleutian Standard Time";
            public const string NT = "Newfoundland Standard Time";
            public const string AST = "Atlantic Standard Time";
            public const string PSTM = "hora de verano del Pacífico de México";
            public const string HEC = "hora estándar central";
            public const string BOT = "Bolivia Time";
        }
        public Clock()
        {
            if (_timeZoneInfo == null)
            {
                _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                // _timeZoneInfo = TimeZoneInfo.Local;
            }

        }

        //public Clock(string timeZoneId)
        //{
        //    _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        //}

        public DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow;
            }
        }

        public DateTime ToLocal(DateTime utcDate)
        {
            return ToLocal(utcDate, _timeZoneInfo);
        }
        public DateTime ToLocalWithDayLightSaving(DateTime utcDate)
        {
            //int hourDiff = TimeZoneInfo.Local.BaseUtcOffset.Hours - _timeZoneInfo.BaseUtcOffset.Hours;
            //var boundaryDate = new DateTime(2018, 11, 4, 2, 0, 0);
            //if (DateTime.Now < boundaryDate)
            //{
            //    if (utcDate < boundaryDate)
            //    {
            //        return ToLocal(utcDate, _timeZoneInfo);
            //    }
            //    else
            //        return ToLocal(utcDate.AddHours(-1), _timeZoneInfo);
            //}
            var boundaryDate = new DateTime(2019, 3, 10, 2, 0, 0);
            if (DateTime.Now < boundaryDate)
            {
                if (utcDate < boundaryDate)
                {
                    return ToLocal(utcDate, _timeZoneInfo);
                }
                else
                    return ToLocal(utcDate.AddHours(1), _timeZoneInfo);
            }
            return ToLocal(utcDate, _timeZoneInfo);
        }

        public DateTime ToLocal(DateTime utcDate, TimeZoneInfo timeZoneInfo)
        {
            if (utcDate == DateTime.MinValue) return utcDate;

            utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);

            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, timeZoneInfo);
        }

        public DateTime ToLocal(DateTime utcDate, double offset)
        {
            //if (utcDate == DateTime.MinValue) return utcDate;
            //utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
            //SetTimeZoneInfo(offset);
            //  return TimeZoneInfo.ConvertTimeFromUtc(utcDate, GetTimeZoneInfo());
            return ToLocal(utcDate);
        }

        public DateTime ToLocal(DateTime utcDate, double offset,int? offset1)
        {
            //if (utcDate == DateTime.MinValue) return utcDate;
            //utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
        var timeZoneInfo=    SetTimeZoneInfoForLocalUse(offset);
            //  return TimeZoneInfo.ConvertTimeFromUtc(utcDate, GetTimeZoneInfo());
            return ToLocal(utcDate, timeZoneInfo);
        }

        public DateTime ToUtc(DateTime localDate)
        {
            return ToUtc(localDate, _timeZoneInfo);
        }

        public DateTime ToUtc(DateTime localDate, TimeZoneInfo timeZoneInfo)
        {
            if (localDate == DateTime.MinValue) return localDate;
            localDate = DateTime.SpecifyKind(localDate, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(localDate, timeZoneInfo);
        }
        public void SetTimeZoneInfo(double offSet)
        {
            var timeSpanOffset = TimeSpan.FromMinutes(offSet);
            _timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("Client Standard Time", timeSpanOffset, "Client Stanadard Time", "CLS");
        }
        public void SetTimeZoneInfo(string timeZoneId)
        {
            timeZoneId = GetTimeZoneId(timeZoneId);
            _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        private string GetTimeZoneId(string timeZoneId)
        {
            if (timeZoneId == "IST")
            {
                timeZoneId = TimeZone.IST;
            }
            else if (timeZoneId == "EST")
            {
                timeZoneId = TimeZone.EST;
            }
            else if (timeZoneId == "CST")
            {
                timeZoneId = TimeZone.CST;
            }
            else if (timeZoneId == "MST")
            {
                timeZoneId = TimeZone.MST;
            }
            else if (timeZoneId == "PST")
            {
                timeZoneId = TimeZone.PST;
            }
            else if (timeZoneId == "AKST")
            {
                timeZoneId = TimeZone.AKST;
            }
            else if (timeZoneId == "DST")
            {
                timeZoneId = TimeZone.AKST;
            }
            else if (timeZoneId == "AKDT")
            {
                timeZoneId = TimeZone.AKST;
            }
            else if (timeZoneId == "AK")
            {
                timeZoneId = TimeZone.AKST;
            }
            else if (timeZoneId == "HDT")
            {
                timeZoneId = TimeZone.HAST;
            }
            else if (timeZoneId == "HDT")
            {
                timeZoneId = TimeZone.HAST;
            }
            else if (timeZoneId == "NT")
            {
                timeZoneId = TimeZone.NT;
            }
            else if (timeZoneId == "AST")
            {
                timeZoneId = TimeZone.AST;
            }
            else if (timeZoneId == "BOT" || timeZoneId == "Bolivia Time")
            {
                timeZoneId = TimeZone.EST;
            }
            else if (timeZoneId == "EDT" || timeZoneId == "Eastern Daylight Time")
            {
                timeZoneId = "Eastern Standard Time";
            }
            else if (timeZoneId == "CDT" || timeZoneId == "Central Daylight Time")
            {
                timeZoneId = "Central Standard Time";
            }
            else if (timeZoneId == "MDT" || timeZoneId == "Mountain Daylight Time")
            {
                timeZoneId = "Mountain Standard Time";
            }
            else if (timeZoneId == "PDT" || timeZoneId == "Pacific Daylight Time")
            {
                timeZoneId = "Pacific Standard Time";
            }
            else if (timeZoneId == "AKDT" || timeZoneId == "Alaska Daylight Time")
            {
                timeZoneId = "Alaska Standard Time";
            }
            else if (timeZoneId == "DST" || timeZoneId == "Alaska Daylight Time")
            {
                timeZoneId = "Alaska Standard Time";
            }
            else if (timeZoneId == "AK" || timeZoneId == "Alaska Daylight Time")
            {
                timeZoneId = "Alaska Standard Time";
            }
            else if (timeZoneId == "HAST")
            {
                timeZoneId = "Hawaii-Aleutian Standard Time";
            }
            else if (timeZoneId == "HST" || timeZoneId == "Hawaii-Aleutian Daylight Time")
            {
                timeZoneId = "Hawaii-Aleutian Standard Time";
            }
            else if (timeZoneId == "NDT" || timeZoneId == "Newfoundland Daylight Time")
            {
                timeZoneId = "Newfoundland Standard Time";
            }
            else if (timeZoneId == "ADT" || timeZoneId == "Atlantic Daylight Time")
            {
                timeZoneId = "Atlantic Standard Time";
            }

            else if (timeZoneId == "ADT" || timeZoneId == "Atlantic Daylight Time")
            {
                timeZoneId = "Atlantic Standard Time";
            }

            else if (timeZoneId == "ADT" || timeZoneId == "hora de verano del Pacífico de México")
            {
                timeZoneId = "Pacific Standard Time";
            }

            else if (timeZoneId == "ADT" || timeZoneId == "Mexican Pacific Daylight Time")
            {
                timeZoneId = "Pacific Standard Time";
            }
            else if (timeZoneId == "ADT" || timeZoneId == "Mexican Pacific Daylight Time")
            {
                timeZoneId = "Pacific Standard Time";
            }
            else if (timeZoneId == "hora estándar del Pacífico de México")
            {
                timeZoneId = "Pacific Standard Time";
            }
            else if (timeZoneId == "Colombia Standard Time")
            {
                timeZoneId = "Eastern Standard Time";
            }
            else if(timeZoneId == "hora estándar central")
            {
                timeZoneId = TimeZone.CST;
            }
            return timeZoneId;
        }


        public TimeZoneInfo GetTimeZoneInfo()
        {
            return _timeZoneInfo;
        }

        public void SetTimeZoneInfo(int offSet)
        {
            var timeSpanOffset = TimeSpan.FromMinutes(offSet);
            _timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("Client Standard Time", timeSpanOffset, "Client Stanadard Time", "CLS");

        }
        public TimeZoneInfo CurrentTimeZone
        {
            get
            {
                return GetTimeZoneInfo();
            }
        }
        public DateTime Now
        {
            get
            {
                return ToLocal(DateTime.UtcNow);
            }
        }

        public string BrowserTimeZone { get; set; }

        public DateTime FirstDayOfWeek(DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            return dt.AddDays(-diff).Date;
        }

        public DateTime LastDayOfWeek(DateTime dt)
        {
            return FirstDayOfWeek(dt).AddDays(6);
        }

        public DateTime FirstDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public DateTime LastDayOfMonth(DateTime dt)
        {
            return FirstDayOfMonth(dt).AddMonths(1).AddDays(-1);
        }
        private TimeZoneInfo SetTimeZoneInfoForLocalUse(double offSet)
        {
            var timeSpanOffset = TimeSpan.FromMinutes(offSet);
          return TimeZoneInfo.CreateCustomTimeZone("Client Standard Time", timeSpanOffset, "Client Stanadard Time", "CLS");
        }
    }
}