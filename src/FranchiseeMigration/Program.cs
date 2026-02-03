using Core.Application;
using Core.Organizations.Impl;
using DependencyInjection;
using System;

namespace FranchiseeMigration
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

            var fCol = ApplicationManager.DependencyInjection.Resolve<FranchiseeFromFileCollection>();

            var collection = fCol.FranchiseesDetails();
            var franchiseeMigrationService = ApplicationManager.DependencyInjection.Resolve<FranchiseeMigrationService>();

            franchiseeMigrationService.ProcessRecords(collection);
        }
    }
}
