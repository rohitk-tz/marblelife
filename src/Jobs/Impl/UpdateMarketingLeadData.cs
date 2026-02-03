using Core.Application;
using Core.MarketingLead;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class UpdateMarketingLeadData : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public UpdateMarketingLeadData(): base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Update Marketing Lead Data Constructor- " + _clock.UtcNow); 
        }

        public override void Execute()
        {
            _logService.Info("Update Marketing Lead Data started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IUpdateMarketingLeadReportDataService>();
                pollingAgent.UpdateData();

            }
            catch (Exception e)
            {
                _logService.Error("Exception - Update Marketing Lead Data parser. ", e);
            }

            _logService.Info("Update Marketing Lead Data end at " + _clock.UtcNow);
        }

    }
}
