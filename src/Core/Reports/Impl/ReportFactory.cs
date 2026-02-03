using Core.Application;
using Core.Application.Attribute;
using Core.Application.Enum;
using Core.Application.Impl;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Notification.Domain;
using Core.Notification.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Reports.Domain;
using Core.Reports.ViewModel;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class ReportFactory : IReportFactory
    {
        private readonly IClock _clock;
        private readonly IInvoiceFactory _invoicefactory;
        private readonly ISettings _settings;
        public readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;

        public ReportFactory(IUnitOfWork unitOfWork, IClock clock, ISettings settings, IInvoiceFactory invoiceFactory, IOrganizationRoleUserInfoService organizationRoleUserInfoService)
        {
            _clock = clock;
            _settings = settings;
            _invoicefactory = invoiceFactory;
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
        }

        public ServiceReportViewModel CreateViewModel(FranchiseeServiceClassCollection franchiseeServiceClass)
        {
            var serviceReport = new ServiceReportViewModel
            {
                ClassTypeId = franchiseeServiceClass.ClassTypeId,
                Franchisee = franchiseeServiceClass.Franchisee.Organization.Name,
                FranchiseeId = franchiseeServiceClass.FranchiseeId,
                Service = franchiseeServiceClass.ServiceType,
                ServiceTypeId = franchiseeServiceClass.ServiceTypeId,
                MarketingClass = franchiseeServiceClass.MarketingClass,
                TotalSales = franchiseeServiceClass.TotalSales,
                FranchiseeEmail = franchiseeServiceClass.Franchisee.Organization.Email,
                PhoneNumbers = franchiseeServiceClass.Franchisee.Organization.Phones.Select(x => new { x.Number, x.Lookup.Name }).ToList(),
                PhoneNumber = string.Join(",", franchiseeServiceClass.Franchisee.Organization.Phones.Select(e => e.Number)),
                PrimaryContact = franchiseeServiceClass.Franchisee.OwnerName
            };
            return serviceReport;
        }

        public ProductChannelReportViewModel CreateModel(FranchiseeServiceClassCollection franchiseeServiceClass)
        {
            var serviceReport = new ProductChannelReportViewModel
            {
                Franchisee = franchiseeServiceClass.Franchisee.Organization.Name,
                FranchiseeId = franchiseeServiceClass.FranchiseeId,
                Service = franchiseeServiceClass.ServiceType,
                ServiceTypeId = franchiseeServiceClass.ServiceTypeId,
                TotalSales = franchiseeServiceClass.TotalSales,
            };
            return serviceReport;
        }

        public LateFeeReportViewModel CreateViewModel(FranchiseeInvoice domain)
        {
            var currencyRate = domain.Invoice.InvoiceItems.Select(x => x.CurrencyExchangeRate).FirstOrDefault();
            var itemTypeId = domain.Invoice.InvoiceItems.Select(x => x.ItemTypeId).First();

            var royaltyLateFee = _invoicefactory.GetSumLateFeeBasedonItemType(domain.Invoice, LateFeeType.Royalty);
            var salesDataLateFee = _invoicefactory.GetSumLateFeeBasedonItemType(domain.Invoice, LateFeeType.SalesData);

            var interestRate = _invoicefactory.GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.InterestRatePerAnnum);

            var lateFeeType = (royaltyLateFee > 0 && salesDataLateFee > 0) ? (LateFeeType.Royalty.ToString() + ", " + "Sales Data") : (royaltyLateFee > 0 ? LateFeeType.Royalty.ToString() : "Sales Data");

            var startDate = _clock.UtcNow;
            var endDate = _clock.UtcNow;
            if (itemTypeId == (long)InvoiceItemType.LateFees)
            {
                startDate = domain.Invoice.InvoiceItems.OrderByDescending(x => x.LateFeeInvoiceItem.EndDate).Select(x => x.LateFeeInvoiceItem.StartDate).FirstOrDefault();
                endDate = domain.Invoice.InvoiceItems.OrderByDescending(x => x.LateFeeInvoiceItem.EndDate).Select(x => x.LateFeeInvoiceItem.EndDate).FirstOrDefault();
            }
            else
            {
                startDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodStartDate : _clock.UtcNow;
                endDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodEndDate : _clock.UtcNow;
            }
            var serviceReport = new LateFeeReportViewModel
            {
                DueDate = domain.Invoice.DueDate,
                FranchiseeId = domain.FranchiseeId,
                Franchisee = domain.Franchisee.Organization.Name,
                EndDate = endDate,
                StartDate = startDate,
                InvoiceId = domain.InvoiceId,
                LateFeeType = lateFeeType,
                PayableAmount = royaltyLateFee + salesDataLateFee + interestRate,
                LateFeeAmount = royaltyLateFee + salesDataLateFee,
                InterestRate = interestRate,
                CurrencyRate = currencyRate != null ? currencyRate.Rate : 1,
                CurrencyCode = domain.Franchisee.Currency,
                Status = domain.Invoice.Lookup.Name
            };
            return serviceReport;
        }

        public WeeklyNotificationReportViewModel CreateViewModelForNotification(FranchiseeInvoice domain, DateTime startDate, DateTime endDate)
        {


            // var isLateFeeInvoice = domain.Invoice.InvoiceItems.Any(x => x.ItemTypeId != (long)InvoiceItemType.RoyaltyFee && x.ItemTypeId != (long)InvoiceItemType.AdFund);
            var isLateFeeInvoice = domain.Invoice.InvoiceItems.Any(x => x.ItemTypeId == (long)InvoiceItemType.LateFees);

            var royaltyLateFee = GetSumLateFeeBasedonItemType(domain.Invoice, LateFeeType.Royalty, startDate, endDate);
            var salesDataLateFee = GetSumLateFeeBasedonItemType(domain.Invoice, LateFeeType.SalesData, startDate, endDate);

            var adFund = _invoicefactory.GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.AdFund);
            var royalty = _invoicefactory.GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.RoyaltyFee);

            var interestRate = _invoicefactory.GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.InterestRatePerAnnum);

            var lateFeeType = (royaltyLateFee > 0 && salesDataLateFee > 0) ? (LateFeeType.Royalty.ToString() + ", " + "Sales Data") : (royaltyLateFee > 0 ? LateFeeType.Royalty.ToString() : "Sales Data");

            var generatedOn = _clock.UtcNow;
            var invoiceStartDate = _clock.UtcNow;
            var invoiceEndDate = _clock.UtcNow;
            if (isLateFeeInvoice)
            {
                invoiceStartDate = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null).OrderByDescending(x => x.LateFeeInvoiceItem.GeneratedOn).Select(y => y.LateFeeInvoiceItem.StartDate).FirstOrDefault();
                invoiceEndDate = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null).OrderByDescending(x => x.LateFeeInvoiceItem.GeneratedOn).Select(y => y.LateFeeInvoiceItem.EndDate).FirstOrDefault();
                generatedOn = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null && x.LateFeeInvoiceItem.GeneratedOn >= startDate && x.LateFeeInvoiceItem.GeneratedOn <= endDate)
                             .Select(x => x.LateFeeInvoiceItem.GeneratedOn).FirstOrDefault();
            }
            else
            {
                invoiceStartDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodStartDate : _clock.UtcNow;
                invoiceEndDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodEndDate : _clock.UtcNow;
                generatedOn = domain.Invoice.GeneratedOn;
            }

            var latefeeamount = royaltyLateFee + salesDataLateFee + interestRate;
            var invoiceAmount = (adFund + royalty) <= 0 ? latefeeamount : (adFund + royalty);
            var serviceReport = new WeeklyNotificationReportViewModel
            {
                DueDate = domain.Invoice.DueDate.ToShortDateString(),
                FranchiseeId = domain.FranchiseeId,
                Franchisee = domain.Franchisee.Organization.Name,
                EndDate = invoiceEndDate.Date,
                StartDate = invoiceStartDate.Date,
                InvoiceId = domain.InvoiceId,
                LateFeeType = lateFeeType,
                PayableAmount = domain.Invoice.InvoiceItems.Sum(y => y.Amount),
                LateFeeAmount = latefeeamount,
                GeneratedOn = generatedOn.ToShortDateString(),
                Status = domain.Invoice.Lookup.Name,
                LateFeeApplicable = isLateFeeInvoice ? "Yes" : "No",
                InvoiceAmount = invoiceAmount
            };
            return serviceReport;
        }

        public WeeklyNotificationReportViewModel CreateViewModelForPreviousDate(FranchiseeInvoice domain, DateTime startDate)
        {
            var isLateFeeInvoice = domain.Invoice.InvoiceItems.Any(x => x.ItemTypeId == (long)InvoiceItemType.LateFees);

            var royaltyLateFee = GetSumLateFeeBasedonItemTypeForPrevioudDate(domain.Invoice, LateFeeType.Royalty, startDate);
            var salesDataLateFee = GetSumLateFeeBasedonItemTypeForPrevioudDate(domain.Invoice, LateFeeType.SalesData, startDate);

            var adFund = _invoicefactory.GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.AdFund);
            var royalty = _invoicefactory.GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.RoyaltyFee);

            var interestRate = _invoicefactory.GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.InterestRatePerAnnum);

            var lateFeeType = (royaltyLateFee > 0 && salesDataLateFee > 0) ? (LateFeeType.Royalty.ToString() + ", " + "Sales Data") : (royaltyLateFee > 0 ? LateFeeType.Royalty.ToString() : "Sales Data");

            var generatedOn = _clock.UtcNow;
            var invoiceStartDate = _clock.UtcNow;
            var invoiceEndDate = _clock.UtcNow;
            if (isLateFeeInvoice)
            {
                invoiceStartDate = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null).OrderByDescending(x => x.LateFeeInvoiceItem.GeneratedOn).Select(y => y.LateFeeInvoiceItem.StartDate).FirstOrDefault();
                invoiceEndDate = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null).OrderByDescending(x => x.LateFeeInvoiceItem.GeneratedOn).Select(y => y.LateFeeInvoiceItem.EndDate).FirstOrDefault();
                generatedOn = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null && x.LateFeeInvoiceItem.GeneratedOn <= startDate)
                             .Select(x => x.LateFeeInvoiceItem.GeneratedOn).FirstOrDefault();
            }
            else
            {
                invoiceStartDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodStartDate : _clock.UtcNow;
                invoiceEndDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodEndDate : _clock.UtcNow;
                generatedOn = domain.Invoice.GeneratedOn;
            }

            var latefeeamount = royaltyLateFee + salesDataLateFee + interestRate;
            var invoiceAmount = (adFund + royalty) <= 0 ? latefeeamount : (adFund + royalty);
            var serviceReport = new WeeklyNotificationReportViewModel
            {
                DueDate = domain.Invoice.DueDate.ToShortDateString(),
                //FranchiseeId = domain.FranchiseeId,
                Franchisee = domain.Franchisee.Organization.Name,
                EndDate = invoiceEndDate,
                StartDate = invoiceStartDate,
                InvoiceId = domain.InvoiceId,
                LateFeeType = lateFeeType,
                PayableAmount = domain.Invoice.InvoiceItems.Sum(y => y.Amount),
                LateFeeAmount = latefeeamount,
                GeneratedOn = generatedOn.ToShortDateString(),
                Status = domain.Invoice.Lookup.Name,
                LateFeeApplicable = isLateFeeInvoice ? "Yes" : "No",
                InvoiceAmount = invoiceAmount,
                //EndDate1 = invoiceEndDate.ToString("MM-dd-yyyy"),
                //StartDate1 = invoiceStartDate.ToString("MM-dd-yyyy"),
            };
            return serviceReport;
        }

        public WeeklyNotification CreateDomain(DateTime date, long notificationTypeId)
        {
            var weeklyNotification = new WeeklyNotification
            {
                NotificationDate = date,
                NotificationTypeId = notificationTypeId,
                IsNew = true
            };
            return weeklyNotification;
        }

        private decimal GetSumLateFeeBasedonItemType(Invoice invoice, LateFeeType type, DateTime startdate, DateTime endDate)
        {
            var query = invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null && x.LateFeeInvoiceItem.LateFeeTypeId == (long)type
                        && x.LateFeeInvoiceItem.GeneratedOn >= startdate && x.LateFeeInvoiceItem.GeneratedOn <= endDate).Select(x => x.Amount);
            if (!query.Any()) return 0;
            return query.Sum();
        }

        private decimal GetSumLateFeeBasedonItemTypeForPrevioudDate(Invoice invoice, LateFeeType type, DateTime startdate)
        {
            var query = invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null && x.LateFeeInvoiceItem.LateFeeTypeId == (long)type
                        && x.LateFeeInvoiceItem.GeneratedOn < startdate).Select(x => x.Amount);
            if (!query.Any()) return 0;
            return query.Sum();
        }

        public CustomerEmailReportViewModel CreateViewModel(EmailReportViewModel model)
        {
            var viewModel = new CustomerEmailReportViewModel
            {
                CustomerWithEmail = model.CurrentCustomerWithEmail,
                PreviousCustomerWithEmail = model.PreviousCustomerWithEmail,
                Franchisee = model.Franchisee,
                FranchiseeId = model.FranchiseeId,
                PreviousCustomers = model.PreviousCustomers,
                TotalCustomer = model.CurrentCustomers,
                PercentageCurrent = model.CurrentCustomers > 0 ?
                                     Math.Round((decimal)((model.CurrentCustomerWithEmail * 100) / model.CurrentCustomers), 2) : 0,
                PercentagePrevious = model.PreviousCustomers > 0 ?
                                     Math.Round((decimal)((model.PreviousCustomerWithEmail * 100) / model.PreviousCustomers), 2) : 0,
                CurrentPercentage = model.CurrentCustomers > 0 ?
                                     Math.Round((decimal)((model.CurrentCustomerWithEmail * 100) / model.CurrentCustomers), 2) + "%" : "0%",
                PreviousPercentage = model.PreviousCustomers > 0 ?
                                     Math.Round((decimal)((model.PreviousCustomerWithEmail * 100) / model.PreviousCustomers), 2) + "%" : "0%",
            };

            return viewModel;
        }

        public ChartViewModel CreateViewModel(EmailChartDataViewModel model)
        {
            var daysInMonth = DateTime.DaysInMonth(model.Year, model.month);
            var viewModel = new ChartViewModel
            {
                Date = new DateTime(model.Year, model.month, daysInMonth),
                Total = model.TotalCustomers > 0 ? (decimal)(Math.Round((model.TotalCustomersWithEmail * 100) / model.TotalCustomers, 2)) : 0,
                Current = model.CurrentCustomers > 0 ? (decimal)(Math.Round((model.CurrentCustomersWithEmail * 100) / model.CurrentCustomers, 2)) : 0,
                Best = model.BestCount > 0 ? (decimal)(Math.Round((model.BestCountWithEmail * 100) / model.BestCount, 2)) : 0
            };
            return viewModel;
        }

        public UploadBatchCollectionViewModel CreateViewModel(BatchUploadRecord domain)
        {
            var model = new UploadBatchCollectionViewModel
            {
                Id = domain.Id,
                FranchiseeId = domain.Franchisee != null ? domain.Franchisee.Id : 0,
                Franchisee = domain.Franchisee != null ? domain.Franchisee.Organization != null ? domain.Franchisee.Organization.Name : string.Empty : string.Empty,
                FeeProfileId = domain.PaymentFrequencyId != null ? domain.PaymentFrequencyId.Value : 0,
                PaymentFrequency = domain.PaymentFrequency != null ? domain.PaymentFrequency.Name : string.Empty,
                IsUploaded = domain.UploadedOn != null ? true : false,
                UploadStatus = domain.UploadedOn != null ? "Yes" : "No",
                EndDate = domain.EndDate.Date,
                StartDate = domain.StartDate.Date,
                ExpectedUploadDate = domain.ExpectedUploadDate,
                ActualUploadDate = domain.UploadedOn != null ? domain.UploadedOn : null,
                WaitPeriod = domain.WaitPeriod,
                EndDateWithWaitTime = domain.EndDate.AddDays(domain.WaitPeriod)
            };
            return model;
        }

        public BatchUploadRecord CreateDomain(DateTime startDate, DateTime endDate, int waitPeriod, long franchiseeId, long? paymentFrequencyId, SalesDataUpload upload)
        {
            bool isUploaded = upload != null ? upload.DataRecorderMetaData.DateCreated.Date <= endDate.AddDays(waitPeriod).Date : false;
            var domain = new BatchUploadRecord
            {
                FranchiseeId = franchiseeId,
                PaymentFrequencyId = paymentFrequencyId,
                StartDate = startDate,
                EndDate = endDate,
                UploadedOn = upload != null ? upload.DataRecorderMetaData.DateCreated : (DateTime?)null,
                ExpectedUploadDate = endDate.AddDays(waitPeriod),
                WaitPeriod = waitPeriod > 0 ? waitPeriod : _settings.DefaultSalesDataLateFeeWaitPeriod,
                IsCorrectUploaded = isUploaded,
                IsNew = true,
            };
            return domain;
        }

        public TopLeadersInfoModel CreateViewModel(TopLeadersInfoModel model, decimal amount)
        {
            model.Percentage = amount > 0 ? Math.Round((model.TotalSales / amount) * 100, 2) : 0;
            return model;
        }

        public WeeklyNotificationReportViewModel CreateViewModelForNotificationForARReport(FranchiseeInvoice domain, DateTime startDate, DateTime endDate)
        {
            var isLateFeeInvoice = domain.Invoice.InvoiceItems.Any(x => x.ItemTypeId != (long)InvoiceItemType.RoyaltyFee && x.ItemTypeId != (long)InvoiceItemType.AdFund);

            var royaltyLateFee = GetSumLateFeeBasedonItemType(domain.Invoice, LateFeeType.Royalty, startDate, endDate);
            var salesDataLateFee = GetSumLateFeeBasedonItemType(domain.Invoice, LateFeeType.SalesData, startDate, endDate);

            var adFund = _invoicefactory.GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.AdFund);
            var royalty = _invoicefactory.GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.RoyaltyFee);

            var interestRate = _invoicefactory.GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.InterestRatePerAnnum);
            var generatedOn = _clock.UtcNow;
            var invoiceStartDate = _clock.UtcNow;
            var invoiceEndDate = _clock.UtcNow;
            var lateFeeType = (royaltyLateFee > 0 && salesDataLateFee > 0) ? (LateFeeType.Royalty.ToString() + ", " + "Sales Data") : (royaltyLateFee > 0 ? LateFeeType.Royalty.ToString() : "Sales Data");
            if (isLateFeeInvoice)
            {
                invoiceStartDate = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null).OrderBy(x => x.LateFeeInvoiceItem.GeneratedOn).Select(y => y.LateFeeInvoiceItem.StartDate).FirstOrDefault();
                invoiceEndDate = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null).OrderBy(x => x.LateFeeInvoiceItem.GeneratedOn).Select(y => y.LateFeeInvoiceItem.EndDate).FirstOrDefault();
                generatedOn = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null && x.LateFeeInvoiceItem.GeneratedOn >= startDate && x.LateFeeInvoiceItem.GeneratedOn <= endDate)
                             .Select(x => x.LateFeeInvoiceItem.GeneratedOn).FirstOrDefault();
            }
            else
            {
                invoiceStartDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodStartDate : _clock.UtcNow;
                invoiceEndDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodEndDate : _clock.UtcNow;
                generatedOn = domain.Invoice.GeneratedOn;
            }

            var latefeeamount = royaltyLateFee + salesDataLateFee + interestRate;
            var invoiceAmount = (adFund + royalty) <= 0 ? latefeeamount : (adFund + royalty);
            var serviceReport = new WeeklyNotificationReportViewModel
            {
                DueDate = domain.Invoice.DueDate.ToShortDateString(),
                FranchiseeId = domain.FranchiseeId,
                Franchisee = domain.Franchisee.Organization.Name,
                EndDate = invoiceEndDate.Date,
                StartDate = invoiceStartDate.Date,
                InvoiceId = domain.InvoiceId,
                PayableAmount = domain.Invoice.InvoiceItems.Sum(y => y.Amount),
                LateFeeApplicable = isLateFeeInvoice ? "Yes" : "No",
            };
            return serviceReport;
        }

        public EmailViewModel CreateViewModel(NotificationQueue domain)
        {
            var ccMails = new List<string>();
            var recipentsMails = new List<string>();
            if (domain.NotificationEmail != null && domain.NotificationEmail.Recipients.Count() > 0 && domain.NotificationEmail.Recipients.Where(x => x.RecipientTypeId == (long)LookupTypes.CC).Count() > 0)
            {
                ccMails = domain.NotificationEmail.Recipients.Where(x => x.RecipientTypeId == (long)LookupTypes.CC).Select(x => x.RecipientEmail).ToList();
            }
            if (domain.NotificationEmail != null && domain.NotificationEmail.Recipients.Count() > 0)
            {
                recipentsMails = domain.NotificationEmail.Recipients.Where(x => x.RecipientTypeId == (long)LookupTypes.TO).Select(x => x.RecipientEmail).ToList();
            }
            var franchiseeEmailList = new EmailViewModel
            {
                FranchiseeName = domain.Organization != null ? domain.Organization.Name : "",
                Subject = domain.NotificationEmail != null ? domain.NotificationEmail.Subject : "",
                Body = domain.NotificationEmail != null ? domain.NotificationEmail.Body : "",
                SendDate = (domain.ServicedAt != null) ? (domain.ServicedAt) : null,
                EmailTemplateId = domain.NotificationEmail != null ? domain.NotificationEmail.Id : 0,
                FromEmail = domain.NotificationEmail != null ? domain.NotificationEmail.FromEmail : "",
                FromName = domain.NotificationEmail != null ? domain.NotificationEmail.FromName : "",
                RecipientEmail = string.Join(", ", recipentsMails),
                RecipientEmailCc = string.Join(", ", ccMails)
            };
            return franchiseeEmailList;
        }

        public SeoHistryListModel CreateSeoViewModel(HoningMeasurement honingMeasurement, OrganizationRoleUser orgRoleUser, List<EstimatePriceNotes> seoPriceNoteList)
        {
            var seoPriceNote = seoPriceNoteList.FirstOrDefault(x1 => x1.HoningmeasurementId == honingMeasurement.Id);
            var scheduler = (honingMeasurement != null && honingMeasurement.EstimateInvoiceService != null && honingMeasurement.EstimateInvoiceService.EstimateInvoice != null)
                ? honingMeasurement.EstimateInvoiceService.EstimateInvoice.JobScheduler : default(JobScheduler);
            var orgRoleUserForNotes = seoPriceNote != null ? _organizationRoleUserRepository.Table.FirstOrDefault(x => x.Id == seoPriceNote.DataRecorderMetaData.CreatedBy) : default(OrganizationRoleUser);
            var franchiseeName = (honingMeasurement.EstimateInvoiceService != null && honingMeasurement.EstimateInvoiceService.EstimateInvoice != null && 
                honingMeasurement.EstimateInvoiceService.EstimateInvoice.Franchisee != null)
                ? honingMeasurement.EstimateInvoiceService.EstimateInvoice.Franchisee.Organization.Name : "";
            var list = new SeoHistryListModel()
            {
                FranchiseeName = franchiseeName,
                Price = honingMeasurement.ShiftPrice,
                UserName = orgRoleUser.Person.Name.FirstName + " " + orgRoleUser.Person.Name.LastName,
                AddedOn = honingMeasurement.DataRecorderMetaData.DateCreated.ToString(),
                SchedulerId = scheduler != null ? scheduler.Id : default(long),
                EstimateId = scheduler != null ? scheduler.EstimateId.GetValueOrDefault() : default(long),
                HoiningMeasurementId = honingMeasurement != null ? honingMeasurement.Id : default(long),
                Notes = seoPriceNote != null ? seoPriceNote.Notes : "",
                NotesAddedBy = orgRoleUserForNotes != null ? orgRoleUserForNotes.Person.Name.FirstName + " " + orgRoleUserForNotes.Person.Name.LastName : "",
            };

            return list;
        }

        public PriceEstimateExcelViewModel CreatePriceEstimateViewModel(PriceEstimateViewModel model)
        {
            if (model.BulkCorporatePrice > default(decimal))
            {
                model.BulkCorporatePrice = decimal.Parse(String.Format("{0:0.00}", model.BulkCorporatePrice));
            }
            else
            {
                model.BulkCorporatePrice = null;
            }
            if (model.BulkCorporateAdditionalPrice > default(decimal))
            {
                model.BulkCorporateAdditionalPrice = decimal.Parse(String.Format("{0:0.00}", model.BulkCorporateAdditionalPrice));
            }
            else
            {
                model.BulkCorporateAdditionalPrice = null;
            }
            if (model.AverageFranchiseePrice > default(decimal))
            {
                model.AverageFranchiseePrice = decimal.Parse(String.Format("{0:0.00}", model.AverageFranchiseePrice));
            }
            else
            {
                model.AverageFranchiseePrice = null;
            }
            if (model.AverageFranchiseeAdditionalPrice > default(decimal))
            {
                model.AverageFranchiseeAdditionalPrice = decimal.Parse(String.Format("{0:0.00}", model.AverageFranchiseeAdditionalPrice));
            }
            else
            {
                model.AverageFranchiseeAdditionalPrice = null;
            }
            if (model.MaximumFranchiseePrice > default(decimal))
            {
                model.MaximumFranchiseePrice = decimal.Parse(String.Format("{0:0.00}", model.MaximumFranchiseePrice));
            }
            else
            {
                model.MaximumFranchiseePrice = null;
            }
            if (model.MaximumFranchiseeAdditionalPrice > default(decimal))
            {
                model.MaximumFranchiseeAdditionalPrice = decimal.Parse(String.Format("{0:0.00}", model.MaximumFranchiseeAdditionalPrice));
            }
            else
            {
                model.MaximumFranchiseeAdditionalPrice = null;
            }
            PriceEstimateExcelViewModel priceEstimateExcelViewModel = new PriceEstimateExcelViewModel()
            {
                Service = model.Service,
                ServiceType = model.ServiceType,
                Note = model.Note,
                MaterialType = model.MaterialType,
                Category = model.Category,
                Unit = model.Unit,
                BulkCorporatePrice = model.BulkCorporatePrice,
                BulkCorporateAdditionalPrice = model.BulkCorporateAdditionalPrice,
                AverageFranchiseePrice = model.AverageFranchiseePrice, //!= null ? decimal.Parse(String.Format("{0:0.00}", model.AverageFranchiseePrice)) : model.AverageFranchiseePrice,
                MaximumFranchiseePrice = model.MaximumFranchiseePrice,
                AverageFranchiseeAdditionalPrice = model.AverageFranchiseeAdditionalPrice,// != null ? decimal.Parse(String.Format("{0:0.00}", model.AverageFranchiseeAdditionalPrice)) : model.AverageFranchiseeAdditionalPrice,
                MaximumFranchiseeAdditionalPrice = model.MaximumFranchiseeAdditionalPrice,
                MaximumFranchiseePriceName = model.MaximumFranchiseePriceName,
                IsDeleted = 0,
                IsUpdated = 0
            };
            return priceEstimateExcelViewModel;
        }


        public PriceEstimateExcelViewModelForFA CreatePriceEstimateViewModelForFA(PriceEstimateViewModelForFA model)
        {
            //if (model.BulkCorporatePrice > default(decimal))
            //{
            //    model.BulkCorporatePrice = decimal.Parse(String.Format("{0:0.00}", model.BulkCorporatePrice));
            //}
            //else
            //{
            //    model.BulkCorporatePrice = null;
            //}
            //if (model.BulkCorporateAdditionalPrice > default(decimal))
            //{
            //    model.BulkCorporateAdditionalPrice = decimal.Parse(String.Format("{0:0.00}", model.BulkCorporateAdditionalPrice));
            //}
            //else
            //{
            //    model.BulkCorporateAdditionalPrice = null;
            //}
            if (model.CorporatePrice > default(decimal))
            {
                model.CorporatePrice = decimal.Parse(String.Format("{0:0.00}", model.CorporatePrice));
            }
            else
            {
                model.CorporatePrice = null;
            }
            if (model.CorporateAdditionalPrice > default(decimal))
            {
                model.CorporateAdditionalPrice = decimal.Parse(String.Format("{0:0.00}", model.CorporateAdditionalPrice));
            }
            else
            {
                model.CorporateAdditionalPrice = null;
            }
            if (model.FranchiseePrice > default(decimal))
            {
                model.FranchiseePrice = decimal.Parse(String.Format("{0:0.00}", model.FranchiseePrice));
            }
            else
            {
                model.FranchiseePrice = null;
            }
            if (model.FranchiseeAdditionalPrice > default(decimal))
            {
                model.FranchiseeAdditionalPrice = decimal.Parse(String.Format("{0:0.00}", model.FranchiseeAdditionalPrice));
            }
            else
            {
                model.FranchiseeAdditionalPrice = null;
            }
            if (model.AverageFranchiseePrice > default(decimal))
            {
                model.AverageFranchiseePrice = decimal.Parse(String.Format("{0:0.00}", model.AverageFranchiseePrice));
            }
            else
            {
                model.AverageFranchiseePrice = null;
            }
            if (model.AverageFranchiseeAdditionalPrice > default(decimal))
            {
                model.AverageFranchiseeAdditionalPrice = decimal.Parse(String.Format("{0:0.00}", model.AverageFranchiseeAdditionalPrice));
            }
            else
            {
                model.AverageFranchiseeAdditionalPrice = null;
            }
            if (model.MaximumFranchiseePrice > default(decimal))
            {
                model.MaximumFranchiseePrice = decimal.Parse(String.Format("{0:0.00}", model.MaximumFranchiseePrice));
            }
            else
            {
                model.MaximumFranchiseePrice = null;
            }
            if (model.MaximumFranchiseeAdditionalPrice > default(decimal))
            {
                model.MaximumFranchiseeAdditionalPrice = decimal.Parse(String.Format("{0:0.00}", model.MaximumFranchiseeAdditionalPrice));
            }
            else
            {
                model.MaximumFranchiseeAdditionalPrice = null;
            }
            PriceEstimateExcelViewModelForFA priceEstimateExcelViewModel = new PriceEstimateExcelViewModelForFA()
            {
                Id = model.Id,
                Service = model.Service,
                ServiceType = model.ServiceType,
                Note = model.Note,
                MaterialType = model.MaterialType,
                Category = model.Category,
                Unit = model.Unit,
                FranchiseeName = model.FranchiseeName,
                //BulkCorporatePrice = model.BulkCorporatePrice,
                //BulkCorporateAdditionalPrice = model.BulkCorporateAdditionalPrice,
                CorporatePrice = model.CorporatePrice,
                CorporateAdditionalPrice = model.CorporateAdditionalPrice,
                FranchiseePrice = model.FranchiseePrice,// != null ? decimal.Parse(String.Format("{0:0.00}", model.FranchiseePrice)) : model.FranchiseePrice,
                FranchiseeAdditionalPrice = model.FranchiseeAdditionalPrice,// != null ? model.FranchiseeAdditionalPrice : null,
                AverageFranchiseePrice = model.AverageFranchiseePrice,// != null ? decimal.Parse(String.Format("{0:0.00}", model.AverageFranchiseePrice)) : model.AverageFranchiseePrice,
                MaximumFranchiseePrice = model.MaximumFranchiseePrice,
                AverageFranchiseeAdditionalPrice = model.AverageFranchiseeAdditionalPrice, //!= null ? decimal.Parse(String.Format("{0:0.00}", model.AverageFranchiseeAdditionalPrice)) : model.AverageFranchiseeAdditionalPrice,
                MaximumFranchiseeAdditionalPrice = model.MaximumFranchiseeAdditionalPrice,
                MaximumFranchiseePriceName = model.MaximumFranchiseePriceName,
                IsDeleted = 0,
                IsUpdated = 0
            };
            return priceEstimateExcelViewModel;
        }

        public PriceEstimateFileUpload CreatePriceEstimateExcelUploadViewModel(PriceEstimateExcelUploadModel model)
        {
            return new PriceEstimateFileUpload
            {
                Id = model.Id,
                FileId = model.FileId,
                StatusId = model.StatusId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                IsNew = model.Id <= 0,
                Notes = model.Notes
            };
        }

        public PriceEstimateDataUploadViewModel CreateViewModelPriceEstimateDataUpload(PriceEstimateFileUpload model)
        {
            var userInfo = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(model.DataRecorderMetaData.CreatedBy.Value);
            var uploadedBy = userInfo.Email;
            return new PriceEstimateDataUploadViewModel
            {
                Id = model.Id,
                UploadedOn = model.DataRecorderMetaData.DateCreated,
                StatusId = model.StatusId,
                Status = model.Lookup.Name,
                UploadedBy = uploadedBy,
                IsEditable = false,
                Notes = model.Notes,
                TempNotes = model.Notes,
                FileId = model.FileId,
                LogFileId = model.ParsedLogFileId.GetValueOrDefault()
            };
        }

        public ChartViewModel CreateViewModel(ReviewChartDataViewModel model)
        {
            var daysInMonth = DateTime.DaysInMonth(model.Year, model.month);
            var viewModel = new ChartViewModel
            {
                Date = new DateTime(model.Year, model.month, daysInMonth),
                Total = model.TotalCustomers > 0 ? (decimal)(Math.Round((model.TotalCustomersWithReview * 100) / model.TotalCustomers, 2)) : 0,
                Current = model.CurrentCustomers > 0 ? (decimal)(Math.Round((model.CurrentCustomersWithReview * 100) / model.CurrentCustomers, 2)) : 0,
                Best = model.BestCount > 0 ? (decimal)(Math.Round((model.BestCountWithReview * 100) / model.BestCount, 2)) : 0
            };
            return viewModel;
        }
    }
}
