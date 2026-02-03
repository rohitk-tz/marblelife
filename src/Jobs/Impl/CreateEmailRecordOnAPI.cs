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
    public class CreateEmailRecordOnAPI : BaseJob
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public CreateEmailRecordOnAPI()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Create Email Record Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("Create Email Record start at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<ICreateEmailRecordForApiService>();
                pollingAgent.CreateEmailRecord();
            }
            catch (Exception e)
            {
                _logService.Error("Exception - Create Email Record. ", e);
            }

            _logService.Info("Create Email Record end at " + _clock.UtcNow);
        }
    }
}
