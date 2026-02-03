using Core.Application;
using Core.MarketingLead;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class GetRoutingNumber : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public GetRoutingNumber() 
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Get Routing number Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Get Routing number start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IGetRoutingNumberService>();
                pollingAgent.GetRoutingNumber();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Routing number. ", e);
            }

            _logService.Info("Get Routing number end at " + _clock.UtcNow);
        }
    }
}
