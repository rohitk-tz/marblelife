using Core.Application;
using Core.Reports;
using Core.Reports.ViewModel;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class BeforeAfterImagesUploadwithS3Bucket : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public BeforeAfterImagesUploadwithS3Bucket() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Before/After Images Upload With S3 Bucket at " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Before/After Images Upload With S3 Bucket at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IBeforeAfterImagesUploadwithS3Bucket>();
            pollingAgent.UploadBeforeAfterImageswithS3Bucket();
            _logService.Info("Before/After Images Upload With S3 Bucket at " + _clock.UtcNow);
        }
    }
}
