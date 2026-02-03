using System;
using Core.Application.Attribute;
using Core.Billing.Domain;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class FranchiseeInvoiceFactory : IFranchiseeInvoiceFactory
    {
        public FranchiseeInvoice CreateDomain(long franchiseeId, long invoiceId, long? salesDataUploadId, DateTime? endDate)
        {
            return new FranchiseeInvoice()
            {
                FranchiseeId = franchiseeId,
                InvoiceId = invoiceId,
                SalesDataUploadId = salesDataUploadId,
                InvoiceDate = endDate,
                IsDownloaded = false,
                IsNew = true
            };
        }
    }
}
