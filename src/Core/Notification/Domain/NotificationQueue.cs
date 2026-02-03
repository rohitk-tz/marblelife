using Core.Application.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Notification.Domain
{
    public class NotificationQueue : DomainBase
    {
        public long NotificationTypeId { get; set; }
        public DateTime NotificationDate { get; set; }
        public long ServiceStatusId { get; set; }
        public DateTime? ServicedAt { get; set; }
        public int? AttemptCount { get; set; }
        public string Source { get; set; }
        public long? FranchiseeId { get; set; }
        public long DataRecorderMetadataId { get; set; }

        [ForeignKey("DataRecorderMetadataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        [ForeignKey("NotificationTypeId")]
        public virtual NotificationType NotificationType { get; set; }
        public virtual NotificationEmail NotificationEmail { get; set; }

        [ForeignKey("ServiceStatusId")]
        public virtual Lookup ServiceStatus { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Organization Organization { get; set; }
    }
}
