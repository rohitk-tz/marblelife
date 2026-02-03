using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class LateFee : DomainBase
    {
        [ForeignKey("Franchisee")]
        public override long Id { get; set; }

        public decimal RoyalityLateFee { get; set; }
        public int RoyalityWaitPeriodInDays { get; set; }
        public decimal RoyalityInterestRatePercentagePerAnnum { get; set; }

        public decimal SalesDataLateFee { get; set; }
        public int SalesDataWaitPeriodInDays { get; set; }
        public virtual Franchisee Franchisee { get; set; }
        public LateFee()
        {
            RoyalityLateFee = 50;
            RoyalityWaitPeriodInDays = 2;
            RoyalityInterestRatePercentagePerAnnum = 18;
            SalesDataLateFee = 50;
            SalesDataWaitPeriodInDays = 1;

        }
    }
}