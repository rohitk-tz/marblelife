using Core.Geo.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class CurrencyExchangeRate : DomainBase
    {
        public long CountryId  { get; set; }

        public decimal Rate { get; set; }
        public DateTime DateTime { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
    }
}
