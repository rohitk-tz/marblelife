using Core.Application;
using Core.MarketingLead;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
  public  class NotificationToFANotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public NotificationToFANotification()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Starting Notification TO FA - " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Starting Notification TO FA " + _clock.UtcNow);
            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<INotificationToFA>();
                pollingAgent.ProcessRecords();
            }
            catch (Exception e)
            {
                _logService.Error("Exception -Starting Notification TO FA. ", e);
            }

            _logService.Info("Starting Notification TO FA end at " + _clock.UtcNow);
        }
    }
}
