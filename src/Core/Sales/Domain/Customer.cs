using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Geo.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class Customer : DomainBase
    {
        public string Name { get; set; }

        public string ContactPerson { get; set; }

        public long? AddressId { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<CustomerEmail> CustomerEmails { get; set; }

        public string Phone { get; set; }

        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("AddressId")]
        [CascadeEntity]
        public virtual Address Address { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public long ClassTypeId { get; set; }

        [ForeignKey("ClassTypeId")]
        public virtual MarketingClass MarketingClass { get; set; }

        public bool ReceiveNotification { get; set; }
        public DateTime? DateCreated { get; set; }
        public decimal? TotalSales { get; set; }
        public int? NoOfSales { get; set; }
        public decimal? AvgSales { get; set; } 

    }
}
