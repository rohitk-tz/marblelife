using Core.Notification.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Reports.Domain
{
    public class WeeklyNotification : DomainBase
    {
        public DateTime NotificationDate { get; set; }
        public long NotificationTypeId { get; set; }
        [ForeignKey("NotificationTypeId")]
        public virtual NotificationType NotificationType { get; set; }
    }
}
