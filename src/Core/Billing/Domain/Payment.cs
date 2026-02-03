using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class Payment : DomainBase
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public long InstrumentTypeId { get; set; }
        public long CurrencyExchangeRateId { get; set; }

        [ForeignKey("CurrencyExchangeRateId")]
        public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }

        public virtual ICollection<InvoicePayment> InvoicePayments { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<PaymentItem> PaymentItems { get; set; }
        public virtual ChargeCardPayment ChargeCardPayment { get; set; }
        public virtual ECheckPayment ECheckPayment { get; set; }
        public virtual CheckPayment CheckPayment { get; set; }
    }
}
