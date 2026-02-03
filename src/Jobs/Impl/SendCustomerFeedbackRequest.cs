using Core.Application;
using Core.Review;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class SendCustomerFeedbackRequest : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public SendCustomerFeedbackRequest()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Review feedback Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Review feedback start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<ISendFeedBackRequestPollingAgent>();
                pollingAgent.SendFeedback();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Review Feedback. ", e);
            }

            _logService.Info("Review feedback end at " + _clock.UtcNow);
        }
    }
}
