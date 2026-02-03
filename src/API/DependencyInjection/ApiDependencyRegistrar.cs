using Api;
using Api.Areas.Application;
using Api.Areas.Application.Impl;
using Core.Application;
using DependencyInjection;

namespace API.DependencyInjection
{
    public class ApiDependencyRegistrar
    {
        public static void RegisterDepndencies()
        {
            DependencyRegistrar.RegisterDependencies();

            ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
            ApplicationManager.DependencyInjection.Register<ISessionContext, SessionContext>();
            ApplicationManager.DependencyInjection.Register<IDropDownHelperService, DropDownHelperService>();

            DependencyRegistrar.SetupCurrentContext(new AppContextStore());
        }
    }
}