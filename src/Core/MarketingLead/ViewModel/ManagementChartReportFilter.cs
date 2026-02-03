using Core.Application.Attribute;
using System;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class ManagementChartReportFilter
    {
        public bool? IsTimeSet { get; set; }
        public long? FranchiseeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsTimeChange { get; set; }

        public int Month { get; set; }
        public string Year { get; set; }
    }
}
