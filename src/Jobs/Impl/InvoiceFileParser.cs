using Core.Application;
using Core.Sales;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class InvoiceFileParser : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public InvoiceFileParser(): base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Invoice File parser Constructor- " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Invoice File parser started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IInvoiceItemUpdateInfoService>();
                pollingAgent.UpdateReport();

            }
            catch (Exception e)
            {
                _logService.Error("Exception - Invoice File parser. ", e);
            }

            _logService.Info("Invoice File parser end at " + _clock.UtcNow);
        }

    }
}
