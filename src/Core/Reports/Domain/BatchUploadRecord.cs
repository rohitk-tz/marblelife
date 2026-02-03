using Core.Application.Domain;
using Core.Organizations.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Reports.Domain
{
    public class BatchUploadRecord : DomainBase
    {
        public long FranchiseeId { get; set; }
        public long? PaymentFrequencyId { get; set; }
        public int WaitPeriod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExpectedUploadDate { get; set; }
        public DateTime? UploadedOn { get; set; }
        public bool IsCorrectUploaded { get; set; } 

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        [ForeignKey("PaymentFrequencyId")]
        public virtual Lookup PaymentFrequency { get; set; } 
    }
}
