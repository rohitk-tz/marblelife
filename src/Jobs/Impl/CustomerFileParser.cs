using Core.Application;
using Core.Sales;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class CustomerFileParser : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public CustomerFileParser()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Customer File parser Constructor- " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Customer File parser started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<ICustomerFileUploadPollingAgent>();
                pollingAgent.ParseCustomerFile();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Customer File parser. ", e);
            }

            _logService.Info("Customer File parser end at " + _clock.UtcNow);
        }
    }
}
