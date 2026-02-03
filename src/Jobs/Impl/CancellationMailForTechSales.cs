using Core.Application;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class CancellationMailForTechSales : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public CancellationMailForTechSales()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Starting Auto Generated Mail For Best Fit- " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Starting Auto Generated Cancellation Mail For Tech/Sales" + _clock.UtcNow);
            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<Core.Scheduler.ICancellationMailForTechSalesNotification>();
                pollingAgent.ProcessRecords();
            }
            catch (Exception e)
            {
                _logService.Error("Exception -Starting Customer Response From Review Push. ", e);
            }

            _logService.Info("Starting Customer Response From Review Push end at " + _clock.UtcNow);
        }
    }
}
