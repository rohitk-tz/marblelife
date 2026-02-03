using Core.Billing.Domain;
using Core.Organizations.Domain;

namespace Core.Billing
{
    public interface IChargeCardPaymentProfileFactory
    {
        FranchiseePaymentProfile CreateDoamin(long accountTypeId, string cmId, Franchisee franchisee);
        PaymentInstrument CreateDoamin(long franchiseePaymentProfileId, long instrumentTypeId, string authNetPaymentRefId, long chargeCardId);
    }
}
