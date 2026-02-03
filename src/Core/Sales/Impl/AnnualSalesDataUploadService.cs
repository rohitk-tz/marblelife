using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Geo;
using Core.Geo.Domain;
using Core.Geo.Impl;
using Core.Notification;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class AnnualSalesDataUploadService : IAnnualSalesDataUploadService
    {
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly ISalesDataUploadFactory _salesDataUploadFactory;
        private readonly IFileService _fileService;
        private readonly ISortingHelper _sortingHelper;
        private IUnitOfWork _unitOfWork;
        private readonly IRepository<FeeProfile> _feeProfileRepository;
        private readonly IClock _clock;
        private readonly ISettings _settings;
        private readonly IRepository<FranchiseeAccountCredit> _franchiseeAccountCreditRepository;
        private readonly IRepository<AnnualSalesDataUpload> _annualSalesDataUploadRepository;
        private readonly IRepository<AuditInvoice> _auditInvoiceRepository;
        private readonly IAuditFactory _auditFactory;
        private readonly IInvoiceService _invoiceService;
        private readonly IRepository<Lookup> _lookupRepository;
        private readonly IRepository<ServiceType> _serviceTypeRepository;
        private readonly IMarketingClassService _marketingClassService;
        private readonly IAnnualAuditNotificationService _annualAuditNotificationService;
        private readonly IRepository<SystemAuditRecord> _systemAuditRecordRepository;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly ISalesDataUploadService _salesDataUploadService;
        private readonly IRepository<AuditAddressDiscrepancy> _auditAddressRepository;
        private readonly IRepository<AddressHistryLog> _addressLogRepository;
        private readonly IRepository<AuditFranchiseeSales> _auditFranchiseeSalesRepository;
        private readonly AddressFactory _addressFactory;
        private readonly IEmailFactory _emailFactory;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<InvoiceAddress> _invoiceAddressRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Zip> _zipRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerEmail> _customerEmailRepository;
        private readonly IExcelFileFormaterCreator _excelFileFormatterCreator;
        public AnnualSalesDataUploadService(IUnitOfWork unitOfWork, ISalesDataUploadFactory salesDataUploadFactory, IFileService fileService,
            ISortingHelper sortingHelper, IClock clock, ISettings settings, IAuditFactory auditFactory,
            IInvoiceService invoiceService, IMarketingClassService marketingClassService, IAnnualAuditNotificationService annualAuditNotificationService,
            IExcelFileCreator excelFileCreator, ISalesDataUploadService salesDataUploadService, AddressFactory addressFactory, IEmailFactory emailFactory, IExcelFileFormaterCreator excelFileFormatterCreator)
        {
            _addressFactory = addressFactory;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _salesDataUploadFactory = salesDataUploadFactory;
            _fileService = fileService;
            _sortingHelper = sortingHelper;
            _feeProfileRepository = unitOfWork.Repository<FeeProfile>();
            _unitOfWork = unitOfWork;
            _clock = clock;
            _settings = settings;
            _franchiseeAccountCreditRepository = unitOfWork.Repository<FranchiseeAccountCredit>();
            _annualSalesDataUploadRepository = unitOfWork.Repository<AnnualSalesDataUpload>();
            _auditInvoiceRepository = unitOfWork.Repository<AuditInvoice>();
            _auditFactory = auditFactory;
            _invoiceService = invoiceService;
            _lookupRepository = unitOfWork.Repository<Lookup>();
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
            _marketingClassService = marketingClassService;
            _annualAuditNotificationService = annualAuditNotificationService;
            _systemAuditRecordRepository = unitOfWork.Repository<SystemAuditRecord>();
            _excelFileCreator = excelFileCreator;
            _salesDataUploadService = salesDataUploadService;
            _auditAddressRepository = unitOfWork.Repository<AuditAddressDiscrepancy>();
            _addressLogRepository = unitOfWork.Repository<AddressHistryLog>();
            _auditFranchiseeSalesRepository = unitOfWork.Repository<AuditFranchiseeSales>();
            _emailFactory = emailFactory;
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _invoiceAddressRepository = unitOfWork.Repository<InvoiceAddress>();
            _cityRepository = unitOfWork.Repository<City>();
            _stateRepository = unitOfWork.Repository<State>();
            _countryRepository = unitOfWork.Repository<Country>();
            _zipRepository = unitOfWork.Repository<Zip>();
            _addressRepository = unitOfWork.Repository<Address>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _customerEmailRepository = unitOfWork.Repository<CustomerEmail>();
            _excelFileFormatterCreator = excelFileFormatterCreator;
        }

        public SalesDataUploadListModel GetBatchList(SalesDataListFilter filter, int pageNumber, int pageSize)
        {
            var salesData = _annualSalesDataUploadRepository.Table.Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                             && (filter.StatusId < 1 || x.StatusId == filter.StatusId)
                             && (filter.ReviewStatusId < 1 || x.AuditActionId == filter.ReviewStatusId)
                             && ((filter.Year == null || x.PeriodStartDate.Year == filter.Year)));

            salesData = _sortingHelper.ApplySorting(salesData, x => x.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Id, filter.SortingOrder);
                        break;
                    case "Franchisee":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "StartDate":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.PeriodStartDate, filter.SortingOrder);
                        break;
                    case "EndDate":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.PeriodEndDate, filter.SortingOrder);
                        break;
                    case "TotalAmount":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.TotalAmount, filter.SortingOrder);
                        break;
                    case "PaidAmount":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.PaidAmount, filter.SortingOrder);
                        break;
                    case "Status":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Lookup.Name, filter.SortingOrder);
                        break;
                    case "UploadedDate":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.DataRecorderMetaData.DateCreated, filter.SortingOrder);
                        break;
                    case "ReviewStatus":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.AuditAction.Name, filter.SortingOrder);
                        break;

                }
            }

            var list = salesData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new SalesDataUploadListModel()
            {
                Collection = list.Select(_salesDataUploadFactory.CreateListModel).ToList(),
                PagingModel = new PagingModel(pageNumber, pageSize, salesData.Count()),
                Filter = filter
            };
        }

        public AnnualAuditSalesListModel GetAnnualSalesData(SalesDataListFilter filter)
        {

            IQueryable<AuditInvoice> collection = GetSaleFilterData(filter);
            List<AnnualGroupedReport> groupCollection = new List<AnnualGroupedReport>();
            var query = collection.ToList();

            var franchiseeId = _annualSalesDataUploadRepository.Table.Where(x => x.Id == filter.AnnualDataUploadId).Select(x => x.FranchiseeId).FirstOrDefault();
            var values = query.Select(_auditFactory.CreateViewModel).ToList();

            if (filter.IsAnnualAudit.GetValueOrDefault())
            {
                var systemInvoices = GetSystemAuditRecord(filter);
                values = values.Concat(systemInvoices).ToList();
            }

            var count = values.Count();
            filter.Count = count;

            groupCollection = getGroupedElements(values, filter, franchiseeId, false, "");
            return new AnnualAuditSalesListModel()
            {
                Filter = filter,
                GroupCollection = groupCollection.ToList()
            };
        }

        private IQueryable<AuditInvoice> GetSaleFilterData(SalesDataListFilter filter)
        {
            var collection = _auditInvoiceRepository.Table.Where(x => (filter.AnnualDataUploadId == x.AnnualUploadId) && x.isActive == true
                                                          && (string.IsNullOrEmpty(filter.QbInvoiceNumber) || (x.QBInvoiceNumber.Equals(filter.QbInvoiceNumber))));

            return collection;
        }

        private IQueryable<Auditaddress> GetSaleFilterDataSorting(SalesDataListFilter filter, IQueryable<Auditaddress> collection)
        {
            switch (filter.SortingColumn)
            {
                case "Id":
                    collection = _sortingHelper.ApplySorting(collection, x => x.Id, filter.SortingOrder);
                    break;
                case "InvoiceId":
                    collection = _sortingHelper.ApplySorting(collection, x => x.InvoiceId, filter.SortingOrder);
                    break;
                case "InvoiceDate":
                    collection = _sortingHelper.ApplySorting(collection, x => x.InvoiceDate, filter.SortingOrder);
                    break;
                case "QBInvoice":
                    collection = _sortingHelper.ApplySorting(collection, x => x.QbInvoice, filter.SortingOrder);
                    break;
                case "AnnualSalesAmount":
                    collection = _sortingHelper.ApplySorting(collection, x => x.AnnualSalesAmount, filter.SortingOrder);
                    break;
                case "AnnualPaidAmount":
                    collection = _sortingHelper.ApplySorting(collection, x => x.AnnualPaidAmount, filter.SortingOrder);
                    break;
                case "WeeklySalesAmount":
                    collection = _sortingHelper.ApplySorting(collection, x => x.WeeklySalesAmount, filter.SortingOrder);
                    break;
                case "WeeklyPaidAmount":
                    collection = _sortingHelper.ApplySorting(collection, x => x.WeeklyPaidAmount, filter.SortingOrder);
                    break;
                case "AnnualReportStatus":
                    collection = _sortingHelper.ApplySorting(collection, x => x.ReportTypeDescription, filter.SortingOrder);
                    break;
                case "CustomerName":
                    collection = _sortingHelper.ApplySorting(collection, x => x.CustomerName, filter.SortingOrder);
                    break;
                case "AnnuallyPaidDifferent":
                    collection = _sortingHelper.ApplySorting(collection, x => x.AnnuallyPaidDifference, filter.SortingOrder);
                    break;
                case "AnnuallySalesDifferent":
                    collection = _sortingHelper.ApplySorting(collection, x => x.AnnuallySalesDifference, filter.SortingOrder);
                    break;
                case "WeeklyDifferent":
                    collection = _sortingHelper.ApplySorting(collection, x => x.WeeklyDifference, filter.SortingOrder);
                    break;
                case "AnnuallyDifferent":
                    collection = _sortingHelper.ApplySorting(collection, x => x.AnnuallyDifference, filter.SortingOrder);
                    break;
            }
            return collection;
        }
        public AuditInvoiceViewModel InvoiceDetails(long invoiceId, long auditInvoiceId)
        {
            var systemInvoice = new InvoiceDetailsViewModel { };
            var auditInvoice = new InvoiceDetailsViewModel { };
            var auditInvoices = new InvoiceDetailsViewModel { };
            List<InvoiceDetailsViewModel> listAuditInvoices = new List<InvoiceDetailsViewModel> { };
            if (auditInvoiceId > 0)
            {
                auditInvoice = GetAuditInvoice(auditInvoiceId);
            }

            if (invoiceId > 0)
            {
                systemInvoice = _invoiceService.InvoiceDetails(invoiceId);
            }

            if (auditInvoiceId > 0)
            {
                if (auditInvoice != null && auditInvoice.QBInvoiceNumbers != null)
                {
                    string[] qbInvoiceIds = auditInvoice.QBInvoiceNumbers.Split(',');
                    foreach (var qbInvoiceId in qbInvoiceIds)
                    {
                        if (qbInvoiceId != "")
                        {
                            var auditInvoiceData = _auditInvoiceRepository.Table.FirstOrDefault(x => x.QBInvoiceNumber == qbInvoiceId);
                            auditInvoices = GetAuditInvoieDetail(auditInvoiceData);
                            listAuditInvoices.Add(auditInvoices);
                        }
                    }
                }
            }
            var invoiceDetails = new AuditInvoiceViewModel
            {
                AnnualUploadId = auditInvoice.AnnualUploadId,
                AuditInvoice = auditInvoice,
                SystemInvoice = systemInvoice,
                AuditInvoices = listAuditInvoices
            };
            return invoiceDetails;
        }

        public InvoiceDetailsViewModel GetAuditInvoice(long invoiceId)
        {
            var auditInvoice = _auditInvoiceRepository.Get(invoiceId);

            var invoice = new InvoiceDetailsViewModel { };
            if (auditInvoice != null)
                invoice = GetAuditInvoieDetail(auditInvoice);
            return invoice;
        }

        public InvoiceDetailsViewModel GetAuditInvoieDetail(AuditInvoice auditInvoice)
        {
            var auditCustomerAddress = _auditFranchiseeSalesRepository.Table.FirstOrDefault(x => x.AuditInvoiceId == auditInvoice.Id && x.QbInvoiceNumber == auditInvoice.QBInvoiceNumber);
            decimal totalAmount = auditInvoice.AuditInvoiceItems.Count() >= 1 ? auditInvoice.AuditInvoiceItems.ToList().Sum(x => x.Amount) : 0;
            decimal totalPayment = auditInvoice.AuditInvoicePayments.Count() >= 1 ? auditInvoice.AuditInvoicePayments.ToList().Sum(x => x.AuditPayment.Amount) : 0;
            var invoice = new InvoiceDetailsViewModel
            {
                AuditInvoiceId = auditInvoice.Id,
                AnnualUploadId = auditInvoice.AnnualUploadId,
                //InvoiceId = auditCustomerAddress.CustomerInvoiceId > 0 ? auditCustomerAddress.CustomerInvoiceId : auditInvoice.InvoiceId,
                InvoiceId =  auditCustomerAddress.AuditInvoiceId,
                QBInvoiceNumber = auditInvoice.QBInvoiceNumber,
                TotalAmount = totalAmount,
                TotalPayment = totalPayment,
                GrandTotal = totalAmount - totalPayment,
                Payments = CreatePaymentModelCollection(auditInvoice.AuditInvoicePayments),
                InvoiceItems = auditInvoice.AuditInvoiceItems.Select(x => _auditFactory.CreateViewModel(x)).ToList(),
                GeneratedOn = auditInvoice.GeneratedOn,
                CurrencyCode = auditInvoice.AnnualSalesDataUpload.Franchisee.Currency,
                ReportId = auditInvoice.ReportTypeId.GetValueOrDefault(),
                Address = _addressFactory.CreateViewModel(auditCustomerAddress),
                ContactPerson = auditCustomerAddress != null ? (auditCustomerAddress.Customer != null ? auditCustomerAddress.Customer.ContactPerson : auditCustomerAddress.AuditCustomer.ContactPerson) : null,
                Customer = auditCustomerAddress != null ? (auditCustomerAddress.Customer != null ? auditCustomerAddress.Customer.Name : auditCustomerAddress.AuditCustomer.Name) : "",
                FranchiseeName = auditCustomerAddress != null ? auditCustomerAddress.Franchisee.DisplayName : "",
                //CustomerEmails = auditCustomerAddress.Customer != null ? auditCustomerAddress.Customer.CustomerEmails.Select(x => _emailFactory.CreateEditModel(x, auditCustomerAddress.Customer.CustomerEmails.EmailId)).ToList()
                //                 : null,
                Email = auditCustomerAddress != null ? auditCustomerAddress.Customer != null ? auditCustomerAddress.Customer.CustomerEmails.Select(x => x.Email).FirstOrDefault() : auditCustomerAddress.AuditCustomer.AuditAddress.Email : "",
                PhoneNumber = auditCustomerAddress != null ? auditCustomerAddress.Customer != null ? auditCustomerAddress.Customer.Phone : auditCustomerAddress.AuditCustomer.Phone : "",
                QBInvoiceNumbers = auditInvoice.QBInvoiceNumbers,
                ReportName = auditInvoice.AnnualReportType != null ? auditInvoice.AnnualReportType.ReportTypeName + " " + auditInvoice.AnnualReportType.Description : ""
            };
            return invoice;
        }

        private ICollection<FranchiseeSalesPaymentEditModel> CreatePaymentModelCollection(ICollection<AuditInvoicePayment> list)
        {
            var collection = new List<FranchiseeSalesPaymentEditModel>();
            foreach (var item in list)
            {
                string instrumentType = null;
                if (item.AuditPayment.InstrumentTypeId >= 1)
                {
                    var lookupItem = _lookupRepository.Get(item.AuditPayment.InstrumentTypeId);
                    if (lookupItem != null)
                        instrumentType = lookupItem.Name;
                }
                var paymentModel = CreateViewModel(item.AuditPayment, instrumentType);
                collection.Add(paymentModel);
            }

            return collection;
        }

        private FranchiseeSalesPaymentEditModel CreateViewModel(AuditPayment domain, string instrumentType)
        {
            return new FranchiseeSalesPaymentEditModel()
            {
                Id = domain.Id,
                Date = domain.Date,
                Amount = domain.Amount,
                InstrumentTypeId = (domain.InstrumentTypeId <= 0) ? (long)InstrumentType.Cash : domain.InstrumentTypeId,
                InstrumentType = instrumentType != null ? instrumentType : null,
                PaymentItems = domain.AuditPaymentItems != null ? domain.AuditPaymentItems.Select(CreatePaymentItemModel).ToList() : new List<PaymentItemEditModel>()
            };
        }

        private PaymentItemEditModel CreatePaymentItemModel(AuditPaymentItem domain)
        {
            string item = "";
            if (domain.ItemTypeId == (long)InvoiceItemType.Service)
            {
                var serviceType = _serviceTypeRepository.Get(domain.ItemId);
                var marketingClass = _marketingClassService.GetMarketingClassByPaymentId(domain.PaymentId);
                if (serviceType != null)
                    item = serviceType.Name;

                if (marketingClass != null && marketingClass.Length > 0)
                    item = marketingClass + ":" + item;
            }

            return new PaymentItemEditModel()
            {
                ItemId = domain.ItemId,
                ItemTypeId = domain.ItemTypeId,
                Item = item
            };
        }

        public bool Delete(long id)
        {
            var record = _annualSalesDataUploadRepository.Get(id);
            if (record == null) return true;

            if (record.StatusId == (long)SalesDataUploadStatus.Failed)
            {
                _annualSalesDataUploadRepository.Delete(record);
                return true;
            }

            CleanDataForAnnualUpload(id);
            _annualSalesDataUploadRepository.Delete(record);

            return true;
        }

        private void CleanDataForAnnualUpload(long id)
        {
            var invoiceRepo = _unitOfWork.Repository<AuditInvoice>();
            var invoiceItemRepo = _unitOfWork.Repository<AuditInvoiceItem>();
            var paymentRepo = _unitOfWork.Repository<AuditPayment>();
            var paymentItemRepo = _unitOfWork.Repository<AuditPaymentItem>();
            var invoicePaymentRepo = _unitOfWork.Repository<AuditInvoicePayment>();
            var systemAuditRepo = _unitOfWork.Repository<SystemAuditRecord>();

            var queryInvoiceIds = invoiceRepo.Table.Where(i => i.AnnualUploadId == id).Select(x => x.Id);
            var invoiceItemIds = invoiceItemRepo.Table.Where(ii => queryInvoiceIds.Contains(ii.AuditInvoiceId)).Select(x => x.Id);
            var queryPaymentIds = invoicePaymentRepo.Table.Where(x => queryInvoiceIds.Contains(x.InvoiceId)).Select(x => x.PaymentId);

            systemAuditRepo.Delete(x => x.AnnualUploadId == id);

            invoiceItemRepo.Delete(x => invoiceItemIds.Contains(x.Id));
            invoiceRepo.Delete(x => queryInvoiceIds.Contains(x.Id));

            paymentItemRepo.Delete(p => queryPaymentIds.Contains(p.PaymentId));

            paymentRepo.Delete(x => queryPaymentIds.Contains(x.Id));
            invoicePaymentRepo.Delete(x => queryInvoiceIds.Contains(x.InvoiceId) || queryPaymentIds.Contains(x.PaymentId));

            invoiceItemRepo.Delete(x => queryInvoiceIds.Contains(x.AuditInvoiceId));
            invoiceRepo.Delete(x => queryInvoiceIds.Contains(x.Id));
        }

        public bool ManageBatch(bool isAccept, long batchId)
        {
            var record = _annualSalesDataUploadRepository.Get(batchId);
            if (record == null) return false;

            if (isAccept)
            {
                record.AuditActionId = (long)AuditActionType.Approved;
                _annualSalesDataUploadRepository.Save(record);
                _annualAuditNotificationService.CreateReviewActionNotification(record, isAccept);
                return true;
            }

            CleanDataForAnnualUpload(batchId);
            record.AuditActionId = (long)AuditActionType.Rejected;
            _annualSalesDataUploadRepository.Save(record);

            _annualAuditNotificationService.CreateReviewActionNotification(record, isAccept);

            return true;
        }

        public AnnualAuditSalesListModel GetAnnualAuditRecord(SalesDataListFilter filter)
        {
            var auditRecordList = _systemAuditRecordRepository.Table.Where(x => filter.AnnualDataUploadId == x.AnnualUploadId
                                                                && string.IsNullOrEmpty(filter.QbInvoiceNumber) || x.QBIdentifier.Equals(filter.QbInvoiceNumber)
                                                                ).ToList();


            return new AnnualAuditSalesListModel()
            {
                Collection = auditRecordList.Select(_auditFactory.CreateViewModel).ToList(),
                Filter = filter
            };
        }

        private List<Auditaddress> GetSystemAuditRecord(SalesDataListFilter filter)
        {
            var auditRecordList = _systemAuditRecordRepository.Table.Where(x => filter.AnnualDataUploadId == x.AnnualUploadId
                                                                && (string.IsNullOrEmpty(filter.QbInvoiceNumber) || x.QBIdentifier.Equals(filter.QbInvoiceNumber))
                                                                ).ToList();

            return auditRecordList.Select(_auditFactory.CreateViewModel).ToList();
        }

        public bool DownloadAnnualDataFile(SalesDataListFilter filter, out string fileName)
        {

            fileName = string.Empty;
            var ds = new DataSet();
            var salesDataCollection = new List<Auditaddress>();
            IQueryable<AuditInvoice> saleData = GetSaleFilterData(filter);
            IQueryable<SystemAuditRecord> systemAuditRecord = _systemAuditRecordRepository.Table.Where(x => filter.AnnualDataUploadId == x.AnnualUploadId
                                                                && string.IsNullOrEmpty(filter.QbInvoiceNumber) || x.QBIdentifier.Equals(filter.QbInvoiceNumber));

            var salesDataList = saleData.ToList();
            var systemAuditRecordList = systemAuditRecord.ToList();
            //prepare item collection
            foreach (var item in salesDataList)
            {
                var model = _auditFactory.CreateViewModel(item);
                salesDataCollection.Add(model);
            }
            foreach (var item in systemAuditRecordList)
            {
                var model = _auditFactory.CreateViewModel(item);
                salesDataCollection.Add(model);
            }
            if (filter.SortingColumn == null)
            {
                var value = salesDataCollection.AsQueryable();
                salesDataCollection = salesDataCollection.OrderBy(x => x.ReportTypeDescription).ThenByDescending(x => x.CustomerName).ToList();
            }
            else if (filter.SortingColumn != null)
            {
                salesDataCollection = GetSaleFilterDataSorting(filter, salesDataCollection.AsQueryable()).ToList();
            }
            //salesDataCollection = salesDataCollection.OrderBy(x => x.ReportTypeDescription).ThenByDescending(x=>x.CustomerName).ToList();
            ds.Tables.Add(_excelFileCreator.ListToDataTable(salesDataCollection, "AnnualData"));
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/annualSalesData-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(ds, fileName);
        }

        public bool DownloadAnnualDataFileFormatted(SalesDataListFilter filter, out string fileName)
        {
            fileName = string.Empty;
            var SortedElements = new List<AnnualGroupedReport>();
            var ds = new DataSet();
            var salesDataCollection = new List<Auditaddress>();
            IQueryable<AuditInvoice> saleData = GetSaleFilterData(filter);
            IQueryable<SystemAuditRecord> systemAuditRecord = _systemAuditRecordRepository.Table.Where(x => filter.AnnualDataUploadId == x.AnnualUploadId
                                                                && (string.IsNullOrEmpty(filter.QbInvoiceNumber) || x.QBIdentifier.Equals(filter.QbInvoiceNumber)));
            var franchiseeName = systemAuditRecord.FirstOrDefault() != null ? systemAuditRecord.FirstOrDefault().Franchisee.Organization.Name : "";
            var salesDataList = saleData.ToList();
            var systemAuditRecordList = systemAuditRecord.ToList();
            //prepare item collection
            foreach (var item in salesDataList)
            {
                var model = _auditFactory.CreateViewModel(item);
                salesDataCollection.Add(model);
            }
            foreach (var item in systemAuditRecordList)
            {
                var model = _auditFactory.CreateViewModel(item);
                salesDataCollection.Add(model);
            }
            var groupedElements = getGroupedElements(salesDataCollection, filter, null, true, franchiseeName);

            if (filter.DownloadSortOrder != null)
            {
                SortedElements = getSortedElements(filter.DownloadSortOrder.Distinct().ToList(), groupedElements);
                groupedElements = SortedElements;
            }

            //  fileName = "Annual Report_" + franchiseeName + ".xlsx";
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/annualSalesData-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileFormatterCreator.CreateExcelDocument(groupedElements, fileName);
        }

        private List<AnnualGroupedReport> getSortedElements(List<AnnualDownloadFilter> filter, List<AnnualGroupedReport> grouptedReports)
        {
            List<AnnualGroupedReport> sortedLists = new List<AnnualGroupedReport>();
            foreach (var grouptedReport in grouptedReports)
            {
                var isPresent = filter.Where(x => x.ReportTypeId == grouptedReport.ReportTypeId);
                var order = isPresent != null ? isPresent.LastOrDefault() : null;
                if (order != null)
                {
                    var sortedList = new List<Auditaddress>();
                    List<AnnualGroupedReport> sortedListsForSorting = new List<AnnualGroupedReport>();

                    sortedList = getDownloadSortedList(order, sortedList, grouptedReport);

                    grouptedReport.GroupedCollection = sortedList;
                    sortedLists.Add(grouptedReport);
                }
                else
                {
                    sortedLists.Add(grouptedReport);
                }
            }
            return sortedLists;
        }

        private List<Auditaddress> getDownloadSortedList(AnnualDownloadFilter order, List<Auditaddress> sortedList, AnnualGroupedReport grouptedReport)
        {

            switch (order.PropName)
            {
                case "InvoiceId":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.InvoiceId).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.InvoiceId).ToList();
                    break;

                case "InvoiceDate":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.InvoiceDate).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.InvoiceDate).ToList();
                    break;

                case "QBInvoice":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.QbInvoice).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.QbInvoice).ToList();
                    break;

                case "AnnualSalesAmount":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.AnnualSalesAmount).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.AnnualSalesAmount).ToList();
                    break;

                case "AnnualPaidAmount":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.AnnualPaidAmount).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.AnnualPaidAmount).ToList();
                    break;

                case "WeeklySalesAmount":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.WeeklySalesAmount).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.WeeklySalesAmount).ToList();
                    break;

                case "WeeklyPaidAmount":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.WeeklyPaidAmount).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.WeeklyPaidAmount).ToList();
                    break;

                case "AnnualReportStatus":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.ReportTypeDescription).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.ReportTypeDescription).ToList();
                    break;

                case "CustomerName":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.CustomerName).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.CustomerName).ToList();
                    break;

                case "AnnuallyPaidDifferent":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.AnnuallyPaidDifference).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.AnnuallyPaidDifference).ToList();
                    break;

                case "AnnuallySalesDifferent":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.AnnuallySalesDifference).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.AnnuallySalesDifference).ToList();
                    break;

                case "WeeklyDifferent":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.WeeklyDifference).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.WeeklyDifference).ToList();
                    break;

                case "AnnuallyDifferent":
                    if (order.Order == 0)
                        sortedList = grouptedReport.GroupedCollection.OrderBy(x => x.AnnuallyDifference).ToList();
                    else
                        sortedList = grouptedReport.GroupedCollection.OrderByDescending(x => x.AnnuallyDifference).ToList();
                    break;
            }
            return sortedList;
        }
        private List<AnnualGroupedReport> getGroupedElements(List<Auditaddress> salesDataCollection, SalesDataListFilter filter, long? franchiseeId, bool isFromDownload, string franchiseeName)
        {

            List<AnnualGroupedReport> groupCollection = new List<AnnualGroupedReport>();
            var underReporting = salesDataCollection.Where(x => x.ReportTypeId == (long)AuditReportType.Type1F).OrderByDescending(x => x.ReportTypeId).ToList();

            if (filter.SortingColumn != null && filter.ReportTypeId == (long)AuditReportType.Type1F)
            {
                underReporting = GetSaleFilterDataSorting(filter, underReporting.AsQueryable()).ToList();
            }
            underReporting = underReporting.OrderBy(x => x.QbInvoice).ToList();
            groupCollection.Add(new AnnualGroupedReport
            {
                GroupedCollection = underReporting,
                ReportTypeId = (long)AuditReportType.Type1F,
                ReportTypeDescription = "Under Reporting Candidate",
                TotalSum = underReporting.Sum(x => x.AnnuallyPaidDifference),
                CurrencyCode = underReporting.Count() > 0 ? underReporting[0].CurrencyCode : null,
                CurrencyRate = underReporting.Count() > 0 ? underReporting[0].CurrencyRate : default(decimal),
                FranchiseeId = franchiseeId.GetValueOrDefault()
            });


            var overReporting = salesDataCollection.Where(x => x.ReportTypeId == (long)AuditReportType.Type2B || x.ReportTypeId == (long)AuditReportType.Type4A || x.ReportTypeId == (long)AuditReportType.Type4B ||
                                                            x.ReportTypeId == (long)AuditReportType.Type18A || x.ReportTypeId == (long)AuditReportType.Type18B || x.ReportTypeId == (long)AuditReportType.Type6 || x.ReportTypeId == (long)AuditReportType.Type4
                                                              || x.ReportTypeId == (long)AuditReportType.Type17E || x.ReportTypeId == (long)AuditReportType.Type3).OrderByDescending(x => x.ReportTypeId).OrderByDescending(x => x.ReportTypeId).ToList();

            overReporting = overReporting.OrderBy(x => x.QbInvoice).ToList();
            if (filter.SortingColumn != null && filter.ReportTypeId == (long)AuditReportType.Type2B)
            {
                overReporting = GetSaleFilterDataSorting(filter, overReporting.AsQueryable()).ToList();
            }
            groupCollection.Add(new AnnualGroupedReport
            {
                GroupedCollection = overReporting,
                ReportTypeId = (long)AuditReportType.Type2B,
                ReportTypeDescription = "Over Reporting Candidate",
                TotalSum = overReporting.Sum(x => x.AnnuallyPaidDifference),
                CurrencyCode = overReporting.Count() > 0 ? overReporting[0].CurrencyCode : null,
                CurrencyRate = overReporting.Count() > 0 ? overReporting[0].CurrencyRate : default(decimal),
            });

            var noReview = salesDataCollection.Where(x => x.ReportTypeId == (long)AuditReportType.Type5 || x.ReportTypeId == (long)AuditReportType.Type1A
                                                           || x.ReportTypeId == (long)AuditReportType.Type1C || x.ReportTypeId == (long)AuditReportType.Type17A
                                                              || x.ReportTypeId == (long)AuditReportType.Type5B || x.ReportTypeId == (long)AuditReportType.Type17D
                                                               || x.ReportTypeId == (long)AuditReportType.Type14 || x.ReportTypeId == (long)AuditReportType.Type5A).OrderByDescending(x => x.ReportTypeId).ToList();

            noReview = noReview.OrderBy(x => x.QbInvoice).ToList();
            if (filter.SortingColumn != null && filter.ReportTypeId == (long)AuditReportType.Type5)
            {
                noReview = GetSaleFilterDataSorting(filter, noReview.AsQueryable()).OrderByDescending(x => x.ReportTypeId).ToList();
            }
            groupCollection.Add(new AnnualGroupedReport
            {
                GroupedCollection = noReview,
                ReportTypeId = (long)AuditReportType.Type5,
                ReportTypeDescription = "REVIEW - NO ACTION",
                TotalSum = noReview.Sum(x => x.AnnuallyPaidDifference),
                CurrencyCode = noReview.Count() > 0 ? noReview[0].CurrencyCode : null,
                CurrencyRate = noReview.Count() > 0 ? noReview[0].CurrencyRate : default(decimal),
            });

            var type1H = salesDataCollection.Where(x => x.ReportTypeId == (long)AuditReportType.Type1H).OrderByDescending(x => x.ReportTypeId).ToList();

            type1H = type1H.OrderBy(x => x.QbInvoice).ToList();
            if (filter.SortingColumn != null && filter.ReportTypeId == (long)AuditReportType.Type1H)
            {
                type1H = GetSaleFilterDataSorting(filter, type1H.AsQueryable()).ToList();
            }
            groupCollection.Add(new AnnualGroupedReport
            {
                GroupedCollection = type1H,
                ReportTypeId = (long)AuditReportType.Type1H,
                ReportTypeDescription = "NON ROYALTY REPORTING CLASS",
                TotalSum = type1H.Sum(x => x.AnnuallyPaidDifference),
                CurrencyCode = type1H.Count() > 0 ? type1H[0].CurrencyCode : null,
                CurrencyRate = type1H.Count() > 0 ? type1H[0].CurrencyRate : default(decimal),
            });



            var missingReport = salesDataCollection.Where(x => x.ReportTypeId == (long)AuditReportType.Type1B).OrderByDescending(x => x.ReportTypeId).ToList();
            missingReport = missingReport.OrderBy(x => x.QbInvoice).ToList();
            if (filter.SortingColumn != null && filter.ReportTypeId == (long)AuditReportType.Type1B)
            {
                missingReport = GetSaleFilterDataSorting(filter, missingReport.AsQueryable()).ToList();
            }
            groupCollection.Add(new AnnualGroupedReport
            {
                GroupedCollection = missingReport,

                ReportTypeId = (long)AuditReportType.Type1B,
                ReportTypeDescription = "Missing Report",
                TotalSum = missingReport.Sum(x => x.AnnuallyPaidDifference),
                CurrencyCode = missingReport.Count() > 0 ? missingReport[0].CurrencyCode : null,
                CurrencyRate = missingReport.Count() > 0 ? missingReport[0].CurrencyRate : default(decimal),
            });

            var missingPaymentReport = salesDataCollection.Where(x => x.ReportTypeId == (long)AuditReportType.Type1D).OrderByDescending(x => x.ReportTypeId).ToList();

            missingPaymentReport = missingPaymentReport.OrderBy(x => x.QbInvoice).ToList();
            if (filter.SortingColumn != null && filter.ReportTypeId == (long)AuditReportType.Type1D)
            {
                missingPaymentReport = GetSaleFilterDataSorting(filter, missingPaymentReport.AsQueryable()).ToList();
            }
            groupCollection.Add(new AnnualGroupedReport
            {
                GroupedCollection = missingPaymentReport,
                ReportTypeId = (long)AuditReportType.Type1D,
                ReportTypeDescription = "Missing Payment Report",
                TotalSum = missingPaymentReport.Sum(x => x.AnnuallyPaidDifference),
                CurrencyCode = missingPaymentReport.Count() > 0 ? missingPaymentReport[0].CurrencyCode : null,
                CurrencyRate = missingPaymentReport.Count() > 0 ? missingPaymentReport[0].CurrencyRate : default(decimal),

            });

            var reviewIndivsiReport = salesDataCollection.Where(x => x.ReportTypeId == (long)AuditReportType.Type2A || x.ReportTypeId == (long)AuditReportType.Type7).OrderByDescending(x => x.ReportTypeId).ToList();

            reviewIndivsiReport = reviewIndivsiReport.OrderBy(x => x.QbInvoice).ToList();
            if (filter.SortingColumn != null && filter.ReportTypeId == (long)AuditReportType.Type7)
            {
                reviewIndivsiReport = GetSaleFilterDataSorting(filter, reviewIndivsiReport.AsQueryable()).ToList();
            }
            groupCollection.Add(new AnnualGroupedReport
            {
                GroupedCollection = reviewIndivsiReport,

                ReportTypeId = (long)AuditReportType.Type7,
                ReportTypeDescription = "REVIEW INDIVIDUALLY",
                TotalSum = reviewIndivsiReport.Sum(x => x.AnnuallyPaidDifference),
                CurrencyCode = reviewIndivsiReport.Count() > 0 ? reviewIndivsiReport[0].CurrencyCode : null,
                CurrencyRate = reviewIndivsiReport.Count() > 0 ? reviewIndivsiReport[0].CurrencyRate : default(decimal),
            });

            var rouding = salesDataCollection.Where(x => x.ReportTypeId == (long)AuditReportType.Type9).OrderByDescending(x => x.ReportTypeId).ToList();

            rouding = rouding.OrderBy(x => x.QbInvoice).ToList();
            if (filter.SortingColumn != null && filter.ReportTypeId == (long)AuditReportType.Type9)
            {
                rouding = GetSaleFilterDataSorting(filter, rouding.AsQueryable()).ToList();
            }
            groupCollection.Add(new AnnualGroupedReport
            {
                GroupedCollection = rouding,

                ReportTypeId = (long)AuditReportType.Type9,
                ReportTypeDescription = "ROUNDING",
                TotalSum = rouding.Sum(x => x.AnnuallyPaidDifference),
                CurrencyCode = rouding.Count() > 0 ? rouding[0].CurrencyCode : null,
                CurrencyRate = rouding.Count() > 0 ? rouding[0].CurrencyRate : default(decimal),
            });

            var splitInvoice = salesDataCollection.Where(x => x.ReportTypeId == (long)AuditReportType.Type13 || x.ReportTypeId == (long)AuditReportType.Type13B).OrderByDescending(x => x.ReportTypeId).ToList();
            if (filter.SortingColumn != null && filter.ReportTypeId == (long)AuditReportType.Type13B)
            {
                splitInvoice = GetSaleFilterDataSorting(filter, splitInvoice.AsQueryable()).ToList();
            }
            splitInvoice = splitInvoice.OrderBy(x => x.QbInvoice).ToList();
            groupCollection.Add(new AnnualGroupedReport
            {
                GroupedCollection = splitInvoice,

                ReportTypeId = (long)AuditReportType.Type13B,
                ReportTypeDescription = "SPLIT INVOICES",
                TotalSum = splitInvoice.Sum(x => x.AnnuallyPaidDifference),
                CurrencyCode = splitInvoice.Count() > 0 ? splitInvoice[0].CurrencyCode : null,
                CurrencyRate = splitInvoice.Count() > 0 ? splitInvoice[0].CurrencyRate : default(decimal),
            });

            var otherTypesCollection = salesDataCollection.Where(x => x.ReportTypeId == 0 || x.ReportTypeId == (long)AuditReportType.Type8
                             || x.ReportTypeId == (long)AuditReportType.Type10B || x.ReportTypeId == (long)AuditReportType.Type11 || x.ReportTypeId == (long)AuditReportType.Type16 || x.ReportTypeId == (long)AuditReportType.Type17).OrderByDescending(x => x.ReportTypeId).ToList();

            otherTypesCollection = otherTypesCollection.OrderBy(x => x.QbInvoice).ToList();
            if (filter.SortingColumn != null && filter.ReportTypeId == (long)AuditReportType.Type17)
            {
                otherTypesCollection = GetSaleFilterDataSorting(filter, otherTypesCollection.AsQueryable()).ToList();
            }
            groupCollection.Add(new AnnualGroupedReport
            {
                GroupedCollection = otherTypesCollection,

                ReportTypeId = (long)AuditReportType.Type17,
                ReportTypeDescription = "Other Types",
                TotalSum = otherTypesCollection.Sum(x => x.AnnuallyPaidDifference),
                CurrencyCode = otherTypesCollection.Count() > 0 ? otherTypesCollection[0].CurrencyCode : null,
                CurrencyRate = otherTypesCollection.Count() > 0 ? otherTypesCollection[0].CurrencyRate : default(decimal),
            });

            if (filter.ReportTypeId != 0 && !isFromDownload && (filter.QbInvoiceNumber != "" || filter.QbInvoiceNumber != null))
            {

                var filteredList = new AnnualGroupedReport();
                foreach (var groupCollections in groupCollection)
                {
                    if (groupCollections.ReportTypeId == filter.ReportTypeId)
                    {
                        filteredList = groupCollections;
                        groupCollections.FranchiseeId = franchiseeId.GetValueOrDefault();
                        groupCollection.Clear();
                        groupCollection.Add(filteredList);
                        break;
                    }
                }
            }
            return groupCollection;
        }
        public bool SaveUpload(AnnualDataUploadCreateModel model)
        {
            var startDate = new DateTime(Convert.ToInt32(model.year), 1, 1);
            var endDate = new DateTime(Convert.ToInt32(model.year), 12, 31);
            var franchisee = model.FranchiseeId > 0 ? _franchiseeRepository.Get(model.FranchiseeId) : null;
            if (franchisee == null)
                return false;
            var currencyRate = GetCurrencyExchangeRate(franchisee, model.AnnualUploadEndDate);

            var upload = new SalesDataUpload();
            var uploadModel = new SalesDataUploadCreateModel
            {
                Id = model.Id,
                AnnualFile = model.AnnualFile,
                AnnualUploadStartDate = startDate,
                AnnualUploadEndDate = endDate,
                FranchiseeId = model.FranchiseeId,
                StatusId = model.StatusId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                CurrencyExchareRateId = currencyRate.Id
            };
            _salesDataUploadService.SaveAnnualUpload(uploadModel, upload);
            return true;
        }

        private CurrencyExchangeRate GetCurrencyExchangeRate(Franchisee franchisee, DateTime endDate)
        {
            long countryId = franchisee.Organization.Address != null ? franchisee.Organization.Address.First().CountryId : 0;

            var currencyExchangeRate = new CurrencyExchangeRate();
            if (countryId > 0)
            {
                currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId && x.DateTime.Year == endDate.Year && x.DateTime.Month == endDate.Month
                                        && x.DateTime.Day == endDate.Day).OrderByDescending(y => y.DateTime).FirstOrDefault();

                if (currencyExchangeRate == null)
                    currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId).OrderByDescending(y => y.DateTime).First();
                return currencyExchangeRate;
            }
            else
            {
                return currencyExchangeRate;
            }
        }

        public AnnualAuditSalesListModel GetAnnualSalesDataAddress(SalesDataListFilter filter)
        {
            IQueryable<AuditInvoice> collection = GetSaleFilterDataAddress(filter);
            var query = collection.ToList();
            var count = query.Count();


            return new AnnualAuditSalesListModel()
            {
                Collection = query.Select(_auditFactory.CreateViewModel).ToList(),
                Filter = filter,

            };
        }

        private IQueryable<AuditInvoice> GetSaleFilterDataAddress(SalesDataListFilter filter)
        {
            var collection = _auditInvoiceRepository.Table.Where(x => (filter.AnnualDataUploadId == x.AnnualUploadId));
            //&& (string.IsNullOrEmpty(filter.QbInvoiceNumber) || (x.QBInvoiceNumber.Equals(filter.QbInvoiceNumber))));

            collection = _sortingHelper.ApplySorting(collection, x => x.Id, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Id, filter.SortingOrder);
                        break;
                    case "InvoiceId":
                        collection = _sortingHelper.ApplySorting(collection, x => x.InvoiceId, filter.SortingOrder);
                        break;
                    case "InvoiceDate":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Invoice.GeneratedOn, filter.SortingOrder);
                        break;
                        //case "QBInvoice":
                        //    collection = _sortingHelper.ApplySorting(collection, x => x.QBInvoiceNumber, filter.SortingOrder);
                        //    break;
                }
            }
            return collection;
        }

        // to get List of Annual Sales Customers Address Changes 
        public AnnualSalesDataCustonerListModel GetAnnualSalesCustomerAddress(AnnualSalesDataListFiltercs filter, int pageNumber, int pageSize)
        {
            IQueryable<AnnualSalesDataCustomerViewModel> collection = null;
            var count = 0;
            if (filter.StatusId == 0 || filter.StatusId == 1)
            {
                collection = GetSaleFilterDataCustomerAudit(filter, out count);
            }
            if (filter.StatusId == 0 || filter.StatusId == 2)
            {
                collection = GetSaleFilterDataCustomerDiscrepancyAddress(filter, out count);
            }
            collection = _sortingHelper.ApplySorting(collection, x => x.Id, (long)SortingOrder.Desc);
            var query = collection != null ? collection.ToList() : default(List<AnnualSalesDataCustomerViewModel>);


            return new AnnualSalesDataCustonerListModel()
            {
                Collection = query,
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, count),
            };
        }

        private IQueryable<AnnualSalesDataCustomerViewModel> GetSaleFilterDataCustomerAudit(AnnualSalesDataListFiltercs filter, out int count)
        {

            var collection = _auditAddressRepository.Table.Where(x => ((filter.QbInvoiceNumber) == "" || filter.QbInvoiceNumber == x.FranchiseeSales.QbInvoiceNumber)
                                                          && ((filter.FranchiseeId) == 0 || (x.FranchiseeSales.FranchiseeId.Equals(filter.FranchiseeId)))
                                                          && ((filter.MarketingClassId == 0 && x.FranchiseeSales != null) || (x.FranchiseeSales.MarketingClass.Id == filter.MarketingClassId))
                                                          && ((filter.PeriodStartDate == null && filter.PeriodEndDate == null)
                                                          || (((x.Invoice != null ? x.Invoice.GeneratedOn : x.invoiceDate) >= filter.PeriodStartDate)
                                                          && ((x.Invoice != null ? x.Invoice.GeneratedOn : x.invoiceDate) <= filter.PeriodEndDate))));


            var invoiceQbinvoiceList3 = (from item in collection
                                         orderby item.FranchiseeSales.Invoice.GeneratedOn descending
                                         group item by item.FranchiseeSales.Customer.Name into newGroup
                                         select newGroup).ToList();

            var invoiceQbinvoiceList2 = (from item in invoiceQbinvoiceList3
                                         select item.OrderByDescending(x => x.FranchiseeSales.Invoice.GeneratedOn).FirstOrDefault()).ToList();

            var invoiceQbinvoiceList = (from item in invoiceQbinvoiceList2
                                        select new QbInvoiceList
                                        {
                                            QbInvoice = item.FranchiseeSales.QbInvoiceNumber,
                                            InvoiceId = item.FranchiseeSales.InvoiceId
                                        }).ToList();

            count = collection.Count();
            collection = collection.OrderBy(x => x.FranchiseeSales.Customer.Name);
            collection = (collection.ToList().Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)).AsQueryable();
            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Id, filter.SortingOrder);
                        break;
                    case "QBInvoice":
                        collection = _sortingHelper.ApplySorting(collection, x => x.FranchiseeSales.QbInvoiceNumber, filter.SortingOrder);
                        break;
                    case "MarketingClass":
                        collection = _sortingHelper.ApplySorting(collection, x => x.FranchiseeSales.MarketingClass.Name, filter.SortingOrder);
                        break;
                }
            }
            var finalCollection = collection.ToList();

            //x => _jobFactory.CreateViewModel(x, query)
            return finalCollection.Select(x => _auditFactory.CreateViewModelForCustomers(x, invoiceQbinvoiceList)).AsQueryable();
            //return finalCollection.Select(_auditFactory.CreateViewModelForCustomers).AsQueryable();
        }
        private IQueryable<AnnualSalesDataCustomerViewModel> GetSaleFilterDataCustomerDiscrepancyAddress(AnnualSalesDataListFiltercs filter, out int count)
        {

            var collection = _addressLogRepository.Table.Where(x => ((filter.QbInvoiceNumber) == "" || filter.QbInvoiceNumber == x.FranchiseeSales.QbInvoiceNumber)
                                                          && ((filter.FranchiseeId) == 0 || (x.FranchiseeSales.FranchiseeId.Equals(filter.FranchiseeId)))
                                                          && ((filter.MarketingClassId == 0 && x.FranchiseeSales != null) || (x.FranchiseeSales.ClassTypeId == filter.MarketingClassId))
                                                          && ((filter.PeriodStartDate == null && filter.PeriodEndDate == null)
                                                          || (((x.Invoice != null ? x.Invoice.GeneratedOn : x.invoiceDate) >= filter.PeriodStartDate)
                                                          && ((x.Invoice != null ? x.Invoice.GeneratedOn : x.invoiceDate) <= filter.PeriodEndDate))));

            var invoiceQbinvoiceList3 = (from item in collection
                                         orderby item.FranchiseeSales.Invoice.GeneratedOn descending
                                         group item by item.FranchiseeSales.Customer.Name into newGroup
                                         select newGroup).ToList();

            var invoiceQbinvoiceList2 = (from item in invoiceQbinvoiceList3
                                         select item.OrderByDescending(x => x.FranchiseeSales.Invoice.GeneratedOn).FirstOrDefault()).ToList();

            var invoiceQbinvoiceList = (from item in invoiceQbinvoiceList2
                                        select new QbInvoiceList
                                        {
                                            QbInvoice = item.FranchiseeSales.QbInvoiceNumber,
                                            InvoiceId = item.FranchiseeSales.InvoiceId
                                        }).ToList();

            count = collection.Count();
            collection = collection.OrderBy(x => x.FranchiseeSales.Customer.Name);
            collection = (collection.ToList().Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)).AsQueryable();

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Id, filter.SortingOrder);
                        break;
                    case "QBInvoice":
                        collection = _sortingHelper.ApplySorting(collection, x => x.FranchiseeSales.QbInvoiceNumber, filter.SortingOrder);
                        break;
                    case "MarketingClass":
                        collection = _sortingHelper.ApplySorting(collection, x => x.FranchiseeSales.MarketingClass.Name, filter.SortingOrder);
                        break;
                }
            }
            var finalCollection = collection.ToList();
            return finalCollection.Select(x => _auditFactory.CreateViewModelForCustomers(x, invoiceQbinvoiceList)).AsQueryable();
        }

        public bool isValidUpload(AnnualDataUploadCreateModel model)
        {
            var startDate = new DateTime(Convert.ToInt32(model.year), 1, 1);
            var endDate = new DateTime(Convert.ToInt32(model.year), 12, 31);
            var uploadedOnStartDate = new DateTime(DateTime.Now.Year, 1, 1);
            var uploadedOnEndDate = new DateTime(DateTime.Now.Year, 12, 31);

            var isValid = _annualSalesDataUploadRepository.Table.Any(x => x.FranchiseeId == model.FranchiseeId && x.PeriodStartDate == startDate && x.PeriodEndDate == endDate &&
                                                    (x.DataRecorderMetaData.DateCreated >= uploadedOnStartDate && x.DataRecorderMetaData.DateCreated <= uploadedOnEndDate &&
                                                    x.StatusId == (long)SalesDataUploadStatus.Parsed && x.AuditActionId != (long)AuditActionType.Rejected));
            return isValid;
        }

        public bool UpdateCustomerAddress(AnnualSalesDataCustomerViewModel filter)
        {
            try
            {
                var invoiceId = long.Parse(filter.invoiceId);
                var franchiseeSales = _franchiseeSalesRepository.Table.Where(x => x.InvoiceId == invoiceId && x.QbInvoiceNumber == filter.qbInvoiceId).FirstOrDefault();
                var customer = franchiseeSales.Customer;
                var address = franchiseeSales.Customer.Address;
                var customerAddress = franchiseeSales.Invoice;
                var inDbInvoiceAddress = _invoiceAddressRepository.Table.Where(x => x.InvoiceId == invoiceId).FirstOrDefault();
                var city = _cityRepository.Table.Where(x => x.Name == filter.newCity).FirstOrDefault();
                var country = _countryRepository.Table.Where(x => x.Name == filter.newCountry).FirstOrDefault();
                var zip = _zipRepository.Table.Where(x => x.Code == filter.newZip).FirstOrDefault();
                var state = _stateRepository.Table.Where(x => x.Name == filter.newState).FirstOrDefault();
                var invoiceAddressId = inDbInvoiceAddress != null ? (long?)inDbInvoiceAddress.Id : null;
                var invoiceAddress = CreateInvoiceAddressViewModel(filter, invoiceAddressId, country, state, city, zip);
                _invoiceAddressRepository.Save(invoiceAddress);
                var addressValues = CreateAddressViewModel(filter, address.Id, country, state, city, zip);
                _addressRepository.Save(addressValues);

                var customerInfo = _customerRepository.Get(customer.Id);
                var customerPhone = CreateCustomerViewModel(filter, customerInfo);
                _customerRepository.Save(customerPhone);

                var customerEmailInfo = _customerEmailRepository.Table.Where(x => x.CustomerId == customer.Id).FirstOrDefault();
                var customerEmail = CreateCustomerEmailViewModel(filter, customerEmailInfo);
                if (customerEmail.CustomerId == default(long))
                {
                    customerEmail.CustomerId = customer.Id;
                }

                _customerEmailRepository.Save(customerEmail);
                _unitOfWork.SaveChanges();
            }
            catch (Exception e1)
            {
                _unitOfWork.Rollback();
                return false;
            }
            return true;
        }

        private InvoiceAddress CreateInvoiceAddressViewModel(AnnualSalesDataCustomerViewModel model, long? invoiceAddressId, Country country, State state, City city, Zip zip)
        {

            return new InvoiceAddress()
            {
                Id = invoiceAddressId.GetValueOrDefault(),
                AddressLine1 = model.newAddressLine1,
                AddressLine2 = model.newAddressLine2,
                CityName = model.newCity,
                CityId = city != null ? (long?)city.Id : null,
                StateId = state != null ? (long?)state.Id : null,
                CountryId = country != null ? (long?)country.Id : 1,
                ZipId = zip != null ? (long?)zip.Id : null,
                ZipCode = model.newZip,
                EmailId = model.newemailId,
                Phone = model.newphoneNumber,
                IsNew = invoiceAddressId.GetValueOrDefault() != null && invoiceAddressId.GetValueOrDefault() != default(long) ? false : true,
                TypeId = 11,
                StateName = model.newState,
                InvoiceId = long.Parse(model.invoiceId),


            };
        }
        private Address CreateAddressViewModel(AnnualSalesDataCustomerViewModel model, long? addressId, Country country, State state, City city, Zip zip)
        {

            return new Address()
            {
                Id = addressId.GetValueOrDefault(),
                AddressLine1 = model.newAddressLine1,
                AddressLine2 = model.newAddressLine2,
                CityName = model.newCity,
                CityId = city != null ? (long?)city.Id : null,
                StateId = state != null ? (long?)state.Id : null,
                CountryId = country != null ? country.Id : 1,
                ZipId = zip != null ? (long?)zip.Id : null,
                ZipCode = model.newZip,
                IsNew = addressId != null && addressId != default(long) ? false : true,
                TypeId = 11
            };
        }

        private Customer CreateCustomerViewModel(AnnualSalesDataCustomerViewModel model, Customer customer)
        {

            return new Customer()
            {
                Phone = model.newphoneNumber,
                IsNew = false,
                ContactPerson = customer.ContactPerson,
                CustomerEmails = customer.CustomerEmails,
                DataRecorderMetaData = customer.DataRecorderMetaData,
                AddressId = customer.AddressId,
                Name = customer.Name,
                IsDeleted = false,
                ClassTypeId = customer.ClassTypeId,
                MarketingClass = customer.MarketingClass,
                TotalSales = customer.TotalSales,
                NoOfSales = customer.NoOfSales,
                ReceiveNotification = customer.ReceiveNotification,
                AvgSales = customer.AvgSales,
                Address = customer.Address,
                DataRecorderMetaDataId = customer.DataRecorderMetaDataId,
                DateCreated = customer.DateCreated,
                Id = customer.Id
            };
        }
        private CustomerEmail CreateCustomerEmailViewModel(AnnualSalesDataCustomerViewModel model, CustomerEmail customerEmailInfo)
        {
            return new CustomerEmail()
            {
                CustomerId = customerEmailInfo != null ? customerEmailInfo.CustomerId : default(long),
                Email = model.newemailId,
                IsNew = customerEmailInfo != null ? false : true,
                Id = customerEmailInfo != null ? customerEmailInfo.Id : 0,
                DateCreated = customerEmailInfo != null ? customerEmailInfo.DateCreated : DateTime.Now,

            };
        }

        public bool ReparseAnnualReport(long? id)
        {
            try
            {
                var annualSalesData = _annualSalesDataUploadRepository.Get(id.GetValueOrDefault());
                var auditInvoices = _auditInvoiceRepository.Table.Where(x => x.AnnualUploadId == id && x.isActive).ToList();
                foreach (var auditInvoice in auditInvoices)
                {
                    auditInvoice.isActive = false;
                    auditInvoice.IsNew = false;
                    _auditInvoiceRepository.Save(auditInvoice);
                }
                var systemInvoices = _systemAuditRecordRepository.Table.Where(x => x.AnnualUploadId == id).ToList();
                foreach (var systemInvoice in systemInvoices)
                {
                    systemInvoice.IsDeleted = true;
                    systemInvoice.IsNew = false;
                    _systemAuditRecordRepository.Delete(systemInvoice);
                    //_systemAuditRecordRepository.Save(systemInvoice);
                }

                var annualSales = annaulSalesModel(annualSalesData);
                _annualSalesDataUploadRepository.Save(annualSales);

            }
            catch (Exception e1)
            {
                _unitOfWork.Rollback();
                return false;
            }
            _unitOfWork.SaveChanges();
            return true;
        }

        private AnnualSalesDataUpload annaulSalesModel(AnnualSalesDataUpload model)
        {
            model.IsNew = false;
            model.IsAuditAddressParsing = false;
            model.TotalAmount = 0;
            model.NoOfMismatchedRecords = 0;
            model.PaidAmount = 0;
            model.TotalAmount = 0;
            model.NoOfParsedRecords = 0;
            model.NoOfFailedRecords = 0;
            model.ParsedLogFileId = null;
            model.WeeklyRoyality = 0;
            model.AnnualRoyality = 0;
            model.StatusId = (long)SalesDataUploadStatus.Uploaded;

            return model;
        }

    }
}
