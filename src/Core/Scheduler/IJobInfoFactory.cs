using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;

namespace Core.Scheduler
{
    public interface IJobInfoFactory
    {
        JobInfoEditModel CreateJobInfoModel(JobScheduler scheduler);
    }
}
