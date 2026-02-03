using Core.Application.Attribute;
using System;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class WebLeadFilter
    {
        public long FranchiseeId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public string PropertyType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ConvertedLead { get; set; }
        public string URL { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
    }
}

