using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Geo.Domain;
using Core.Geo.ViewModel;
using Core.Notification;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Reports;
using Core.Reports.Domain;
using Core.Review;
using Core.Review.Domain;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
using Core.Scheduler;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class SalesDataParsePollingAgent : ISalesDataParsePollingAgent
    {
        private ILogService _logService;
        private IClock _clock;
        private IFileService _fileService;
        private IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly ISettings _settings;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly ICustomerService _customerService;
        private readonly IInvoiceService _invoiceService;
        private readonly IInvoiceItemService _invoiceItemService;
        private readonly IFranchiseeSalesService _franchiseeSalesService;
        private readonly IAccountCreditService _creditMemoService;
        private readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        private readonly ICustomerFeedbackService _reviewService;
        private readonly ICustomerFeedbackFactory _customerFeedbackFactory;
        private IUnitOfWork _unitOfWork;
        private readonly IRepository<CustomerFeedbackRequest> _reviewFeedbackRequestRepository;
        private readonly ICustomerEmailAPIRecordFactory _customeremailAPIRecordFactory;
        private readonly IRepository<CustomerEmailAPIRecord> _customerEmailAPIRecordRepository;
        private readonly IRepository<PartialPaymentEmailApiRecord> _partialcustomerEmailAPIRecordRepository;
        private readonly IRepository<Organizations.Domain.ServiceType> _serviceTypeRepository;
        private readonly IUpdateBatchUploadRecordService _updateBatchUploadRecordService;
        private readonly IRepository<InvoicePayment> _invoicePaymentRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<InvoiceAddress> _invoiceAddressRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<JobScheduler> _jobScehdulerRepository;
        private readonly IRepository<CustomerEmail> _customerEmailRepository;
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Zip> _zipRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IJobService _jobService;
        public SalesDataParsePollingAgent(IUnitOfWork unitOfWork, ILogService logService, IFileService fileService, ISettings settings,
            ICustomerService customerService, IInvoiceService invoiceService, IInvoiceItemService invoiceItemService,
            IFranchiseeSalesService franchiseeSalesService, IAccountCreditService creditMemoService, IClock clock, ICustomerFeedbackService reviewService,
            ICustomerFeedbackFactory customerFeedbackFactory, ICustomerEmailAPIRecordFactory customeremailAPIRecordFactory,
            IUpdateBatchUploadRecordService updateBatchUploadRecordService, INotificationService notificationService, INotificationModelFactory notificationModelFactory, IJobService jobService)
        {
            _unitOfWork = unitOfWork;
            _invoiceRepository = unitOfWork.Repository<Invoice>();
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _logService = logService;
            _fileService = fileService;
            _customerService = customerService;
            _invoiceService = invoiceService;
            _invoiceItemService = invoiceItemService;
            _franchiseeSalesService = franchiseeSalesService;
            _creditMemoService = creditMemoService;
            _franchiseeSalesPaymentRepository = unitOfWork.Repository<FranchiseeSalesPayment>();
            _clock = clock;
            _settings = settings;
            _customerFeedbackFactory = customerFeedbackFactory;
            _reviewService = reviewService;
            _reviewFeedbackRequestRepository = unitOfWork.Repository<CustomerFeedbackRequest>();
            _customeremailAPIRecordFactory = customeremailAPIRecordFactory;
            _customerEmailAPIRecordRepository = unitOfWork.Repository<CustomerEmailAPIRecord>();
            _serviceTypeRepository = unitOfWork.Repository<Organizations.Domain.ServiceType>();
            _updateBatchUploadRecordService = updateBatchUploadRecordService;
            _paymentRepository = unitOfWork.Repository<Payment>();
            _invoicePaymentRepository = unitOfWork.Repository<InvoicePayment>();
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _partialcustomerEmailAPIRecordRepository = unitOfWork.Repository<PartialPaymentEmailApiRecord>();
            _invoiceAddressRepository = unitOfWork.Repository<InvoiceAddress>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _jobScehdulerRepository = unitOfWork.Repository<JobScheduler>();
            _customerEmailRepository = unitOfWork.Repository<CustomerEmail>();
            _jobService = jobService;
            _stateRepository = unitOfWork.Repository<State>();
            _cityRepository = unitOfWork.Repository<City>();
            _zipRepository = unitOfWork.Repository<Zip>();
            _countryRepository = unitOfWork.Repository<Country>();
        }

        public void ParseFile()
        {
            var salesDataUpload = GetFileToParse();
            if (salesDataUpload == null)
            {
                _logService.Debug("No file found for parsing");
                return;
            }
            //get country to use curency code
            var currencyExchangeRate = GetCurrencyExchangeRate(salesDataUpload.Franchisee, salesDataUpload.PeriodEndDate);
            salesDataUpload.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
            salesDataUpload.CurrencyExchangeRateId = currencyExchangeRate.Id;
            _salesDataUploadRepository.Save(salesDataUpload);
            _unitOfWork.SaveChanges();
            DataTable data;
            IList<ParsedFileParentModel> collection;
            var sbList = new List<string>();
            try
            {
                var filePath = MediaLocationHelper.FilePath(salesDataUpload.File.RelativeLocation, salesDataUpload.File.Name).ToFullPath();
                data = ExcelFileParser.ReadExcel(filePath);
                var salesDataFileParser = ApplicationManager.DependencyInjection.Resolve<ISalesDataFileParser>();
                salesDataFileParser.PrepareHeaderIndex(data);
                string message;
                if (!salesDataFileParser.CheckForValidHeader(data, out message))
                {
                    sbList.Add(Log("Please upload correct File! " + message));
                    CreateLogFile(sbList, "Sales_" + salesDataUpload.Id);
                    MarkUploadAsFailed(salesDataUpload);
                    return;
                }
                string result;
                if (!salesDataFileParser.CheckForValidClassName(data, out result))
                {
                    sbList.Add(Log("Please upload correct File! " + result));
                    CreateLogFile(sbList, "Sales_" + salesDataUpload.Id);
                    MarkUploadAsFailed(salesDataUpload);
                    return;
                }
                collection = salesDataFileParser.PrepareDomainFromDataTable(data);
                if (collection.Count <= 0)
                {
                    sbList.Add(Log(message));
                }
                var applyAddressAndPhoneValidation = _settings.ApplyAddressAndPhoneValidation;
                if (applyAddressAndPhoneValidation)
                {
                    var invalidRecords = 0;
                    invalidRecords = GetNumberOfRecordsFailingAddressPhoneValidation(collection, sbList, invalidRecords);
                    if (invalidRecords > 0)
                    {
                        CreateLogFile(sbList, "Sales_" + salesDataUpload.Id);
                        //Update Sales data upload
                        MarkUploadAsFailed(salesDataUpload);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                sbList.Add(Log("Some issue occured in File Parsing. Please check the file."));
                LogException(sbList, ex);
                CreateLogFile(sbList, "Sales_" + salesDataUpload.Id);
                //Update Sales data upload
                MarkUploadAsFailed(salesDataUpload);
                return;
            }
            // Check to implement if date range doesn't fall in between the selected period
            var invalidRecordCount = 0;
            foreach (var item in collection)
            {
                if (item.Date < salesDataUpload.PeriodStartDate || item.Date > salesDataUpload.PeriodEndDate)
                {
                    sbList.Add(Log(string.Format("Please upload the correct file. Found Customer {0} , that has InvoiceDate {1} outside the period selected.", item.Customer.Name, item.Date.ToShortDateString())));
                    invalidRecordCount++;
                    break;
                }
            }
            if (invalidRecordCount > 0)
            {
                CreateLogFile(sbList, "Sales_" + salesDataUpload.Id);
                //Update Sales data upload
                MarkUploadAsFailed(salesDataUpload);
                return;
            }
            // Check to implement if file is for some other Franchisee
            var parsedRecords = 0;
            var failedRecords = 0;
            decimal totalAmount = 0;
            decimal paidAmount = 0;
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            var customers = new List<long>();
            var invoices = new List<long>();
            int x = 0;
            var franchiseeSalesMainList = _franchiseeSalesRepository.Table.Where(fs => fs.Invoice != null && !string.IsNullOrEmpty(fs.QbInvoiceNumber)).ToList();
            var jobSchedulerDomainList = _jobScehdulerRepository.IncludeMultiple(x1 => x1.Job.JobCustomer).Where(y => y.FranchiseeId == salesDataUpload.FranchiseeId).ToList();
            var franchiseeSalesPaymentList = _franchiseeSalesPaymentRepository.Table.ToList();
            var customerEmailAPIRecordList = _customerEmailAPIRecordRepository.Table.Where(ce => ce.IsSynced).ToList();
            var serviceItemIdsMaintenance = _serviceTypeRepository.Table.Where(si => si.CategoryId != (long)ServiceTypeCategory.Maintenance).Select(y => y.Id).ToList();
            var customerRepository = _customerRepository.IncludeMultiple(x2 => x2.DataRecorderMetaData, x3 => x3.CustomerEmails, x5 => x5.Address, x6 => x6.Address.City, x5 => x5.Address.State, x5 => x5.Address.Zip, x5 => x5.Address.Country).ToList();
            foreach (var record in collection)
            {
                try
                {
                    if ((salesDataUpload.FranchiseeId == Convert.ToInt64(_settings.MlDisturbutionId)
                       ) && record.ServiceTypeId == (long)ServiceTypes.Stonelife)
                    {
                        record.ServiceTypeId = (long)ServiceTypes.OTHER;
                        var invoiceItems = record.Invoice.InvoiceItems.Select(x1 => x1).ToList();
                        foreach (var invoiceItem in invoiceItems)
                        {
                            invoiceItem.ItemId = (long)ServiceTypes.OTHER;
                        }
                        var paymentItems = record.Invoice.Payments.Select(x1 => x1.PaymentItems).ToList();
                        foreach (var paymentItem in paymentItems)
                        {
                            foreach (var payment in paymentItem)
                            {
                                payment.ItemId = (long)ServiceTypes.OTHER;
                            }
                        }
                    }
                    _unitOfWork.StartTransaction();
                    var franchiseeSalesList = franchiseeSalesMainList.Where(fs => fs.CustomerId == record.Customer.Id).ToList();
                    var stats = SaveModel(record, salesDataUpload, currencyExchangeRate, franchiseeSalesList,
                         franchiseeSalesPaymentList, customerEmailAPIRecordList, serviceItemIdsMaintenance, customerRepository);
                    totalAmount += stats.TotalAmount;
                    paidAmount += stats.PaidAmount;
                    if (!customers.Contains(stats.CustomerId)) customers.Add(stats.CustomerId);
                    if (!invoices.Contains(stats.InvoiceId)) invoices.Add(stats.InvoiceId);
                    sbList.Add(stats.Logs);
                    parsedRecords++;
                    _unitOfWork.SaveChanges();
                    var jobSchedulerDomain = jobSchedulerDomainList.Where(y => y.QBInvoiceNumber == record.QbIdentifier).FirstOrDefault();
                    if (jobSchedulerDomain != default(JobScheduler))
                    {
                        var franchiseeSales = franchiseeSalesMainList.FirstOrDefault(x1 => x1.QbInvoiceNumber == record.QbIdentifier && x1.FranchiseeId == salesDataUpload.FranchiseeId);
                        if (franchiseeSales != null)
                            _jobService.SaveFileForImageAttachment(stats.InvoiceId, franchiseeSales, jobSchedulerDomain.Job, 1, true, jobSchedulerDomain.Id);
                    }
                    foreach(var credit in record.Invoice.Payments)
                    {
                        creditAmount = creditAmount + (decimal)(credit.CreditAmount ?? 0).ToDefaultCurrency(currencyExchangeRate.Rate);
                    }
                    //creditAmount += (decimal)(record.Invoice.InvoiceItems.FirstOrDefault(z => z.CreditAmount != null)?.CreditAmount ?? 0);
                    foreach (var debit in record.Invoice.InvoiceItems)
                    {
                        debitAmount = debitAmount + (decimal)(debit.DebitAmount ?? 0).ToDefaultCurrency(currencyExchangeRate.Rate);
                    }
                    //debitAmount += (decimal)(record.Invoice.InvoiceItems.FirstOrDefault(z => z.DebitAmount != null)?.DebitAmount ?? 0);
                }
                catch (Exception ex)
                {
                    failedRecords++;
                    _unitOfWork.Rollback();
                    LogException(sbList, ex);
                }
                finally
                {
                    x++;
                    _unitOfWork.ResetContext();
                }
            }

            //Refresh SalesData upload after context is Reset
            salesDataUpload = _salesDataUploadRepository.Get(salesDataUpload.Id);

            //add all stats to sales data uipload
            //use file stream to save it in media location
            //save it in file table
            UpdateSalesDataStatus(salesDataUpload, totalAmount, paidAmount, customers.Count(), invoices.Count(), failedRecords, parsedRecords, creditAmount, debitAmount);
            CreateLogFile(sbList, "Sales_" + salesDataUpload.Id);

            //Update Sales data upload
            salesDataUpload.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            SaveSalesDataUpload(salesDataUpload);


            //save record in BatchUpload report_partialPaymentAPIRecordRepository
            if (salesDataUpload.StatusId == (long)SalesDataUploadStatus.Parsed)
            {
                try
                {
                    _updateBatchUploadRecordService.UpdateBatchRecord(salesDataUpload);
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Error saving Batch  upload Record :", ex.StackTrace));
                }
            }
        }

        private static int GetNumberOfRecordsFailingAddressPhoneValidation(IList<ParsedFileParentModel> collection, List<String> sbList, int invalidRecords)
        {
            foreach (var item in collection)
            {
                var address = item.Customer.Address;
                if (address == null || string.IsNullOrWhiteSpace(address.AddressLine1) || (address.StateId <= 0 && string.IsNullOrWhiteSpace(address.State)) ||
                    string.IsNullOrEmpty(address.City) || string.IsNullOrEmpty(address.ZipCode))
                {
                    sbList.Add(Log("Found Customer " + item.Customer.Name + " with incomplete Address.Please Upload correct File"));
                    invalidRecords++;
                }
                else if (string.IsNullOrWhiteSpace(item.Customer.Phone))
                {
                    sbList.Add(Log("Found Customer " + item.Customer.Name + " with No Phone Number.Please Upload correct File"));
                    invalidRecords++;
                }
                else if (item.Customer.Phone.Count() < 10)
                {
                    sbList.Add(Log("Found Customer " + item.Customer.Name + " with Invalid Phone Number.Please Include Area Code and Upload correct File"));
                    invalidRecords++;
                }
            }

            return invalidRecords;
        }

        private void MarkUploadAsFailed(SalesDataUpload salesDataUpload)
        {
            salesDataUpload.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            salesDataUpload.StatusId = (long)SalesDataUploadStatus.Failed;
            SaveSalesDataUpload(salesDataUpload);
        }

        private static void LogException(List<string> sbList, Exception ex)
        {
            sbList.Add(Log("Error - " + ex.Message));
            sbList.Add(Log("Error - " + ex.StackTrace));
            if (ex.InnerException != null && ex.InnerException.StackTrace != null)
                sbList.Add(Log("Error - " + ex.InnerException.StackTrace));
        }

        private void SaveSalesDataUpload(SalesDataUpload salesDataUpload)
        {
            try
            {
                _unitOfWork.StartTransaction();
                var fileModel = PrepareLogFileModel("Sales_" + salesDataUpload.Id);
                var file = _fileService.SaveModel(fileModel);
                salesDataUpload.ParsedLogFileId = file.Id;

                _salesDataUploadRepository.Save(salesDataUpload);

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw new Exception(ex.Message);
            }
        }

        private void UpdateSalesDataStatus(SalesDataUpload salesDataUpload, decimal totalAmount, decimal paidAmount, int noOfCust, int noOfInvoice, int failedRecords, int parsedRecords, decimal credit, decimal debit)
        {
            salesDataUpload.TotalAmount = totalAmount;
            salesDataUpload.PaidAmount = paidAmount;
            salesDataUpload.NumberOfCustomers = noOfCust;
            salesDataUpload.NumberOfInvoices = noOfInvoice;
            salesDataUpload.NumberOfFailedRecords = failedRecords;
            salesDataUpload.NumberOfParsedRecords = parsedRecords;
            salesDataUpload.StatusId = (long)(failedRecords > 0 && parsedRecords == 0 ? SalesDataUploadStatus.Failed : SalesDataUploadStatus.Parsed);
            salesDataUpload.PaidAmount = credit;
            salesDataUpload.AccruedAmount = debit;
        }

        private FileModel PrepareLogFileModel(string name)
        {
            var fileModel = new FileModel();
            fileModel.Name = name;
            fileModel.Caption = name;
            fileModel.MimeType = "application/text";
            fileModel.RelativeLocation = MediaLocationHelper.GetMediaLocationForLogs().Path.ToRelativePath();
            fileModel.Size = new FileInfo(MediaLocationHelper.GetMediaLocationForLogs().Path + "" + name).Length;
            return fileModel;
        }

        private void CreateLogFile(List<string> sbList, string fileName)
        {
            var path = MediaLocationHelper.GetMediaLocationForLogs().Path + fileName;
            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write(string.Join(" ", sbList).ToString());
            }
        }

        private SalesDataUpload GetFileToParse()
        {
            return _salesDataUploadRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded && x.IsActive).OrderBy(x => x.FranchiseeId).OrderBy(x => x.PeriodStartDate).FirstOrDefault();
        }

        private SaveModelStats SaveModel(ParsedFileParentModel record, SalesDataUpload salesDataUpload, 
            CurrencyExchangeRate currencyExchangeRate, List<FranchiseeSales> franchiseeSalesList, List<FranchiseeSalesPayment> franchiseeSalesPaymentsForExistingRecord, 
            List<CustomerEmailAPIRecord> customerEmailAPIRecords, List<long> serviceItemIdsForMaintainance, List<Customer> customerRepository)
        {
            long franchiseeId = salesDataUpload.FranchiseeId,
                salesDataUploadId = salesDataUpload.Id;
            var stats = new SaveModelStats();
            var franchiseeSales = _franchiseeSalesService.Get(record.QbIdentifier, franchiseeId, record.Customer.Name);
            record.Invoice.InvoiceItems.Select(x => x.CurrencyExchangeRateId = currencyExchangeRate.Id).ToList();
            record.Invoice.InvoiceItems.Select(x => x.Rate = x.Rate.ToDefaultCurrency(currencyExchangeRate.Rate)).ToList();
            record.Invoice.InvoiceItems.Select(x => x.Amount = x.Amount.ToDefaultCurrency(currencyExchangeRate.Rate)).ToList();
            record.Invoice.Payments.Select(x => x.CurrencyExchangeRateId = currencyExchangeRate.Id).ToList();
            record.Invoice.Payments.Select(x => x.Amount = x.Amount.ToDefaultCurrency(currencyExchangeRate.Rate)).ToList();
            stats.Logs += Log("Starting data save for Customer " + record.Customer.Name + " and QB-Invoice #" + record.QbIdentifier);
            var isCreditRecord = record.AccountCredit != null;
            // Create email API Record Domain
            var list = CreateCustomerEmailAPIRecord(record.Customer, franchiseeId, customerEmailAPIRecords);
            decimal totalAmount = 0;
            if (record.Customer != null && record.Customer.Address != null)
            {
                record.Customer.Address.CountryId = currencyExchangeRate.CountryId;
            }
            if (franchiseeSales != null)
            {
                stats.Logs += Log(string.Format("Found existing Invoice {0} & Customer {1} ", franchiseeSales.InvoiceId, franchiseeSales.CustomerId));
                if (isCreditRecord)
                    record.AccountCredit.Id = franchiseeSales.AccountCreditId != null ? franchiseeSales.AccountCreditId.Value : 0;
                else
                    record.Invoice.Id = franchiseeSales.InvoiceId.Value;
                FillCustomerModel(record.Customer, franchiseeSales.Customer);
            }
            else
            {
                var customer = SearchCustomer(record.Customer, stats, customerRepository);
                if (customer != null)
                    FillCustomerModel(record.Customer, customer);
                franchiseeSales = new FranchiseeSales
                {
                    IsNew = true,
                    QbInvoiceNumber = record.QbIdentifier,
                    FranchiseeId = franchiseeId,
                    ClassTypeId = record.MarketingClassId,
                    SalesRep = record.SalesRep,
                    Amount = totalAmount,
                    CurrencyExchangeRateId = currencyExchangeRate.Id,
                    DataRecorderMetaData = new DataRecorderMetaData { DateCreated = DateTime.UtcNow },
                    SubClassTypeId = record.SubMarketingClassId != null ? record.SubMarketingClassId.GetValueOrDefault() : default(long?)
                };
                franchiseeSales.SalesDataUploadId = salesDataUploadId;
            }
            //update customer sales and avg sales
            UpdateCustomerSales(record.Customer, record.Invoice, record.QbIdentifier, franchiseeSalesList);
            var savedCustomer = _customerService.SaveCustomer(record.Customer);
            franchiseeSales.CustomerId = savedCustomer.Id;
            // Save Email API Record
            try
            {
                foreach (var item in list)
                {
                    if (item.FranchiseeId != Convert.ToInt64(_settings.MlDisturbutionId) && (item.FranchiseeId != 67))
                    {
                        item.CustomerId = savedCustomer.Id;
                        _customerEmailAPIRecordRepository.Save(item);
                        stats.Logs += Log(string.Format("Saving Email Record {0} For API sync for Customer - {1}", item.CustomerEmail, item.Customer.Name));
                    }
                    else
                    {
                        stats.Logs += Log(string.Format("Cannot Save Email Record {0}", item.CustomerEmail));
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error saving Email record :", ex.StackTrace));
            }
            CustomerFeedbackRequest feedbackRequest = null;
            var currentDate = _clock.UtcNow;
            if (isCreditRecord)
            {
                if (record.AccountCredit.Id < 1)
                {
                    totalAmount = record.AccountCredit.AccountCreditItems.Sum(ci => ci.Amount);
                    franchiseeSales.Amount = totalAmount;
                    record.AccountCredit.CustomerId = savedCustomer.Id;
                    record.AccountCredit.AccountCreditItems.Select(x => x.CurrencyExchangeRateId = currencyExchangeRate.Id).ToList();
                    record.AccountCredit.AccountCreditItems.Select(x => x.Amount.ToDefaultCurrency(currencyExchangeRate.Rate)).ToList();
                    var creditMemo = _creditMemoService.Save(record.AccountCredit, currencyExchangeRate.Id);
                    franchiseeSales.AccountCreditId = creditMemo.Id;
                }
                else
                    record.AccountCredit.Id = franchiseeSales.AccountCreditId.Value;
                stats.Logs += Log(string.Format("Saving Credit Amount {0} for QB-Invoice # {1}", totalAmount, record.AccountCredit.QbInvoiceNumber));
            }
            else
            {
                totalAmount = record.Invoice.InvoiceItems.Sum(ii => ii.Amount);
                franchiseeSales.Amount = totalAmount;
                decimal paymentSumNow = record.Invoice.Payments.Sum(x => x.Amount);
                decimal invoiceSum = record.Invoice.InvoiceItems.Sum(x => x.Amount);
                if (record.Invoice.Id < 1)
                {
                    var invoice = CreateNewInvoice(record.Invoice, stats, currencyExchangeRate);
                    var customer = _customerRepository.Get(franchiseeSales.CustomerId);
                    var invoiceAddress = CreateNewInvoiceAddress(invoice, customer);
                    _invoiceAddressRepository.Save(invoiceAddress);
                    franchiseeSales.InvoiceId = invoice.Id;
                    stats.Logs += Log(string.Format("New QB Invoice {0} ", record.QbIdentifier));
                    try
                    {
                        var customerEmail = record.Customer.CustomerEmails.Any() ? record.Customer.CustomerEmails.FirstOrDefault().Email : null;
                        if ((invoiceSum - paymentSumNow) > 0)
                        {
                            if (customerEmail != null && IsNotMaintenanceService(invoice, serviceItemIdsForMaintainance) && (franchiseeSales.FranchiseeId != Convert.ToInt64(_settings.MlDisturbutionId)
                                && franchiseeSales.FranchiseeId != 67))
                            {
                                var savedModel = _customeremailAPIRecordFactory.CreateDomain(customerEmail, franchiseeSales.FranchiseeId, franchiseeSales.InvoiceId, savedCustomer.Id, (long)LookupTypes.PartialPayment);
                                _partialcustomerEmailAPIRecordRepository.Save(savedModel);
                            }
                            else
                            {
                                stats.Logs += Log(string.Format("Service System : no email Found for customer {0} or Invalid QBIdentifier or type of service is Maintenance!", record.Customer.Name));
                            }
                        }
                        else
                        {
                            if (record.Customer.CustomerEmails.Any() && record.QbIdentifier != null
                                && IsNotMaintenanceService(invoice, serviceItemIdsForMaintainance) &&
                                (franchiseeSales.FranchiseeId != Convert.ToInt64(_settings.MlDisturbutionId)
                                && franchiseeSales.FranchiseeId != 67))
                            {
                                var savedModel = _customeremailAPIRecordFactory.CreateDomain(customerEmail, franchiseeSales.FranchiseeId, franchiseeSales.InvoiceId, savedCustomer.Id, (long)LookupTypes.Paid);
                                _partialcustomerEmailAPIRecordRepository.Save(savedModel);
                                var response = _reviewService.TriggerEmail(savedCustomer, record.Customer, franchiseeId, record.QbIdentifier, customerEmail, record.MarketingClassId);
                                if (response != null && response.errorCode == 0)
                                {
                                    feedbackRequest = _customerFeedbackFactory.CreateDomain(savedCustomer.Id, franchiseeSales.Id, currentDate, customerEmail, record.QbIdentifier, false, franchiseeId, response.ReviewSystemRecordId);
                                    stats.Logs += Log(string.Format("Review System : Saving Feedback Request for API generated Record {0} ", record.Customer.Name));
                                }
                                else if (response != null && response.errorCode == 101)
                                {
                                    stats.Logs += Log(string.Format("Review System : FeedBack is Disabled for Record {0} ", record.Customer.Name));
                                }
                                else
                                {
                                    stats.Logs += Log(string.Format("Review System : Error Saving Feedback Request: {0}", response.errorMessage));
                                }
                            }
                            else
                            {
                                stats.Logs += Log(string.Format("Review System : no email Found for customer {0} or Invalid QBIdentifier or type of service is Maintenance!", record.Customer.Name));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Exception 1 :{0}", ex.Message));
                    }
                }
                else
                {
                    try
                    {
                        decimal totalAmountPaid = 0;
                        List<long> paymentIds = (_invoicePaymentRepository.Table.Where(x => x.InvoiceId == record.Invoice.Id)).Select(x => x.PaymentId).ToList();
                        var invoice = (_invoiceRepository.Table.Where(x => x.Id == record.Invoice.Id)).FirstOrDefault();
                        decimal totalAmountToBePaid = (_invoiceItemRepository.Table.Where(x => x.InvoiceId == record.Invoice.Id).Sum(x => x.Amount));
                        var customerEmail = record.Customer.CustomerEmails.Any() ? record.Customer.CustomerEmails.FirstOrDefault().Email : null;
                        foreach (var paymentId in paymentIds)
                        {
                            totalAmountPaid += (_paymentRepository.Table.Where(x => x.Id == paymentId).Select(x => x.Amount)).FirstOrDefault();
                        }
                        decimal remainingAmount = totalAmountToBePaid - (totalAmountPaid + paymentSumNow);
                        if (remainingAmount == 0)
                        {
                            if (customerEmail != null && IsNotMaintenanceService(invoice, serviceItemIdsForMaintainance) && (franchiseeSales.FranchiseeId != Convert.ToInt64(_settings.MlDisturbutionId)
                                && franchiseeSales.FranchiseeId != 67))
                            {
                                var savedModel = _customeremailAPIRecordFactory.CreateDomain(customerEmail, franchiseeSales.FranchiseeId, franchiseeSales.InvoiceId, savedCustomer.Id, (long)LookupTypes.Paid);
                                _partialcustomerEmailAPIRecordRepository.Save(savedModel);
                            }
                            if (record.Customer.CustomerEmails.Any() && record.QbIdentifier != null && IsNotMaintenanceService(invoice, serviceItemIdsForMaintainance))
                            {
                                var response = _reviewService.TriggerEmail(savedCustomer, record.Customer, franchiseeId, record.QbIdentifier, customerEmail, record.MarketingClassId);
                                if (response != null && response.errorCode == 0)
                                {
                                    feedbackRequest = _customerFeedbackFactory.CreateDomain(savedCustomer.Id, franchiseeSales.Id, currentDate, customerEmail, record.QbIdentifier, false, franchiseeId, response.ReviewSystemRecordId);
                                    stats.Logs += Log(string.Format("Review System : Saving Feedback Request for API generated Record {0} ", record.Customer.Name));
                                }
                                else
                                {
                                    stats.Logs += Log(string.Format("Review System : Error Saving Feedback Request: {0}", response.errorMessage));
                                }
                            }
                            else
                            {
                                stats.Logs += Log(string.Format("Review System : no email Found for customer {0} or Invalid QBIdentifier or type of service is Maintenance!", record.Customer.Name));
                            }
                        }
                        franchiseeSales = UpdatePaymentAndInvoiceItem(record, stats, franchiseeSales);
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Exception 2 :{0}", ex.Message));
                    }
                }
            }
            _franchiseeSalesRepository.Save(franchiseeSales);
            if (feedbackRequest != null)
            {
                feedbackRequest.IsNew = true;
                feedbackRequest.StatusId = null;
                feedbackRequest.FranchiseeSalesId = franchiseeSales.Id;
                feedbackRequest.AuditActionId = (long)AuditActionType.Pending;
                _reviewFeedbackRequestRepository.Save(feedbackRequest);
            }
            if (franchiseeSales != null && franchiseeSales.Invoice != null && franchiseeSales.Invoice.InvoicePayments != null)
            {
                foreach (var item in franchiseeSales.Invoice.InvoicePayments)
                {
                    var inDbFranchiseePayment = franchiseeSalesPaymentsForExistingRecord.Where(x => x.InvoiceId == item.InvoiceId && x.PaymentId == item.PaymentId).FirstOrDefault();
                    if (inDbFranchiseePayment != null)
                        continue;
                    var franchiseeSalespayment = new FranchiseeSalesPayment
                    {
                        FranchiseeSalesId = franchiseeSales.Id,
                        SalesDataUploadId = salesDataUploadId,
                        InvoiceId = item.InvoiceId,
                        PaymentId = item.PaymentId,
                        IsNew = true
                    };
                    _franchiseeSalesPaymentRepository.Save(franchiseeSalespayment);
                }
            }
            if (isCreditRecord)
                stats.CreditMemoId = franchiseeSales.AccountCreditId.Value;
            else
                stats.InvoiceId = franchiseeSales.InvoiceId.Value;
            stats.CustomerId = franchiseeSales.CustomerId;
            stats.TotalAmount = totalAmount;
            return stats;
        }


        private CustomerCreateEditModel UpdateCustomerSales(CustomerCreateEditModel customer, InvoiceEditModel invoice, string qbInvoice, List<FranchiseeSales> franchiseeSalesList)
        {
            var totalSales = invoice.InvoiceItems.Sum(x => x.Amount);
            var isNewRecord = !franchiseeSalesList.Any(x => x.QbInvoiceNumber.Equals(qbInvoice));
            if (franchiseeSalesList.Any())
            {
                totalSales += franchiseeSalesList.Sum(x => x.Amount);
            }
            customer.TotalSales = totalSales;
            customer.NoOfSales = isNewRecord ? franchiseeSalesList.Count() + 1 : franchiseeSalesList.Count();
            customer.AvgSales = (customer.TotalSales > 0 && customer.NoOfSales > 0) ? customer.TotalSales / customer.NoOfSales : null;
            return customer;
        }

        private List<CustomerEmailAPIRecord> CreateCustomerEmailAPIRecord(CustomerCreateEditModel customer, long franchiseeId, List<CustomerEmailAPIRecord> customerEmailAPIRecords)
        {
            CustomerEmailAPIRecord customerMailRecord = null;
            var list = new List<CustomerEmailAPIRecord>();

            if (customer.CustomerEmails.Any())
            {
                //save data 
                foreach (var item in customer.CustomerEmails)
                {
                    if (item.Email != null && !IsSyncedEmail(item.Email, customerEmailAPIRecords))
                    {
                        customerMailRecord = _customeremailAPIRecordFactory.CreateDomain(customer, item.Email, franchiseeId);
                        list.Add(customerMailRecord);
                    }
                }
            }
            return list;
        }

        private bool IsSyncedEmail(string email, List<CustomerEmailAPIRecord> customerEmailAPIRecords)
        {
            var record = customerEmailAPIRecords.Where(x => x.IsSynced && (x.CustomerEmail.ToLower().Trim().Equals(email.ToLower().Trim()))).ToList();
            if (record.Any())
                return true;
            return false;
        }

        private bool IsNotMaintenanceService(Invoice invoice, List<long> serviceItemIds)
        {
            var result = invoice.InvoiceItems.Any(x => (serviceItemIds.Contains(x.ItemId.Value)));
            return result;
        }

        private FranchiseeSales UpdatePaymentAndInvoiceItem(ParsedFileParentModel record, SaveModelStats stats, FranchiseeSales franchiseeSales)
        {
            if (record.Invoice.GeneratedOn != franchiseeSales.Invoice.GeneratedOn)
            {
                var inDbInvoiceItems = franchiseeSales.Invoice.InvoiceItems.ToArray();

                var invoiceItemsToSave = record.Invoice.InvoiceItems.Where(ivt => ivt.ItemTypeId == (long)InvoiceItemType.Discount).ToArray();
                foreach (var item in invoiceItemsToSave)
                {
                    item.InvoiceId = record.Invoice.Id;
                    _invoiceItemService.Save(item);
                }

                SanitizeInvoiceItems(record.Invoice);
            }

            var dbPayments = franchiseeSales.Invoice.InvoicePayments == null ? new Payment[0]
                : franchiseeSales.Invoice.InvoicePayments.Select(x => x.Payment).ToArray();

            var paymentsToSave = record.Invoice.Payments.Where(rp => !dbPayments.Any(dp => dp.Amount == rp.Amount && dp.Date == rp.Date)).ToArray();
            stats.TotalAmount = 0;
            stats.PaidAmount = paymentsToSave.Sum(ps => ps.Amount);

            foreach (var item in paymentsToSave)
            {
                _invoiceService.SavePaymentItem(franchiseeSales.Invoice, item);
            }

            stats.Logs += Log(string.Format("Invoice {0} already recorded in the system. Only updating the Paid Amount", record.QbIdentifier));

            return franchiseeSales;
        }

        private Invoice CreateNewInvoice(InvoiceEditModel invoice, SaveModelStats stats, CurrencyExchangeRate currencyExchangeRate)
        {
            SanitizeInvoiceItems(invoice);
            stats.PaidAmount = invoice.Payments.Sum(x => x.Amount);

            if (stats.TotalAmount <= stats.PaidAmount)
            {
                invoice.StatusId = (long)InvoiceStatus.Paid;
            }
            else if (stats.TotalAmount > stats.PaidAmount && stats.PaidAmount > 0)
            {
                invoice.StatusId = (long)InvoiceStatus.PartialPaid;
            }
            else
            {
                invoice.StatusId = (long)InvoiceStatus.Unpaid;
            }


            var savedInvoice = _invoiceService.Save(invoice);
            return savedInvoice;
        }

        private InvoiceAddress CreateNewInvoiceAddress(Invoice invoice, Customer customer)
        {
            var invoiceAddress = new InvoiceAddress
            {
                AddressLine1 = customer.Address != null ? customer.Address.AddressLine1 : "",
                AddressLine2 = customer.Address != null ? customer.Address.AddressLine2 : "",
                CityName = customer.Address != null && customer.Address.City != null ? customer.Address.City.Name : customer.Address.CityName,
                CityId = customer.Address != null && customer.Address.City != null ? customer.Address.City.Id : default(long?),
                StateName = customer.Address != null && customer.Address.State != null ? customer.Address.State.Name : customer.Address.StateName,
                StateId = customer.Address != null && customer.Address.State != null ? customer.Address.State.Id : default(long?),
                CountryId = customer.Address != null && customer.Address.Country != null ? customer.Address.CountryId : default(long?),
                InvoiceId = invoice.Id,
                Phone = customer.Phone,
                EmailId = customer.CustomerEmails.Count() > 0 ? customer.CustomerEmails.Select(x => x.Email).FirstOrDefault() : "",
                IsNew = true,
                ZipCode = customer.Address != null && customer.Address.Zip != null ? customer.Address.ZipCode : "",
                ZipId = customer.Address != null && customer.Address.Zip != null ? customer.Address.ZipId : default(long?),
                TypeId = 11,

            };
            return invoiceAddress;
        }

        private class ServiceDiscountItem
        {
            public long ServiceTypeId { get; set; }
            public decimal Discount { get; set; }
        }

        private void SanitizeInvoiceItems(InvoiceEditModel model)
        {
            var currencyExchangerateId = model.InvoiceItems.Any() ? model.InvoiceItems.Select(x => x.CurrencyExchangeRateId).First() : 1;
            ServiceDiscountItem unaccountedDiscount = new ServiceDiscountItem { };

            foreach (var item in model.InvoiceItems.Where(ii => ii.ItemTypeId == (long)InvoiceItemType.Discount).ToArray())
            {
                var discAmount = item.Amount < 0 ? item.Amount * -1 : item.Amount;
                var payment = model.Payments.FirstOrDefault(x => x.Amount == discAmount);
                if (payment == null)
                {
                    unaccountedDiscount.Discount += discAmount;
                    unaccountedDiscount.ServiceTypeId = item.ItemId != null ? item.ItemId.Value : 0;
                    continue;
                }
                model.Payments.Remove(payment);
            }

            if (unaccountedDiscount.Discount != 0 && unaccountedDiscount.ServiceTypeId != 0)
            {
                model.Payments.Add(new FranchiseeSalesPaymentEditModel
                {
                    Amount = -1 * unaccountedDiscount.Discount,
                    Date = model.GeneratedOn,
                    CurrencyExchangeRateId = currencyExchangerateId,
                    PaymentItems = new[]
                    {
                    new PaymentItemEditModel
                    {
                        ItemTypeId = (long)InvoiceItemType.Discount,
                        ItemId = unaccountedDiscount.ServiceTypeId !=0 ? unaccountedDiscount.ServiceTypeId : (long)ServiceTypes.Other //should be nullable
                    }
                }
                });
            }
        }

        private Customer SearchCustomer(CustomerCreateEditModel customer, SaveModelStats stats, List<Customer> customerRepository)
        {
            var customerColl = _customerService.GetCustomerByEmail(customer.CustomerEmails.Select(x => x.Email).ToList(), customerRepository);

            if (customerColl != null)
            {
                stats.Logs += Log(string.Format("Found existing Customer by email. Found Records {0}", string.Join(", ", customerColl.Select(x => string.Format("[Id:{0}, Name: {1}", x.Id, x.Name)).ToList())));
            }
            else
            {
                customerColl = _customerService.GetCustomerByPhone(customer.Phone, customerRepository);
                if (customerColl != null)
                {
                    stats.Logs += Log(string.Format("Found existing Customer by phone. Found Records {0}", string.Join(", ", customerColl.Select(x => string.Format("[Id:{0}, Name: {1}", x.Id, x.Name)).ToList())));
                }
            }

            if (customerColl != null && customerColl.Any() && customer.Address != null)
            {
                //foreach (var item in customerColl)
                //{
                //    if (item.Address == null) return item;
                //    if (CompareAddress(item.Address, customer.Address)) return item;
                //}

                var preFiltered = customerColl.Where(item => item.Address == null || (item.Address != null && !string.IsNullOrEmpty(customer.Address.AddressLine1)));
                var matchedCustomer = preFiltered.FirstOrDefault(item => item.Address == null || CompareAddress(item.Address, customer.Address));

                if (matchedCustomer != null)
                    return matchedCustomer;

                stats.Logs += Log(string.Format("Address did not match for the customers found.", customer.Id, customer.Phone));
                return null;
            }
            else if (customer.Address != null)
            {
                return _customerService.GetCustomerByNameAndAddress(customer.Name, customer.Address, customerRepository);
            }

            return customerColl.FirstOrDefault();
        }

        private bool CompareAddress(Address inDb, AddressEditModel model)
        {
            //if (MatchFieldsCaseInsensitive(model.AddressLine1, inDb.AddressLine1)
            //    && MatchFieldsCaseInsensitive(model.AddressLine2, inDb.AddressLine2)
            //    && (MatchFieldsCaseInsensitive(model.City, inDb.CityName) || (inDb.City != null && MatchFieldsCaseInsensitive(model.City, inDb.City.Name)))
            //    && (MatchFieldsCaseInsensitive(model.ZipCode, inDb.ZipCode) || (inDb.Zip != null && MatchFieldsCaseInsensitive(model.ZipCode, inDb.Zip.Code)))
            //    && ((inDb.State != null && (MatchFieldsCaseInsensitive(model.State, inDb.State.ShortName) || MatchFieldsCaseInsensitive(model.State, inDb.State.Name))
            //    || model.StateId == inDb.StateId || MatchFieldsCaseInsensitive(model.State, inDb.StateName))))
            //    return true;

            //return false;

            try
            {
                string dbAddress1 = inDb.AddressLine1?.ToLower().Trim();
                string modelAddress1 = model.AddressLine1?.ToLower().Trim();

                string dbAddress2 = inDb.AddressLine2?.ToLower().Trim();
                string modelAddress2 = model.AddressLine2?.ToLower().Trim();

                string dbCity = inDb.CityName?.ToLower().Trim();
                string modelCity = model.City?.ToLower().Trim();

                string dbZipCode = inDb.ZipCode?.ToLower().Trim();
                string modelZipCode = model.ZipCode?.ToLower().Trim();

                string dbStateShort = inDb.State?.ShortName?.ToLower().Trim();
                string dbStateName = inDb.State?.Name?.ToLower().Trim();
                string modelState = model.State?.ToLower().Trim();

                if (dbAddress1 != modelAddress1 || dbAddress2 != modelAddress2)
                    return false;

                if (dbCity != modelCity && (inDb.City == null || modelCity != inDb.City.Name?.ToLower().Trim()))
                    return false;

                if (dbZipCode != modelZipCode && (inDb.Zip == null || modelZipCode != inDb.Zip.Code?.ToLower().Trim()))
                    return false;

                if (inDb.State != null)
                {
                    if (modelState != dbStateShort && modelState != dbStateName && model.StateId != inDb.StateId)
                        return false;
                }
                else if (modelState != inDb.StateName?.ToLower().Trim())
                {
                    return false;
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private bool MatchFieldsCaseInsensitive(string field1, string field2)
        {
            if (string.IsNullOrWhiteSpace(field2) || string.IsNullOrWhiteSpace(field1)) return true;
            return field1.ToLower().Trim() == field2.ToLower().Trim();
        }

        private static void FillCustomerModel(CustomerCreateEditModel model, Customer inDb)
        {
            var listEmail = new List<CustomerEmail>();
            listEmail = model.CustomerEmails.ToList();
            model.Id = inDb.Id;
            model.DateCreated = inDb.DateCreated != null ? inDb.DateCreated : model.DateCreated;
            foreach (var inDbEmail in inDb.CustomerEmails)
            {
                foreach (var customerEmail in model.CustomerEmails)
                {
                    if (inDbEmail.Email != null && inDbEmail.Email.Equals(customerEmail.Email))
                        listEmail.Remove(customerEmail);
                }
            }
            model.CustomerEmails = listEmail;
        }

        private static string Log(string message)
        {
            return DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss:fff tt") + " \t\t " + message + "\r\n\r\n";
        }

        class SaveModelStats
        {
            public long InvoiceId;
            public long CreditMemoId;
            public long CustomerId;
            public decimal TotalAmount;
            public decimal PaidAmount;

            public string Logs;
        }

        private CurrencyExchangeRate GetCurrencyExchangeRate(Franchisee franchisee, DateTime endDate)
        {
            long countryId = franchisee.Organization.Address != null ? franchisee.Organization.Address.First().CountryId : 0;

            //var sDate = endDate.Date;
            //var eDate = endDate.AddDays(1).Date;

            var currencyExchangeRate = new CurrencyExchangeRate();
            if (countryId > 0)
            {
                //currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId && x.DateTime >= sDate && x.DateTime < endDate).OrderByDescending(y => y.DateTime).FirstOrDefault();

                currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId && x.DateTime.Year == endDate.Year && x.DateTime.Month == endDate.Month
                                        && x.DateTime.Day == endDate.Day).OrderByDescending(y => y.DateTime).FirstOrDefault();

                if (currencyExchangeRate == null)
                    currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId).OrderByDescending(y => y.DateTime).First();
                return currencyExchangeRate;
            }
            else
            {
                _logService.Debug("No Currency Code found for franchisee" + franchisee.OwnerName);
                return currencyExchangeRate;
            }
        }
    }
}
