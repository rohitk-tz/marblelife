using System;
using Core.Application;
using Infrastructure.Billing;

namespace Jobs.Impl
{
    public class CurrencyExchangeRateGenerator : BaseJob
    {       
        private readonly ILogService _logService;
        private readonly IClock _clock;

        public CurrencyExchangeRateGenerator()
           : base(ApplicationManager.DependencyInjection.Resolve<ILogService>())
        {
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("CurrencyExchangeRateGenerator Constructor- " + _clock.UtcNow);
        }
        public override void Execute()
        {
            _logService.Info("CurrencyExchangeRateGenerator started at " + _clock.UtcNow);

            try
            {
                var pollingAgent = ApplicationManager.DependencyInjection.Resolve<ICurrencyRateService>();
                pollingAgent.AllCurrencyRateByDate();

            }
            catch (Exception e)
            {
                _logService.Error("Exception - CurrencyExchangeRateGenerator. ", e);
            }

            _logService.Info("CurrencyExchangeRateGenerator end at " + _clock.UtcNow);
        }
    }
}
