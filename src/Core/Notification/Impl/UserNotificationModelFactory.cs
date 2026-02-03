using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Exceptions;
using Core.Application.Impl;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Notification.Domain;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Reports;
using Core.Review.ViewModel;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Users.Enum;
using Core.Geo.Domain;
using System.Web;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class UserNotificationModelFactory : IUserNotificationModelFactory
    {
        private readonly IRepository<Core.Application.Domain.File> _fileRepository;
        private readonly INotificationService _notificationService;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        private readonly INotificationModelFactory _notificationModelFactory;
        private readonly IInvoiceItemFactory _invoiceItemFactory;
        private readonly IFranchiseeSalesPaymentService _franchiseeSalesPaymentService;
        private readonly IClock _clock;
        private IReportFactory _reportFactory;
        private ISettings _settings;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<CustomerSchedulerReminderAudit> _jobReminderAuditRepository;
        private readonly IRepository<TechAndSalesSchedulerReminder> _techAndSalesSchedulerRepository;
        private readonly IRepository<JobCustomer> _jobCustomerRepository;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IRepository<JobEstimate> _jobEstimateRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<EmailSignatures> _emailSignaturesRepository;
        private readonly IRepository<EstimateInvoiceCustomer> _estimateInvoiceCustomerRepository;


        public UserNotificationModelFactory(IUnitOfWork unitOfWork, IOrganizationRoleUserInfoService organizationRoleUserInfoService, IFranchiseeSalesPaymentService franchiseeSalesPaymentService,
           INotificationService notificationService, INotificationModelFactory notificationModelFactory, IInvoiceItemFactory invoiceItemFactory, IClock clock,
           IReportFactory reportFactory, ISettings settings)
        {
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
            _notificationService = notificationService;
            _notificationModelFactory = notificationModelFactory;
            _invoiceItemFactory = invoiceItemFactory;
            _franchiseeSalesPaymentService = franchiseeSalesPaymentService;
            _clock = clock;
            _reportFactory = reportFactory;
            _settings = settings;
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
            _jobReminderAuditRepository = unitOfWork.Repository<CustomerSchedulerReminderAudit>();
            _fileRepository = unitOfWork.Repository<Core.Application.Domain.File>();
            _jobCustomerRepository = unitOfWork.Repository<JobCustomer>();
            _techAndSalesSchedulerRepository = unitOfWork.Repository<TechAndSalesSchedulerReminder>();
            _jobEstimateRepository = unitOfWork.Repository<JobEstimate>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _emailSignaturesRepository = unitOfWork.Repository<EmailSignatures>();
            _estimateInvoiceCustomerRepository = unitOfWork.Repository<EstimateInvoiceCustomer>();
        }
        public void CreateForgetPasswordNotification(string passwordLink, Person person)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserbyUserId(person.Id) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserbyUserId(person.Id);
            var model = new UserForgetPasswordNotificationViewModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = person.FirstName + " " + person.LastName,
                PasswordLink = passwordLink,
                UserName = person.UserLogin.UserName,
                Franchisee = organizationRoleUser.Organization.Name
            };
            _notificationService.QueueUpNotificationEmail(NotificationTypes.ForgetPassword, model, organizationRoleUser.Id);
        }

        public void CreateLoginCredentialNotification(Person person, string password, bool includeSetupGuide)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserbyUserId(person.UserLogin.Id) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserbyUserId(person.UserLogin.Id);
            if (organizationRoleUser.Organization == null)
            {
                if (_organizationRoleUserInfoService.GetOrganizationByOrganizationId(organizationRoleUser.OrganizationId) == null)
                    throw new InvalidDataProvidedException("No Organization found.");
                organizationRoleUser.Organization = _organizationRoleUserInfoService.GetOrganizationByOrganizationId(organizationRoleUser.OrganizationId);
            }
            var model = new SendLoginCredentialViewModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = person.FirstName + " " + person.LastName,
                UserName = person.UserLogin.UserName,
                Password = password,
                Franchisee = organizationRoleUser.Organization.Name
            };
            if (!includeSetupGuide)
                _notificationService.QueueUpNotificationEmail(NotificationTypes.SendLoginCredential, model, organizationRoleUser.Id);
            else
                _notificationService.QueueUpNotificationEmail(NotificationTypes.SendLoginCredentialWithSetupGuide, model, organizationRoleUser.Id);
        }

        public void CreateInvoiceDetailNotification(long organizationId, IList<FranchiseeInvoice> franchiseeInvoiceList)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(organizationId) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(organizationId).FirstOrDefault();
            var organizationName = organizationRoleUser.Organization.Name;
            var person = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(organizationRoleUser.Id);

            var model = new InvoiceDetailNotificationViewModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = person.FirstName + " " + person.LastName,
                Franchisee = organizationName,
            };

            var salesDataUpload = franchiseeInvoiceList.Select(x => x.SalesDataUpload).FirstOrDefault();
            if (salesDataUpload != null)
            {
                model.StartDate = salesDataUpload.PeriodStartDate.ToShortDateString();
                model.EndDate = salesDataUpload.PeriodEndDate.ToShortDateString();
            }
            model.InvoiceDetailList = new List<InvoiceViewModelForDetail>();
            foreach (var item in franchiseeInvoiceList)
            {
                var adFund = GetSumOfInvoiceBasedOnType(item.Invoice, InvoiceItemType.AdFund);
                var royalty = GetSumOfInvoiceBasedOnType(item.Invoice, InvoiceItemType.RoyaltyFee);
                var totalpayment = string.Format("{0:0.00}", item.Invoice.InvoiceItems.ToList().Sum(x => x.Amount));
                var royaltyAmount = string.Format("{0:0.00}", royalty);
                var adFundAmount = string.Format("{0:0.00}", adFund);

                var invoiceModel = new InvoiceViewModelForDetail { };
                invoiceModel.InvoiceItems = item.Invoice.InvoiceItems.Select(x => _invoiceItemFactory.CreateViewModel(x)).ToList();
                invoiceModel.InvoiceId = item.InvoiceId;
                invoiceModel.GeneratedOn = item.Invoice.GeneratedOn.ToShortDateString();
                invoiceModel.DueDate = item.Invoice.DueDate.ToShortDateString();
                invoiceModel.TotalPayment = totalpayment;
                invoiceModel.AdFund = adFundAmount;
                invoiceModel.Royalty = royaltyAmount;
                model.DueDate = item.Invoice.DueDate.ToShortDateString();
                model.InvoiceDetailList.Add(invoiceModel);
            }

            _notificationService.QueueUpNotificationEmail(NotificationTypes.SendInvoiceDetail, model, organizationRoleUser.Id);
        }

        private decimal GetSumOfInvoiceBasedOnType(Invoice invoice, InvoiceItemType type)
        {
            var invoiceItem = invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type).Select(x => x.Amount);
            if (!invoiceItem.Any()) return 0;
            return invoiceItem.Sum();
        }

        public void CreatePaymentReminderNotification(IList<FranchiseeInvoice> franchiseeInvoiceList, Franchisee franchisee)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(franchisee.Organization.Id) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(franchisee.Organization.Id).FirstOrDefault();
            var person = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(organizationRoleUser.Id);

            var model = new InvoicePaymentReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = person.FirstName + " " + person.LastName,
                Franchisee = organizationRoleUser.Organization.Name,
            };
            model.InvoiceDetailList = new List<PaymentViewModelForInvoice>();
            foreach (var item in franchiseeInvoiceList)
            {
                var adFund = GetSumOfInvoiceBasedOnType(item.Invoice, InvoiceItemType.AdFund);
                var royalty = GetSumOfInvoiceBasedOnType(item.Invoice, InvoiceItemType.RoyaltyFee);
                var amount = string.Format("{0:0.00}", item.Invoice.InvoiceItems.ToList().Sum(x => x.Amount));
                var royaltyAmount = string.Format("{0:0.00}", royalty);
                var adFundAmount = string.Format("{0:0.00}", adFund);
                var invoiceModel = new PaymentViewModelForInvoice { };
                invoiceModel.InvoiceId = item.InvoiceId;
                invoiceModel.DueDate = item.Invoice.DueDate.ToShortDateString();
                invoiceModel.GeneratedOn = item.Invoice.GeneratedOn.ToShortDateString();
                invoiceModel.AdFund = adFundAmount;
                invoiceModel.Royalty = royaltyAmount;
                invoiceModel.Amount = amount;
                model.InvoiceDetailList.Add(invoiceModel);
            }

            _notificationService.QueueUpNotificationEmail(NotificationTypes.PaymentReminder, model, organizationRoleUser.Id);
        }

        public void CreateSalesDataReminderNotification(SalesDataUpload salesData, DateTime startDate, DateTime endDate, long? paymentFrequencyId)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(salesData.Franchisee.Organization.Id) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(salesData.Franchisee.Organization.Id).FirstOrDefault();
            var person = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(organizationRoleUser.Id);

            var model = new SalesdataUploadReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = person.FirstName + " " + person.LastName,
                Franchisee = organizationRoleUser.Organization.Name,
            };
            var currentDate = _clock.UtcNow;
            model.DateRange = new List<DateRangeViewModel>();
            if (paymentFrequencyId == (long)PaymentFrequency.Weekly)
            {
                int weekCount = currentDate.Subtract(salesData.PeriodEndDate).Days / 7;
                for (int i = 0; i < weekCount; i++)
                {
                    var dateRangeModel = new DateRangeViewModel { };
                    dateRangeModel.StartDate = startDate.ToShortDateString();
                    dateRangeModel.EndDate = endDate.ToShortDateString();
                    model.DateRange.Add(dateRangeModel);
                    startDate = endDate.AddDays(1);
                    endDate = startDate.AddDays(6);
                }
            }
            if (paymentFrequencyId == null || paymentFrequencyId == (long)PaymentFrequency.Monthly)
            {
                int monthCount = (currentDate.Month - endDate.Month) + 12 * (currentDate.Year - endDate.Year);
                for (int i = 0; i < monthCount; i++)
                {
                    var dateRangeModel = new DateRangeViewModel { };
                    dateRangeModel.StartDate = startDate.ToShortDateString();
                    dateRangeModel.EndDate = endDate.ToShortDateString();
                    model.DateRange.Add(dateRangeModel);
                    startDate = endDate.Date.AddDays(1);
                    endDate = startDate.Date.AddMonths(1).AddDays(-1);
                }
            }
            if (model.DateRange.Count > 0)
            {
                _notificationService.QueueUpNotificationEmail(NotificationTypes.SalesDataUploadReminder, model, organizationRoleUser.Id);
            }
        }

        public void CreateLateFeeReminderNotification(InvoiceItem invoiceItem, long organizationId, long lateFeeTypeId, DateTime currentDate)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(organizationId) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(organizationId).FirstOrDefault();
            var person = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(organizationRoleUser.Id);

            var model = new LateFeeReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = person.FirstName + " " + person.LastName,
                Franchisee = organizationRoleUser.Organization.Name,
                LateFee = invoiceItem.LateFeeInvoiceItem.Amount,
                InvoiceId = invoiceItem.InvoiceId,
                Amount = invoiceItem.Amount,
                Description = invoiceItem.Description,
                ExpectedDate = currentDate.ToShortDateString(),
            };
            if (lateFeeTypeId == (long)LateFeeType.Royalty)
            {
                _notificationService.QueueUpNotificationEmail(NotificationTypes.LateFeeReminderForPayment, model, organizationRoleUser.Id);
            }
            if (lateFeeTypeId == (long)LateFeeType.SalesData)
            {
                model.StartDate = invoiceItem.LateFeeInvoiceItem.StartDate.ToShortDateString();
                model.EndDate = invoiceItem.LateFeeInvoiceItem.EndDate.ToShortDateString();
                _notificationService.QueueUpNotificationEmail(NotificationTypes.LateFeeReminderForSalesData, model, organizationRoleUser.Id);
            }
        }

        public void CreatePaymentConfirmationNotification(Invoice invoice, Payment payment, long organizationId)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(organizationId) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(organizationId).FirstOrDefault();
            var person = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(organizationRoleUser.Id);
            var payments = _franchiseeSalesPaymentService.CreatePaymentModelCollection(invoice.InvoicePayments);

            var adFund = GetSumOfInvoiceBasedOnType(invoice, InvoiceItemType.AdFund);
            var royalty = GetSumOfInvoiceBasedOnType(invoice, InvoiceItemType.RoyaltyFee);
            var amount = string.Format("{0:0.00}", invoice.InvoiceItems.ToList().Sum(x => x.Amount));
            var royaltyAmount = string.Format("{0:0.00}", royalty);
            var adFundAmount = string.Format("{0:0.00}", adFund);

            var model = new PaymentConfirmationNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = person.FirstName + " " + person.LastName,
                Franchisee = organizationRoleUser.Organization.Name,
                InvoiceId = invoice.Id,
                Amount = amount,
                PaymentDate = payment.Date.ToShortDateString(),
                Payments = payments,
                GeneratedOn = invoice.GeneratedOn.ToShortDateString(),
                DueDate = invoice.DueDate.ToShortDateString(),
                AdFund = adFundAmount,
                Royalty = royaltyAmount
            };
            _notificationService.QueueUpNotificationEmail(NotificationTypes.PaymentConfirmation, model, organizationRoleUser.Id);
        }

        public void CreateWeeklyNotification(File file, IEnumerable<FranchiseeInvoice> franchiseeInvoices, DateTime startDate, DateTime endDate, NotificationTypes type)
        {

            var personName = _settings.OwnerName;
            var currentWeekInvoices = franchiseeInvoices;
            var franchiseeWiseInvoices = franchiseeInvoices;
            var previousInvoices = franchiseeInvoices;
            var recipients = _settings.CCToMarketing;

            if (type == NotificationTypes.WeeklyUnpaidInvoiceNotification)
            {
                currentWeekInvoices = franchiseeInvoices.Where(x => x.Invoice.GeneratedOn >= startDate && x.Invoice.GeneratedOn <= endDate).
                                           OrderByDescending(x => x.FranchiseeId).ThenByDescending(x => x.Invoice.GeneratedOn);
                previousInvoices = franchiseeInvoices.Where(x => x.Invoice.GeneratedOn < startDate && x.Invoice.StatusId == (long)InvoiceStatus.Unpaid).
                                        OrderByDescending(x => x.FranchiseeId).ThenByDescending(x => x.Invoice.GeneratedOn);
                //recipients = _settings.UnpainInvoiceRecipients;
            }
            else if (type == NotificationTypes.WeeklyLateFeeNotification)
            {
                currentWeekInvoices = franchiseeInvoices.Where(x => x.Invoice.InvoiceItems.Any
                                        (y => y.LateFeeInvoiceItem != null && y.LateFeeInvoiceItem.GeneratedOn >= startDate && y.LateFeeInvoiceItem.GeneratedOn <= endDate))
                                        .OrderByDescending(x => x.FranchiseeId).ThenByDescending(x => x.Invoice.DueDate);
                previousInvoices = franchiseeInvoices.Where(x => x.Invoice.InvoiceItems.Any
                                        (y => y.LateFeeInvoiceItem != null && y.LateFeeInvoiceItem.GeneratedOn < startDate)
                                        && x.Invoice.StatusId == (long)InvoiceStatus.Unpaid)
                                        .OrderByDescending(x => x.FranchiseeId).ThenByDescending(x => x.Invoice.DueDate);
            }

            var model = new WeeklyNotificationListModel(_notificationModelFactory.CreateBaseDefault())
            {
                WeeklyCollection = currentWeekInvoices.Select(x => _reportFactory.CreateViewModelForNotification(x, startDate, endDate)).ToList(),
                PreviousCollection = previousInvoices.Select(x => _reportFactory.CreateViewModelForPreviousDate(x, startDate)).ToList(),
                FullName = personName,
                StartDate = startDate.ToString("MM-dd-yyyy"),
                EndDate = endDate.ToString("MM-dd-yyyy")
            };
            var resource = new NotificationResource { Resource = file, ResourceId = file.Id, IsNew = true };
            _notificationService.QueueUpNotificationEmail(type, model, _settings.CompanyName, _settings.FromEmail,
            recipients, _clock.UtcNow, null, new List<NotificationResource> { resource });
        }

        public ReviewAPIResponseModel SendEmailFeedbackRequest(string customerEmail, string customerName, Franchisee franchisee)
        {
            var response = new ReviewAPIResponseModel { };
            if (franchisee.Reviewpush == null || franchisee.Reviewpush.NewRp_ID != "")
            {
                response.errorMessage = string.Format("Invalid Business Id of {0} for customer {1}", franchisee.Organization.Name, customerName);
                return response;
            }
            string link = _settings.KioskLink;
            if (string.IsNullOrEmpty(link))
            {
                response.errorMessage = string.Format("Invalid URL!");
                return response;
            }
            var currentDate = _clock.UtcNow;
            var url = string.Format(link + franchisee.Reviewpush.NewRp_ID);
            var ownerName = franchisee.OwnerName;
            var model = new SendCustomerFeedbackNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                Link = url,
                Franchisee = string.IsNullOrEmpty(franchisee.DisplayName) ? franchisee.Organization.Name : franchisee.DisplayName,
                FullName = customerName,
                Owner = ownerName
            };
            _notificationService.QueueUpNotificationEmail(NotificationTypes.CustomerFeedbackRequest, model, _settings.CompanyName, _settings.FromEmail,
            customerEmail, currentDate, null);
            response.IsQueued = true;
            return response;
        }

        public void CreateMonthlyNotificationModel(File file, DateTime startDate, DateTime endDate, NotificationTypes notificationType, File file2 = null)
        {
            var currentDate = _clock.UtcNow;
            var personName = _settings.OwnerName;
            var recipients = string.Empty;
            var resource2 = new NotificationResource();
            recipients = _settings.CCToAdmin;
            var model = new MonthlyNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                StartDate = startDate.ToShortDateString(),
                EndDate = endDate.ToShortDateString(),
                FullName = personName
            };
            var resource = new NotificationResource { Resource = file, ResourceId = file.Id, IsNew = true };
            if ((long)notificationType == (long)NotificationTypes.MonthlyMailChimpReport)
            {
                resource2 = new NotificationResource { Resource = file2, ResourceId = file.Id, IsNew = true };
                _notificationService.QueueUpNotificationEmail(notificationType, model, _settings.CompanyName, _settings.FromEmail,
                        recipients, currentDate, null, new List<NotificationResource> { resource, resource2 }, null);
            }
            else if ((long)notificationType == (long)NotificationTypes.ListCustomerMonthlyNotification)
            {
                _notificationService.QueueUpNotificationEmail(notificationType, model, _settings.CompanyName, _settings.FromEmail,
                        recipients, currentDate, null, new List<NotificationResource> { resource }, null);
            }
        }

        public void CreateReviewSystemRecordNotification(File file, DateTime startDate, DateTime endDate, long organizationId)
        {
            var currentDate = _clock.UtcNow;
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(organizationId) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(organizationId).FirstOrDefault();
            var person = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(organizationRoleUser.Id);
            var toEMail = _settings.CCToAdmin;
            var model = new SendReviewSystemRecordNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = person.FirstName + " " + person.LastName,
                Franchisee = organizationRoleUser.Organization.Name,
                StartDate = startDate.ToShortDateString(),
                EndDate = endDate.ToShortDateString(),

            };
            toEMail = organizationRoleUser.Organization.Email;
            var resource = new NotificationResource { Resource = file, ResourceId = file.Id, IsNew = true };
            _notificationService.QueueUpNotificationEmail(NotificationTypes.MonthlyReviewNotification, model, _settings.CompanyName, _settings.FromEmail,
            toEMail, currentDate, null, new List<NotificationResource> { resource });
        }

        public void CreateSalesUploadNotification(File file, DateTime startDate, DateTime endDate)
        {
            var personName = _settings.CCToMarketing;

            var model = new MonthlySalesUploadNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = personName,
            };
            var resource = new NotificationResource { Resource = file, ResourceId = file.Id, IsNew = true };
            _notificationService.QueueUpNotificationEmail(NotificationTypes.MonthlySalesUploadNotification, model, _settings.CompanyName, _settings.FromEmail,
            _settings.CCToMarketing, _clock.UtcNow, null, new List<NotificationResource> { resource });
        }

        public void CreateAnnualUploadNotification(AnnualSalesDataUpload annualFileUpload)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(annualFileUpload.Franchisee.Organization.Id) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");

            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(annualFileUpload.Franchisee.Organization.Id).FirstOrDefault();
            var person = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(organizationRoleUser.Id);
            var recipients = _settings.AuditRecipients;
            var currentDate = _clock.UtcNow;
            var adminName = _settings.OwnerName;

            var model = new AnnualAuditNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                AdminName = adminName,
                FullName = person.FirstName + " " + person.LastName,
                Franchisee = organizationRoleUser.Organization.Name,
                Year = annualFileUpload.PeriodStartDate.Year
            };

            if (annualFileUpload.StatusId == (long)SalesDataUploadStatus.Parsed)
            {
                //Notification to Franchisee
                _notificationService.QueueUpNotificationEmail(NotificationTypes.AnnualUploadParsedNotification, model, organizationRoleUser.Id);
                //Notification To Admin
                _notificationService.QueueUpNotificationEmail(NotificationTypes.AnnualUploadNotificationToAdmin, model, _settings.CompanyName, _settings.FromEmail,
            recipients, currentDate, null);
            }
            else if (annualFileUpload.StatusId == (long)SalesDataUploadStatus.Failed)
            {
                _notificationService.QueueUpNotificationEmail(NotificationTypes.AnnualUploadFailNotification, model, organizationRoleUser.Id);
            }
        }

        public void CreateReviewActionNotification(AnnualSalesDataUpload annualFileUpload, bool isAccept)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(annualFileUpload.Franchisee.Organization.Id) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");

            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(annualFileUpload.Franchisee.Organization.Id).FirstOrDefault();
            var person = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(organizationRoleUser.Id);
            var currentDate = _clock.UtcNow;

            var model = new AnnualAuditNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = person.FirstName + " " + person.LastName,
                Franchisee = organizationRoleUser.Organization.Name,
                Year = annualFileUpload.PeriodStartDate.Year
            };

            if (isAccept)
            {
                _notificationService.QueueUpNotificationEmail(NotificationTypes.AnnualUploadApproved, model, organizationRoleUser.Id);
            }
            else
            {
                _notificationService.QueueUpNotificationEmail(NotificationTypes.AnnualUploadRejected, model, organizationRoleUser.Id);
            }
        }

        public void CreateDocumentUploadNotification(string fileName, OrganizationRoleUser uploadedBy, Franchisee franchisee)
        {
            var currentDate = _clock.UtcNow;
            var personName = _settings.OwnerName;
            var recipients = _settings.CCToAdmin;
            var franchiseeName = uploadedBy.Organization.Name;
            var type = NotificationTypes.DocumentUploadNotification;
            long? orgRoleUserId = null;

            if (franchisee != null)
            {
                if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(franchisee.Id) == null)
                    throw new InvalidDataProvidedException("No Email recipient found.");
                var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(franchisee.Id).FirstOrDefault();
                var person = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(organizationRoleUser.Id);

                personName = person.Name.ToString();
                recipients = person.Email;
                franchiseeName = franchisee.Organization.Name;
                type = NotificationTypes.DocumentUploadNotificationToFranchisee;
                orgRoleUserId = organizationRoleUser.Id;
            }

            var model = new DocumentUploadNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = personName,
                UploadedBy = uploadedBy.Person.Name.ToString(),
                Email = uploadedBy.Person.Email,
                DocName = fileName,
                Franchisee = franchiseeName,
                Role = uploadedBy.Role.Name.ToString()
            };
            _notificationService.QueueUpNotificationEmail(type, model, _settings.CompanyName, _settings.FromEmail,
             recipients, currentDate, orgRoleUserId);
        }

        public void CreateDocumentExpiryNotification(FranchiseDocument doc)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(doc.Franchisee.Id) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(doc.Franchisee.Id).FirstOrDefault();
            var person = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(organizationRoleUser.Id);

            var model = new DocumentUploadNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FullName = person.Name.ToString(),
                DocName = doc.File.Name,
                Franchisee = doc.Franchisee.Organization.Name,
                ExpiryDate = doc.ExpiryDate != null ? doc.ExpiryDate.Value.ToShortDateString() : null
            };
            _notificationService.QueueUpNotificationEmail(NotificationTypes.DocumentExpiryNotification, model, organizationRoleUser.Id);
        }

        public void ScheduleReminderNotification(JobScheduler schedule, DateTime startDate, DateTime endDate, string encryptedData, NotificationTypes ntificationTypes)
        {
            string address = "";
            var phone = schedule.Job != null ? schedule.Job.JobCustomer.PhoneNumber : schedule.Estimate.JobCustomer.PhoneNumber;
            var techLists = new List<JobScheduler>();
            var estimateIds = new List<long>();
            string name = ApplicationManager.Settings.SiteRootUrl;
            var phoneNumber = "";
            string jobtype = "";
            List<TechListViewModel> techList = new List<TechListViewModel>();
            List<CustomerMailViewModel> oneDayTechLists = new List<CustomerMailViewModel>();
            string techListNames = "";
            var baseUrl = _settings.MediaRootPath + "\\Images\\";
            var baseUrlForEcryption = _settings.SiteRootUrl + "#/confirmation";
            bool isPicturedBelowVisible = false;
            string displayPicturedBelow = "none";
            DateTime timeNow = DateTime.Now;
            var tomorrowStartDate = _clock.ToUtc(timeNow.AddDays(1));
            var tomorrowEndDate = _clock.ToUtc(tomorrowStartDate.AddHours(23).AddMinutes(59));
            if (schedule.EstimateId != null && schedule.JobId == null)
            {
                jobtype = "ESTIMATE";
            }
            else
            {
                jobtype = "JOB";
            }

            string linkUrl = "";
            if (schedule.JobId != null)
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + schedule.JobId + "/edit/" + schedule.Id;
            }
            else
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + schedule.EstimateId + "/manage/" + schedule.Id;
            }

            string filename = MediaLocationHelper.GetMediaLocationForLogs().Path;
            DateTime tomorrowDate = DateTime.Now;
            var orgInfo = schedule.Franchisee.Organization;
            phoneNumber = (orgInfo.Phones != null && orgInfo.Phones.Any()) ? (orgInfo.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter)
                ? orgInfo.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault() : null) : null;

            if (phoneNumber == null)
            {
                phoneNumber = (orgInfo.Phones != null && orgInfo.Phones.Any()) ? (orgInfo.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? orgInfo.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            }
            if (schedule.EstimateId != null)
            {
                estimateIds = getParentEstimateIds(schedule);
                if (estimateIds.Count() == 0)
                {
                    estimateIds.Add(schedule.EstimateId.GetValueOrDefault());
                }
            }

            if (encryptedData != "")
            {
                techLists = (from jobscehuler in _jobSchedulerRepository.Table
                             where (schedule.JobId == null ? estimateIds.Contains(jobscehuler.EstimateId.Value) : jobscehuler.JobId == schedule.JobId) && jobscehuler.IsActive
                             && jobscehuler.StartDate >= tomorrowStartDate.Date && jobscehuler.StartDate <= tomorrowEndDate
                             select jobscehuler).AsEnumerable().ToList();
            }
            else
            {
                techLists = (from jobscehuler in _jobSchedulerRepository.Table
                             where (schedule.JobId == null ? estimateIds.Contains(jobscehuler.EstimateId.Value) : jobscehuler.JobId == schedule.JobId) && jobscehuler.IsActive
                             select jobscehuler).AsEnumerable().ToList();
            }

            techList = techLists.Select(jobscehuler => new TechListViewModel
            {
                FirstName = jobscehuler.OrganizationRoleUser.Person.FirstName,
                MiddleName = jobscehuler.OrganizationRoleUser.Person.MiddleName,
                LastName = jobscehuler.OrganizationRoleUser.Person.LastName,
                Role = jobscehuler.OrganizationRoleUser.Role.Name,
                fileId = jobscehuler.OrganizationRoleUser != null && jobscehuler.OrganizationRoleUser.Person.FileId != null ? jobscehuler.OrganizationRoleUser.Person.FileId : default(long),
                display = jobscehuler.OrganizationRoleUser.Person.FileId != null ? "display" : "none",
                StartDate = jobscehuler.StartDateTimeString.ToString("MM-dd-yyyy h:mm tt"),
                EndDate = jobscehuler.EndDateTimeString.ToString("MM-dd-yyyy h:mm tt"),
                Id = jobscehuler.Id
            }).AsEnumerable().Distinct().ToList();


            techList = techList.OrderBy(x => x.StartDate).ToList();

            List<long?> schedulerIds = techList.Select(x => x.Id).ToList();
            bool isCustomerAlreadySent = _jobReminderAuditRepository.Table.Any(x => (schedulerIds).Contains(x.JobSchedulerId));
            if ((techList.Count() == 0 || isCustomerAlreadySent) && encryptedData != "")
            {
                return;
            }
            for (int index = 0; index < techList.Count(); ++index)
            {
                techList[index].src = name + "/api/Application/File/GetFile/" + techList[index].fileId.Value;
                if (!techListNames.Contains(techList[index].FullName))
                    techListNames += techList[index].FullName + ", ";
                if (techList[index].fileId.Value != 0)
                {
                    displayPicturedBelow = "block";
                    isPicturedBelowVisible = true;
                }
                else
                {
                    displayPicturedBelow = "none";
                    isPicturedBelowVisible = false;
                }
            }

            if (isPicturedBelowVisible == false && techListNames != null)
            {
                int lastCommaIndex = techListNames.LastIndexOf(",", StringComparison.Ordinal);
                techListNames = techListNames.Substring(0, lastCommaIndex) + ".";
            }
            else
            {
                if (techListNames != null)
                {
                    int lastCommaIndex = techListNames.LastIndexOf(",", StringComparison.Ordinal);
                    techListNames = techListNames.Substring(0, lastCommaIndex);
                    if (encryptedData != "")
                    {
                        techListNames += ", (pictured below).";
                    }
                    else
                    {
                        techListNames += ".";
                    }
                }
            }

            var assigneeId = _jobSchedulerRepository.Table.Where(x => schedule.JobId != null ? x.JobId == schedule.JobId : x.EstimateId == schedule.EstimateId).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            string AssigneeNumber = assigneeData.Person.Phones.Count > 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            string AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            string encryptedDataUrl = baseUrlForEcryption + "/" + encryptedData + "/" + "confirmed";
            string cancleDataUrl = baseUrlForEcryption + "/" + encryptedData + "/" + "cancle";
            if (schedule.Job != null)
            {
                address = GetAddress(schedule.Job.JobCustomer.Address);

            }
            else
            {
                address = GetAddress(schedule.Estimate.JobCustomer.Address);
            }
            string techRepresentation = "";

            if (schedule.JobId != null)
            {
                if (techList.Count() == 1)
                {
                    techRepresentation = "Technician/SalesRep";
                }
                else
                {
                    techRepresentation = "Technicians/SalesReps";
                }
            }
            else
            {
                if (techList.Count() == 1)
                {
                    techRepresentation = "Technician/SalesRep";
                }
                else
                {
                    techRepresentation = "Technicians/SalesReps";
                }
            }

            // To Send Mail to Client for next Dayy Job
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                CustomerId = schedule.Job != null ? schedule.Job.JobCustomer.Id : schedule.Estimate.JobCustomer.Id,
                FullName = schedule.Job != null ? schedule.Job.JobCustomer.CustomerName : schedule.Estimate.JobCustomer.CustomerName,
                FranchiseeName = schedule.Franchisee.Organization.Name,
                Address = address, // get job customer address : //get estimae customer address,
                Email = schedule.JobId != null ? schedule.Job.JobCustomer.Email : schedule.Estimate.JobCustomer.Email,
                StartDate = schedule.StartDateTimeString.ToString("MM-dd-yyyy"),
                EndDate = schedule.EndDateTimeString.ToString(),
                OrganizationId = schedule.OrganizationRoleUser.OrganizationId,
                jobType = jobtype,
                Time = schedule.StartDateTimeString.ToShortTimeString(),
                EndTime = schedule.EndDateTimeString.ToShortTimeString(),
                AssigneePhone = phoneNumber != null ? phoneNumber : null,
                AssigneeName = AssigneeName,
                recipientMail = schedule.OrganizationRoleUser.Organization.Franchisee.ContactEmail,
                TechList = techList,
                fromMail = schedule.OrganizationRoleUser.Organization.Franchisee.ContactEmail,
                display = displayPicturedBelow,
                ConfirmUrl = encryptedDataUrl,
                CancleUrl = cancleDataUrl,
                jobTitle = schedule.Title,
                PhoneNumber = phone != null ? phone : null,
                LinkUrl = linkUrl,
                techDesignation = techRepresentation,
                Date = schedule.StartDate.ToString("MM-dd-yyyy")
            };
            model.techNames = techListNames;
            var currentDate = _clock.UtcNow;

            var dateRangeModel = new DateRangeViewModel { };
            var TechListModel = new TechListViewModel { };

            dateRangeModel.StartDate = startDate.ToShortDateString();
            dateRangeModel.EndDate = endDate.ToShortDateString();
            model.TechList = techList;

            if (model.OrganizationId == _settings.SEMIFranchiseeId)
                model.fromMail = _settings.SEMIFromEmail;

            _notificationService.QueueUpNotificationEmail(ntificationTypes, model, _settings.CompanyName, model.fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null, assigneeData.Person.Email);

            if (encryptedData != "")
                updateCustomerSchedulerReminderAudit(schedule, model, techList.Select(x => x.Id).ToList());
        }
        private List<long> getParentEstimateIds(JobScheduler scheduler)
        {
            var jobEstimate = _jobEstimateRepository.Table.ToList();
            List<long> parentIds = new List<long>();
            long? estimateId = default(long?);
            long? parentEstimateId = default(long?);
            estimateId = scheduler.EstimateId;
            while (parentEstimateId != 0)
            {
                var jobParentEstimate = jobEstimate.Where(x => x.Id == estimateId).FirstOrDefault();
                if (jobParentEstimate.ParentEstimateId != null)
                {
                    var jobSameEstimates = jobEstimate.Where(x => x.ParentEstimateId == jobParentEstimate.ParentEstimateId).ToList();

                    foreach (var jobSameEstimate in jobSameEstimates)
                    {
                        parentIds.Add(jobSameEstimate.Id);
                    }

                    parentEstimateId = jobParentEstimate.ParentEstimateId.GetValueOrDefault();
                    estimateId = jobParentEstimate.ParentEstimateId;
                    parentIds.Add(estimateId.GetValueOrDefault());
                }
                else
                {
                    var jobSameEstimates = jobEstimate.Where(x => x.ParentEstimateId == estimateId).ToList();

                    if (jobSameEstimates == null)
                    {
                        break;
                    }
                    foreach (var jobSameEstimate in jobSameEstimates)
                    {
                        parentIds.Add(jobSameEstimate.Id);
                    }

                    parentEstimateId = jobParentEstimate.ParentEstimateId.GetValueOrDefault();

                    estimateId = jobParentEstimate.ParentEstimateId;
                    if (estimateId != null)
                        parentIds.Add(estimateId.GetValueOrDefault());
                    else
                        parentIds.Add(scheduler.EstimateId.GetValueOrDefault());
                }

            }
            return parentIds;
        }
        void updateCustomerSchedulerReminderAudit(JobScheduler schedule, NewJobOrEstimateReminderNotificationModel model, List<long?> ids)
        {
            foreach (var id in ids)
            {
                var auditModel = new CustomerSchedulerReminderAudit()
                {
                    CustomerId = model.CustomerId,
                    CreatedOn = _clock.UtcNow,
                    JobCustomer = _jobCustomerRepository.Table.Where(x => x.Id == model.CustomerId).FirstOrDefault(),
                    IsNew = true,
                    JobSchedulerId = id,
                    JobId = schedule.JobId == null ? null : schedule.JobId,
                    EstimateId = schedule.JobId == null ? schedule.EstimateId : null
                };
                _jobReminderAuditRepository.Save(auditModel);
            }
        }
        public void NewJobOrEstimateReminderNotificationtoTech(JobEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            string isDisplayVisible = "none";
            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault() : null) : null;
            if (phoneNumber == null)
            {
                phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            }
            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }
            string linkUrl = "";
            if (franchiseeData.jobType.ToLower() == "job")
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.JobId + "/edit/" + franchiseeData.Id;
            }
            else
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.EstimateId + "/manage/" + franchiseeData.Id;
            }
            // To Send Mail to Tech for new Job
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = organizationData.Organization.Name,
                Address = franchiseeData.JobCustomer.Address.FullAddressString() + ", " + franchiseeData.JobCustomer.Address.City + " " + franchiseeData.JobCustomer.Address.ZipCode,
                Email = organizationData.Person.Email,
                TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                //StartDate = _clock.ToLocal(franchiseeData.StartDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                //EndDate = _clock.ToLocal(franchiseeData.EndDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                StartDate = (franchiseeData.StartDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                EndDate = (franchiseeData.EndDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                OrganizationId = organizationData.OrganizationId,
                jobTitle = franchiseeData.Title,
                FullName = franchiseeData.JobCustomer.CustomerName,
                jobType = franchiseeData.jobType,
                jobTypeName = franchiseeData.jobTypeName,
                Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                AssigneeName = franchiseeData.AssigneeName,
                AssigneePhone = phoneNumber,
                recipientMail = organizationData.Organization.Email,
                fromMail = organizationData.Organization.Franchisee.ContactEmail,
                Description = franchiseeData.Description,
                IsDisplayVisible = isDisplayVisible,
                LinkUrl = linkUrl
                //Subject= franchiseeData.Subject
            };

            var currentDate = _clock.UtcNow;
            //model.DateRange = new List<DateRangeViewModel>();

            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.fromMail = _settings.SEMIFromEmail;
            }
            //model.DateRange.Add(dateRangeModel);
            //_notificationService.QueueUpNotificationEmail(NotificationTypes.NewJobNotificationToTech, model, _settings.CompanyName, _settings.CCToAdmin, model.Email, _clock.UtcNow, model.OrganizationId, null);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.NewJobNotificationToTech, model, _settings.CompanyName, model.fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }
        public void CancelJobOrEstimateReminderNotificationtoTechForDeleteButton(JobScheduler jobscheduler, string AssigneeName, string AssigneePhone, bool isFromEstimate)
        {
            string isDisplayVisible = "none";
            string descriptionJobEstimate = null;
            //To Send Mail to Tech for Job Cancellation
            var phoneNumber = (jobscheduler.OrganizationRoleUser.Organization.Phones != null && jobscheduler.OrganizationRoleUser.Organization.Phones.Any()) ? (jobscheduler.OrganizationRoleUser.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? jobscheduler.OrganizationRoleUser.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;

            if (jobscheduler.Job != null)
            {
                descriptionJobEstimate = jobscheduler.Job.Description;
            }
            else if (jobscheduler.Estimate != null)
            {
                descriptionJobEstimate = jobscheduler.Estimate.Description;
            }
            if (descriptionJobEstimate != null)
            {
                isDisplayVisible = "block";
            }
            string linkUrl = "";

            linkUrl = isFromEstimate ?
                _settings.SiteRootUrl + "/#/scheduler/" + jobscheduler.EstimateId + "/manage/" + jobscheduler.Id :
                 _settings.SiteRootUrl + "/#/scheduler/" + jobscheduler.JobId + "/edit/" + jobscheduler.Id;

            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = jobscheduler.Franchisee.Organization.Name,
                Address = jobscheduler.Job != null ? jobscheduler.Job.JobCustomer.Address.AddressLine1 : jobscheduler.Estimate.JobCustomer.Address.AddressLine1,
                Email = jobscheduler.OrganizationRoleUser.Person.Email,
                TechName = jobscheduler.OrganizationRoleUser.Person.FirstName + " " + jobscheduler.OrganizationRoleUser.Person.MiddleName + " " + jobscheduler.OrganizationRoleUser.Person.LastName,
                PhoneNumber = jobscheduler.Job != null ? jobscheduler.Job.JobCustomer.PhoneNumber : jobscheduler.Estimate.JobCustomer.PhoneNumber,
                StartDate = jobscheduler.Job != null ? _clock.ToLocal(jobscheduler.Job.StartDate).ToString("MM-dd-yyyy") : _clock.ToLocal(jobscheduler.StartDate).ToString("MM-dd-yyyy"),
                EndDate = jobscheduler.Job != null ? _clock.ToLocal(jobscheduler.Job.EndDate).ToString("MM-dd-yyyy") : _clock.ToLocal(jobscheduler.EndDate).ToString("MM-dd-yyyy"),
                OrganizationId = jobscheduler.OrganizationRoleUser.OrganizationId,
                jobTitle = jobscheduler.Title,
                FullName = jobscheduler.Job != null ? jobscheduler.Job.JobCustomer.CustomerName : jobscheduler.Estimate.JobCustomer.CustomerName,
                jobType = jobscheduler.Job != null ? "Job" : "Estimate",
                jobTypeName = jobscheduler.Job != null ? "a Job" : "Estimate",
                Time = jobscheduler.Job != null ? _clock.ToLocal(jobscheduler.Job.StartDate).ToShortTimeString() : _clock.ToLocal(jobscheduler.StartDate).ToShortTimeString(),
                AssigneePhone = phoneNumber,
                AssigneeName = AssigneeName,
                recipientMail = jobscheduler.Franchisee.ContactEmail,
                Description = descriptionJobEstimate,
                IsDisplayVisible = isDisplayVisible,
                LinkUrl = linkUrl
            };

            var currentDate = _clock.UtcNow;
            //model.DateRange = new List<DateRangeViewModel>();

            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.recipientMail = _settings.SEMIFromEmail;
            }

            //model.DateRange.Add(dateRangeModel);
            //_notificationService.QueueUpNotificationEmail(NotificationTypes.CancleJobNotificationToTech, model, _settings.CompanyName, _settings.CCToAdmin, model.Email, _clock.UtcNow, model.OrganizationId, null);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.DeletionJobNotificationToTech, model, _settings.CompanyName, model.recipientMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }
        public void CancelJobOrEstimateReminderNotificationtoTech(JobEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            string isDisplayVisible = "none";
            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter)
               ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault() : null) : null;
            if (phoneNumber == null)
            {
                phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            }
            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }

            string linkUrl = "";
            if (franchiseeData.jobType.ToLower() == "job")
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.JobId + "/edit/" + franchiseeData.Id;
            }
            else
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.EstimateId + "/manage/" + franchiseeData.Id;
            }
            // To Send Mail to Tech for new Job
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = organizationData.Organization.Name,
                Address = franchiseeData.JobCustomer.Address.FullAddressString() + ", " + franchiseeData.JobCustomer.Address.City + " " + franchiseeData.JobCustomer.Address.ZipCode,
                Email = organizationData.Person.Email,
                TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                //PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                PhoneNumber = phoneNumber,
                //StartDate = _clock.ToLocal(franchiseeData.StartDate).ToString("MM-dd-yyyy"),
                //EndDate = _clock.ToLocal(franchiseeData.EndDate).ToString(),
                StartDate = (franchiseeData.ActualStartDateString).ToString("MM-dd-yyyy"),
                EndDate = (franchiseeData.ActualEndDateString).ToString(),

                OrganizationId = organizationData.OrganizationId,
                jobTitle = franchiseeData.Title,
                FullName = franchiseeData.JobCustomer.CustomerName,
                jobType = franchiseeData.jobType,
                jobTypeName = franchiseeData.jobTypeName,
                //Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                Time = (franchiseeData.ActualStartDateString).ToShortTimeString(),
                AssigneeName = franchiseeData.AssigneeName,
                AssigneePhone = phoneNumber,
                recipientMail = organizationData.Organization.Franchisee.ContactEmail,
                fromMail = organizationData.Organization.Franchisee.ContactEmail,
                Description = franchiseeData.Description,
                IsDisplayVisible = isDisplayVisible,
                LinkUrl = linkUrl
                //AssigneeName= organizationData.
            };
            var currentDate = _clock.UtcNow;
            //model.DateRange = new List<DateRangeViewModel>();

            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.fromMail = _settings.SEMIFromEmail;
            }
            //model.DateRange.Add(dateRangeModel);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.CancleJobNotificationToTech, model, _settings.CompanyName, model.fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
            //_notificationService.QueueUpNotificationEmail(NotificationTypes.CancleJobNotificationToTech, model, _settings.CompanyName, _settings.CCToAdmin, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }
        public void UpdateJobOrEstimateReminderNotificationtoTech(JobEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            string isDisplayVisible = "none";
            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;

            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }

            string linkUrl = "";
            if (franchiseeData.jobType.ToLower() == "job")
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.JobId + "/edit/" + franchiseeData.Id;
            }
            else
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.EstimateId + "/manage/" + franchiseeData.Id;
            }
            // To Send Mail to Tech for Job or Estimate Updation
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = organizationData.Organization.Name,
                Address = franchiseeData.JobCustomer.Address.AddressLine1 + " " + franchiseeData.JobCustomer.Address.AddressLine2 + ", " + franchiseeData.JobCustomer.Address.City + " " + franchiseeData.JobCustomer.Address.ZipCode,
                Email = organizationData.Person.Email,
                TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                //PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                //StartDate = _clock.ToLocal(franchiseeData.StartDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                //EndDate = _clock.ToLocal(franchiseeData.EndDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                StartDate = franchiseeData.ActualStartDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                EndDate = franchiseeData.ActualEndDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                OrganizationId = organizationData.OrganizationId,
                jobTitle = franchiseeData.Title,
                FullName = franchiseeData.JobCustomer.CustomerName,
                jobType = franchiseeData.jobType,
                jobTypeName = franchiseeData.jobTypeName,
                //Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                //EndTime = _clock.ToLocal(franchiseeData.EndDate).ToShortTimeString(),
                Time = franchiseeData.ActualStartDateString.ToShortTimeString(),
                EndTime = franchiseeData.ActualEndDateString.ToShortTimeString(),
                AssigneeName = franchiseeData.AssigneeName,
                AssigneePhone = phoneNumber,
                recipientMail = organizationData.Organization.Franchisee.ContactEmail,
                Description = franchiseeData.Description,
                IsDisplayVisible = isDisplayVisible,
                LinkUrl = linkUrl
            };
            var currentDate = _clock.UtcNow;
            //model.DateRange = new List<DateRangeViewModel>();

            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.recipientMail = _settings.SEMIFromEmail;
            }
            //model.DateRange.Add(dateRangeModel);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.UpdateJobNotificationToTech, model, _settings.CompanyName, model.recipientMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }
        public void NewJobOrEstimateReminderNotificationtoTechForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            string isDisplayVisible = "none";
            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter)
              ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault() : null) : null;
            if (phoneNumber == null)
            {
                phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            }
            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }
            string linkUrl = "";

            linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.Estimateid + "/manage/" + franchiseeData.SchedulerId;

            // To Send Mail to Tech for new Job
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = organizationData.Organization.Name,
                Address = franchiseeData.JobCustomer.Address.FullAddressString() + ", " + franchiseeData.JobCustomer.Address.City + " " + franchiseeData.JobCustomer.Address.ZipCode,
                Email = organizationData.Person.Email,
                TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                //PhoneNumber = phoneNumber,
                //StartDate = _clock.ToLocal(franchiseeData.StartDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                //EndDate = _clock.ToLocal(franchiseeData.EndDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                StartDate = franchiseeData.ActualStartDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                EndDate = franchiseeData.ActualEndDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                OrganizationId = organizationData.OrganizationId,
                jobTitle = franchiseeData.Title,
                FullName = franchiseeData.JobCustomer.CustomerName,
                jobType = franchiseeData.JobType,
                jobTypeName = franchiseeData.jobTypeName,
                //Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                Time = franchiseeData.ActualStartDateString.ToShortTimeString(),
                AssigneeName = franchiseeData.AssigneeName,
                AssigneePhone = phoneNumber,
                recipientMail = organizationData.Organization.Email,
                fromMail = organizationData.Organization.Franchisee.ContactEmail,
                Description = franchiseeData.Description,
                IsDisplayVisible = isDisplayVisible,
                LinkUrl = linkUrl
            };
            var currentDate = _clock.UtcNow;
            //model.DateRange = new List<DateRangeViewModel>();

            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.fromMail = _settings.SEMIFromEmail;
            }
            //model.DateRange.Add(dateRangeModel);
            //_notificationService.QueueUpNotificationEmail(NotificationTypes.NewJobNotificationToTech, model, _settings.CompanyName, _settings.CCToAdmin, model.Email, _clock.UtcNow, model.OrganizationId, null);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.NewJobNotificationToTech, model, _settings.CompanyName, model.fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }
        public void CancelJobOrEstimateReminderNotificationtoTechForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            string isDisplayVisible = "none";
            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter)
              ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault() : null) : null;
            if (phoneNumber == null)
            {
                phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            }
            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }

            string linkUrl = "";

            linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.Estimateid + "/manage/" + franchiseeData.SchedulerId;
            // To Send Mail to Tech for new Job

            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = organizationData.Organization.Name,
                Address = franchiseeData.JobCustomer.Address.FullAddressString() + ", " + franchiseeData.JobCustomer.Address.City + " " + franchiseeData.JobCustomer.Address.ZipCode,
                Email = organizationData.Person.Email,
                TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                //PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                PhoneNumber = phoneNumber,
                //StartDate = _clock.ToLocal(franchiseeData.StartDate).ToString("MM-dd-yyyy"),
                //EndDate = _clock.ToLocal(franchiseeData.EndDate).ToString(),

                StartDate = franchiseeData.ActualStartDateString.ToString("MM-dd-yyyy"),
                EndDate = franchiseeData.ActualEndDateString.ToString(),
                OrganizationId = organizationData.OrganizationId,
                jobTitle = franchiseeData.Title,
                FullName = franchiseeData.JobCustomer.CustomerName,
                jobType = franchiseeData.JobType,
                jobTypeName = franchiseeData.jobTypeName,
                //Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                Time = franchiseeData.ActualStartDateString.ToShortTimeString(),
                AssigneeName = franchiseeData.AssigneeName,
                AssigneePhone = phoneNumber,
                EndTime = _clock.ToLocal(franchiseeData.EndDate).ToShortTimeString(),
                recipientMail = organizationData.Organization.Franchisee.ContactEmail,
                Description = franchiseeData.Description,
                IsDisplayVisible = isDisplayVisible,
                LinkUrl = linkUrl
            };
            var currentDate = _clock.UtcNow;
            //model.DateRange = new List<DateRangeViewModel>();

            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.recipientMail = _settings.SEMIFromEmail;
            }
            //model.DateRange.Add(dateRangeModel);
            //_notificationService.QueueUpNotificationEmail(NotificationTypes.CancleJobNotificationToTech, model, _settings.CompanyName, _settings.CCToAdmin, model.Email, _clock.UtcNow, model.OrganizationId, null);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.CancleJobNotificationToTech, model, _settings.CompanyName, model.recipientMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }
        public void UpdateJobOrEstimateReminderNotificationtoTechForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            string isDisplayVisible = "none";
            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }
            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter)
              ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault() : null) : null;
            if (phoneNumber == null)
            {
                phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            }
            string linkUrl = "";

            linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.Estimateid + "/manage/" + franchiseeData.SchedulerId;
            // To Send Mail to Tech for new Estimate
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = organizationData.Organization.Name,
                Address = franchiseeData.JobCustomer.Address.FullAddressString() + "," + franchiseeData.JobCustomer.Address.City + " " + franchiseeData.JobCustomer.Address.ZipCode,
                Email = organizationData.Person.Email,
                TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                //PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                //StartDate = _clock.ToLocal(franchiseeData.StartDate).ToString("MM-d-yyyy h:mm:ss tt"),
                //EndDate = _clock.ToLocal(franchiseeData.EndDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                StartDate = franchiseeData.ActualStartDateString.ToString("MM-d-yyyy h:mm:ss tt"),
                EndDate = franchiseeData.ActualEndDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                OrganizationId = organizationData.OrganizationId,
                jobTitle = franchiseeData.Title,
                FullName = franchiseeData.JobCustomer.CustomerName,
                jobType = franchiseeData.JobType,
                jobTypeName = franchiseeData.jobTypeName,
                //Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                //EndTime = _clock.ToLocal(franchiseeData.EndDate).ToShortTimeString(),
                Time = franchiseeData.ActualStartDateString.ToShortTimeString(),
                EndTime = franchiseeData.ActualEndDateString.ToShortTimeString(),
                AssigneeName = franchiseeData.AssigneeName,
                AssigneePhone = phoneNumber,
                recipientMail = organizationData.Organization.Franchisee.ContactEmail,
                Description = franchiseeData.Description,
                IsDisplayVisible = isDisplayVisible,
                LinkUrl = linkUrl
            };

            var currentDate = _clock.UtcNow;
            //model.DateRange = new List<DateRangeViewModel>();

            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.recipientMail = _settings.SEMIFromEmail;
            }
            //model.DateRange.Add(dateRangeModel);
            //_notificationService.QueueUpNotificationEmail(NotificationTypes.UpdateJobNotificationToTech, model, _settings.CompanyName, _settings.CCToAdmin, model.Email, _clock.UtcNow, model.OrganizationId, null);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.UpdateJobNotificationToTech, model, _settings.CompanyName, model.recipientMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }
        public void ScheduleReminderNotificationToUser(JobScheduler schedule, DateTime startDate, DateTime endDate)
        {
            var phoneNumber = (schedule.OrganizationRoleUser.Organization.Phones != null && schedule.OrganizationRoleUser.Organization.Phones.Any()) ? (schedule.OrganizationRoleUser.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? schedule.OrganizationRoleUser.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            string name = ApplicationManager.Settings.SiteRootUrl;
            string jobtype = "";
            List<TechListViewModel> techList = new List<TechListViewModel>();
            List<String> display = new List<String>();
            var baseUrl = _settings.MediaRootPath + "\\Images\\";
            if (schedule.EstimateId != null)
            {
                jobtype = "ESTIMATE";
            }
            else
            {
                jobtype = "JOB";
            }


            string filename = MediaLocationHelper.GetMediaLocationForLogs().Path;

            var phone = (from cust in _organizationRoleUserRepository.Table
                         where cust.Id == schedule.Franchisee.Id
                         select new
                         {
                             TechnicianNumber = cust.Person.Phones.Select(x => x.Number),
                         }).FirstOrDefault();

            techList = (from jobscehuler in _jobSchedulerRepository.Table
                        where ((schedule.JobId == null ? jobscehuler.EstimateId == schedule.EstimateId : jobscehuler.JobId == schedule.JobId))
                        select new TechListViewModel
                        {
                            FirstName = jobscehuler.OrganizationRoleUser.Person.FirstName,
                            MiddleName = jobscehuler.OrganizationRoleUser.Person.MiddleName,
                            LastName = jobscehuler.OrganizationRoleUser.Person.LastName,
                            Role = jobscehuler.OrganizationRoleUser.Role.Name,
                            fileId = jobscehuler.OrganizationRoleUser.Person.FileId != null ? jobscehuler.OrganizationRoleUser.Person.FileId : default(long),
                        }).ToList();

            for (int index = 0; index < techList.Count(); ++index)
            {

                techList[index].src = name + "/api/Application/File/GetFile/" + techList[index].fileId.Value;
            }

            // To Send Mail to Client for next Dayy Job
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                CustomerId = schedule.Job != null ? schedule.Job.JobCustomer.Id : schedule.Estimate.JobCustomer.Id,
                FullName = schedule.Job != null ? schedule.Job.JobCustomer.CustomerName : schedule.Estimate.JobCustomer.CustomerName,
                FranchiseeName = schedule.Franchisee.Organization.Name,
                Address = schedule.JobId != null ? schedule.Job.JobCustomer.Address.AddressLine1 : schedule.Estimate.JobCustomer.Address.AddressLine1, // get job customer address : //get estimae customer address,
                Email = schedule.JobId != null ? schedule.Job.JobCustomer.Email : schedule.Estimate.JobCustomer.Email,
                //PhoneNumber = phone != null ? phone.TechnicianNumber.FirstOrDefault() : "",
                PhoneNumber = phoneNumber,
                //StartDate = _clock.ToLocal(schedule.StartDate).ToString(),
                //EndDate = _clock.ToLocal(schedule.EndDate).ToString(),

                StartDate = schedule.ActualStartDate.ToString(),
                EndDate = schedule.ActualEndDate.ToString(),

                OrganizationId = schedule.OrganizationRoleUser.OrganizationId,
                jobType = jobtype,
                //EndTime = _clock.ToLocal(schedule.EndDate).ToShortTimeString(),

                EndTime = schedule.ActualEndDate.ToShortTimeString(),
                recipientMail = schedule.Franchisee.ContactEmail
            };
            var currentDate = _clock.UtcNow;
            //model.DateRange = new List<DateRangeViewModel>();

            var dateRangeModel = new DateRangeViewModel { };
            var TechListModel = new TechListViewModel { };

            dateRangeModel.StartDate = startDate.ToShortDateString();
            dateRangeModel.EndDate = endDate.ToShortDateString();
            //model.DateRange.Add(dateRangeModel);
            model.TechList = techList;
            long? franchiseeId = model.OrganizationId;

            //_notificationService.QueueUpNotificationEmail(NotificationTypes.NewJobNotificationToUser, model, _settings.CompanyName, _settings.CCToAdmin, model.Email, _clock.UtcNow, franchiseeId, null);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.NewJobNotificationToUser, model, _settings.CompanyName, model.recipientMail, model.Email, _clock.UtcNow, franchiseeId, null);
            var auditModel = new CustomerSchedulerReminderAudit()
            {
                CustomerId = model.CustomerId,
                CreatedOn = currentDate,
                JobCustomer = _jobCustomerRepository.Table.Where(x => x.Id == model.CustomerId).FirstOrDefault(),
                IsNew = true,
                JobSchedulerId = schedule.Id

            };
            _jobReminderAuditRepository.Save(auditModel);
        }
        public void ScheduleReminderNotificationToUserOnDay(JobScheduler schedule, DateTime startDate, DateTime endDate)
        {
            var phoneNumber = (schedule.OrganizationRoleUser.Organization.Phones != null && schedule.OrganizationRoleUser.Organization.Phones.Any()) ? (schedule.OrganizationRoleUser.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? schedule.OrganizationRoleUser.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            string name = ApplicationManager.Settings.SiteRootUrl;
            string jobtype = "";
            List<TechListViewModel> techList = new List<TechListViewModel>();
            if (schedule.EstimateId != null)
            {
                jobtype = "ESTIMATE";
            }
            else
            {
                jobtype = "JOB";
            }


            var assigneeId = _jobSchedulerRepository.Table.Where(x => schedule.JobId != null ? x.JobId == schedule.JobId : x.EstimateId == schedule.EstimateId).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            string AssigneeNumber = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            string AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;

            // To Send Mail to Tech or Sales on Dayy
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                CustomerId = schedule.Job != null ? schedule.Job.JobCustomer.Id : schedule.Estimate.JobCustomer.Id,
                FullName = schedule.OrganizationRoleUser.Person.FirstName + " " + schedule.OrganizationRoleUser.Person.MiddleName + " " + schedule.OrganizationRoleUser.Person.LastName,
                FranchiseeName = schedule.Franchisee.Organization.Name,
                Address = schedule.JobId != null ? schedule.Job.JobCustomer.Address.AddressLine1 : schedule.Estimate.JobCustomer.Address.AddressLine1, // get job customer address : //get estimae customer address,
                Email = schedule.Job != null ? schedule.Job.JobCustomer.Email : schedule.Estimate.JobCustomer.Email,
                //PhoneNumber = phone != null ? phone.TechnicianNumber.FirstOrDefault() : "",
                PhoneNumber = phoneNumber,
                //StartDate = _clock.ToLocal(schedule.StartDate, schedule.Offset.GetValueOrDefault()).ToString("MM-dd-yyyy"),
                //EndDate = _clock.ToLocal(schedule.EndDate, schedule.Offset.GetValueOrDefault()).ToString(),

                StartDate = schedule.ActualStartDate.ToString("MM-dd-yyyy"),
                EndDate = schedule.ActualEndDate.ToString(),
                OrganizationId = schedule.OrganizationRoleUser.OrganizationId,
                jobType = jobtype,
                TechName = schedule.Job != null ? schedule.Job.JobCustomer.CustomerName : schedule.Estimate.JobCustomer.CustomerName,
                //Time = _clock.ToLocal(schedule.StartDate, schedule.Offset.GetValueOrDefault()).ToShortTimeString(),

                Time = schedule.ActualStartDate.ToShortTimeString(),
                jobTypeName = jobtype,
                AssigneeName = AssigneeName,
                AssigneePhone = phoneNumber,
                //EndTime = _clock.ToLocal(schedule.EndDate, schedule.Offset.GetValueOrDefault()).ToShortTimeString(),
                EndTime = schedule.ActualEndDate.ToShortTimeString(),
                CustomerPhoneNumber = schedule.Job != null ? schedule.Job.JobCustomer.PhoneNumber : schedule.Estimate.JobCustomer.PhoneNumber,
                UserEmail = schedule.OrganizationRoleUser.Person.Email,
                AssigneeEmail = schedule.Franchisee.ContactEmail
            };
            var currentDate = _clock.UtcNow;
            var dateRangeModel = new DateRangeViewModel { };
            var TechListModel = new TechListViewModel { };
            dateRangeModel.StartDate = startDate.ToShortDateString();
            dateRangeModel.EndDate = endDate.ToShortDateString();
            model.TechList = techList;
            long? franchiseeId = model.OrganizationId;
            _notificationService.QueueUpNotificationEmail(NotificationTypes.NewJobNotificationToUserOnDay, model, _settings.CompanyName, model.AssigneeEmail, model.UserEmail, _clock.UtcNow, franchiseeId, null);
            
            var auditModel = new TechAndSalesSchedulerReminder()
            {
                CustomerId = model.CustomerId,
                CreatedOn = currentDate,
                JobCustomer = _jobCustomerRepository.Table.Where(x => x.Id == model.CustomerId).FirstOrDefault(),
                IsNew = true,
                JobSchedulerId = schedule.Id
            };
            _techAndSalesSchedulerRepository.Save(auditModel);
        }
        public void NewJobOrEstimateReminderNotificationtoTechForRescheduled(JobEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            string isDisplayVisible = "none";
            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
               ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;

            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }

            string linkUrl = "";
            if (franchiseeData.jobType.ToLower() == "job")
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.JobId + "/edit/" + franchiseeData.Id;
            }
            else
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.EstimateId + "/manage/" + franchiseeData.Id;
            }

            // To Send Mail to Tech for Rescheduling Job
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = organizationData.Organization.Name,
                Address = franchiseeData.JobCustomer.Address.AddressLine1,
                Email = organizationData.Person.Email,
                TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                //PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                PhoneNumber = phoneNumber,
                //StartDate = _clock.ToLocal(franchiseeData.StartDate).ToString("MM-dd-yyyy"),
                //EndDate = _clock.ToLocal(franchiseeData.EndDate).ToString(),
                StartDate = franchiseeData.ActualStartDateString.ToString(),
                EndDate = franchiseeData.ActualEndDateString.ToString(),
                OrganizationId = organizationData.OrganizationId,
                jobTitle = franchiseeData.Title,
                FullName = franchiseeData.JobCustomer.CustomerName,
                jobType = franchiseeData.jobType,
                jobTypeName = franchiseeData.jobTypeName,
                //Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                Time = franchiseeData.ActualStartDateString.ToShortTimeString(),
                AssigneeName = franchiseeData.AssigneeName,
                AssigneePhone = phoneNumber,
                AssigneeEmail = organizationData.Organization.Email,
                fromMail = organizationData.Organization.Franchisee.ContactEmail,
                Description = franchiseeData.Description,
                IsDisplayVisible = isDisplayVisible,
                linkUrl = linkUrl

            };
            var currentDate = _clock.UtcNow;
            //model.DateRange = new List<DateRangeViewModel>();

            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.fromMail = _settings.SEMIFromEmail;
            }
            //model.DateRange.Add(dateRangeModel);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.NewJobNotificationToUserReassigned, model, _settings.CompanyName, model.fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }

        public void NewJobOrEstimateReminderNotificationtoTechForRescheduledForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            string isDisplayVisible = "none";
            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter)
               ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault() : null) : null;
            if (phoneNumber == null)
            {
                phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            }
            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }
            string linkUrl = "";

            linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.Estimateid + "/manage/" + franchiseeData.SchedulerId;
            // To Send Mail to Tech for Rescheduling Job
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = organizationData.Organization.Name,
                Address = franchiseeData.JobCustomer.Address.FullAddressString(),
                Email = organizationData.Person.Email,
                TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                //PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                PhoneNumber = phoneNumber,
                //StartDate = _clock.ToLocal(franchiseeData.StartDate).ToString("MM-dd-yyyy"),
                //EndDate = _clock.ToLocal(franchiseeData.EndDate).ToString(),
                StartDate = franchiseeData.ActualStartDateString.ToString("MM-dd-yyyy"),
                EndDate = franchiseeData.ActualEndDateString.ToString(),
                OrganizationId = organizationData.OrganizationId,
                jobTitle = franchiseeData.Title,
                FullName = franchiseeData.JobCustomer.CustomerName,
                jobType = franchiseeData.JobType,
                jobTypeName = franchiseeData.jobTypeName,
                //Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                Time = franchiseeData.ActualStartDateString.ToShortTimeString(),
                AssigneeName = franchiseeData.AssigneeName,
                EndTime = _clock.ToLocal(franchiseeData.EndDate).ToShortTimeString(),
                AssigneePhone = phoneNumber,
                recipientMail = organizationData.Organization.Email,
                fromMail = organizationData.Organization.Franchisee.ContactEmail,
                Description = franchiseeData.Description,
                IsDisplayVisible = isDisplayVisible,
                LinkUrl = linkUrl
            };
            var currentDate = _clock.UtcNow;
            //model.DateRange = new List<DateRangeViewModel>();

            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.fromMail = _settings.SEMIFromEmail;
            }
            //model.DateRange.Add(dateRangeModel);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.NewJobNotificationToUserReassigned, model, _settings.CompanyName, model.fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }
        public void UrgentJobOrEstimateReminderNotificationtoTech(JobEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            string isDisplayVisible = "none";
            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }

            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault() : null) : null;
            if (phoneNumber == null)
            {
                phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            }
            string linkUrl = "";
            if (franchiseeData.jobType.ToLower() == "job")
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.JobId + "/edit/" + franchiseeData.Id;
            }
            else
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.EstimateId + "/manage/" + franchiseeData.Id;
            }
            // To Send Mail to Tech for Job or Estimate Updation Urgent
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = organizationData.Organization.Name,
                Address = franchiseeData.JobCustomer.Address.FullAddressString() + ", " + franchiseeData.JobCustomer.Address.City + " " + franchiseeData.JobCustomer.Address.ZipCode,
                Email = organizationData.Person.Email,
                TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                //StartDate = _clock.ToLocal(franchiseeData.StartDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                //EndDate = _clock.ToLocal(franchiseeData.EndDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                StartDate = franchiseeData.ActualStartDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                EndDate = franchiseeData.ActualEndDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                OrganizationId = organizationData.OrganizationId,
                jobTitle = franchiseeData.Title,
                FullName = franchiseeData.JobCustomer.CustomerName,
                jobType = franchiseeData.jobType,
                jobTypeName = franchiseeData.jobTypeName,
                LinkUrl = linkUrl,
                //Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                //EndTime = _clock.ToLocal(franchiseeData.EndDate).ToShortTimeString(),

                Time = franchiseeData.ActualStartDateString.ToShortTimeString(),
                EndTime = franchiseeData.ActualEndDateString.ToShortTimeString(),
                AssigneeName = franchiseeData.AssigneeName,
                AssigneePhone = phoneNumber,
                recipientMail = organizationData.Organization.Email,
                dateType = franchiseeData.dateType,
                CustomerPhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                fromMail = organizationData.Organization.Franchisee.ContactEmail,
                Description = franchiseeData.Description,
                IsDisplayVisible = isDisplayVisible,
                linkUrl = linkUrl
            };
            var currentDate = _clock.UtcNow;
            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.fromMail = _settings.SEMIFromEmail;
            }
            //model.DateRange.Add(dateRangeModel);
            _notificationService.QueueUpNotificationEmail(NotificationTypes.UrgentJobNotificationToUser, model, _settings.CompanyName, model.fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }
        public void UrgentEstimateReminderNotificationtoTech(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            string isDisplayVisible = "none";
            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }
            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter)
              ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault() : null) : null;
            if (phoneNumber == null)
            {
                phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
                string linkUrl = "";

                if (franchiseeData.jobTypeName.ToLower() == "estimate")
                {
                    linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.Estimateid + "/manage/" + franchiseeData.SchedulerId;
                }



                // To Send Mail to Tech for Job or Estimate Updation Urgent
                var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
                {
                    FranchiseeName = organizationData.Organization.Name,
                    Address = franchiseeData.JobCustomer.Address.FullAddressString() + ", " + franchiseeData.JobCustomer.Address.City + " " + franchiseeData.JobCustomer.Address.ZipCode,
                    Email = organizationData.Person.Email,
                    TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                    PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                    //StartDate = _clock.ToLocal(franchiseeData.StartDate).ToString("MM-dd-yyyy h:mm:ss tt"),
                    //EndDate = _clock.ToLocal(franchiseeData.EndDate).ToString("MM-dd-yyyy h:mm:ss tt"),

                    StartDate = franchiseeData.ActualStartDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                    EndDate = franchiseeData.ActualEndDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                    OrganizationId = organizationData.OrganizationId,
                    jobTitle = franchiseeData.Title,
                    FullName = " " + franchiseeData.JobCustomer.CustomerName,
                    jobType = franchiseeData.JobType,
                    jobTypeName = franchiseeData.jobTypeName,
                    //Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                    //EndTime = _clock.ToLocal(franchiseeData.EndDate).ToShortTimeString(),

                    Time = franchiseeData.ActualStartDateString.ToShortTimeString(),
                    EndTime = franchiseeData.ActualEndDateString.ToShortTimeString(),
                    AssigneeName = franchiseeData.AssigneeName,
                    AssigneePhone = phoneNumber,
                    recipientMail = organizationData.Organization.Franchisee.ContactEmail,
                    dateType = franchiseeData.dateType,
                    CustomerPhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                    Description = franchiseeData.Description,
                    IsDisplayVisible = isDisplayVisible,
                    LinkUrl = linkUrl
                };
                var currentDate = _clock.UtcNow;
                var dateRangeModel = new DateRangeViewModel { };
                dateRangeModel.StartDate = model.StartDate;
                dateRangeModel.EndDate = model.EndDate;
                //model.DateRange.Add(dateRangeModel);
                if (model.OrganizationId == _settings.SEMIFranchiseeId)
                {
                    model.recipientMail = _settings.SEMIFromEmail;
                }
                _notificationService.QueueUpNotificationEmail(NotificationTypes.UrgentJobNotificationToUser, model, _settings.CompanyName, model.recipientMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
            }
        }

        public void CreateWeeklyNotificationForArReport(File file, IEnumerable<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel> franchiseeInvoices, DateTime startDate, DateTime endDate, NotificationTypes type, decimal totalAmount = default(decimal))
        {
            var personName = _settings.OwnerName;
            var recipients = _settings.CCToAdmin;
            var currentWeekInvoices = franchiseeInvoices;
            var previousInvoices = franchiseeInvoices;
            var model = new WeeklyNotificationListModel(_notificationModelFactory.CreateBaseDefault())
            {
                WeeklyCollectionFranchiseeWise = franchiseeInvoices,
                FullName = personName,
                StartDate = startDate.ToString("MM-dd-yyyy"),
                EndDate = endDate.ToString("MM-dd-yyyy"),
                TotalAmount = "$ " + Convert.ToString(totalAmount),
            };
            var resource = new NotificationResource { Resource = file, ResourceId = file.Id, IsNew = true };
            _notificationService.QueueUpNotificationEmail(type, model, _settings.CompanyName, _settings.FromEmail,
            recipients, _clock.UtcNow, null, new List<NotificationResource> { resource });

        }


        public long? BeforeAfterImageNotificationtoCustomer(BeforeAfterImageMailViewModel franchiseeData, File fileDomain, NotificationTypes notificationId)
        {
            // To Send Mail to Tech for new Job
            var model = new BeforeAfterImageMailNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = franchiseeData.FranchiseeName,
                CustomerName = franchiseeData.CustomerName,
                Description = franchiseeData.Description,
                EndDate = franchiseeData.EndDate,
                StartDate = franchiseeData.StartDate,
                Email = franchiseeData.EmailId,
                CustomerId = franchiseeData.CustomerId,
                FranchiseeId = franchiseeData.FranchiseeId,
                FromMail = franchiseeData.FromMail
            };

            var currentDate = _clock.UtcNow;
            model.CCMail = _settings.SEMIFromEmail;
            var resource = new NotificationResource { Resource = fileDomain, ResourceId = fileDomain.Id, IsNew = true };
            var notificationMail = _notificationService.QueueUpNotificationEmail(notificationId, model, _settings.CompanyName, model.FromMail, model.Email, _clock.UtcNow, null, new List<NotificationResource> { resource });
            return notificationMail.Id;
        }

        public void CreateLoanCompletionNotification(FranchiseeLoan loanSchedule)
        {
            if (_organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(loanSchedule.FranchiseeId) == null)
                throw new InvalidDataProvidedException("No Email recipient found.");
            var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(loanSchedule.FranchiseeId).FirstOrDefault();

            var recipients = _settings.AuditRecipients;
            var fromMail = _settings.FromEmail;
            var model = new FranchiseeLoanCompletionNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                Franchisee = organizationRoleUser.Organization.Name,
                Description = loanSchedule.Description,
                LoanAmount = loanSchedule.Amount.ToString(),
                LoanCreatedOn = loanSchedule.DateCreated,
                LoanTensure = loanSchedule.Duration.ToString(),
                LoanId = loanSchedule.Id.ToString(),
            };
            _notificationService.QueueUpNotificationEmail(NotificationTypes.FranchiseeLoanCompletion, model, _settings.CompanyName, fromMail, recipients, _clock.ToUtc(DateTime.Now), null);
        }

        private string GetAddress(Address address)
        {
            string addressString;
            addressString = address.AddressLine1;
            if (address.AddressLine2 != null)
                addressString += ", " + address.AddressLine2;
            if (address.Country != null)
                addressString += ", " + address.Country.Name;
            if (address.State != null)
                addressString += ", " + address.State.Name;
            else
                addressString += ", " + address.StateName;
            if (address.City != null)
                addressString += ", " + address.City.Name;
            else
                addressString += ", " + address.CityName;
            if (address.ZipCode != null)
                addressString += ", " + address.ZipCode;

            return addressString;
        }

        public BeforeAfterBestPairViewModel CreateBeforeAfterBestPairModel(JobEstimateImage jobEstimateImageBefore,
            JobEstimateImage jobEstimateImageAfter, JobScheduler scheduler, MarkbeforeAfterImagesHistry markbeforeAfterImagesHistry)
        {
            string name = MediaLocationHelper.GetAttachmentMediaLocation().Path;
            var personName = "";
            if (markbeforeAfterImagesHistry != null)
            {
                var personDomain = _organizationRoleUserRepository.Get(markbeforeAfterImagesHistry.DataRecorderMetaData.CreatedBy.Value);
                if (personDomain != null)
                {
                    personName = personDomain.Person != null ? personDomain.Person.FirstName + " " + personDomain.Person.LastName : "";
                }
            }
            var viewModel = new BeforeAfterBestPairViewModel()
            {
                AfterImageUrl = jobEstimateImageAfter.File.RelativeLocation + "\\" + jobEstimateImageAfter.File.Name,
                BeforeImageUrl = jobEstimateImageAfter.File.RelativeLocation + "\\" + jobEstimateImageBefore.File.Name,
                FranchiseeName = scheduler.Franchisee.Organization.Name,
                PersonName = personName,
                BestImageMarkedOn = markbeforeAfterImagesHistry != null ? (_clock.ToLocal(markbeforeAfterImagesHistry.DataRecorderMetaData.DateCreated)).ToString("MM/dd/yyyy") : ""

            };
            return viewModel;
        }

        public BeforeAfterBestPairViewModel CreateBeforeAfterPairModel(JobEstimateImage jobEstimateImageBefore, JobEstimateImage jobEstimateImageAfter, JobScheduler scheduler, List<OrganizationRoleUser> organizations)
        {
            string name = MediaLocationHelper.GetAttachmentMediaLocation().Path;

            var viewModel = new BeforeAfterBestPairViewModel()
            {
                AfterImageUrl = jobEstimateImageAfter != null && jobEstimateImageAfter.File != null ? jobEstimateImageAfter.File.RelativeLocation + "\\" + jobEstimateImageAfter.File.Name : "",
                BeforeImageUrl = jobEstimateImageBefore != null && jobEstimateImageBefore.File != null ? jobEstimateImageBefore.File.RelativeLocation + "\\" + jobEstimateImageBefore.File.Name : "",
                FranchiseeName = scheduler != null ? scheduler.Franchisee.Organization.Name : "",
                BeforeImageCss = jobEstimateImageBefore != null && jobEstimateImageBefore.File != null ? jobEstimateImageBefore.File.css == null ? "rotate(0)" : jobEstimateImageBefore.File.css : "rotate(0)",
                AfterImageCss = jobEstimateImageAfter != null && jobEstimateImageAfter.File != null ? jobEstimateImageAfter.File.css == null ? "rotate(0)" : jobEstimateImageAfter.File.css : "rotate(0)"

            };
            return viewModel;
        }

        public void NewJobOrEstimateReminderNotificationtoTechForMeeting(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData,
            NotificationTypes notificationTypes)
        {
            var franchisee = _organizationRepository.Table.FirstOrDefault(x => x.Id == franchiseeData.FranchiseeId);
            string isDisplayVisible = "none";
            string techNamesForMeetingMembers = "";
            string email = "";
            var orgUserRole = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.UserId == franchiseeData.PersonId && x.OrganizationId == franchiseeData.FranchiseeId);
            if (orgUserRole == null)
                orgUserRole = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.Id == franchiseeData.AssigneeId && x.OrganizationId == franchiseeData.FranchiseeId);
            var phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter)
              ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault() : null) : null;
            if (phoneNumber == null)
            {
                phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault() : null) : null;
            }
            if(phoneNumber == null)
            {
                phoneNumber = (organizationData.Organization.Phones != null && organizationData.Organization.Phones.Any()) ? (organizationData.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.Office)
                ? organizationData.Organization.Phones.Select(y => y.Number).FirstOrDefault() : null) : null;
            }
            if (franchiseeData.Description != null)
            {
                isDisplayVisible = "block";
            }
            string linkUrl = "";
            if (!franchiseeData.IsVacation)
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.SchedulerId + "/meeting";
            else
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + franchiseeData.SchedulerId + "/vacation";
            if (franchiseeData.IsVacation)
            {
                franchiseeData.idList = new List<long>();
                franchiseeData.idList.Add(franchiseeData.AssigneeId);
                var orgRoleUser = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.Id == franchiseeData.AssigneeId);
                if (orgRoleUser == null)
                    orgRoleUser = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.UserId == franchiseeData.AssigneeId);
                email += orgRoleUser.Person.Email + ",";
            }
            else
            {
                foreach (var id in franchiseeData.idList)
                {
                    var orgRoleUser = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.UserId == id);
                    if (orgRoleUser == null)
                    {
                        franchiseeData.PersonId = id;
                        orgRoleUser = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.Id == id);
                    }
                    var fullName = orgRoleUser.Person.FirstName + " " + orgRoleUser.Person.LastName;
                    if (id != franchiseeData.PersonId)
                        techNamesForMeetingMembers += fullName + " ,";
                    if (id == franchiseeData.PersonId)
                        email += orgRoleUser.Person.Email + ",";
                }
            }
            if (techNamesForMeetingMembers != "" && !franchiseeData.IsVacation)
            {
                int lastCommaIndex = techNamesForMeetingMembers.LastIndexOf(",", StringComparison.Ordinal);
                techNamesForMeetingMembers = techNamesForMeetingMembers.Substring(0, lastCommaIndex);
            }
            if (email != "")
            {
                int lastCommaIndex = email.LastIndexOf(",", StringComparison.Ordinal);
                email = email.Substring(0, lastCommaIndex);
            }
            // To Send Mail to Tech for new Meeting
            var model = new NewJobOrEstimateReminderNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = franchisee != null ? franchisee.Name : "",
                Address = franchiseeData.JobCustomer.Address.FullAddressString() + ", " + franchiseeData.JobCustomer.Address.City + " " + franchiseeData.JobCustomer.Address.ZipCode,
                Email = email,
                TechName = organizationData.Person.Name.FirstName + " " + organizationData.Person.Name.MiddleName + " " + organizationData.Person.Name.LastName,
                PhoneNumber = franchiseeData.JobCustomer.PhoneNumber,
                StartDate = franchiseeData.ActualStartDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                EndDate = franchiseeData.ActualEndDateString.ToString("MM-dd-yyyy h:mm:ss tt"),
                OrganizationId = franchisee.Id,
                jobTitle = franchiseeData.Title,
                FullName = franchiseeData.JobCustomer.CustomerName,
                jobType = franchiseeData.JobType,
                jobTypeName = franchiseeData.jobTypeName,
                //Time = _clock.ToLocal(franchiseeData.StartDate).ToShortTimeString(),
                Time = franchiseeData.ActualStartDateString.ToShortTimeString(),
                AssigneeName = franchiseeData.AssigneeName,
                AssigneePhone = phoneNumber,
                recipientMail = organizationData.Organization.Email,
                fromMail = organizationData.Organization.Franchisee != null ? organizationData.Organization.Franchisee.ContactEmail : _settings.MarketingEmail,
                Description = franchiseeData.Title,
                IsDisplayVisible = isDisplayVisible,
                LinkUrl = linkUrl,
                techNames = techNamesForMeetingMembers,
                PersonName = orgUserRole.Person.FirstName + " " + orgUserRole.Person.LastName,
                display = techNamesForMeetingMembers != "" ? "block" : "none"
            };
            var currentDate = _clock.UtcNow;

            var dateRangeModel = new DateRangeViewModel { };
            dateRangeModel.StartDate = model.StartDate;
            dateRangeModel.EndDate = model.EndDate;
            if (model.OrganizationId == _settings.SEMIFranchiseeId)
            {
                model.fromMail = _settings.SEMIFromEmail;
            }

            _notificationService.QueueUpNotificationEmail(notificationTypes, model, _settings.CompanyName, model.fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
        }

        public long? InvoiceCustomerNotificationtoCustomer(BeforeAfterImageMailViewModel franchiseeData, List<File> fileDomainList, NotificationTypes notificationId)
        {
            // To Send Mail to Tech for new Job
            var resource = new List<NotificationResource>();
            var model = new BeforeAfterImageMailNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = franchiseeData.FranchiseeName,
                CustomerName = franchiseeData.CustomerName,
                Description = franchiseeData.Description,
                EndDate = franchiseeData.EndDate,
                StartDate = franchiseeData.StartDate,
                Email = franchiseeData.EmailId,
                CustomerId = franchiseeData.CustomerId,
                FranchiseeId = franchiseeData.FranchiseeId,
                FromMail = franchiseeData.FromMail,
                AssigneePhone = _settings.OwnerPhone,
                Code = franchiseeData.Code,
                Url = franchiseeData.Url,
                IsSigned = franchiseeData.IsSigned,
                Name = franchiseeData.Name,
                SchedulerUrl = franchiseeData.SchedulerUrl,
                JobId = franchiseeData.JobId,
                OfficeNumber = franchiseeData.OfficeNumber
            };

            var currentDate = _clock.UtcNow;
            var estimateInvoiceCustomerCCEmail = default(string); 
            if(franchiseeData.ToEmailId != 0)
            {
                estimateInvoiceCustomerCCEmail = _estimateInvoiceCustomerRepository.Table.FirstOrDefault(y => y.Id == franchiseeData.ToEmailId).CCEmail;
            }
            
            if(estimateInvoiceCustomerCCEmail != null)
            {
                model.CCMail = estimateInvoiceCustomerCCEmail;
            }
            else
            {
                if (franchiseeData.FranchiseeId == 62 || franchiseeData.FranchiseeId == 38)
                {
                    model.CCMail = franchiseeData.CcEmail == "" ? _settings.SEMIFromEmail : _settings.SEMIFromEmail + " ," + franchiseeData.CcEmail + "," + franchiseeData.SchedulerEmail;
                }
                else
                {
                    model.CCMail = franchiseeData.CcEmail == "" ? franchiseeData.SchedulerEmail : franchiseeData.CcEmail + "," + franchiseeData.SchedulerEmail;
                }
                
            }
            //model.CCMail = franchiseeData.CcEmail == "" ? _settings.SEMIFromEmail : _settings.SEMIFromEmail + " ," + franchiseeData.CcEmail + "," + franchiseeData.SchedulerEmail;
            foreach (var fileDomain in fileDomainList)
            {
                resource.Add(new NotificationResource { Resource = fileDomain, ResourceId = fileDomain.Id, IsNew = true });
            }
            if (notificationId == (NotificationTypes.PostJobFeedbackToAdmin))
            {
                var jobScheduler = _jobSchedulerRepository.Get(franchiseeData.SchedulerId.GetValueOrDefault());
                if (jobScheduler != null)
                {
                    model.Email = jobScheduler.Franchisee.SchedulerEmail;
                    var frontOfficeExecutives = _organizationRoleUserRepository.Table.Where(x => x.RoleId == (long?)RoleType.FrontOfficeExecutive).ToList();

                    if (frontOfficeExecutives.Count() > 0)
                    {
                        foreach (var frontOfficeExecutive in frontOfficeExecutives)
                        {
                            model.Email += "," + frontOfficeExecutive.Person.Email;
                        }
                    }
                }
            }

            if (notificationId == (NotificationTypes.PostJobFeedbackToCustomer))
            {
                var jobScheduler = _jobSchedulerRepository.Get(franchiseeData.SchedulerId.GetValueOrDefault());
                if (jobScheduler != null)
                {
                    model.Email = jobScheduler.Job != null ? jobScheduler.Job.JobCustomer.Email : model.Email;
                }
            }
            if (notificationId != NotificationTypes.PostJobFeedbackToAdmin)
            {
                var notificationMail = _notificationService.QueueUpNotificationDyamicEmail(notificationId, model, _settings.CompanyName, model.FromMail, model.Email, _clock.UtcNow, franchiseeData.Body, null, resource, null);
                return notificationMail.Id;
            }
            else
            {
                var organizationRoleUser = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(franchiseeData.FranchiseeId.GetValueOrDefault()).FirstOrDefault();
                var notificationMail = _notificationService.QueueUpNotificationEmail(notificationId, model, _settings.CompanyName, model.FromMail, model.Email, _clock.UtcNow, organizationRoleUser.Id, resource);
                return notificationMail.Id;
            }
        }

        public long? InvoiceCustomerNotificationtoCustomerForSignedInvoices(BeforeAfterImageMailViewModel franchiseeData, List<File> fileDomainList, NotificationTypes notificationId)
        {
            // To Send Mail to Tech for new Job
            var resource = new List<NotificationResource>();
            var model = new BeforeAfterImageMailNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                FranchiseeName = franchiseeData.FranchiseeName,
                CustomerName = franchiseeData.CustomerName,
                Description = franchiseeData.Description,
                EndDate = franchiseeData.EndDate,
                StartDate = franchiseeData.StartDate,
                Email = franchiseeData.EmailId,
                CustomerId = franchiseeData.CustomerId,
                FranchiseeId = franchiseeData.FranchiseeId,
                FromMail = franchiseeData.FromMail,
                AssigneePhone = _settings.OwnerPhone,
                Code = franchiseeData.Code,
                Url = franchiseeData.Url,
                IsSigned = franchiseeData.IsSigned,
                AllInvoicesSigned = franchiseeData.AllInvoicesSigned ? "block" : "none",
                AllInvoicesNotSigned = franchiseeData.AllInvoicesSigned ? "none" : "block",
                InvoicesSignedBy = GetInvoiceSignedByString(franchiseeData.IsFromURL, franchiseeData.SalesRepName),
                InvoicesName = franchiseeData.MailToSalesRep ? GetInvoiceNamesStringForSalesRep(franchiseeData.SignedInvoicesName, franchiseeData.UnsignedInvoicesName, franchiseeData.CustomerName) : GetInvoiceNamesString(franchiseeData.SignedInvoicesName, franchiseeData.UnsignedInvoicesName),
                SalesRepName = franchiseeData.SalesRepName,
                IsFromJob = franchiseeData.IsFromJob,
                IsFromUrl = franchiseeData.IsFromURL,
                DoneFrom = franchiseeData.DoneFrom
            };

            var currentDate = _clock.UtcNow;
            model.CCMail = franchiseeData.SchedulerEmail != "" ? franchiseeData.SchedulerEmail : "";
            foreach (var fileDomain in fileDomainList)
            {
                resource.Add(new NotificationResource { Resource = fileDomain, ResourceId = fileDomain.Id, IsNew = true });
            }

            var notificationMail = _notificationService.QueueUpNotificationEmail(notificationId, model, _settings.CompanyName, model.FromMail, model.Email, _clock.UtcNow, null, resource);
            return notificationMail.Id;
        }

        private string GetInvoiceNamesString(string signedInvoicesName, string unsignedInvoicesName)
        {
            string invoicesNames = "";
            if (signedInvoicesName != string.Empty && unsignedInvoicesName != string.Empty)
            {
                invoicesNames = "You have successfully signed your Marblelife proposal." + signedInvoicesName + GetLetterSplelling(signedInvoicesName) + " signed, and " + unsignedInvoicesName + GetLetterSplelling(unsignedInvoicesName) + "left unsigned.";
            }
            else if (signedInvoicesName != string.Empty && unsignedInvoicesName == string.Empty)
            {
                invoicesNames = "You have successfully signed your Marblelife proposal." + signedInvoicesName + GetLetterSplelling(signedInvoicesName) + " signed.";
            }
            else if (signedInvoicesName == string.Empty && unsignedInvoicesName != string.Empty)
            {
                invoicesNames = "You have successfully signed your Marblelife proposal." + unsignedInvoicesName + GetLetterSplelling(unsignedInvoicesName) + " left unsigned.";
            }
            else
            {
                invoicesNames = "You have successfully signed your Marblelife proposal.";
            }
            return invoicesNames;
        }
        private string GetInvoiceNamesStringForSalesRep(string signedInvoicesName, string unsignedInvoicesName, string customerName)
        {
            string invoicesNames = "";
            if (signedInvoicesName != string.Empty && unsignedInvoicesName != string.Empty)
            {
                invoicesNames = signedInvoicesName + GetLetterSplelling(signedInvoicesName) + "signed by " + customerName + ", and " + unsignedInvoicesName + GetLetterSplelling(unsignedInvoicesName) + "left unsigned.";
            }
            else if (signedInvoicesName != string.Empty && unsignedInvoicesName == string.Empty)
            {
                invoicesNames = signedInvoicesName + GetLetterSplelling(signedInvoicesName) + "signed by " + customerName;
            }
            else if (signedInvoicesName == string.Empty && unsignedInvoicesName != string.Empty)
            {
                invoicesNames = unsignedInvoicesName + GetLetterSplelling(unsignedInvoicesName) + " left unsigned.";
            }
            return invoicesNames;
        }

        private string GetLetterSplelling(string invoiceName)
        {
            var splittedInvoice = invoiceName.Split(',');
            if (splittedInvoice.Length > 1)
            {
                return " were ";
            }
            else
            {
                return " was ";
            }
        }
        public string GetInvoiceSignedByString(bool isFromUrl, string salesRepName)
        {
            string invoicesSignedBy = "";
            if (isFromUrl)
            {
                invoicesSignedBy = "Signature done from: E - signature link for customer";
            }
            else
            {
                invoicesSignedBy = "Signature done from: Marblelife Application in presence of " + salesRepName;
            }
            return invoicesSignedBy;
        }

        public void SendWebLeadsNotification(NotificationTypes notificationTypes, DateTime date)
        {
            // To Send Mail to Client for next Dayy Job
            var emails = _settings.WebLeadsToEmail.Split(',');
            var model = new WebLeadsNotificationModel(_notificationModelFactory.CreateBaseDefault())
            {
                Date = date.ToString("MM-dd-yyyy"),
                FromMail = _settings.MarketingEmail,
                CCMail = emails[1],
                ToMail = emails[0]
            };
            _notificationService.QueueUpNotificationEmail(notificationTypes, model, _settings.CompanyName, model.FromMail, model.ToMail, _clock.UtcNow, null, null);
        }
    }
}
