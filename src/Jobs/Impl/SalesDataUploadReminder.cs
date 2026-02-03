using Core.Application;
using Core.Sales;
using System;

namespace Jobs.Impl
{
    public class SalesDataUploadReminder : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public SalesDataUploadReminder()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("SalesData Upload Reminder - " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("SalesData Upload started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<ISalesDataUploadReminderPollingAgent>();
                pollingAgent.CreateNotificationReminderForSalesDataUpload();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - SalesData Upload Reminder. ", e);
            }

            _logService.Info("SalesData Upload Reminder end at " + _clock.UtcNow);
        }
    }
}
