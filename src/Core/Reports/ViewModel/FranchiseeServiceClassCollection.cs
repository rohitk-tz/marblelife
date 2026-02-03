using Core.Billing.Domain;
using Core.Organizations.Domain;

namespace Core.Reports.ViewModel
{
    public class FranchiseeServiceClassCollection
    {
        public Franchisee Franchisee { get; set; }
        public string MarketingClass { get; set; }
        public string ServiceType { get; set; }
        public long FranchiseeId { get; set; }
        public long ServiceTypeId { get; set; }
        public long ClassTypeId { get; set; }
        public decimal TotalSales { get; set; }
        public PaymentItem PaymentItem { get; set; }

    }
}
