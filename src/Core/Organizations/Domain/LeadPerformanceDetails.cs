using Core.Application.Attribute;
using Core.Application.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class LeadPerformanceFranchiseeDetails : DomainBase
    {
        public LeadPerformanceFranchiseeDetails()
        {
            Year = DateTime.Year;
        }
        public long FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public double Amount { get; set; }
        public int Month { get; set; }
        public DateTime DateTime { get; set; }
        public long? CategoryId { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Lookup Category { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public int Year { get; set; }
        public bool IsSEOActive { get; set; }
        public long? week { get; set; }
    }
}
