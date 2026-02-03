using Core.Application;
using Core.Reports;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class SyncedEmailNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public SyncedEmailNotification() 
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("MailChimp Integration Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("MailChimp Integration Notification start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IEmailAPIIntegrationNotificationService>();
                pollingAgent.CreateNotification();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - MailChimp Integration Notification. ", e);
            }

            _logService.Info("MailChimp Integration Notification end at " + _clock.UtcNow);
        }
    }
}
