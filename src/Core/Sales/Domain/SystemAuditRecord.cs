using Core.Billing.Domain;
using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class SystemAuditRecord : DomainBase
    {
        public long InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        public long FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long AnnualUploadId { get; set; }
        [ForeignKey("AnnualUploadId")]
        public virtual AnnualSalesDataUpload AnnualSalesDataUpload { get; set; } 

        public string QBIdentifier { get; set; }

        public long? AnnualReportTypeId { get; set; }
        [ForeignKey("AnnualReportTypeId")]
        public virtual AnnualReportType AnnualReportType { get; set; }
    }
}
