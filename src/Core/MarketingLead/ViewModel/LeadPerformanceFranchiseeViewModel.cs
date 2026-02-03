using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class LeadPerformanceFranchiseeViewModel
    {
        public long Id { get; set; }
        public long FranchiseeId { get; set; }

        public double Amount { get; set; }
        public int Month { get; set; }
        public DateTime DateTime { get; set; }
        public string UserName { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public bool? IsActive { get; set; }
        public long? SeoBillingPeriod { get; set; }
    }
}
