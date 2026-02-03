using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class CheckFactory : ICheckFactory
    {
        public Check CreateDomain(CheckPaymentEditModel model)
        {
            var check = new Check
            {
                AccountNumber = model.AccountNumber,
                CheckNumber = model.CheckNumber,
                Name = model.Name,
                AccountTypeId = model.AccountTypeId!=0?model.AccountTypeId:(long?)null,
                IsNew = true
            };
            return check;
        }

        public CheckPayment CreateCheckPayment(Check check, Payment payment)
        {
            var checkPayment = new CheckPayment
            {
                Id = payment.Id,
                CheckId = check.Id,
                Amount = payment.Amount,
                IsNew = true
            };
            return checkPayment;
        }
    }
}
