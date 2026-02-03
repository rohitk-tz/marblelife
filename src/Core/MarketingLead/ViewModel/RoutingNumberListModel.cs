using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    public class RoutingNumberListModel
    {
        public IEnumerable<RoutingNumberViewModel> Collection { get; set; }
        public PagingModel PagingModel { get; set; }
        public CallDetailFilter Filter { get; set; }
    }
}
