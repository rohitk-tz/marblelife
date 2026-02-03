using Core.Application.Attribute;
using System;


namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class AnnualSalesDataListFiltercs
    {
        public string Text { get; set; }
        public long SalesDataUploadId { get; set; }
        public long AnnualDataUploadId { get; set; }
        public string CustomerName { get; set; }
        public string QbInvoiceNumber { get; set; }
        public long MarketingClassId { get; set; }
        public long FranchiseeId { get; set; }
        public long CustomerId { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public long StatusId { get; set; }
        public long? Year { get; set; }
        public long ReviewStatusId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}
