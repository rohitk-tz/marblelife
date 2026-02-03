using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobSchedulerEditModel : EditModelBase
    {
        public long Id { get; set; }
        public long? JobId { get; set; }
        public long? EstimateId { get; set; }
        public long? AssigneeId { get; set; }
        public long FranchiseeId { get; set; }
        public long? SalesRepId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ActualStartDateString { get; set; }
        public DateTime ActualEndDateString { get; set; }
        public long? PersonId { get; set; }
        public string Alias { get; set; }
        public string AssigneeName { get; set; }
        public string Title { get; set; }
        //public IEnumerable<FileModel> FileList { get; set; }
        public bool IsActive { get; set; }
        public bool IsImported { get; set; }
        public long? ServiceTypeId { get; set; }
        public int? Offset { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        public long? SchedulerStatus { get; set; }

        public bool IsFromId { get; set; }

        public List<long> InvoiceNumbers { get;set;}
        public string InvoiceNames {get;set;}

        public List<InvoiceNumbersEditModel> InvoiceNumber { get; set; }
        public JobSchedulerEditModel()
        {
            //FileList = new List<FileModel>();
            IsActive = true;
        }
    }

    [NoValidatorRequired]
    public class JobInvoiceDownloadViewModel
    {
        public long SchedulerId { get; set; }

        public List<long> InvoiceNumbers { get; set; }
    }

}
