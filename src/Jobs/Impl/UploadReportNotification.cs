using Core.Application;
using Core.Reports;
using System;

namespace Jobs.Impl
{
    class UploadReportNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public UploadReportNotification() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("UploadReport Notification Constructor- " + _clock.UtcNow); 
        }
        public override void Execute()
        {
            _logService.Info("UploadReport Notification started at " + _clock.UtcNow);

            try
            {
                var UpdateData = ApplicationManager.DependencyInjection.Resolve<ISalesDataUploadReportNotificationService>();
                UpdateData.CreateNotification(); 
            }
            catch (Exception e)
            {
                _logService.Error("Exception - UploadReport Notification. ", e);
            }

            _logService.Info("UploadReport Notification end at " + _clock.UtcNow);
        }
    }
}
