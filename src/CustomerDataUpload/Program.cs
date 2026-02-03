using Core.Application;
using DependencyInjection;
using System;

namespace CustomerDataUpload
{
    class Program
    {
        static void Main(string[] args)
        {
            GC.Collect();
            DependencyRegistrar.RegisterDependencies();
            ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
            ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
            DependencyRegistrar.SetupCurrentContextWinJob();

            var pollingAgent = ApplicationManager.DependencyInjection.Resolve<CustomerDataUploadPollingAgent>();
            pollingAgent.ParseCustomerData();
        }
    }
}
