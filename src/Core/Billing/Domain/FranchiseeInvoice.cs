using Core.Notification.ViewModel;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class FranchiseeInvoice : DomainBase
    {
        public long FranchiseeId { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public long InvoiceId { get; set; }

        public long? SalesDataUploadId { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        public bool IsDownloaded { get; set; }

        [ForeignKey("SalesDataUploadId")]
        public virtual SalesDataUpload SalesDataUpload { get; set; }


        
    }
}
