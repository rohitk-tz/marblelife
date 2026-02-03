using Core.Billing.ViewModel;

using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    public class AuditInvoiceViewModel
    {
        public long AnnualUploadId { get; set; } 
        public InvoiceDetailsViewModel AuditInvoice { get; set; }
        public InvoiceDetailsViewModel SystemInvoice { get; set; }
        public IEnumerable<InvoiceDetailsViewModel> AuditInvoices { get; set; }
    }
}
