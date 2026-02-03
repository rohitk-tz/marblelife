using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class JobEstimate : DomainBase
    {
      
        public int EstimateHour { get; set; }

        public long CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual JobCustomer JobCustomer { get; set; }


        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
        public decimal Amount { get; set; }
        public virtual ICollection<JobNote> JobNote { get; set; }

        public string GeoCode { get; set; }
        public long? TypeId { get; set; }
        [ForeignKey("TypeId")]
        public virtual MarketingClass MarketingClass { get; set; }
        public double? Offset { get; set; }
        public DateTime StartDateTimeString { get; set; }
        public DateTime EndDateTimeString { get; set; }
        public long? ParentEstimateId { get; set; }
        [ForeignKey("ParentEstimateId")]
        public virtual JobEstimate JobEstimates { get; set; }

        public JobEstimate()
        {
            JobNote = new Collection<JobNote>();
            Jobs = new Collection<Job>();
        }
    }
}
