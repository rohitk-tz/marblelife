using Core.Billing.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class AccountCreditItem : DomainBase
    {
        public long AccountCreditId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public long CurrencyExchangeRateId { get; set; }

        [ForeignKey("CurrencyExchangeRateId")]
        public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    }
}
