using Core.Application;
using Core.Sales;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class SalesDataParser : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public SalesDataParser()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Sales Data parser Constructor- " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Sales Data parser started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<ISalesDataParsePollingAgent>();
                pollingAgent.ParseFile();

            }
            catch (Exception e)
            {
                _logService.Error("Exception - Sales Data parser. ", e);
            }

            _logService.Info("Sales Data parser end at " + _clock.UtcNow);
        }
    }
}
