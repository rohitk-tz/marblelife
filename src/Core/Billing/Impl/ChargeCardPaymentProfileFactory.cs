using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using System;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class ChargeCardPaymentProfileFactory : IChargeCardPaymentProfileFactory
    {
        public FranchiseePaymentProfile CreateDoamin(long accountTypeId,string cmId, Franchisee franchisee)
        {
            var franchiseePaymentProfile = new FranchiseePaymentProfile
            {
                Franchisee = franchisee,
                FranchiseeId = franchisee.Id,
                CMID = cmId,
                IsActive = true,
                DataRecorderMetaData = new DataRecorderMetaData(DateTime.UtcNow),
                IsNew = true,
                ProfileTypeId=accountTypeId
            };
            return franchiseePaymentProfile;
        }

        public PaymentInstrument CreateDoamin(long franchiseePaymentProfileId, long instrumentTypeId, string authNetPaymentRefId, long chargeCardId)
        {
            var paymentInstrument = new PaymentInstrument
            {
                FranchiseePaymentProfileId = franchiseePaymentProfileId,
                IsPrimary = false,
                IsActive = true,
                InstrumentEntityId = chargeCardId,
                InstrumentTypeId = instrumentTypeId,
                InstrumentProfileId = authNetPaymentRefId,
                DataRecorderMetaData = new DataRecorderMetaData(DateTime.UtcNow),
                IsNew = true
            };
            return paymentInstrument;
        }
    }
}
