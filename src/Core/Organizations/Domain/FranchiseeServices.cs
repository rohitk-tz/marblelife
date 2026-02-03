using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FranchiseeService : DomainBase
    {
        public long FranchiseeId { get; set; }
        public long ServiceTypeId { get; set; }
        public bool CalculateRoyalty { get; set; }
        public bool IsActive { get; set; }
        public bool IsCertified { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }
    }
}
