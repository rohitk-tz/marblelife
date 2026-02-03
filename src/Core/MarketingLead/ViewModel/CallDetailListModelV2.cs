using Core.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
   public class CallDetailListModelV2
    {
        public IEnumerable<CallDetailViewModelV2> Collection { get; set; }
        public CallDetailFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
