using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Geo.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Application.Attribute;
using Core.Users.Domain;

namespace Core.Organizations
{
    public class Organization : DomainBase
    {
        public string Name { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        public long TypeId { get; set; }

        public string About { get; set; }

        public string Email { get; set; }
        public string DeactivationnNote { get; set; }

        [ForeignKey("TypeId")]
        public virtual Lookup Type { get; set; }

        [CascadeEntity]
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<Address> Address { get; set; }


        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<Phone> Phones { get; set; }

        [CascadeEntity]
        public virtual Franchisee Franchisee { get; set; }

        public bool IsActive { get; set; }


       

        public Organization()
        {
            Address = new List<Address>();
        }
    }
}
