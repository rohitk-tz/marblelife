using Core.Application.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FranchiseeServiceFee : DomainBase
    {
        public long FranchiseeId { get; set; }
        public long ServiceFeeTypeId { get; set; }
        public decimal Amount { get; set; }
        public decimal? Percentage { get; set; }
        public bool IsActive { get; set; }

        public long? FrequencyId { get; set; }
        [ForeignKey("FrequencyId")]
        public virtual Lookup Frequency { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        [ForeignKey("ServiceFeeTypeId")]
        public virtual Lookup ServiceFeeType { get; set; }

        public DateTime? SaveDateForSeoCost { get; set; }
        public DateTime? InvoiceDateForSeoCost { get; set; }

    }
}
