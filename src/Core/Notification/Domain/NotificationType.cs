namespace Core.Notification.Domain
{
    public class NotificationType : DomainBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsServiceEnabled { get; set; }
        public bool IsQueuingEnabled { get; set; }
    }
}
