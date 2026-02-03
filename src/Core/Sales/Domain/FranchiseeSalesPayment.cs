using Core.Billing.Domain;
using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class FranchiseeSalesPayment : DomainBase
    {
        public long FranchiseeSalesId { get; set; }
        public long InvoiceId { get; set; }
        public long PaymentId { get; set; }

        [ForeignKey("FranchiseeSalesId")]
        public virtual FranchiseeSales FranchiseeSales { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

        public long SalesDataUploadId { get; set; }

        [ForeignKey("SalesDataUploadId")]
        public virtual SalesDataUpload SalesDataUpload { get; set; }
    }
}
