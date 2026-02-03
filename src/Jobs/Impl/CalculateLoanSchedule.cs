using Core.Application;
using Core.Billing;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    public class CalculateLoanSchedule : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public CalculateLoanSchedule() : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Loan Re-Schedule Constructor- " + _clock.UtcNow);
        }

        public override void Execute()
        {
            _logService.Info("Loan Re-Schedule started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<ICalculateLoanScheduleService>();
                pollingAgent.CalculateSchedule();

            }
            catch (Exception e)
            {
                _logService.Error("Exception - Loan Re-Schedule. ", e);
            }

            _logService.Info("Loan Re-Schedule end at " + _clock.UtcNow);
        }
    }
}
