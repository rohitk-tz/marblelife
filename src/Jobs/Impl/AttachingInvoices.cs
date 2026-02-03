using Core.Application;
using Core.Scheduler;
using Quartz;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class AttachingInvoices : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public AttachingInvoices() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Attaching Of Invoice Details Starting at " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Attaching Of Invoice Details started at " + _clock.UtcNow);
            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<IAttachingInvoicesServices>();
            pollingAgent.AttachInvoice();
            _logService.Info("Attaching Of Invoice Details Ended at " + _clock.UtcNow);
        }
    }
}
