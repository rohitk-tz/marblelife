using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Notification.Domain
{
    public class EmailTemplate : DomainBase
    {
        public long NotificationTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool isActive { get; set; }
        public bool IsRequired { get; set; }
        [ForeignKey("NotificationTypeId")]
        public virtual NotificationType NotificationType { get; set; }

        public long LanguageId { get; set; }

        [ForeignKey("LanguageId")]
        public virtual Lookup LookUp { get; set; }
    }
}
