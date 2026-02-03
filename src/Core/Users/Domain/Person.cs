using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.ValueType;
using Core.Geo.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Users.Domain
{
    public class Person : DomainBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public long? FileId { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }
        public bool IsRecruitmentFeeApplicable { get; set; } 

        public virtual UserLogin UserLogin { get; set; }


        public string CalendarPreference { get; set; }
        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<Address> Addresses { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<Phone> Phones { get; set; }

        [NotMapped]
        private Name name { get; set; }

        [NotMapped]
        public Name Name
        {
            get { return new Name(FirstName, MiddleName, LastName); }
            set { name = value; }

        }


        [NotMapped]
        public string FullNameUser
        {
            get {
                var a = new Name(FirstName, MiddleName, LastName);
                return a.FullName;
            }
        }


        public Person()
        {
            Addresses = new Collection<Address>();
        }

    }
}
