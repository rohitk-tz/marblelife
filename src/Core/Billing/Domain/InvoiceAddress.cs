using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Geo.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
   public class InvoiceAddress : DomainBase
    {
        public long? InvoiceId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public long TypeId { get; set; }
        public long? CityId { get; set; }
        public long? ZipId { get; set; }
        public long? StateId { get; set; }
        public long? CountryId { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }

        public string EmailId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        [ForeignKey("ZipId")]
        public virtual Zip Zip { get; set; }

        [ForeignKey("StateId")]
        public virtual State State { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
    }
}
