using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler
{
   public interface IDebuggerLog
    {
        string CreateDebugger(JobEstimateServices jobEstimateServicesFromUI, List<JobEstimateServices> jobEstimateServicesFromDb,string text,out string debuggerLogs);
        string CreateDebuggerForImage(JobEstimateImage jobEstimateServicesImageFromUI, List<JobEstimateImage> jobEstimateServicesFromDb, string text, out string debuggerLogs);
    }
}
