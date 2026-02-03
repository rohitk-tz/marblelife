using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Reports.ViewModel
{

    public class MLFSReportConfigurationListModel
    {
        public List<MLFSReportConfigurationViewModel> MLFSReportConfigurationViewModel {get; set;}
    }
    [NoValidatorRequired]
    public class MLFSReportConfigurationViewModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public bool IsActive { get; set; }
        public string ColorCode { get; set; }
    }
    [NoValidatorRequired]
    public class MLFSEditModel
    {
        public long? UserId { get; set; }
        public List<MLFSReportConfigurationViewModel> StatusList { get; set; }
    }
}
