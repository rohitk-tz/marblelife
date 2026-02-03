using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Sales;
using Core.Sales.Impl;
using Core.Users;
using Core.Users.Impl;
using Infrastructure.Application.Impl;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Reflection;

namespace DependencyInjection
{
    public class DependencyRegistrar
    {
        public static void RegisterDependencies()
        {
            RegisterDefaultImplementations();
            IoC.Register<IDependencyInjectionHelper, DependencyInjectionHelper>();
            IoC.Register<ILoginAuthenticationModelValidator, LoginAuthenticationModelValidator>();
            IoC.Register<ISalesDataUploadCreateModelValidator, SalesDataUploadCreateModelValidator>();

            // IoC.Register(typeof(IRepository<>), typeof(Repository<>));
            // IoC.Register<IUserCodeAccessModelValidator, UserCodeAccessModelValidator>();


            // TODO: Random is not thread safe
            IoC.RegisterInstance(new Random());
            SetApplicationManager();
        }

        public static void SetupCurrentContextWinJob()
        {
            IoC.Register<IUnitOfWork, UnitOfWork>(new PerThreadLifetimeManager());
            IoC.Register<IClock, Clock>();

        }

        public static void SetupCurrentContext(IAppContextStore appContextStore)
        {
            //  IoC.Register<ISessionLocator, SessionLocator>(new AppContextLifetimeManager(appContextStore));

            IoC.Register<IUnitOfWork, UnitOfWork>(new AppContextLifetimeManager(IoC.Resolve<IAppContextStore>()));
            IoC.Register<IClock, Clock>(new AppContextLifetimeManager(IoC.Resolve<IAppContextStore>()));

        }

        /* public static void SetupCurrentContextWinApp()
         {
             IoC.Register<ISessionLocator, SessionLocator>(new PerThreadLifetimeManager());
             IoC.Register<ITransactionHelper, TransactionHelper>(new PerThreadLifetimeManager());
         }*/

        public static void SetApplicationManager()
        {
            ApplicationManager.DependencyInjection = IoC.Resolve<IDependencyInjectionHelper>();
            ApplicationManager.Settings = IoC.Resolve<ISettings>();
            // ApplicationManager.Clock = IoC.Resolve<IClock>();
        }

        private static void RegisterDefaultImplementations()
        {
            var publicTypes = typeof(DependencyRegistrar).Assembly.GetReferencedAssemblies().Select(Assembly.Load).SelectMany(a => a.GetTypes());
            var interfaces = publicTypes.Where(t => t.IsInterface);
            var defaultImplementations = publicTypes.Where(type => Attribute.IsDefined(type, typeof(DefaultImplementationAttribute)));

            foreach (var type in defaultImplementations)
            {
                var attribute = (DefaultImplementationAttribute)type.GetCustomAttributes(typeof(DefaultImplementationAttribute), false).Single();
                Type typeClosure = type;

                attribute.Interface = attribute.Interface ?? interfaces.Single(i => i.Name == "I" + typeClosure.Name);
                IoC.Register(attribute.Interface, type);
            }
        }
        public static void SetLocalizationBasedRegistration(string systemTimeZoneId)
        {
            IoC.Register<IClock, Clock>(new InjectionConstructor(systemTimeZoneId));
        }

    }
}
