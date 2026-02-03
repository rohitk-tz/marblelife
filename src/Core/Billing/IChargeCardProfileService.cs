using Core.Billing.Domain;
using Core.Billing.ViewModel;
using System.Collections.Generic;

namespace Core.Billing
{
    public interface IChargeCardProfileService
    {
        List<PaymentInstrumentViewModel> GetInstrumentList(long franchiseeId, long paymentTypeId);
        ProcessorResponse CreateProfile(ChargeCardEditModel model, long franchiseeId);
        ProcessorResponse CreateAuthorizeNetProfile(long accountTypeId, ChargeCardEditModel model, ProcessorResponse processorResponse, long franchiseeId);
        bool ManageInstrument(string instrumentId, bool isActive);
        bool DeleteInstrument(string instrumentId);
        ProcessorResponse SetPrimary(long franchiseeId, string paymentInstrumentId);
        bool CheckExpiryDate(int expireMonth, int expireYear);
        List<FranchiseePaymentInstrumentViewModel> GetFranchiseeInstrumentList(long franchiseeId);
    }
}
