using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Domain
{
    public class CustomerSchedulerReminderAudit : DomainBase
    {
        public DateTime CreatedOn { get; set; }

        public long? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual JobCustomer JobCustomer { get; set; }

        public long? JobSchedulerId { get; set; }

        public long? JobId { get; set; }
        public long? EstimateId { get; set; }

        [ForeignKey("JobSchedulerId")]
        public virtual JobScheduler JobScheduler { get; set; }
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }
        [ForeignKey("EstimateId")]
        public virtual JobEstimate JobEstimate { get; set; }

    }
}
