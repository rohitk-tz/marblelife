using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Core.Scheduler.Domain
{
    public class JobEstimateImageCategory : DomainBase
    {
        public long? SchedulerId { get; set; }
        public long? MarkertingClassId { get; set; }
        public long? JobId { get; set; }
        public long? EstimateId { get; set; }
        [ForeignKey("JobId")]
        public virtual JobScheduler JobSchedulerJobId { get; set; }
        [ForeignKey("EstimateId")]
        public virtual JobScheduler JobSchedulerEstimateId { get; set; }

        [ForeignKey("MarkertingClassId")]
        public virtual MarketingClass MarketingClass { get; set; }
        [ForeignKey("SchedulerId")]
        public virtual JobScheduler JobScheduler { get; set; }
       


    }
}
