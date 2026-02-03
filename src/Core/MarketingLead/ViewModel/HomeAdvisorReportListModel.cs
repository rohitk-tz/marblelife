using Core.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    public class HomeAdvisorReportListModel
    {
        public IEnumerable<HomeAdvisorParentModel> Collection { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
