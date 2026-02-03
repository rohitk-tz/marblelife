using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class Check : DomainBase
    {
        public string CheckNumber { get; set; }

        public long? AccountTypeId { get; set; }

        public string Name { get; set; }

        public string AccountNumber { get; set; }

        [ForeignKey("AccountTypeId")]
        public virtual Lookup Lookup { get; set; }
    }
}
