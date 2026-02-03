using Core.Application.Attribute;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class LateFeeEditModel
    {
        public long FranchiseeId { get; set; }
        public decimal RoyalityLateFee { get; set; }
        public int RoyalityWaitPeriodInDays { get; set; }
        public decimal RoyalityInterestRate { get; set; }

        public decimal SalesDataLateFee { get; set; }
        public int SalesDataWaitPeriodInDays { get; set; }
        public bool IsRoyalityLateFeeApplicable { get; set; }
        public bool IsSalesDateLateFeeApplicable { get; set; }
        public LateFeeEditModel()
        {
            RoyalityLateFee = 50;
            RoyalityWaitPeriodInDays = 3;
            RoyalityInterestRate = 18;
            SalesDataLateFee = 50;
            SalesDataWaitPeriodInDays = 3;
            IsRoyalityLateFeeApplicable = true;
            IsSalesDateLateFeeApplicable = true;
        }
    }
}
