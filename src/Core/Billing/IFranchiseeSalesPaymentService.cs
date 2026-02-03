using Core.Billing.Domain;
using Core.Billing.ViewModel;
using System.Collections.Generic;

namespace Core.Billing
{
    public interface IFranchiseeSalesPaymentService
    {

        Payment Save(FranchiseeSalesPaymentEditModel model);
        string GetPaymentInstrument(ICollection<InvoicePayment> list);
        ICollection<FranchiseeSalesPaymentEditModel> CreatePaymentModelCollection(ICollection<InvoicePayment> list);
        PaymentModeDetailViewModel GetCheckDetails(ICollection<InvoicePayment> list);
        decimal GetAccountCreditAmount(ICollection<InvoicePayment> list);
    }
}
