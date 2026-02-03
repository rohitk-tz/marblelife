using Core.Application.Attribute;
using Core.Organizations.ViewModel;
using System.Collections.Generic;

namespace Core.Organizations.ViewModels
{
    [NoValidatorRequired]
    public class FeeProfileEditModel
    {
        public long FranchiseeId { get; set; }
        public long? PaymentFrequencyId { get; set; }
        public decimal MinimumRoyaltyPerMonth { get; set; }
        public bool SalesBasedRoyalty { get; set; }
        public decimal AdFundPercentage { get; set; }       
        public decimal? FixedAmount { get; set; }
        public IEnumerable<RoyaltyFeeSlabsEditModel> Slabs { get; set; }
        public IEnumerable<MinRoyaltyFeeSlabsEditModel> MinRoyalitySlabs { get; set; }
        public FeeProfileEditModel()
        {
            Slabs = new List<RoyaltyFeeSlabsEditModel>();
            SalesBasedRoyalty = true;
            MinRoyalitySlabs = new List<MinRoyaltyFeeSlabsEditModel>();
        }
    }
}
