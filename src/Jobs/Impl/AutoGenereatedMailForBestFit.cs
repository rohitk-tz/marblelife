using Core.Application;
using Core.Scheduler;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class AutoGenereatedMailForBestFit : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public AutoGenereatedMailForBestFit()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Starting Auto Generated Mail For Best Fit- " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Starting Auto Generated Mail For Best Fit" + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IAutoGenereatedMailForBestFitNotification>();
                pollingAgent.ProcessRecords();
            }
            catch (Exception e)
            {
                _logService.Error("Exception -Starting Customer Response From Review Push. ", e);
            }

            _logService.Info("Starting Customer Response From Review Push end at " + _clock.UtcNow);
        }
    }

}
