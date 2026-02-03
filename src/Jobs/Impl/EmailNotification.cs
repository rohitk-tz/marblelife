using Core.Application;
using Core.Notification;
using Quartz;
using System;


namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class EmailNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public EmailNotification()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("EmailNotification Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Email notification service started at " + _clock.UtcNow);

            try
            {
                var notificationPollingAgent = ApplicationManager.DependencyInjection.Resolve<INotificationPollingAgent>();
                notificationPollingAgent.PollForNotifications();

            }
            catch (Exception e)
            {
                _logService.Error("Email notification. " + e);
            }

            _logService.Info("Email notification service end at " + _clock.UtcNow);
        }
    }
}
