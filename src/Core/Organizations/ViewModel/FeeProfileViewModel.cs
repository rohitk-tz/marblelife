namespace Core.Organizations.ViewModel
{
    public class FeeProfileViewModel
    {
        public long FranchiseeId { get; set; }
        public long? PaymentFrequencyId { get; set; }
        public decimal MinimumRoyaltyPerMonth { get; set; }
        public bool SalesBasedRoyalty { get; set; }
        public decimal AdFundPercentage { get; set; }
        public decimal? FixedAmount { get; set; }
    }
}
