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
using Core.Sales.Domain;
using Core.Sales.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class AnnualSalesDataParsePollingAgent : IAnnualSalesDataParsePollingAgent
    {
        private readonly ILogService _logService;
        private readonly IRepository<AnnualSalesDataUpload> _annualFileUploadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IFranchiseeSalesService _franchiseeSalesService;
        private readonly IAuditFactory _auditFactory;
        private readonly IRepository<AuditInvoice> _auditInvoiceRepository;
        private readonly IRepository<AuditPayment> _auditPaymentRepository;
        private readonly IRepository<AuditInvoicePayment> _auditInvoicePaymentRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IAnnualAuditNotificationService _annualAuditNotificationService;
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly IRepository<SystemAuditRecord> _systemAuditRecordRepository;
        private readonly IRepository<Customer> _customersRepository;
        private readonly IRepository<AuditAddressDiscrepancy> _auditAddressDiscrepanctRepository;
        private readonly IRepository<AddressHistryLog> _addressLogRepository;
        private readonly IRepository<Lookup> _lookupRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<CustomerEmail> _customerEmailRepository;
        private readonly IRepository<AnnualReportType> _annualReportTypelRepository;
        private readonly IRepository<AuditCustomer> _annualCustomerRepository;
        private readonly IRepository<AuditAddress> _annualAddressRepository;
        private readonly IRepository<InvoiceAddress> _invoiceAddressRepository;
        private readonly IRepository<AuditFranchiseeSales> _auditFranchiseeSalesRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemsRepository;
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private readonly IRepository<FeeProfile> _feeProfileRepository;
        private readonly IRepository<RoyaltyFeeSlabs> _royalityFeeSlabsRepository;
        private readonly IRepository<AnnualRoyality> _annualRoyalityRepository;
        private readonly IRepository<FranchiseeService> _franchiseeServiceRepository;
        private readonly ISettings _setting;
        private DateTime startDate = new DateTime();
        private IInvoiceItemService _invoiceItemService;
        private DateTime endDate = new DateTime();
        private long? franchiseeId;
        private decimal? totallyYearlySales = default(decimal);
        public AnnualSalesDataParsePollingAgent(IUnitOfWork unitOfWork, ILogService logService, IFileService fileService, IFranchiseeSalesService franchiseeSalesService,
            IAuditFactory auditFactory, IAnnualAuditNotificationService annualAuditNotificationService, IInvoiceItemService invoiceItemService, ISettings setting)
        {
            _logService = logService;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _annualFileUploadRepository = unitOfWork.Repository<AnnualSalesDataUpload>();
            _franchiseeSalesService = franchiseeSalesService;
            _auditFactory = auditFactory;
            _auditInvoiceRepository = unitOfWork.Repository<AuditInvoice>();
            _auditPaymentRepository = unitOfWork.Repository<AuditPayment>();
            _auditInvoicePaymentRepository = unitOfWork.Repository<AuditInvoicePayment>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _annualAuditNotificationService = annualAuditNotificationService;
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _systemAuditRecordRepository = unitOfWork.Repository<SystemAuditRecord>();
            _customersRepository = unitOfWork.Repository<Customer>();
            _auditAddressDiscrepanctRepository = unitOfWork.Repository<AuditAddressDiscrepancy>();
            _addressLogRepository = unitOfWork.Repository<AddressHistryLog>();
            _addressRepository = unitOfWork.Repository<Address>();
            _customerEmailRepository = unitOfWork.Repository<CustomerEmail>();
            _lookupRepository = unitOfWork.Repository<Lookup>();
            _annualReportTypelRepository = unitOfWork.Repository<AnnualReportType>();
            _annualCustomerRepository = unitOfWork.Repository<AuditCustomer>();
            _annualAddressRepository = unitOfWork.Repository<AuditAddress>();
            _invoiceAddressRepository = unitOfWork.Repository<InvoiceAddress>();
            _auditFranchiseeSalesRepository = unitOfWork.Repository<AuditFranchiseeSales>();
            _invoiceRepository = unitOfWork.Repository<Invoice>();
            _invoiceItemsRepository = unitOfWork.Repository<InvoiceItem>();
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _feeProfileRepository = unitOfWork.Repository<FeeProfile>();
            _royalityFeeSlabsRepository = unitOfWork.Repository<RoyaltyFeeSlabs>();
            _annualRoyalityRepository = unitOfWork.Repository<AnnualRoyality>();
            _invoiceItemService = invoiceItemService;
            _setting = setting;
            _franchiseeServiceRepository = unitOfWork.Repository<FranchiseeService>();
        }
        public void ParseFile()
        {
            decimal weeklyRoyality = default(decimal);
            decimal anuallyRoyality = default(decimal);
            var annualFileUpload = GetFileToParse();

            if (annualFileUpload == null)
            {
                _logService.Debug("No file found for parsing");
                return;
            }

            _logService.Info("Starting annual file parsing for ," + annualFileUpload.Id);

            DataTable data;
            IList<ParsedFileParentModel> collection;
            var sb = new StringBuilder();
            startDate = annualFileUpload.PeriodStartDate;
            endDate = annualFileUpload.PeriodEndDate;
            franchiseeId = annualFileUpload.FranchiseeId;

            if (annualFileUpload.SalesDataUpload != null && annualFileUpload.SalesDataUpload.StatusId == (long)SalesDataUploadStatus.Failed)
            {
                var newUpload = _salesDataUploadRepository.Table.Where(sd => sd.FranchiseeId == annualFileUpload.FranchiseeId
                                            && sd.PeriodStartDate == annualFileUpload.SalesDataUpload.PeriodStartDate
                                            && sd.PeriodEndDate == annualFileUpload.SalesDataUpload.PeriodEndDate
                                            && sd.StatusId == (long)SalesDataUploadStatus.Parsed).OrderByDescending(y => y.Id).FirstOrDefault();
                if (newUpload == null)
                {
                    _logService.Info("Annual file can only be uploaded when last batch File is successfully uploaded.Please try again!");
                    sb.Append(Log("Annual file can only be uploaded when last batch File is successfully uploaded.Please try again!"));
                    CreateLogFile(sb, "AnnualSales_" + annualFileUpload.Id);
                    MarkUploadAsFailed(annualFileUpload);
                    return;
                }
            }
            if (annualFileUpload.SalesDataUpload != null && annualFileUpload.SalesDataUpload.StatusId != (long)SalesDataUploadStatus.Parsed)
            {
                annualFileUpload.StatusId = (long)SalesDataUploadStatus.Uploaded;
                _annualFileUploadRepository.Save(annualFileUpload);
                _unitOfWork.SaveChanges();
                return;
            }

            annualFileUpload.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
            _annualFileUploadRepository.Save(annualFileUpload);

            _unitOfWork.SaveChanges();

            _logService.Info("Starting data save for annual file # ," + annualFileUpload.Id);

            try
            {
                var filePath = MediaLocationHelper.FilePath(annualFileUpload.File.RelativeLocation, annualFileUpload.File.Name).ToFullPath();
                data = ExcelFileParser.ReadExcel(filePath);

                var salesDataFileParser = ApplicationManager.DependencyInjection.Resolve<ISalesDataFileParser>();
                salesDataFileParser.PrepareHeaderIndex(data);
                string message;
                if (!salesDataFileParser.CheckForValidHeader(data, out message))
                {
                    sb.Append(Log("Please upload correct File! " + message));
                    CreateLogFile(sb, "AnnualSales_" + annualFileUpload.Id);
                    MarkUploadAsFailed(annualFileUpload);
                    return;
                }

                collection = salesDataFileParser.PrepareDomainFromDataTable(data);
                if (collection.Count <= 0)
                    sb.Append(Log(message));

                var invalidRecords = 0;
                var invalidRecordsCustomers = 0;

                if (annualFileUpload.IsAuditAddressParsing.GetValueOrDefault())
                {
                    invalidRecords = GetNumberOfRecordsFailingAddressPhoneValidation(collection, sb, invalidRecords);
                }

                if (_setting.IsAddressAuditEnabled && annualFileUpload.IsAuditAddressParsing.GetValueOrDefault())
                {
                    collection = collection.OrderBy(x1 => x1.Date).ToList();
                    invalidRecordsCustomers = GetNumberOfRecordsCustomersAddressPhoneValidation(collection, sb, invalidRecords, annualFileUpload.FranchiseeId, annualFileUpload.Id);
                }
                annualFileUpload = _annualFileUploadRepository.Get(annualFileUpload.Id);
                if (invalidRecords > 0)
                {
                    CreateLogFile(sb, "AnnualSales_" + annualFileUpload.Id);
                    //Update Sales data upload
                    //MarkUploadAsFailed(annualFileUpload);
                }
            }

            catch (Exception ex)
            {
                sb.Append(Log("Some issue occured in File Parsing. Please check the file."));
                LogException(sb, ex);

                CreateLogFile(sb, "AnnualSales_" + annualFileUpload.Id);

                //Update Sales data upload
                MarkUploadAsFailed(annualFileUpload);
                return;
            }

            // Check to implement if date range doesn't fall in between the selected period
            annualFileUpload = _annualFileUploadRepository.Get(annualFileUpload.Id);
            var invalidRecordCount = 0;
            foreach (var item in collection)
            {
                _logService.Info("Starting annual file parsing for QbInvoice:" + item.QbIdentifier);
                if (item.Date < annualFileUpload.PeriodStartDate || item.Date > annualFileUpload.PeriodEndDate)
                {
                    sb.Append(Log(string.Format("Please upload the correct file. Found Customer {0} , that has InvoiceDate {1} outside the period selected.", item.Customer.Name, item.Date.ToShortDateString())));
                    invalidRecordCount++;
                    break;
                }
            }

            if (!annualFileUpload.IsAuditAddressParsing.GetValueOrDefault())
            {
                sb.Append(Log(string.Format("Annual Address Updation is Disabled for Reparsing Of Annual Sales File.")));
            }
            if (invalidRecordCount > 0)
            {
                CreateLogFile(sb, "AnnualSales_" + annualFileUpload.Id);

                //Update Sales data upload
                MarkUploadAsFailed(annualFileUpload);
                return;
            }

            var parsedRecords = 0;
            var failedRecords = 0;
            var mismatchedRecords = 0;
            decimal totalAmount = 0;
            decimal paidAmount = 0;

            int x = 0;

            foreach (var record in collection)
            {
                try
                {

                    var franchiseeSalesDomainList = _franchiseeSalesRepository.Table.Where(x1 => x1.FranchiseeId == franchiseeId
                                               && x1.SalesDataUpload.PeriodStartDate >= startDate && x1.SalesDataUpload.PeriodEndDate <= endDate
                                                  && (x1.Customer.Name == record.Customer.Name || x1.Customer.ContactPerson == record.Customer.ContactPerson)).ToList();
                    _unitOfWork.StartTransaction();

                    var stats = PrepareInvoiceModel(record, annualFileUpload, collection, franchiseeSalesDomainList, out weeklyRoyality, out anuallyRoyality);

                    totalAmount += stats.TotalAmount;
                    paidAmount += stats.PaidAmount;
                    if (stats.IsMismatch) mismatchedRecords++;

                    sb.Append(stats.Logs);
                    parsedRecords++;

                    _unitOfWork.SaveChanges();

                }
                catch (Exception ex)
                {
                    failedRecords++;
                    sb.Append(Log("Error in QbInvoiceNumber " + record.QbIdentifier));

                    _unitOfWork.Rollback();
                    LogException(sb, ex);
                }
                finally
                {
                    x++;
                    _unitOfWork.ResetContext();
                }
            }
            //Refresh SalesData upload after context is Reset
            annualFileUpload = _annualFileUploadRepository.Get(annualFileUpload.Id);

            weeklyRoyality = GetWeeklyRoyality(annualFileUpload.FranchiseeId, annualFileUpload);
            anuallyRoyality = GetAnnuallyRoyality(annualFileUpload.Franchisee, annualFileUpload, collection);

            //add all stats to sales data uipload
            //use file stream to save it in media location
            //save it in file table
            UpdateSalesDataStatus(annualFileUpload, totalAmount, paidAmount, failedRecords, parsedRecords, mismatchedRecords);
            if (mismatchedRecords <= 0 && annualFileUpload.StatusId == (long)SalesDataUploadStatus.Parsed)
            {
                sb.Append(Log("File Is successfully parsed, and has no mismatched records!"));
            }
            CreateLogFile(sb, "AnnualSales_" + annualFileUpload.Id);

            var franchiseeSalesList = _franchiseeSalesRepository.Table.Where(fs => fs.Invoice != null
                                       && fs.FranchiseeId == annualFileUpload.FranchiseeId
                                       && fs.Invoice.GeneratedOn >= annualFileUpload.PeriodStartDate
                                       && fs.Invoice.GeneratedOn <= annualFileUpload.PeriodEndDate);

            var auditQBRecords = collection.Select(c => c.QbIdentifier);
            var mismatchedInvoices = franchiseeSalesList.Where(c => !auditQBRecords.Contains(c.QbInvoiceNumber)).ToList();

            if (mismatchedInvoices.Any())
            {

                var annulalFile = CreateSystemAuditRecord(mismatchedInvoices, annualFileUpload, sb, collection);
            }
            annualFileUpload = _annualFileUploadRepository.Get(annualFileUpload.Id);
            //Update Sales data upload
            annualFileUpload.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            annualFileUpload.WeeklyRoyality = weeklyRoyality;
            annualFileUpload.AnnualRoyality = anuallyRoyality;
            SaveSalesDataUpload(annualFileUpload);
        }
        class MonthSalesData
        {
            public int Month { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal Sales { get; set; }
            public decimal YTDSales { get; set; }
            public decimal MTDSales { get; set; }
            public bool IsEndOfMonth { get; set; }
            public decimal RYTSales { get; set; }
            public bool IsMinRoyalityReached { get; set; }
            public decimal MTDRouyality { get; set; }
            public decimal MTDPaid { get; set; }
        }
        class YearSalesData
        {
            public int Year { get; set; }
            //public decimal MonthlySales { get; set; }
            public ICollection<MonthSalesData> MonthWiseSalesCollection { get; set; }
            public YearSalesData()
            {
                MonthWiseSalesCollection = new List<MonthSalesData>();
            }
        }
        class RoyaltyCharge
        {
            public decimal Amount;
            public decimal Percentage;
            public decimal Royalty
            {
                get
                {
                    return Amount * Percentage / 100;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="sb"></param>
        /// <param name="invalidCustomerRecords"></param>
        /// <param name="franchiseeId"></param>
        /// <param name="annualSalesUploadId"></param>
        /// <returns></returns>
        private int GetNumberOfRecordsCustomersAddressPhoneValidation(IList<ParsedFileParentModel> collection, StringBuilder sb, int invalidCustomerRecords, long franchiseeId, long annualSalesUploadId)
        {
            long? classId = default(long?);
            foreach (var item in collection)
            {
                try
                {
                    var salesInfo = _franchiseeSalesRepository.Table.FirstOrDefault(x => x.QbInvoiceNumber == item.QbIdentifier && x.FranchiseeId == franchiseeId);
                    var invoiceAddress = salesInfo != null ? _invoiceAddressRepository.Table.Where(x => x.InvoiceId == salesInfo.InvoiceId).FirstOrDefault() : null;
                    if (salesInfo != null && invoiceAddress != null)
                    {
                        bool isValid = _customersRepository.Table.Any(x => x.Name == item.Customer.Name);
                        var customerinfo = _customersRepository.Table.Where(x => x.Name == item.Customer.Name).FirstOrDefault();
                        if (isValid)
                        {
                            var customerInfo = salesInfo.Customer;
                            if (customerinfo.Id != customerInfo.Id)
                            {
                                sb.Append(Log("Address Updation For Customer " + item.Customer.Name + " with QbInvoice Id " + item.QbIdentifier + " is not possible as CustomerName is Incorrect"));
                                continue;
                            }
                            if (item.Customer.Address != null && (item.Customer.Address.AddressLine1 == null || item.Customer.Address.AddressLine1 == ""))
                            {
                                sb.Append(Log("Address Updation For Customer " + item.Customer.Name + " with QbInvoice Id " + item.QbIdentifier + " is not possible as AddressLine1 is incomplete"));
                                continue;
                            }
                            if (item.Customer.Address != null && (item.Customer.Address.City == null || item.Customer.Address.CityId == default(long) && item.Customer.Address.City == ""))
                            {
                                sb.Append(Log("Address Updation For Customer " + item.Customer.Name + " with QbInvoice Id " + item.QbIdentifier + " is not possible as City is incomplete"));
                                continue;
                            }

                            if (item?.Customer?.Address != null && invoiceAddress != null)
                            {
                                // Check and get AddressLine1 values with null handling
                                string customerAddressLine1 = item.Customer.Address.AddressLine1 != null
                                                              ? item.Customer.Address.AddressLine1.Trim().ToUpper()
                                                              : string.Empty;

                                string invoiceAddressLine1 = invoiceAddress.AddressLine1 != null
                                                             ? invoiceAddress.AddressLine1.Trim().ToUpper()
                                                             : string.Empty;

                                // Check and get City values with null handling
                                string customerCity = item.Customer.Address.CityId != null && item.Customer.Address.City != null
                                                      ? item.Customer.Address.City.Trim().ToUpper()
                                                      : string.Empty;

                                string invoiceCity = invoiceAddress.City != null && invoiceAddress.City.Name != null
                                                     ? invoiceAddress.City.Name.Trim().ToUpper()
                                                     : (invoiceAddress.CityName != null
                                                        ? invoiceAddress.CityName.Trim().ToUpper()
                                                        : string.Empty);

                                // Compare AddressLine1 and City values
                                if (!customerAddressLine1.Equals(invoiceAddressLine1) || !customerCity.Equals(invoiceCity))
                                {
                                    try
                                    {
                                        ++invalidCustomerRecords;
                                        _unitOfWork.StartTransaction();
                                        var newCustomerInfoDispensary = _auditFactory.CreateViewModelAudit(item.Customer, salesInfo.Id, false, salesInfo.Customer.Address.Country.Name, annualSalesUploadId, item.MarketingClassId);
                                        newCustomerInfoDispensary.InvoiceId = (salesInfo != null && salesInfo.Invoice != null) ? salesInfo.Invoice.Id : default(long?);
                                        newCustomerInfoDispensary.invoiceDate = (salesInfo == null || salesInfo.Invoice == null) ? salesInfo.Invoice.GeneratedOn : item.Date;
                                        _auditAddressDiscrepanctRepository.Save(newCustomerInfoDispensary);
                                        _unitOfWork.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        _unitOfWork.Rollback();
                                        LogException(sb, ex);
                                    }
                                    finally
                                    {
                                        _unitOfWork.ResetContext();
                                    }
                                }
                            }
                            //if ((item.Customer.Address != null && invoiceAddress != null) && !((item.Customer.Address.AddressLine1).Trim().ToUpper().Equals((invoiceAddress.AddressLine1).Trim().ToUpper()) ||
                            //    !(item.Customer.Address.CityId != null ? item.Customer.Address.City : item.Customer.Address.City).Trim().ToUpper().Equals((invoiceAddress.City != null ? invoiceAddress.City.Name : invoiceAddress.CityName
                            //                ).Trim().ToUpper())))
                            //{
                                
                            //}
                            else
                            {
                                bool isEmailEquals = isEmailEqual(item, customerInfo);
                                bool isPhoneEquals = isPhoneEqual(item, customerInfo);

                                if (!isAddressEqual(item, customerInfo) || (!isEmailEquals) || (!isPhoneEquals))
                                {
                                    try
                                    {
                                        var newInvoiceAddressInfo = new InvoiceAddress();
                                        _unitOfWork.StartTransaction();
                                        //var oldAddress = _addressRepository.Get(item.Customer.Address.Id);
                                        classId = customerInfo.Address != null ? customerInfo.Address.TypeId : default(long);
                                        var customerAddressInfo = _auditFactory.CreateViewModel(salesInfo.Customer, item.Customer.Address != null ? item.Customer.Address.Id : default(long), true, invoiceAddress != null ? invoiceAddress.CountryId : default(long), salesInfo.Id, classId, item.MarketingClassId);
                                        customerAddressInfo.AnnualSalesDataUploadId = annualSalesUploadId;
                                        customerAddressInfo.InvoiceId = (salesInfo != null && salesInfo.Invoice != null) ? invoiceAddress.Invoice.Id : default(long?);
                                        customerAddressInfo.invoiceDate = (salesInfo == null || salesInfo.Invoice == null) ? invoiceAddress.Invoice.GeneratedOn : item.Date;
                                        _addressLogRepository.Save(customerAddressInfo);
                                        var newCustomerInfo = _auditFactory.CreateViewModel(item.Customer, item.Customer.Address != null ? item.Customer.Address.Id : default(long), true, salesInfo.Customer, annualSalesUploadId);
                                        newCustomerInfo.IsNew = newCustomerInfo.Id != 0 ? false : true;
                                        _addressRepository.Save(newCustomerInfo);
                                        newInvoiceAddressInfo = _auditFactory.CreateModel(item.Customer, invoiceAddress.InvoiceId, (invoiceAddress != null ? (long?)invoiceAddress.Id : null));
                                        if (customerAddressInfo != null && (customerAddressInfo.CountryId != null || customerAddressInfo.CountryId != default(long)))
                                        {
                                            newInvoiceAddressInfo.CountryId = customerAddressInfo.CountryId;
                                        }
                                        if (salesInfo.Customer != null && salesInfo.Customer.Id != 0 && newCustomerInfo.IsNew)
                                        {
                                            var customer = _customersRepository.Get(salesInfo.Customer.Id);
                                            customer.AddressId = newCustomerInfo.Id;
                                            customer.IsNew = newCustomerInfo.Id != 0 && newCustomerInfo.Id > 0 ? false : true;
                                            _customersRepository.Save(customer);
                                        }
                                        _invoiceAddressRepository.Save(newInvoiceAddressInfo);
                                        if (!isPhoneEquals)
                                        {
                                            salesInfo.Customer.Phone = item.Customer.Phone;
                                            _customersRepository.Save(salesInfo.Customer);
                                        }
                                        if (!isEmailEquals)
                                        {
                                            var customerEmail = _customerEmailRepository.Table.Where(x => x.CustomerId == salesInfo.CustomerId).FirstOrDefault();
                                            if (customerEmail != null)
                                            {
                                                customerEmail.Email = item.Customer.CustomerEmails.Select(x => x.Email).FirstOrDefault();
                                                _customerEmailRepository.Save(customerEmail);
                                            }
                                            else
                                            {
                                                var email = item.Customer.CustomerEmails.Select(x => x.Email).FirstOrDefault();
                                                var newcustomerInfo = _auditFactory.CreateDomain(salesInfo.Customer, email);
                                                _customerEmailRepository.Save(newcustomerInfo);
                                            }
                                        }
                                        _unitOfWork.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        _unitOfWork.Rollback();
                                        LogException(sb, ex);
                                    }
                                    finally
                                    {
                                        _unitOfWork.ResetContext();
                                    }
                                }
                            }
                        }
                        else
                        {
                            sb.Append(Log("Address Updation For Customer " + item.Customer.Name + " with QbInvoice Id " + item.QbIdentifier + " is not possible as Customer  is New"));
                            continue;
                        }

                    }
                }
                catch (Exception ex)
                {
                    sb.Append(Log("Some Issue In Address and Phone Validation Function 2" + ex));
                }
            }
            return invalidCustomerRecords;
        }

        private long CreateAuditCustomer(ParsedFileParentModel record)
        {
            //bool isValidAddress = checkForAddress(record.Customer.Address);
            var address = new Address();
            var newCustomerAddressInfo = _auditFactory.CreateViewModel(record.Customer);
            _annualAddressRepository.Save(newCustomerAddressInfo);
            var newCustomerInfo = _auditFactory.CreateModel(record.Customer);
            newCustomerInfo.AuditAddressId = newCustomerAddressInfo.Id;
            _annualCustomerRepository.Save(newCustomerInfo);
            _unitOfWork.SaveChanges();
            return newCustomerInfo.Id;
        }

        public bool isPhoneEqual(ParsedFileParentModel item, Customer customerInfo)
        {
            if (customerInfo.Phone == item.Customer.Phone)
            {
                return true;
            }

            return false;
        }
        public bool isEmailEqual(ParsedFileParentModel item, Customer customerInfo)
        {
            int matchedCount = 0;

            if (customerInfo.CustomerEmails.Count() == 0 && item.Customer.CustomerEmails.Count() == 0)
            {
                return true;
            }

            if (customerInfo.CustomerEmails.Count > 0)
            {
                var emails = customerInfo.CustomerEmails.ToList();

                foreach (var email in emails)
                {
                    if ((item.Customer.CustomerEmails.Select(y => (y.Email))).Contains(email.Email))
                    {
                        ++matchedCount;
                    }
                }
            }

            if (matchedCount == 0)
            {
                return false;
            }
            return true;
        }
        public bool isAddressEqual(ParsedFileParentModel item, Customer customerInfo)
        {
            if ((item.Customer != null && item.Customer.Address != null && customerInfo.Address != null && customerInfo != null) && (item.Customer.Address.AddressLine2 != null ? item.Customer.Address.AddressLine2 : "").Trim().ToUpper().Equals((customerInfo.Address.AddressLine2 != null ? customerInfo.Address.AddressLine2 : "").Trim().ToUpper())
            && (item.Customer.Address.ZipCode != null ? item.Customer.Address.ZipCode : "").Trim().Equals((customerInfo.Address.ZipCode != null ? customerInfo.Address.ZipCode : customerInfo.Address.Zip != null ? customerInfo.Address.Zip.Code : "").Trim())
            && ((item.Customer.Address.State != null ? item.Customer.Address.State : "").Trim().ToUpper().Equals((customerInfo.Address.StateName != null ? customerInfo.Address.StateName : customerInfo.Address.State != null ? customerInfo.Address.State.ShortName : "").Trim().ToUpper())
            || (item.Customer.Address.State != null ? item.Customer.Address.State : "").Trim().ToUpper().Equals((customerInfo.Address.StateName != null ? customerInfo.Address.StateName : customerInfo.Address.State != null ? customerInfo.Address.State.Name : "").Trim().ToUpper())))
            {
                return true;
            }
            return false;
        }
        private static int GetNumberOfRecordsFailingAddressPhoneValidation(IList<ParsedFileParentModel> collection, StringBuilder sb, int invalidRecords)
        {
            foreach (var item in collection)
            {
                try
                {
                    if (item.Customer != null)
                    {
                        var address = item.Customer.Address;
                        if (address != null)
                        {
                            if (string.IsNullOrWhiteSpace(address.AddressLine1))
                            {
                                sb.Append(Log("Found Customer " + item.Customer.Name + " with incomplete AddressLine 1."));
                                invalidRecords++;
                            }

                            else if ((string.IsNullOrWhiteSpace(address.AddressLine2)))
                            {
                                sb.Append(Log("Found Customer " + item.Customer.Name + " with incomplete AddressLine 2."));
                                invalidRecords++;
                            }

                            else if (((address.StateId == null || address.StateId <= 0) && string.IsNullOrWhiteSpace(address.State)))
                            {
                                sb.Append(Log("Found Customer " + item.Customer.Name + " with incomplete State."));
                                invalidRecords++;
                            }

                            else if (((address.CityId == null || address.CityId <= 0) && string.IsNullOrWhiteSpace(address.City)))
                            {
                                sb.Append(Log("Found Customer " + item.Customer.Name + " with incomplete City."));
                                invalidRecords++;
                            }

                            else if (((address.ZipId == null || address.ZipId <= 0) && string.IsNullOrEmpty(address.ZipCode)))
                            {
                                sb.Append(Log("Found Customer " + item.Customer.Name + " with incomplete Zip Code"));
                                invalidRecords++;
                            }

                            else if (string.IsNullOrWhiteSpace(item.Customer.Phone))
                            {
                                sb.Append(Log("Found Customer " + item.Customer.Name + " with incomplete Phone Number."));
                                invalidRecords++;
                            }
                            else if ((item.Customer.CustomerEmails.Count() == 0))
                            {
                                sb.Append(Log("Found Customer " + item.Customer.Name + " with incomplete EmailId."));
                                invalidRecords++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    sb.Append(Log("Some Issue In Address and Phone Validation" + ex));
                }
            }
            return invalidRecords;
        }

        private AnnualSalesDataUpload CreateSystemAuditRecord(IEnumerable<FranchiseeSales> mismatchedInvoices, AnnualSalesDataUpload annualFileUpload, StringBuilder sb, IList<ParsedFileParentModel> collection)
        {

            var parsedRecords = annualFileUpload.NoOfParsedRecords != null ? annualFileUpload.NoOfParsedRecords : 0;
            var failedRecords = annualFileUpload.NoOfFailedRecords != null ? annualFileUpload.NoOfFailedRecords : 0;
            var mismatchedRecords = annualFileUpload.NoOfMismatchedRecords != null ? annualFileUpload.NoOfMismatchedRecords : 0;
            decimal totalAmount = annualFileUpload.TotalAmount > 0 ? annualFileUpload.TotalAmount.Value : 0;
            decimal paidAmount = annualFileUpload.PaidAmount > 0 ? annualFileUpload.PaidAmount.Value : 0;
            string qbInvoice = default(string);
            long invoiceId = default(long);

            foreach (var item in mismatchedInvoices)
            {
                try
                {
                    bool isChanged = isInvoiceChanged(null, item, collection, out qbInvoice, out invoiceId);
                    if (isChanged)
                    {
                        continue;
                    }
                    _unitOfWork.StartTransaction();
                    var systemAuditRecord = _auditFactory.CreateDomain(item, annualFileUpload);
                    var inActiveServices = _franchiseeServiceRepository.Table.Where(x => x.FranchiseeId == annualFileUpload.FranchiseeId && !x.CalculateRoyalty).Select(x => x.ServiceTypeId).ToList();
                    if (inActiveServices.Contains(item.Invoice.InvoiceItems.Select(x => x.ItemTypeId).FirstOrDefault()))
                    {
                        systemAuditRecord.AnnualReportTypeId = (long)AuditReportType.Type1H;
                    }
                    else
                    {
                        decimal weeklySalesAmount = item.Invoice.InvoiceItems.Sum(ii => ii.Amount);
                        decimal weeklPaidyAmount = item.Invoice.InvoicePayments.Sum(p => p.Payment.Amount);

                        if (weeklySalesAmount < weeklPaidyAmount)
                            systemAuditRecord.AnnualReportTypeId = (long)AuditReportType.Type5B;
                        else if (weeklySalesAmount > weeklPaidyAmount)
                            systemAuditRecord.AnnualReportTypeId = (long)AuditReportType.Type5A;
                        else if (weeklySalesAmount == weeklPaidyAmount)
                            systemAuditRecord.AnnualReportTypeId = (long)AuditReportType.Type5;
                    }
                    _systemAuditRecordRepository.Save(systemAuditRecord);

                    totalAmount += item.Invoice.InvoiceItems.Sum(ii => ii.Amount);
                    paidAmount += item.Invoice.InvoicePayments.Sum(p => p.Payment.Amount);

                    parsedRecords++;
                    mismatchedRecords++;

                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    failedRecords++;
                    _unitOfWork.Rollback();
                    LogException(sb, ex);
                }
            }
            UpdateSalesDataStatus(annualFileUpload, totalAmount, paidAmount, failedRecords.Value, parsedRecords.Value, mismatchedRecords.Value);
            return annualFileUpload;
        }

        private SaveModelStats PrepareInvoiceModel(ParsedFileParentModel record, AnnualSalesDataUpload annualFileUpload,
            IList<ParsedFileParentModel> records, List<FranchiseeSales> franchiseeSalesList, out decimal weeklyRoyality, out decimal anuallyRoyality)
        {
            var stats = new SaveModelStats();
            string qbInvoices = default(string);
            weeklyRoyality = default(decimal);
            anuallyRoyality = default(decimal);
            long franchiseeId = annualFileUpload.FranchiseeId,
               uploadId = annualFileUpload.Id;
            long? salesDataUploadId = annualFileUpload.SalesDataUploadId != null ? annualFileUpload.SalesDataUploadId.Value : (long?)null;
            var currencyExchangeRate = annualFileUpload.CurrencyExchangeRate;
            var franchiseeSales = new FranchiseeSales();
            int n;
            bool isNumeric = int.TryParse(record.QbIdentifier, out n);
            if (isNumeric)
                franchiseeSales = GetRecord(record.QbIdentifier, franchiseeId, record.Date);
            else
                franchiseeSales = GetRecordWithoutPrefix(record.QbIdentifier, franchiseeId, record.Date);

            string qbInvoice = default(string);
            long invoiceId = default(long);

            record.Invoice.InvoiceItems.Select(x => x.CurrencyExchangeRateId = currencyExchangeRate.Id).ToList();
            record.Invoice.InvoiceItems.Select(x => x.Rate = x.Rate.ToDefaultCurrency(currencyExchangeRate.Rate)).ToList();
            record.Invoice.InvoiceItems.Select(x => x.Amount = x.Amount.ToDefaultCurrency(currencyExchangeRate.Rate)).ToList();

            record.Invoice.Payments.Select(x => x.CurrencyExchangeRateId = currencyExchangeRate.Id).ToList();
            record.Invoice.Payments.Select(x => x.Amount = x.Amount.ToDefaultCurrency(currencyExchangeRate.Rate)).ToList();
            record.Invoice.AnnualSalesDataUploadId = annualFileUpload.Id;
            //stats.Logs += Log("Starting data save for Customer " + record.Customer.Name + " and QB-Invoice #" + record.QbIdentifier + "and Franchisee" + franchiseeId);


            decimal totalAmount = 0, paidAmount = 0;

            var invoice = CreateNewInvoice(record.Invoice, stats);
            totalAmount = record.Invoice.InvoiceItems.Sum(ii => ii.Amount);
            paidAmount = record.Invoice.Payments.Sum(p => p.Amount);

            if (franchiseeSales == null || franchiseeSales.Invoice == null)
            {
                bool isChanged = isInvoiceChangedForInvoice(record, null, null, franchiseeSalesList, out qbInvoice, out invoiceId);
                bool isTypeBtype = isTypeB(record.QbIdentifier, annualFileUpload.Id);
                if (isChanged)
                {
                    stats.Logs += Log(string.Format("QB Invoice {0}, Is Mismatch with Weekly QB Invoice.{1} ", record.QbIdentifier, qbInvoice));
                }
                else
                {
                    stats.Logs += Log(string.Format("New QB Invoice {0}, Unable to find it in existing records. ", record.QbIdentifier));
                }

                stats.IsMismatch = true;
                var auditInvoice = CreateAuditRecord(record, franchiseeSales, null, annualFileUpload.FranchiseeId, null, annualFileUpload.Id);
            }

            else
            {
                var isSame = CompareData(record, franchiseeSales);
                long reportTypeId = AnnualReportType(record, franchiseeSales, records, out qbInvoices);

                if (reportTypeId == 0)
                {
                    reportTypeId = (long)AuditReportType.Type17;
                }
                if (reportTypeId != 22)
                {

                    var auditInvoices = CreateAuditRecord(record, franchiseeSales, reportTypeId, franchiseeSales.FranchiseeId, qbInvoices, annualFileUpload.Id);
                    stats.Logs += Log(string.Format("Invoice Descrepency for QB invoice {0}", record.QbIdentifier));
                }

                //if (reportTypeId == 22)
                //{

                //    var auditInvoices = CreateAuditRecord(record, franchiseeSales, reportTypeId, franchiseeSales.FranchiseeId, qbInvoices, annualFileUpload.Id);
                //    stats.Logs += Log(string.Format("Invoice Parsing for QB invoice {0}", record.QbIdentifier));
                //}

                //var auditInvoice = CreateAuditRecord(record, franchiseeSales, reportTypeId, franchiseeSales.FranchiseeId, qbInvoices, annualFileUpload.Id);
                if (!isSame)
                {
                    stats.IsMismatch = true;
                    stats.Logs += Log(string.Format("Data mismatch found for QB Invoice {0}", record.QbIdentifier));
                }
            }

            stats.QBIdentifier = record.QbIdentifier;
            stats.TotalAmount = totalAmount;
            stats.PaidAmount = paidAmount;
            return stats;
        }

        private decimal GetWeeklyRoyality(long franchiseeId, AnnualSalesDataUpload annualSales)
        {
            int startYear = annualSales.PeriodStartDate.Year;
            var startDayOfYear = new DateTime(startYear, 1, 1);
            var endDayOfYear = new DateTime(startYear, 12, 31);
            decimal royalityAmount = default(decimal);

            var weeklySalesDataIds = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.PeriodStartDate >= startDayOfYear && x.PeriodEndDate <= endDayOfYear).Select(x => x.Id).ToList();
            var franchiseeInvoices = _franchiseeInvoiceRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.SalesDataUploadId.HasValue && weeklySalesDataIds.Contains(x.SalesDataUploadId.Value)).Select(x => x.InvoiceId).ToList();

            var royalityInvoices = _invoiceItemsRepository.Table.Where(x => franchiseeInvoices.Contains(x.InvoiceId) && x.ItemTypeId == (long)LookupTypes.RoyaltyFee).ToList();
            if (royalityInvoices.Count() > 1)
            {
                royalityAmount = royalityInvoices.Sum(x => x.Amount);
            }

            //if (invoicesIdList.Count() > 1)
            //{
            //    var invoiceItems = _invoiceItemsRepository.Table.Where(x => invoicesIdList.Contains(x.InvoiceId) && (x.ItemTypeId == (long)LookupTypes.RoyaltyFee || x.ItemTypeId == (long)LookupTypes.LateFee || x.ItemTypeId == (long)LookupTypes.InterestIncome)).ToList();
            //    foreach (var invoiceItem in invoiceItems)
            //    {
            //        royalityAmount += invoiceItem.Amount;
            //    }
            //}

            return royalityAmount;
        }
        private decimal GetAnnuallyRoyality(Franchisee franchisee, AnnualSalesDataUpload annualSales, IList<ParsedFileParentModel> records)
        {
            var royalityAmount = default(decimal);
            royalityAmount = GetAnnuallyRoyality(franchisee, annualSales.PeriodStartDate, annualSales.PeriodEndDate, annualSales, records);
            return royalityAmount;
        }

        private decimal GetAnnuallyRoyality(Franchisee franchisee, DateTime periodStartDate, DateTime periodEndDate, AnnualSalesDataUpload annualData, IList<ParsedFileParentModel> records)
        {
            var invoiceItems = new List<InvoiceItemEditModel>();
            decimal royalityAmount = default(decimal);
            decimal defaultAmount = default(decimal);


            var royalitySlabs = franchisee.FeeProfile.RoyaltyFeeSlabs;
            var feeProfile = franchisee.FeeProfile;
            if (!feeProfile.SalesBasedRoyalty)
            {
                if (feeProfile.FixedAmount == null)
                {
                    defaultAmount = Convert.ToDecimal(ApplicationManager.Settings.DefaultRoyaltyAmount);
                }
                else
                {
                    defaultAmount = feeProfile.FixedAmount.GetValueOrDefault();
                }
            }
            else
            {
                var periodStructure = PreparePeriodStructure(franchisee, startDate, endDate, records);
                foreach (var data in periodStructure)
                {
                    royalityAmount = data.MonthWiseSalesCollection.Sum(x => x.RYTSales);
                }
            }
            if (defaultAmount != default(decimal))
            {
                royalityAmount = defaultAmount * 12;
            }


            return royalityAmount;
        }

        private IList<InvoiceItemEditModel> GetInvoiceItemsByFeeProfile(Franchisee franchisee, decimal? paidAmount, FeeProfile feeProfile, decimal yearToDaySales,
          DateTime startDate, DateTime endDate)
        {
            var invoiceItems = new List<InvoiceItemEditModel>();
            var invoiceItem = new InvoiceItemEditModel();
            if (feeProfile == null)
                return invoiceItems;

            if (!feeProfile.SalesBasedRoyalty)
            {
                var fixedAmountInvoiceItem = new InvoiceItemEditModel();
                fixedAmountInvoiceItem.ItemTypeId = (long)InvoiceItemType.RoyaltyFee;

                if (feeProfile.FixedAmount == null)
                {
                    var defaultAmount = ApplicationManager.Settings.DefaultRoyaltyAmount;
                    fixedAmountInvoiceItem.Amount = decimal.Parse(defaultAmount);
                }
                else
                {
                    fixedAmountInvoiceItem.Amount = feeProfile.FixedAmount.Value;
                }

                fixedAmountInvoiceItem.Description = string.Format("Fixed Amount for ${0} will be charged. Period- {1:MM/dd/yyyy} to {2:MM/dd/yyyy}",
                    fixedAmountInvoiceItem.Amount, startDate, endDate);

                invoiceItems.Add(fixedAmountInvoiceItem);

                return invoiceItems;
            }

            if (feeProfile.SalesBasedRoyalty == true)
            {
                var minRoyalty = feeProfile.MinimumRoyaltyPerMonth;
                var adFundPercentage = feeProfile.AdFundPercentage;

                var royaltyCharges = GetSlabPercentageForPaidAmount(yearToDaySales, paidAmount, feeProfile.RoyaltyFeeSlabs);
                CreateRoyaltyInvoiceItem(invoiceItems, minRoyalty, royaltyCharges, startDate, endDate, franchisee.Id);
            }
            return invoiceItems;
        }
        private void CreateRoyaltyInvoiceItem(List<InvoiceItemEditModel> invoiceItems, decimal minRoyalty, IList<RoyaltyCharge> royaltyCharges,
           DateTime startDate, DateTime endDate, long franchiseeId)
        {
            InvoiceItemEditModel royaltyFeeInvoiceItem;
            if (endDate.AddDays(1).Month != endDate.Month)
            {
                //Charge Min Royalty
                var totalPayment = royaltyCharges.Select(x => x.Royalty).Sum();
                decimal monthPay = 0;
                if (startDate.Day != 1)
                {
                    monthPay = _invoiceItemService.GetRoyaltyGeneratedForGivenMonthYear(franchiseeId, startDate.Month, startDate.Year);
                }

                var totalMonthPayout = totalPayment + monthPay;
                var diffMinRoyalty = minRoyalty - totalMonthPayout;

                if (diffMinRoyalty > 0)
                {
                    royaltyFeeInvoiceItem = CreateRoyaltyInvoiceItem(diffMinRoyalty,
                        string.Format("Charged Minimum Royalty, as Royalty for the month {0}/{1} on Sales is ${2}.", startDate.Month, startDate.Year, totalMonthPayout),
                        null, startDate, endDate);

                    invoiceItems.Add(royaltyFeeInvoiceItem);
                }

            }

        }
        private static InvoiceItemEditModel CreateRoyaltyInvoiceItem(decimal amount, string description, decimal? percentage, DateTime startDate, DateTime endDate, decimal? salesAmount = null)
        {
            var royaltyFeeInvoiceItem = new RoyaltyInvoiceItemEditModel();
            royaltyFeeInvoiceItem.ItemTypeId = (long)InvoiceItemType.RoyaltyFee;
            royaltyFeeInvoiceItem.Amount = amount;
            royaltyFeeInvoiceItem.Quantity = 1;
            royaltyFeeInvoiceItem.Rate = amount;
            royaltyFeeInvoiceItem.Description = description;
            royaltyFeeInvoiceItem.Percentage = percentage;
            royaltyFeeInvoiceItem.StartDate = startDate;
            royaltyFeeInvoiceItem.EndDate = endDate;
            royaltyFeeInvoiceItem.SalesAmount = salesAmount ?? 0;
            return royaltyFeeInvoiceItem;
        }
        private List<RoyaltyCharge> GetSlabPercentageForPaidAmount(decimal yearToDaySales, decimal? paidAmount, IEnumerable<RoyaltyFeeSlabs> slabs)
        {
            var royaltyChargeCollection = new List<RoyaltyCharge>();
            if (paidAmount == null) return royaltyChargeCollection;

            var ytdSalesWithCurrentPaidAmount = yearToDaySales + paidAmount;

            foreach (var record in slabs.OrderBy(x => x.MinValue).ToArray())
            {
                if (record.MaxValue == null || ytdSalesWithCurrentPaidAmount <= record.MaxValue)
                {
                    var amountToCharge = ytdSalesWithCurrentPaidAmount - record.MinValue;
                    amountToCharge = amountToCharge > paidAmount ? paidAmount : amountToCharge;

                    royaltyChargeCollection.Add(new RoyaltyCharge { Amount = amountToCharge.Value, Percentage = record.ChargePercentage });
                    break;
                }

                if (yearToDaySales < record.MaxValue)
                {
                    var amountToCharge = record.MaxValue.Value - yearToDaySales;
                    royaltyChargeCollection.Add(new RoyaltyCharge { Amount = amountToCharge, Percentage = record.ChargePercentage });
                }
            }

            return royaltyChargeCollection;
        }
        private ICollection<YearSalesData> PreparePeriodStructure(Franchisee franchisee, DateTime startDate, DateTime endDate, IList<ParsedFileParentModel> records)
        {
            var collection = new List<YearSalesData>();
            var collection2 = new List<MonthSalesData>();
            bool isMinRoyalityReached = default(bool);
            decimal monthlyPaid = default(decimal);
            var model = new YearSalesData
            {
                Year = endDate.Year,
            };
            decimal monthlySales = default(decimal);
            decimal montlyRoyality = default(decimal);
            for (int monthIndex = startDate.Month; monthIndex <= endDate.Month; monthIndex++)
            {
                var startDateMonth = new DateTime(startDate.Year, monthIndex, 1);
                int lastDay = DateTime.DaysInMonth(startDate.Year, monthIndex);
                var endDateMonth = new DateTime(startDate.Year, monthIndex, lastDay);

                var monthlyRoyalitySales = PaymentSalesBetweenPeriod(franchisee, startDateMonth, endDateMonth, records, out monthlySales, out isMinRoyalityReached, out montlyRoyality, out monthlyPaid);

                var model2 = new MonthSalesData
                {
                    Month = monthIndex,
                    Sales = monthlySales,
                    IsEndOfMonth = true,
                    EndDate = endDateMonth,
                    RYTSales = monthlyRoyalitySales,
                    StartDate = startDateMonth,
                    IsMinRoyalityReached = isMinRoyalityReached,
                    MTDRouyality = montlyRoyality,
                    MTDPaid = monthlyPaid
                };

                collection2.Add(model2);
                model.MonthWiseSalesCollection = collection2;

            }
            foreach (var monthlySalesData in collection2)
            {
                var annuallyRoyality = createModel(monthlySalesData, franchisee);
                _annualRoyalityRepository.Save(annuallyRoyality);
            }

            collection.Add(model);
            return collection;
        }

        private AnnualRoyality createModel(MonthSalesData monthlySalesData, Franchisee franchisee)
        {
            var annuallyRoyality = new AnnualRoyality
            {
                FranchiseeId = franchisee.Id,
                Date = monthlySalesData.StartDate,
                Royality = monthlySalesData.RYTSales,
                Sales = monthlySalesData.Sales,
                isMinRoyalityReached = monthlySalesData.IsMinRoyalityReached,
                IsNew = true,
                MonthlyRoyality = monthlySalesData.MTDRouyality,
                Payment = monthlySalesData.MTDPaid
            };
            return annuallyRoyality;
        }

        private decimal PaymentSalesBetweenPeriod(Franchisee franchisee, DateTime fromDate, DateTime toDate, IList<ParsedFileParentModel> records, out decimal monthlySales, out bool isMinRoyalityReached, out decimal monthlyRoyality, out decimal monthlyPaid)
        {
            var yearlytoCurrentDayPayedAmount = default(decimal);
            var monthlySalesSum = default(decimal);
            isMinRoyalityReached = default(bool);
            monthlyRoyality = default(decimal);
            monthlySales = default(decimal);
            var monthlypayedSum1 = default(decimal);
            var servicesOffered = franchisee.FranchiseeServices.Where(fs => fs.CalculateRoyalty == true && fs.IsActive == true).Select(fs => fs.ServiceType.Id).ToList();
            var minRoyalityPerMonth = franchisee.FeeProfile.MinimumRoyaltyPerMonth;
            var royalityAmount = default(decimal);
            var royalitySlabs = franchisee.FeeProfile.RoyaltyFeeSlabs;
            var inActiveServices = _franchiseeServiceRepository.Table.Where(x => x.FranchiseeId == franchisee.Id && !x.CalculateRoyalty).Select(x => x.ServiceTypeId).ToList();
            records = records.Where(x => x.Date >= fromDate && x.Date <= toDate && !inActiveServices.Contains(x.ServiceTypeId)).ToList();

            var invoiceitems = records.Select(x => x.Invoice.InvoiceItems);

            var invoiceitems2 = records.Select(x => x.Invoice.Payments);

            foreach (var serviceOffered in servicesOffered)
            {
                foreach (var invoiceItem in invoiceitems2)
                {
                    monthlypayedSum1 += invoiceItem.Where(x => (servicesOffered != null) && x.PaymentItems.Select(x1 => x1.ItemId).Contains(serviceOffered)).Sum(x => x.Amount);
                }
            }

            foreach (var invoiceItem in invoiceitems)
            {
                monthlySalesSum += invoiceItem.Where(x => (servicesOffered != null) && servicesOffered.Contains(x.ItemId.GetValueOrDefault())).Sum(x => x.Amount);
            }
            monthlySales = monthlySalesSum;
            monthlyPaid = monthlypayedSum1;
            yearlytoCurrentDayPayedAmount = totallyYearlySales.GetValueOrDefault() + monthlypayedSum1;

            foreach (var royalitySlab in royalitySlabs.OrderBy(x => x.MinValue).ToArray())
            {
                if (royalitySlab.MaxValue == null || yearlytoCurrentDayPayedAmount <= royalitySlab.MaxValue)
                {
                    var amountToCharge = yearlytoCurrentDayPayedAmount - royalitySlab.MinValue;
                    amountToCharge = amountToCharge > monthlypayedSum1 ? monthlypayedSum1 : amountToCharge;
                    royalityAmount = (amountToCharge.GetValueOrDefault() * royalitySlab.ChargePercentage) / 100;
                    break;
                }
                if (totallyYearlySales < royalitySlab.MaxValue)
                {
                    var amountToCharge = royalitySlab.MaxValue.Value - totallyYearlySales;
                    royalityAmount = (amountToCharge.GetValueOrDefault() * royalitySlab.ChargePercentage) / 100;
                }
            }
            monthlyRoyality = royalityAmount;
            isMinRoyalityReached = royalityAmount > minRoyalityPerMonth ? true : false;
            royalityAmount = royalityAmount > minRoyalityPerMonth ? royalityAmount : minRoyalityPerMonth;
            totallyYearlySales += monthlypayedSum1;
            return royalityAmount;
        }
        private FranchiseeSales GetRecord(string qbIdentifier, long franchiseeId, DateTime date)
        {
            var getPrefix = GetPrefix(date);


            var qbIdentifierModified = long.Parse(getPrefix + qbIdentifier);
            var franchiseeSales = new FranchiseeSales();
            var franchiseeSalesList = _franchiseeSalesRepository.Fetch(x => x.QbInvoiceNumber.Equals(qbIdentifier) && x.FranchiseeId == franchiseeId).ToList();
            if (franchiseeSalesList.Count() > 1)
            {
                franchiseeSales = franchiseeSalesList.FirstOrDefault(x => x.QbInvoiceNumber.Equals(qbIdentifier) && x.FranchiseeId == franchiseeId && (x.SalesDataUpload.PeriodStartDate <= date && x.SalesDataUpload.PeriodEndDate >= date));
            }
            else
            {
                franchiseeSales = franchiseeSalesList.FirstOrDefault(x => x.QbInvoiceNumber.Equals(qbIdentifier) && x.FranchiseeId == franchiseeId && (x.SalesDataUpload.PeriodStartDate <= date && x.SalesDataUpload.PeriodEndDate >= date));
            }
            if (franchiseeSales == null)
            {
                var previousDates = date.AddDays(-7);
                var nextDates = date.AddDays(7);
                franchiseeSales = _franchiseeSalesRepository.Table.FirstOrDefault(x => x.QbInvoiceNumber.Equals(qbIdentifier) && x.FranchiseeId == franchiseeId && ((x.SalesDataUpload.PeriodStartDate >= previousDates && x.SalesDataUpload.PeriodStartDate <= nextDates) ||
                      (x.SalesDataUpload.PeriodStartDate >= nextDates && x.SalesDataUpload.PeriodStartDate <= nextDates)));
            }
            if (franchiseeSales == null)
            {
                franchiseeSales = _franchiseeSalesRepository.Table.FirstOrDefault(x => x.CustomerQbInvoiceId.Equals(qbIdentifierModified) && x.FranchiseeId == franchiseeId);
            }
            if (franchiseeSales == null)
                return null;
            return franchiseeSales;
        }

        private FranchiseeSales GetRecordWithoutPrefix(string qbIdentifier, long franchiseeId, DateTime date)
        {
            //var qbIdentifierModified = long.Parse(qbIdentifier);
            var franchiseeSales = new FranchiseeSales();
            var franchiseeSalesList = _franchiseeSalesRepository.Fetch(x => x.QbInvoiceNumber.Equals(qbIdentifier) && x.FranchiseeId == franchiseeId).ToList();
            if (franchiseeSalesList.Count() > 1)
            {
                franchiseeSales = franchiseeSalesList.FirstOrDefault(x => x.QbInvoiceNumber.Equals(qbIdentifier) && x.FranchiseeId == franchiseeId && (x.SalesDataUpload.PeriodStartDate <= date && x.SalesDataUpload.PeriodEndDate >= date));
            }
            else
            {
                franchiseeSales = franchiseeSalesList.FirstOrDefault(x => x.QbInvoiceNumber.Equals(qbIdentifier) && x.FranchiseeId == franchiseeId && (x.SalesDataUpload.PeriodStartDate <= date && x.SalesDataUpload.PeriodEndDate >= date));
            }
            if (franchiseeSales == null)
            {
                var previousDates = date.AddDays(-7);
                var nextDates = date.AddDays(7);
                franchiseeSales = _franchiseeSalesRepository.Table.FirstOrDefault(x => x.QbInvoiceNumber.Equals(qbIdentifier) && x.FranchiseeId == franchiseeId && ((x.SalesDataUpload.PeriodStartDate >= previousDates && x.SalesDataUpload.PeriodStartDate <= nextDates) ||
                      (x.SalesDataUpload.PeriodStartDate >= nextDates && x.SalesDataUpload.PeriodStartDate <= nextDates)));
            }
            //if (franchiseeSales == null)
            //{
            //    franchiseeSales = _franchiseeSalesRepository.Table.FirstOrDefault(x => x.CustomerQbInvoiceId.Equals(qbIdentifierModified) && x.FranchiseeId == franchiseeId);
            //}
            if (franchiseeSales == null)
                return null;
            return franchiseeSales;
        }
        private long AnnualReportType(ParsedFileParentModel record, FranchiseeSales franchiseeSales, IList<ParsedFileParentModel> records, out string qbInvoicesId)
        {

            string qbInvoices = default(string);
            qbInvoicesId = "";
            long reportId = default(long);
            var annualSalesAmount = record.Invoice.InvoiceItems.Sum(ii => ii.Amount);
            var annualPaidAmount = record.Invoice.Payments.Sum(x => x.Amount);

            var weeklySalesAmount = franchiseeSales.Invoice.InvoiceItems.Sum(ii => ii.Amount);
            var weeklyPaidAmount = franchiseeSales.Invoice.InvoicePayments.Sum(ip => ip.Payment.Amount);
            var inActiveServices = _franchiseeServiceRepository.Table.Where(x => x.FranchiseeId == franchiseeSales.FranchiseeId && !x.CalculateRoyalty).Select(x => x.ServiceTypeId).ToList();
            if (qbInvoices == null)
            {
                qbInvoices = franchiseeSales.QbInvoiceNumber;
            }
            if (inActiveServices.Contains(record.ServiceTypeId))
            {
                reportId = (long)AuditReportType.Type1H;
                qbInvoicesId = qbInvoices;
            }
            else if (weeklySalesAmount == annualSalesAmount && annualPaidAmount > weeklyPaidAmount)
            {
                reportId = (long)AuditReportType.Type1D;
                return reportId;
            }
            else if (annualSalesAmount == annualPaidAmount && weeklyPaidAmount == weeklySalesAmount && annualSalesAmount != weeklySalesAmount)
            {
                reportId = CompareDataForCases(record, franchiseeSales, weeklySalesAmount, weeklyPaidAmount, annualSalesAmount, annualPaidAmount, 1, out qbInvoices, records);
                qbInvoicesId = qbInvoices;
            }
            //else if (annualSalesAmount == annualPaidAmount && weeklyPaidAmount == weeklySalesAmount && annualSalesAmount == weeklySalesAmount)
            //{
            //    reportId = (long)AuditReportType.Type1C;
            //    qbInvoicesId = qbInvoices;
            //}

            else if (annualSalesAmount == annualPaidAmount && annualPaidAmount == weeklyPaidAmount && weeklySalesAmount != weeklyPaidAmount)
            {
                reportId = CompareDataForCases(record, franchiseeSales, weeklySalesAmount, weeklyPaidAmount, annualSalesAmount, annualPaidAmount, 2, out qbInvoices);
                qbInvoicesId = qbInvoices;
            }

            else if (weeklyPaidAmount == annualSalesAmount && weeklyPaidAmount == weeklySalesAmount && annualPaidAmount != weeklySalesAmount)
            {
                reportId = CompareDataForCases(record, franchiseeSales, weeklySalesAmount, weeklyPaidAmount, annualSalesAmount, annualPaidAmount, 3, out qbInvoices);
                qbInvoicesId = qbInvoices;
            }

            else if (weeklyPaidAmount == annualPaidAmount && annualSalesAmount == weeklySalesAmount)
            {
                reportId = CompareDataForCases(record, franchiseeSales, weeklySalesAmount, weeklyPaidAmount, annualSalesAmount, annualPaidAmount, 4, out qbInvoices);
                qbInvoicesId = qbInvoices;
            }

            else if (weeklyPaidAmount != annualPaidAmount && weeklyPaidAmount != annualSalesAmount && annualSalesAmount != annualPaidAmount && annualPaidAmount != weeklyPaidAmount)
            {
                reportId = CompareDataForCases(record, franchiseeSales, weeklySalesAmount, weeklyPaidAmount, annualSalesAmount, annualPaidAmount, 5, out qbInvoices);
                qbInvoicesId = qbInvoices;
            }

            else if (annualSalesAmount == annualPaidAmount && annualPaidAmount == weeklySalesAmount && weeklySalesAmount != weeklyPaidAmount)
            {
                reportId = CompareDataForCases(record, franchiseeSales, weeklySalesAmount, weeklyPaidAmount, annualSalesAmount, annualPaidAmount, 6, out qbInvoices, records);
                qbInvoicesId = qbInvoices;
            }

            else if (weeklySalesAmount == annualPaidAmount && annualSalesAmount > weeklySalesAmount)
            {
                reportId = (long)AuditReportType.Type3;
                return reportId;
            }

            else if (annualPaidAmount != annualSalesAmount && annualPaidAmount == weeklyPaidAmount && annualSalesAmount > annualPaidAmount)
            {
                reportId = (long)AuditReportType.Type17D;
                return reportId;
            }

            else if (annualPaidAmount == annualSalesAmount && weeklyPaidAmount > annualPaidAmount && weeklySalesAmount > annualPaidAmount)
            {
                if (annualSalesAmount == 0)
                {
                    reportId = (long)AuditReportType.Type5A;
                }
                else
                {
                    reportId = (long)AuditReportType.Type18A;
                }
                return reportId;
            }

            else if (annualPaidAmount == annualSalesAmount && annualPaidAmount != weeklySalesAmount && annualPaidAmount != weeklyPaidAmount && weeklyPaidAmount > annualPaidAmount && weeklySalesAmount < annualPaidAmount)
            {
                reportId = (long)AuditReportType.Type18B;
                return reportId;
            }

            //else if (annualPaidAmount == annualSalesAmount && weeklySalesAmount == weeklyPaidAmount && weeklyPaidAmount > annualPaidAmount)
            //{
            //    reportId = (long)AuditReportType.Type18C;
            //    return reportId;
            //}

            //else if (annualPaidAmount == annualSalesAmount && annualPaidAmount == weeklyPaidAmount && weeklySalesAmount > weeklyPaidAmount)
            //{
            //    reportId = (long)AuditReportType.Type18D;
            //    return reportId;
            //}

            else if (annualPaidAmount == annualSalesAmount && annualSalesAmount == 0 && weeklySalesAmount > weeklyPaidAmount)
            {
                reportId = (long)AuditReportType.Type5A;
                return reportId;
            }

            else if (annualPaidAmount == annualSalesAmount && annualSalesAmount == weeklySalesAmount && weeklyPaidAmount < weeklySalesAmount)
            {
                reportId = (long)AuditReportType.Type4B;
                return reportId;
            }
            return reportId;
        }

        //    Type 6,Type1C, Type5,Type1B,Type13B,TYpe7,Type8,Type11   CaseId 1
        //    Type1A,Type14,Type9                                      CaseId 2
        //    Type1D, Type16                                           CasrId 3
        //    Type 12, perfect                                         CasrId 4
        //    Type2B, Type2A                                           CasrId 5
        //    Type4, Type 13, Type10B, Type1F                          CasrId 6

        private long CompareDataForCases(ParsedFileParentModel record, FranchiseeSales franchiseeSales, decimal weeklySalesAmount, decimal weeklyPaidAmount, decimal AnnualSalesAmount,
                                                   decimal AnnualPaidAmount, long caseId, out string qbInvoiceIds, IList<ParsedFileParentModel> collection = null)
        {
            long reportId = default(long);
            qbInvoiceIds = "";
            string qbInvoice = "";
            if (caseId == 1)
            {
                if (weeklyPaidAmount > AnnualPaidAmount)
                {
                    if (isSplitInMany(record, franchiseeSales, collection, out qbInvoice))
                    {
                        reportId = (long)AuditReportType.Type13B;
                        qbInvoiceIds = qbInvoice;
                    }
                    else if (weeklyPaidAmount > (AnnualPaidAmount))
                    {
                        reportId = (long)AuditReportType.Type8;
                    }

                    else if (AnnualPaidAmount == 0)
                    {
                        reportId = (long)AuditReportType.Type5;
                    }

                    else if (Math.Round(((double)AnnualPaidAmount * 1.06), 0) == Math.Round((double)weeklyPaidAmount, 0))
                    {
                        reportId = (long)AuditReportType.Type11;
                    }

                }
                else
                {

                    if (AnnualPaidAmount > (weeklyPaidAmount))
                    {
                        reportId = (long)AuditReportType.Type1C;
                    }
                }

            }
            else if (caseId == 2)
            {
                if (Math.Round(((double)AnnualPaidAmount * 1.06), 0) == Math.Round((double)weeklySalesAmount, 0))
                {
                    reportId = (long)AuditReportType.Type11;
                }
                else if (weeklySalesAmount > weeklyPaidAmount)
                {

                    if (weeklySalesAmount - weeklyPaidAmount <= 1 && weeklySalesAmount - weeklyPaidAmount >= 0)
                    {
                        reportId = (long)AuditReportType.Type9;
                    }
                    else
                    {
                        reportId = (long)AuditReportType.Type1A;
                    }
                }

                else if (weeklyPaidAmount > weeklySalesAmount)
                {
                    reportId = (long)AuditReportType.Type14;
                }
            }
            else if (caseId == 3)
            {

                if (AnnualSalesAmount > AnnualPaidAmount)
                {
                    reportId = (long)AuditReportType.Type16;
                }

            }

            else if (caseId == 4)
            {
                if (AnnualPaidAmount == 0)
                {
                    reportId = (long)AuditReportType.Type12;
                }

                else
                {
                    reportId = (long)AuditReportType.Type1;
                }
            }

            else if (caseId == 5)
            {
                if (AnnualPaidAmount > weeklyPaidAmount)
                {
                    reportId = (long)AuditReportType.Type2A;
                }

                else if (AnnualPaidAmount < weeklyPaidAmount)
                {
                    reportId = (long)AuditReportType.Type2B;
                }
            }

            else if (caseId == 6)
            {

                if (weeklyPaidAmount > weeklySalesAmount)
                {
                    if (isSplitInTwo(record, franchiseeSales, collection, out qbInvoice))
                    {
                        reportId = (long)AuditReportType.Type13;
                        qbInvoiceIds = qbInvoice;
                    }
                    else
                    {
                        if (weeklyPaidAmount > (weeklySalesAmount))
                        {
                            if (weeklyPaidAmount == 2 * weeklySalesAmount)
                                reportId = (long)AuditReportType.Type4A;
                            else
                                reportId = (long)AuditReportType.Type4;
                        }
                    }

                }

                else if (weeklyPaidAmount == 0)
                {
                    reportId = (long)AuditReportType.Type10B;
                }

                else if (weeklySalesAmount > (weeklyPaidAmount))
                {
                    reportId = (long)AuditReportType.Type1F;

                }
            }
            return reportId;
        }

        private bool isSplitInTwo(ParsedFileParentModel annualRecord, FranchiseeSales sales, IList<ParsedFileParentModel> collection, out string qbInvoice)
        {
            bool hasSplit = false;
            qbInvoice = null;
            if (collection != null)
            {
                var coll = collection.Where(x => x.Date == annualRecord.Date && x.Customer.Name == annualRecord.Customer.Name && x.QbIdentifier != annualRecord.QbIdentifier);
                if (coll != null)
                {
                    collection = coll.ToList();
                }
                else
                {
                    return false;
                }
                decimal salesDifference = sales.Invoice.InvoicePayments.Sum(x => x.Payment.Amount) - sales.Invoice.InvoiceItems.Sum(x => x.Amount);

                if (collection.Count() > 1)
                {
                    hasSplit = isSplitted(collection, collection.Count(), salesDifference, out qbInvoice);
                }
                else
                {
                    foreach (var salesData in collection)
                    {
                        if (salesData.Invoice.InvoiceItems.Sum(x => x.Amount) == salesDifference)
                        {
                            hasSplit = true;
                            qbInvoice = salesData.QbIdentifier.ToString();
                            break;
                        }
                    }
                }
            }


            return hasSplit;
        }
        private bool isSplitInMany(ParsedFileParentModel record, FranchiseeSales sales, IList<ParsedFileParentModel> collection, out string qbInvoice)
        {

            bool hasSplit = false;
            qbInvoice = "";
            if (collection != null)
            {
                var coll = collection.Where(x => x.Date == record.Date && x.Customer.Name == record.Customer.Name && x.QbIdentifier != record.QbIdentifier);
                if (coll != null)
                {
                    collection = coll.ToList();
                }
                else
                {
                    return false;
                }
                decimal salesDifference = sales.Invoice.InvoicePayments.Sum(x => x.Payment.Amount) - record.Invoice.Payments.Sum(x => x.Amount);
                var annualData = collection.Where(x => x.Invoice.Payments.Sum(x1 => x1.Amount) == salesDifference && x.Customer.Name == record.Customer.Name
                                                            && x.Date == record.Date).ToList();

                if (collection.Count() > 1)
                {
                    hasSplit = isSplitted(collection, collection.Count(), salesDifference, out qbInvoice);

                }
                else
                {
                    foreach (var salesData in collection)
                    {
                        if (salesData.Invoice.InvoiceItems.Sum(x => x.Amount) == salesDifference)
                        {
                            hasSplit = true;
                            qbInvoice = salesData.QbIdentifier.ToString();
                            break;
                        }
                    }
                }
            }
            return hasSplit;
        }
        private bool isSplitted(IList<ParsedFileParentModel> collection, int length, decimal sum, out string qbInvoice)
        {
            long[,] pass2 = new long[11, 3] { { 1,2,3 } ,
                                  { 1,2,4 }, { 1,2,5 } , { 1,3,4}, { 1,3,5 }, { 2,3,4 }, { 2,3,5 }, { 3,4,5}, { 3,4,2 }, { 4,5,1}, { 4,5,2 }};

            qbInvoice = "";
            int len = length;
            for (int parses = 0; parses <= (length - 1); parses++)
            {
                if (parses == 0)
                {
                    for (int index = 0; index < length; index++)
                    {
                        while (index < length)
                        {
                            length--;
                            if (collection[index].Invoice.InvoiceItems.Sum(x => x.Amount) + collection[length].Invoice.InvoiceItems.Sum(x => x.Amount) == sum)
                            {
                                qbInvoice = collection[index].QbIdentifier + "," + collection[length].QbIdentifier;
                                return true;
                            }
                        }
                        length = len;
                    }
                }
                if (parses == 1)
                {
                    decimal sum2 = default(decimal);
                    for (var count = 0; count <= collection.Count() - 1; count++)
                    {
                        sum2 += collection[count].Invoice.InvoiceItems.Sum(x => x.Amount);
                    }
                    if (sum2 == sum)
                    {
                        qbInvoice = collection[0].QbIdentifier + "," + collection[1].QbIdentifier + "," + collection[2].QbIdentifier;
                        return true;
                    }
                }
            }
            return false;
        }
        private bool isInvoiceChanged(ParsedFileParentModel record, FranchiseeSales sales, IList<ParsedFileParentModel> collection, out string qbInvoice, out long invoiceId)
        {
            bool hasChanged = false;
            qbInvoice = default(string);
            invoiceId = default(long);
            if (sales == null)
            {
                var franchiseeSales = _franchiseeSalesRepository.Table.Where(x => x.FranchiseeId == franchiseeId
                                               && x.SalesDataUpload.PeriodStartDate >= startDate && x.SalesDataUpload.PeriodEndDate <= endDate
                                                  && (x.Customer.Name == record.Customer.Name || x.Customer.ContactPerson == record.Customer.ContactPerson)).ToList();


                foreach (var franchiseeSalesData in franchiseeSales)
                {
                    var invoiceCollections = franchiseeSales.Select(x => x.InvoiceId).ToList();
                    var invoicesList = _invoiceRepository.Table.Where(x => invoiceCollections.Contains(x.Id)).ToList();
                    var description = new List<string>();

                    //var description2 = record.Invoice.InvoiceItems.Select(x1 => x1.Description).ToList();
                    //invoicesList.Where(x => x.Id == franchiseeSalesData.InvoiceId).ToList().ForEach(x1 => description.AddRange(x1.InvoiceItems.Select(x => x.Description)));

                    var invoicesList2 = invoicesList.Where(x => (x.Id == franchiseeSalesData.InvoiceId) && (x.InvoiceItems.Sum(x1 => x1.Amount) == (decimal)record.Invoice.InvoiceItems.Sum(y => y.Amount))
                                                     && (franchiseeSalesData.Invoice.GeneratedOn >= (record.Date).AddDays(-7) && franchiseeSalesData.Invoice.GeneratedOn <= (record.Date).AddDays(7))
                                                     && (x.InvoicePayments.Sum(x1 => x1.Payment.Amount) == (decimal)record.Invoice.Payments.Sum(y => y.Amount))).ToList();
                    if (invoicesList2.Count() >= 1)
                    {
                        //var invoice = CreateAuditRecord(record, sales, (long)AuditReportType.Type7, franchiseeId);
                        qbInvoice = franchiseeSalesData.QbInvoiceNumber;
                        invoiceId = franchiseeSalesData.InvoiceId.GetValueOrDefault();
                        hasChanged = true;
                        break;
                    }
                }
            }
            else
            {
                var invoicesList2 = collection.Where(x => (x.Invoice.InvoiceItems.Sum(x1 => x1.Amount) == sales.Invoice.InvoiceItems.Sum(y => y.Amount))
                                                    && (x.Invoice.Payments.Sum(x1 => x1.Amount) == sales.Invoice.InvoicePayments.Sum(y => y.Payment.Amount))
                                                    && ((sales.Customer != null && x.Customer != null) ? (x.Customer.Name == sales.Customer.Name && x.Customer.ContactPerson == sales.Customer.ContactPerson) : true)).ToList();
                if (invoicesList2.Count() >= 1)
                {
                    qbInvoice = invoicesList2.Select(x => x.QbIdentifier).FirstOrDefault();
                    //var invoice = CreateAuditRecord(record, sales, (long)AuditReportType.Type7, franchiseeId);
                    hasChanged = true;
                }

            }

            return hasChanged;
        }


        private bool CompareData(ParsedFileParentModel record, FranchiseeSales franchiseeSales)
        {
            var salesAmount = record.Invoice.InvoiceItems.Sum(ii => ii.Amount);
            var paidAmount = record.Invoice.Payments.Sum(x => x.Amount);

            var inDBSalesAmount = franchiseeSales.Invoice.InvoiceItems.Sum(ii => ii.Amount);
            var inDBpaidAmount = franchiseeSales.Invoice.InvoicePayments.Sum(ip => ip.Payment.Amount);

            if (salesAmount != inDBSalesAmount)
            {
                //var invoice = CreateAuditRecord(record, franchiseeSales, null, franchiseeSales.FranchiseeId);
                return false;
            }
            if (paidAmount != inDBpaidAmount)
            {
                //var invoice = CreateAuditRecord(record, franchiseeSales, null, franchiseeSales.FranchiseeId);
                return false;
            }
            return true;
        }

        private AuditInvoice CreateAuditRecord(ParsedFileParentModel record, FranchiseeSales franchiseeSales, long? reportTypeId = null, long? franchiseeId = null, string qaInvoiceIds = null, long annualUploadId = default(long))
        {

            var invoice = _auditFactory.CreateDomain(record.Invoice);
            invoice.ReportTypeId = reportTypeId;
            string qbInvoice = default(string);
            long auditCustomerId = default(long);
            long invoiceId = default(long);
            StringBuilder sb = new StringBuilder();
            if (franchiseeSales == null && reportTypeId == null)
            {
                //var customerInfo = salesInfo.Customer;
                auditCustomerId = CreateAuditCustomer(record);
                bool isChanged = isInvoiceChanged(record, null, null, out qbInvoice, out invoiceId);
                invoice.InvoiceId = franchiseeSales != null ? franchiseeSales.InvoiceId : null;
                invoice.QBInvoiceNumber = record.QbIdentifier;
                bool isTypeBvalue = isTypeB(record.QbIdentifier, annualUploadId);

                if (isTypeBvalue)
                {
                    invoice.isActive = false;
                }
                if (isChanged == true)
                {
                    invoice.ReportTypeId = (long)AuditReportType.Type7;
                    invoice.InvoiceId = invoiceId;
                }
                else
                {
                    invoice.ReportTypeId = (long)AuditReportType.Type1B;
                    invoice.QBInvoiceNumbers = qaInvoiceIds;
                }
            }
            else
            {
                if (franchiseeSales.InvoiceId != null)
                {
                    invoice.QBInvoiceNumbers = qaInvoiceIds;
                    invoice.InvoiceId = franchiseeSales.InvoiceId;
                    invoice.QBInvoiceNumber = record.QbIdentifier;
                }
            }
            invoice.CustomerQbInvoiceId = franchiseeSales != null && franchiseeSales.CustomerQbInvoiceId != null ? franchiseeSales.CustomerQbInvoiceId.ToString() : "";
            _auditInvoiceRepository.Save(invoice);

            foreach (var item in record.Invoice.Payments)
            {
                var payment = SavePayment(item);
                var auditInvoicePayment = _auditFactory.CreateDomain(invoice.Id, payment.Id);

                _auditInvoicePaymentRepository.Save(auditInvoicePayment);
            }
            var auditFranchiseeSales = _auditFactory.CreateViewModel(record, franchiseeId.GetValueOrDefault(), (franchiseeSales != null ?
                                                 (long?)franchiseeSales.CustomerId : null), (franchiseeSales == null ? (long?)auditCustomerId : null),
                                                 franchiseeSales != null ?
                                                 (long?)franchiseeSales.AccountCreditId : null);
            auditFranchiseeSales.AuditInvoiceId = invoice.Id;
            auditFranchiseeSales.CustomerInvoiceIdString = franchiseeSales != null ? franchiseeSales.CustomerInvoiceIdString : "";
            auditFranchiseeSales.CustomerInvoiceId = franchiseeSales != null ? franchiseeSales.CustomerInvoiceId : default(long);
            auditFranchiseeSales.CustomerQbInvoiceId = franchiseeSales != null && franchiseeSales.CustomerQbInvoiceId != null ? franchiseeSales.CustomerQbInvoiceId.ToString() : "";
            _auditFranchiseeSalesRepository.Save(auditFranchiseeSales);
            return invoice;
        }

        private bool isTypeB(string qbInvoice, long annualUploadId)
        {
            bool isTypeB = false;
            var auditInvoices = _auditInvoiceRepository.Table.Where(x => x.AnnualUploadId == annualUploadId && x.QBInvoiceNumber != null).ToList();
            if (auditInvoices.Count > 0)
            {
                foreach (var qbInvoices in auditInvoices)
                {
                    isTypeB = auditInvoices.Any(x => x.QBInvoiceNumbers != null && x.QBInvoiceNumbers.Split(',').Contains(qbInvoice));
                }
            }
            return isTypeB;
        }
        private bool checkForAddress(AddressEditModel model)
        {
            bool isValid = false;
            if (model != null)
            {
                isValid = _addressRepository.Table.Any(x => x.AddressLine1 == model.AddressLine1 && (x.StateId == model.StateId && x.StateName == model.State)
                                          && (x.CityId == model.CityId && x.CityName == model.City));
            }
            return isValid;
        }
        private AuditPayment SavePayment(FranchiseeSalesPaymentEditModel model)
        {
            var payment = _auditFactory.CreateDomain(model);
            _auditPaymentRepository.Save(payment);
            return payment;
        }

        private InvoiceEditModel CreateNewInvoice(InvoiceEditModel invoice, SaveModelStats stats)
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
            else if (stats.TotalAmount == 0)
            {
                invoice.StatusId = (long)InvoiceStatus.ZeroDue;
            }
            else
            {
                invoice.StatusId = (long)InvoiceStatus.Unpaid;
            }
            //var savedInvoice = _invoiceService.Save(invoice);
            return invoice;
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

        private static void LogException(StringBuilder sb, Exception ex)
        {
            sb.Append(Log("Error - " + ex.Message));
            sb.Append(Log("Error - " + ex.StackTrace));
            if (ex.InnerException != null && ex.InnerException.StackTrace != null)
                sb.Append(Log("Error - " + ex.InnerException.StackTrace));
        }

        private void MarkUploadAsFailed(AnnualSalesDataUpload annualFileUpload)
        {
            annualFileUpload.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            annualFileUpload.StatusId = (long)SalesDataUploadStatus.Failed;
            SaveSalesDataUpload(annualFileUpload);
        }

        private static string Log(string message)
        {
            return DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss:fff tt") + " \t\t " + message + "\r\n\r\n";
        }

        private void SaveSalesDataUpload(AnnualSalesDataUpload annualFileUpload)
        {
            try
            {
                _unitOfWork.StartTransaction();
                var fileModel = PrepareLogFileModel("AnnualSales_" + annualFileUpload.Id);
                var file = _fileService.SaveModel(fileModel);
                annualFileUpload.ParsedLogFileId = file.Id;

                _annualFileUploadRepository.Save(annualFileUpload);

                _unitOfWork.SaveChanges();
                CreateNotifications(annualFileUpload);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw new Exception(ex.Message);
            }
        }

        private void CreateNotifications(AnnualSalesDataUpload annualFileUpload)
        {
            try
            {
                _unitOfWork.StartTransaction();
                // send Notification to franchisee
                _annualAuditNotificationService.CreateAnnualUploadNotification(annualFileUpload);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Info(string.Format("Unable to send Notification to franchisee - {0}, error - {1}", annualFileUpload.FranchiseeId, ex.StackTrace));
            }
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

        private void CreateLogFile(StringBuilder sb, string fileName)
        {
            var path = MediaLocationHelper.GetMediaLocationForLogs().Path + fileName;
            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write(sb.ToString());
            }
        }

        private AnnualSalesDataUpload GetFileToParse()
        {
            return _annualFileUploadRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        class SaveModelStats
        {
            public string QBIdentifier;
            public decimal TotalAmount;
            public decimal PaidAmount;
            public bool IsMismatch;
            public string Logs;
        }

        private void UpdateSalesDataStatus(AnnualSalesDataUpload annualFileUpload, decimal totalAmount, decimal paidAmount, int failedRecords,
            int parsedRecords, int mismatchedRecords)
        {
            annualFileUpload.TotalAmount = totalAmount;
            annualFileUpload.PaidAmount = paidAmount;
            annualFileUpload.NoOfFailedRecords = failedRecords;
            annualFileUpload.NoOfParsedRecords = parsedRecords;
            annualFileUpload.NoOfMismatchedRecords = mismatchedRecords;

            annualFileUpload.StatusId = (long)(failedRecords > 0 && parsedRecords == 0 ? SalesDataUploadStatus.Failed : SalesDataUploadStatus.Parsed);
        }

        private bool isInvoiceChangedForInvoice(ParsedFileParentModel record, FranchiseeSales sales, IList<ParsedFileParentModel> collection, List<FranchiseeSales> franchiseeSales, out string qbInvoice, out long invoiceId)
        {
            bool hasChanged = false;
            qbInvoice = default(string);
            invoiceId = default(long);
            if (sales == null)
            {


                foreach (var franchiseeSalesData in franchiseeSales)
                {
                    var invoiceCollections = franchiseeSales.Select(x => x.InvoiceId).ToList();
                    var invoicesList = _invoiceRepository.Table.Where(x => invoiceCollections.Contains(x.Id)).ToList();
                    var description = new List<string>();

                    //var description2 = record.Invoice.InvoiceItems.Select(x1 => x1.Description).ToList();
                    //invoicesList.Where(x => x.Id == franchiseeSalesData.InvoiceId).ToList().ForEach(x1 => description.AddRange(x1.InvoiceItems.Select(x => x.Description)));

                    var invoicesList2 = invoicesList.Where(x => (x.Id == franchiseeSalesData.InvoiceId) && (x.InvoiceItems.Sum(x1 => x1.Amount) == (decimal)record.Invoice.InvoiceItems.Sum(y => y.Amount))
                                                     && (franchiseeSalesData.Invoice.GeneratedOn >= (record.Date).AddDays(-7) && franchiseeSalesData.Invoice.GeneratedOn <= (record.Date).AddDays(7))
                                                     && (x.InvoicePayments.Sum(x1 => x1.Payment.Amount) == (decimal)record.Invoice.Payments.Sum(y => y.Amount))).ToList();
                    if (invoicesList2.Count() >= 1)
                    {
                        //var invoice = CreateAuditRecord(record, sales, (long)AuditReportType.Type7, franchiseeId);
                        qbInvoice = franchiseeSalesData.QbInvoiceNumber;
                        invoiceId = franchiseeSalesData.InvoiceId.GetValueOrDefault();
                        hasChanged = true;
                        break;
                    }
                }
            }
            else
            {
                var invoicesList2 = collection.Where(x => (x.Invoice.InvoiceItems.Sum(x1 => x1.Amount) == sales.Invoice.InvoiceItems.Sum(y => y.Amount))
                                                    && (x.Invoice.Payments.Sum(x1 => x1.Amount) == sales.Invoice.InvoicePayments.Sum(y => y.Payment.Amount))
                                                    && ((sales.Customer != null && x.Customer != null) ? (x.Customer.Name == sales.Customer.Name && x.Customer.ContactPerson == sales.Customer.ContactPerson) : true)).ToList();
                if (invoicesList2.Count() >= 1)
                {
                    qbInvoice = invoicesList2.Select(x => x.QbIdentifier).FirstOrDefault();
                    //var invoice = CreateAuditRecord(record, sales, (long)AuditReportType.Type7, franchiseeId);
                    hasChanged = true;
                }

            }

            return hasChanged;
        }


        private long GetPrefix(DateTime franchiseeSalesDates)
        {
            var prefix = 2018;
            var date = new DateTime(2019, 1, 2).Date;
            var date2 = new DateTime(2021, 5, 13).Date;

            if (franchiseeSalesDates <= date)
            {
                prefix = 2018;
            }
            else if (franchiseeSalesDates > date && franchiseeSalesDates <= date2)
            {
                prefix = 2020;
            }
            return prefix;
        }
        private class ServiceDiscountItem
        {
            public long ServiceTypeId { get; set; }
            public decimal Discount { get; set; }
        }
    }
}
