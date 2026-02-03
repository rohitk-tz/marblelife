using Core.Application;
using Core.Billing;
using System;

namespace Jobs.Impl
{
    public class InvoiceLateFeeGenerator : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public InvoiceLateFeeGenerator() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("InvoiceLateFee Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("InvoiceLateFeeGenerator started at " + _clock.UtcNow);

            try
            {
                var invoicelatefee = ApplicationManager.DependencyInjection.Resolve<IInvoiceLateFeePollingAgent>();
                invoicelatefee.LateFeeGenerator();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - InvoiceLateFeeGenerator. ", e);
            }

            _logService.Info("InvoiceLateFeeGenerator end at " + _clock.UtcNow);
        }
    }
}
