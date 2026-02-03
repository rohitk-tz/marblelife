using Core.Application;
using DependencyInjection;

namespace UpdateInvoiceItemInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            DependencyRegistrar.RegisterDependencies();
            ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
            ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
            DependencyRegistrar.SetupCurrentContextWinJob();

            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<UpdateInvoiceItemInfoService>(); 
            pollingAgent.UpdateReport();
        }
    }
}
