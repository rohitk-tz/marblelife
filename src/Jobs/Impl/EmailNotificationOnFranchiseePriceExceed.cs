using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application;
using Core.Reports.ViewModel;
using Quartz;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class EmailNotificationOnFranchiseePriceExceed : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public EmailNotificationOnFranchiseePriceExceed() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Send Email Notification on Franchisee Price Exceed at " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Send Email Notification on Franchisee Price Exceed at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IEmailNotificationOnFranchiseePriceExceed>();
            pollingAgent.NotificationOnFranchiseePriceExceed();
            _logService.Info("Send Email Notification on Franchisee Price Exceed at " + _clock.UtcNow);
        }
    }
}
