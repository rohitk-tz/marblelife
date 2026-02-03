using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.MarketingLead.Domain;
using Core.Notification;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class FranchiseeInvoiceGenerationPollingAgent : IFranchiseeInvoiceGenerationPollingAgent
    {
        private ILogService _logService;
        private IInvoiceService _invoiceService;
        private IRepository<SalesDataUpload> _salesDataUploadRepository;
        private IRepository<FeeProfile> _feeProfileRepository;
        private IFranchiseeInvoiceFactory _franchiseeInvoiceFactory;
        private IFranchiseeTechnicianMailService _franchiseeTechnicianMailService;
        private IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private IUnitOfWork _unitOfWork;
        private IRepository<Payment> _paymentRepository;
        private IRepository<FranchiseeTechMailService> _franchiseeTechMailServiceRepository;
        private IInvoiceItemService _invoiceItemService;
        private IInvoiceNotificationService _invoiceNotificationService;
        private IFranchiseeInfoService _franchiseeInfoService;
        private IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        private IRepository<AccountCredit> _accountCreditRepository;
        private IInvoiceLateFeePollingAgent _invoiceLateFeePollingAgent;
        private ISettings _settings;
        private IClock _clock;
        private IFranchiseeServiceFeeService _franchiseeServiceFeeService;
        private IRepository<OneTimeProjectFee> _oneTimeProjectFeeRepository;
        private IRepository<FranchiseeLoan> _franchiseeLoanRepository;
        private IRepository<OnetimeprojectfeeAddFundRoyality> _onetimeprojectfeeAddFundRoyalityRepository;
        private IRepository<Phonechargesfee> _phonechargesfeeRepository;
        private IRepository<InvoiceItem> _invoiceItemRepository;
        public FranchiseeInvoiceGenerationPollingAgent(IUnitOfWork unitOfWork, ILogService logService, IInvoiceService invoiceService,
            IFranchiseeInvoiceFactory franchiseeInvoiceFactory, IInvoiceItemService invoiceItemService, IInvoiceNotificationService invoiceNotificationService,
            IInvoiceLateFeePollingAgent invoiceLateFeePollingAgent, ISettings settings, IClock clock, IFranchiseeServiceFeeService franchiseeServiceFeeService,
            IFranchiseeTechnicianMailService franchiseeTechnicianMailService, IFranchiseeInfoService franchiseeInfoService)
        {
            _unitOfWork = unitOfWork;
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _feeProfileRepository = unitOfWork.Repository<FeeProfile>();
            _logService = logService;
            _invoiceService = invoiceService;
            _franchiseeInvoiceFactory = franchiseeInvoiceFactory;
            _invoiceItemService = invoiceItemService;
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _paymentRepository = unitOfWork.Repository<Payment>();
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _invoiceNotificationService = invoiceNotificationService;
            _franchiseeSalesPaymentRepository = unitOfWork.Repository<FranchiseeSalesPayment>();
            _accountCreditRepository = unitOfWork.Repository<AccountCredit>();
            _settings = settings;
            _clock = clock;
            _invoiceLateFeePollingAgent = invoiceLateFeePollingAgent;
            _franchiseeServiceFeeService = franchiseeServiceFeeService;
            _oneTimeProjectFeeRepository = unitOfWork.Repository<OneTimeProjectFee>();
            _franchiseeLoanRepository = unitOfWork.Repository<FranchiseeLoan>();
            _franchiseeTechnicianMailService = franchiseeTechnicianMailService;
            _franchiseeTechMailServiceRepository = unitOfWork.Repository<FranchiseeTechMailService>();
            _onetimeprojectfeeAddFundRoyalityRepository = unitOfWork.Repository<OnetimeprojectfeeAddFundRoyality>();
            _franchiseeInfoService = franchiseeInfoService;
            _phonechargesfeeRepository = unitOfWork.Repository<Phonechargesfee>();
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
        }

        public void ProcessRecords()
        {
            var salesDataCollection = GetSalesDataToParse();
            if (salesDataCollection == null || salesDataCollection.Count() < 1)
            {
                _logService.Debug("No records found.");
                return;
            }

            foreach (var record in salesDataCollection)
            {
                try
                {
                    _logService.Info(string.Format("SDU - Starting for ID - {0}  {1} - {2} ", record.Id,
                        record.PeriodStartDate.ToShortDateString(),
                        record.PeriodEndDate.ToShortDateString()));

                    _unitOfWork.StartTransaction();
                    List<FranchiseeInvoice> franchiseeInvoiceList = new List<FranchiseeInvoice>();
                    var startDate = record.PeriodStartDate;
                    var endDate = record.PeriodEndDate;
                    var royaltyLateFeeWaitDays = record.Franchisee.LateFee.RoyalityWaitPeriodInDays > 0 ? record.Franchisee.LateFee.RoyalityWaitPeriodInDays : _settings.DefaultRoyaltyLateFeeWaitPeriod;
                    var currentDate = _clock.UtcNow;

                    var invoiceModel = GetInvoiceModel(endDate, royaltyLateFeeWaitDays);

                    //get country to use curency code                   
                    var currencyExchangeRate = GetCurrencyExchangeRate(record.Franchisee, record.PeriodEndDate);
                    var allInvoiceItems = GetAllInvoiceItems(record.Franchisee, record.Id, startDate, endDate).ToList();
                    var royalityinvoiceModel = GetInvoiceModel(endDate, royaltyLateFeeWaitDays);
                    royalityinvoiceModel.InvoiceItems = allInvoiceItems.Where(x => x.ItemTypeId != (long)InvoiceItemType.AdFund).ToList();
                    royalityinvoiceModel.InvoiceItems.Select(x => x.CurrencyExchangeRateId = currencyExchangeRate.Id).ToList();
                    royalityinvoiceModel.Payments.Select(x => x.CurrencyExchangeRateId = currencyExchangeRate.Id).ToList();

                    //Add Charges In Royalty
                    if (royalityinvoiceModel.InvoiceItems.Any())
                    {
                        var invoice = _invoiceService.Save(royalityinvoiceModel);
                        var franchiseeInvoice = _franchiseeInvoiceFactory.CreateDomain(record.FranchiseeId, invoice.Id, record.Id, record.PeriodEndDate);
                        franchiseeInvoice.Franchisee = record.Franchisee;
                        _franchiseeInvoiceRepository.Save(franchiseeInvoice);
                        franchiseeInvoiceList.Add(franchiseeInvoice);

                        var invoiceDate = franchiseeInvoice.Invoice.DueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                        var utcDate = _clock.ToUtc(invoiceDate);

                        if (royalityinvoiceModel.InvoiceItems.Sum(x => x.Amount) > 0 && franchiseeInvoice.Franchisee.LateFee != null
                            && franchiseeInvoice.Franchisee.LateFee.RoyalityLateFee > 0
                            && franchiseeInvoice.Invoice.DueDate >= _settings.LateFeeStartDate && utcDate < currentDate)
                        {
                            _invoiceLateFeePollingAgent.SaveRoyalityLateFee(franchiseeInvoice, DateTime.UtcNow);
                        }
                        //Save Franchisee Service Fee
                        SaveFranchiseeServiceFee(franchiseeInvoice);
                        SaveFranchiseeServiceFeePhoneCallCharges(franchiseeInvoice);
                        var invoiceItems = _invoiceItemRepository.Table.Where(x => x.InvoiceId == franchiseeInvoice.InvoiceId).ToList();
                        if (invoiceItems.Sum(x => x.Amount) < 0)
                            franchiseeInvoice.Invoice.StatusId = (long)InvoiceStatus.Paid;
                        else if (invoiceItems.Sum(x => x.Amount) == 0)
                            franchiseeInvoice.Invoice.StatusId = (long)InvoiceStatus.ZeroDue;
                    }

                    var adfundinvoiceModel = GetInvoiceModel(endDate, royaltyLateFeeWaitDays);
                    adfundinvoiceModel.InvoiceItems = allInvoiceItems.Where(x => x.ItemTypeId == (long)InvoiceItemType.AdFund).ToList();
                    adfundinvoiceModel.InvoiceItems.Select(x => x.CurrencyExchangeRateId = currencyExchangeRate.Id).ToList();
                    adfundinvoiceModel.Payments.Select(x => x.CurrencyExchangeRateId = currencyExchangeRate.Id).ToList();

                    //Add Charges In AdFund
                    if (adfundinvoiceModel.InvoiceItems.Any())
                    {
                        var adfundinvoice = _invoiceService.Save(adfundinvoiceModel);
                        var franchiseeadfundInvoice = _franchiseeInvoiceFactory.CreateDomain(record.FranchiseeId, adfundinvoice.Id, record.Id, record.PeriodEndDate);
                        franchiseeadfundInvoice.Franchisee = record.Franchisee;
                        _franchiseeInvoiceRepository.Save(franchiseeadfundInvoice);

                        franchiseeInvoiceList.Add(franchiseeadfundInvoice);

                        SaveFranchiseeServiceFeeForAdfund(franchiseeadfundInvoice);
                        SaveFranchiseeEmailFeeForAdfund(record, franchiseeadfundInvoice);
                        SaveFranchiseeServiceFeeOneTimeProject(franchiseeadfundInvoice);
                        SaveFranchiseeSeoChargesForAdFund(franchiseeadfundInvoice);

                        var invoiceItems = _invoiceItemRepository.Table.Where(x => x.InvoiceId == franchiseeadfundInvoice.InvoiceId).ToList();
                        if (invoiceItems.Sum(x => x.Amount) < 0)
                            franchiseeadfundInvoice.Invoice.StatusId = (long)InvoiceStatus.Paid;
                        else if (invoiceItems.Sum(x => x.Amount) == 0)
                            franchiseeadfundInvoice.Invoice.StatusId = (long)InvoiceStatus.ZeroDue;
                    }

                    record.IsInvoiceGenerated = true;
                    _salesDataUploadRepository.Save(record);

                    _unitOfWork.SaveChanges();

                    _unitOfWork.StartTransaction();
                    if (franchiseeInvoiceList != null && franchiseeInvoiceList.Count() > 0)
                        _invoiceNotificationService.CreateInvoiceDetailNotification(franchiseeInvoiceList, record.FranchiseeId);
                    _unitOfWork.SaveChanges();

                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logService.Error(ex);
                }
            }
        }

        private void SaveFranchiseeServiceFee(FranchiseeInvoice franchiseeInvoice)
        {
            var isInRoyality = true;
            var isSEOInRoyality = true;
            try
            {

                if (franchiseeInvoice.Franchisee.FranchiseeServiceFee != null && franchiseeInvoice.SalesDataUpload.PaidAmount > 0 && franchiseeInvoice.Franchisee.FranchiseeServiceFee.Any(x => x.IsActive))
                {
                    _franchiseeServiceFeeService.SaveServiceFeeItem(franchiseeInvoice);
                }
                var otpList = _oneTimeProjectFeeRepository.Table.Where(x => x.Amount > 0 && x.InvoiceItemId == null
                && x.FranchiseeId == franchiseeInvoice.FranchiseeId).ToList();
                var oneTimeProjectAdFundRoyality = _onetimeprojectfeeAddFundRoyalityRepository.Table.FirstOrDefault(x => x.FranchiseeId == franchiseeInvoice.FranchiseeId);

                if (oneTimeProjectAdFundRoyality != null)
                {
                    isInRoyality = oneTimeProjectAdFundRoyality.IsInRoyality;
                }
                if (oneTimeProjectAdFundRoyality != null)
                {
                    isSEOInRoyality = oneTimeProjectAdFundRoyality.IsSEOInRoyalty;
                }
                if (otpList.Any() && isInRoyality && franchiseeInvoice.SalesDataUpload.PaidAmount > 0)
                {
                    _franchiseeServiceFeeService.SaveOneTimeProjectFee(franchiseeInvoice, otpList);
                }
                if (isSEOInRoyality && franchiseeInvoice.SalesDataUpload.PaidAmount >= 0)
                {
                    SaveFranchiseeSeoChargesForRoyalty(franchiseeInvoice);
                }
                var franchiseeLoanList = _franchiseeLoanRepository.Table.Where(x => x.FranchiseeId == franchiseeInvoice.FranchiseeId && !x.IsCompleted.Value).ToList();
                if (franchiseeLoanList.Any())
                {
                    foreach (var franchiseeLoan in franchiseeLoanList)
                    {
                        if (franchiseeLoan.IsRoyality.Value)
                        {
                            var loanPayment = franchiseeLoan.FranchiseeLoanSchedule.Where(x => x.PayableAmount > 0 && x.InvoiceItemId == null
                                                            && x.DueDate >= franchiseeInvoice.SalesDataUpload.PeriodStartDate.Date && x.DueDate <= franchiseeInvoice.SalesDataUpload.PeriodEndDate.Date
                                                            && !x.IsPrePaid).OrderBy(x => x.Id).FirstOrDefault();
                            if (loanPayment != null)
                            {
                                _franchiseeServiceFeeService.SaveLoanInvoiceItem(franchiseeInvoice, loanPayment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }
        }

        private void SaveFranchiseeServiceFeeForAdfund(FranchiseeInvoice franchiseeInvoice)
        {
            try
            {
                var franchiseeLoanList = _franchiseeLoanRepository.Table.Where(x => x.FranchiseeId == franchiseeInvoice.FranchiseeId).ToList();
                if (franchiseeLoanList.Any())
                {
                    foreach (var franchiseeLoan in franchiseeLoanList)
                    {
                        if (!franchiseeLoan.IsRoyality.Value)
                        {
                            var loanPayment = franchiseeLoan.FranchiseeLoanSchedule.Where(x => x.PayableAmount > 0 && x.InvoiceItemId == null
                                                            && x.DueDate >= franchiseeInvoice.SalesDataUpload.PeriodStartDate.Date && x.DueDate <= franchiseeInvoice.SalesDataUpload.PeriodEndDate.Date
                                                            && !x.IsPrePaid).OrderBy(x => x.Id).FirstOrDefault();
                            if (loanPayment != null)
                            {
                                _franchiseeServiceFeeService.SaveLoanInvoiceItem(franchiseeInvoice, loanPayment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }
        }


        private void SaveFranchiseeSeoChargesForRoyalty(FranchiseeInvoice franchiseeInvoice)
        {
            var isSEOInRoyalty = true;
            var oneTimeProjectAdFundRoyality = _onetimeprojectfeeAddFundRoyalityRepository.Table.FirstOrDefault(x => x.FranchiseeId == franchiseeInvoice.FranchiseeId);
            if (oneTimeProjectAdFundRoyality != null)
            {
                isSEOInRoyalty = oneTimeProjectAdFundRoyality.IsSEOInRoyalty;
            }
            if (isSEOInRoyalty && franchiseeInvoice.SalesDataUpload.PaidAmount >= 0)
            {
                _franchiseeServiceFeeService.SaveServiceFeeItemForSeo(franchiseeInvoice);
            }
        }
        private void SaveFranchiseeSeoChargesForAdFund(FranchiseeInvoice franchiseeInvoice)
        {
            var isSEOInRoyalty = true;
            var oneTimeProjectAdFundRoyality = _onetimeprojectfeeAddFundRoyalityRepository.Table.FirstOrDefault(x => x.FranchiseeId == franchiseeInvoice.FranchiseeId);
            if (oneTimeProjectAdFundRoyality != null)
            {
                isSEOInRoyalty = oneTimeProjectAdFundRoyality.IsSEOInRoyalty;
            }
            if (!isSEOInRoyalty && franchiseeInvoice.SalesDataUpload.PaidAmount >= 0)
            {
                _franchiseeServiceFeeService.SaveServiceFeeItemForSeo(franchiseeInvoice);
            }
        }
        private void SaveFranchiseeServiceFeeOneTimeProject(FranchiseeInvoice franchiseeInvoice)
        {
            try
            {
                var isInRoyality = true;
                var otpList = _oneTimeProjectFeeRepository.Table.Where(x => x.Amount > 0 && x.InvoiceItemId == null
                   && x.FranchiseeId == franchiseeInvoice.FranchiseeId).ToList();
                var oneTimeProjectAdFundRoyality = _onetimeprojectfeeAddFundRoyalityRepository.Table.FirstOrDefault(x => x.FranchiseeId == franchiseeInvoice.FranchiseeId);

                if (oneTimeProjectAdFundRoyality != null)
                {
                    isInRoyality = oneTimeProjectAdFundRoyality.IsInRoyality;
                }
                if (otpList.Any() && !isInRoyality && franchiseeInvoice.SalesDataUpload.PaidAmount > 0)
                {
                    _franchiseeServiceFeeService.SaveOneTimeProjectFee(franchiseeInvoice, otpList);
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }
        }

        private void SaveFranchiseeServiceFeePhoneCallCharges(FranchiseeInvoice franchiseeInvoice)
        {
            try
            {
                var phoneChargeFeesList = _phonechargesfeeRepository.Table.Where(x => x.FranchiseeId == franchiseeInvoice.FranchiseeId && x.IsInvoiceInQueue && x.FranchiseeServiceFee != null).ToList();
                if (phoneChargeFeesList.Count() > 0 && franchiseeInvoice.SalesDataUpload.PaidAmount > 0)
                {
                    _franchiseeServiceFeeService.SavePhoneChargeFess(franchiseeInvoice, phoneChargeFeesList);
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }

        }

        private ICollection<InvoiceItemEditModel> GetAllInvoiceItems(Franchisee franchisee, long salesDataUploadId, DateTime startDate, DateTime endDate)
        {
            var invoiceItems = new List<InvoiceItemEditModel>();
            var periodStructure = PreparePeriodStructure(franchisee, startDate, endDate, salesDataUploadId);

            foreach (var yearSalesData in periodStructure)
            {
                foreach (var monthSalesData in yearSalesData.MonthWiseSalesCollection)
                {
                    invoiceItems.AddRange(GetInvoiceItemsByFeeProfile(franchisee, monthSalesData.Sales, franchisee.FeeProfile, monthSalesData.YTDSales, monthSalesData.StartDate, monthSalesData.EndDate));
                }
            }

            return invoiceItems;
        }

        private decimal YearToDaySales(Franchisee franchisee, DateTime toDate)
        {
            var startDayOfYear = new DateTime(toDate.Year, 1, 1);
            return PaymentSalesBetweenPeriod(franchisee, startDayOfYear, toDate);
        }

        private decimal PaymentSalesForSalesDataUpload(Franchisee franchisee, long salesDataUploadId, DateTime fromDate, DateTime toDate)
        {
            var servicesOffered = franchisee.FranchiseeServices.Where(fs => fs.CalculateRoyalty == true && fs.IsActive == true).Select(fs => fs.ServiceType.Id).ToList();

            decimal accountCreditAmount = GetCreditAmount(franchisee, fromDate, toDate, salesDataUploadId);

            var paymentIds = _franchiseeSalesPaymentRepository.Table.Where(x => x.SalesDataUploadId == salesDataUploadId && x.FranchiseeSales.FranchiseeId == franchisee.Id)
                .Select(x => x.Payment.Id).Distinct();

            if (!servicesOffered.Contains((long)ServiceTypes.Stonelife)) { servicesOffered.Add((long)ServiceTypes.Stonelife); }

            var paymentAmounts = _paymentRepository.Table.Where(x => x.Date >= fromDate && x.Date <= toDate
                                    && x.PaymentItems.Any(ii => servicesOffered.Contains(ii.ItemId) && (ii.ItemTypeId == (long)InvoiceItemType.Service || ii.ItemTypeId == (long)InvoiceItemType.Discount))
                                    && paymentIds.Contains(x.Id))
                                .Select(m => m.Amount);

            if (paymentAmounts.Count() < 1) return 0;
            var amount = paymentAmounts.Sum() - accountCreditAmount;
            return amount;
        }

        private decimal GetCreditAmount(Franchisee franchisee, DateTime fromDate, DateTime toDate, long salesDataUploadId = 0)
        {
            var accountCreditIds = _franchiseeSalesRepository.Table.Where(x => x.FranchiseeId == franchisee.Id
                                    && (salesDataUploadId <= 0 || x.SalesDataUploadId == salesDataUploadId)
                                    && x.AccountCreditId != null)
                                      .Select(x => x.AccountCreditId).Distinct().ToArray();


            if (accountCreditIds.Any())
            {
                var accountCreditItems = _accountCreditRepository.Table.Where(x => x.CreditedOn >= fromDate && x.CreditedOn <= toDate
                                         && accountCreditIds.Contains(x.Id)).SelectMany(x => x.CreditMemoItems);

                return accountCreditItems == null || !accountCreditItems.Any() ? 0 : accountCreditItems.Sum(x => x.Amount);
            }
            return 0;
        }

        private decimal PaymentSalesBetweenPeriod(Franchisee franchisee, DateTime fromDate, DateTime toDate)
        {
            var servicesOffered = franchisee.FranchiseeServices.Where(fs => fs.CalculateRoyalty == true && fs.IsActive == true).Select(fs => fs.ServiceType.Id).ToList();
            //var invoices = _franchiseeInvoiceRepository.Table.Where(x => x.FranchiseeId == franchisee.Id).Select(x => x.InvoiceId);
            //var paymentIds = _franchiseeSalesRepository.Table.Where(x => x.FranchiseeId == franchisee.Id).SelectMany(x => x.Invoice.InvoicePayments.Select(p => p.PaymentId));
            decimal accountCreditAmount = GetCreditAmount(franchisee, fromDate, toDate);

            var franchiseeSalesIds = _franchiseeSalesRepository.Table.Where(x => x.FranchiseeId == franchisee.Id).Select(x => x.Id);

            if (franchiseeSalesIds.Count() <= 0)
                return 0;

            var paymentIds = _franchiseeSalesPaymentRepository.Table.Where(x => franchiseeSalesIds.Contains(x.FranchiseeSalesId))
                .Select(p => p.PaymentId).Distinct();

            if (!servicesOffered.Contains((long)ServiceTypes.Stonelife)) { servicesOffered.Add((long)ServiceTypes.Stonelife); }

            // This generates 2 level deep nested query with parent id in there, not getting executed
            var paymentAmounts = _paymentRepository.Table.Where(x => x.Date >= fromDate && x.Date <= toDate
                                    && x.PaymentItems.Any(ii => servicesOffered.Contains(ii.ItemId) && (ii.ItemTypeId == (long)InvoiceItemType.Service || ii.ItemTypeId == (long)InvoiceItemType.Discount))
                                    //&& x.Invoices.Any(i => invoices.Any(m => m == i.Id))
                                    && paymentIds.Contains(x.Id)
                                    )
                                .Select(m => m.Amount);

            if (paymentAmounts.Count() < 1) return 0;
            var amount = paymentAmounts.Sum() - accountCreditAmount;
            return amount;
        }

        private InvoiceEditModel GetInvoiceModel(DateTime endDate, long royaltyLateFeeWaitDays)
        {
            var invoice = new InvoiceEditModel();
            invoice.GeneratedOn = DateTime.UtcNow;
            invoice.DueDate = endDate.AddDays(royaltyLateFeeWaitDays); //To Do : Considering due date after 2 days of bill date
            invoice.StatusId = (long)InvoiceStatus.Unpaid;
            invoice.DataRecorderMetaData = new DataRecorderMetaData(DateTime.UtcNow);
            return invoice;
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
                var minRoyalityAsPerDocument = _franchiseeInfoService.GetMinimumRoyalityForFranchisee(feeProfile.Franchisee.Id);
                franchisee.IsMinRoyalityFixed = false;
                if (!franchisee.IsMinRoyalityFixed)
                {
                    minRoyalty = minRoyalityAsPerDocument;
                }

                var adFundPercentage = feeProfile.AdFundPercentage;

                CreateAdFundInvoiceItem(paidAmount ?? 0, invoiceItems, adFundPercentage, startDate, endDate);

                var royaltyCharges = GetSlabPercentageForPaidAmount(yearToDaySales, paidAmount, feeProfile.RoyaltyFeeSlabs);
                CreateRoyaltyInvoiceItem(invoiceItems, minRoyalty, royaltyCharges, startDate, endDate, franchisee.Id);
            }
            return invoiceItems;
        }

        private void CreateRoyaltyInvoiceItem(List<InvoiceItemEditModel> invoiceItems, decimal minRoyalty, IList<RoyaltyCharge> royaltyCharges,
            DateTime startDate, DateTime endDate, long franchiseeId)
        {
            InvoiceItemEditModel royaltyFeeInvoiceItem;
            //if (royaltyCharges.Count < 1)
            //{
            //    royaltyFeeInvoiceItem = CreateRoyaltyInvoiceItem(minRoyalty, string.Format("Charged Minimum Royalty in the absence of any Sales"));
            //    invoiceItems.Add(royaltyFeeInvoiceItem);
            //    return;
            //}

            foreach (var charge in royaltyCharges)
            {
                royaltyFeeInvoiceItem = CreateRoyaltyInvoiceItem(charge.Royalty, string.Format("Charging {0}% as Royalty Amount for Customer Payments ${1}. Period- {2:MM/dd/yyyy} to {3:MM/dd/yyyy}",
                    charge.Percentage, charge.Amount, startDate, endDate), charge.Percentage, startDate, endDate, charge.Amount);

                invoiceItems.Add(royaltyFeeInvoiceItem);
            }


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
            description = "Sales - Royalties" + " " + description;
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

        private static void CreateAdFundInvoiceItem(decimal paidAmount, List<InvoiceItemEditModel> invoiceItems, decimal adFundPercentage, DateTime startDate, DateTime endDate)
        {

            var adFundInvoiceItem = new AdFundInvoiceItemEditModel();
            adFundInvoiceItem.ItemTypeId = (long)InvoiceItemType.AdFund;
            adFundInvoiceItem.Amount = (paidAmount * adFundPercentage) / 100;
            adFundInvoiceItem.Quantity = 1;
            adFundInvoiceItem.Rate = adFundInvoiceItem.Amount;
            adFundInvoiceItem.Description = string.Format("Royalties Charging {0}% as AdFund for Customer Payments ${1}. Period- {2:MM/dd/yyyy} to {3:MM/dd/yyyy}", adFundPercentage, paidAmount, startDate, endDate);
            adFundInvoiceItem.Percentage = adFundPercentage;
            adFundInvoiceItem.StartDate = startDate;
            adFundInvoiceItem.EndDate = endDate;
            adFundInvoiceItem.SalesAmount = paidAmount;
            invoiceItems.Add(adFundInvoiceItem);
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

        private IEnumerable<SalesDataUpload> GetSalesDataToParse()
        {
            //Check if parsed and not exist in FranchiseeInvoice
            var salesDataUploadIds = _franchiseeInvoiceRepository.Table.Where(x => x.SalesDataUploadId != null).Select(x => x.SalesDataUploadId);
            var bools = salesDataUploadIds.Contains((long)3110);
            return _salesDataUploadRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Parsed
                    && x.PeriodStartDate.Year >= 2016 // Not to Process Invoice for previous year records
                    && x.IsActive // Parse only Active Files
                                  //&& (x.TotalAmount > 0 || x.PaidAmount > 0) //Need to generate $0 invoices
                    && !x.IsInvoiceGenerated
                    && !salesDataUploadIds.Contains(x.Id)).OrderBy(x => x.PeriodStartDate).ToList();
        }

        private ICollection<YearSalesData> PreparePeriodStructure(Franchisee franchisee, DateTime startDate, DateTime endDate, long salesDataUploadId)
        {
            var collection = new List<YearSalesData>();
            for (int yearIndex = startDate.Year; yearIndex <= endDate.Year; yearIndex++)
            {
                var model = new YearSalesData
                {
                    Year = yearIndex,
                    YTDSales = 0
                };

                var startDateYear = new DateTime(yearIndex, 1, 1);
                var endDateYear = new DateTime(yearIndex + 1, 1, 1).AddMinutes(-1);

                if (startDateYear < startDate)
                {
                    model.YTDSales = YearToDaySales(franchisee, startDate.AddDays(-1));
                    startDateYear = startDate;
                }

                if (endDateYear >= endDate)
                {
                    endDateYear = endDate;
                }

                PrepareMonthSalesDataModel(franchisee, yearIndex, model, startDateYear, endDateYear, model.YTDSales, salesDataUploadId);

                collection.Add(model);
            }

            return collection;
        }

        private void PrepareMonthSalesDataModel(Franchisee franchisee, int currentYear, YearSalesData yearSalesDataModel, DateTime startDateYear, DateTime endDateYear,
            decimal ytdSales, long salesDataUploadId)
        {
            for (int monthIndex = startDateYear.Month; monthIndex <= endDateYear.Month; monthIndex++)
            {
                var startDateMonth = new DateTime(currentYear, monthIndex, 1);
                var endDateMonth = startDateMonth.AddMonths(1).AddMinutes(-1);
                var isMonthEnd = true;

                if (endDateMonth > endDateYear)
                {
                    endDateMonth = endDateYear;
                    isMonthEnd = false;
                }

                if (startDateMonth < startDateYear)
                {
                    startDateMonth = startDateYear;
                }

                var monthModel = new MonthSalesData
                {
                    Month = monthIndex,
                    YTDSales = ytdSales,
                    StartDate = startDateMonth,
                    EndDate = endDateMonth,
                    Sales = PaymentSalesForSalesDataUpload(franchisee, salesDataUploadId, startDateMonth, endDateMonth),
                    IsEndOfMonth = isMonthEnd
                };

                ytdSales += monthModel.Sales;
                yearSalesDataModel.MonthWiseSalesCollection.Add(monthModel);
            }
        }


        private void SaveFranchiseeEmailFeeForAdfund(SalesDataUpload salesDataUpload, FranchiseeInvoice franchiseeInvoice)
        {
            var currentMonth = salesDataUpload.PeriodStartDate.Month;
            var currentYear = salesDataUpload.PeriodStartDate.Year;
            var monthStartDate = new DateTime(currentYear, currentMonth, 1);
            var monthEndDate = new DateTime(currentYear, currentMonth, DateTime.DaysInMonth(currentYear, currentMonth));
            var periodStartDate = salesDataUpload.PeriodStartDate;
            var periodEndDate = salesDataUpload.PeriodEndDate;
            if (periodStartDate <= monthEndDate && periodEndDate >= monthEndDate && salesDataUpload.PaidAmount > 0)
            {
                var franchiseeTechMailServieDomain = _franchiseeTechMailServiceRepository.Table.FirstOrDefault(x => x.FranchiseeId == salesDataUpload.FranchiseeId);
                if (franchiseeTechMailServieDomain != null)
                {
                    var invoiceItemEditModel = _franchiseeTechnicianMailService.CreateModel(franchiseeTechMailServieDomain, franchiseeInvoice, salesDataUpload.PeriodStartDate, salesDataUpload.PeriodEndDate);
                    var bools = _franchiseeTechnicianMailService.Save(invoiceItemEditModel, franchiseeTechMailServieDomain, invoiceItemEditModel.InvoiceId);
                }
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

        class YearSalesData
        {
            public int Year { get; set; }
            public decimal YTDSales { get; set; }
            public ICollection<MonthSalesData> MonthWiseSalesCollection { get; set; }
            public YearSalesData()
            {
                MonthWiseSalesCollection = new List<MonthSalesData>();
            }
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
                _logService.Debug("No Currency Code found for franchisee" + franchisee.OwnerName);
                return currencyExchangeRate;
            }
        }
    }
}
