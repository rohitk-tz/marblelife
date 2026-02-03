using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Notification.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
   public class BeforeAfterImageMailAudit : DomainBase
    {
        public long? SchedulerId { get; set; }
        [ForeignKey("SchedulerId")]
        public virtual JobScheduler JobScheduler { get; set; }

        public long? FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Organization Organization { get; set; }

        public long? BeforeAfterCategoryIdBeforeImageId { get; set; }
        [ForeignKey("BeforeAfterCategoryIdBeforeImageId")]
        public virtual JobEstimateServices JobEstimateServices { get; set; }
        public long? BeforeAfterCategoryIdAfterImageId { get; set; }
        [ForeignKey("BeforeAfterCategoryIdAfterImageId")]
        public virtual JobEstimateServices JobEstimateServices1 { get; set; }
        public long? FileId { get; set; }
        [ForeignKey("FileId")]
        public virtual File File { get; set; }

        public long? NotificationQueueId { get; set; }
        [ForeignKey("NotificationQueueId")]
        public virtual NotificationQueue NotificationQueue { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
