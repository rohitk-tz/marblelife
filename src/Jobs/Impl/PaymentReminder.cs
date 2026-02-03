using Core.Application;
using Core.Notification;
using System;

namespace Jobs.Impl
{
    public class PaymentReminder : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public PaymentReminder()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Payment Reminder - " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Payment Reminder started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IPaymentReminderPollingAgent>();
                pollingAgent.CreateNotificationReminderForPayment();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Payment Reminder. ", e);
            }

            _logService.Info("Payment Reminder end at " + _clock.UtcNow);
        }
    }
}
