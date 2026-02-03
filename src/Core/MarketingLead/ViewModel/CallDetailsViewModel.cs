using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
   public class CallDetailsViewModel
    {
        public string PhoneLabel { get; set; }
        public int Count { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? FranchiseeId { get; set; }
        public List<long> Id { get; set; }
    }
}
