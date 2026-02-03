using System;

namespace Core.Application
{
    public interface IClock
    {
        DateTime UtcNow { get; }
        string BrowserTimeZone { get; set; }
        DateTime ToLocal(DateTime dateTime);
        DateTime ToLocal(DateTime utcDate, TimeZoneInfo timeZoneInfo);
        DateTime ToLocal(DateTime utcDate, double offset);
        DateTime ToUtc(DateTime localDate);
        DateTime ToUtc(DateTime localDate, TimeZoneInfo timeZoneInfo);
        void SetTimeZoneInfo(string timeZoneId);
        void SetTimeZoneInfo(int offSet);
        TimeZoneInfo GetTimeZoneInfo();
        TimeZoneInfo CurrentTimeZone { get; }
        DateTime Now { get; }
        DateTime ToLocalWithDayLightSaving(DateTime utcDate);
        DateTime ToLocal(DateTime utcDate, double offset, int? offset1);
    }
}