using Core.Application;
using Core.Sales;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public  class UpdatingInvoiceIdsNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public UpdatingInvoiceIdsNotification() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Updation Of Invoice Ids Starting at " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Updation Of Invoice Ids started at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IUpdatingInvoiceIdsNotificationServices>();
            pollingAgent.UpdateInvoiceIds();
            _logService.Info("Updation Of Invoice Ids Ended at " + _clock.UtcNow);
        }
    }
}
