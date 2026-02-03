using Core.Application.Domain;

namespace Core.Notification.Domain
{
    public class NotificationResource : DomainBase
    {
        public long ResourceId { get; set; }  
        public long NotificationEmailId { get; set; } 
        public virtual NotificationEmail NotificationEmail { get; set; }
        public virtual File Resource { get; set; } 
    }
}
