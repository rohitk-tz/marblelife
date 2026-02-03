using Core.Application;
using Core.Reports;
using Quartz;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class CalendarImagesMigrationSync : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public CalendarImagesMigrationSync() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Calendar Images Migration at " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Calendar Images Migration at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<ICalendarImagesMigration>();
            pollingAgent.CalendarImagesMigrationToNewApplication();
            _logService.Info("Calendar Images Migration at " + _clock.UtcNow);
        }
    }
}
