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
    public class MailForNonResidentalBuildingType : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public MailForNonResidentalBuildingType()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Starting Mail For Before After For Franchisee Admin- " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Starting Mail For Before After For Franchisee Admin " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IMailForNonResidentalBuildingTypeNotification>();
                pollingAgent.ProcessRecords();
            }
            catch (Exception e)
            {
                _logService.Error("Exception -Before After For Franchisee Admin. ", e);
            }

            _logService.Info("Starting Before After For Franchisee Admin end at " + _clock.UtcNow);
        }
    }
}
