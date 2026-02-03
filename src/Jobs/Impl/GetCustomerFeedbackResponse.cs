using Core.Application;
using Core.Review;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class GetCustomerFeedbackResponse : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public GetCustomerFeedbackResponse()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Customer feedback Response Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Customer feedback Response start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IGetCustomerFeedbackService>();
                pollingAgent.GetFeedbackResponse();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Customer feedback Response. ", e);
            }

            _logService.Info("Customer feedback Response end at " + _clock.UtcNow);
        }
    }
}
