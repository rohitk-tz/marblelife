using Core.Application.Attribute;

namespace Core.Organizations.ViewModels
{
    [NoValidatorRequired]
    public class RoyaltyFeeSlabsEditModel
    {
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal ChargePercentage { get; set; }

    }
}
