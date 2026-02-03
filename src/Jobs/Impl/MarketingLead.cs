using Core.Application;
using Core.MarketingLead;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class MarketingLead : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public MarketingLead()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Create Marketing Lead Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Get Marketing start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IMarketingLeadsService>();
                pollingAgent.GetMarketingLeads(); 
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Marketing Lead. ", e);
            }

            _logService.Info("Get marketing lead end at " + _clock.UtcNow);
        }
    }
}
