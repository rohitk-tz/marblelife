using Core.Application;
using Core.Notification;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class WeeklyNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public WeeklyNotification()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Weekly Notification start at- " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Weekly Notification start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IWeeklyNotificationPollingAgent>();
                pollingAgent.CreateWeeklyNotification();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Weekly notification. ", e);
            }

            _logService.Info("Weekly Notification end at " + _clock.UtcNow);
        }
    }
}
