using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class SalesFunnelNationalListModel
    {
        public IEnumerable<SalesFunnelNationalViewModel> Collection { get; set; }
        public SalesFunnelNationalBestViewModel BestFranchiseeCollection { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<SalesFunnelLocalViewModel> LocalCollection { get; set; }
    }
}
