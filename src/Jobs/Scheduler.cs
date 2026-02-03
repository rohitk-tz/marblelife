using Core.Application;
using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Jobs
{
    [RunInstaller(true)]
    partial class Scheduler : ServiceBase
    {
        private readonly IClock _clock;
        private readonly ILogService _logService;
        ITaskScheduler _scheduler;
        public Scheduler()
        {
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            if (Environment.UserInteractive)
            {
                _logService.Info("Email notification service started at " + _clock.Now);
                //Console.WriteLine("Initializing Email notification service");
            }
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {

            if (Environment.UserInteractive)
            {
                _logService.Info("Starting job in verbose mode " + _clock.Now);
                //Console.WriteLine("Starting Email notification on start");
            }
            else
            {
                _logService.Info("Starting job as service " + _clock.Now);
            }
            _scheduler = new TaskScheduler();
            _scheduler.Run();
        }

        public void OnStartPublic(string[] args)
        {
            OnStart(args);
        }

        public void OnStopPublic()
        {
            OnStop();
        }

        protected override void OnStop()
        {
            if (Environment.UserInteractive)
            {
                _logService.Info("Initializing  job on stop " + _clock.Now);
                //Console.WriteLine("Initializing Email notification service on stop");
            }
            else
            {
                _logService.Info("Initializing job service on stop " + _clock.Now);
            }
            if (_scheduler != null)
            {
                _scheduler.Stop();
            }
        }
    }
}
