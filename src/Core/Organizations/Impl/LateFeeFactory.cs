using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class LateFeeFactory : ILateFeeFactory
    {
        public LateFee CreateDomain(LateFeeEditModel model, long franchiseeId, LateFee inDb)
        {
            var domain = inDb != null ? inDb : new LateFee()
            {
                Id = franchiseeId
            };
            domain.RoyalityLateFee = model.RoyalityLateFee;
            domain.RoyalityWaitPeriodInDays = model.RoyalityWaitPeriodInDays;
            domain.RoyalityInterestRatePercentagePerAnnum = model.RoyalityInterestRate;
            domain.SalesDataLateFee = model.SalesDataLateFee;
            domain.SalesDataWaitPeriodInDays = model.SalesDataWaitPeriodInDays;
            return domain;
        }

        public LateFeeEditModel CreateEditModel(LateFee domain)
        {
            return new LateFeeEditModel
            {
                FranchiseeId = domain.Id,
                RoyalityLateFee = domain.RoyalityLateFee,
                RoyalityWaitPeriodInDays = domain.RoyalityWaitPeriodInDays,
                RoyalityInterestRate = domain.RoyalityInterestRatePercentagePerAnnum,
                IsRoyalityLateFeeApplicable = domain.Id > 0 && domain.RoyalityLateFee > 0 ? true : false,
                SalesDataLateFee = domain.SalesDataLateFee,
                SalesDataWaitPeriodInDays = domain.SalesDataWaitPeriodInDays,
                IsSalesDateLateFeeApplicable = domain.Id > 0 && domain.SalesDataLateFee > 0 ? true : false
            };
        }
    }
}
