using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobEditModel : EditModelBase
    {
        public long? Id { get; set; }
        public long JobId { get; set; }
        public long? EstimateId { get; set; }
        public long JobTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ActualStartDateString { get; set; }
        public DateTime ActualEndDateString { get; set; }
        public long FranchiseeId { get; set; }
        public string QBInvoiceNumber { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public string Franchisee { get; set; }
        public string jobType { get; set; }
        public string StatusColor { get; set; }
        public ICollection<long> TechIds { get; set; }
        public ICollection<long> JobAssigneeIds { get; set; }
        public IEnumerable<JobSchedulerEditModel> JobSchedulerList { get; set; }
        public IEnumerable<SchedulerNoteModel> Notes { get; set; }
        public JobCustomerEditModel JobCustomer { get; set; }
        public long? SalesRepId { get; set; }
        public long SchedulerStatus { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string SalesRep { get; set; }
        public bool IsEstimateDeleted { get; set; }
        public string CreatedBy { get; set; }
        public bool SetGeoCode { get; set; }
        public string GeoCode { get; set; }
        public long AssigneeId { get; set; }
        public string Location { get; set; }
        public bool IsImported { get; set; }
        public long? ServiceTypeId { get; set; }
        public string jobTypeName { get; set; }
        public string AssigneeName { get; set; }
        public string AssigneePhone { get; set; }
        public string dateType { get; set; }
        public DateTime EmailstartDate { get; set; }
        public DateTime EmailendDate { get; set; }
        public JobOccurenceListModel JobOccurence { get; set; }
        public long? LoggedInUserId { get; set; }
        public long? EstimateSchedulerId { get; set; }
        public string SchedulerStatusName { get; set; }
        public string SchedulerStatusColor { get; set; }
        public bool IsActive { get; set; }
        public bool IsDataToBeUpdateForAllJobs { get; set; }
        public bool IsRepeat { get; set; }
        public bool IsHavingMoreThanOneDay { get; set; }
        public string FullAddress { get; set; }
        public long? SchedulerId { get; set; }
        public long? TodayToDoCount { get; set; }
        public long? InvoiceId { get; set; }
        public long? EstimateInvoiceId { get; set; }
        public decimal? Worth { get; set; }
        public bool IsInvoicePresent { get; set; }
        public bool IsInvoiceRequired { get; set; }
        public bool? IsCustomerMailSend { get; set; }
        public List<JobEstimateSignatureEditModel> JobSignaturePre { get; set; }
        public List<JobEstimateSignatureEditModel> JobSignaturePost { get; set; }
        public List<long?> AllInvoiceNumbersSignedPre { get; set; }
        public List<long?> AllInvoiceNumbersSignedPost { get; set; }
        public List<long?> AllInvoiceNumbersSignedForEstimate { get; set; }
        public bool? IsSigned { get; set; }
        public string MailBody { get; set; }
        public string MailBodyToAdmin { get; set; }
        public decimal? LessDeposit { get; set; }
        public DateTime? StartDateForEstimate { get; set; }
        public string Assignee { get; set; }
        public bool? IsCustomerAvailable { get; set; }
        public DateTime CreatedOn { get; set; }
        public JobEditModel()
        {
            JobCustomer = new JobCustomerEditModel();
            IsImported = false;
        }
    }
}
