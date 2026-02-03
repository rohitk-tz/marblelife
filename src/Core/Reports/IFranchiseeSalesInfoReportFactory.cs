using Core.Reports.Domain;
using Core.Reports.ViewModel;
using System;

namespace Core.Reports
{
    public interface IFranchiseeSalesInfoReportFactory
    {
        FranchiseeSalesInfo CreateDomain(long franchiseeId, int month, int year, decimal amount, decimal amountInLocalCurrency, DateTime currentDate,
            long classTypeId, long serviceTypeId, FranchiseeSalesInfo Info = null);
        GrowthReportViewModel CreateViewModel(FranchiseeSalesInfoList infoList, GrowthReportFilter filter);
    }
}

