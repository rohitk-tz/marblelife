using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using System.Collections.Generic;

namespace Core.Organizations
{
    public interface IFeeProfileFactory
    {
        FeeProfileEditModel CreateEditModel(FeeProfile domain,List<MinRoyaltyFeeSlabs> minRoyalitySlabs);
        FeeProfile CreateDomain(FeeProfileEditModel model, long franchiseeId, FeeProfile inDb);
        FeeProfileViewModel CreateViewModel(FeeProfile domain);
    }
}
