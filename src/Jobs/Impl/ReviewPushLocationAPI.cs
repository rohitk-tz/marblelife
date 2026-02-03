using Core.Application;
using Core.MarketingLead;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    class ReviewPushLocationAPI : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public ReviewPushLocationAPI()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Starting Review Push Location API- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Starting Review Push Location API start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IReviewPushLocationAPI>();
                pollingAgent.ProcessRecord();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Starting Review Push Location API. ", e);
            }

            _logService.Info("Starting Review Push Location API end at " + _clock.UtcNow);
        }
    }
}
