using Core.Application.Attribute;
using System;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class UploadReportFilter
    {
        public long FranchiseeId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? StatusId { get; set; } 
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }    
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public int? IsOnTimeUpload { get; set; }
    }
}
