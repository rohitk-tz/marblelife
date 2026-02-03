using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Core.Scheduler.Domain
{
  public class TechAndSalesSchedulerReminder : DomainBase
    {
        public DateTime CreatedOn { get; set; }

        public long? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual JobCustomer JobCustomer { get; set; }

        public long? JobSchedulerId { get; set; }

        [ForeignKey("JobSchedulerId")]
        public virtual JobScheduler JobScheduler { get; set; }

        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }
    }
}
