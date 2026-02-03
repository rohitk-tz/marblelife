using Core.Billing.Domain;
using Core.Billing.ViewModel;

namespace Core.Billing
{
    public interface IECheckService
    {
        ECheck Save(ECheckEditModel model);
    }
}
