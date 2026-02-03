using Core.Scheduler.ViewModel;
using System.Collections.Generic;

namespace Core.Scheduler
{
   public interface IJobSchedulerService
    {
        void ChangeSchedulerDateTime(JobEditModel model, DatetimeModel dateTimeModel, List<OldSchedulerModel> oldSchedulerList);
    }
}
