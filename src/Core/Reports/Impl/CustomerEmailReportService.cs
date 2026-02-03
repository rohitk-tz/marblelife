using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Notification.Domain;
using Core.Notification.Enum;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Reports.ViewModel;
using Core.Sales.Domain;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.Application.Enum;
using Core.Review.Domain;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class CustomerEmailReportService : ICustomerEmailReportService
    {
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IReportFactory _reportFactory;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IClock _clock;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerEmail> _customerEmailRepository;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IRepository<CustomerEmailAPIRecord> _customerEmailAPIRecordRepository;
        private readonly IRepository<NotificationQueue> _notificationQueueRepository;
        private readonly IRepository<NotificationEmailRecipient> _notificationEmailRecipientRepository;
        private readonly ISortingHelper _sortingHelper;
        private readonly IRepository<PartialPaymentEmailApiRecord> _partialcustomerEmailAPIRecordRepository;
        private readonly IRepository<CustomerFeedbackResponse> _customerFeedbackResponseRepository;
        public CustomerEmailReportService(IUnitOfWork unitOfWork, IClock clock, IReportFactory reportFactory, IExcelFileCreator excelFileCreator, ISortingHelper sortingHelper)
        {
            _sortingHelper = sortingHelper;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _reportFactory = reportFactory;
            _organizationRepository = unitOfWork.Repository<Organization>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _clock = clock;
            _customerRepository = unitOfWork.Repository<Customer>();
            _customerEmailRepository = unitOfWork.Repository<CustomerEmail>();
            _excelFileCreator = excelFileCreator;
            _customerEmailAPIRecordRepository = unitOfWork.Repository<CustomerEmailAPIRecord>();
            _notificationQueueRepository = unitOfWork.Repository<NotificationQueue>();
            _notificationEmailRecipientRepository = unitOfWork.Repository<NotificationEmailRecipient>();
            _partialcustomerEmailAPIRecordRepository = unitOfWork.Repository<PartialPaymentEmailApiRecord>();
            _customerFeedbackResponseRepository = unitOfWork.Repository<CustomerFeedbackResponse>();
        }

        public CustomerEmailReportListModel GetCustomerEmailReportList(CustomerEmailReportFilter filter)
        {
            var customerEmailList = GetCustomerEmailReportFilter(filter);
            var list = customerEmailList.Select(_reportFactory.CreateViewModel).OrderByDescending(x => x.PercentageCurrent).ToList();
            if (!list.Any())
                return new CustomerEmailReportListModel();

            var bestFranchisee = list.FirstOrDefault();

            var currentCustomers = list.Sum(x => x.TotalCustomer);
            var currentCustomerWithEmail = list.Sum(x => x.CustomerWithEmail);
            var previousCustomers = list.Sum(x => x.PreviousCustomers);
            var previousCustomerWithEmail = list.Sum(x => x.PreviousCustomerWithEmail);
            decimal currenctPercentage = currentCustomers > 0 ? Math.Round((decimal)((currentCustomerWithEmail * 100) / currentCustomers), 2) : 0;
            decimal previousPercentage = previousCustomers > 0 ? Math.Round((decimal)((previousCustomerWithEmail * 100) / previousCustomers), 2) : 0;


            var total = new CustomerEmailReportViewModel
            {
                TotalCustomer = currentCustomers,
                CustomerWithEmail = currentCustomerWithEmail,
                PreviousCustomers = previousCustomers,
                PreviousCustomerWithEmail = previousCustomerWithEmail,
                PercentageCurrent = currenctPercentage,
                PercentagePrevious = previousPercentage
            };
            return new CustomerEmailReportListModel
            {
                Collection = list,
                BestFranchisee = bestFranchisee,
                Total = total,
                Filter = filter,
            };
        }

        private IEnumerable<EmailReportViewModel> GetCustomerEmailReportFilter(CustomerEmailReportFilter filter)
        {
            const int LastMonth = 12;
            const int FirstMonth = 1;

            if (filter.Year <= 0)
            {
                filter.Year = _clock.UtcNow.Year;
            }

            var franchiseeIds = _franchiseeRepository.Table.Where(x => (filter.FranchiseeId <= 1 || filter.FranchiseeId == x.Id)).Select(y => y.Id);
            var franchiseeList = _organizationRepository.Table.Where(x => franchiseeIds.Contains(x.Id) && x.IsActive).ToList();

            var customerIdsWithEmail = _customerEmailRepository.Table.Select(x => x.CustomerId).Distinct();

            var currentCustomerList = (from fs in _franchiseeSalesRepository.Table
                                       join c in _customerRepository.Table on fs.CustomerId equals c.Id
                                       where (filter.FranchiseeId <= 1 || fs.FranchiseeId == filter.FranchiseeId)
                                       && ((filter.Month == null || filter.Month <= 0) || fs.Invoice.GeneratedOn.Month == filter.Month)
                                       && (fs.Invoice.GeneratedOn.Year == filter.Year)
                                       select new { fs.FranchiseeId, CustomerId = c.Id });

            var yearToCompare = (filter.Month != null && filter.Month > 0) ? filter.Year : filter.Year - 1;
            var monthToCompare = filter.Month - 1;

            if (filter.Month == FirstMonth)
            {
                yearToCompare = filter.Year - 1;
                monthToCompare = LastMonth;
            }

            var previousCustomerList = (from fs in _franchiseeSalesRepository.Table
                                        join c in _customerRepository.Table on fs.CustomerId equals c.Id
                                        where (filter.FranchiseeId <= 1 || fs.FranchiseeId == filter.FranchiseeId)
                                        && ((filter.Month == null || filter.Month <= 0) || fs.Invoice.GeneratedOn.Month == monthToCompare)
                                        && (fs.Invoice.GeneratedOn.Year == yearToCompare)
                                        select new { fs.FranchiseeId, CustomerId = c.Id });

            var currentCustomerWithEmail = currentCustomerList.Where(x => customerIdsWithEmail.Contains(x.CustomerId));
            var previousCustomerWithEmail = previousCustomerList.Where(x => customerIdsWithEmail.Contains(x.CustomerId));

            var result = franchiseeList.
                     Select(x => new EmailReportViewModel
                     {
                         FranchiseeId = x.Id,
                         Franchisee = x.Name,
                         CurrentCustomers = currentCustomerList.Where(y => y.FranchiseeId == x.Id).Select(z => z.CustomerId).Distinct().Count(),
                         CurrentCustomerWithEmail = currentCustomerWithEmail.Where(y => y.FranchiseeId == x.Id).Select(z => z.CustomerId).Distinct().Count(),
                         PreviousCustomers = previousCustomerList.Where(y => y.FranchiseeId == x.Id).Select(z => z.CustomerId).Distinct().Count(),
                         PreviousCustomerWithEmail = previousCustomerWithEmail.Where(y => y.FranchiseeId == x.Id).Select(z => z.CustomerId).Distinct().Count()
                     }).ToArray();

            return result;
        }

        public EmailChartDataListModel GetChartData(long franchiseeId, DateTime startDate, DateTime endDate)
        {
            var currentDate = _clock.UtcNow;
            if (startDate == null || endDate == null)
            {
                startDate = new DateTime(currentDate.Year, 1, 31);
                endDate = currentDate;
            }

            var franchisee = _franchiseeRepository.Get(franchiseeId);

            var frSalesList = _franchiseeSalesRepository.Table.Where(x => (startDate == null || x.Invoice.GeneratedOn >= startDate)
                                                    && (endDate == null || x.Invoice.GeneratedOn <= endDate));

            var result = GetChartModel(frSalesList, startDate, endDate, franchiseeId);

            return new EmailChartDataListModel
            {
                Franchisee = franchisee.Organization.Name,
                ChartData = result.Select(_reportFactory.CreateViewModel)
            };
        }

        private List<EmailChartDataViewModel> GetChartModel(IQueryable<FranchiseeSales> frSalesList, DateTime startDate, DateTime endDate, long franchiseeId)
        {
            var list = new List<EmailChartDataViewModel>();

            var months = MonthsBetween(startDate, endDate);
            var customerIdsWithEmail = _customerEmailRepository.Table.Select(x => x.CustomerId).Distinct();

            foreach (var item in months)
            {
                var totalCustomers = frSalesList.Where(x => x.Customer.DateCreated.Value.Month == item.Item1);
                var currentCustomers = totalCustomers.Where(x => x.FranchiseeId == franchiseeId).Select(x => x.Customer);

                var totalCustomersWithEmail = totalCustomers.Where(x => customerIdsWithEmail.Contains(x.CustomerId));
                var currentCustomersWithEmail = currentCustomers.Where(x => customerIdsWithEmail.Contains(x.Id));

                var model = new EmailChartDataViewModel { };
                model.Year = item.Item2;
                model.month = item.Item1;
                model.TotalCustomers = totalCustomers.Select(x => x.CustomerId).Distinct().Count();
                model.TotalCustomersWithEmail = totalCustomersWithEmail.Select(x => x.CustomerId).Distinct().Count();
                model.CurrentCustomers = currentCustomers.Select(x => x.Id).Distinct().Count();
                model.CurrentCustomersWithEmail = currentCustomersWithEmail.Select(x => x.Id).Distinct().Count();
                list.Add(model);
            }

            // add best franchisee to list.

            return list;
        }

        private static IEnumerable<Tuple<int, int>> MonthsBetween(DateTime startDate, DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator <= limit)
            {
                yield return Tuple.Create(iterator.Month, iterator.Year);
                iterator = iterator.AddMonths(1);
            }
        }

        public bool DownloadEmailReport(CustomerEmailReportFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<CustomerEmailReportViewModel>();
            IEnumerable<EmailReportViewModel> reportList = GetCustomerEmailReportFilter(filter);

            //prepare item collection
            foreach (var item in reportList)
            {
                var model = _reportFactory.CreateViewModel(item);
                model.Trend = model.PercentagePrevious > model.PercentageCurrent
                    ? (model.PercentagePrevious - model.PercentageCurrent) + "% (Down)"
                    : model.PercentageCurrent - model.PercentagePrevious + "% (Up)";
                if (model.PercentagePrevious == model.PercentageCurrent)
                    model.Trend = model.PercentageCurrent - model.PercentagePrevious + "%";
                reportCollection.Add(model);
            }
            reportCollection = reportCollection.OrderByDescending(x => x.PercentageCurrent).ToList();

            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/emailReport-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }

        public bool IsCustomerSyncedToEmailAPI(long customerId)
        {
            var emailAPIRecord = _customerEmailAPIRecordRepository.Table.Where(x => x.CustomerId == customerId && x.IsSynced).ToList();
            if (emailAPIRecord.Any())
                return true;
            return false;
        }

        public bool IsCustomerEmailSyncedToEmailAPI(long customerId, string email)
        {
            var emailAPIRecord = _customerEmailAPIRecordRepository.Table.Where(x => x.CustomerId == customerId && x.IsSynced && x.CustomerEmail.ToLower().Equals(email)).ToList();
            if (emailAPIRecord.Any())
                return true;
            return false;
        }

        public MailListModel GetFranchiseeWiseMail(MailListFilter filter)
        {
            if (filter.PeriodStartDate != null && filter.PeriodStartDate == filter.PeriodEndDate)
            {
                filter.PeriodStartDate = _clock.ToUtc(DateTime.Now);
                filter.PeriodEndDate = filter.PeriodEndDate.GetValueOrDefault().AddMonths(-2);
            }
            else if(filter.PeriodStartDate > filter.PeriodEndDate)
            {
                var startDate = filter.PeriodStartDate;
                filter.PeriodStartDate = filter.PeriodEndDate;
                filter.PeriodEndDate = startDate;
            }
            if (filter.PeriodStartDate == null)
            {
                filter.PeriodEndDate = _clock.ToUtc(DateTime.Now).AddDays(1);
                filter.PeriodStartDate = _clock.ToUtc(filter.PeriodEndDate.GetValueOrDefault().AddMonths(-2));
            }

            var emailIds = _notificationQueueRepository.Table.Where(x => (filter.FranchiseeId <= 0 || filter.FranchiseeId == x.FranchiseeId)
                                                                             && x.FranchiseeId != null

                                                                             && (filter.Text == "" || (x.NotificationEmail.Subject.StartsWith(filter.Text)
                                                                             || x.NotificationEmail.Subject.Contains(filter.Text) || x.NotificationEmail.Subject.EndsWith(filter.Text)))

                                                                             && (x.NotificationEmail.FromEmail.Contains(filter.EmailFrom))

                                                                             && (filter.PeriodEndDate == default(DateTime?)
                                                                             || (x.ServicedAt >= filter.PeriodStartDate && x.ServicedAt < filter.PeriodEndDate))
                                                                             && x.ServiceStatusId == (long)ServiceStatus.Success).OrderByDescending(x=>x.Id).ToList();
            if(!string.IsNullOrEmpty(filter.EmailCc))
            {
                emailIds = emailIds.Where(x => x.NotificationEmail.Recipients.Where(z => z.RecipientTypeId == (long)LookupTypes.CC).Select(y => y.RecipientEmail).Contains(filter.EmailCc)).ToList();
            }
            if (!string.IsNullOrEmpty(filter.EmailTo))
            {
                emailIds = emailIds.Where(x => x.NotificationEmail.Recipients.Where(z => z.RecipientTypeId == (long)LookupTypes.TO).Select(y => y.RecipientEmail).Contains(filter.EmailTo)).ToList();
            }

            int totalRecords = emailIds.Count();

            emailIds = emailIds.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
            var collections = emailIds.Select(_reportFactory.CreateViewModel);
            if (filter.PropName != "")
            {
                collections = GetFranchiseeMailFilterData(filter, collections.AsQueryable()).ToList();
            }
            else
            {
                collections = collections.OrderByDescending(x => x.EmailTemplateId);
            }
            //collections = collections.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
            return new MailListModel
            {
                Collection = collections,
                PagingModel = new PagingModel(filter.PageNumber, filter.PageSize, totalRecords),
                EndDate= filter.PeriodEndDate,
                StartDate= filter.PeriodStartDate
            };
        }

        private IQueryable<EmailViewModel> GetFranchiseeMailFilterData(MailListFilter filter, IQueryable<EmailViewModel> collection)
        {

            if (filter.PropName != null)
            {
                switch (filter.PropName)
                {

                    case "Id":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Id, filter.Order);
                        break;
                    case "FranchiseeName":
                        collection = _sortingHelper.ApplySorting(collection, x => x.FranchiseeName, filter.Order);
                        break;
                    case "SendDate":
                        collection = _sortingHelper.ApplySorting(collection, x => x.SendDate, filter.Order);
                        break;
                    case "RecipientEmail":
                        collection = _sortingHelper.ApplySorting(collection, x => x.RecipientEmail, filter.Order);
                        break;
                    case "FromEmail":
                        collection = _sortingHelper.ApplySorting(collection, x => x.FromEmail, filter.Order);
                        break;
                    case "EmailTitle":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Subject, filter.Order);
                        break;
                    case "RecipientEmailCC":
                        collection = _sortingHelper.ApplySorting(collection, x => x.RecipientEmailCc, filter.Order);
                        break;
                }
            }
            return collection;
        }

        public ReviewChartDataListModel GetChartDataForReview(long franchiseeId, DateTime startDate, DateTime endDate)
        {
            var currentDate = _clock.UtcNow;
            if (startDate == null || endDate == null)
            {
                startDate = new DateTime(currentDate.Year, 1, 31);
                endDate = currentDate;
            }

            var franchisee = _franchiseeRepository.Get(franchiseeId);

            var paymentList = _partialcustomerEmailAPIRecordRepository.Table.Where(x => (startDate == null || x.DateCreated >= startDate)
                                                      && (endDate == null || x.DateCreated <= endDate)
                                                      && (x.IsSynced == true));

            var result = GetChartModelForReview(paymentList, startDate, endDate, franchiseeId);

            return new ReviewChartDataListModel
            {
                Franchisee = franchisee.Organization.Name,
                ChartData = result.Select(_reportFactory.CreateViewModel)
            };
        }

        private List<ReviewChartDataViewModel> GetChartModelForReview(IQueryable<PartialPaymentEmailApiRecord> paymentList, DateTime startDate, DateTime endDate, long franchiseeId)
        {
            var list = new List<ReviewChartDataViewModel>();

            var months = MonthsBetween(startDate, endDate);

            var customerReviews = _customerFeedbackResponseRepository.Table.Where(x => (startDate == null || x.DateOfReview >= startDate)
                                                    && (endDate == null || x.DateOfReview <= endDate));

            foreach (var item in months)
            {
                var totalReviewSent = paymentList.Where(x => x.DateCreated.Value.Month == item.Item1);
                var tReviewSent = totalReviewSent.ToList();
                var currentReviewSent = totalReviewSent.Where(x => x.FranchiseeId == franchiseeId);

                var totalReviewReceived = customerReviews.Where(x => x.DateOfReview.Month == item.Item1);
                var ttotalReviewReceived = totalReviewReceived.ToList();
                var currentReviewReceived = totalReviewReceived.Where(x => x.FranchiseeId == franchiseeId);

                
                

                var model = new ReviewChartDataViewModel { };
                model.Year = item.Item2;
                model.month = item.Item1;
                model.TotalCustomers = totalReviewSent.Select(x => x.Customer).Distinct().Count();
                model.TotalCustomersWithReview = totalReviewReceived.Select(x => x.CustomerName).Distinct().Count();
                model.CurrentCustomers = currentReviewSent.Select(x => x.Id).Distinct().Count();
                model.CurrentCustomersWithReview = currentReviewReceived.Select(x => x.Id).Distinct().Count();
                list.Add(model);
            }
            return list;
        }

        public ReviewCountModel GetReviewCounts(long franchiseeId)
        {
            try
            {
                ReviewCountModel reviewCountModel = new ReviewCountModel();

                var currentYear = DateTime.Now.Year;
                var paymentList = _partialcustomerEmailAPIRecordRepository.Table.Where(x => (x.DateCreated.Value.Year == currentYear)
                                                      && (x.IsSynced == true)).ToList();

                var customerReviews = _customerFeedbackResponseRepository.Table.Where(x => x.DateOfReview.Year == currentYear).ToList();

                reviewCountModel.TotalReviewSent = paymentList.Count;
                reviewCountModel.TotalReviewReceived = customerReviews.Count;
                reviewCountModel.TotalReviewSentForFranchisee = paymentList.Where(x => x.FranchiseeId == franchiseeId).ToList().Count;
                reviewCountModel.TotalReviewReceivedForFranchisee = customerReviews.Where(x => x.FranchiseeId == franchiseeId).ToList().Count;

                return reviewCountModel;
            }
            catch(Exception ex)
            {
                return new ReviewCountModel();
            }
        }
    }
}
