using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class LeadPerformanceEditModel
    {
        public string PpcSpend { get; set; }
        public string SeoCost { get; set; }
        public string SeoCostOriginal { get; set; }
        public bool IsSEOActive { get; set; }
        public bool IsSEOActiveOriginal { get; set; }
        public long SeoCostBillingPeriodId { get; set; }
        public long SeoCostBillingPeriodIdOriginal { get; set; }
    }
}
