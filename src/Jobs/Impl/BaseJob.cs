using Core.Application;
using Quartz;
using System;

namespace Jobs.Impl
{
    public abstract class BaseJob : IJob
    {
        private readonly ILogService _logService;
        protected BaseJob(ILogService logService)
        {
            _logService = logService;
        }
        public void Execute(IJobExecutionContext context)
        {
            var sessionLocator = ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>();
            sessionLocator.Setup();
            try
            {
                Execute();
            }
            catch
            {
            }
            finally
            {
                DisposeSession(sessionLocator);
            }
        }
        private void DisposeSession(IUnitOfWork uow)
        {
            //_logService.Info("Base JOb Dispose- ");
            try
            {
                if (uow != null)
                {
                    uow.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logService.Error("While disposing session!", ex);
            }
        }

        public abstract void Execute();
    }
}
