using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler
{
  public  interface IBeforeAfterImageService
    {
        List<BeforeAfterForImageViewModel> GetBeforeAfterImagesForFranchiseeAdmin(List<long> schedulerIds, DateTime startDate,
              DateTime endDates, BeforeAfterImageFilter filter, List<JobScheduler> jobSchedulerList
              , List<JobEstimateServices> jobEstimateServicesList, List<JobEstimateImage> jobEstimateImagesList);
    }
}
