using Core.Scheduler.ViewModel;

namespace Core.Scheduler
{
    public interface IJobInfoService
    {
        JobInfoEditModel Get(long id);
        bool UpdateJobTime(JobInfoEditModel model);
        bool Delete(long id);
    }
}
