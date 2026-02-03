using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Dashboard.ViewModel
{
    [NoValidatorRequired]
    public class SalesSummaryListModel
    {
        public IEnumerable<SalesSummaryViewModel> Collection { get; set; }
    }
}
