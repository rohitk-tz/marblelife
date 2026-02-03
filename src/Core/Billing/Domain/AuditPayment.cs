using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Billing.Domain
{
    public class AuditPayment : DomainBase
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public long InstrumentTypeId { get; set; }

        public virtual ICollection<AuditInvoicePayment> AuditInvoicePayments { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<AuditPaymentItem> AuditPaymentItems { get; set; }

    }
}
