using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class MeetingEditModel : EditModelBase
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public long? EstimateId { get; set; }
        public long JobTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long FranchiseeId { get; set; }
        public string QBInvoiceNumber { get; set; }
        public long StatusId { get; set; }
        public long? MeetingId { get; set; }
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
        public string Assignee { get; set; }
        public DateTime ActualStartDateString { get; set; }
        public DateTime ActualEndDateString { get; set; }
        public JobOccurenceListModel JobOccurence { get; set; }
        public bool IsUser { get; set; }
        public bool IsEquipment { get; set; }
        public MeetingEditModel()
        {
            JobCustomer = new JobCustomerEditModel();
            IsImported = false;
        }
    }
}
