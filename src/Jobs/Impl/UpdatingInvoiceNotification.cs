using Core.Application;
using Core.Sales;
using Quartz;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class UpdatingInvoiceNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public UpdatingInvoiceNotification() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Updation Of Invoice Details Starting at " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Updation Of Invoice Details started at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IUpdatingInvoiceNotificationServices>();
            pollingAgent.ProcessRecords();
            _logService.Info("Updation Of Invoice Details Ended at " + _clock.UtcNow);
        }
    }
}
