using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Users.Domain
{
    public class OrganizationRoleUserFranchisee : DomainBase
    {
        public long OrganizationRoleUserId { get; set; }
        public long FranchiseeId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }

        [ForeignKey("OrganizationRoleUserId")]
        public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

    }
}
