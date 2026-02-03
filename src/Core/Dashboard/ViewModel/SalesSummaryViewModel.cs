using Core.Application.Attribute;
using System;

namespace Core.Dashboard.ViewModel
{
    [NoValidatorRequired]
    public class SalesSummaryViewModel
    {
        public string Franchise { get; set; }
        public decimal? TotalSales { get; set; }
        public decimal? Received { get; set; }
        public decimal? Accrued { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public long FileId { get; set; }
        public int? TotalCustomers { get; set; }
        public int? TotalInvoices { get; set; }
        public decimal CurrencyRate { get; set; }
        public string CurrencyCode { get; set; }
    }
}
