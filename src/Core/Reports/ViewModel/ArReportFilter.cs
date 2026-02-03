using Core.Application.Attribute;
using System;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
   public class ArReportFilter
    {
        public long FranchiseeId { get; set; }
        public DateTime? ReportDateStart { get; set; }
    }
}
