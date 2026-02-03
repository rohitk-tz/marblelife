using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Review.ViewModel
{
    [NoValidatorRequired] 
    public class CustomerFeedbackReportListModel 
    {
        public IEnumerable<CustomerFeedbackReportViewModel> Collection { get; set; }
        public CustomerFeedbackReportFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; } 
    }
}
