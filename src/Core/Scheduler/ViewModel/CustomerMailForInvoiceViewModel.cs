using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
  public  class CustomerMailForInvoiceViewModel
    {
        public string FranchiseeName { get; set; }
        public string CustomerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string EmailId { get; set; }
        public string FromMail { get; set; }
        public long? FranchiseeId { get; set; }
        public long? CustomerId { get; set; }
        public string CCMail { get; set; }
    }
}
