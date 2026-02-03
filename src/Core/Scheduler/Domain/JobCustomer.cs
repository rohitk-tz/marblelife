using Core.Application.Attribute;
using Core.Geo.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class JobCustomer : DomainBase
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public long? AddressId { get; set; }

        [ForeignKey("AddressId")]
        [CascadeEntity]
        public virtual Address Address { get; set; }
        public string CustomerAddress { get; set; } 
    }
}
