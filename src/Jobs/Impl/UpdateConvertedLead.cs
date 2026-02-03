using Core.Application;
using Core.MarketingLead;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class UpdateConvertedLead : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public UpdateConvertedLead()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Update Converted Lead Constructor - " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Update Converted Lead Constructor start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IUpdateConvertedLeadsService>();
                pollingAgent.UpdateLeads();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Update Converted Lead Constructor. ", e);
            }

            _logService.Info("Update Converted Lead Constructor end at " + _clock.UtcNow);
        }
    }
}
