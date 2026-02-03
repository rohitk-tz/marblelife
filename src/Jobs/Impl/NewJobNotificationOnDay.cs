using Core.Application;
using Core.Sales;
using Core.Scheduler;
using Jobs.Impl;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class NewJobNotificationOnDay : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public NewJobNotificationOnDay()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("New JobEstimateReminder Reminder - " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("New JobEstimateReminder started at " + _clock.UtcNow);

            try
            {
                //    var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IJobReminderNotificationtoUsersService>();
                //    pollingAgent.CreateNotification();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - JobEstimateReminder to User Constructor. ", e);
            }

            _logService.Info("JobEstimateReminder to User Constructor end at " + _clock.UtcNow);
        }
    }

}
