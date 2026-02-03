using Core.Application;
using Core.Reports;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class SyncS3bucket : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public SyncS3bucket() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Before/After Images Upload In Every 2 Min With S3 Bucket at " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Before/After Images Upload With S3 Bucket at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IS3BucketSync>();
            pollingAgent.S3BucketSyncInEvery2min();
            _logService.Info("Before/After Images Upload In Every 2 Min With S3 Bucket at " + _clock.UtcNow);
        }
    }
}
