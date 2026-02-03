using Core.Application.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Notification.Domain
{
    public class NotificationEmail : DomainBase
    {
        public long EmailTemplateId { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        [ForeignKey("EmailTemplateId")]
        public virtual EmailTemplate EmailTemplate { get; set; }
        public virtual NotificationQueue NotificationQueue { get; set; }
        public virtual IList<NotificationEmailRecipient> Recipients { get; set; }
        public virtual IList<NotificationResource> Resources { get; set; }
        public bool? IsDynamicEmail { get; set; }
        public NotificationEmail()
        {
            Recipients = new List<NotificationEmailRecipient>();
        }
    }
}
