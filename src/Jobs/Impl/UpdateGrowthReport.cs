using Core.Application;
using Core.Reports;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class UpdateGrowthReport : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public UpdateGrowthReport()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Update Growth Report Data Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Update Growth Report start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IUpdateSalesAmountService>();
                pollingAgent.UpdateData();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Update Growth Report. ", e);
            }

            _logService.Info("Update Growth Report end at " + _clock.UtcNow);
        }
    }
}
