using Core.Application.Attribute;
using System;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class MLFSReportListFilter
    {
        public int StartDate { get; set; }
        public int EndDate { get; set; }
        public long? FranchiseeId { get; set; }
        public string Text { get; set; }
        public long? UserId { get; set; }
    }

    [NoValidatorRequired]
    public class MLFSConfigurationFilter
    {
        public long? FranchiseeId { get; set; }
        public string Text { get; set; }
        public long? UserId { get; set; }
    }
    
}
