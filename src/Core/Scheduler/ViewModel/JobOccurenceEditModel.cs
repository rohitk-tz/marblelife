using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobOccurenceEditModel : EditModelBase
    {
        public long ScheduleId { get; set; }
        public long AssigneeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ActualStartDateString { get; set; }
        public DateTime ActualEndDateString { get; set; }
        public long ParentJobId { get; set; }
        public long ParentEstimateId { get; set; }
        public long FranchiseeId { get; set; }
        public string Title { get; set; }
        public long? ServiceTypeId { get; set; }
        public List<InvoiceNumbersEditModel> InvoiceNumber { get; set; }
    }

    [NoValidatorRequired]
    public class InvoiceNumbersEditModel
    {
        public long? Id { get; set; }

        public string Label { get; set; }
    }
}
