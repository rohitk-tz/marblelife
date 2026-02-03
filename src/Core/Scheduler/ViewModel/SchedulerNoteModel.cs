using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class SchedulerNoteModel : EditModelBase
    {
        public long? JobId { get; set; }
        public long? EstimateId { get; set; }
        public long? VacationId { get; set; }
        public long? MeetingId { get; set; }
        public long? StatusId { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status { get; set; }
        public long? Id { get; set; }
    }
}
