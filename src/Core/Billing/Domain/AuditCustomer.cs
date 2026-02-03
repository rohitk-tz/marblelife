using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Geo.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class AuditCustomer : DomainBase
    {
        public string Name { get; set; }

        public string ContactPerson { get; set; }

        public long? AuditAddressId { get; set; }


        public string Phone { get; set; }


        [ForeignKey("AuditAddressId")]
        [CascadeEntity]
        public virtual AuditAddress AuditAddress { get; set; }


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
