using Core.Application;
using Core.Reports;
using Core.Scheduler;
using Quartz;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class UploadPriceEstimateData : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public UploadPriceEstimateData() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Upload Price Estimate Data Starting at " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Upload Price Estimate Data started at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IPriceEstimateParserNotificationService>();
            pollingAgent.ProcessRecords();
            _logService.Info("Upload Price Estimate Data Ended at " + _clock.UtcNow);
        }

    }
}
