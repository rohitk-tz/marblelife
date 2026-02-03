using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    public class WebLeadListViewModel
    {
        public IEnumerable<WebLeadInfoModel> Collection { get; set; }
        public WebLeadFilter Filter { get; set; } 
        public PagingModel PagingModel { get; set; }
    }
}
