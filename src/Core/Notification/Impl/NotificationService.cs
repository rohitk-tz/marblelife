using Core.Application;
using Core.Application.Attribute;
using Core.Application.Exceptions;
using Core.Notification.Domain;
using Core.Notification.Enum;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class NotificationService : INotificationService
    {
        private readonly IRepository<NotificationQueue> _notificationRepository;
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        private readonly IRepository<Person> _personRepository;
        private readonly ISettings _settings;
        private readonly IClock _clock;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<EmailSignatures> _emailSignaturesRepository;

        public NotificationService(IUnitOfWork unitOfWork, IOrganizationRoleUserInfoService organizationRoleUserInfoService, ISettings settings, IClock clock)
        {
            _personRepository = unitOfWork.Repository<Person>();
            _notificationRepository = unitOfWork.Repository<NotificationQueue>();
            _emailTemplateRepository = unitOfWork.Repository<EmailTemplate>();
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
            _settings = settings;
            _clock = clock;
            _organizationRepository = unitOfWork.Repository<Organization>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _emailSignaturesRepository = unitOfWork.Repository<EmailSignatures>();
        }

        public NotificationQueue QueueUpNotificationEmail<T>(NotificationTypes notificationTypes, T model, long organizationRoleUserId, DateTime? notificationDateTime = null, List<NotificationResource> resource = null)
        {
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserbyId(organizationRoleUserId);
            var person = _personRepository.Get(organizationRoleUser.UserId);
            var email = person.Email;
            if ((long)notificationTypes == (long)NotificationTypes.SendInvoiceDetail || (long)notificationTypes == (long)NotificationTypes.SalesDataUploadReminder ||
                (long)notificationTypes == (long)NotificationTypes.PaymentConfirmation || (long)notificationTypes == (long)NotificationTypes.PaymentReminder || (long)notificationTypes == (long)NotificationTypes.SendInvoiceDetail
               || (long)notificationTypes == (long)NotificationTypes.DocumentExpiryNotification || (long)notificationTypes == (long)NotificationTypes.LateFeeReminderForPayment
               || (long)notificationTypes == (long)NotificationTypes.LateFeeReminderForSalesData || (long)notificationTypes == (long)NotificationTypes.PaymentConfirmation
               || (long)notificationTypes == (long)NotificationTypes.AnnualUploadApproved || (long)notificationTypes == (long)NotificationTypes.AnnualUploadRejected
               || (long)notificationTypes == (long)NotificationTypes.AnnualUploadParsedNotification || (long)notificationTypes == (long)NotificationTypes.AnnualUploadFailNotification
               )
            {
                var accountPersonEmail = organizationRoleUser.Organization.Franchisee.AccountPersonEmail;
                if (accountPersonEmail != null)
                {
                    email = accountPersonEmail;
                }
                else if (accountPersonEmail == null)
                {
                    var contactEmail = organizationRoleUser.Organization.Franchisee.ContactEmail;
                    if (contactEmail != null)
                    {
                        email = contactEmail;
                    }
                }

            }
            if (!string.IsNullOrEmpty(person.Email))
            {
                //save email in NotificationQueue
                return QueueUpNotificationEmail(notificationTypes, model, _settings.CompanyName, _settings.FromEmail, email, notificationDateTime ?? _clock.UtcNow, organizationRoleUserId, resource);
            }
            return null;
        }
        public NotificationQueue QueueUpNotificationEmail<T>(NotificationTypes notificationTypes, T model, string fromName,
                                        string fromEmail, string toEmail, DateTime notificationDateTime, 
                                        long? organizationRoleUserId = null, List<NotificationResource> resource = null, 
                                        string recipientEmail = null)
        {
            // get Email Template
            var emailTemplate = _emailTemplateRepository.Table.FirstOrDefault(x => x.NotificationType.Id == (long)notificationTypes);
            var EmptynotificationQueue = new NotificationQueue();
            string franchiseeName = "";
            string ccMail = "";
            dynamic d = model;
            var franchisee = new Organization();
            long? franchiseeId = default(long);
            PropertyInfo[] properties = d.GetType().GetProperties();
            var bodyTemplate = string.Empty;
            bodyTemplate = emailTemplate.Body;
            if (properties.Any(x => x.Name == "CCMail"))
            {
                if ((d.CCMail) != "")
                {
                    ccMail = (d.CCMail);
                }
            }
            try
            {
                var value = properties.Any(x => x.Name == "Franchisee");
                if (value)
                {
                    franchiseeName = (d.Franchisee);
                }
                else if (properties.Any(x => x.Name == "FranchiseeName"))
                {
                    franchiseeName = (d.FranchiseeName);
                }
                franchisee = _organizationRepository.TableNoTracking.Where(x => x.Name == franchiseeName).FirstOrDefault();
                franchiseeId = _organizationRepository.TableNoTracking.Where(x => x.Name == franchiseeName).Select(x => x.Id).FirstOrDefault();
                if (franchisee != null && franchiseeId != 0)
                {
                    var organizationDomain = _franchiseeRepository.Table.FirstOrDefault(x => x.Id == franchiseeId);
                    var emailTemplateLanguageWise = _emailTemplateRepository.Get(x => x.NotificationType.Id == (long)notificationTypes &&
                                       x.LanguageId == organizationDomain.LanguageId);
                    if (emailTemplateLanguageWise != null)
                    {
                        emailTemplate = emailTemplateLanguageWise;
                    }
                }
            }
            catch (Exception)
            {
                franchiseeId = null;
            }
            if (emailTemplate == null || emailTemplate.NotificationType == null)
                throw new InvalidDataProvidedException("No Email template found.");
            if (fromEmail == null)
                throw new InvalidDataProvidedException("Empty FromEmail");
            if (toEmail == null)
                throw new InvalidDataProvidedException("Empty toEmail");
            if (!emailTemplate.isActive)
            {
                return EmptynotificationQueue;
            }
            if (emailTemplate.NotificationType != null && emailTemplate.NotificationType.IsQueuingEnabled == false)
            {
                return null;
            }
            if (notificationTypes == NotificationTypes.SendInvoiceDetail ||
                notificationTypes == NotificationTypes.PaymentReminder
                || notificationTypes == NotificationTypes.SalesDataUploadReminder
                || notificationTypes == NotificationTypes.LateFeeReminderForPayment
                || notificationTypes == NotificationTypes.PaymentConfirmation
                || notificationTypes == NotificationTypes.LateFeeReminderForSalesData
                || notificationTypes == NotificationTypes.WeeklyLateFeeNotification
                || notificationTypes == NotificationTypes.WeeklyUnpaidInvoiceNotification
                || notificationTypes == NotificationTypes.MonthlySalesUploadNotification
                || notificationTypes == NotificationTypes.NewJobNotificationToUser
                || notificationTypes == NotificationTypes.PostEstimatetoCustomer
                || notificationTypes == NotificationTypes.NewJobNotificationToUserOnDay
                || notificationTypes == NotificationTypes.NewJobNotificationToUserReassigned
                || notificationTypes == NotificationTypes.UrgentJobNotificationToUser
                || notificationTypes == NotificationTypes.NewCustomerMail
                || notificationTypes == NotificationTypes.UpdateCustomerMail
                || notificationTypes == NotificationTypes.PersonalMailForMemebers
                || notificationTypes == NotificationTypes.DeletionJobNotificationToTech
                || notificationTypes == NotificationTypes.MailToCustomerForInvoice
                || notificationTypes == NotificationTypes.MailToCustomerForSignedInvoice
                || notificationTypes == NotificationTypes.MailToSalesRepForSignedInvoice
                || notificationTypes == NotificationTypes.MailToCustomerForPostJobCompletion
                || notificationTypes == NotificationTypes.PostJobFeedbackToCustomer
                || notificationTypes == NotificationTypes.PostJobFeedbackToSalesRep)
            {
                if (properties.Any(x => x.Name == "Signature"))
                {
                    var emailSignature = _emailSignaturesRepository.Table.Where(x => x.IsDefault && x.IsActive && x.Person.Email == fromEmail).FirstOrDefault();
                    if (emailSignature != null)
                    {
                        d.HasCustomSignature = "block";
                        d.NotHasCustomSignature = "none";
                        d.Signature = emailSignature.Signature;
                    }
                    else
                    {
                        d.NotHasCustomSignature = "block";
                        d.HasCustomSignature = "none";
                        d.Signature = "";
                    }
                }
                else
                {
                    d.NotHasCustomSignature = "block";
                    d.HasCustomSignature = "none";
                    d.Signature = "";
                }
                
                bodyTemplate = bodyTemplate.Replace("@Model.Signature", d.Signature);
                var xyCustomSignature =  model.GetType().GetProperty("HasCustomSignature");
                if (xyCustomSignature != null)
                {
                    xyCustomSignature.SetValue(model, d.HasCustomSignature);
                }
                var xyNotCustomSignature = model.GetType().GetProperty("NotHasCustomSignature");
                if (xyNotCustomSignature != null)
                {
                    xyNotCustomSignature.SetValue(model, d.NotHasCustomSignature);
                }
            }            
            string body = NotificationServiceHelper.FormatContent(bodyTemplate, model);
            string subject = NotificationServiceHelper.FormatContent(emailTemplate.Subject, model);
            
            var notificationQueue = NotificationServiceHelper.CreateDomain(emailTemplate.NotificationType, notificationDateTime);
            notificationQueue.NotificationEmail = NotificationServiceHelper.CreateDomain(emailTemplate.Id, 
                fromEmail, fromName, subject, body, resource);
            
            if ((long)notificationTypes == (long)NotificationTypes.LateFeeReminderForPayment || 
                (long)notificationTypes == (long)NotificationTypes.LateFeeReminderForSalesData
                || (long)notificationTypes == (long)NotificationTypes.AnnualUploadApproved || 
                (long)notificationTypes == (long)NotificationTypes.AnnualUploadRejected)
            {
                if (franchiseeName != null)
                {
                    franchisee = _organizationRepository.TableNoTracking.Where(x => x.Name == franchiseeName).FirstOrDefault();
                    var ownerEmail = franchisee.Email;
                    notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient 
                    { 
                        IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, RecipientEmail = ownerEmail, 
                        RecipientTypeId = (long)RecipientType.CC });
                    }
            }
            if ((long)notificationTypes == (long)NotificationTypes.PaymentConfirmation || 
                (long)notificationTypes == (long)NotificationTypes.SendInvoiceDetail
                || (long)notificationTypes == (long)NotificationTypes.DocumentExpiryNotification)
            {
                notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { 
                    IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, 
                    RecipientEmail = _settings.CCToAdmin, RecipientTypeId = (long)RecipientType.CC });
            }
            if ((long)notificationTypes == (long)NotificationTypes.BeforeAfterImages)
            {
                var mail = _settings.RecipientEmail;
                notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
                    IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, 
                    RecipientEmail = ccMail, RecipientTypeId = (long)RecipientType.CC });
            }
            if ((long)notificationTypes == (long)NotificationTypes.NewJobNotificationToUser)
            {
                if (franchiseeId == (long?)OrganizationNames.MIDetroit || 
                    franchiseeId == (long?)OrganizationNames.PAPhiladelphia || 
                    franchiseeId == (long?)OrganizationNames.PAPittsburgh || 
                    franchiseeId == (long?)OrganizationNames.MIGrandRapids)
                {
                    var mail = _settings.RecipientEmail;
                    notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
                        IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, 
                        RecipientEmail = mail, RecipientTypeId = (long)RecipientType.CC });
                }
            }
            var mailList = toEmail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (notificationTypes == NotificationTypes.BeforeAfterImageForFA || 
                notificationTypes == NotificationTypes.RenewableMailForFranchiseeBefore8Month
                || notificationTypes == NotificationTypes.RenewableMailForFranchiseeBefore9Month 
                || notificationTypes == NotificationTypes.MeetingMailForMemebers
                || notificationTypes == NotificationTypes.PersonalMailForMemebers 
                || notificationTypes == NotificationTypes.MailToCustomerForInvoice 
                || notificationTypes == NotificationTypes.MailToCustomerForSignedInvoice 
                || notificationTypes == NotificationTypes.MailToSalesRepForSignedInvoice)
            {
                var isFirst = true;
                foreach (var mail in mailList)
                {
                    if (isFirst)
                    {
                        notificationQueue.NotificationEmail.Recipients.Add(
                            new NotificationEmailRecipient { 
                                IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, 
                                RecipientEmail = mail, OrganizationRoleUserId = organizationRoleUserId,
                                RecipientTypeId = (long)RecipientType.TO 
                            });
                        isFirst = false;
                    }
                    else
                    {
                        notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { 
                            IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, 
                            RecipientEmail = mail, OrganizationRoleUserId = organizationRoleUserId, 
                            RecipientTypeId = (long)RecipientType.CC
                        });
                    }
                }
            }
            else
            {
                foreach (var mail in mailList)
                {
                    notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { 
                        IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, 
                        RecipientEmail = mail, OrganizationRoleUserId = organizationRoleUserId, 
                        RecipientTypeId = (long)RecipientType.TO
                    });
                }
            }
            if (notificationTypes == NotificationTypes.MailToCustomerForInvoice)
            {
                var ccMailList = ccMail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                ccMailList = ccMailList.Distinct().ToList();
                foreach (var mail in ccMailList)
                {
                    notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { 
                        IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, 
                        RecipientEmail = mail, OrganizationRoleUserId = organizationRoleUserId, 
                        RecipientTypeId = (long)RecipientType.CC });
                }
            }
            if(notificationTypes == NotificationTypes.WebLeadsMail)
            {
                var ccMailList = ccMail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                ccMailList = ccMailList.Distinct().ToList();
                foreach (var mail in ccMailList)
                {
                    notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient {
                        IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, 
                        RecipientEmail = mail, RecipientTypeId = (long)RecipientType.CC });
                }
            }
            if (notificationTypes == NotificationTypes.MailToCustomerForSignedInvoice)
            {
                var ccMailList = ccMail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                ccMailList = ccMailList.Distinct().ToList();
                foreach (var mail in ccMailList)
                {
                    notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient
                    {
                        IsNew = true,
                        NotificationEmail = notificationQueue.NotificationEmail,
                        RecipientEmail = mail,
                        RecipientTypeId = (long)RecipientType.CC
                    });
                }
            }
            if (franchiseeId == 0)
            {
                franchiseeId = 1;
            }
            notificationQueue.FranchiseeId = franchiseeId;
            _notificationRepository.Save(notificationQueue);
            return notificationQueue;
        }

        public NotificationQueue QueueUpNotificationDyamicEmail<T>(NotificationTypes notificationTypes, T model, string fromName,
                                            string fromEmail, string toEmail, DateTime notificationDateTime, string body1, long? organizationRoleUserId = null, List<NotificationResource> resource = null, string recipientEmail = null)
        {
            // get Email Template
            var emailTemplate = _emailTemplateRepository.Table.FirstOrDefault(x => x.NotificationType.Id == (long)notificationTypes);
            var EmptynotificationQueue = new NotificationQueue();
            string franchiseeName = "";
            string ccMail = "";
            dynamic d = model;
            var franchisee = new Organization();
            long? franchiseeId = default(long);
            PropertyInfo[] properties = d.GetType().GetProperties();
            if (properties.Any(x => x.Name == "CCMail"))
            {
                if ((d.CCMail) != "")
                {
                    ccMail = (d.CCMail);
                }
            }
            try
            {
                var value = properties.Any(x => x.Name == "Franchisee");

                if (value)
                {
                    franchiseeName = (d.Franchisee);
                }
                else if (properties.Any(x => x.Name == "FranchiseeName"))
                {
                    franchiseeName = (d.FranchiseeName);
                }
                franchisee = _organizationRepository.TableNoTracking.Where(x => x.Name == franchiseeName).FirstOrDefault();

                franchiseeId = _organizationRepository.TableNoTracking.Where(x => x.Name == franchiseeName).Select(x => x.Id).FirstOrDefault();


                if (franchisee != null && franchiseeId != 0)
                {
                    var organizationDomain = _franchiseeRepository.Table.FirstOrDefault(x => x.Id == franchiseeId);
                    var emailTemplateLanguageWise = _emailTemplateRepository.Get(x => x.NotificationType.Id == (long)notificationTypes &&
                                       x.LanguageId == organizationDomain.LanguageId);
                    if (emailTemplateLanguageWise != null)
                    {
                        emailTemplate = emailTemplateLanguageWise;
                    }
                }
            }
            catch (Exception)
            {
                franchiseeId = null;
            }



            if (emailTemplate == null || emailTemplate.NotificationType == null)
                throw new InvalidDataProvidedException("No Email template found.");
            if (fromEmail == null)
                throw new InvalidDataProvidedException("Empty FromEmail");
            if (toEmail == null)
                throw new InvalidDataProvidedException("Empty toEmail");


            if (!emailTemplate.isActive)
            {
                return EmptynotificationQueue;
            }

            if (emailTemplate.NotificationType != null && emailTemplate.NotificationType.IsQueuingEnabled == false)
            {
                return null;
            }
            if (notificationTypes == NotificationTypes.SendInvoiceDetail ||
                notificationTypes == NotificationTypes.PaymentReminder
                || notificationTypes == NotificationTypes.SalesDataUploadReminder
                || notificationTypes == NotificationTypes.LateFeeReminderForPayment
                || notificationTypes == NotificationTypes.PaymentConfirmation
                || notificationTypes == NotificationTypes.LateFeeReminderForSalesData
                || notificationTypes == NotificationTypes.WeeklyLateFeeNotification
                || notificationTypes == NotificationTypes.WeeklyUnpaidInvoiceNotification
                || notificationTypes == NotificationTypes.MonthlySalesUploadNotification
                || notificationTypes == NotificationTypes.NewJobNotificationToUser
                || notificationTypes == NotificationTypes.PostEstimatetoCustomer
                || notificationTypes == NotificationTypes.NewJobNotificationToUserOnDay
                || notificationTypes == NotificationTypes.NewJobNotificationToUserReassigned
                || notificationTypes == NotificationTypes.UrgentJobNotificationToUser
                || notificationTypes == NotificationTypes.NewCustomerMail
                || notificationTypes == NotificationTypes.UpdateCustomerMail
                || notificationTypes == NotificationTypes.PersonalMailForMemebers
                || notificationTypes == NotificationTypes.DeletionJobNotificationToTech
                || notificationTypes == NotificationTypes.MailToCustomerForInvoice
                || notificationTypes == NotificationTypes.MailToCustomerForSignedInvoice
                || notificationTypes == NotificationTypes.MailToSalesRepForSignedInvoice
                || notificationTypes == NotificationTypes.MailToCustomerForPostJobCompletion
                || notificationTypes == NotificationTypes.PostJobFeedbackToCustomer
                || notificationTypes == NotificationTypes.PostJobFeedbackToSalesRep)
            {
                if (properties.Any(x => x.Name == "Signature"))
                {
                    var emailSignature = _emailSignaturesRepository.Table.Where(x => x.IsDefault && x.IsActive && x.OrganizationRoleUser.Person.Email == fromEmail).FirstOrDefault();
                    if (emailSignature != null)
                    {
                        d.HasCustomSignature = "block";
                        d.NotHasCustomSignature = "none";
                        d.Signature = emailSignature.Signature;
                    }
                    else
                    {
                        d.NotHasCustomSignature = "block";
                        d.HasCustomSignature = "none";
                        d.Signature = "";
                    }

                }
                else
                {
                    d.NotHasCustomSignature = "block";
                    d.HasCustomSignature = "none";
                    d.Signature = "";
                }
                body1 = body1.Replace("@Model.Signature", d.Signature);
                //body1 = body1.Replace("@Model.NotHasCustomSignature", d.NotHasCustomSignature);
                //body1 = body1.Replace("@Model.HasCustomSignature", d.HasCustomSignature);
                var xyCustomSignature = model.GetType().GetProperty("HasCustomSignature");
                if (xyCustomSignature != null)
                {
                    xyCustomSignature.SetValue(model, d.HasCustomSignature);
                }
                var xyNotCustomSignature = model.GetType().GetProperty("NotHasCustomSignature");
                if (xyNotCustomSignature != null)
                {
                    xyNotCustomSignature.SetValue(model, d.NotHasCustomSignature);
                }

            }
            
            string body = NotificationServiceHelper.FormatContent(body1, model);
            string subject = NotificationServiceHelper.FormatContent(emailTemplate.Subject, model);

            var notificationQueue = NotificationServiceHelper.CreateDomain(emailTemplate.NotificationType, notificationDateTime);

            notificationQueue.NotificationEmail = NotificationServiceHelper.CreateDomain(emailTemplate.Id, fromEmail, fromName, subject, body, resource);

            notificationQueue.NotificationEmail.IsDynamicEmail = true;
            if ((long)notificationTypes == (long)NotificationTypes.LateFeeReminderForPayment || (long)notificationTypes == (long)NotificationTypes.LateFeeReminderForSalesData
                || (long)notificationTypes == (long)NotificationTypes.AnnualUploadApproved || (long)notificationTypes == (long)NotificationTypes.AnnualUploadRejected)
            {
                if (franchiseeName != null)
                {
                    franchisee = _organizationRepository.TableNoTracking.Where(x => x.Name == franchiseeName).FirstOrDefault();
                    var ownerEmail = franchisee.Email;
                    notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, RecipientEmail = ownerEmail, RecipientTypeId = (long)RecipientType.CC });
                }
            }

            if ((long)notificationTypes == (long)NotificationTypes.PaymentConfirmation || (long)notificationTypes == (long)NotificationTypes.SendInvoiceDetail
                || (long)notificationTypes == (long)NotificationTypes.DocumentExpiryNotification)
            {
                notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, RecipientEmail = _settings.CCToAdmin, RecipientTypeId = (long)RecipientType.CC });
            }

            if ((long)notificationTypes == (long)NotificationTypes.BeforeAfterImages)
            {
                var mail = _settings.RecipientEmail;
                notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, RecipientEmail = ccMail, RecipientTypeId = (long)RecipientType.CC });
            }
            if ((long)notificationTypes == (long)NotificationTypes.NewJobNotificationToUser)
            {
                if (franchiseeId == (long?)OrganizationNames.MIDetroit || franchiseeId == (long?)OrganizationNames.PAPhiladelphia || franchiseeId == (long?)OrganizationNames.PAPittsburgh || franchiseeId == (long?)OrganizationNames.MIGrandRapids)
                {
                    var mail = _settings.RecipientEmail;
                    notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, RecipientEmail = mail, RecipientTypeId = (long)RecipientType.CC });
                }
            }

            var mailList = toEmail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (notificationTypes == NotificationTypes.BeforeAfterImageForFA || notificationTypes == NotificationTypes.RenewableMailForFranchiseeBefore8Month
                || notificationTypes == NotificationTypes.RenewableMailForFranchiseeBefore9Month || notificationTypes == NotificationTypes.MeetingMailForMemebers
                || notificationTypes == NotificationTypes.PersonalMailForMemebers || notificationTypes == NotificationTypes.MailToCustomerForInvoice)
            {
                var isFirst = true;
                foreach (var mail in mailList)
                {
                    if (isFirst)
                    {
                        notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, RecipientEmail = mail, OrganizationRoleUserId = organizationRoleUserId, RecipientTypeId = (long)RecipientType.TO });
                        isFirst = false;
                    }
                    else
                    {
                        notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, RecipientEmail = mail, OrganizationRoleUserId = organizationRoleUserId, RecipientTypeId = (long)RecipientType.CC });

                    }
                }
            }
            else
            {
                foreach (var mail in mailList)
                {
                    notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, RecipientEmail = mail, OrganizationRoleUserId = organizationRoleUserId, RecipientTypeId = (long)RecipientType.TO });
                }
            }

            if (notificationTypes == NotificationTypes.MailToCustomerForInvoice)
            {
                var ccMailList = ccMail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                ccMailList = ccMailList.Distinct().ToList();
                foreach (var mail in ccMailList)
                {
                    notificationQueue.NotificationEmail.Recipients.Add(new NotificationEmailRecipient { IsNew = true, NotificationEmail = notificationQueue.NotificationEmail, RecipientEmail = mail, OrganizationRoleUserId = organizationRoleUserId, RecipientTypeId = (long)RecipientType.CC });
                }
            }
            if (franchiseeId == 0)
            {
                franchiseeId = 1;
            }
            notificationQueue.FranchiseeId = franchiseeId;
            _notificationRepository.Save(notificationQueue);

            return notificationQueue;
        }

    }
}
