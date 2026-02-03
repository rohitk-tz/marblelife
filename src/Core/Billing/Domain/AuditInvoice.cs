using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class AuditInvoice : DomainBase
    {
        public long? InvoiceId { get; set; }
        public long AnnualUploadId { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string QBInvoiceNumber { get; set; }

        public string CustomerQbInvoiceId { get; set; }
        public string QBInvoiceNumbers { get; set; }
        //public decimal WeeklyRoyality { get; set; }

        //public decimal AnnualRoyality { get; set; }
        public long StatusId { get; set; }
        public long? ReportTypeId { get; set; }
        public bool isActive { get; set; }


        public virtual ICollection<AuditInvoicePayment> AuditInvoicePayments { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<AuditInvoiceItem> AuditInvoiceItems { get; set; }

        [ForeignKey("StatusId")]
        public virtual Lookup Lookup { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [ForeignKey("AnnualUploadId")]
        public virtual AnnualSalesDataUpload AnnualSalesDataUpload { get; set; }

        
        [ForeignKey("ReportTypeId")]
        public virtual AnnualReportType AnnualReportType { get; set; }




    }
}
