using Core.Application.Domain;
using Core.Notification.Domain;
using Core.Notification.Enum;
using System;
using System.Collections.Generic;

namespace Core.Notification
{
    public interface INotificationService
    {
        NotificationQueue QueueUpNotificationEmail<T>(NotificationTypes notificationTypes, T model, long organizationRoleUserId, DateTime? notificationDateTime = null,
            List<NotificationResource> resource = null);

        NotificationQueue QueueUpNotificationEmail<T>(NotificationTypes notificationTypes, T model, string fromName,
                                               string fromEmail, string toEmail, DateTime notificationDateTime, long? organizationRoleUserId, List<NotificationResource> resource = null, string recipientEmail = null);

        NotificationQueue QueueUpNotificationDyamicEmail<T>(NotificationTypes notificationTypes, T model, string fromName,
                                               string fromEmail, string toEmail, DateTime notificationDateTime, string body, long? organizationRoleUserId, List<NotificationResource> resource = null, string recipientEmail = null);
    }
}
