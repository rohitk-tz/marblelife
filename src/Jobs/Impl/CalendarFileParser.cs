using Core.Application;
using Core.Scheduler;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class CalendarFileParser : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public CalendarFileParser() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Calendar parser Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Calendar parser started at " + _clock.UtcNow);

            try
            {
                var calendarParser = ApplicationManager.DependencyInjection.Resolve<ICalendarParsePollingAgent>();
                calendarParser.ParseCalendarFile();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Calendar parser. ", e);
            }

            _logService.Info("Calendar parser  end at " + _clock.UtcNow);
        }
    }
}
