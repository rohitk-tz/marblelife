using Core.Organizations;
using Core.Users.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Geo.Domain
{
    public class Address : DomainBase
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long TypeId { get; set; }
        public long? CityId { get; set; }
        public long? ZipId { get; set; }
        public long? StateId { get; set; }
        public long CountryId { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string ZipCode { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        [ForeignKey("ZipId")]
        public virtual Zip Zip { get; set; }

        [ForeignKey("StateId")]
        public virtual State State { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }

        public virtual ICollection<Person> Persons { get; set; }
        public virtual ICollection<Organization> Organizations { get; set; }

        public Address(string line1, string line2, State state, City city, Zip zip, long stateId, long countryId, string stateName, string cityName, string zipCode)
        {
            AddressLine1 = line1;
            AddressLine2 = line2;
            CityId = city != null ? city.Id : (long?)null;
            StateId = state != null ? state.Id : (long?)null;
            ZipId = zip != null ? zip.Id : (long?)null;
            CountryId = countryId;
            StateName = state != null ? state.Name : stateName;
            CityName = city != null ? city.Name : cityName;
            ZipCode = zip != null ? zip.Code : zipCode;
        }

        public Address()
        {
            Organizations = new List<Organization>();
        }
    }
}
