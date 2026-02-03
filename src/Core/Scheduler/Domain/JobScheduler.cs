using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class JobScheduler : DomainBase
    {
        public long? JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        public long? EstimateId { get; set; }
        [ForeignKey("EstimateId")]
        public virtual JobEstimate Estimate { get; set; }

        public string Title { get; set; }

        public long? AssigneeId { get; set; }
        [ForeignKey("AssigneeId")]
        public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime StartDateTimeString { get; set; }
        public DateTime EndDateTimeString { get; set; }

        public long FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long? SalesRepId { get; set; }
        [ForeignKey("SalesRepId")]
        public virtual OrganizationRoleUser SalesRep { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        [CascadeEntity]
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }


        public double? Offset { get; set; }
        public bool IsImported { get; set; }

        public bool IsActive { get; set; }
        public long? ServiceTypeId { get; set; }
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }
        public bool IsVacation { get; set; }

        public long SchedulerStatus { get; set; }
        [ForeignKey("SchedulerStatus")]
        public virtual Lookup Lookup { get; set; }
        public long? MeetingID { get; set; }
        [ForeignKey("MeetingID")]
        public virtual Meeting Meeting { get; set; }

        public long? PersonId { get; set; }
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }
        public string QBInvoiceNumber { get; set; }
        //public double? Offset { get; set; }

        public long? ParentJobId { get; set; }
        [ForeignKey("ParentJobId")]
        public virtual JobScheduler JobScheduler1 { get; set; }
        public virtual ICollection<JobNote> VacationNote { get; set; }

        public bool IsRepeat { get; set; }

        public bool IsCancellationMailSend { get; set; }
        public bool IsInvoiceRequired { get; set; }

        public string InvoiceReason { get; set; }

        public bool IsCustomerMailSend { get; set; }
        public decimal? EstimateWorth { get; set; }
        public long? InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        public bool IsJobConverted { get; set; }

        public bool IsCustomerAvailable { get; set; }

        [NotMapped]
        public DateTime ActualStartDate
        {
            get
            {
                var a= StartDate.AddMinutes(Offset.GetValueOrDefault());
                return a;
            }
        }

        [NotMapped]
        public DateTime ActualEndDate
        {
            get
            {
              var a=  EndDate.AddMinutes(Offset.GetValueOrDefault());
                return a;
            }
        }

        [NotMapped]
        public DateTime ActualStartDateInLocal
        {
            get
            {
                var a = StartDateTimeString.Date;
                return a;
            }
        }

        [NotMapped]
        public DateTime ActualEndDateInLocal
        {
            get
            {
                var a = EndDateTimeString.Date;
                return a;
            }
        }

    }
}
