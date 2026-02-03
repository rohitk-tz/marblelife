using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class RoyaltyReportViewModel
    {
        public long ClassId { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<ServiceAmountViewModel> ServiceAmountViewModel { get; set; }
    }

}
