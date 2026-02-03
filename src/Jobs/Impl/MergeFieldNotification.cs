using Core.Application;
using Core.Reports;
using Quartz;
using System;

namespace Jobs.Impl
{
    [DisallowConcurrentExecution]
    class MergeFieldNotification : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        public MergeFieldNotification()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Monthly serviced customer Notification Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Monthly serviced customer notification start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<ICreateMergeRecordForApiService>();
                pollingAgent.AddMergeField();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Monthly serviced customer notification. ", e);
            }

            _logService.Info("Monthly serviced customer Notification end at " + _clock.UtcNow);
        }
    }
}
