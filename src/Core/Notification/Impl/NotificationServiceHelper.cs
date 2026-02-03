using Core.Application.Domain;
using Core.Notification.Domain;
using Core.Notification.Enum;
using RazorEngine;
using System;
using System.Collections.Generic;

namespace Core.Notification.Impl
{
    public class NotificationServiceHelper
    {
        public static NotificationQueue CreateDomain(NotificationType notificationtype, DateTime notificationdate)
        {
            return new NotificationQueue
            {
                NotificationDate = notificationdate,
                NotificationType = notificationtype,
                Source = string.Empty,
                ServiceStatusId = (long)ServiceStatus.Pending,
                IsNew = true,
                DataRecorderMetaData = new DataRecorderMetaData(),
            };
        }

        public static NotificationEmail CreateDomain(long id, string fromEmail, string fromName, string subject, string body, List<NotificationResource> resource)
        {
            return new NotificationEmail
            {
                EmailTemplateId = id,
                FromEmail = fromEmail,
                FromName = fromName,
                Subject = subject,
                Body = body,
                Resources = resource 
            };
        }

        public static string FormatContent<T>(string text, T model)
        {
            return Razor.Parse(text, model);
        }
    }
}
