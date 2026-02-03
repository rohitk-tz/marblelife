using Core.Application;
using Core.MarketingLead;
using Core.Scheduler;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class ZipParserNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public ZipParserNotification() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Zip Parser Starting at " + _clock.UtcNow);
        }
            public override void Execute()
        {
            _logService.Info("Zip Parser started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IZipParserNotificationService>();
                pollingAgent.ProcessRecords();

            }
            catch (Exception e)
            {
                _logService.Error("Exception - Zip Parser Starting at. ", e);
            }

            _logService.Info("Zip Parser end at " + _clock.UtcNow);
        }
    }
}
