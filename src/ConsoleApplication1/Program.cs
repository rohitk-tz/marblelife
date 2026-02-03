using Core.Application;
using DependencyInjection;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            DependencyRegistrar.RegisterDependencies();
            ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
            ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
            DependencyRegistrar.SetupCurrentContextWinJob();

            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<CurrencyExchangeRateService>();
            pollingAgent.GetAllCurrencyRate();
        }
    }
}
