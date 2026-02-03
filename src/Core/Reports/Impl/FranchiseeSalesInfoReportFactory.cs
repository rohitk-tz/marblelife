using Core.Application;
using Core.Application.Attribute;
using Core.Reports.Domain;
using Core.Reports.ViewModel;
using Core.Users.Enum;
using System;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class FranchiseeSalesInfoReportFactory : IFranchiseeSalesInfoReportFactory
    {
        private readonly ISessionContext _sessionContext;
        private readonly IClock _clock;
        public FranchiseeSalesInfoReportFactory(ISessionContext sessionContext, IClock clock)
        {
            _sessionContext = sessionContext;
            _clock = clock;
        }
        public FranchiseeSalesInfo CreateDomain(long franchiseeId, int month, int year, decimal amount, decimal amountInLocalCurrency, DateTime currentDate,
            long classTypeId, long serviceTypeId, FranchiseeSalesInfo Info = null)
        {
            var domain = new FranchiseeSalesInfo
            {
                Month = month,
                Year = year,
                SalesAmount = amount,
                FranchiseeId = franchiseeId,
                UpdatedDate = currentDate,
                AmountInLocalCurrency = amountInLocalCurrency,
                ClassTypeId = classTypeId,
                ServiceTypeId = serviceTypeId,
                IsNew = Info == null ? true : Info.Id <= 0
            };
            if (Info != null)
                domain.Id = Info.Id;
            return domain;
        }

        public GrowthReportViewModel CreateViewModel(FranchiseeSalesInfoList infoList, GrowthReportFilter filter)
        {
            const int defaultMonth = 12;
            if (filter.Month <= 0 && filter.Year != _clock.UtcNow.Year)
                filter.Month = defaultMonth;
            else if (filter.Month <= 0)
                filter.Month = _clock.UtcNow.Month - 1;
            var className = filter.ClassTypeId > 0 ? (infoList.FranchiseeSalesInfo.FirstOrDefault() != null ? infoList.FranchiseeSalesInfo.FirstOrDefault().MarketingClass.Name : null) : null;
            var service = filter.ServiceTypeId > 0 ? (infoList.FranchiseeSalesInfo.FirstOrDefault() != null ? infoList.FranchiseeSalesInfo.FirstOrDefault().Service.Name : null) : null;

            var totalSalesLastYear = infoList.FranchiseeSalesInfo.Where(x => x.Year == filter.Year - 1).Sum(y => y.SalesAmount);
            var totalsSalesYtdCurrent = infoList.FranchiseeSalesInfo.Where(x => x.Year == filter.Year && x.Month <= filter.Month).Sum(y => y.SalesAmount);
            var totalSalesYTDLast = infoList.FranchiseeSalesInfo.Where(x => x.Month <= filter.Month && x.Year == filter.Year - 1).Sum(y => y.SalesAmount);

            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                totalSalesLastYear = infoList.FranchiseeSalesInfo.Where(x => x.Year == filter.Year - 1).Sum(y => y.AmountInLocalCurrency);
                totalsSalesYtdCurrent = infoList.FranchiseeSalesInfo.Where(x => x.Year == filter.Year && x.Month <= filter.Month).Sum(y => y.AmountInLocalCurrency);
                totalSalesYTDLast = infoList.FranchiseeSalesInfo.Where(x => x.Month == filter.Month && x.Year == filter.Year - 1).Sum(y => y.AmountInLocalCurrency);
            }

            var percentageGrowth = totalSalesYTDLast > 0 ?
                                     Math.Round(((totalsSalesYtdCurrent - totalSalesYTDLast) * 100) / totalSalesYTDLast, 2) : 0;

            if (totalSalesYTDLast == 0 && totalsSalesYtdCurrent > 0)
                percentageGrowth = 100;
            else if (totalSalesYTDLast > 0 && totalsSalesYtdCurrent < 0)
                percentageGrowth = -100;

            var model = new GrowthReportViewModel
            {
                TotalSalesLastYear = Math.Round(totalSalesLastYear, 2),
                YTDSalesCurrentYear = Math.Round(totalsSalesYtdCurrent, 2),
                YTDSalesLastYear = Math.Round(totalSalesYTDLast, 2),
                AmountDifference = Math.Round(totalsSalesYtdCurrent - totalSalesYTDLast, 2),
                AmountToDisplay = Math.Round(Math.Abs(totalsSalesYtdCurrent - totalSalesYTDLast), 2),
                FranchiseeId = infoList.FranchiseeId,
                Franchisee = infoList.Franchisee,
                PercentageDifference = Math.Round(percentageGrowth, 2),
                LastYearAveragePerMonth = totalSalesYTDLast > 0 ? Math.Round(totalSalesYTDLast / filter.Month, 2) : 0,
                CurrentYearAveragePerMonth = totalsSalesYtdCurrent > 0 ? Math.Round(totalsSalesYtdCurrent / filter.Month, 2) : 0,
                Class = className,
                Service = service,
            };
            model.AverageGrowth = model.CurrentYearAveragePerMonth - model.LastYearAveragePerMonth;
            model.AverageGrowthToDisplay = Math.Abs(model.AverageGrowth);
            return model;
        }
    }
}
