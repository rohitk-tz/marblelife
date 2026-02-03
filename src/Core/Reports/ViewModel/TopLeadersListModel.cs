using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    public class TopLeadersListModel
    {
        public IEnumerable<TopLeadersViewModel> Collection { get; set; }
        public TopLeadersFilter Filter { get; set; }
    }
}
