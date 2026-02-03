using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using System.Collections.Generic;

namespace Core.Organizations
{
    public interface IRoyaltyFeeSlabsFactory
    {
        RoyaltyFeeSlabsEditModel CreateEditModel(RoyaltyFeeSlabs domain);
        IEnumerable<RoyaltyFeeSlabsEditModel> CreateModelCollection(IEnumerable<RoyaltyFeeSlabs> domainCollection);
        IEnumerable<RoyaltyFeeSlabs> CreateDomainCollection(IEnumerable<RoyaltyFeeSlabsEditModel> modelCollection, FeeProfile feeProfile);
        MinRoyaltyFeeSlabsEditModel CreateEditModelForMinRoyality(MinRoyaltyFeeSlabs domain);
        IEnumerable<MinRoyaltyFeeSlabsEditModel> CreateModelForMinRoyalityCollection(IEnumerable<MinRoyaltyFeeSlabs> domainCollection);
    }
}
