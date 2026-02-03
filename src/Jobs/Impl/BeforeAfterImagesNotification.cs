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
    public  class BeforeAfterImagesNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public BeforeAfterImagesNotification() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Zip Parser Starting at " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Before After Images Migration started at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IBeforeAfterImagesNotificationServices>();
            pollingAgent.ProcessRecords();
            _logService.Info("Before After Images Migration Ended at " + _clock.UtcNow);
        }
    }
}
