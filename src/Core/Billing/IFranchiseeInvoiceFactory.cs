using Core.Billing.Domain;
using System;

namespace Core.Billing
{
    public interface IFranchiseeInvoiceFactory
    {
        FranchiseeInvoice CreateDomain(long franchiseeId, long invoiceId, long? salesDataUploadId, DateTime? endDate);
    }
}
