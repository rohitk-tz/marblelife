using Core.Application;
using Core.Application.Attribute;
using Core.Notification.Domain;
using Core.Notification.ViewModel;
//using MailChimp.Net.Core;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
//using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
//using Attachment = SendGrid.Helpers.Mail.Attachment;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class EmailDispatcher : IEmailDispatcher
    {
        private readonly ILogService _logService;
        private readonly ISettings _settings;
        private static readonly string MessageId = "X-Message-Id";

        public EmailDispatcher(ISettings settings, ILogService logService)
        {
            _settings = settings;
            _logService = logService;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
        public async void SendEmail(string body, string subject, string fromName, string fromEmail, string[] ccEmails, IEnumerable<NotificationResource> resources, params string[] toEmail)
        {
            try
            {
                var _client = new SendGridClient(_settings.SmtpEmailApiKey);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var emailMessage = new SendGridMessage()
                {
                    From = new EmailAddress(fromEmail, fromName),
                    Subject = subject,
                    HtmlContent = body,

                };
                emailMessage.Headers = new Dictionary<string, string>();
                //foreach (var toemail in toEmail)
                //{
                emailMessage.AddTo(new EmailAddress(toEmail[0], toEmail[0]));

                if (ccEmails.Length >= 1)
                {
                    foreach (var ccEmail in ccEmails)
                    {
                        emailMessage.AddCc(new EmailAddress() { Email = ccEmail, Name = "" });
                    }
                }
                emailMessage.Attachments = new List<SendGrid.Helpers.Mail.Attachment>();
                emailMessage.Headers.Add("Priority", "Urgent");
                foreach (var resource in resources)
                {
                    var filePth = resource.Resource.RelativeLocation + "/" + resource.Resource.Name;
                    AttachResource(emailMessage, filePth, resource.Resource.MimeType, resource.Resource.Name);
                }
                var from = new EmailAddress(fromEmail, fromName);
                var subject1 = subject;
                var toemail1 = new EmailAddress(toEmail[0], toEmail[0]);
                var plainTextContent = "";
                var htmlContent = body;
                var msg = MailHelper.CreateSingleEmail(from, toemail1, subject, plainTextContent, htmlContent);
                //var x = await _client.SendEmailAsync(msg).ConfigureAwait(false);
                if (emailMessage.Attachments.Count() > 0)
                {
                   ProcessResponse(Task.Run(async () => await _client.SendEmailAsync(emailMessage).ConfigureAwait(true)).Result);
                    //var x = await ;
                }
                else
                {
                    ProcessResponse(Task.Run(async () => await _client.SendEmailAsync(msg).ConfigureAwait(true)).Result);
                    //var x = await _client.SendEmailAsync(msg).ConfigureAwait(false);
                }
            }
            catch (Exception e1)
            {
                _logService.Info("Sending Mail Failed with Exception  " + e1.InnerException);
            }
            //}


        }


        private void ProcessResponse(Response response)
        {
            _logService.Info("Status of Message Send " + response.StatusCode.ToString());
        }


        public void AttachResource(SendGridMessage message, string filePath, string contentType, string resourceName)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Attachment not found.", filePath);

            SendGrid.Helpers.Mail.Attachment attachment = new SendGrid.Helpers.Mail.Attachment();
            Byte[] bytes = File.ReadAllBytes(filePath);
            String file = Convert.ToBase64String(bytes);
            attachment.Type = contentType;
            attachment.Content = file;
            attachment.Filename = resourceName;
            attachment.Disposition = "inline";
            message.AddAttachment(resourceName, file);
        }

        public void SendNormalEmail(string body, string subject, string fromName, string fromEmail, string[] ccEmails, IEnumerable<NotificationResource> resources, params string[] toEmail)
        {
            try
            {
                string from = "abhishekkpr9@gmail.com"; //example:- sourabh9303@gmail.com  
                using (MailMessage mail = new MailMessage("abhishekkpr9@yahoo.com", "Akapoor@159"))
                {
                    mail.Subject = subject;
                    mail.Body = body;
                    //foreach (var resource in resources)
                    //{
                    //    var filePath = resource.Resource.RelativeLocation + "/" + resource.Resource.Name;
                    //    Byte[] bytes = File.ReadAllBytes(filePath);
                    //    String file = Convert.ToBase64String(bytes);
                    //    mail.Attachments.Add(new System.Net.Mail.Attachment(filePath));
                    //}

                    mail.IsBodyHtml = false;
                    mail.From = new MailAddress(fromEmail);
                    foreach (var to in toEmail)
                    {
                        mail.To.Add(new MailAddress(to));
                    }
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.mail.yahoo.com";
                    smtp.EnableSsl = true;
                    NetworkCredential networkCredential = new NetworkCredential("abhishekkpr9@yahoo.com", "Taazaa@2503");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCredential;
                    smtp.Port = 465;
                    smtp.Send(mail);
                }
            }
            catch (Exception e1)
            {

            }
            //}
        }
    }
}
