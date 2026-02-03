using Core.Application;
using Core.Sales;
using Core.Scheduler;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class NewJobNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public NewJobNotification()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("New Job Reminder - " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("New Job started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IJobReminderNotificationService>();
                pollingAgent.CreateNotification();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - JobReminder to Customer Constructor. ", e);
            }

            _logService.Info("JobReminder to Customer Constructor end at " + _clock.UtcNow);
        }
    }
}
