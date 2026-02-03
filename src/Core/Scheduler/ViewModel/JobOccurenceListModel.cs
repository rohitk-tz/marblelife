using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobOccurenceListModel
    {
        public long ParentEstimateId { get; set; }
        public long ParentJobId { get; set; }
        public long FranchiseeId { get; set; }
        public long SchedulerId { get; set; }
        public long ServiceTypeId { get; set; }
        public string Title { get; set; }
        public IEnumerable<JobOccurenceEditModel> Collection { get; set; }
    }
}
