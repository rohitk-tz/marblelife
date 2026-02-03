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
    public class EmailNotificationForPhotoReport : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public EmailNotificationForPhotoReport() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Send Email Notification for Photo Report at " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Send Email Notification for Photo Report at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IEmailNotificationForPhotoReport>();
            pollingAgent.SendEmailNotificationForPhotoReport();
            _logService.Info("Send Email Notification for Photo Report at " + _clock.UtcNow);
        }
    }
}
