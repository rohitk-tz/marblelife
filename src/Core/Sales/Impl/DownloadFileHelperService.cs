using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Geo.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Scheduler;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class DownloadFileHelperService : IDownloadFileHelperService
    {
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private readonly IInvoiceFactory _invoiceFactory;
        private readonly IGeoCodeFactory _geoCodeFactory;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IFranchiseeSalesPaymentService _franchiseeSalesPaymentService;
        private readonly IPaymentFactory _paymentFactory;
        private readonly IRepository<BatchUploadRecord> _batchUploadRecordRepository;
        public readonly IRepository<Country> _countryRepository;
        public readonly IRepository<City> _cityRepository;
        public readonly IRepository<County> _countyRepository;
        public readonly IRepository<FranchiseeLoan> _franchiseeLoanRepository;
        public readonly IRepository<FranchiseeLoanSchedule> _franchiseeLoanScheduleRepository;
        public readonly IRepository<Organization> _organizationRepository;
        public DownloadFileHelperService(IUnitOfWork unitOfWork, IInvoiceFactory invoiceFactory, IExcelFileCreator excelFileCreator, IFranchiseeSalesPaymentService franchiseeSalesPaymentService,
            IPaymentFactory paymentFactory, IGeoCodeFactory geoCodeFactory)
        {
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _invoiceFactory = invoiceFactory;
            _excelFileCreator = excelFileCreator;
            _franchiseeSalesPaymentService = franchiseeSalesPaymentService;
            _paymentFactory = paymentFactory;
            _batchUploadRecordRepository = unitOfWork.Repository<BatchUploadRecord>();
            _geoCodeFactory = geoCodeFactory;
            _countryRepository = unitOfWork.Repository<Country>();
            _countyRepository = unitOfWork.Repository<County>();
            _cityRepository = unitOfWork.Repository<City>();
            _franchiseeLoanRepository = unitOfWork.Repository<FranchiseeLoan>();
            _franchiseeLoanScheduleRepository = unitOfWork.Repository<FranchiseeLoanSchedule>();
            _organizationRepository = unitOfWork.Repository<Organization>();
        }

        public List<DownloadInvoiceModel> CreateDataForAdFundInvoice(long[] invoiceIds)
        {
            // adFundInvoiceFile = string.Empty;
            var invoiceCollection = new List<DownloadInvoiceModel>();
            var franchiseeInvoices = _franchiseeInvoiceRepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();
            var adFundInvoices = new List<InvoiceItem>();
            //prepare item collection
            foreach (var item in franchiseeInvoices)
            {
                var isRoyality = IsLoanAdjustmentinRoyalityOrAdfund(item);
                var invoiceId = item.InvoiceId;
                var dueDate = item.Invoice.DueDate.Date;
                //var paymentDate = item.Invoice.InvoicePayments.FirstOrDefault().Payment.Date;
                var startDate = item.SalesDataUpload.PeriodStartDate;
                var endDate = item.InvoiceDate != null ? item.InvoiceDate.Value : dueDate;
                var paymentMode = _franchiseeSalesPaymentService.GetPaymentInstrument(item.Invoice.InvoicePayments);
                if (paymentMode == null && item.Invoice.StatusId == (long)InvoiceStatus.Paid
                    && item.Invoice.InvoicePayments.Any(x => x.Payment.InstrumentTypeId == (long)InstrumentType.AccountCredit))
                {
                    paymentMode = "Account Credit";
                }
                adFundInvoices = item.Invoice.InvoiceItems.ToList();
                foreach (var invoiceItem in adFundInvoices)
                {
                    var model = _invoiceFactory.CreateModel(invoiceId, startDate, endDate, dueDate, item.Franchisee, invoiceItem, paymentMode);
                    model.AdfundRoyalty = "A";
                    invoiceCollection.Add(model);
                }
            }
            invoiceCollection = invoiceCollection.Where(x => !x.Description.Contains("SEO Charges")).ToList();
            return invoiceCollection;

            //adFundInvoiceFile = MediaLocationHelper.GetTempMediaLocation().Path + "/adFund_invoices-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            //return _excelFileCreator.CreateExcelDocument(invoiceCollection, adFundInvoiceFile);
        }

        public List<DownloadInvoiceModel> CreateDataForRoyaltyInvoice(long[] invoiceIds)
        {
            // royaltyInvoieFile = string.Empty;
            var invoiceCollection = new List<DownloadInvoiceModel>();
            var franchiseeInvoices = _franchiseeInvoiceRepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();
            var royaltyInvoices = new List<InvoiceItem>();
            //prepare item collection
            foreach (var item in franchiseeInvoices)
            {
                var isRoyality = IsLoanAdjustmentinRoyalityOrAdfund(item);
                var invoiceId = item.InvoiceId; 
                var dueDate = item.Invoice.DueDate.Date;
                //var paymentDate = item.Invoice.InvoicePayments.FirstOrDefault().Payment.Date;
                var startDate = item.SalesDataUpload != null && item.SalesDataUpload.PeriodStartDate != null ? item.SalesDataUpload.PeriodStartDate : DateTime.Now;
                var endDate = item.InvoiceDate != null ? item.InvoiceDate.Value : dueDate;
                var paymentMode = _franchiseeSalesPaymentService.GetPaymentInstrument(item.Invoice.InvoicePayments);
                if (paymentMode == null && item.Invoice.StatusId == (long)InvoiceStatus.Paid
                    && item.Invoice.InvoicePayments.Any(x => x.Payment.InstrumentTypeId == (long)InstrumentType.AccountCredit))
                {
                    paymentMode = "Account Credit";
                }
                royaltyInvoices = item.Invoice.InvoiceItems.ToList();
                foreach (var invoiceItem in royaltyInvoices)
                {
                    var model = _invoiceFactory.CreateModel(invoiceId, startDate, endDate, dueDate, item.Franchisee, invoiceItem, paymentMode);
                    model.AdfundRoyalty = "R";
                    invoiceCollection.Add(model);
                }
            }
            invoiceCollection = invoiceCollection.Where(x => !x.Description.Contains("SEO Charges")).ToList();
            return invoiceCollection;
        }

        public bool IsLoanAdjustmentinRoyalityOrAdfund(FranchiseeInvoice invoice)
        {
            var invoiceItems = invoice.Invoice.InvoiceItems.Select(x => x.Id);
            var franchiseeLoan = _franchiseeLoanScheduleRepository.Table.Where(x => invoiceItems.Contains(x.InvoiceItemId.Value)).FirstOrDefault();
            if (franchiseeLoan != null)
                return franchiseeLoan.IsRoyality;
            else
                return true;
        }

        public List<DownloadPaymentModel> CreateDataForRoyaltyPayments(long[] invoiceIds)
        {
            //royaltyPaymentFile = string.Empty;
            var paymentList = new List<DownloadPaymentModel>();
            var franchiseeInvoices = _franchiseeInvoiceRepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();
            var royaltyPayments = new List<InvoicePayment>();
            //prepare item collection
            foreach (var item in franchiseeInvoices)
            {
                var isRoyality = IsLoanAdjustmentinRoyalityOrAdfund(item);
                var franchiseeName = item.Franchisee.Organization.Name;
                var invoiceId = item.InvoiceId;
                var date = item.Invoice.DueDate.Date;

                if (isRoyality)
                    royaltyPayments = item.Invoice.InvoiceItems.Where(x => !(x.ItemTypeId == (long)InvoiceItemType.AdFund) && !(x.ItemTypeId == (long)InvoiceItemType.LoanServiceFeeInterestRatePerAnnum)
                                          && !(x.ItemTypeId == (long)InvoiceItemType.LoanServiceFee)).SelectMany(y => y.Invoice.InvoicePayments).Distinct()
                        .OrderByDescending(x => x.PaymentId).ToList();

                else
                {
                    royaltyPayments = item.Invoice.InvoiceItems.Where(x => !(x.ItemTypeId == (long)InvoiceItemType.AdFund)).SelectMany(y => y.Invoice.InvoicePayments).Distinct()
                        .OrderByDescending(x => x.PaymentId).ToList();
                }
                if (royaltyPayments.Any())
                {
                    paymentList = CreateModel(invoiceId, franchiseeName, royaltyPayments, paymentList);
                }
            }
            paymentList = paymentList.Where(x => !x.Memo.Contains("SEO Web Services")).ToList();
            return paymentList;
        }

        public List<DownloadPaymentModel> CreateDataForAdFundPayments(long[] invoiceIds)
        {
            //adFundPaymentFile = string.Empty;
            var paymentList = new List<DownloadPaymentModel>();
            var franchiseeInvoices = _franchiseeInvoiceRepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();
            var adFundPayments = new List<InvoicePayment>();
            //prepare item collection
            foreach (var item in franchiseeInvoices)
            {
                var isRoyality = IsLoanAdjustmentinRoyalityOrAdfund(item);
                var franchiseeName = item.Franchisee.Organization.Name;
                var invoiceId = item.InvoiceId;
                var date = item.Invoice.DueDate.Date;

                if (isRoyality)
                    adFundPayments = item.Invoice.InvoiceItems.Where(x => (x.ItemTypeId == (long)InvoiceItemType.AdFund) || (x.ItemTypeId == (long)InvoiceItemType.LoanServiceFeeInterestRatePerAnnum)
                                          || (x.ItemTypeId == (long)InvoiceItemType.LoanServiceFee)).SelectMany(y => y.Invoice.InvoicePayments).Distinct().
                        OrderByDescending(x => x.PaymentId).ToList();
                else
                    adFundPayments = item.Invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)InvoiceItemType.AdFund).SelectMany(y => y.Invoice.InvoicePayments)
                                          .Distinct().OrderByDescending(x => x.PaymentId).ToList();

                if (adFundPayments.Any())
                {
                    paymentList = CreateModel(invoiceId, franchiseeName, adFundPayments, paymentList);
                }
            }
            paymentList = paymentList.Where(x => !x.Memo.Contains("SEO Web Services")).ToList();
            return paymentList;
        }

        public List<DownloadInvoiceModel> CreateDataForRoyaltyInvoiceFilter(List<FranchiseeInvoice> frInvoice)
        {
            //fileName = string.Empty;
            var invoiceCollection = new List<DownloadInvoiceModel>();

            //prepare item collection
            foreach (var item in frInvoice)
            {
                var invoiceId = item.InvoiceId;
                var dueDate = item.Invoice.DueDate.Date;
                //var paymentDate = item.Invoice.InvoicePayments.FirstOrDefault().Payment.Date;
                var startDate = item.SalesDataUpload.PeriodStartDate;
                var endDate = item.InvoiceDate != null ? item.InvoiceDate.Value : dueDate;
                var paymentMode = _franchiseeSalesPaymentService.GetPaymentInstrument(item.Invoice.InvoicePayments);
                if (paymentMode == null && item.Invoice.StatusId == (long)InvoiceStatus.Paid
                    && item.Invoice.InvoicePayments.Any(x => x.Payment.InstrumentTypeId == (long)InstrumentType.AccountCredit))
                {
                    paymentMode = "Account Credit";
                }
                var royaltyInvoices = item.Invoice.InvoiceItems.Where(x => !(x.ItemTypeId == (long)InvoiceItemType.AdFund)).ToList();

                foreach (var invoiceItem in royaltyInvoices)
                {
                    var model = _invoiceFactory.CreateModel(invoiceId, startDate, endDate, dueDate, item.Franchisee, invoiceItem, paymentMode);
                    invoiceCollection.Add(model);
                }
            }
            invoiceCollection = invoiceCollection.Where(x => !x.Description.Contains("SEO Charges")).ToList();
            return invoiceCollection;
        }

        public List<DownloadInvoiceModel> CreateDataForAdFundInvoiceFilter(List<FranchiseeInvoice> frInvoice)
        {
            //fileName = string.Empty;
            var invoiceCollection = new List<DownloadInvoiceModel>();

            //prepare item collection
            foreach (var item in frInvoice)
            {
                var invoiceId = item.InvoiceId;
                var dueDate = item.Invoice.DueDate.Date;
                //var paymentDate = item.Invoice.InvoicePayments.FirstOrDefault().Payment.Date;
                var startDate = item.SalesDataUpload.PeriodStartDate;
                var endDate = item.InvoiceDate != null ? item.InvoiceDate.Value : dueDate;
                var paymentMode = _franchiseeSalesPaymentService.GetPaymentInstrument(item.Invoice.InvoicePayments);
                if (paymentMode == null && item.Invoice.StatusId == (long)InvoiceStatus.Paid
                    && item.Invoice.InvoicePayments.Any(x => x.Payment.InstrumentTypeId == (long)InstrumentType.AccountCredit))
                {
                    paymentMode = "Account Credit";
                }
                var adFundInvoices = item.Invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)InvoiceItemType.AdFund).ToList();
                foreach (var invoiceItem in adFundInvoices)
                {
                    var model = _invoiceFactory.CreateModel(invoiceId, startDate, endDate, dueDate, item.Franchisee, invoiceItem, paymentMode);
                    invoiceCollection.Add(model);
                }
            }
            return invoiceCollection;
        }

        public List<DownloadPaymentModel> CreateDataForRoyaltyPaymentFilter(List<FranchiseeInvoice> frInvoice)
        {
            //fileName = string.Empty;
            var paymentList = new List<DownloadPaymentModel>();

            foreach (var item in frInvoice)
            {
                var franchiseeName = item.Franchisee.Organization.Name;
                var invoiceId = item.InvoiceId;
                var royaltyPayments = item.Invoice.InvoiceItems.Where(x => !(x.ItemTypeId == (long)InvoiceItemType.AdFund)).SelectMany(y => y.Invoice.InvoicePayments).Distinct().
                    OrderByDescending(x => x.PaymentId);
                if (royaltyPayments.Any())
                {
                    paymentList = CreateModel(invoiceId, franchiseeName, royaltyPayments, paymentList);
                }
            }
            paymentList = paymentList.Where(x => !x.Memo.Contains("SEO Charges")).ToList();
            return paymentList;
        }


       

        public List<DownloadPaymentModel> CreateDataForAdFundPaymentFilter(List<FranchiseeInvoice> frInvoice)
        {
            //fileName = string.Empty;
            var paymentList = new List<DownloadPaymentModel>();
            foreach (var item in frInvoice)
            {
                var franchiseeName = item.Franchisee.Organization.Name;
                var invoiceId = item.InvoiceId;
                var adFundPayments = item.Invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)InvoiceItemType.AdFund).SelectMany(y => y.Invoice.InvoicePayments).Distinct().
                    OrderByDescending(x => x.PaymentId);
                if (adFundPayments.Any())
                {
                    paymentList = CreateModel(invoiceId, franchiseeName, adFundPayments, paymentList);
                }
            }
            paymentList = paymentList.Where(x => !x.Memo.Contains("SEO Charges")).ToList();
            return paymentList;
        }

        private List<DownloadPaymentModel> CreateModel(long invoiceId, string franchiseeName, IEnumerable<InvoicePayment> payments, List<DownloadPaymentModel> paymentCollection)
        {
            var transactionDate = payments.FirstOrDefault().Payment.Date.ToShortDateString();
            var amount = payments.Sum(x => x.Payment.Amount);
            var memo = string.Empty;
            var paymentMode = string.Empty;
            var checkDetails = new PaymentModeDetailViewModel(); ;
            decimal accountCreditAmount = 0;
            foreach (var paymentItem in payments)
            {
                paymentMode = _franchiseeSalesPaymentService.GetPaymentInstrument(paymentItem.Invoice.InvoicePayments);
                checkDetails = _franchiseeSalesPaymentService.GetCheckDetails(paymentItem.Invoice.InvoicePayments);
                accountCreditAmount = _franchiseeSalesPaymentService.GetAccountCreditAmount(paymentItem.Invoice.InvoicePayments);
                if (string.IsNullOrEmpty(paymentMode))
                {
                    paymentMode = "Account Credit";
                    checkDetails.QBPaymentDetail = "XXXX";
                    memo = "The Amount of $" + accountCreditAmount + " has been charged from Account Credit.";
                }
                if (accountCreditAmount > 0 && amount != accountCreditAmount)
                    memo = "The Amount of $" + (amount - accountCreditAmount) + " has been charged from " + checkDetails.PaymentDetail + " and the Amount of $" + accountCreditAmount + " has been charged from Account Credit.";
                if (accountCreditAmount <= 0)
                    memo = "The Amount of $" + amount + " has been charged from " + checkDetails.PaymentDetail;
            }

            var model = _paymentFactory.CreateModel(invoiceId, franchiseeName, amount, transactionDate, paymentMode, memo, checkDetails.QBPaymentDetail);
            paymentCollection.Add(model);
            return paymentCollection;
        }

        public List<BatchUploadRecord> CreateDataForBatchRecord(DateTime startDateTime, DateTime endDateTime)
        {
            IQueryable<BatchUploadRecord> list;
            if (endDateTime == default(DateTime))
            {
                list = _batchUploadRecordRepository.Table.Where(x => (x.Franchisee.Organization.IsActive) && (!x.IsCorrectUploaded) && (!x.UploadedOn.HasValue && x.UploadedOn == null) && (x.StartDate <= startDateTime.Date && x.EndDate <= startDateTime.Date));
            }
            else
            {
                list = _batchUploadRecordRepository.Table.Where(x => (x.Franchisee.Organization.IsActive) && (!x.IsCorrectUploaded) && (!x.UploadedOn.HasValue && x.UploadedOn == null) && ((x.StartDate >= startDateTime.Date && x.EndDate <= endDateTime.Date)
                                                                        || (x.StartDate <= startDateTime.Date && x.EndDate >= startDateTime.Date)));
            }

            return list.ToList();

        }

        public List<DownloadCountyModel> CreateDataForCounty(List<County> countyList)
        {
            var organizationList = _organizationRepository.Table.ToList();

            var countyCollection = new List<DownloadCountyModel>();
            var countrylist = _countryRepository.TableNoTracking.Select(x => x).ToArray();
            foreach (var item in countyList)
            {
                var currentOrganization = organizationList.Where(x => x.Id == item.FranchiseeId).FirstOrDefault();
                var countryName = string.Empty;
                var country = item.CountryId != null ? countrylist.Where(x => x.Id == item.CountryId).FirstOrDefault() : null;

                if (country != null)
                    countryName = country.Name;

                var model = _geoCodeFactory.CreateModelForCounty(item, countryName);
                //if (item.FranchiseeId > 0 && currentOrganization != null)
                //{
                //    //model.FranchiseeEmail = currentOrganization.Email;
                //    var currentOrganizationWithTransferableNumber = currentOrganization.Phones.Where(x => x.IsTransferable == true).ToList();
                //    //if (currentOrganizationWithTransferableNumber.Count() > 0)
                //    //{
                //    //    model.FranchiseeTransferableNumber = currentOrganizationWithTransferableNumber.FirstOrDefault().Number;
                //    //}
                //}
                countyCollection.Add(model);
            }
            return countyCollection;
        }

        public List<DownloadZipCodeModel> CreateDataForZipCode(List<ZipCode> zipCodeList)
        {
            //fileName = string.Empty;
            var zipCodeCollection = new List<DownloadZipCodeModel>();
            var cityList = _cityRepository.TableNoTracking.Select(x => x).ToArray();
            var countyList = _countyRepository.TableNoTracking.Select(x => x).ToArray();
            //prepare item collection
            foreach (var item in zipCodeList)
            {
                string countyName = item.CountyName;
                string cityName = item.CityName;

                var city = cityList.Where(x => x.Id == item.CityId).FirstOrDefault();
                if (city != null)
                    cityName = city.Name;

                var county = countyList.Where(x => x.Id == item.CountyId).FirstOrDefault();
                if (county != null)
                    countyName = county.CountyName;

                var model = _geoCodeFactory.CreateModelForZipCode(item, countyName, cityName);
                //model.FranchiseeTransferableNumber = county!=null && county.Franchisee!=null  && county.Franchisee.Organization.Phones.Count()>0 &&
                //                                      county.Franchisee.Organization.Phones.Count()>0? county.Franchisee.Organization.Phones.Where(x => x.IsTransferable == true).Select(x=>x.Number).FirstOrDefault():"" ;
                //model.FranchiseeName = county != null && county.Franchisee != null && county.Franchisee.Organization!=null ? county.Franchisee.Organization.Name : "";
                //model.FranchiseeTransferableNumber
                zipCodeCollection.Add(model);
            }
            return zipCodeCollection;
        }


        public List<DownloadInstructionModel> CreateDataForInstruction()
        {
            //fileName = string.Empty;
            var instructionCollection = new List<DownloadInstructionModel>();
            //prepare item collection
            instructionCollection.Add(new DownloadInstructionModel() { Instructions = "1. This file as 2 data tabs – a ZIP-to-COUNTY assignment tab and a FRANCHISE-to-COUNTY tab" });
            instructionCollection.Add(new DownloadInstructionModel() { Instructions = "2. Periodically we may need to add a NEW zip to the COUNTY, in which case that line ID should be set to 0 which will signify that this is a NEW and ADDED line." });
            instructionCollection.Add(new DownloadInstructionModel() { Instructions = "3. Periodically we may need to change FRANCHISE assignments for the COUNTY.  Remember each franchise is a STATE-METRO AREA name." });
            instructionCollection.Add(new DownloadInstructionModel() { Instructions = "4. PHONE number is updated on the FRANCHISE DASHBOARD via the TRANSFER PHONE check box on the list of phone numbers in the first column" });
            instructionCollection.Add(new DownloadInstructionModel() { Instructions = "5. EMAIL is updated via the MARKETING EMAIL box in the DASHBOARD" });
            instructionCollection.Add(new DownloadInstructionModel() { Instructions = "6. Records that you want to edit, change column IsUpdated to 1 and records that you want to delete, change column IsDeleted to 1" });
            instructionCollection.Add(new DownloadInstructionModel() { Instructions = "7. If there is any change in ZipCode update IsUpdated to 1 for all its corresponding Counties in County Tab." });
            return instructionCollection;
        }

        public List<DownloadInvoiceModel> CreateDataForWebSeoInvoice(long[] invoiceIds)
        {
            // royaltyInvoieFile = string.Empty;
            var invoiceCollection = new List<DownloadInvoiceModel>();
            var franchiseeInvoices = _franchiseeInvoiceRepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();
            var royaltyInvoices = new List<InvoiceItem>();
            //prepare item collection
            foreach (var item in franchiseeInvoices)
            {
                var isRoyality = IsLoanAdjustmentinRoyalityOrAdfund(item);
                var invoiceId = item.InvoiceId;
                var dueDate = item.Invoice.DueDate.Date;
                //var paymentDate = item.Invoice.InvoicePayments.FirstOrDefault().Payment.Date;
                var startDate = item.SalesDataUpload.PeriodStartDate;
                var endDate = item.InvoiceDate != null ? item.InvoiceDate.Value : dueDate;
                var paymentMode = _franchiseeSalesPaymentService.GetPaymentInstrument(item.Invoice.InvoicePayments);
                if (paymentMode == null && item.Invoice.StatusId == (long)InvoiceStatus.Paid
                    && item.Invoice.InvoicePayments.Any(x => x.Payment.InstrumentTypeId == (long)InstrumentType.AccountCredit))
                {
                    paymentMode = "Account Credit";
                }
                if (!isRoyality)
                    royaltyInvoices = item.Invoice.InvoiceItems.Where(x => !(x.ItemTypeId == (long)InvoiceItemType.AdFund) && !(x.ItemTypeId == (long)InvoiceItemType.LoanServiceFeeInterestRatePerAnnum)
                                      && !(x.ItemTypeId == (long)InvoiceItemType.LoanServiceFee)).ToList();
                else
                    royaltyInvoices = item.Invoice.InvoiceItems.Where(x => !(x.ItemTypeId == (long)InvoiceItemType.AdFund)).ToList();

                foreach (var invoiceItem in royaltyInvoices)
                {
                    var model = _invoiceFactory.CreateModel(invoiceId, startDate, endDate, dueDate, item.Franchisee, invoiceItem, paymentMode);
                    model.AdfundRoyalty = "A";
                    invoiceCollection.Add(model);
                }
            }
            invoiceCollection = invoiceCollection.Where(x => x.Description.Contains("SEO Charges")).ToList();
            return invoiceCollection;
        }

        public List<DownloadPaymentModel> CreateDataForWebSeoPayments(long[] invoiceIds)
        {
            //royaltyPaymentFile = string.Empty;
            var paymentList = new List<DownloadPaymentModel>();
            var franchiseeInvoices = _franchiseeInvoiceRepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();
            var royaltyPayments = new List<InvoicePayment>();
            //prepare item collection
            foreach (var item in franchiseeInvoices)
            {
                var isRoyality = IsLoanAdjustmentinRoyalityOrAdfund(item);
                var franchiseeName = item.Franchisee.Organization.Name;
                var invoiceId = item.InvoiceId;
                var date = item.Invoice.DueDate.Date;

                if (isRoyality)
                    royaltyPayments = item.Invoice.InvoiceItems.Where(x => (x.ItemTypeId == (long)InvoiceItemType.AdFund || x.ItemTypeId == (long)InvoiceItemType.RoyaltyFee)
                                          && !(x.ItemTypeId == (long)InvoiceItemType.LoanServiceFee)).SelectMany(y => y.Invoice.InvoicePayments).Distinct()
                        .OrderByDescending(x => x.PaymentId).ToList();

                else
                {
                    royaltyPayments = item.Invoice.InvoiceItems.Where(x => !(x.ItemTypeId == (long)InvoiceItemType.AdFund)).SelectMany(y => y.Invoice.InvoicePayments).Distinct()
                        .OrderByDescending(x => x.PaymentId).ToList();
                }
                if (royaltyPayments.Any())
                {
                    paymentList = CreateModel(invoiceId, franchiseeName, royaltyPayments, paymentList);
                }
            }
            paymentList = paymentList.Where(x => x.Memo.Contains("SEO Web Services")).ToList();
            return paymentList;
        }

        public List<DownloadPaymentModel> CreateDataForWebSeoPaymentFilter(List<FranchiseeInvoice> frInvoice)
        {
            //fileName = string.Empty;
            var paymentList = new List<DownloadPaymentModel>();

            foreach (var item in frInvoice)
            {
                var franchiseeName = item.Franchisee.Organization.Name;
                var invoiceId = item.InvoiceId;
                var royaltyPayments = item.Invoice.InvoiceItems.Where(x => (x.ItemTypeId == (long)InvoiceItemType.AdFund || x.ItemTypeId == (long)InvoiceItemType.RoyaltyFee)).SelectMany(y => y.Invoice.InvoicePayments).Distinct().
                    OrderByDescending(x => x.PaymentId);
                if (royaltyPayments.Any())
                {
                    paymentList = CreateModel(invoiceId, franchiseeName, royaltyPayments, paymentList);
                }
            }
            paymentList = paymentList.Where(x => x.Memo.Contains("SEO Charges")).ToList();
            return paymentList;
        }


        public List<DownloadInvoiceModel> CreateDataForWebSeoInvoiceFilter(List<FranchiseeInvoice> frInvoice)
        {
            //fileName = string.Empty;
            var invoiceCollection = new List<DownloadInvoiceModel>();

            //prepare item collection
            foreach (var item in frInvoice)
            {
                var invoiceId = item.InvoiceId;
                var dueDate = item.Invoice.DueDate.Date;
                //var paymentDate = item.Invoice.InvoicePayments.FirstOrDefault().Payment.Date;
                var startDate = item != null && item.SalesDataUpload != null && item.SalesDataUpload.PeriodStartDate != null ? item.SalesDataUpload.PeriodStartDate : default(DateTime);
                var endDate = item.InvoiceDate != null ? item.InvoiceDate.Value : dueDate;
                var paymentMode = _franchiseeSalesPaymentService.GetPaymentInstrument(item.Invoice.InvoicePayments);
                if (paymentMode == null && item.Invoice.StatusId == (long)InvoiceStatus.Paid
                    && item.Invoice.InvoicePayments.Any(x => x.Payment.InstrumentTypeId == (long)InstrumentType.AccountCredit))
                {
                    paymentMode = "Account Credit";
                }
                var royaltyInvoices = item.Invoice.InvoiceItems.Where(x => (x.ItemTypeId == (long)InvoiceItemType.AdFund || x.ItemTypeId == (long)InvoiceItemType.RoyaltyFee)).ToList();

                foreach (var invoiceItem in royaltyInvoices)
                {
                    var model = _invoiceFactory.CreateModel(invoiceId, startDate, endDate, dueDate, item.Franchisee, invoiceItem, paymentMode);
                    invoiceCollection.Add(model);
                }
            }
            invoiceCollection = invoiceCollection.Where(x => x.Description.Contains("SEO Charges")).ToList();
            return invoiceCollection;
        }

        public List<DownloadInvoiceModel> CreateDataForWebSeoRoyaltyInvoice(long[] invoiceIds)
        {
            // royaltyInvoieFile = string.Empty;
            var invoiceCollection = new List<DownloadInvoiceModel>();
            var franchiseeInvoices = _franchiseeInvoiceRepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();
            var royaltyInvoices = new List<InvoiceItem>();
            //prepare item collection
            foreach (var item in franchiseeInvoices)
            {
                var isRoyality = IsLoanAdjustmentinRoyalityOrAdfund(item);
                var invoiceId = item.InvoiceId;
                var dueDate = item.Invoice.DueDate.Date;
                //var paymentDate = item.Invoice.InvoicePayments.FirstOrDefault().Payment.Date;
                var startDate = item != null && item.SalesDataUpload != null && item.SalesDataUpload.PeriodStartDate != null ? item.SalesDataUpload.PeriodStartDate : default(DateTime);
                var endDate = item.InvoiceDate != null ? item.InvoiceDate.Value : dueDate;
                var paymentMode = _franchiseeSalesPaymentService.GetPaymentInstrument(item.Invoice.InvoicePayments);
                if (paymentMode == null && item.Invoice.StatusId == (long)InvoiceStatus.Paid
                    && item.Invoice.InvoicePayments.Any(x => x.Payment.InstrumentTypeId == (long)InstrumentType.AccountCredit))
                {
                    paymentMode = "Account Credit";
                }
                if (isRoyality)
                    royaltyInvoices = item.Invoice.InvoiceItems.Where(x => !(x.ItemTypeId == (long)InvoiceItemType.RoyaltyFee) && !(x.ItemTypeId == (long)InvoiceItemType.LoanServiceFeeInterestRatePerAnnum)
                                      && !(x.ItemTypeId == (long)InvoiceItemType.LoanServiceFee)).ToList();
                else
                    royaltyInvoices = item.Invoice.InvoiceItems.Where(x => !(x.ItemTypeId == (long)InvoiceItemType.AdFund)).ToList();

                foreach (var invoiceItem in royaltyInvoices)
                {
                    var model = _invoiceFactory.CreateModel(invoiceId, startDate, endDate, dueDate, item.Franchisee, invoiceItem, paymentMode);
                    model.AdfundRoyalty = "R";
                    invoiceCollection.Add(model);
                }
            }
            invoiceCollection = invoiceCollection.Where(x => x.Description.Contains("SEO Charges")).ToList();
            return invoiceCollection;
        }
    }
}
