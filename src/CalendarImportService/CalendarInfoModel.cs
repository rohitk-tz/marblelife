using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;

namespace CalendarImportService
{
    public class CalendarInfoModel
    {
        public long JobId { get; set; }
        public long? EstimateId { get; set; }
        public long JobTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
        public string Description { get; set; }
        public string Title { get; set; }
        public string SalesRep { get; set; }
        public bool IsEstimateDeleted { get; set; }
        public string CreatedBy { get; set; }
        public bool SetGeoCode { get; set; }
        public string GeoCode { get; set; }

    }
}
