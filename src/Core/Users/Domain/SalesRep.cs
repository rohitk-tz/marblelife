using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Users.Domain
{
    public class SalesRep : DomainBase
    {
        [Key, ForeignKey("OrganizationRoleUser")]
        public override long Id { get; set; }
        public string Alias { get; set; }
        public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }

    }
}
