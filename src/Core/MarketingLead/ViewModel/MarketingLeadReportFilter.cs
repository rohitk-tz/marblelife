using Core.Application.Attribute;
using System;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class MarketingLeadReportFilter
    {

        public long? FranchiseeId { get; set; }
        public string Text { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public string URL { get; set; }
        public int PageNumber { get; set; }
        public int WebPageNumber { get; set; }
        public int PageSize { get; set; }
        public int ViewTypeId { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }
        public int Count { get; set; }

        public long CallTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ConvertedLead { get; set; }
        public int? MappedFranchisee { get; set; }
        public long? TagId { get; set; }
        public string Year { get; set; }
    }
}
