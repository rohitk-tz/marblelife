using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    public class JobAmount
    {
        public decimal? JobValue { get; set; }
        public long InvoiceNumber { get; set; }
        public long EstimateInvoiceId { get; set; }
    }

    public class JobAmountDuplicacyCheck
    {
        public long InvoiceNumber { get; set; }
        public long EstimateInvoiceId { get; set; }
    }

    public class JobViewModel
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public long TechId { get; set; }
        public string Assignee { get; set; }
        public string Alias { get; set; }
        public long JobId { get; set; }
        public string JobTitle { get; set; }
        public long EstimateId { get; set; }
        public long SalesRepId { get; set; }
        public long CustomerId { get; set; }
        public IEnumerable<JobResourceEditModel> JobResource { get; set; }
        public JobCustomerEditModel JobCustomer { get; set; }
        public string Franchisee { get; set; }
        public string JobType { get; set; }
        public string QBinvoiceNumber { get; set; }
        public string SalesRep { get; set; }
        public string Description { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public string title { get; set; }
        public bool allDay { get; set; }
        public string backgroundColor { get; set; }
        public string color { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
        public long StatusId { get; set; }
        public string GeoCode { get; set; }
        public bool durationEditable { get; set; }
        public string CreatedBy { get; set; }
        public string SchedulerTitle { get; set; }
        public bool Imported { get; set; }
        public string EstimateType { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsVacation { get; set; }
        public long MeetingId { get; set; }
        public bool isParent { get; set; }
        public bool IsLock { get; set; }
        public long? SchedulerStatus { get; set; }
        public string SchedulerStatusColor { get; set; }
        public bool CanSchedule { get; set; }
        public bool? IsCustomerMailSend { get; set; }
        public decimal? Amount { get; set; }
        public List<JobAmount> JobAmount { get; set; }
        public bool? IsInvoiceSigned { get; set; }
        public string ServiceTypeName { get; set; }
        public bool IsInvoicePresent { get; set; }
        public decimal? EstimatedValue { get; set; }
        public float? InvoiceValue { get; set; }
        public decimal? JobTotal { get; set; }
        public string NotesForEstimate { get; set; }
        public string NotesForJob { get; set; }
        public decimal EstimatedAmount { get; set; }
        public float InvoiceAmount { get; set; }
    }
}
