using Core.Application;
using Core.Application.Attribute;
using Core.Notification.Domain;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class NotificationPollingAgent : INotificationPollingAgent
    {
        private readonly IRepository<NotificationQueue> _notificationQueueRepository;
        private readonly ILogService _logService;
        private readonly int _attemptCount; // todo: derive it from settings
        private readonly IClock _clock;
        private readonly IEmailDispatcher _emailDispatcher;
        private readonly ISettings _settings;

        public NotificationPollingAgent(IUnitOfWork unitOfWork, ILogService logService, IClock clock, IEmailDispatcher emailDispatcher, ISettings settings)
        {
            _notificationQueueRepository = unitOfWork.Repository<NotificationQueue>();
            _clock = clock;
            _logService = logService;
            _emailDispatcher = emailDispatcher;
            _attemptCount = 3;
            _settings = settings; 
        }
        public void PollForNotifications()
        {
            foreach (var notification in GetNotificationsToService())
            {
                if (notification.AttemptCount > _attemptCount)
                {
                    notification.ServiceStatusId = (long)ServiceStatus.Failed;
                    _logService.Info(string.Format("Already many failed attempts for Type - {0} for NotificationId - {1} ", notification.NotificationType.Title, notification.Id));
                }
                else
                {
                    notification.AttemptCount = (notification.AttemptCount ?? 0) + 1;
                    try
                    {
                        if (notification.NotificationEmail != null)
                        {
                             var emails = notification.NotificationEmail.Recipients.Where(x => x.RecipientTypeId == (long)RecipientType.TO).Select(x => x.RecipientEmail).Distinct().ToArray();


                            _logService.Info(string.Format("Send Email Type - {0} to {1} ", notification.NotificationType.Title, string.Join(",", emails)));
                            if (emails.Count() == 0)
                            {
                                continue;
                            }
                            ServiceEmailNotification(notification.NotificationEmail, emails);
                            //SendEmail(notification.NotificationEmail, emails);
                        }
                        notification.ServiceStatusId = (long)ServiceStatus.Success;
                    }
                    catch (Exception exception)
                    {
                        if (notification.AttemptCount > _attemptCount)
                        {
                            notification.ServiceStatusId = (long)ServiceStatus.Failed;
                        }

                        _logService.Error("Failed to dispatch notification " + notification.Id, exception);
                    }
                }
                try
                {
                    _logService.Info("Update Notification");
                    notification.ServicedAt = _clock.UtcNow;
                    _notificationQueueRepository.Save(notification);
                    ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>().SaveChanges();
                }
                catch (Exception exception)
                {
                    _logService.Error("Could not update notification " + notification.Id, exception);
                }
            }
        }
        private IEnumerable<NotificationQueue> GetNotificationsToService()
        {
            return _notificationQueueRepository.Fetch(
                     x => x.NotificationDate < _clock.UtcNow && x.ServiceStatus.Id == (long)ServiceStatus.Pending && x.NotificationType.IsServiceEnabled) ;
        }

        private void ServiceEmailNotification(NotificationEmail notification, string[] recipients)
        {
            try
            {
                var ccrecipientsemails = notification.Recipients.Where(x => x.RecipientTypeId == (long)RecipientType.CC).Select(x => x.RecipientEmail).ToArray();
                _emailDispatcher.SendEmail(notification.Body, notification.Subject, notification.FromName, notification.FromEmail, ccrecipientsemails, notification.Resources, recipients);
                //_emailDispatcher.SendEmail(notification.Body, notification.Subject, notification.FromName, notification.FromEmail, ccrecipientsemails, notification.Resources, recipients);
            }
            catch (Exception exception)
            {
                string errorMessage = string.Format("Could not send email for notification {0} to {1}", notification.Id, string.Join(",", recipients));
                _logService.Error(errorMessage, exception);
                throw new ApplicationException(errorMessage);
            }
        }
        public int SendEmail(NotificationEmail notification, string[] recipients)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("kguljar503@gmail.com");
            mailMessage.To.Add(recipients[0]);
            mailMessage.Subject = "";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = notification.Body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("kguljar503@gmail.com", "Kgulj@r18050");
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email Sent Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return 0;
        }
    }
}
