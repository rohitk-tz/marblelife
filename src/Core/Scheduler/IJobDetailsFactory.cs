using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler
{
    public interface IJobDetailsFactory
    {
        JobDetails CreateDomain(Job model);
        JobDetails CreateDomain(JobEstimateEditModel model);
        JobDetails CreateDomain(JobEstimate jobEstimate);
    }
}
