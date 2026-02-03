using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public  class DragDropSchedulerModel
    {
        public double? Days { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? MeetingId { get; set; }
        public long? JobId { get; set; }
        public long? EstimateId { get; set; }
        public long? PersonalId { get; set; }
        public double? Seconds { get; set; }
        public long? Id { get; set; }
    }
}
