using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Geo.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class CustomerJobEstimate : DomainBase
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public long? AddressId { get; set; }

        [ForeignKey("AddressId")]
        [CascadeEntity]
        public virtual Address Addresses { get; set; }
        public long? JobCustomerId { get; set; }
        [ForeignKey("JobCustomerId")]
        public virtual JobCustomer JobCustomer { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
        public string Address { get; set; }
        public long CustomerFrom { get; set; }
        public bool IsActive { get; set; }
    }
}