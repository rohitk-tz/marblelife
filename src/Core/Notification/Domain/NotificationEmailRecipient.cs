using Core.Application.Domain;
using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Notification.Domain
{
    public class NotificationEmailRecipient : DomainBase
    {
        public long NotificationId { get; set; }
        public long? OrganizationRoleUserId { get; set; }
        public string RecipientEmail { get; set; }

        [ForeignKey("NotificationId")]
        public virtual NotificationEmail NotificationEmail { get; set; }

        [ForeignKey("OrganizationRoleUserId")]
        public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }

        public long RecipientTypeId { get; set; }
        [ForeignKey("RecipientTypeId")]
        public virtual Lookup RecipientType { get; set; }
    }
}
