using Core.Billing.Domain;
using System;

namespace Core.Billing
{
    public interface IInvoiceLateFeePollingAgent
    {
        void LateFeeGenerator();
        void SaveRoyalityLateFee(FranchiseeInvoice item, DateTime currenctdate);
    }
}
