using Core.Application;
using Core.Review;
using DependencyInjection;

namespace ReviewSystemAPITest
{
    class Program
    {
        static void Main(string[] args)
        {
            DependencyRegistrar.RegisterDependencies();
            ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
            ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
            DependencyRegistrar.SetupCurrentContextWinJob();

            string clientId = "5a475638898b9b51b8fa2f241d24bc241ab8603b";
            int businessId = 34700;
            int businessId_2 = 35346;
            long customerId = 28477443;

            var triggerMail = ApplicationManager.DependencyInjection.Resolve<ICustomerFeedbackService>();
            //triggerMail.GetFeedback(clientId, businessId);
        }
    }
}
