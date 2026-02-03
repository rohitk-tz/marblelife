using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class VacationRepeatEditModel : EditModelBase
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public long RepeatFrequency { get; set; }
        public long FranchiseeId { get; set; }
        public long AssigneeId { get; set; }
        public long Id { get; set; }
        public long VacationId { get; set; }
        public string Title { get; set; }
        public long? ParentId { get; set; }
        public long? MeetingId { get; set; }
        public long? PersonId { get; set; }
    }
}
