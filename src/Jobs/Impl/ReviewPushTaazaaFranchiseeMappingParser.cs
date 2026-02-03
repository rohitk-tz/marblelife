using Core.Application;
using Core.Organizations;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public  class ReviewPushGettingCustomerFeedbackParser : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public ReviewPushGettingCustomerFeedbackParser()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Starting Customer Response From Google API- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Starting Customer Response From Google API " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IReviewPushGettingCustomerFeedback>();
                pollingAgent.ProcessRecords();
            }
            catch (Exception e)
            {
                _logService.Error("Exception -Starting Starting Customer Response From Google API. ", e);
            }

            _logService.Info("Starting Starting Customer Response From Google API end at " + _clock.UtcNow);
        }
    }
}
