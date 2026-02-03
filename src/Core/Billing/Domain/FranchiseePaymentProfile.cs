using Core.Application.Domain;
using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class FranchiseePaymentProfile : DomainBase
    {
        public string CMID { get; set; }

        public bool IsActive { get; set; }
        public long FranchiseeId { get; set; }

        public long DataRecorderMetaDataId { get; set; }
    
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long ProfileTypeId { get; set; }
        [ForeignKey("ProfileTypeId")]
        public virtual Lookup ProfileType { get; set; }
    }
}
