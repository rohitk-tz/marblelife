using Core.Organizations.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users.Domain
{
    public class EmailSignatures : DomainBase
    {
        public long? UserId { get; set; }
        public long? OrganizationRoleUserId { get; set; }
        public string SignatureName { get; set; }
        public string Signature { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }

        [ForeignKey("OrganizationRoleUserId")]
        public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }
    }
}
