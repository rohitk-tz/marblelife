using Core.Application.Attribute;
using Core.Application.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class Invoice : DomainBase
    {
        public DateTime GeneratedOn { get; set; }
        public DateTime DueDate { get; set; }

        public long StatusId { get; set; }

        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public virtual ICollection<InvoicePayment> InvoicePayments { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }

        [ForeignKey("StatusId")]
        public virtual Lookup Lookup { get; set; }

        public virtual ICollection<PaymentMailReminder> PaymentMailReminders { get; set; }
        public long CustomerInvoiceId { get; set; }
        public string CustomerInvoiceIdString { get; set; }

        public long CustomerQbInvoiceId { get; set; }
        public string CustomerQbInvoiceIdString { get; set; }
        public string ReconciliationNotes { get; set; }

    }
}
