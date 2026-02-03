
using Core.Application;
using Core.MarketingLead;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class HomeAdvisorParserNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public HomeAdvisorParserNotification()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Starting Auto Generated Mail For Best Fit- " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Starting Home Advisor Job" + _clock.UtcNow);
            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IHomeAdvisorParser>();
                pollingAgent.ProcessRecords();
            }
            catch (Exception e)
            {
                _logService.Error("Exception -Home Advisor Job. ", e);
            }

            _logService.Info("Starting Home Advisor Job end at " + _clock.UtcNow);
        }
    }
}
