using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Notification.ViewModel;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Reports.ViewModel;
using Core.Sales;
using Core.Sales.Domain;
using Core.Users.Enum;
using Core.Reports.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Application.Domain;
using Core.Users.Domain;
using Core.Scheduler.Domain;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class ReportService : IReportService
    {
        private readonly IRepository<OrganizationRoleUser> _perspnRepository;
        private readonly IRepository<HoningMeasurement> _honingMeasurementRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly ISortingHelper _sortingHelper;
        private readonly IRepository<FranchiseeService> _franchiseeServiceRepository;
        private readonly IReportFactory _reportFactory;
        private readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        private readonly IRoyaltyReportFactory _royaltyReportFactory;
        private readonly IRepository<ServiceType> _serviceTypeRepository;
        private readonly IRepository<MarketingClass> _marketingClassRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<PaymentItem> _paymentItemRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IClock _clock;
        public readonly IRepository<BatchUploadRecord> _batchUploadRecordRepository;
        public readonly IRepository<InvoicePayment> _invoicePaymentRepository;
        public readonly IRepository<ServicesTag> _servicesTagRepository;
        public readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        public readonly IRepository<Lookup> _lookupRepository;
        public readonly IRepository<PriceEstimateServices> _priceEstimateServicesRepository;
        public readonly IRepository<ShiftCharges> _shiftChargesRepository;
        public readonly IRepository<ReplacementCharges> _replacementChargesRepository;
        public readonly IRepository<MaintenanceCharges> _maintenanceChargesRepository;
        public readonly IRepository<FloorGrindingAdjustment> _floorGrindingAdjustmentRepository;
        public readonly IRepository<FloorGrindingAdjustmentNotes> _floorGrindingAdjustmentNotesRepository;
        public readonly IRepository<EstimatePriceNotes> _seoPriceNotesRepository;
        public readonly IRepository<PriceEstimateFileUpload> _priceEstimateFileUploadRepository;
        private readonly ISettings _settings;
        private readonly IFileService _fileService;
        public ReportService(IUnitOfWork unitOfWork, ISortingHelper sortingHelper, IClock clock, IReportFactory reportFactory,
            IRoyaltyReportFactory royaltyReportFactory, IExcelFileCreator excelFileCreator, ISettings settings, IFileService fileService)
        {
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _sortingHelper = sortingHelper;
            _reportFactory = reportFactory;
            _franchiseeServiceRepository = unitOfWork.Repository<FranchiseeService>();
            _franchiseeSalesPaymentRepository = unitOfWork.Repository<FranchiseeSalesPayment>();
            _royaltyReportFactory = royaltyReportFactory;
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
            _marketingClassRepository = unitOfWork.Repository<MarketingClass>();
            _paymentRepository = unitOfWork.Repository<Payment>();
            _paymentItemRepository = unitOfWork.Repository<PaymentItem>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _excelFileCreator = excelFileCreator;
            _clock = clock;
            _batchUploadRecordRepository = unitOfWork.Repository<BatchUploadRecord>();
            _invoicePaymentRepository = unitOfWork.Repository<InvoicePayment>();
            _settings = settings;
            _servicesTagRepository = unitOfWork.Repository<ServicesTag>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _lookupRepository = unitOfWork.Repository<Lookup>();
            _priceEstimateServicesRepository = unitOfWork.Repository<PriceEstimateServices>();
            _shiftChargesRepository = unitOfWork.Repository<ShiftCharges>();
            _replacementChargesRepository = unitOfWork.Repository<ReplacementCharges>();
            _maintenanceChargesRepository = unitOfWork.Repository<MaintenanceCharges>();
            _floorGrindingAdjustmentRepository = unitOfWork.Repository<FloorGrindingAdjustment>();
            _floorGrindingAdjustmentNotesRepository = unitOfWork.Repository<FloorGrindingAdjustmentNotes>();
            _honingMeasurementRepository = unitOfWork.Repository<HoningMeasurement>();
            _perspnRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _seoPriceNotesRepository = unitOfWork.Repository<EstimatePriceNotes>();
            _priceEstimateFileUploadRepository = unitOfWork.Repository<PriceEstimateFileUpload>();
            _fileService = fileService;
        }


        public ServiceReportListModel GetReportsForService(ServiceReportListFilter filter, int pageNumber, int pageSize)
        {
            var franchiseeServiceClassList = GetFranchiseeServiceClassList(filter);

            var finalcollection = franchiseeServiceClassList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new ServiceReportListModel
            {
                Collection = finalcollection.Select(_reportFactory.CreateViewModel).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, franchiseeServiceClassList.Count())
            };
        }
        private IEnumerable<FranchiseeServiceClassCollection> GetFranchiseeServiceClassList(ServiceReportListFilter filter)
        {
            if (filter.ClassTypeId <= 0 && filter.FranchiseeId <= 0 && filter.ServiceTypeId <= 0 && filter.PaymentDateStart == null && filter.PaymentDateEnd == null)
            {
                var clock = new Clock();
                var currentDate = _clock.UtcNow;
                filter.PaymentDateStart = clock.FirstDayOfMonth(currentDate);
                filter.PaymentDateEnd = clock.LastDayOfMonth(currentDate);
            }


            var franchiseeSalespayment = (from collection in _franchiseeSalesRepository.Table
                                          join invoicePayment in _invoicePaymentRepository.Table on collection.InvoiceId equals invoicePayment.InvoiceId
                                          join payment in _paymentRepository.Table on invoicePayment.PaymentId equals payment.Id
                                          join paymentItem in _paymentItemRepository.Table on payment.Id equals paymentItem.PaymentId
                                          //join franchiseeSales in _franchiseeSalesRepository.Table on collection.FranchiseeSalesId equals franchiseeSales.Id
                                          where ((filter.FranchiseeId < 1 || collection.FranchiseeId == filter.FranchiseeId)
                                            && (filter.ClassTypeId < 1 || collection.ClassTypeId == filter.ClassTypeId)
                                            && (filter.ServiceTypeId < 1 || ((paymentItem.ItemTypeId == (long)InvoiceItemType.Service || paymentItem.ItemTypeId == (long)InvoiceItemType.Discount)
                                            && paymentItem.ItemId == filter.ServiceTypeId))
                                            && (filter.PaymentDateStart == null || (payment.Date >= filter.PaymentDateStart))
                                            && (filter.PaymentDateEnd == null || (payment.Date <= filter.PaymentDateEnd)))
                                          select new
                                          {
                                              FranchiseeId = collection.FranchiseeId,
                                              ServiceTypeId = paymentItem.ItemId,
                                              ServiceType = paymentItem.ServiceType.Name,
                                              ClassTypeId = collection.ClassTypeId,
                                              Franchisee = collection.Franchisee,
                                              MarketingClass = collection.MarketingClass.Name,
                                              PaymentItem = paymentItem
                                          }).GroupBy(y => new { y.ClassTypeId, y.FranchiseeId, y.ServiceTypeId, y.Franchisee, y.MarketingClass, y.ServiceType }).ToList();

            var finalResult = franchiseeSalespayment.Select(x => new FranchiseeServiceClassCollection
            {
                FranchiseeId = x.Key.FranchiseeId,
                ClassTypeId = x.Key.ClassTypeId,
                ServiceTypeId = x.Key.ServiceTypeId,
                Franchisee = x.Key.Franchisee,
                MarketingClass = x.Key.MarketingClass,
                ServiceType = x.Key.ServiceType,
                TotalSales = x.Sum(y => y.PaymentItem.Payment.Amount)
            });

            return finalResult;
        }

        public LateFeeReportListModel GetLateFeeReportList(LateFeeReportFilter filter, int pageNumber, int pageSize)
        {
            var lateFeeReportList = GetLateFeeReportListFilter(filter);

            var finalcollection = lateFeeReportList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new LateFeeReportListModel
            {
                Collection = finalcollection.Select(_reportFactory.CreateViewModel).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, lateFeeReportList.Count())
            };
        }

        private IQueryable<FranchiseeInvoice> GetLateFeeReportListFilter(LateFeeReportFilter filter)
        {
            var franchiseeInvoiceList = _franchiseeInvoiceRepository.Table.Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                                        && (string.IsNullOrEmpty(filter.Text) || (x.InvoiceId.ToString().Equals(filter.Text)))
                                        && (filter.DueDateStart == null || x.Invoice.DueDate >= filter.DueDateStart)
                                        && (filter.DueDateEnd == null || x.Invoice.DueDate <= filter.DueDateEnd)
                                        && (filter.StatusId < 1 || x.Invoice.StatusId == filter.StatusId)
                                        && (filter.LateFeeTypeId < 1 | (x.Invoice.InvoiceItems.Any(y => y.LateFeeInvoiceItem != null && y.LateFeeInvoiceItem.LateFeeTypeId == filter.LateFeeTypeId)))
                                        && (filter.StartDate == null
                                            || (x.SalesDataUpload != null ? x.SalesDataUpload.PeriodEndDate >= filter.StartDate
                                            : x.Invoice.InvoiceItems.Any(y => y.LateFeeInvoiceItem != null && y.LateFeeInvoiceItem.EndDate >= filter.StartDate)))
                                        && (filter.EndDate == null
                                            || (x.SalesDataUpload != null ? x.SalesDataUpload.PeriodEndDate <= filter.EndDate
                                            : x.Invoice.InvoiceItems.Any(y => y.LateFeeInvoiceItem != null && y.LateFeeInvoiceItem.EndDate <= filter.StartDate)))
                                        && x.Invoice.InvoiceItems.Any(y => y.LateFeeInvoiceItem != null));
            franchiseeInvoiceList = _sortingHelper.ApplySorting(franchiseeInvoiceList, x => x.Invoice.DueDate, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Franchisee":
                        franchiseeInvoiceList = _sortingHelper.ApplySorting(franchiseeInvoiceList, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "StartDate":
                        franchiseeInvoiceList = _sortingHelper.ApplySorting(franchiseeInvoiceList, x => x.SalesDataUpload.PeriodStartDate, filter.SortingOrder);
                        break;
                    case "EndDate":
                        franchiseeInvoiceList = _sortingHelper.ApplySorting(franchiseeInvoiceList, x => x.SalesDataUpload.PeriodEndDate, filter.SortingOrder);
                        break;
                    case "DueDate":
                        franchiseeInvoiceList = _sortingHelper.ApplySorting(franchiseeInvoiceList, x => x.Invoice.DueDate, filter.SortingOrder);
                        break;
                    case "InvoiceId":
                        franchiseeInvoiceList = _sortingHelper.ApplySorting(franchiseeInvoiceList, x => x.SalesDataUpload.TotalAmount, filter.SortingOrder);
                        break;
                    case "Status":
                        franchiseeInvoiceList = _sortingHelper.ApplySorting(franchiseeInvoiceList, x => x.Invoice.Lookup.Name, filter.SortingOrder);
                        break;
                }
            }

            return franchiseeInvoiceList;
        }

        public bool DownloadSalesReport(ServiceReportListFilter filter, out string fileName)
        {
            fileName = string.Empty;

            if (filter.ClassTypeId <= 0 && filter.FranchiseeId <= 0 && filter.ServiceTypeId <= 0 && filter.PaymentDateStart == null && filter.PaymentDateEnd == null)
            {
                var clock = new Clock();
                var currentDate = _clock.UtcNow;
                filter.PaymentDateStart = clock.FirstDayOfMonth(currentDate);
                filter.PaymentDateEnd = clock.LastDayOfMonth(currentDate);
            }

            var reportCollection = new List<ServiceReportViewModel>();
            IEnumerable<FranchiseeServiceClassCollection> reportList = GetFranchiseeServiceClassList(filter).ToList();

            //prepare item collection
            foreach (var item in reportList)
            {
                var model = _reportFactory.CreateViewModel(item);
                reportCollection.Add(model);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/salesReport-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }

        public bool DownloadLateFeeReport(LateFeeReportFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<LateFeeReportViewModel>();
            IEnumerable<FranchiseeInvoice> franchiseeInvoiceList = GetLateFeeReportListFilter(filter).ToList();

            //prepare item collection
            foreach (var item in franchiseeInvoiceList)
            {
                var model = _reportFactory.CreateViewModel(item);
                reportCollection.Add(model);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/lateFeeReport-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }

        public TopLeadersListModel GetServiceLeaderList(TopLeadersFilter filter)
        {
            var serviceLeadersList = ServiceReportViewModel(filter).ToList();

            return new TopLeadersListModel
            {
                Collection = serviceLeadersList.OrderByDescending(x => x.Collection.Count()),
                Filter = filter,
            };
        }

        private List<TopLeadersViewModel> ServiceReportViewModel(TopLeadersFilter filter)
        {
            var serviceListids = filter.TypeIds.Split(',').ToArray();
            var serviceTypeIds = new List<long>();
            foreach (var item in serviceListids)
            {
                serviceTypeIds.Add(Convert.ToInt64(item));
            }
            var serviceList = _serviceTypeRepository.Table.Where(x => x.CategoryId != (long)Organizations.Enum.ServiceTypeCategory.ProductChannel && x.IsActive);
            var servicesTypes = serviceList.Where(x => (serviceTypeIds.Count() <= 0 || serviceTypeIds.Contains(x.Id))).ToList();
            var listServiceLeader = new List<TopLeadersInfoModel>();
            var finalList = new List<TopLeadersViewModel>();

            if (servicesTypes.Any())
            {
                foreach (var servicesType in servicesTypes)
                {
                    if (servicesType.Id > 0)
                    {
                        var listPayment2 = (from franchiseeSalesPayment in _franchiseeSalesPaymentRepository.Table
                                            join franchiseeSales in _franchiseeSalesRepository.Table on franchiseeSalesPayment.FranchiseeSalesId equals franchiseeSales.Id
                                            join payment in _paymentRepository.Table on franchiseeSalesPayment.PaymentId equals payment.Id
                                            join paymentItem in _paymentItemRepository.Table on payment.Id equals paymentItem.PaymentId
                                            where (paymentItem.ItemId == servicesType.Id && (filter.StartDate == null || payment.Date >= filter.StartDate)
                                            && (filter.EndDate == null || payment.Date <= filter.EndDate))
                                            select payment.Id).ToList();



                        var listPayment = (from franchiseeSalesPayment in _franchiseeSalesPaymentRepository.Table
                                           join franchiseeSales in _franchiseeSalesRepository.Table on franchiseeSalesPayment.FranchiseeSalesId equals franchiseeSales.Id
                                           join payment in _paymentRepository.Table on franchiseeSalesPayment.PaymentId equals payment.Id
                                           join paymentItem in _paymentItemRepository.Table on payment.Id equals paymentItem.PaymentId
                                           where (paymentItem.ItemId == servicesType.Id
                                           && (filter.StartDate == null || payment.Date >= filter.StartDate)
                                           && (filter.EndDate == null || payment.Date <= filter.EndDate))
                                           select new
                                           {
                                               FranchiseeId = franchiseeSales.FranchiseeId,
                                               Franchisee = franchiseeSales.Franchisee,
                                               Payment = payment
                                           }).GroupBy(y => new { y.FranchiseeId, y.Franchisee });

                        var finalResult = listPayment.Select(x => new TopLeadersInfoModel
                        {
                            FranchiseeId = x.Key.FranchiseeId,
                            Franchisee = x.Key.Franchisee.Organization.Name,
                            CurrencyCode = x.Key.Franchisee.Currency,
                            CurrencyRate = x.Select(y => y.Payment.CurrencyExchangeRate.Rate).FirstOrDefault(),
                            TotalSales = x.Sum(y => y.Payment.Amount)
                        }).OrderByDescending(z => z.TotalSales).ToList();

                        for (int i = 0; i < finalResult.Count; i++)
                        {
                            finalResult[i].Rank = i + 1;
                        }
                        var topTenList = finalResult.Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)).Take(10);

                        var totalSales = new TopLeadersInfoModel { };
                        var totalTopSales = new TopLeadersInfoModel { };
                        if (finalResult.Any())
                        {
                            totalSales.TotalSales = finalResult.Sum(x => x.TotalSales);
                            totalSales.CurrencyCode = finalResult.FirstOrDefault().CurrencyCode;
                            totalSales.CurrencyRate = finalResult.FirstOrDefault().CurrencyRate;
                        }
                        if (topTenList != null && topTenList.Any())
                        {
                            totalTopSales.TotalSales = topTenList.Sum(x => x.TotalSales);
                            totalTopSales.CurrencyCode = topTenList.FirstOrDefault().CurrencyCode;
                            totalTopSales.CurrencyRate = topTenList.FirstOrDefault().CurrencyRate;
                        };
                        var list = new TopLeadersViewModel
                        {
                            Collection = topTenList.Select(x => _reportFactory.CreateViewModel(x, totalSales.TotalSales)),
                            Type = servicesType.Name,
                            TypeId = servicesType.Id,
                            TotalSales = totalSales,
                            TotalTopSales = totalTopSales
                        };

                        if (filter.LoggedInFranchiseeId != null && !list.Collection.Any(x => x.FranchiseeId == filter.LoggedInFranchiseeId))
                        {
                            list.FranchiseeInfo = finalResult.Where(x => (filter.LoggedInFranchiseeId == null) || x.FranchiseeId == filter.LoggedInFranchiseeId).FirstOrDefault();
                            if (list.FranchiseeInfo != null)
                            {
                                list.FranchiseeInfo.Percentage = list.TotalSales.TotalSales > 0
                                    ? Math.Round((list.FranchiseeInfo.TotalSales / list.TotalSales.TotalSales) * 100, 2) : 0;
                                if (list.TotalSales != null)
                                {
                                    list.TotalSales.FranchiseeId = filter.LoggedInFranchiseeId != null ? filter.LoggedInFranchiseeId.Value : 0;
                                    list.TotalSales.CurrencyCode = list.FranchiseeInfo.CurrencyCode;
                                    list.TotalSales.CurrencyRate = list.FranchiseeInfo.CurrencyRate;
                                }
                                if (list.TotalTopSales != null)
                                {
                                    list.TotalTopSales.FranchiseeId = filter.LoggedInFranchiseeId != null ? filter.LoggedInFranchiseeId.Value : 0;
                                    list.TotalTopSales.CurrencyCode = list.FranchiseeInfo.CurrencyCode;
                                    list.TotalTopSales.CurrencyRate = list.FranchiseeInfo.CurrencyRate;
                                }
                            }
                        }

                        finalList.Add(list);
                    }
                }
            }
            return finalList;
        }

        public TopLeadersListModel GetClassLeaderList(TopLeadersFilter filter)
        {
            var classLeadersList = GetClassReportFilter(filter).ToList();

            return new TopLeadersListModel
            {
                Collection = classLeadersList.OrderByDescending(x => x.Collection.Count()),
                Filter = filter,
            };
        }

        private List<TopLeadersViewModel> GetClassReportFilter(TopLeadersFilter filter)
        {
            var classListids = filter.TypeIds.Split(',').ToArray();
            var classTypeIds = new List<long>();
            var classTypes = new List<MarketingClass>();
            var marketingClass = _marketingClassRepository.Table.ToList();
            foreach (var item in classListids)
            {
                classTypeIds.Add(Convert.ToInt64(item));
            }
            if (classTypeIds.Count() > 30)
            {
                var classTypesLocal = new List<MarketingClass>();
                foreach (var classType in classTypeIds)
                {
                    var isPresent = marketingClass.Where(x => (classType == (x.Id))).ToList();
                    if (isPresent.Count() > 0)
                    {
                        classTypesLocal.AddRange(isPresent);
                    }
                }
                classTypes = classTypesLocal;
            }
            else

            {
                classTypes = marketingClass.Where(x => (classTypeIds.Count() <= 0
                               || classTypeIds.Contains(x.Id))).ToList();
            }
            var finalList = new List<TopLeadersViewModel>();

            if (classTypes.Any())
            {
                foreach (var classType in classTypes)
                {
                    if (classType.Id > 0)
                    {
                        var listPayment = (from franchiseeSalesPayment in _franchiseeSalesPaymentRepository.Table
                                           join franchiseeSales in _franchiseeSalesRepository.Table on franchiseeSalesPayment.FranchiseeSalesId equals franchiseeSales.Id
                                           join payment in _paymentRepository.Table on franchiseeSalesPayment.PaymentId equals payment.Id
                                           join paymentItem in _paymentItemRepository.Table on payment.Id equals paymentItem.PaymentId
                                           where (franchiseeSales.ClassTypeId == classType.Id
                                           && (filter.StartDate == null || payment.Date >= filter.StartDate)
                                           && (filter.EndDate == null || payment.Date <= filter.EndDate))
                                           select new
                                           {
                                               FranchiseeId = franchiseeSales.FranchiseeId,
                                               Franchisee = franchiseeSales.Franchisee,
                                               Payment = payment
                                           }).GroupBy(y => new { y.FranchiseeId, y.Franchisee }).ToList();

                        var finalResult = listPayment.Select(x => new TopLeadersInfoModel
                        {
                            FranchiseeId = x.Key.FranchiseeId,
                            Franchisee = x.Key.Franchisee.Organization.Name,
                            CurrencyCode = x.Key.Franchisee.Currency,
                            CurrencyRate = x.Select(y => y.Payment.CurrencyExchangeRate.Rate).FirstOrDefault(),
                            TotalSales = x.Sum(y => y.Payment.Amount),
                        }).OrderByDescending(z => z.TotalSales).ToList();

                        for (int i = 0; i < finalResult.Count; i++)
                        {
                            finalResult[i].Rank = i + 1;
                        }

                        var topTenList = finalResult.Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)).Take(10);

                        var totalSales = new TopLeadersInfoModel { };
                        var totalTopSales = new TopLeadersInfoModel { };
                        if (finalResult.Any())
                        {
                            totalSales.TotalSales = finalResult.Sum(x => x.TotalSales);
                            totalSales.CurrencyCode = finalResult.FirstOrDefault().CurrencyCode;
                            totalSales.CurrencyRate = finalResult.FirstOrDefault().CurrencyRate;
                        }
                        if (topTenList != null && topTenList.Any())
                        {
                            totalTopSales.TotalSales = topTenList.Sum(x => x.TotalSales);
                            totalTopSales.CurrencyCode = topTenList.FirstOrDefault().CurrencyCode;
                            totalTopSales.CurrencyRate = topTenList.FirstOrDefault().CurrencyRate;
                        };

                        var list = new TopLeadersViewModel
                        {
                            Collection = topTenList.Select(x => _reportFactory.CreateViewModel(x, totalSales.TotalSales)),
                            Type = classType.Name,
                            TypeId = classType.Id,
                            TotalSales = totalSales,
                            TotalTopSales = totalTopSales
                        };
                        if (filter.LoggedInFranchiseeId != null && !list.Collection.Any(x => x.FranchiseeId == filter.LoggedInFranchiseeId))
                        {
                            list.FranchiseeInfo = finalResult.Where(x => (filter.LoggedInFranchiseeId == null) || x.FranchiseeId == filter.LoggedInFranchiseeId).FirstOrDefault();
                            if (list.FranchiseeInfo != null)
                            {
                                list.FranchiseeInfo.Percentage = list.TotalSales.TotalSales > 0
                                    ? Math.Round((list.FranchiseeInfo.TotalSales / list.TotalSales.TotalSales) * 100, 2) : 0;
                                if (list.TotalSales != null)
                                {
                                    list.TotalSales.FranchiseeId = filter.LoggedInFranchiseeId != null ? filter.LoggedInFranchiseeId.Value : 0;
                                    list.TotalSales.CurrencyCode = list.FranchiseeInfo.CurrencyCode;
                                    list.TotalSales.CurrencyRate = list.FranchiseeInfo.CurrencyRate;
                                }
                                if (list.TotalTopSales != null)
                                {
                                    list.TotalTopSales.FranchiseeId = filter.LoggedInFranchiseeId != null ? filter.LoggedInFranchiseeId.Value : 0;
                                    list.TotalTopSales.CurrencyCode = list.FranchiseeInfo.CurrencyCode;
                                    list.TotalTopSales.CurrencyRate = list.FranchiseeInfo.CurrencyRate;
                                }
                            }
                        }
                        finalList.Add(list);
                    }
                }
            }
            return finalList;
        }

        public UploadBatchReportListModel GetBatchReport(UploadReportFilter filter, int pageNumber, int pageSize)
        {
            var uploadBatchReportList = GetBatchReportFilterList(filter);


            if (filter.IsOnTimeUpload == 1)
            {
                var changedUploadBatchReportList = uploadBatchReportList.AsEnumerable().Where(x => x.ExpectedUploadDate >= x.UploadedOn
                             && x.StartDate <= x.UploadedOn).ToList();
                uploadBatchReportList = changedUploadBatchReportList.AsQueryable();
            }
            var finalcollection = uploadBatchReportList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new UploadBatchReportListModel
            {
                Collection = finalcollection.Select(x => _reportFactory.CreateViewModel(x)).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, uploadBatchReportList.Count())
            };
        }

        private IQueryable<BatchUploadRecord> GetBatchReportFilterList(UploadReportFilter filter)
        {

            //if (filter.Year == null)
            //    filter.Year = _clock.UtcNow.Year;

            var startDateOfFilter = default(DateTime);
            var endDateOfFilter = default(DateTime);
            if (filter.PeriodStartDate != default(DateTime?))
            {
                startDateOfFilter = filter.PeriodStartDate.Value.Date;
                endDateOfFilter = filter.PeriodEndDate.Value.Date;
            }

            var uploadList = _batchUploadRecordRepository.Table.Where(x => x.Franchisee.Organization.IsActive && !x.IsCorrectUploaded
                                                                    && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                                                    //&& (filter.Year == null || x.StartDate.Year == filter.Year)
                                                                    && (!filter.PeriodStartDate.HasValue
                                                                        || (x.StartDate >= startDateOfFilter.Date && x.EndDate <= endDateOfFilter.Date)
                                                                        || (x.StartDate <= startDateOfFilter.Date && x.EndDate >= startDateOfFilter.Date)
                                                                        || (x.StartDate <= endDateOfFilter.Date && x.EndDate >= endDateOfFilter.Date))
                                                                     && (filter.StatusId == null || (filter.StatusId == 0 ? x.UploadedOn == default(DateTime?) : x.UploadedOn != default(DateTime?)))
                                                                        ).Distinct();

            uploadList = _sortingHelper.ApplySorting(uploadList, x => x.Id, (long)SortingOrder.Desc);
            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "ID":
                        uploadList = _sortingHelper.ApplySorting(uploadList, x => x.Id, filter.SortingOrder);
                        break;
                    case "Franchisee":
                        uploadList = _sortingHelper.ApplySorting(uploadList, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "FeeProfile":
                        uploadList = _sortingHelper.ApplySorting(uploadList, x => x.PaymentFrequency.Name, filter.SortingOrder);
                        break;
                    case "WaitPeriod":
                        uploadList = _sortingHelper.ApplySorting(uploadList, x => x.WaitPeriod, filter.SortingOrder);
                        break;
                    case "ExpectedDate":
                        uploadList = _sortingHelper.ApplySorting(uploadList, x => x.ExpectedUploadDate, filter.SortingOrder);
                        break;
                    case "ActualDate":
                        uploadList = _sortingHelper.ApplySorting(uploadList, x => x.UploadedOn, filter.SortingOrder);
                        break;
                }
            }
            return uploadList;
        }

        public bool DownloadUploadReport(UploadReportFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<UploadBatchCollectionViewModel>();
            IEnumerable<BatchUploadRecord> franchiseeInvoiceList = GetBatchReportFilterList(filter).ToList();

            //prepare item collection
            foreach (var item in franchiseeInvoiceList)
            {
                var model = _reportFactory.CreateViewModel(item);
                reportCollection.Add(model);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/missingUploadReport-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }

        public ARReportListModel GetARReportList(ArReportFilter filter)
        {
            var uploadBatchReportList = GetARReportFilterList(filter);
            return new ARReportListModel
            {
                Collection = uploadBatchReportList.ToList(),
                Filter = filter,
            };
        }

        private IEnumerable<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel> GetARReportFilterList(ArReportFilter filter)
        {
            var currentWeekDate = _clock.UtcNow.Date;
            //currentWeekDate = GetWeekDay(currentWeekDate, _settings.WeeklyReminderDay);
            //currentWeekDate = GetWeekDay(currentWeekDate, _settings.WeeklyReminderDay);
            //var previousWeekDay = currentWeekDate.AddDays(-6);
            var previousWeekDay = currentWeekDate.AddDays(-30);
            var unpaidInvoiceList = _franchiseeInvoiceRepository.Table.Where(x => x.Invoice.StatusId == (long)InvoiceStatus.Unpaid
                                            && x.Invoice.InvoiceItems.Sum(y => y.Amount) > 0).ToList();
            var listInvoice2 = GetArReportModel(unpaidInvoiceList, previousWeekDay, currentWeekDate, filter);
            return listInvoice2;
        }
        public static DateTime GetWeekDay(DateTime dt, int dayOfWeek)
        {
            int diff = Convert.ToInt32(dt.DayOfWeek) - dayOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
        public IEnumerable<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel> GetArReportModel(IEnumerable<FranchiseeInvoice> franchiseeInvoices, DateTime startDate, DateTime endDate, ArReportFilter filter = null)
        {
            var invoiceCollection = new List<WeeklyNotificationReportViewModel>();
            var invoiceCollectionDateWise = new List<WeeklyNotificationReportViewModel>();

            DateTime currentUtc = _clock.UtcNow.Date.AddDays(-1);
            if (filter != null)
            {
                currentUtc = filter.ReportDateStart.GetValueOrDefault();
            }
            DateTime pastUtc = currentUtc.AddDays(-30);

            var invoiceList = franchiseeInvoices.OrderByDescending(x => x.FranchiseeId).ThenByDescending(x => x.Invoice.GeneratedOn).ToList();

            foreach (var item in invoiceList)
            {
                var model = _reportFactory.CreateViewModelForNotificationForARReport(item, startDate, endDate);
                invoiceCollection.Add(model);
            }

            //var listInvoice = new List<WeeklyUnpaidInvoiceNotificationReportModel>();
            var listInvoice2 = new List<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel>();

            // New Code Added for UnPaid Invoice Franchisee Wise

            List<long> franchiseeIds = invoiceCollection.Where(x => (filter == null || filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId)).Select(x => x.FranchiseeId).Distinct().ToList();

            //invoiceCollectionDateWise = invoiceCollection.Where(x => x.StartDate >= pastUtc && x.EndDate)
            foreach (long franchiseeId in franchiseeIds)
            {
                var model2 = CreateViewModelForNotification(invoiceCollection, franchiseeId, currentUtc, pastUtc);
                listInvoice2.Add(model2);
            }
            listInvoice2 = listInvoice2.OrderByDescending(x => x.TotalInt).ToList();
            return listInvoice2;
        }

        private WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel CreateViewModelForNotification(List<WeeklyNotificationReportViewModel> franchiseeInvoice, long franchiseeId, DateTime endDateTime, DateTime startDateTime)
        {

            DateTime startDate = startDateTime;
            string oneMonthInvoice = string.Empty;
            string twoMonthInvoice = string.Empty;
            string threemonthInvoice = string.Empty;
            string moreThanThreemonthInvoice = string.Empty;
            decimal totalAmount = 0;
            var franchiseeName = franchiseeInvoice.Where(x => x.FranchiseeId == franchiseeId).Select(x => x.Franchisee != null ? x.Franchisee : "").FirstOrDefault();
            var invoicesList = franchiseeInvoice.AsQueryable().Where(x => x.FranchiseeId == franchiseeId).Select(x => x).ToList();

            var invpoice = franchiseeInvoice.AsQueryable().Where(x => x.FranchiseeId == franchiseeId && x.EndDate >= startDateTime.Date && x.EndDate <= endDateTime.Date).Select(x => x).ToList();
            var last1Month = franchiseeInvoice.AsQueryable().Where(x => x.FranchiseeId == franchiseeId && x.EndDate >= startDateTime.Date && x.EndDate <= endDateTime.Date).Select(x => x.PayableAmount).Sum();

            endDateTime = startDateTime.AddDays(-1);
            startDateTime = startDateTime.AddDays(-30);


            var last2Month = franchiseeInvoice.AsQueryable().Where(x => x.FranchiseeId == franchiseeId && x.EndDate >= startDateTime.Date && x.EndDate <= endDateTime.Date).Select(x => x.PayableAmount).Sum();
            endDateTime = startDateTime.AddDays(-1);
            startDateTime = startDateTime.AddDays(-30);


            var moreThan2Month = franchiseeInvoice.AsQueryable().Where(x => x.FranchiseeId == franchiseeId && x.EndDate >= startDateTime.Date && x.EndDate <= endDateTime.Date).Select(x => x.PayableAmount).Sum();
            endDateTime = startDateTime.AddDays(-1);
            startDateTime = startDateTime.AddDays(-30);


            var moreThan3Month = franchiseeInvoice.AsQueryable().Where(x => x.FranchiseeId == franchiseeId && x.EndDate <= endDateTime.Date).Select(x => x.PayableAmount).Sum();
            var total = moreThan3Month + last1Month + moreThan2Month + last2Month;
            totalAmount += total;
            string totalString = "$ " + (total);
            var invoiceModel = new WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel
            {
                Franchisee = franchiseeName,
                moreThanNinety = "$ " + moreThan3Month,
                Thirty = "$ " + last1Month,
                Sixty = "$ " + last2Month,
                Ninety = "$ " + moreThan2Month,
                Total = totalString,
                TotalInt = total
            };
            return invoiceModel;
        }

        public PriceEstimatePageViewModel GetPriceEstimateList(
    PriceEstimateGetModel getmodel, long userId, long roleUserId)
        {
            // 1️⃣ Build ServiceTag query (SQL level filtering)
            var servicesQuery = _servicesTagRepository.Table
                .Where(x => x.Service != null && x.IsActive);

            if (getmodel.CategoryId != default(int))
                servicesQuery = servicesQuery.Where(x => x.CategoryId == getmodel.CategoryId);

            if (getmodel.ServiceTypeId != null && getmodel.ServiceTypeId.Count > 0)
                servicesQuery = servicesQuery.Where(x => getmodel.ServiceTypeId.Contains(x.ServiceTypeId));

            if (getmodel.ListOfService != null && getmodel.ListOfService.Count > 0)
                servicesQuery = servicesQuery.Where(x => getmodel.ListOfService.Contains(x.Service));

            var servicesTag = servicesQuery.ToList();

            // 2️⃣ Load price estimates once and group by ServiceTagId
            var priceEstimatesGrouped = _priceEstimateServicesRepository.Table
                .Where(x => x.ServiceTagId != null)
                .ToList()
                .GroupBy(x => x.ServiceTagId.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<PriceEstimateViewModel>();

            foreach (var service in servicesTag)
            {
                var pricemodel = new PriceEstimateViewModel
                {
                    ServiceTagId = service.Id,
                    Note = service.Notes,
                    Service = service.Service,
                    ServiceType = service.ServiceType.Name,
                    MaterialType = service.MaterialType,
                    Category = service.Category.Name,
                    CategoryId = service.Category.Id,
                    Unit = GetUnit(service.Category.Id),
                    HasTwoPriceColumns =
                        service.CategoryId == (long)ServiceTagCategory.EVENT ||
                        service.CategoryId == (long)ServiceTagCategory.LINEARFT ||
                        service.CategoryId == (long)ServiceTagCategory.AREA,
                    IsActiveService = getmodel.ServiceTagSelectedIds?.Contains(service.Id) ?? false,
                    IsDisabledService = getmodel.SelectedCategoryId != null &&
                                        getmodel.SelectedCategoryId != service.Category.Id,
                    CategoryType = service.Category.Name == "TIME"
                };

                if (!priceEstimatesGrouped.TryGetValue(service.Id, out var priceEstimates))
                {
                    pricemodel.HasPriceValues = false;
                    result.Add(pricemodel);
                    continue;
                }

                // 🔒 Ensure deterministic order (LAST = highest Id)
                priceEstimates = priceEstimates
                    .OrderBy(x => x.Id)
                    .ToList();

                pricemodel.HasPriceValues = true;

                var first = priceEstimates[0];
                pricemodel.BulkCorporatePrice = first.BulkCorporatePrice;
                pricemodel.BulkCorporateAdditionalPrice = first.BulkCorporateAdditionalPrice;

                // -------- Franchisee Price --------
                var validFranchiseePrices = priceEstimates
                    .Where(x => x.IsPriceChangedByFranchisee && x.FranchiseePrice != null)
                    .ToList();

                if (validFranchiseePrices.Count > 0)
                {
                    // ✅ EXACT original average logic
                    pricemodel.AverageFranchiseePrice =
                        Math.Round(
                            (decimal)(
                                priceEstimates.Sum(x => x.FranchiseePrice ?? 0) /
                                (decimal)priceEstimates.Count
                            ),
                            2
                        );

                    // ✅ LAST record wins for max price
                    var maxFranchiseePriceRecord = validFranchiseePrices
                        .OrderBy(x => x.FranchiseePrice)
                        .ThenBy(x => x.Id)
                        .Last();

                    pricemodel.MaximumFranchiseePrice =
                        maxFranchiseePriceRecord.FranchiseePrice;

                    pricemodel.MaximumFranchiseePriceName =
                        maxFranchiseePriceRecord.Franchisee.Organization.Name;
                }

                // -------- Franchisee Additional Price --------
                var validAdditionalPrices = priceEstimates
                    .Where(x => x.IsPriceChangedByFranchisee && x.FranchiseeAdditionalPrice != null)
                    .ToList();

                if (validAdditionalPrices.Count > 0)
                {
                    // ✅ EXACT original average logic
                    pricemodel.AverageFranchiseeAdditionalPrice =
                        Math.Round(
                            (decimal)(
                                priceEstimates.Sum(x => x.FranchiseeAdditionalPrice ?? 0) /
                                (decimal)priceEstimates.Count
                            ),
                            2
                        );

                    // ✅ LAST record wins for max additional price
                    var maxAdditionalPriceRecord = validAdditionalPrices
                        .OrderBy(x => x.FranchiseeAdditionalPrice)
                        .ThenBy(x => x.Id)
                        .Last();

                    pricemodel.MaximumFranchiseeAdditionalPrice =
                        maxAdditionalPriceRecord.FranchiseeAdditionalPrice;

                    pricemodel.MaximumFranchiseeAdditionalPriceName =
                        maxAdditionalPriceRecord.Franchisee.Organization.Name;
                }

                result.Add(pricemodel);
            }

            // 3️⃣ Sorting (unchanged behavior)
            var query = result.AsQueryable();

            if (!string.IsNullOrEmpty(getmodel.SortingColumn))
            {
                switch (getmodel.SortingColumn)
                {
                    case "Material":
                        query = _sortingHelper.ApplySorting(query, x => x.MaterialType, getmodel.SortingOrder);
                        break;
                    case "CorporatePrice":
                        query = _sortingHelper.ApplySorting(query, x => x.BulkCorporatePrice, getmodel.SortingOrder);
                        break;
                    case "AverageFranchiseePrice":
                        query = _sortingHelper.ApplySorting(query, x => x.AverageFranchiseePrice, getmodel.SortingOrder);
                        break;
                    case "MaximumFranchiseePrice":
                        query = _sortingHelper.ApplySorting(query, x => x.MaximumFranchiseePrice, getmodel.SortingOrder);
                        break;
                    case "FranchiseeName":
                        query = _sortingHelper.ApplySorting(query, x => x.MaximumFranchiseePriceName, getmodel.SortingOrder);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Service).ThenBy(x => x.ServiceType);
            }

            return new PriceEstimatePageViewModel
            {
                PriceEstimateViewModelList = query.ToList()
            };
        }



        //public PriceEstimatePageViewModel GetPriceEstimateList(PriceEstimateGetModel getmodel, long userId, long roleUserId)
        //{
        //    PriceEstimatePageViewModel model = new PriceEstimatePageViewModel();
        //    List<PriceEstimateViewModel> ListModel = new List<PriceEstimateViewModel>();
        //    var servicesTag = _servicesTagRepository.Table.Where(x => x.Service != null && x.IsActive).ToList();
        //    if (getmodel.CategoryId != default(int))
        //    {
        //        servicesTag = servicesTag.Where(x => x.CategoryId == getmodel.CategoryId).ToList();
        //    }
        //    if (getmodel.ServiceTypeId.Count > 0)
        //    {
        //        servicesTag = servicesTag.Where(x => getmodel.ServiceTypeId.Contains(x.ServiceTypeId)).ToList();
        //    }
        //    if (getmodel.ListOfService != null && getmodel.ListOfService.Count > 0)
        //    {
        //        servicesTag = servicesTag.Where(x => getmodel.ListOfService.Contains(x.Service)).ToList();
        //    }
        //    var priceEstimatesList = _priceEstimateServicesRepository.Table.Where(x => x.ServiceTagId != null).ToList();
        //    foreach (var service in servicesTag)
        //    {
        //        if(service.Id == 1156)
        //        {

        //        }
        //        PriceEstimateViewModel pricemodel = new PriceEstimateViewModel();
        //        var priceEstimates = priceEstimatesList.Where(x => x.ServiceTagId == service.Id).ToList();
        //        if (priceEstimates.Count > 0)
        //        {
        //            var priceEstimatesForFranchisee = priceEstimates.Where(x => x.IsPriceChangedByFranchisee && x.FranchiseePrice != null).ToList();
        //            var priceEstimatesForFranchiseePriceNotExceed = priceEstimatesForFranchisee.Where(y => y.FranchiseePrice < (150 * y.CorporatePrice) / 100).ToList();
        //            var priceEstimatesForFranchiseeAdditional = priceEstimates.Where(x => x.IsPriceChangedByFranchisee && x.FranchiseeAdditionalPrice != null).ToList();
        //            var priceEstimatesForFranchiseeAdditionalPriceNotExceed = priceEstimatesForFranchiseeAdditional.Where(x => x.FranchiseeAdditionalPrice < (150 * x.CorporateAdditionalPrice) / 100).ToList();
        //            pricemodel.HasPriceValues = true;
        //            pricemodel.BulkCorporatePrice = priceEstimates.FirstOrDefault().BulkCorporatePrice;
        //            pricemodel.BulkCorporateAdditionalPrice = priceEstimates.FirstOrDefault().BulkCorporateAdditionalPrice;
        //            if (priceEstimatesForFranchiseePriceNotExceed.Count > 0)
        //            {
        //                //pricemodel.AverageFranchiseePrice = (decimal)(Math.Round((double)priceEstimatesForFranchiseePriceNotExceed.Sum(x => x.FranchiseePrice) / (priceEstimatesForFranchiseePriceNotExceed.Count), 2));
        //                pricemodel.AverageFranchiseePrice = (decimal)(Math.Round((double)priceEstimates.Sum(x => x.FranchiseePrice) / (priceEstimates.Count), 2));
        //                //pricemodel.MaximumFranchiseePrice = priceEstimatesForFranchiseePriceNotExceed.Max(x => x.FranchiseePrice);
        //                pricemodel.MaximumFranchiseePrice = priceEstimates.Where(y => y.IsPriceChangedByFranchisee && y.FranchiseePrice != null).Max(x => x.FranchiseePrice);
        //                var maxFranchiseePriceNames = priceEstimates.Where(x => x.FranchiseePrice == pricemodel.MaximumFranchiseePrice && x.Franchisee?.Organization != null).Select(x => x.Franchisee.Organization.Name).ToList();
        //                pricemodel.MaximumFranchiseePriceName = String.Join(", ", maxFranchiseePriceNames);
        //            }
        //            if (priceEstimatesForFranchiseeAdditionalPriceNotExceed.Count > 0)
        //            {
        //                //pricemodel.AverageFranchiseeAdditionalPrice = (decimal)(Math.Round((double)priceEstimatesForFranchiseeAdditionalPriceNotExceed.Sum(x => x.FranchiseeAdditionalPrice) / (priceEstimatesForFranchiseeAdditionalPriceNotExceed.Count), 2));
        //                pricemodel.AverageFranchiseeAdditionalPrice = (decimal)(Math.Round((double)priceEstimates.Sum(x => x.FranchiseeAdditionalPrice) / (priceEstimates.Count), 2));
        //                //pricemodel.MaximumFranchiseeAdditionalPrice = priceEstimatesForFranchiseeAdditionalPriceNotExceed.Max(x => x.FranchiseeAdditionalPrice);
        //                pricemodel.MaximumFranchiseeAdditionalPrice = priceEstimates.Where(y => y.IsPriceChangedByFranchisee && y.FranchiseePrice != null).Max(x => x.FranchiseeAdditionalPrice);
        //                var maxFranchiseeAdditionalPriceNames = priceEstimates.Where(x => x.FranchiseeAdditionalPrice == pricemodel.MaximumFranchiseeAdditionalPrice && x.Franchisee?.Organization != null).Select(x => x.Franchisee.Organization.Name).ToList();
        //                pricemodel.MaximumFranchiseeAdditionalPriceName = String.Join(", ", maxFranchiseeAdditionalPriceNames);
        //            }
        //        }
        //        else
        //        {
        //            pricemodel.HasPriceValues = false;
        //        }
        //        pricemodel.ServiceTagId = service.Id;
        //        pricemodel.Note = service.Notes;
        //        pricemodel.Service = service.Service;
        //        pricemodel.ServiceType = service.ServiceType.Name;
        //        pricemodel.MaterialType = service.MaterialType;
        //        pricemodel.Category = service.Category.Name;
        //        pricemodel.CategoryId = service.Category.Id;
        //        pricemodel.Unit = GetUnit(service.Category.Id);
        //        pricemodel.HasTwoPriceColumns = (service.CategoryId == (long)ServiceTagCategory.EVENT || service.CategoryId == (long)ServiceTagCategory.LINEARFT || service.CategoryId == (long)ServiceTagCategory.AREA) ? true : false;
        //        pricemodel.IsActiveService = getmodel.ServiceTagSelectedIds != null ? getmodel.ServiceTagSelectedIds.Contains(pricemodel.ServiceTagId) : false;
        //        pricemodel.IsDisabledService = getmodel.SelectedCategoryId != null ? (getmodel.SelectedCategoryId == pricemodel.CategoryId ? false : true) : false;
        //        ListModel.Add(pricemodel);
        //        pricemodel.CategoryType = service.Category.Name != null && service.Category.Name == "TIME" ? true : false;
        //    }
        //    var queryList = ListModel.AsQueryable();
        //    if (!string.IsNullOrEmpty(getmodel.SortingColumn))
        //    {
        //        switch (getmodel.SortingColumn)
        //        {
        //            case "Material":
        //                queryList = _sortingHelper.ApplySorting(queryList, x => x.MaterialType, getmodel.SortingOrder);
        //                break;
        //            case "CorporatePrice":
        //                queryList = _sortingHelper.ApplySorting(queryList, x => x.BulkCorporatePrice, getmodel.SortingOrder);
        //                break;
        //            case "AverageFranchiseePrice":
        //                queryList = _sortingHelper.ApplySorting(queryList, x => x.AverageFranchiseePrice, getmodel.SortingOrder);
        //                break;
        //            case "MaximumFranchiseePrice":
        //                queryList = _sortingHelper.ApplySorting(queryList, x => x.MaximumFranchiseePrice, getmodel.SortingOrder);
        //                break;
        //            case "FranchiseeName":
        //                queryList = _sortingHelper.ApplySorting(queryList, x => x.MaximumFranchiseePriceName, getmodel.SortingOrder);
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        queryList = queryList.OrderBy(x => x.Service).ThenBy(x => x.ServiceType);
        //    }

        //    return new PriceEstimatePageViewModel
        //    {
        //        PriceEstimateViewModelList = queryList.ToList()
        //    };
        //}

        public PriceEstimateViewModel GetPriceEstimate(PriceEstimateGetModel model, long userId, long roleUserId)
        {

            var servicesTag = _servicesTagRepository.Table.FirstOrDefault(x => x.Service != null && x.Id == model.ServiceTagId && x.IsActive);
            PriceEstimateViewModel pricemodel = new PriceEstimateViewModel();
            pricemodel.ServiceTagId = servicesTag.Id;
            pricemodel.Note = servicesTag.Notes;
            pricemodel.Service = servicesTag.Service;
            pricemodel.ServiceType = servicesTag.ServiceType.Name;
            pricemodel.MaterialType = servicesTag.MaterialType;
            pricemodel.Category = servicesTag.Category.Name;
            pricemodel.CategoryId = servicesTag.Category.Id;
            pricemodel.HasTwoPriceColumns = (servicesTag.CategoryId == (long)ServiceTagCategory.EVENT || servicesTag.CategoryId == (long)ServiceTagCategory.LINEARFT || servicesTag.CategoryId == (long)ServiceTagCategory.AREA) ? true : false;
            var priceEstimateServices = new List<PriceEstimateServiceModel>();
            var frachiseeList = new List<Franchisee>();
            var assignedFranchiseeIdList = new List<long>();
            var priceEstimates = new List<PriceEstimateServices>();
            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                frachiseeList = _franchiseeRepository.Table.Where(x => !x.Organization.Name.StartsWith("0-") && x.Id != 2).ToList();
                priceEstimates = _priceEstimateServicesRepository.Table.Where(x => x.ServiceTagId == model.ServiceTagId).ToList();
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).ToList();
                frachiseeList = _franchiseeRepository.Table.Where(x => assignedFranchiseeIdList.Contains(x.Organization.Id) && x.Id != 2 && !x.Organization.Name.StartsWith("0-")).ToList();
                priceEstimates = _priceEstimateServicesRepository.Table.Where(x => x.ServiceTagId == model.ServiceTagId && assignedFranchiseeIdList.Contains((long)x.FranchiseeId)).ToList();
            }
            if (!model.ShowAllFranchisee)
            {
                foreach (var price in priceEstimates)
                {
                    PriceEstimateServiceModel priceEstimateServiceModel = new PriceEstimateServiceModel();
                    priceEstimateServiceModel.FranchiseeId = price.FranchiseeId;
                    priceEstimateServiceModel.Franchisee = price.Franchisee != null && price.Franchisee.Organization != null && price.Franchisee.Organization.Name != null ? price.Franchisee.Organization.Name : "";
                    priceEstimateServiceModel.FranchiseePrice = price.FranchiseePrice;
                    priceEstimateServiceModel.FranchiseeAdditionalPrice = price.FranchiseeAdditionalPrice;
                    priceEstimateServiceModel.CorporatePrice = price.CorporatePrice;
                    priceEstimateServiceModel.CorporateAdditionalPrice = price.CorporateAdditionalPrice;
                    priceEstimateServiceModel.AlternativeSolution = price.AlternativeSolution != null ? price.AlternativeSolution : "";
                    priceEstimateServiceModel.CategoryType = servicesTag.Category.Name == "TIME" ? true : false;
                    priceEstimateServices.Add(priceEstimateServiceModel);
                }
            }
            else
            {
                foreach (var franchisee in frachiseeList)
                {
                    var priceEstimateForFranchisee = priceEstimates.LastOrDefault(x => x.FranchiseeId == franchisee.Id);
                    PriceEstimateServiceModel priceEstimateServiceModel = new PriceEstimateServiceModel();
                    priceEstimateServiceModel.FranchiseeId = franchisee.Id;
                    priceEstimateServiceModel.Franchisee = franchisee.Organization.Name;
                    priceEstimateServiceModel.IsActiveFranchisee = model.SelectedFranchiseeIds != null ? model.SelectedFranchiseeIds.Contains(franchisee.Id) : false;
                    priceEstimateServiceModel.CategoryType = servicesTag.Category.Name == "TIME" ? true : false;
                    if (priceEstimateForFranchisee != null)
                    {
                        priceEstimateServiceModel.FranchiseePrice = priceEstimateForFranchisee.FranchiseePrice;
                        priceEstimateServiceModel.FranchiseeAdditionalPrice = priceEstimateForFranchisee.FranchiseeAdditionalPrice;
                        priceEstimateServiceModel.CorporatePrice = priceEstimateForFranchisee.CorporatePrice;
                        priceEstimateServiceModel.CorporateAdditionalPrice = priceEstimateForFranchisee.CorporateAdditionalPrice;
                        priceEstimateServiceModel.AlternativeSolution = priceEstimateForFranchisee.AlternativeSolution;
                    }
                    if (priceEstimateServiceModel.FranchiseePrice == null || !priceEstimateForFranchisee.IsPriceChangedByFranchisee)
                    {
                        priceEstimateServiceModel.IsFranchiseePriceZero = true;
                    }
                    else
                    {
                        priceEstimateServiceModel.IsFranchiseePriceZero = false;
                    }
                    if (priceEstimateServiceModel.FranchiseeAdditionalPrice == null || !priceEstimateForFranchisee.IsPriceChangedByFranchisee)
                    {
                        priceEstimateServiceModel.IsFranchiseeAdditionalPriceZero = true;
                    }
                    else
                    {
                        priceEstimateServiceModel.IsFranchiseeAdditionalPriceZero = false;
                    }
                    priceEstimateServices.Add(priceEstimateServiceModel);
                }
            }

            pricemodel.PriceEstimateServices = priceEstimateServices;
            return pricemodel;
        }

        private string GetUnit(long categoryId)
        {
            var unitCategories = _lookupRepository.Table.Where(x => x.LookupTypeId == (long)LookUpTypeCategory.Units).ToList();
            string unit = string.Empty;
            var unitCategory = new Lookup();
            if (categoryId == (long)ServiceTagCategory.AREA)
            {
                unitCategory = unitCategories.FirstOrDefault(x => x.Alias == ServiceTagCategory.AREA.ToString());
                unit = unitCategory != null ? unitCategory.Name : string.Empty;
            }
            else if (categoryId == (long)ServiceTagCategory.LINEARFT)
            {
                unitCategory = unitCategories.FirstOrDefault(x => x.Alias == ServiceTagCategory.LINEARFT.ToString());
                unit = unitCategory != null ? unitCategory.Name : string.Empty;
            }
            else if (categoryId == (long)ServiceTagCategory.EVENT)
            {
                unitCategory = unitCategories.FirstOrDefault(x => x.Alias == ServiceTagCategory.EVENT.ToString());
                unit = unitCategory != null ? unitCategory.Name : string.Empty;
            }
            else if (categoryId == (long)ServiceTagCategory.PRODUCTPRICE)
            {
                unitCategory = unitCategories.FirstOrDefault(x => x.Alias == ServiceTagCategory.PRODUCTPRICE.ToString());
                unit = unitCategory != null ? unitCategory.Name : string.Empty;
            }
            else if (categoryId == (long)ServiceTagCategory.MAINTAINANCE)
            {
                unitCategory = unitCategories.FirstOrDefault(x => x.Alias == ServiceTagCategory.MAINTAINANCE.ToString());
                unit = unitCategory != null ? unitCategory.Name : string.Empty;
            }
            else if (categoryId == (long)ServiceTagCategory.TAXRATE)
            {
                unitCategory = unitCategories.FirstOrDefault(x => x.Alias == ServiceTagCategory.TAXRATE.ToString());
                unit = unitCategory != null ? unitCategory.Name : string.Empty;
            }
            else if (categoryId == (long)ServiceTagCategory.TIME)
            {
                unitCategory = unitCategories.FirstOrDefault(x => x.Alias == ServiceTagCategory.TIME.ToString());
                unit = unitCategory != null ? unitCategory.Name : string.Empty;
            }
            else
            {
                unit = string.Empty;
            }
            return unit;
        }

        public bool SaveBulkCorporatePriceEstimate(PriceEstimateSaveCorporatePriceModel model, long roleUserId)
        {
            var frachiseeList = _franchiseeRepository.Table.Where(x => !x.Organization.Name.StartsWith("0-") && x.Id != 2).ToList();
            foreach (var serviceTag in model.PriceEstimateSaveBulkModel)
            {
                foreach (var franchisee in frachiseeList)
                {
                    if(serviceTag.BulkCorporateAdditionalPrice <= 0)
                    {
                        serviceTag.BulkCorporateAdditionalPrice = null;
                    }
                    if (serviceTag.BulkCorporatePrice <= 0)
                    {
                        serviceTag.BulkCorporatePrice = null;
                    }
                    var priceEstimateService = _priceEstimateServicesRepository.Table.FirstOrDefault(x => x.ServiceTagId == serviceTag.ServiceTagId && x.FranchiseeId == franchisee.Id);
                    var domain = new PriceEstimateServices();
                    domain.FranchiseeId = franchisee.Id;
                    domain.BulkCorporatePrice = serviceTag.BulkCorporatePrice;
                    domain.BulkCorporateAdditionalPrice = serviceTag.BulkCorporateAdditionalPrice;
                    if (priceEstimateService != null)
                    {
                        domain.AlternativeSolution = priceEstimateService.AlternativeSolution;
                        if (priceEstimateService.IsPriceChangedByFranchisee)
                        {
                            domain.IsPriceChangedByFranchisee = priceEstimateService.IsPriceChangedByFranchisee;
                        }
                        if (priceEstimateService.IsPriceChangedByAdmin)
                        {
                            domain.IsPriceChangedByAdmin = priceEstimateService.IsPriceChangedByAdmin;
                        }
                        if (!priceEstimateService.IsPriceChangedByAdmin)
                        {
                            domain.CorporatePrice = serviceTag.BulkCorporatePrice;
                            domain.CorporateAdditionalPrice = serviceTag.BulkCorporateAdditionalPrice;
                        }
                        else
                        {
                            domain.CorporatePrice = priceEstimateService.CorporatePrice;
                            domain.CorporateAdditionalPrice = priceEstimateService.CorporateAdditionalPrice;
                        }
                        if (!priceEstimateService.IsPriceChangedByFranchisee)
                        {
                            domain.FranchiseePrice = domain.CorporatePrice;
                            domain.FranchiseeAdditionalPrice = domain.CorporateAdditionalPrice;
                        }
                        else
                        {
                            domain.FranchiseePrice = priceEstimateService.FranchiseePrice;
                            domain.FranchiseeAdditionalPrice = priceEstimateService.FranchiseeAdditionalPrice;
                        }
                    }
                    else
                    {
                        domain.CorporatePrice = serviceTag.BulkCorporatePrice;
                        domain.CorporateAdditionalPrice = serviceTag.BulkCorporateAdditionalPrice;
                        domain.FranchiseePrice = serviceTag.BulkCorporatePrice;
                        domain.FranchiseeAdditionalPrice = serviceTag.BulkCorporateAdditionalPrice;
                    }
                    domain.ServiceTagId = serviceTag.ServiceTagId;
                    domain.IsNew = priceEstimateService != null ? false : true;
                    domain.Id = priceEstimateService != null ? priceEstimateService.Id : 0;
                    _priceEstimateServicesRepository.Save(domain);
                }
            }
            return true;
        }
        public bool BulkUpdateCorporatePrice(PriceEstimateBulkUpdateModel model, long roleUserId)
        {
            var frachiseeList = _franchiseeRepository.Table.Where(x => !x.Organization.Name.StartsWith("0-") && x.Id != 2).ToList();
            foreach (var serviceTag in model.ServiceTagId)
            {
                foreach (var franchisee in frachiseeList)
                {
                    var priceEstimateService = _priceEstimateServicesRepository.Table.FirstOrDefault(x => x.ServiceTagId == serviceTag && x.FranchiseeId == franchisee.Id);
                    var domain = new PriceEstimateServices();
                    if (priceEstimateService != null && priceEstimateService.ServicesTag != null && priceEstimateService.ServicesTag.Category.Name == "TIME")
                    {
                        if(model.BulkCorporatePrice >= 0)
                        {
                            model.BulkCorporatePrice = null;
                            model.BulkCorporateAdditionalPrice = null;
                            model.FranchiseePrice = null;
                        }
                    }
                    domain.FranchiseeId = franchisee.Id;
                    if (model.BulkCorporateAdditionalPrice <= 0)
                    {
                        model.BulkCorporateAdditionalPrice = null;
                    }
                    if (model.BulkCorporatePrice <= 0)
                    {
                        model.BulkCorporatePrice = null;
                    }
                    domain.BulkCorporatePrice = model.BulkCorporatePrice;
                    domain.BulkCorporateAdditionalPrice = model.BulkCorporateAdditionalPrice;
                    if (priceEstimateService != null)
                    {
                        domain.AlternativeSolution = priceEstimateService.AlternativeSolution;
                        domain.BulkCorporatePrice = model.BulkCorporatePrice;
                        domain.BulkCorporateAdditionalPrice = model.BulkCorporateAdditionalPrice;
                        domain.AlternativeSolution = priceEstimateService.AlternativeSolution;
                        if (priceEstimateService.IsPriceChangedByFranchisee)
                        {
                            domain.IsPriceChangedByFranchisee = priceEstimateService.IsPriceChangedByFranchisee;
                        }
                        if (priceEstimateService.IsPriceChangedByAdmin)
                        {
                            domain.IsPriceChangedByAdmin = priceEstimateService.IsPriceChangedByAdmin;
                        }
                        if (!priceEstimateService.IsPriceChangedByAdmin)
                        {
                            domain.CorporatePrice = priceEstimateService.BulkCorporatePrice;
                            domain.CorporateAdditionalPrice = priceEstimateService.BulkCorporateAdditionalPrice;
                        }
                        else
                        {
                            domain.CorporatePrice = priceEstimateService.CorporatePrice;
                            domain.CorporateAdditionalPrice = priceEstimateService.CorporateAdditionalPrice;
                        }
                        if (!priceEstimateService.IsPriceChangedByFranchisee)
                        {
                            domain.FranchiseePrice = domain.CorporatePrice;
                            domain.FranchiseeAdditionalPrice = domain.CorporateAdditionalPrice;
                        }
                        else
                        {
                            domain.FranchiseePrice = priceEstimateService.FranchiseePrice;
                            domain.FranchiseeAdditionalPrice = priceEstimateService.FranchiseeAdditionalPrice;
                        }
                    }
                    else
                    {
                        domain.CorporatePrice = model.BulkCorporatePrice;
                        domain.CorporateAdditionalPrice = model.BulkCorporateAdditionalPrice;
                        domain.FranchiseePrice = model.BulkCorporatePrice;
                        domain.FranchiseeAdditionalPrice = model.BulkCorporateAdditionalPrice;
                    }
                    domain.ServiceTagId = serviceTag;
                    domain.IsNew = priceEstimateService != null ? false : true;
                    domain.Id = priceEstimateService != null ? priceEstimateService.Id : 0;
                    _priceEstimateServicesRepository.Save(domain);
                }
            }
            return true;
        }
        public bool SavePriceEstimateFranchiseeWise(PriceEstimateSaveModel model, long roleUserId)
        {
            var isPriceChangedByFranchisee = false;
            var isFranchiseePriceExceed = false;
            var isFranchiseePriceExceedForEmail = false;
            var isPriceChangedByAdmin = false;
            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                isPriceChangedByFranchisee = false;
                isPriceChangedByAdmin = true;
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                isPriceChangedByFranchisee = true;
                isPriceChangedByAdmin = false;
            }
            foreach (var serviceTag in model.ServiceTagId)
            {
                foreach (var price in model.PriceEstimateServices)
                {
                    isFranchiseePriceExceed = false;
                    isFranchiseePriceExceedForEmail = false;
                    if (price.FranchiseeAdditionalPrice <= 0)
                    {
                        price.FranchiseeAdditionalPrice = null;
                    }
                    if (price.FranchiseePrice <= 0)
                    {
                        price.FranchiseePrice = null;
                    }
                    if (price.CorporatePrice <= 0)
                    {
                        price.CorporatePrice = null;
                    }
                    if (price.CorporateAdditionalPrice <= 0)
                    {
                        price.CorporateAdditionalPrice = null;
                    }
                    var priceEstimateService = _priceEstimateServicesRepository.Table.FirstOrDefault(x => x.ServiceTagId == serviceTag && x.FranchiseeId == price.FranchiseeId);
                    if (price.FranchiseePrice > price.CorporatePrice)
                    {
                        var PerValue = (price.CorporatePrice * 150) / 100;
                        if (price.FranchiseePrice >= PerValue)
                        {
                            isFranchiseePriceExceed = true;
                            isFranchiseePriceExceedForEmail = true;
                        }
                        else
                        {
                            isFranchiseePriceExceed = false;
                            isFranchiseePriceExceedForEmail = false;
                        }
                    }
                    var domain = new PriceEstimateServices();
                    domain.FranchiseeId = price.FranchiseeId;
                    if (priceEstimateService != null)
                    {
                        domain.BulkCorporateAdditionalPrice = priceEstimateService.BulkCorporateAdditionalPrice;
                        domain.BulkCorporatePrice = priceEstimateService.BulkCorporatePrice;
                        if (priceEstimateService.IsPriceChangedByFranchisee)
                        {
                            domain.IsPriceChangedByFranchisee = priceEstimateService.IsPriceChangedByFranchisee;
                        }
                        else
                        {
                            domain.IsPriceChangedByFranchisee = isPriceChangedByFranchisee;
                        }
                        if (priceEstimateService.IsPriceChangedByAdmin)
                        {
                            domain.IsPriceChangedByAdmin = priceEstimateService.IsPriceChangedByAdmin;
                        }
                        else
                        {
                            domain.IsPriceChangedByAdmin = isPriceChangedByAdmin;
                        }
                        if (!isPriceChangedByFranchisee && !priceEstimateService.IsPriceChangedByFranchisee)
                        {
                            domain.FranchiseePrice = price.CorporatePrice;
                            domain.FranchiseeAdditionalPrice = price.CorporateAdditionalPrice;
                        }
                        else if (isPriceChangedByFranchisee)
                        {
                            domain.FranchiseePrice = price.FranchiseePrice;
                            domain.FranchiseeAdditionalPrice = price.FranchiseeAdditionalPrice;
                            domain.IsFranchiseePriceExceed = isFranchiseePriceExceed;
                            domain.IsFranchiseePriceExceedForEmail = isFranchiseePriceExceedForEmail;
                        }
                        else
                        {
                            domain.FranchiseePrice = priceEstimateService.FranchiseePrice;
                            domain.FranchiseeAdditionalPrice = priceEstimateService.FranchiseeAdditionalPrice;
                        }
                        domain.CorporatePrice = price.CorporatePrice;
                        domain.CorporateAdditionalPrice = price.CorporateAdditionalPrice;
                    }
                    else
                    {
                        if (roleUserId == (long)RoleType.SuperAdmin)
                        {
                            domain.CorporatePrice = price.CorporatePrice;
                            domain.CorporateAdditionalPrice = price.CorporateAdditionalPrice;
                            domain.FranchiseePrice = price.CorporatePrice;
                            domain.FranchiseeAdditionalPrice = price.CorporateAdditionalPrice;
                        }
                        else
                        {
                            domain.FranchiseePrice = price.FranchiseePrice;
                            domain.FranchiseeAdditionalPrice = price.FranchiseeAdditionalPrice;
                        }
                        domain.IsPriceChangedByFranchisee = isPriceChangedByFranchisee;
                        domain.IsPriceChangedByAdmin = isPriceChangedByAdmin;
                    }

                    domain.AlternativeSolution = price.AlternativeSolution;
                    domain.ServiceTagId = serviceTag;
                    domain.IsNew = priceEstimateService != null ? false : true;
                    domain.Id = priceEstimateService != null ? priceEstimateService.Id : 0;
                    _priceEstimateServicesRepository.Save(domain);
                }
            }
            return true;
        }
        public bool SavePriceEstimateBulkUpdate(PriceEstimateBulkUpdateModel model, long roleUserId)
        {
            var isPriceChangedByFranchisee = false;
            var isPriceChangedByAdmin = false;
            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                isPriceChangedByFranchisee = false;
                isPriceChangedByAdmin = true;
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                isPriceChangedByFranchisee = true;
                isPriceChangedByAdmin = false;
            }
            if (model.FranchiseeAdditionalPrice <= 0)
            {
                model.FranchiseeAdditionalPrice = null;
            }
            if (model.FranchiseePrice <= 0)
            {
                model.FranchiseePrice = null;
            }
            if (model.CorporatePrice <= 0)
            {
                model.CorporatePrice = null;
            }
            if (model.CorporateAdditionalPrice <= 0)
            {
                model.CorporateAdditionalPrice = null;
            }
            if (model.FranchiseePrice != null || model.FranchiseeAdditionalPrice != null || model.CorporatePrice != null || model.CorporateAdditionalPrice != null)
            {
                foreach (var serviceTagId in model.ServiceTagId)
                {
                    foreach (var franchiseeId in model.FranchiseeId)
                    {
                        var priceEstimateService = _priceEstimateServicesRepository.Table.FirstOrDefault(x => x.ServiceTagId == serviceTagId && x.FranchiseeId == franchiseeId);
                        var domain = new PriceEstimateServices();
                        if (priceEstimateService != null)
                        {
                            domain.BulkCorporateAdditionalPrice = priceEstimateService.BulkCorporateAdditionalPrice;
                            domain.BulkCorporatePrice = priceEstimateService.BulkCorporatePrice;

                            if (priceEstimateService.IsPriceChangedByFranchisee)
                            {
                                domain.IsPriceChangedByFranchisee = priceEstimateService.IsPriceChangedByFranchisee;
                            }
                            else
                            {
                                domain.IsPriceChangedByFranchisee = isPriceChangedByFranchisee;
                            }
                            if (priceEstimateService.IsPriceChangedByAdmin)
                            {
                                domain.IsPriceChangedByAdmin = priceEstimateService.IsPriceChangedByAdmin;
                            }
                            else
                            {
                                domain.IsPriceChangedByAdmin = isPriceChangedByAdmin;
                            }
                            if (isPriceChangedByAdmin)
                            {
                                domain.CorporatePrice = model.CorporatePrice;
                                domain.CorporateAdditionalPrice = model.CorporateAdditionalPrice;
                            }
                            else
                            {
                                domain.CorporatePrice = priceEstimateService.CorporatePrice;
                                domain.CorporateAdditionalPrice = priceEstimateService.CorporateAdditionalPrice;
                            }
                            if (!isPriceChangedByFranchisee && !priceEstimateService.IsPriceChangedByFranchisee)
                            {
                                domain.FranchiseePrice = domain.CorporatePrice;
                                domain.FranchiseeAdditionalPrice = domain.CorporateAdditionalPrice;
                            }
                            else if (isPriceChangedByFranchisee)
                            {
                                domain.FranchiseePrice = model.FranchiseePrice;
                                domain.FranchiseeAdditionalPrice = model.FranchiseePrice;
                            }
                            else
                            {
                                domain.FranchiseePrice = priceEstimateService.FranchiseePrice;
                                domain.FranchiseeAdditionalPrice = priceEstimateService.FranchiseeAdditionalPrice;
                            }
                        }
                        else
                        {
                            if (roleUserId == (long)RoleType.SuperAdmin)
                            {
                                domain.CorporatePrice = model.CorporatePrice;
                                domain.CorporateAdditionalPrice = model.CorporateAdditionalPrice;
                                domain.FranchiseePrice = model.CorporatePrice;
                                domain.FranchiseeAdditionalPrice = model.CorporateAdditionalPrice;
                            }
                            else
                            {
                                domain.FranchiseePrice = model.FranchiseePrice;
                                domain.FranchiseeAdditionalPrice = model.FranchiseeAdditionalPrice;
                            }
                            domain.IsPriceChangedByFranchisee = isPriceChangedByFranchisee;
                            domain.IsPriceChangedByAdmin = isPriceChangedByAdmin;
                        }
                        domain.ServiceTagId = serviceTagId;
                        domain.FranchiseeId = franchiseeId;
                        domain.AlternativeSolution = model.AlternativeSolution;
                        domain.IsNew = priceEstimateService != null ? false : true;
                        domain.Id = priceEstimateService != null ? priceEstimateService.Id : 0;
                        _priceEstimateServicesRepository.Save(domain);
                    }

                }
            }
            return true;
        }

        public PriceEstimatePageViewModel GetPriceEstimateCollectionPerFranchisee(PriceEstimateGetModel getmodel, long userId, long roleUserId)
        {
            PriceEstimatePageViewModel model = new PriceEstimatePageViewModel();
            List<PriceEstimateViewModel> ListModel = new List<PriceEstimateViewModel>();
            var servicesTag = _servicesTagRepository.Table.Where(x => x.Service != null && x.IsActive).ToList();
            foreach (var service in servicesTag)
            {
                PriceEstimateViewModel pricemodel = new PriceEstimateViewModel();
                pricemodel.ServiceTagId = service.Id;
                pricemodel.Note = service.Notes;
                pricemodel.Service = service.Service;
                pricemodel.ServiceType = service.ServiceType.Name;
                pricemodel.MaterialType = service.MaterialType;
                pricemodel.Category = service.Category.Name;
                pricemodel.CategoryId = service.Category.Id;

                var priceEstimates = _priceEstimateServicesRepository.Table.OrderByDescending(x => x.Id).Where(x => x.ServiceTagId == service.Id).ToList();
                if (priceEstimates.Count > 0)
                {
                    var priceEstimatesForFranchisee = priceEstimates.Where(x => x.FranchiseeId == getmodel.FranchiseeId).ToList();
                    if (priceEstimatesForFranchisee.Count() > 0)
                    {
                        pricemodel.BulkCorporatePrice = priceEstimatesForFranchisee.FirstOrDefault().BulkCorporatePrice;
                        pricemodel.BulkCorporateAdditionalPrice = priceEstimatesForFranchisee.FirstOrDefault().BulkCorporateAdditionalPrice;
                        pricemodel.FranchiseeCorporatePrice = priceEstimatesForFranchisee.FirstOrDefault().FranchiseePrice;
                        pricemodel.FranchiseeAdditionalCorporatePrice = priceEstimatesForFranchisee.FirstOrDefault().FranchiseeAdditionalPrice;
                        //pricemodel.FranchiseePrice = priceEstimatesForFranchisee[0].FranchiseePrice;
                    }
                    else
                    {
                        pricemodel.FranchiseePrice = 0;
                        pricemodel.FranchiseeCorporatePrice = 0;
                        pricemodel.FranchiseeAdditionalCorporatePrice = 0;
                    }
                }
                else
                {
                    pricemodel.FranchiseePrice = 0;
                    pricemodel.FranchiseeCorporatePrice = 0;
                    pricemodel.FranchiseeAdditionalCorporatePrice = 0;
                }

                ListModel.Add(pricemodel);
            }
            return new PriceEstimatePageViewModel
            {
                PriceEstimateViewModelList = ListModel,
            };
        }

        public ShiftChargesViewModel GetShiftCharges(long userId, long roleUserId)
        {
            ShiftChargesViewModel model = new ShiftChargesViewModel();
            var shiftCharge = new ShiftCharges();
            var frachiseeList = new List<Franchisee>();
            var assignedFranchiseeIdList = new long();
            var shiftCharges = _shiftChargesRepository.Table.Where(x => x.IsActive == true && x.IsPriceChangedByFranchisee == true && x.FranchiseeId != null).ToList();
            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                shiftCharge = _shiftChargesRepository.Table.FirstOrDefault(x => x.FranchiseeId == null && x.IsActive == true);
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).FirstOrDefault();
                shiftCharge = _shiftChargesRepository.Table.FirstOrDefault(x => x.FranchiseeId == assignedFranchiseeIdList && x.IsActive == true);
                var corporateShiftCharges = _shiftChargesRepository.Table.FirstOrDefault(x => x.FranchiseeId == null && x.IsActive == true);
                model.TechDayShiftPriceCorporate = corporateShiftCharges.TechDayShiftPrice;
                model.MaintenanceTechNightShiftPriceCorporate = corporateShiftCharges.MaintenanceTechNightShiftPrice;
                model.CommercialRestorationShiftPriceCorporate = corporateShiftCharges.CommercialRestorationShiftPrice;
            }
            model.TechDayShiftPrice = shiftCharge.TechDayShiftPrice;
            model.CommercialRestorationShiftPrice = shiftCharge.CommercialRestorationShiftPrice;
            model.MaintenanceTechNightShiftPrice = shiftCharge.MaintenanceTechNightShiftPrice;
            model.IsPriceChangedByFranchisee = shiftCharge.IsPriceChangedByFranchisee;
            model.IsActive = shiftCharge.IsActive;
            if (shiftCharges.Count > 0)
            {
                model.AverageTechDayShiftPrice = (decimal)(Math.Round((double)shiftCharges.Sum(x => x.TechDayShiftPrice) / (shiftCharges.Count), 2));
                model.AverageCommercialRestorationShiftPrice = (decimal)(Math.Round((double)shiftCharges.Sum(x => x.CommercialRestorationShiftPrice) / (shiftCharges.Count), 2));
                model.AverageMaintenanceTechNightShiftPrice = (decimal)(Math.Round((double)shiftCharges.Sum(x => x.MaintenanceTechNightShiftPrice) / (shiftCharges.Count), 2));
                model.MaximumTechDayShiftPrice = shiftCharges.Max(x => x.TechDayShiftPrice);
                var maximumTechDayShiftPriceNames = shiftCharges.Where(x => x.TechDayShiftPrice == model.MaximumTechDayShiftPrice).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                model.FranchiseeNameForMaximumTechDayShiftPrice = String.Join(", ", maximumTechDayShiftPriceNames);

                model.MaximumCommercialRestorationShiftPrice = shiftCharges.Max(x => x.CommercialRestorationShiftPrice);
                var maximumCommercialRestorationShiftPriceNames = shiftCharges.Where(x => x.CommercialRestorationShiftPrice == model.MaximumCommercialRestorationShiftPrice).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                model.FranchiseeNameForMaximumCommercialRestorationShiftPrice = String.Join(", ", maximumCommercialRestorationShiftPriceNames);


                model.MaximumMaintenanceTechNightShiftPrice = shiftCharges.Max(x => x.MaintenanceTechNightShiftPrice);
                var maximumMaintenanceTechNightShiftPriceNames = shiftCharges.Where(x => x.MaintenanceTechNightShiftPrice == model.MaximumMaintenanceTechNightShiftPrice).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                model.FranchiseeNameForMaximumMaintenanceTechNightShiftPrice = String.Join(", ", maximumMaintenanceTechNightShiftPriceNames);

            }
            return model;
        }

        public bool SaveShiftCharges(ShiftChargesSaveModel model, long userId, long roleUserId)
        {
            var shiftChargeList = new List<ShiftCharges>();
            var frachiseeList = new List<Franchisee>();
            var assignedFranchiseeIdList = new List<long>();
            var shiftchargeIdNotUpdatedByFranchisee = new List<long?>();
            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                shiftChargeList = _shiftChargesRepository.Table.Where(x => x.FranchiseeId == null || x.IsPriceChangedByFranchisee == false && x.IsActive == true).ToList();

                shiftchargeIdNotUpdatedByFranchisee = shiftChargeList.Select(x => x.FranchiseeId).ToList();

                foreach (var charge in shiftChargeList)
                {
                    charge.IsActive = false;
                    charge.IsNew = false;
                    charge.DataRecorderMetaData = new DataRecorderMetaData();
                    _shiftChargesRepository.Save(charge);
                }

                foreach (var franchiseeId in shiftchargeIdNotUpdatedByFranchisee)
                {
                    var shiftCharge = new ShiftCharges();
                    shiftCharge.TechDayShiftPrice = model.TechDayShiftPrice;
                    shiftCharge.CommercialRestorationShiftPrice = model.CommercialRestorationShiftPrice;
                    shiftCharge.MaintenanceTechNightShiftPrice = model.MaintenanceTechNightShiftPrice;
                    shiftCharge.FranchiseeId = franchiseeId;
                    shiftCharge.IsActive = true;
                    shiftCharge.IsNew = true;
                    shiftCharge.DataRecorderMetaData = new DataRecorderMetaData();
                    _shiftChargesRepository.Save(shiftCharge);
                }
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).ToList();
                shiftChargeList = _shiftChargesRepository.Table.Where(x => assignedFranchiseeIdList.Contains((long)x.FranchiseeId)).ToList();
                shiftchargeIdNotUpdatedByFranchisee = shiftChargeList.Select(x => x.FranchiseeId).ToList();
                foreach (var shiftCharge in shiftChargeList)
                {
                    shiftCharge.IsActive = false;
                    shiftCharge.IsNew = false;
                    shiftCharge.DataRecorderMetaData = new DataRecorderMetaData();
                    _shiftChargesRepository.Save(shiftCharge);
                }
                foreach (var franchiseeId in shiftchargeIdNotUpdatedByFranchisee)
                {
                    var shiftCharge = new ShiftCharges();
                    shiftCharge.TechDayShiftPrice = model.TechDayShiftPrice;
                    shiftCharge.CommercialRestorationShiftPrice = model.CommercialRestorationShiftPrice;
                    shiftCharge.MaintenanceTechNightShiftPrice = model.MaintenanceTechNightShiftPrice;
                    shiftCharge.IsPriceChangedByFranchisee = true;
                    shiftCharge.FranchiseeId = franchiseeId;
                    shiftCharge.IsActive = true;
                    shiftCharge.IsNew = true;
                    shiftCharge.DataRecorderMetaData = new DataRecorderMetaData();
                    _shiftChargesRepository.Save(shiftCharge);
                }
            }
            return true;
        }

        public ReplacementChargesViewModel GetReplacementCharges(long userId, long roleUserId)
        {
            ReplacementChargesViewModel model = new ReplacementChargesViewModel();
            model.ReplacementChargesList = new List<ReplacementChargesModel>();
            var replacementCharge = new List<ReplacementCharges>();
            var frachiseeList = new List<Franchisee>();
            var assignedFranchiseeIdList = new long();
            var replacementChargesList = _replacementChargesRepository.Table.Where(x => x.FranchiseeId != null && x.IsActive == true && x.IsPriceChangedByFranchisee == true).ToList();

            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                replacementCharge = _replacementChargesRepository.Table.Where(x => x.FranchiseeId == null && x.IsActive == true).ToList();
                foreach (var charge in replacementCharge)
                {
                    ReplacementChargesModel replacementCharges = new ReplacementChargesModel();
                    replacementCharges.CostOfInstallingTile = charge.CostOfInstallingTile;
                    replacementCharges.CostOfRemovingTile = charge.CostOfRemovingTile;
                    replacementCharges.CostOfTileMaterial = charge.CostOfTileMaterial;
                    replacementCharges.TotalReplacementCost = charge.TotalReplacementCost;
                    replacementCharges.Material = charge.Material;
                    replacementCharges.IsActive = charge.IsActive;
                    replacementCharges.Order = charge.Order;
                    var chargesForParticularMaterial = replacementChargesList.Where(x => x.Material == charge.Material).ToList();
                    if (chargesForParticularMaterial.Count > 0)
                    {
                        replacementCharges.AverageCostOfInstallingTile = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.CostOfInstallingTile) / (chargesForParticularMaterial.Count), 2));
                        replacementCharges.AverageCostOfRemovingTile = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.CostOfRemovingTile) / (chargesForParticularMaterial.Count), 2));
                        replacementCharges.AverageCostOfTileMaterial = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.CostOfTileMaterial) / (chargesForParticularMaterial.Count), 2));
                        replacementCharges.AverageTotalReplacementCost = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.TotalReplacementCost) / (chargesForParticularMaterial.Count), 2));

                        replacementCharges.MaximumCostOfInstallingTile = chargesForParticularMaterial.Max(x => x.CostOfInstallingTile);
                        var maximumCostOfInstallingTileNames = chargesForParticularMaterial.Where(x => x.CostOfInstallingTile == replacementCharges.MaximumCostOfInstallingTile).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        replacementCharges.FranchiseeNameForMaximumCostOfInstallingTile = String.Join(", ", maximumCostOfInstallingTileNames);

                        replacementCharges.MaximumCostOfRemovingTile = chargesForParticularMaterial.Max(x => x.CostOfRemovingTile);
                        var maximumCostOfRemovingTile = chargesForParticularMaterial.Where(x => x.CostOfRemovingTile == replacementCharges.MaximumCostOfRemovingTile).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        replacementCharges.FranchiseeNameForMaximumCostOfRemovingTile = String.Join(", ", maximumCostOfRemovingTile);

                        replacementCharges.MaximumCostOfTileMaterial = chargesForParticularMaterial.Max(x => x.CostOfTileMaterial);
                        var maximumCostOfTileMaterial = chargesForParticularMaterial.Where(x => x.CostOfTileMaterial == replacementCharges.MaximumCostOfTileMaterial).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        replacementCharges.FranchiseeNameForMaximumCostOfTileMaterial = String.Join(", ", maximumCostOfTileMaterial);

                        replacementCharges.MaximumTotalReplacementCost = chargesForParticularMaterial.Max(x => x.TotalReplacementCost);
                        var maximumTotalReplacementCost = chargesForParticularMaterial.Where(x => x.TotalReplacementCost == replacementCharges.MaximumTotalReplacementCost).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        replacementCharges.FranchiseeNameForMaximumTotalReplacementCost = String.Join(", ", maximumTotalReplacementCost);
                    }
                    model.ReplacementChargesList.Add(replacementCharges);
                }
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).FirstOrDefault();
                replacementCharge = _replacementChargesRepository.Table.Where(x => x.FranchiseeId == assignedFranchiseeIdList && x.IsActive == true).ToList();
                var corporatePricingForReplacementCharges = _replacementChargesRepository.Table.Where(x => x.FranchiseeId == null && x.IsActive == true).ToList();
                foreach (var charge in replacementCharge)
                {
                    ReplacementChargesModel replacementCharges = new ReplacementChargesModel();
                    replacementCharges.CostOfInstallingTile = charge.CostOfInstallingTile;
                    replacementCharges.CostOfRemovingTile = charge.CostOfRemovingTile;
                    replacementCharges.CostOfTileMaterial = charge.CostOfTileMaterial;
                    replacementCharges.TotalReplacementCost = charge.TotalReplacementCost;
                    replacementCharges.Material = charge.Material;
                    replacementCharges.IsActive = charge.IsActive;
                    replacementCharges.Order = charge.Order;
                    var chargesForParticularMaterial = replacementChargesList.Where(x => x.Material == charge.Material).ToList();
                    var corporateChargesForParticularMaterial = corporatePricingForReplacementCharges.FirstOrDefault(x => x.Material == charge.Material);
                    replacementCharges.CorporateCostOfRemovingTile = corporateChargesForParticularMaterial.CostOfRemovingTile;
                    replacementCharges.CorporateCostOfInstallingTile = corporateChargesForParticularMaterial.CostOfInstallingTile;
                    replacementCharges.CorporateCostOfTileMaterial = corporateChargesForParticularMaterial.CostOfTileMaterial;
                    replacementCharges.CorporateTotalReplacementCost = corporateChargesForParticularMaterial.TotalReplacementCost;
                    if (chargesForParticularMaterial.Count > 0)
                    {
                        replacementCharges.AverageCostOfInstallingTile = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.CostOfInstallingTile) / (chargesForParticularMaterial.Count), 2));
                        replacementCharges.AverageCostOfRemovingTile = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.CostOfRemovingTile) / (chargesForParticularMaterial.Count), 2));
                        replacementCharges.AverageCostOfTileMaterial = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.CostOfTileMaterial) / (chargesForParticularMaterial.Count), 2));
                        replacementCharges.AverageTotalReplacementCost = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.TotalReplacementCost) / (chargesForParticularMaterial.Count), 2));

                        replacementCharges.MaximumCostOfInstallingTile = chargesForParticularMaterial.Max(x => x.CostOfInstallingTile);
                        var maximumCostOfInstallingTileNames = chargesForParticularMaterial.Where(x => x.CostOfInstallingTile == replacementCharges.MaximumCostOfInstallingTile).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        replacementCharges.FranchiseeNameForMaximumCostOfInstallingTile = String.Join(", ", maximumCostOfInstallingTileNames);

                        replacementCharges.MaximumCostOfRemovingTile = chargesForParticularMaterial.Max(x => x.CostOfRemovingTile);
                        var maximumCostOfRemovingTile = chargesForParticularMaterial.Where(x => x.CostOfRemovingTile == replacementCharges.MaximumCostOfRemovingTile).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        replacementCharges.FranchiseeNameForMaximumCostOfRemovingTile = String.Join(", ", maximumCostOfRemovingTile);

                        replacementCharges.MaximumCostOfTileMaterial = chargesForParticularMaterial.Max(x => x.CostOfTileMaterial);
                        var maximumCostOfTileMaterial = chargesForParticularMaterial.Where(x => x.CostOfTileMaterial == replacementCharges.MaximumCostOfTileMaterial).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        replacementCharges.FranchiseeNameForMaximumCostOfTileMaterial = String.Join(", ", maximumCostOfTileMaterial);

                        replacementCharges.MaximumTotalReplacementCost = chargesForParticularMaterial.Max(x => x.TotalReplacementCost);
                        var maximumTotalReplacementCost = chargesForParticularMaterial.Where(x => x.TotalReplacementCost == replacementCharges.MaximumTotalReplacementCost).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        replacementCharges.FranchiseeNameForMaximumTotalReplacementCost = String.Join(", ", maximumTotalReplacementCost);
                    }
                    model.ReplacementChargesList.Add(replacementCharges);
                }
            }
            model.ReplacementChargesList = model.ReplacementChargesList.OrderBy(x => x.Order).ToList();
            return model;
        }

        public bool SaveReplacementCharges(ReplacementChargesSaveModel model, long userId, long roleUserId)
        {
            var replacementChargeList = new List<ReplacementCharges>();
            var frachiseeList = new List<Franchisee>();
            var assignedFranchiseeIdList = new List<long>();
            var replacementchargeIdNotUpdatedByFranchisee = new List<long?>();
            var materialList = model.ReplacementChargesList.Select(x => x.Material).ToList();
            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                replacementChargeList = _replacementChargesRepository.Table.Where(x => (x.FranchiseeId == null || x.IsPriceChangedByFranchisee == false) && materialList.Contains(x.Material) && x.IsActive == true).ToList();
                replacementchargeIdNotUpdatedByFranchisee = replacementChargeList.Select(x => x.FranchiseeId).Distinct().ToList();

                foreach (var charge in replacementChargeList)
                {
                    charge.IsActive = false;
                    charge.IsNew = false;
                    charge.DataRecorderMetaData = new DataRecorderMetaData();
                    _replacementChargesRepository.Save(charge);
                }

                foreach (var franchiseeId in replacementchargeIdNotUpdatedByFranchisee)
                {
                    foreach (var charge in model.ReplacementChargesList)
                    {
                        var replacementCharge = new ReplacementCharges();
                        replacementCharge.Material = charge.Material;
                        replacementCharge.CostOfInstallingTile = charge.CostOfInstallingTile;
                        replacementCharge.CostOfRemovingTile = charge.CostOfRemovingTile;
                        replacementCharge.CostOfTileMaterial = charge.CostOfTileMaterial;
                        replacementCharge.TotalReplacementCost = charge.TotalReplacementCost;
                        replacementCharge.FranchiseeId = franchiseeId;
                        replacementCharge.Order = GetOrder(charge.Material);
                        replacementCharge.IsActive = true;
                        replacementCharge.IsNew = true;
                        replacementCharge.DataRecorderMetaData = new DataRecorderMetaData();
                        _replacementChargesRepository.Save(replacementCharge);
                    }
                }
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).ToList();
                replacementChargeList = _replacementChargesRepository.Table.Where(x => assignedFranchiseeIdList.Contains((long)x.FranchiseeId) && materialList.Contains(x.Material)).ToList();
                replacementchargeIdNotUpdatedByFranchisee = replacementChargeList.Select(x => x.FranchiseeId).Distinct().ToList();
                foreach (var replacementCharge in replacementChargeList)
                {
                    replacementCharge.IsActive = false;
                    replacementCharge.IsNew = false;
                    replacementCharge.DataRecorderMetaData = new DataRecorderMetaData();
                    _replacementChargesRepository.Save(replacementCharge);
                }
                foreach (var franchiseeId in replacementchargeIdNotUpdatedByFranchisee)
                {
                    foreach (var charge in model.ReplacementChargesList)
                    {
                        var replacementCharge = new ReplacementCharges();
                        replacementCharge.Material = charge.Material;
                        replacementCharge.CostOfInstallingTile = charge.CostOfInstallingTile;
                        replacementCharge.CostOfRemovingTile = charge.CostOfRemovingTile;
                        replacementCharge.CostOfTileMaterial = charge.CostOfTileMaterial;
                        replacementCharge.TotalReplacementCost = charge.TotalReplacementCost;
                        replacementCharge.FranchiseeId = franchiseeId;
                        replacementCharge.IsPriceChangedByFranchisee = true;
                        replacementCharge.Order = GetOrder(charge.Material);
                        replacementCharge.IsActive = true;
                        replacementCharge.IsNew = true;
                        replacementCharge.DataRecorderMetaData = new DataRecorderMetaData();
                        _replacementChargesRepository.Save(replacementCharge);
                    }
                }
            }
            return true;
        }

        public MaintenanceChargesViewModel GetMaintenanceCharges(long userId, long roleUserId)
        {
            MaintenanceChargesViewModel model = new MaintenanceChargesViewModel();
            model.MaintenanceChargesList = new List<MaintenanceChargesModel>();
            var maintenanceCharge = new List<MaintenanceCharges>();
            var frachiseeList = new List<Franchisee>();
            var assignedFranchiseeIdList = new long();
            var maintenanceChargesList = _maintenanceChargesRepository.Table.Where(x => x.FranchiseeId != null && x.IsActive == true && x.IsPriceChangedByFranchisee == true).ToList();

            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                maintenanceCharge = _maintenanceChargesRepository.Table.Where(x => x.FranchiseeId == null && x.IsActive == true).ToList();
                foreach (var charge in maintenanceCharge)
                {
                    var chargesForParticularMaterial = maintenanceChargesList.Where(x => x.Material == charge.Material).ToList();
                    MaintenanceChargesModel maintenanceCharges = new MaintenanceChargesModel();
                    maintenanceCharges.High = charge.High;
                    maintenanceCharges.Low = charge.Low;
                    maintenanceCharges.UOM = charge.UOM;
                    maintenanceCharges.Material = charge.Material;
                    maintenanceCharges.IsActive = charge.IsActive;
                    maintenanceCharges.Order = charge.Order;
                    maintenanceCharges.Notes = charge.Notes;
                    if (chargesForParticularMaterial.Count > 0)
                    {
                        maintenanceCharges.AverageHigh = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.High) / (chargesForParticularMaterial.Count), 2));
                        maintenanceCharges.AverageLow = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.Low) / (chargesForParticularMaterial.Count), 2));

                        maintenanceCharges.MaximumHigh = chargesForParticularMaterial.Max(x => x.High);
                        maintenanceCharges.MaximumLow = chargesForParticularMaterial.Max(x => x.Low);
                        var maximumHighNames = chargesForParticularMaterial.Where(x => x.High == maintenanceCharges.MaximumHigh).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        maintenanceCharges.FranchiseeNameForMaximumHigh = String.Join(", ", maximumHighNames);
                        var maximumLowNames = chargesForParticularMaterial.Where(x => x.Low == maintenanceCharges.MaximumLow).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        maintenanceCharges.FranchiseeNameForMaximumLow = String.Join(", ", maximumLowNames);
                    }
                    model.MaintenanceChargesList.Add(maintenanceCharges);
                }
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).FirstOrDefault();
                maintenanceCharge = _maintenanceChargesRepository.Table.Where(x => x.FranchiseeId == assignedFranchiseeIdList && x.IsActive == true).ToList();

                var corporatePricingForMaintenanceCharges = _maintenanceChargesRepository.Table.Where(x => x.FranchiseeId == null && x.IsActive == true).ToList();
                foreach (var charge in maintenanceCharge)
                {
                    var chargesForParticularMaterial = maintenanceChargesList.Where(x => x.Material == charge.Material).ToList();
                    MaintenanceChargesModel maintenanceCharges = new MaintenanceChargesModel();
                    maintenanceCharges.High = charge.High;
                    maintenanceCharges.Low = charge.Low;
                    maintenanceCharges.UOM = charge.UOM;
                    maintenanceCharges.Material = charge.Material;
                    maintenanceCharges.IsActive = charge.IsActive;
                    maintenanceCharges.Order = charge.Order;
                    maintenanceCharges.Notes = charge.Notes;
                    var corporateChargesForParticularMaterial = corporatePricingForMaintenanceCharges.FirstOrDefault(x => x.Material == charge.Material);
                    maintenanceCharges.CorporateHigh = corporateChargesForParticularMaterial.High;
                    maintenanceCharges.CorporateLow = corporateChargesForParticularMaterial.Low;

                    if (chargesForParticularMaterial.Count > 0)
                    {
                        maintenanceCharges.AverageHigh = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.High) / (chargesForParticularMaterial.Count), 2));
                        maintenanceCharges.AverageLow = (decimal)(Math.Round((double)chargesForParticularMaterial.Sum(x => x.Low) / (chargesForParticularMaterial.Count), 2));
                        maintenanceCharges.MaximumHigh = chargesForParticularMaterial.Max(x => x.High);
                        maintenanceCharges.MaximumLow = chargesForParticularMaterial.Max(x => x.Low);
                        var maximumHighNames = chargesForParticularMaterial.Where(x => x.High == maintenanceCharges.MaximumHigh).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        maintenanceCharges.FranchiseeNameForMaximumHigh = String.Join(", ", maximumHighNames);
                        var maximumLowNames = chargesForParticularMaterial.Where(x => x.Low == maintenanceCharges.MaximumLow).Select(x => x.Franchisee.Organization.Name).Distinct().ToList();
                        maintenanceCharges.FranchiseeNameForMaximumLow = String.Join(", ", maximumLowNames);
                    }
                    model.MaintenanceChargesList.Add(maintenanceCharges);
                }
            }
            model.MaintenanceChargesList = model.MaintenanceChargesList.OrderBy(x => x.Order).ToList();
            return model;
        }

        public bool SaveMaintenanceCharges(MaintenanceChargesSaveModel model, long userId, long roleUserId)
        {
            var maintenanceChargeList = new List<MaintenanceCharges>();
            var frachiseeList = new List<Franchisee>();
            var assignedFranchiseeIdList = new List<long>();
            var maintenancechargeIdNotUpdatedByFranchisee = new List<long?>();
            var materialList = model.MaintenanceChargesList.Select(x => x.Material).ToList();
            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                maintenanceChargeList = _maintenanceChargesRepository.Table.Where(x => (x.FranchiseeId == null || x.IsPriceChangedByFranchisee == false) && materialList.Contains(x.Material) && x.IsActive == true).ToList();
                maintenancechargeIdNotUpdatedByFranchisee = maintenanceChargeList.Select(x => x.FranchiseeId).Distinct().ToList();

                foreach (var charge in maintenanceChargeList)
                {
                    charge.IsActive = false;
                    charge.IsNew = false;
                    charge.DataRecorderMetaData = new DataRecorderMetaData();
                    _maintenanceChargesRepository.Save(charge);
                }

                foreach (var franchiseeId in maintenancechargeIdNotUpdatedByFranchisee)
                {
                    foreach (var charge in model.MaintenanceChargesList)
                    {
                        var maintenanceCharge = new MaintenanceCharges();
                        maintenanceCharge.Material = charge.Material;
                        maintenanceCharge.High = charge.High;
                        maintenanceCharge.Low = charge.Low;
                        maintenanceCharge.UOM = charge.UOM;
                        maintenanceCharge.Notes = charge.Notes;
                        maintenanceCharge.FranchiseeId = franchiseeId;
                        maintenanceCharge.Order = GetOrder(charge.Material);
                        maintenanceCharge.IsActive = true;
                        maintenanceCharge.IsNew = true;
                        maintenanceCharge.DataRecorderMetaData = new DataRecorderMetaData();
                        _maintenanceChargesRepository.Save(maintenanceCharge);
                    }
                }
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).ToList();
                maintenanceChargeList = _maintenanceChargesRepository.Table.Where(x => assignedFranchiseeIdList.Contains((long)x.FranchiseeId) && materialList.Contains(x.Material)).ToList();
                maintenancechargeIdNotUpdatedByFranchisee = maintenanceChargeList.Select(x => x.FranchiseeId).Distinct().ToList();
                foreach (var maintenanceCharge in maintenanceChargeList)
                {
                    maintenanceCharge.IsActive = false;
                    maintenanceCharge.IsNew = false;
                    maintenanceCharge.DataRecorderMetaData = new DataRecorderMetaData();
                    _maintenanceChargesRepository.Save(maintenanceCharge);
                }
                foreach (var franchiseeId in maintenancechargeIdNotUpdatedByFranchisee)
                {
                    foreach (var charge in model.MaintenanceChargesList)
                    {
                        var maintenanceCharge = new MaintenanceCharges();
                        maintenanceCharge.Material = charge.Material;
                        maintenanceCharge.High = charge.High;
                        maintenanceCharge.Low = charge.Low;
                        maintenanceCharge.UOM = charge.UOM;
                        maintenanceCharge.FranchiseeId = franchiseeId;
                        maintenanceCharge.Notes = charge.Notes;
                        maintenanceCharge.IsPriceChangedByFranchisee = true;
                        maintenanceCharge.Order = GetOrder(charge.Material);
                        maintenanceCharge.IsActive = true;
                        maintenanceCharge.IsNew = true;
                        maintenanceCharge.DataRecorderMetaData = new DataRecorderMetaData();
                        _maintenanceChargesRepository.Save(maintenanceCharge);
                    }
                }
            }
            return true;
        }

        public FloorGrindingAdjustmentViewModel GetFloorGrindingAdjustment(long userId, long roleUserId)
        {
            FloorGrindingAdjustmentViewModel model = new FloorGrindingAdjustmentViewModel();
            model.FloorGrindingAdjustmentList = new List<FloorGrindingAdjustmentModel>();
            var floorGrindingAdjustmentList = new List<FloorGrindingAdjustment>();
            var frachiseeList = new List<Franchisee>();
            var assignedFranchiseeIdList = new long();
            var note = new FloorGrindingAdjustmentNotes();
            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                floorGrindingAdjustmentList = _floorGrindingAdjustmentRepository.Table.Where(x => x.FranchiseeId == null && x.IsActive == true).ToList();
                foreach (var charge in floorGrindingAdjustmentList)
                {
                    FloorGrindingAdjustmentModel floorGrindingAdjustment = new FloorGrindingAdjustmentModel();
                    floorGrindingAdjustment.DiameterOfGrindingPlate = charge.DiameterOfGrindingPlate;
                    floorGrindingAdjustment.Area = charge.Area;
                    floorGrindingAdjustment.AdjustmentFactor = charge.AdjustmentFactor;
                    floorGrindingAdjustment.IsActive = charge.IsActive;
                    model.FloorGrindingAdjustmentList.Add(floorGrindingAdjustment);
                }
                note = _floorGrindingAdjustmentNotesRepository.Table.Where(x => x.FranchiseeId == null && x.IsActive == true).FirstOrDefault();
                if (note != null)
                {
                    model.Note = note.Note;
                }
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).FirstOrDefault();
                floorGrindingAdjustmentList = _floorGrindingAdjustmentRepository.Table.Where(x => x.FranchiseeId == assignedFranchiseeIdList && x.IsActive == true).ToList();
                foreach (var charge in floorGrindingAdjustmentList)
                {
                    FloorGrindingAdjustmentModel floorGrindingAdjustment = new FloorGrindingAdjustmentModel();
                    floorGrindingAdjustment.DiameterOfGrindingPlate = charge.DiameterOfGrindingPlate;
                    floorGrindingAdjustment.Area = charge.Area;
                    floorGrindingAdjustment.AdjustmentFactor = charge.AdjustmentFactor;
                    floorGrindingAdjustment.IsActive = charge.IsActive;
                    model.FloorGrindingAdjustmentList.Add(floorGrindingAdjustment);
                }
                note = _floorGrindingAdjustmentNotesRepository.Table.Where(x => x.FranchiseeId == assignedFranchiseeIdList && x.IsActive == true).FirstOrDefault();
                if (note != null)
                {
                    model.Note = note.Note;
                }
            }
            return model;
        }

        public bool SaveFloorGrindingAdjustmentNote(long userId, long roleUserId, FloorGrindingAdjustNoteSaveModel model)
        {
            var frachiseeList = new List<Franchisee>();
            var assignedFranchiseeIdList = new List<long>();
            var floorGrindingNotesIdNotUpdatedByFranchisee = new List<long?>();
            var floorGrindingAdjustmentNotes = new List<FloorGrindingAdjustmentNotes>();
            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                floorGrindingAdjustmentNotes = _floorGrindingAdjustmentNotesRepository.Table.Where(x => (x.FranchiseeId == null || x.IsChangedByFranchisee == false) && x.IsActive == true).ToList();
                floorGrindingNotesIdNotUpdatedByFranchisee = floorGrindingAdjustmentNotes.Select(x => x.FranchiseeId).Distinct().ToList();

                if (floorGrindingAdjustmentNotes.Count == 0)
                {
                    frachiseeList = _franchiseeRepository.Table.Where(x => !x.Organization.Name.StartsWith("0-") && x.Id != 2).ToList();
                    foreach (var franchisee in frachiseeList)
                    {
                        var floorGrindingAdjustmentNote = new FloorGrindingAdjustmentNotes();
                        floorGrindingAdjustmentNote.Note = model.Note;
                        floorGrindingAdjustmentNote.FranchiseeId = franchisee.Id;
                        floorGrindingAdjustmentNote.IsActive = true;
                        floorGrindingAdjustmentNote.IsNew = true;
                        floorGrindingAdjustmentNote.DataRecorderMetaData = new DataRecorderMetaData();
                        _floorGrindingAdjustmentNotesRepository.Save(floorGrindingAdjustmentNote);
                    }
                    var floorGrindingAdjustmentNoteForCorporate = new FloorGrindingAdjustmentNotes();
                    floorGrindingAdjustmentNoteForCorporate.Note = model.Note;
                    floorGrindingAdjustmentNoteForCorporate.FranchiseeId = null;
                    floorGrindingAdjustmentNoteForCorporate.IsActive = true;
                    floorGrindingAdjustmentNoteForCorporate.IsNew = true;
                    floorGrindingAdjustmentNoteForCorporate.DataRecorderMetaData = new DataRecorderMetaData();
                    _floorGrindingAdjustmentNotesRepository.Save(floorGrindingAdjustmentNoteForCorporate);
                }
                else
                {
                    foreach (var note in floorGrindingAdjustmentNotes)
                    {
                        note.IsActive = false;
                        note.IsNew = false;
                        note.DataRecorderMetaData = new DataRecorderMetaData();
                        _floorGrindingAdjustmentNotesRepository.Save(note);
                    }

                    foreach (var franchiseeId in floorGrindingNotesIdNotUpdatedByFranchisee)
                    {
                        var floorGrindingAdjustmentNote = new FloorGrindingAdjustmentNotes();
                        floorGrindingAdjustmentNote.Note = model.Note;
                        floorGrindingAdjustmentNote.FranchiseeId = franchiseeId;
                        floorGrindingAdjustmentNote.IsActive = true;
                        floorGrindingAdjustmentNote.IsNew = true;
                        floorGrindingAdjustmentNote.DataRecorderMetaData = new DataRecorderMetaData();
                        _floorGrindingAdjustmentNotesRepository.Save(floorGrindingAdjustmentNote);
                    }
                }
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).ToList();
                floorGrindingAdjustmentNotes = _floorGrindingAdjustmentNotesRepository.Table.Where(x => assignedFranchiseeIdList.Contains((long)x.FranchiseeId)).ToList();
                floorGrindingNotesIdNotUpdatedByFranchisee = floorGrindingAdjustmentNotes.Select(x => x.FranchiseeId).Distinct().ToList();

                if (floorGrindingAdjustmentNotes.Count == 0)
                {
                    foreach (var franchiseeId in assignedFranchiseeIdList)
                    {
                        var floorGrindingAdjustmentNote = new FloorGrindingAdjustmentNotes();
                        floorGrindingAdjustmentNote.Note = model.Note;
                        floorGrindingAdjustmentNote.FranchiseeId = franchiseeId;
                        floorGrindingAdjustmentNote.IsActive = true;
                        floorGrindingAdjustmentNote.IsNew = true;
                        floorGrindingAdjustmentNote.IsChangedByFranchisee = true;
                        floorGrindingAdjustmentNote.DataRecorderMetaData = new DataRecorderMetaData();
                        _floorGrindingAdjustmentNotesRepository.Save(floorGrindingAdjustmentNote);
                    }
                }
                else
                {
                    foreach (var note in floorGrindingAdjustmentNotes)
                    {
                        note.IsActive = false;
                        note.IsNew = false;
                        note.DataRecorderMetaData = new DataRecorderMetaData();
                        _floorGrindingAdjustmentNotesRepository.Save(note);
                    }
                    foreach (var franchiseeId in floorGrindingNotesIdNotUpdatedByFranchisee)
                    {
                        var floorGrindingAdjustmentNote = new FloorGrindingAdjustmentNotes();
                        floorGrindingAdjustmentNote.Note = model.Note;
                        floorGrindingAdjustmentNote.FranchiseeId = franchiseeId;
                        floorGrindingAdjustmentNote.IsActive = true;
                        floorGrindingAdjustmentNote.IsNew = true;
                        floorGrindingAdjustmentNote.IsChangedByFranchisee = true;
                        floorGrindingAdjustmentNote.DataRecorderMetaData = new DataRecorderMetaData();
                        _floorGrindingAdjustmentNotesRepository.Save(floorGrindingAdjustmentNote);

                    }
                }
            }
            return true;
        }

        public SeoHistryViewModel GetSeoHistry(SeoHistryModel model)
        {
            var honingMeasurementsList = _honingMeasurementRepository.Table.Where(x => x.ShiftName == model.Text && x.IsShiftPriceChanged == true).OrderByDescending(x => x.Id).ToList();
            var seoNotesList = _seoPriceNotesRepository.Table.OrderByDescending(x => x.Id).ToList();
            var count = honingMeasurementsList.Count();
            var finalcollection = honingMeasurementsList.Skip((model.pageNumber - 1) * model.pageSize).Take(model.pageSize).ToList();
            var honingMeasurementForSearchedTextList = finalcollection.Select(x => _reportFactory.CreateSeoViewModel(x, _perspnRepository.Table.FirstOrDefault(x1 => x.DataRecorderMetaData.CreatedBy == x1.Id), seoNotesList)).ToList();
            return new SeoHistryViewModel()
            {
                SeoHistryListModel = honingMeasurementForSearchedTextList,
                Count = count
            };
        }
        private int GetOrder(string material)
        {
            int order = default(int);
            switch (material)
            {
                case "Marble":
                    order = 1;
                    break;
                case "Granite":
                    order = 2;
                    break;
                case "Ceramic":
                    order = 3;
                    break;
                case "Concrete":
                    order = 4;
                    break;
                case "Terrazzo":
                    order = 5;
                    break;
                case "Vinyl":
                    order = 6;
                    break;
                case "Terracotta":
                    order = 7;
                    break;
                case "Slate":
                    order = 8;
                    break;
            }
            return order;
        }

        public bool SaveSeoNotes(SeoNotesModel model)
        {
            try
            {
                var seoNotes = new EstimatePriceNotes()
                {
                    DataRecorderMetaData = new DataRecorderMetaData(),
                    HoningmeasurementId = model.HoiningMeasurementId,
                    Notes = model.Notes,
                    IsNew = true,
                    IsDeleted = false
                };
                _seoPriceNotesRepository.Save(seoNotes);
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        public PriceEstimatePageViewModelForFA GetPriceEstimateValuesForFA(PriceEstimateGetModel getmodel, long userId, long roleUserId)
        {
            PriceEstimatePageViewModelForFA model = new PriceEstimatePageViewModelForFA();
            List<PriceEstimateViewModelForFA> ListModel = new List<PriceEstimateViewModelForFA>();
            var servicesTag = _servicesTagRepository.Table.Where(x => x.Service != null && x.IsActive).ToList();
            if (getmodel.CategoryId != default(int))
            {
                servicesTag = servicesTag.Where(x => x.CategoryId == getmodel.CategoryId).ToList();
            }
            if (getmodel.ServiceTypeId.Count > 0)
            {
                servicesTag = servicesTag.Where(x => getmodel.ServiceTypeId.Contains(x.ServiceTypeId)).ToList();
            }
            if (getmodel.ListOfService != null && getmodel.ListOfService.Count > 0)
            {
                servicesTag = servicesTag.Where(x => getmodel.ListOfService.Contains(x.Service)).ToList();
            }
            List<long> assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).Distinct().ToList();
            List<Franchisee> frachiseeList = _franchiseeRepository.Table.Where(x => assignedFranchiseeIdList.Contains(x.Organization.Id) && x.Id != 2 && !x.Organization.Name.StartsWith("0-")).ToList();
            foreach (var service in servicesTag)
            {
                foreach (var id in assignedFranchiseeIdList)
                {
                    var franchisee = frachiseeList.Where(x => x.Organization.Id == id).FirstOrDefault();
                    PriceEstimateViewModelForFA pricemodel = new PriceEstimateViewModelForFA();
                    var priceEstimates = _priceEstimateServicesRepository.Table.Where(x => x.ServiceTagId == service.Id).ToList();
                    if (priceEstimates.Count > 0)
                    {
                        var priceEstimatesForFranchisee = priceEstimates.Where(x => x.IsPriceChangedByFranchisee && x.FranchiseePrice != null && x.ServiceTagId == service.Id).ToList();
                        var priceEstimatesForFranchiseeAdditional = priceEstimates.Where(x => x.IsPriceChangedByFranchisee && x.FranchiseeAdditionalPrice != null && x.ServiceTagId == service.Id).ToList();
                        var priceEstimatesForFranchiseeDisplayingFP = priceEstimates.Where(x => x.IsPriceChangedByFranchisee && x.FranchiseeId == id).ToList();
                        var priceEstimatesForCorporate = priceEstimates.Where(x => x.ServiceTagId == service.Id).ToList();
                        
                        if(priceEstimatesForCorporate.Count > 0)
                        {
                            pricemodel.CorporatePrice = priceEstimatesForCorporate.LastOrDefault().CorporatePrice;
                            pricemodel.CorporateAdditionalPrice = priceEstimatesForCorporate.LastOrDefault().CorporateAdditionalPrice;

                            pricemodel.AverageFranchiseePrice = (decimal)(Math.Round((double)priceEstimatesForCorporate.Sum(x => x.FranchiseePrice) / (priceEstimatesForCorporate.Count), 2));
                            pricemodel.AverageFranchiseeAdditionalPrice = (decimal)(Math.Round((double)priceEstimatesForCorporate.Sum(x => x.FranchiseeAdditionalPrice) / (priceEstimatesForCorporate.Count), 2));

                            pricemodel.MaximumFranchiseePrice = priceEstimatesForCorporate.Max(x => x.FranchiseePrice);
                            var maxFranchiseePriceNames = priceEstimatesForCorporate.Where(x => x.FranchiseePrice == pricemodel.MaximumFranchiseePrice && x.Franchisee?.Organization != null).Select(x => x.Franchisee.Organization.Name).ToList();
                            //var maxFranchiseePriceNames = priceEstimatesForCorporate.Where(x => x.FranchiseePrice == pricemodel.MaximumFranchiseePrice).Select(x => x.Franchisee.Organization.Name).ToList();
                            pricemodel.MaximumFranchiseePriceName = String.Join(", ", maxFranchiseePriceNames);

                            pricemodel.MaximumFranchiseeAdditionalPrice = priceEstimatesForCorporate.Max(x => x.FranchiseeAdditionalPrice);
                            var maxFranchiseeAdditionalPriceNames = priceEstimatesForCorporate.Where(x => x.FranchiseeAdditionalPrice == pricemodel.MaximumFranchiseeAdditionalPrice && x.Franchisee?.Organization != null).Select(x => x.Franchisee.Organization.Name).ToList();
                            //var maxFranchiseeAdditionalPriceNames = priceEstimatesForCorporate.Where(x => x.FranchiseeAdditionalPrice == pricemodel.MaximumFranchiseeAdditionalPrice).Select(x => x.Franchisee.Organization.Name).ToList();
                            pricemodel.MaximumFranchiseeAdditionalPriceName = String.Join(", ", maxFranchiseeAdditionalPriceNames);
                        }

                        if (priceEstimatesForFranchisee.Count > 0)
                        {
                            pricemodel.BulkCorporatePrice = priceEstimatesForFranchisee.FirstOrDefault().BulkCorporatePrice;
                            pricemodel.BulkCorporateAdditionalPrice = priceEstimatesForFranchisee.FirstOrDefault().BulkCorporateAdditionalPrice;
                        }
                        //if (priceEstimatesForFranchiseeAdditional.Count > 0)
                        //{
                            
                            
                        //}
                        if (priceEstimatesForFranchiseeDisplayingFP.Count > 0)
                        {
                            pricemodel.FranchiseePrice = priceEstimatesForFranchiseeDisplayingFP.FirstOrDefault().FranchiseePrice;
                            pricemodel.FranchiseeAdditionalPrice = priceEstimatesForFranchiseeDisplayingFP.FirstOrDefault().FranchiseeAdditionalPrice;
                            pricemodel.Id = priceEstimatesForFranchiseeDisplayingFP.FirstOrDefault().Id;
                        }
                    }
                    pricemodel.FranchiseeName = franchisee != null && franchisee.Organization != null ? franchisee.Organization.Name : "";
                    pricemodel.ServiceTagId = service.Id;
                    pricemodel.Note = service.Notes;
                    pricemodel.Service = service.Service;
                    pricemodel.ServiceType = service.ServiceType.Name;
                    pricemodel.MaterialType = service.MaterialType;
                    pricemodel.Category = service.Category.Name;
                    pricemodel.CategoryId = service.Category.Id;
                    pricemodel.Unit = GetUnit(service.Category.Id);
                    ListModel.Add(pricemodel);
                }
            }
            var queryList = ListModel.AsQueryable();
            if (!string.IsNullOrEmpty(getmodel.SortingColumn))
            {
                switch (getmodel.SortingColumn)
                {
                    case "Material":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.MaterialType, getmodel.SortingOrder);
                        break;
                    case "CorporatePrice":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.BulkCorporatePrice, getmodel.SortingOrder);
                        break;
                    case "AverageFranchiseePrice":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.AverageFranchiseePrice, getmodel.SortingOrder);
                        break;
                    case "MaximumFranchiseePrice":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.MaximumFranchiseePrice, getmodel.SortingOrder);
                        break;
                    case "FranchiseeName":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.MaximumFranchiseePriceName, getmodel.SortingOrder);
                        break;
                    default:
                        queryList = queryList.OrderBy(x => x.Service).ThenBy(x => x.ServiceType);
                        break;
                }
            }
            else
            {
                queryList = queryList.OrderBy(x => x.Service).ThenBy(x => x.ServiceType);
            }

            return new PriceEstimatePageViewModelForFA
            {
                PriceEstimateViewModelForFAList = queryList.ToList()
            };
        }


        public bool DownloadPriceEstimateDataFile(PriceEstimateGetModel model, long userId, long roleUserId, out string fileName)
        {
            fileName = string.Empty;

            if (model.IsSuperAdmin)
            {
                var priceCollection = new List<PriceEstimateExcelViewModel>();
                PriceEstimatePageViewModel prices = GetPriceEstimateList(model, userId, roleUserId);
                foreach (var item in prices.PriceEstimateViewModelList)
                {
                    var excelModel = _reportFactory.CreatePriceEstimateViewModel(item);
                    priceCollection.Add(excelModel);
                }
                fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/priceEstimateData-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
                return _excelFileCreator.CreateExcelDocument(priceCollection, fileName);
            }
            else
            {
                var priceCollection = new List<PriceEstimateExcelViewModelForFA>();
                PriceEstimatePageViewModelForFA prices = GetPriceEstimateValuesForFA(model, userId, roleUserId);
                foreach (var item in prices.PriceEstimateViewModelForFAList)
                {
                    var excelModel = _reportFactory.CreatePriceEstimateViewModelForFA(item);
                    priceCollection.Add(excelModel);
                }
                fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/priceEstimateData-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
                return _excelFileCreator.CreateExcelDocument(priceCollection, fileName);
            }
        }

        public void SaveFile(PriceEstimateExcelUploadModel model)
        {
            var priceEstimateFileUpload = _reportFactory.CreatePriceEstimateExcelUploadViewModel(model);
            var file = _fileService.SaveModel(model.File);
            priceEstimateFileUpload.FileId = file.Id;
            if(model.RoleUserId == (long)RoleType.SuperAdmin)
            {
                priceEstimateFileUpload.IsFranchiseeAdmin = false;
            }
            else
            {
                priceEstimateFileUpload.IsFranchiseeAdmin = model.IsFranchiseeAdmin;
            }
            _priceEstimateFileUploadRepository.Save(priceEstimateFileUpload);
        }

        public PriceEstimateDataUploadListModel GetPriceEstimateUploadList(PriceEstimateDataListFilter filter, int pageNumber, int pageSize, long organisationRoleUserId)
        {
            var priceEstimateData = _priceEstimateFileUploadRepository.Table.Where(x=>x.DataRecorderMetaData.CreatedBy == organisationRoleUserId).AsQueryable();
            priceEstimateData = _sortingHelper.ApplySorting(priceEstimateData, x => x.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc);
            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        priceEstimateData = _sortingHelper.ApplySorting(priceEstimateData, x => x.Id, filter.SortingOrder);
                        break;
                    case "Status":
                        priceEstimateData = _sortingHelper.ApplySorting(priceEstimateData, x => x.Lookup.Name, filter.SortingOrder);
                        break;
                    case "UploadedDate":
                        priceEstimateData = _sortingHelper.ApplySorting(priceEstimateData, x => x.DataRecorderMetaData.DateCreated, filter.SortingOrder);
                        break;
                }
            }

            var list = priceEstimateData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PriceEstimateDataUploadListModel()
            {
                Collection = list.Select(_reportFactory.CreateViewModelPriceEstimateDataUpload).ToList(),
                PagingModel = new PagingModel(pageNumber, pageSize, priceEstimateData.Count()),
                Filter = filter
            };
        }
    
        public bool SaveServiceTagNotes(PriceEstimateServiceTagNotesModel filter, long userId)
        {
            var serviceTag = _servicesTagRepository.Table.Where(x => x.IsActive && x.Id == filter.ServiceTagId).FirstOrDefault();
            if (serviceTag != null)
            {
                serviceTag.Notes = filter.Note;
                serviceTag.NotesSavedBy = userId;
                serviceTag.IsNew = false;
                _servicesTagRepository.Save(serviceTag);
            }
            return true;
        }

        public PriceEstimateServiceTagNotesGetModel GetServiceTagNotes(PriceEstimateServiceTagNotesGetModel filter)
        {
            var serviceTag = _servicesTagRepository.Table.Where(x => x.IsActive && x.Id == filter.ServiceTagId).FirstOrDefault();
            string note = serviceTag.Notes;
            return new PriceEstimateServiceTagNotesGetModel()
            {
                ServiceTagId = filter.ServiceTagId,
                Note = note
            };
        }
    }
}
