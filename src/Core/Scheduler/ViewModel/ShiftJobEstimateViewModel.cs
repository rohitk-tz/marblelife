
using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class ShiftJobEstimateViewModel : EditModelBase
    {
        public long? Id { get; set; }
        public long? MarketingClassId { get; set; }
        public long? JobId { get; set; }
        public long? EstimateId { get; set; }
        public long? SchedulerId { get; set; }
        public List<long?> FileIds { get; set; }

        public long? ShiftId { get; set; }
    }
}
