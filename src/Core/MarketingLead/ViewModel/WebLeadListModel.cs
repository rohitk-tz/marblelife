using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    public class WebLeadListModel
    {
        public string result { get; set; }
        public string code { get; set; } 
        public List<WebLeadViewModel> info { get; set; } 
    }
}
