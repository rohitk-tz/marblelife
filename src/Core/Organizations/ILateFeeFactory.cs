using Core.Organizations.Domain;
using Core.Organizations.ViewModel;

namespace Core.Organizations
{
    public interface ILateFeeFactory
    {
        LateFee CreateDomain(LateFeeEditModel model, long franchiseeId, LateFee inDb);
        LateFeeEditModel CreateEditModel(LateFee domain);
    }
}
