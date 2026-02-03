using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface ICheckService
    {
        ProcessorResponse SaveCheck(CheckPaymentEditModel model, long franchiseeId, long invoiceId);
    }
}
