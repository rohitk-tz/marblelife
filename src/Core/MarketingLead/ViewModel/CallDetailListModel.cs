using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    public class CallDetailListModel
    {
        public IEnumerable<CallDetailViewModel> Collection { get; set; }
        public CallDetailFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
