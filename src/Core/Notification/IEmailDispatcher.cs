using Core.Notification.Domain;
using System.Collections.Generic;

namespace Core.Notification
{
    public interface IEmailDispatcher
    {
        void SendEmail(string body, string subject, string fromName, string fromEmail, string[] ccEmails, IEnumerable<NotificationResource> resources, params string[] toEmail);
        void SendNormalEmail(string body, string subject, string fromName, string fromEmail, string[] ccEmails, IEnumerable<NotificationResource> resources, params string[] toEmail);
    }
}
