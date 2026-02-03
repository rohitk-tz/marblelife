using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Dashboard.Enum;
using Core.Dashboard.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Sales;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Dashboard.Impl
{
    [DefaultImplementation]
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<SalesRep> _salesRepRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IDashboardFactory _dashboardFactory;
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ISalesDataUploadCreateModelValidator _salesDataUploadCreateModelValidator;
        private readonly ISalesDataUploadService _salesDataUploadService;
        private readonly ISettings _settings;
        private readonly IClock _clock;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<FranchiseeService> _franchiseeServiceRepository;
        private readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<AnnualSalesDataUpload> _annualSalesDataUploadRepository;
        private readonly IRepository<FranchiseeDocumentType> _franchiseeDocumentTypeRepository;
        private readonly IRepository<FranchiseDocument> _franchiseDocumentRepository;
        private readonly IRepository<Organizations.Domain.DocumentType> _documentTypeDocumentRepository;
        private long? PaymentFrequencyId;
        public DashboardService(IUnitOfWork unitOfWork, IDashboardFactory dashboardFactory, ISettings settings, ISalesDataUploadCreateModelValidator salesDataUploadCreateModelValidator,
            ISalesDataUploadService salesDataUploadService, IClock clock)
        {
            _organizationRepository = unitOfWork.Repository<Organization>();
            _dashboardFactory = dashboardFactory;
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _salesRepRepository = unitOfWork.Repository<SalesRep>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _salesDataUploadCreateModelValidator = salesDataUploadCreateModelValidator;
            _salesDataUploadService = salesDataUploadService;
            _settings = settings;
            _clock = clock;
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _franchiseeServiceRepository = unitOfWork.Repository<FranchiseeService>();
            _franchiseeSalesPaymentRepository = unitOfWork.Repository<FranchiseeSalesPayment>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _annualSalesDataUploadRepository = unitOfWork.Repository<AnnualSalesDataUpload>();
            _franchiseDocumentRepository = unitOfWork.Repository<FranchiseDocument>();
            _franchiseeDocumentTypeRepository = unitOfWork.Repository<FranchiseeDocumentType>();
            _documentTypeDocumentRepository = unitOfWork.Repository<Organizations.Domain.DocumentType>();
        }

        public FranchiseeDirectoryListModel GetFranchiseeDirectoryList()
        {
            var organizationList = _organizationRepository.Table.Where(x => x.TypeId != (long)OrganizationType.Franchisor).ToList();
            return new FranchiseeDirectoryListModel
            {
                Collection = organizationList.Select(_dashboardFactory.CreateViewModel).ToList()
            };
        }

        public SalesSummaryListModel GetSalesSummary(long franchiseeId)
        {
            var salesDataUpload = _salesDataUploadRepository.Table.Where(x => (franchiseeId <= 1 || franchiseeId == x.FranchiseeId) &&
                 x.StatusId == (long)SalesDataUploadStatus.Parsed && x.IsActive).OrderByDescending(x => x.DataRecorderMetaData.DateModified).Take(_settings.RecordCountForDashboard).ToArray();

            return new SalesSummaryListModel
            {
                Collection = salesDataUpload.Select(_dashboardFactory.CreateViewModel).ToList()
            };
        }

        public RecentInvoiceListModel GetRecentInvoices(long franchiseeId)
        {
            var franchiseInvoice = _franchiseeInvoiceRepository.Table.Where(x => (franchiseeId <= 1 || x.FranchiseeId == franchiseeId))
                .OrderByDescending(x => x.Invoice.StatusId)
                .ThenBy(x => x.Invoice.DueDate).Take(_settings.RecordCountForDashboard).ToArray();

            return new RecentInvoiceListModel
            {
                Collection = franchiseInvoice.Select(_dashboardFactory.CreateViewModel).ToArray()
            };
        }

        public IList<SalesRepLeaderboardViewModel> GetSalesRepLeaderboard(long franchiseeId, DateTime startDate, DateTime endDate)
        {
            var franchiseeSales = _franchiseeSalesRepository.
                IncludeMultiple(x=>x.Invoice, x=>x.Franchisee, x=>x.Invoice.InvoiceItems, 
                x => x.Invoice.InvoicePayments).Where(x => x.FranchiseeId == franchiseeId)
                .Select(x => new { x.SalesRep, x.Invoice, x.Franchisee })
                .Where(x => x.Invoice != null && (x.Invoice.GeneratedOn == null || ((x.Invoice.GeneratedOn >= startDate)
                && x.Invoice.GeneratedOn <= endDate))).Distinct().GroupBy(y => y.SalesRep).ToList();


            var finalResult = franchiseeSales.Select(y => new SalesRepLeaderboardViewModel
            {
                SalesRep = y.Key,
                Amount = y.Sum(x => x.Invoice.InvoiceItems.Sum(m => m.Amount)),
                NoOfJobs = y.Count(),
                CurrencyRate = y.Select(x => x.Invoice.InvoiceItems.Select(z => z.CurrencyExchangeRate.Rate).FirstOrDefault()).FirstOrDefault(),
                PaidAmount = y.Sum(x => x.Invoice.InvoicePayments.Sum(m => m.Payment.Amount)),
                CurrencyCode = y.Select(x => x.Franchisee.Currency).First().ToString(),
                AvgSales = y.Sum(x => x.Invoice.InvoiceItems.Sum(m => m.Amount)) / y.Count()
            }).OrderByDescending(x => x.Amount).Take(_settings.RecordCountForDashboard).ToList();

            //var allFranchiseeSalesRep = _salesRepRepository.Fetch(x => x.OrganizationRoleUser.Organization.Id == franchiseeId);

            return finalResult;
        }


        public IList<FranchiseeLeaderboardViewModel> GetFranchiseeLeaderboard(long franchiseeId, DateTime startDate, DateTime endDate)
        {
            var franchiseeSales = _franchiseeSalesRepository.IncludeMultiple(x=>x.Invoice, x=>x.Invoice.InvoiceItems,
                x=>x.Franchisee, x => x.Franchisee.Organization, x =>x.Invoice.InvoicePayments, x => x.Invoice.InvoicePayments.Select(y=>y.Payment)).Select(x => new
            {
                FranchiseeId = x.Franchisee.Id,
                Name = x.Franchisee.Organization.Name,
                x.Invoice,
                x.Franchisee
            }).Where(x => x.Invoice != null && (x.Invoice.GeneratedOn == null || ((x.Invoice.GeneratedOn >= startDate)
                 && x.Invoice.GeneratedOn <= endDate))
                 && (franchiseeId <= 1 || x.FranchiseeId == franchiseeId)).Distinct().
                 GroupBy(y => new { y.FranchiseeId, y.Name }).ToList();

            var finalResult = franchiseeSales.Select(y => new FranchiseeLeaderboardViewModel
            {
                Id = y.Key.FranchiseeId,
                Name = y.Key.Name,
                Amount = y.Sum(x => x.Invoice.InvoiceItems.Sum(m => m.Amount)),
                PaidAmount = y.Sum(x => x.Invoice.InvoicePayments.Sum(m => m.Payment.Amount)),
                NoOfJobs = y.Count(),
                CurrencyRate = y.Select(x => x.Invoice.InvoiceItems.Select(z => z.CurrencyExchangeRate.Rate).FirstOrDefault()).FirstOrDefault(),
                CurrencyCode = y.Select(x => x.Franchisee.Currency).First().ToString(),
                AvgSales = y.Sum(x => x.Invoice.InvoiceItems.Sum(m => m.Amount)) / y.Count()
            }).OrderByDescending(x => x.Amount).Take(_settings.RecordCountForDashboard).ToList();

            
            return finalResult;
        }

        public List<ChartDataModel> GetChartData(long franchiseeId, DateTime startDate, DateTime endDate)
        {
            if (startDate == null || endDate == null)
            {
                endDate = DateTime.UtcNow;
                var days = DayOfWeek.Monday - endDate.DayOfWeek;
                startDate = endDate.AddDays(days);
            }

            var servicesOffered = _franchiseeServiceRepository.Table.Where(fs => (franchiseeId < 1 || (fs.FranchiseeId == franchiseeId)))
                                   .Select(fs => fs.ServiceType.Id).Distinct();

            var result = _franchiseeSalesPaymentRepository.IncludeMultiple(x=>x.Payment, x=>x.Payment.PaymentItems,
                x=> x.FranchiseeSales.MarketingClass, x => x.FranchiseeSales, x=>x.Payment.CurrencyExchangeRate).Where(x =>
                     (franchiseeId < 1 || x.FranchiseeSales.FranchiseeId == franchiseeId)
                      && x.Payment != null && x.Payment.PaymentItems.Any()
                      && (x.Payment.Date == null || (x.Payment.Date >= startDate && x.Payment.Date <= endDate)))
                     .Select(x => new { x.FranchiseeSales.MarketingClass.Name, x.FranchiseeSales.MarketingClass.ColorCode, x.Payment }).Distinct().GroupBy(x => new { x.Name, x.ColorCode }).ToList();

            var categoryResult = result.Select(x => new ChartDataModel
            {
                Category = x.Key.Name,
                ColorCode = x.Key.ColorCode,
                Income = franchiseeId < 1
                         ? x.Sum(y => y.Payment.PaymentItems.Where(pi => servicesOffered.Contains(pi.ItemId) && (pi.ItemTypeId == (long)InvoiceItemType.Service || pi.ItemTypeId == (long)InvoiceItemType.Discount))
                          .Sum(z => z.Payment.Amount))
                         : x.Sum(y => y.Payment.PaymentItems.Where(pi => servicesOffered.Contains(pi.ItemId) && (pi.ItemTypeId == (long)InvoiceItemType.Service || pi.ItemTypeId == (long)InvoiceItemType.Discount))
                           .Sum(z => z.Payment.Amount.ToLocalCurrency(z.Payment.CurrencyExchangeRate.Rate)))
            }).OrderByDescending(x => x.Income).ToList();

            categoryResult.RemoveAll(x => x.ColorCode == null);

            categoryResult.Add(new ChartDataModel { Category = "Total", Income = categoryResult.Sum(x => x.Income), ColorCode = "#800000" });

            return categoryResult;
        }

        public List<ChartDataModel> GetChartDataForService(long franchiseeId, DateTime startDate, DateTime endDate)
        {
            if (startDate == null || endDate == null)
            {
                endDate = DateTime.UtcNow;
                var days = DayOfWeek.Monday - endDate.DayOfWeek;
                startDate = endDate.AddDays(days);
            }
            var servicesOffered = _franchiseeServiceRepository.Table.Where(fs => (franchiseeId < 1 || (fs.FranchiseeId == franchiseeId)))
                                    .Select(fs => fs.ServiceType.Id).Distinct().ToList();


            //var result1 = _franchiseeSalesPaymentRepository.Table.Where(x =>
            //        (franchiseeId < 1 || x.FranchiseeSales.FranchiseeId == franchiseeId)
            //        && x.Payment != null && x.Payment.PaymentItems.Any()
            //        && (franchiseeId < 1 || x.Payment.PaymentItems.Any(ii => servicesOffered.Contains(ii.ItemId)
            //        && (ii.ItemTypeId == (long)InvoiceItemType.Service || ii.ItemTypeId == (long)InvoiceItemType.Discount)))
            //        && (x.Payment.Date == null || (x.Payment.Date >= startDate && x.Payment.Date <= endDate)))
            //   .SelectMany(x => x.Payment.PaymentItems).ToList();


            var result = _franchiseeSalesPaymentRepository.Table.Where(x =>
                     (franchiseeId < 1 || x.FranchiseeSales.FranchiseeId == franchiseeId)
                     && x.Payment != null && x.Payment.PaymentItems.Any()
                     && (franchiseeId < 1 || x.Payment.PaymentItems.Any(ii => servicesOffered.Contains(ii.ItemId)
                     && (ii.ItemTypeId == (long)InvoiceItemType.Service || ii.ItemTypeId == (long)InvoiceItemType.Discount)))
                     && (x.Payment.Date == null || (x.Payment.Date >= startDate && x.Payment.Date <= endDate)))
                .SelectMany(x => x.Payment.PaymentItems).ToList();


          var  result1 = result.GroupBy(x => new { x.ServiceType.Name, x.ServiceType.ColorCode }).ToList();
            var categoryResult = result1.Select(x => new ChartDataModel
            {
                Category = x.Key.Name,
                ColorCode = x.Key.ColorCode,
                Income = franchiseeId < 1 ? x.Sum(y => y.Payment.Amount) : x.Sum(y => y.Payment.Amount.ToLocalCurrency(y.Payment.CurrencyExchangeRate.Rate))
            }).OrderByDescending(x => x.Income).ToList();
            categoryResult.Add(new ChartDataModel { Category = "Total", Income = categoryResult.Sum(x => x.Income), ColorCode = "#800000" });

            return categoryResult;
        }

        public CustomerCountViewModel GetCustomerCount(long franchiseeId, DateTime startDate, DateTime endDate)
        {
            endDate = endDate.AddTicks(-1).AddDays(1);
            var customerList = _customerRepository.Table.Where(x => (franchiseeId <= 1 ||
                                            (from fs in _franchiseeSalesRepository.Table where fs.FranchiseeId == franchiseeId && fs.CustomerId == x.Id select fs).Any())
                                             && x.DateCreated >= startDate
                                             && x.DateCreated <= endDate).Distinct();
            var totalCustomers = customerList.Count();
            var customerWithEmails = customerList.Where(x => x.CustomerEmails.Any()).Count();
            var customerWithAddress = customerList.Where(x => x.Address != null).Count();

            return new CustomerCountViewModel
            {
                TotalCustomers = totalCustomers,
                CustomerWithEmails = customerWithEmails,
                CustomerWithAddress = customerWithAddress
            };
        }

        public IEnumerable<StartEndDateViewModel> GetPendingUploadRange(long franchiseeId)
        {
            var lastUpload = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.StatusId != (long)SalesDataUploadStatus.Failed
            && x.IsActive).OrderByDescending(y => y.PeriodEndDate).FirstOrDefault();
            var dateRange = new List<StartEndDateViewModel>();

            if (lastUpload == null)
                return dateRange;

            var feeProfile = lastUpload.Franchisee.FeeProfile;

            var currentDate = _clock.UtcNow;
            var uploadStartDate = _clock.UtcNow;
            var uploadEndDate = uploadStartDate.AddDays(6);

            if (feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Weekly)
            {
                var startDate = lastUpload.PeriodEndDate.Date.AddDays(1);
                var endDate = startDate.AddDays(6);

                var result = _salesDataUploadCreateModelValidator.CheckIfDatesAreValidWeek(startDate, endDate);
                if (result)
                {
                    uploadStartDate = startDate;
                    uploadEndDate = endDate;
                }
                else
                {
                    uploadStartDate = _salesDataUploadService.StartOfWeek(startDate, DayOfWeek.Monday);
                    uploadEndDate = uploadStartDate.Date.AddDays(6);
                }
                if (currentDate >= uploadEndDate)
                {
                    int weekCount = currentDate.Subtract(lastUpload.PeriodEndDate).Days / 7;
                    for (int i = 0; i < weekCount; i++)
                    {
                        var dateRangeModel = new StartEndDateViewModel { };
                        dateRangeModel.StartDate = startDate;
                        dateRangeModel.EndDate = endDate;
                        dateRange.Add(dateRangeModel);
                        startDate = endDate.AddDays(1);
                        endDate = startDate.AddDays(6);
                    }
                }
            }
            if (feeProfile.PaymentFrequencyId == null || feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Monthly)
            {
                var startDate = lastUpload.PeriodEndDate.Date.AddDays(1);
                var endDate = startDate.Date.AddMonths(1).AddDays(-1);

                var result = _salesDataUploadCreateModelValidator.CheckDatesAreValidMonth(startDate, endDate);
                if (result)
                {
                    uploadStartDate = startDate;
                    uploadEndDate = endDate;
                }
                else
                {
                    uploadStartDate = new DateTime(startDate.Year, startDate.Month, 1);
                    uploadEndDate = uploadStartDate.Date.AddMonths(1).AddDays(-1);
                }
                if (currentDate >= uploadEndDate)
                {
                    int monthCount = (currentDate.Month - endDate.Month) + 12 * (currentDate.Year - endDate.Year);
                    for (int i = 0; i < monthCount; i++)
                    {
                        var dateRangeModel = new StartEndDateViewModel { };
                        dateRangeModel.StartDate = startDate;
                        dateRangeModel.EndDate = endDate;
                        dateRange.Add(dateRangeModel);
                        startDate = endDate.Date.AddDays(1);
                        endDate = startDate.Date.AddMonths(1).AddDays(-1);
                    }
                }
            }
            return dateRange;
        }

        public RecentInvoiceListModel GetUnpaidInvoices(long franchiseeId)
        {
            var franchiseInvoice = _franchiseeInvoiceRepository.Table.Where(x => (franchiseeId <= 1 || x.FranchiseeId == franchiseeId)
                                     && x.Invoice.StatusId == (long)InvoiceStatus.Unpaid)
                                    .OrderByDescending(x => x.Invoice.StatusId)
                                    .ThenBy(x => x.Invoice.DueDate).ToArray();

            return new RecentInvoiceListModel
            {
                Collection = franchiseInvoice.Select(_dashboardFactory.CreateViewModel).ToArray()
            };
        }

        public AnnualUploadResponseModel GetAnnualUploadResponse(long franchiseeId)
        {
            var model = new AnnualUploadResponseModel { };
            var date = _clock.UtcNow;
            const int year = 2017;

            var franchisee = _franchiseeRepository.Get(franchiseeId);
            if (franchisee == null)
                return model;
            var lastUploadStartDate = _salesDataUploadService.GetLastUploadedBatch(franchiseeId);
            if (lastUploadStartDate == null)
            {
                model.IsUploaded = true;
                return model;
            }
            if (lastUploadStartDate.Value.Year != year)
            {
                var lastYear = lastUploadStartDate.Value.Year - 1;
                lastUploadStartDate = new DateTime(lastYear, 12, 1);
            }
            //if (franchisee.FeeProfile.PaymentFrequencyId != null)
            //{
            if (franchisee.FeeProfile.PaymentFrequencyId == null)
            {
                PaymentFrequencyId = null;
            }
            else
            {
                PaymentFrequencyId = franchisee.FeeProfile.PaymentFrequencyId.Value;
            }
            var lastUploadDate = _salesDataUploadService.GetLastUploadDateOfYear(lastUploadStartDate.Value, PaymentFrequencyId);
            model.NotificationStartDate = lastUploadDate;
            model.Year = lastUploadDate.Year;
            model.UploadStartDate = new DateTime(model.Year, 01, 01);
            model.UploadEndDate = new DateTime(model.Year, 12, 31);

            var yearLastUpload = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.PeriodStartDate == lastUploadDate.Date)
                .OrderByDescending(y => y.Id).FirstOrDefault();

            if (yearLastUpload != null)
            {
                model.IsLastBatchUploaded = true;
                model.LastUploadFailed = yearLastUpload.StatusId == (long)SalesDataUploadStatus.Failed ? true : false;
            }

            if (lastUploadDate > date)
            {
                model.IsUploaded = true;
                model.isFailed = false;
                model.isRejected = false;
                return model;
            }
            //}
            var annualUpload = _annualSalesDataUploadRepository.Table.Where(au => au.PeriodStartDate.Year == lastUploadStartDate.Value.Year
                            && au.FranchiseeId == franchiseeId).OrderByDescending(x => x.Id).ToList().FirstOrDefault();
            if (annualUpload != null)
            {
                model.IsUploaded = true;
                if (annualUpload.StatusId == (long)SalesDataUploadStatus.Failed)
                    model.isFailed = true;
                if (annualUpload.AuditActionId == (long)AuditActionType.Rejected)
                    model.isRejected = true;
            }
            return model;
        }

        public DocumentSummaryListModel GetDocumentsSummary(long franchiseeId)
        {
            var startDate = _clock.UtcNow.Date;
            int index = 0;
            int deletionIndex = 0;
            List<int> docToBeRemoved = new List<int>();
            var endDate = _clock.UtcNow.Date.AddDays(15);
            var documentIds = _franchiseeDocumentTypeRepository.Table.Where(x => (franchiseeId < 1 || franchiseeId == x.FranchiseeId)).Select(x => x.DocumentTypeId).ToList();
            var expiryingDocuments = _franchiseDocumentRepository.Table.AsEnumerable().Where(x => (franchiseeId <= 1 || franchiseeId == x.FranchiseeId)
                                       && documentIds.Contains(x.DocumentTypeId.GetValueOrDefault())
                                                      && (x.ExpiryDate >= startDate && x.ExpiryDate < endDate)).ToList();
            foreach (var doc in expiryingDocuments)
            {
                var expiringDocument = new FranchiseDocument();
                expiringDocument = expiryingDocuments[index];
                long documentTypeId = expiringDocument.DocumentTypeId.GetValueOrDefault();
                bool isRenewed = _franchiseDocumentRepository.Table.AsEnumerable().Any(x => (franchiseeId == x.FranchiseeId)
                                                                        && (x.DocumentTypeId == expiringDocument.DocumentTypeId.GetValueOrDefault())
                                                      && (x.ExpiryDate > endDate));
                if (isRenewed)
                {
                    docToBeRemoved.Add(index);
                }
                ++index;
            }
            foreach (int indexToBeRemove in docToBeRemoved)
            {
                expiryingDocuments.RemoveAt(indexToBeRemove - deletionIndex);
                ++deletionIndex;
            }
            return new DocumentSummaryListModel
            {
                Collection = expiryingDocuments.Select(x => _dashboardFactory.CreateViewModel(x, franchiseeId)).ToList()
            };
        }
        public DocumentPendingListModel GetDocumentsPending(long franchiseeId)
        {
            List<Organizations.Domain.DocumentType> docPending = new List<Organizations.Domain.DocumentType>();
            var documentIds = _franchiseeDocumentTypeRepository.Table.Where(x => (franchiseeId < 1 || franchiseeId == x.FranchiseeId)).Select(x => x.DocumentTypeId).ToList();
            var pendingDocuments = new FranchiseDocument();
            if (documentIds.Count == 0)
            {
                return new DocumentPendingListModel();
            }
            foreach (var documentId in documentIds)
            {
                pendingDocuments = _franchiseDocumentRepository.Table.Where(x => (franchiseeId <= 1 || x.FranchiseeId == franchiseeId)
                                           && x.DocumentTypeId == documentId).FirstOrDefault();
                if (pendingDocuments == null)
                {
                    var franchiseeType = _documentTypeDocumentRepository.Table.Where(x => x.Id == documentId).FirstOrDefault();
                    docPending.Add(franchiseeType);
                }
            }
            return new DocumentPendingListModel
            {
                Collection = docPending
            };
        }

        public FranchiseeDirectoryListModel GetFranchiseeDirectoryListForSuperAdmin(string name)
        {
            var organizationFinalsList = new List<Organization>();
            string[]  sortingOrder = { "USA", "Canada", "South Africa", "Cayman Islands", "Bahamas", "United Arab Emirates","Mexico" };
            var organizationList = _organizationRepository.Table.Where(x => x.TypeId != (long)OrganizationType.Franchisor && (name=="" || x.Name.StartsWith(name)) && x.IsActive).ToList();
            var groupedOrganization = organizationList.Where(x=> x.Address.Count() > 0).GroupBy(x =>  x.Address.FirstOrDefault().Country.Name).ToList();
            foreach (var countryName in sortingOrder)
            {
                var localGroupedOrganization = groupedOrganization.Where(x => x.Key == countryName).Select(x=>x).ToList();
                if (localGroupedOrganization.Count() > 0)
                {
                    var localGroupedOrganization2 = localGroupedOrganization[0].ToList();
                    if (localGroupedOrganization2.Any(x => x.Name.StartsWith("0-")))
                    {
                        var zerofranchiseeNames = localGroupedOrganization2.Where(x => x.Name.StartsWith("0-")).OrderBy(x=>x.Name).ToList();
                        var withoutZerofranchiseeNames = localGroupedOrganization2.Where(x => !x.Name.StartsWith("0-")).OrderBy(x => x.Name).ToList();
                        localGroupedOrganization2 = new List<Organization>();
                        localGroupedOrganization2.AddRange(withoutZerofranchiseeNames);
                        localGroupedOrganization2.AddRange(zerofranchiseeNames);
                        organizationFinalsList.AddRange(localGroupedOrganization2);
                    }
                    else
                    {
                        organizationFinalsList.AddRange(localGroupedOrganization2.OrderBy(x => x.Name));
                    }
                }
            }
            return new FranchiseeDirectoryListModel
            {
                Collection = organizationFinalsList.Select(_dashboardFactory.CreateViewModel).ToList()
            };
        }

        public string Redirection(string token)
        {
            var url = _settings.BulkPhotouploadURL + "/guest/" + token;
            return url;
        }
    }

  


    public class ChartDataModel
    {
        public string Category { get; set; }

        public decimal Income { get; set; }
        public string ColorCode { get; set; }


    }
}
