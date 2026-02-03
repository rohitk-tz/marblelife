using Core.Application;
using Core.Scheduler;
using Quartz;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class SalesTaxAPI : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public SalesTaxAPI() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Gettting Sales Tax Data  Starting at " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Getting Sales Tax Data For Every State started at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<ISalesTaxAPIServices>();
            pollingAgent.GetSalesTaxAPI();
            _logService.Info("Getting Sales Tax Data For Every State Ended at " + _clock.UtcNow);
        }

    }
}
