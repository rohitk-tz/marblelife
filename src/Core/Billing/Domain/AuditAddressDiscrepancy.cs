using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Billing.Domain
{
    public class AuditAddressDiscrepancy : DomainBase
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public long? FranchiseeSalesId { get; set; }
        public long? InvoiceId { get; set; }
        public DateTime? invoiceDate { get; set; }

        public bool? isUpdated { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public long? annualSalesdatauploadId { get; set; }

        public long? MarketingClassId { get; set; }
        [ForeignKey("FranchiseeSalesId")]
        public virtual FranchiseeSales FranchiseeSales { get; set; }

        [ForeignKey("annualSalesdatauploadId")]
        public virtual AnnualSalesDataUpload AnnualSalesDataUpload { get; set; }

        [ForeignKey("MarketingClassId")]
        public virtual MarketingClass MarketingClass { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

    }
}
