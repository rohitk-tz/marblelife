using Core.Application;
using Core.Reports;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class CustomerFeedbackNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public CustomerFeedbackNotification()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Monthly Notification Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Monthly notification start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IMonthlyReviewNotificationService>();
                pollingAgent.CreateNotification();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Monthly notification. ", e);
            }

            _logService.Info("Monthly Notification end at " + _clock.UtcNow);
        }
    }
}
