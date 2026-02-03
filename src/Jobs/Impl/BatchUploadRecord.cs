using Core.Application;
using Core.Reports;
using System;

namespace Jobs.Impl
{
    class BatchUploadRecord : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public BatchUploadRecord() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Upload Record Notification Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("UploadReport Notification started at " + _clock.UtcNow);

            try
            {
                var uploadReport = ApplicationManager.DependencyInjection.Resolve<IUpdateBatchUploadRecordService>();
                uploadReport.UpdateData();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Upload Record Notification. ", e);
            }

            _logService.Info("Upload Record Notification end at " + _clock.UtcNow);
        }
    }
}
