using Core.Application.Attribute;
using Core.Users.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class OrganizationRoleUser : DomainBase
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public long OrganizationId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        public string ColorCode { get; set; }
        public string ColorCodeSale { get; set; }

        public virtual ICollection<OrganizationRoleUserFranchisee> OrganizationRoleUserFranchisee { get; set; }

        public OrganizationRoleUser()
        {
            IsActive = true;
        }
    }
}
