using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class Job : DomainBase
    {
        public long JobTypeId { get; set; }
        [ForeignKey("JobTypeId")]
        public virtual MarketingClass JobType { get; set; } 

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string QBInvoiceNumber { get; set; }
        public string Description { get; set; }

        public long StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual JobStatus JobStatus { get; set; }

        public long CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual JobCustomer JobCustomer { get; set; }

        public virtual ICollection<JobScheduler> JobScheduler { get; set; }

        public long? EstimateId { get; set; }
        public double? Offset { get; set; }
        public DateTime StartDateTimeString { get; set; }
        public DateTime EndDateTimeString { get; set; }
        [ForeignKey("EstimateId")]
        public virtual JobEstimate JobEstimate { get; set; }

        public virtual ICollection<JobNote> JobNote { get; set; }

        public string GeoCode { get; set; } 
        public Job()
        {
            JobNote = new Collection<JobNote>();
        }
    }
}
