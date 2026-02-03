using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FeeProfileFactory : IFeeProfileFactory
    {
        private readonly IRoyaltyFeeSlabsFactory _royaltyFeeSlabsFactory;

        public FeeProfileFactory(IRoyaltyFeeSlabsFactory royaltyFeeSlabsFactory)
        {
            _royaltyFeeSlabsFactory = royaltyFeeSlabsFactory;
        }

        public FeeProfile CreateDomain(FeeProfileEditModel model, long franchiseeId, FeeProfile inDb)
        {
            var domain = inDb != null ? inDb : new FeeProfile()
            {
                Id = franchiseeId
            };

            domain.AdFundPercentage = model.AdFundPercentage;           
            domain.MinimumRoyaltyPerMonth = model.MinimumRoyaltyPerMonth;
            domain.PaymentFrequencyId = (model.PaymentFrequencyId != null && model.PaymentFrequencyId > 0) ? model.PaymentFrequencyId : null;
            domain.SalesBasedRoyalty = model.SalesBasedRoyalty;
            domain.FixedAmount = model.FixedAmount;

            domain.RoyaltyFeeSlabs = _royaltyFeeSlabsFactory.CreateDomainCollection(model.Slabs, domain).ToList();
            return domain;
        }

        public FeeProfileEditModel CreateEditModel(FeeProfile domain, List<MinRoyaltyFeeSlabs> minRoyalitySlabs)
        {
            if (domain == null)
            {
                return new FeeProfileEditModel();
            }

            return new FeeProfileEditModel
            {
                FixedAmount = domain.FixedAmount,
                AdFundPercentage = domain.AdFundPercentage,               
                MinimumRoyaltyPerMonth = domain.MinimumRoyaltyPerMonth,
                PaymentFrequencyId = domain.PaymentFrequencyId,
                SalesBasedRoyalty = domain.SalesBasedRoyalty,
                Slabs = _royaltyFeeSlabsFactory.CreateModelCollection(domain.RoyaltyFeeSlabs),
                MinRoyalitySlabs= _royaltyFeeSlabsFactory.CreateModelForMinRoyalityCollection(minRoyalitySlabs),
            };
        }

        public FeeProfileViewModel CreateViewModel(FeeProfile domain)
        {
            if(domain==null)
            {
                var model2 = new FeeProfileViewModel { };
                return model2;
            }
            var model = new FeeProfileViewModel
            {
                AdFundPercentage = domain.AdFundPercentage,
                FixedAmount = domain.FixedAmount,
                MinimumRoyaltyPerMonth = domain.MinimumRoyaltyPerMonth,
                PaymentFrequencyId = domain.PaymentFrequencyId,
                SalesBasedRoyalty = domain.SalesBasedRoyalty
            };
            return model;
        }
    }
}
