using Core.Application;
using Core.Sales;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class AnnualFileParser : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public AnnualFileParser() 
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Annual Audit Data parser Constructor- " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Annual Audit Data parser started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IAnnualSalesDataParsePollingAgent>();
                pollingAgent.ParseFile(); 

            }
            catch (Exception e)
            {
                _logService.Error("Exception - Annual Audit Data parser. ", e);
            }

            _logService.Info("Annual Audit Data end at " + _clock.UtcNow);
        }
    }
}
