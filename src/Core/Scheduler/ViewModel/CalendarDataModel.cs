using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class CalendarDataModel
    {
        public string CalendarName { get; set; }
        public string Email { get; set; }
        public long TypeId { get; set; }
        public long FranchiseeId { get; set; } 
        public long AssigneeId { get; set; }
        public ICollection<JobEditModel> JobList { get; set; }
        public ICollection<JobEstimateEditModel> EstimateList { get; set; }
    }
}
