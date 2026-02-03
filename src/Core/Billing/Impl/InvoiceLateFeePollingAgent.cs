using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Notification;
using Core.Notification.ViewModel;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Sales;
using Core.Sales.Domain;
using Core.Sales.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class InvoiceLateFeePollingAgent : IInvoiceLateFeePollingAgent
    {
        private IUnitOfWork _unitOfWork;
        private IClock _clock;
        private ILogService _logService;
        private InvoiceFactory _invoicefactory;
        private InvoiceItemFactory _invoiceItemfactory;
        private IRepository<Franchisee> _franchiseeRepository;
        private IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private IRepository<Invoice> _invoiceRepository;
        private IRepository<InvoiceItem> _invoiceItemRepository;
        private ILateFeeNotificationService _lateFeeNotificationService;
        private IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private IFranchiseeInvoiceFactory _franchiseeInvoiceFactory;
        private ISettings _settings;
        private ISalesDataUploadCreateModelValidator _salesDataUploadCreateModelValidator;
        private ISalesDataUploadService _salesDataUploadService;

        public InvoiceLateFeePollingAgent(IUnitOfWork unitOfWork, IClock clock, ILogService logService, InvoiceFactory invoicefactory,
            InvoiceItemFactory invoiceItemfactory, ILateFeeNotificationService lateFeeNotificationService, IFranchiseeInvoiceFactory franchiseeInvoiceFactory,
            ISettings settings, ISalesDataUploadCreateModelValidator salesDataUploadCreateModalValidator, ISalesDataUploadService salesDataUploadService)
        {
            _unitOfWork = unitOfWork;
            _clock = clock;
            _logService = logService;
            _invoicefactory = invoicefactory;
            _invoiceItemfactory = invoiceItemfactory;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _invoiceRepository = unitOfWork.Repository<Invoice>();
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _lateFeeNotificationService = lateFeeNotificationService;
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _franchiseeInvoiceFactory = franchiseeInvoiceFactory;
            _settings = settings;
            _salesDataUploadCreateModelValidator = salesDataUploadCreateModalValidator;
            _salesDataUploadService = salesDataUploadService;
        }
        public void LateFeeGenerator()
        {
            var currenctdate = _clock.UtcNow;

            if (ApplicationManager.Settings.ApplyLateFee == false)
            {
                _logService.Info("Late Fee Generation turned off");
                return;
            }

            // sales data late fee
            _logService.Info("start adding SalesData  late fee.");

            SalesDateLateFee();

            _logService.Info("end adding SalesData  late fee.");

            // royality late fee
            var franchiseeopenInvoiceCollection = GetFranchiseeUnpaidInvoice(currenctdate.Date);
            if (franchiseeopenInvoiceCollection == null || franchiseeopenInvoiceCollection.Count() < 1)
            {
                _logService.Info("No franchisee Invoice  found.");

            }
            _logService.Info("start adding  Royalty late fee.");
            foreach (var item in franchiseeopenInvoiceCollection)
            {
                SaveRoyalityLateFee(item, currenctdate);
            }
            _logService.Info("End Royalty late fee.");

        }
        private IEnumerable<FranchiseeInvoice> GetFranchiseeUnpaidInvoice(DateTime currenctdate)
        {
            var franchiseeUnpaidInvoices = (from ii in _invoiceItemRepository.Table
                                            join fi in _franchiseeInvoiceRepository.Table on ii.InvoiceId equals fi.InvoiceId
                                            where ii.ItemTypeId == (long)InvoiceItemType.RoyaltyFee && fi.Invoice.StatusId == (long)InvoiceStatus.Unpaid && currenctdate > fi.Invoice.DueDate
                   && (fi.Franchisee.LateFee != null && fi.Franchisee.LateFee.RoyalityLateFee > 0
                   || (fi.Franchisee.LateFee.RoyalityWaitPeriodInDays > 0 && fi.Franchisee.LateFee.RoyalityInterestRatePercentagePerAnnum > 0)
                   && (ii.Invoice.InvoiceItems.Count() > 0 && ii.Invoice.InvoiceItems.Sum(x => x.Amount) > 0))
                                            select fi).Distinct().ToList();

            var finalcollection = franchiseeUnpaidInvoices.Where(x => (x.Invoice.DueDate >= _settings.LateFeeStartDate)
                                    && x.Franchisee.Organization.IsActive).Select(y => y).ToList();
            return finalcollection;

        }
        public void SaveRoyalityLateFee(FranchiseeInvoice item, DateTime currenctdate)
        {
            if (!item.Invoice.InvoiceItems.Any(x => x.ItemTypeId == (long)InvoiceItemType.LateFees && x.LateFeeInvoiceItem.LateFeeTypeId == (long)LateFeeType.Royalty))
            {
                SaveRoyalityLateFeeInvoiceItem(item, currenctdate);
            }

            _logService.Info(string.Format("Params - {0} && {1} && {2}", item.Franchisee.LateFee.RoyalityInterestRatePercentagePerAnnum,
                currenctdate.Date, item.Invoice.DueDate.AddDays(2).Date));

            if (item.Franchisee.LateFee.RoyalityInterestRatePercentagePerAnnum > 0 && (currenctdate.Date >= item.Invoice.DueDate.AddDays(2).Date))
            {
                _logService.Info(string.Format("Start Adding Late Fee  Interest Rate{0}  {1} - {2} ", item.Id, item.FranchiseeId, item.InvoiceId));
                try
                {
                    var invoiceitemeditmodel = RoyalityLateFeeInterestRateInvoiceItem(item, currenctdate);
                    _unitOfWork.StartTransaction();

                    var invoiceitemdomain = _invoiceItemfactory.CreateDomain(invoiceitemeditmodel);
                    _invoiceItemRepository.Save(invoiceitemdomain);

                    _unitOfWork.SaveChanges();
                    _logService.Info(string.Format("End Adding Late Fee Interest Rate {0}  {1} - {2} ", item.Id, item.FranchiseeId, item.InvoiceId));
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logService.Error(ex);
                }
            }
        }
        private InvoiceItemEditModel RoyalityLateFeeInvoiceItem(FranchiseeInvoice item, DateTime currenctdate)
        {
            var invoiceitemeditmodel = new LateFeeInvoiceItemEditModel();
            invoiceitemeditmodel.Amount = item.Franchisee.LateFee.RoyalityLateFee;
            invoiceitemeditmodel.ItemTypeId = (long)InvoiceItemType.LateFees;
            invoiceitemeditmodel.InvoiceId = item.InvoiceId;
            invoiceitemeditmodel.Quantity = 1;
            invoiceitemeditmodel.Rate = item.Franchisee.LateFee.RoyalityLateFee;
            invoiceitemeditmodel.Description = "Sales - Late Fees Sales - Late Fees Royalty Late Fee charged on " + currenctdate.Date.ToString("MM/dd/yyyy");

            invoiceitemeditmodel.SalesAmount = item.Franchisee.LateFee.RoyalityLateFee;
            invoiceitemeditmodel.LateFeeTypeId = (long)LateFeeType.Royalty;
            invoiceitemeditmodel.WaitPeriod = item.Franchisee.LateFee.RoyalityWaitPeriodInDays;
            invoiceitemeditmodel.StartDate = item.SalesDataUpload.PeriodStartDate;
            invoiceitemeditmodel.EndDate = item.SalesDataUpload.PeriodEndDate;
            invoiceitemeditmodel.CurrencyExchangeRateId = item.SalesDataUpload.CurrencyExchangeRateId;
            invoiceitemeditmodel.GeneratedOn = currenctdate;
            return invoiceitemeditmodel;
        }
        private InvoiceItemEditModel RoyalityLateFeeInterestRateInvoiceItem(FranchiseeInvoice item, DateTime currenctdate)
        {
            var lateFeeInvoiceItem = item.Invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)InvoiceItemType.InterestRatePerAnnum).OrderByDescending(x => x.Id).FirstOrDefault();

            int lateFeeNoOfDays = (int)(currenctdate - item.Invoice.DueDate.AddDays(1)).TotalDays;

            decimal amount = item.Invoice.InvoiceItems.Where(x => x.ItemTypeId != (long)InvoiceItemType.InterestRatePerAnnum).Sum(x => x.Amount);
            decimal perdaylatefee = 0;

            var invoiceitemeditmodel = new InterestRateInvoiceItemEditModel();

            invoiceitemeditmodel.ItemTypeId = (long)InvoiceItemType.InterestRatePerAnnum;
            invoiceitemeditmodel.InvoiceId = item.InvoiceId;
            invoiceitemeditmodel.Description = "Interest - Royalty Late Fee Interest Rate charged on " + currenctdate.Date.ToString("MM/dd/yyyy");
            invoiceitemeditmodel.EndDate = currenctdate;
            invoiceitemeditmodel.Quantity = 1;

            invoiceitemeditmodel.WaitPeriod = item.Franchisee.LateFee.RoyalityWaitPeriodInDays;
            invoiceitemeditmodel.Percentage = item.Franchisee.LateFee.RoyalityInterestRatePercentagePerAnnum;
            invoiceitemeditmodel.CurrencyExchangeRateId = item.SalesDataUpload.CurrencyExchangeRateId;

            if (lateFeeNoOfDays > 0 && item.Franchisee.LateFee.RoyalityInterestRatePercentagePerAnnum > 0)
            {
                perdaylatefee = (((amount * item.Franchisee.LateFee.RoyalityInterestRatePercentagePerAnnum * lateFeeNoOfDays / 100)) / 365);// late fee per day                        
            }

            if (lateFeeInvoiceItem != null)
            {
                invoiceitemeditmodel.Id = lateFeeInvoiceItem.Id;
                invoiceitemeditmodel.Amount = perdaylatefee;
                invoiceitemeditmodel.StartDate = lateFeeInvoiceItem.InterestRateInvoiceItem.StartDate;
                invoiceitemeditmodel.SalesAmount = lateFeeInvoiceItem.InterestRateInvoiceItem.Amount;
            }
            else
            {
                invoiceitemeditmodel.StartDate = item.Invoice.DueDate.AddDays(2);
                invoiceitemeditmodel.Amount = perdaylatefee;
                invoiceitemeditmodel.SalesAmount = amount;
            }
            invoiceitemeditmodel.Rate = invoiceitemeditmodel.Amount;

            return invoiceitemeditmodel;

        }
        private void SaveRoyalityLateFeeInvoiceItem(FranchiseeInvoice item, DateTime currenctdate)
        {
            try
            {
                var latefeeinvoiceitemeditmodel = RoyalityLateFeeInvoiceItem(item, currenctdate);

                _logService.Info(string.Format("Start Adding Late Fee {0}  {1} - {2} ", item.Id, item.FranchiseeId, item.InvoiceId));

                _unitOfWork.StartTransaction();

                var latefeeinvoiceitemdomain = _invoiceItemfactory.CreateDomain(latefeeinvoiceitemeditmodel);
                _invoiceItemRepository.Save(latefeeinvoiceitemdomain);

                _lateFeeNotificationService.CreateLateFeeNotification(latefeeinvoiceitemdomain, item.Franchisee.Organization.Id, currenctdate);

                _unitOfWork.SaveChanges();
                _logService.Info(string.Format("End Adding Late Fee {0}  {1} - {2} ", item.Id, item.FranchiseeId, item.InvoiceId));

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }

        }
        private void SalesDateLateFee()
        {
            try
            {
                var currenctdate = _clock.UtcNow.Date;
                var franchiseeList = _franchiseeRepository.Table.Where(x => x.LateFee.SalesDataLateFee > 0 && x.Organization.IsActive).ToList();
                foreach (var franchisee in franchiseeList)
                {
                    var salesDataUploadList = franchisee.SalesDataUploads.Where(x => x.IsActive);

                    if (!salesDataUploadList.Any())
                    {
                        _logService.Debug("No records found.");
                        continue;
                    }
                    var currentDate = _clock.UtcNow;

                    var lastUpload = salesDataUploadList.Where(x => x.StatusId != (long)SalesDataUploadStatus.Failed)
                        .OrderByDescending(x => x.PeriodEndDate).FirstOrDefault();
                    if (lastUpload == null)
                        continue;
                    var feeProfile = franchisee.FeeProfile;

                    var lastUploadStartdate = lastUpload.PeriodStartDate.Date;
                    var lastUploadEndDate = lastUpload.PeriodEndDate.Date;

                    var startEndDateList = new List<StartEndDateViewModel>();
                    //var salesDataLateFeeinvoices = franchisee.FranchiseeInvoices.SelectMany(x => x.Invoice.InvoiceItems
                    //                                .Where(y => y.LateFeeInvoiceItem != null && y.LateFeeInvoiceItem.LateFeeTypeId == (long)LateFeeType.SalesData)).ToList();

                    var salesDataLateFeeinvoices = franchisee.FranchiseeInvoices.SelectMany(x => x.Invoice != null && x.Invoice.InvoiceItems != null ? x.Invoice.InvoiceItems
                         .Where(y => y.LateFeeInvoiceItem != null && y.LateFeeInvoiceItem.LateFeeTypeId == (long)LateFeeType.SalesData)
                     : Enumerable.Empty<InvoiceItem>()).ToList();

                    if ((feeProfile == null || feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Monthly))
                    {
                        var lateFeeStartdate = lastUploadEndDate.AddDays(1);
                        var lateFeeEndDate = lateFeeStartdate.AddMonths(1).AddDays(-1);

                        var result = _salesDataUploadCreateModelValidator.CheckDatesAreValidMonth(lateFeeStartdate, lateFeeEndDate);
                        if (!result)
                        {
                            lateFeeStartdate = new DateTime(lateFeeStartdate.Year, lateFeeStartdate.Month, 1);
                            lateFeeEndDate = lateFeeStartdate.Date.AddMonths(1).AddDays(-1);
                        }

                        int monthCount = (currentDate.Month - lateFeeEndDate.Month) + 12 * (currentDate.Year - lateFeeEndDate.Year);
                        for (int i = 0; i < monthCount; i++)
                        {
                            var dateRangeModel = new StartEndDateViewModel { };
                            dateRangeModel.StartDate = lateFeeStartdate;
                            dateRangeModel.EndDate = lateFeeEndDate;

                            startEndDateList.Add(dateRangeModel);

                            lateFeeStartdate = lateFeeEndDate.Date.AddDays(1);
                            lateFeeEndDate = lateFeeStartdate.Date.AddMonths(1).AddDays(-1);
                        }

                        foreach (var item in startEndDateList)
                        {
                            if (!salesDataLateFeeinvoices.Where(x => x.LateFeeInvoiceItem.StartDate.Equals(item.StartDate)
                                     && x.LateFeeInvoiceItem.EndDate.Equals(item.EndDate)).Any())
                                SaveSalesDataLateFeeInvoice(franchisee, currenctdate, item.StartDate, item.EndDate);
                        }
                    }
                    else if (feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Weekly)
                    {
                        var lateFeeStartdate = lastUploadEndDate.AddDays(1);
                        var lateFeeEndDate = lateFeeStartdate.AddDays(6);

                        var result = _salesDataUploadCreateModelValidator.CheckIfDatesAreValidWeek(lateFeeStartdate, lateFeeEndDate);
                        if (!result)
                        {
                            lateFeeStartdate = _salesDataUploadService.StartOfWeek(lateFeeStartdate, DayOfWeek.Monday);
                            lateFeeEndDate = lateFeeStartdate.Date.AddDays(6);
                        }

                        int weekCount = currentDate.Subtract(lastUploadEndDate).Days / 7;
                        for (int i = 0; i < weekCount; i++)
                        {
                            var dateRangeModel = new StartEndDateViewModel { };
                            dateRangeModel.StartDate = lateFeeStartdate;
                            dateRangeModel.EndDate = lateFeeEndDate;

                            startEndDateList.Add(dateRangeModel);

                            lateFeeStartdate = lateFeeEndDate.AddDays(1);
                            lateFeeEndDate = lateFeeStartdate.AddDays(6);

                        }

                        foreach (var item in startEndDateList)
                        {
                            if (!salesDataLateFeeinvoices.Where(x => x.LateFeeInvoiceItem.StartDate.Equals(item.StartDate)
                                     && x.LateFeeInvoiceItem.EndDate.Equals(item.EndDate)).Any())
                                SaveSalesDataLateFeeInvoice(franchisee, currenctdate, item.StartDate, item.EndDate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error In Applying Late Fee On Sales Data- {0}", ex));
                return;
            }
        }
        private InvoiceEditModel GetInvoiceModel(DateTime currenctdate, int waitPeriod)
        {
            int lateFeeWaitPeriod = waitPeriod > 0 ? waitPeriod : _settings.DefaultSalesDataLateFeeWaitPeriod;
            var invoice = new InvoiceEditModel();
            invoice.GeneratedOn = currenctdate;
            invoice.DueDate = currenctdate.AddDays(lateFeeWaitPeriod);
            invoice.StatusId = (long)InvoiceStatus.Unpaid;
            invoice.DataRecorderMetaData = new DataRecorderMetaData(DateTime.UtcNow);
            invoice.InvoiceItems = new List<InvoiceItemEditModel>();
            return invoice;
        }
        private bool IsNewInvoiceCreated(Franchisee franchisee)
        {
            //if (!franchisee.FranchiseeInvoices.Any()) return true;
            //else if (franchisee.FranchiseeInvoices.Any(x => x.Invoice.StatusId == (long)InvoiceStatus.Unpaid
            //        && (!x.Invoice.InvoiceItems.Any(y => y.ItemTypeId == (long)InvoiceItemType.RoyaltyFee || y.ItemTypeId == (long)InvoiceItemType.AdFund)))) return false;
            //return true;
            if (franchisee == null || franchisee.FranchiseeInvoices == null)
            {
                // Handle the null case here
                return true;
            }

            // Check if there are no franchisee invoices
            if (!franchisee.FranchiseeInvoices.Any())
            {
                return true;
            }

            // Check for unpaid invoices without RoyaltyFee or AdFund items
            var hasUnpaidInvoiceWithoutRoyaltyOrAdFundItems = franchisee.FranchiseeInvoices.Any(x =>
                x.Invoice != null && // Ensure invoice is not null
                x.Invoice.StatusId == (long)InvoiceStatus.Unpaid &&
                (x.Invoice.InvoiceItems == null || !x.Invoice.InvoiceItems.Any(y =>
                    y != null && // Ensure invoice item is not null
                    (y.ItemTypeId == (long)InvoiceItemType.RoyaltyFee ||
                     y.ItemTypeId == (long)InvoiceItemType.AdFund)
                ))
            );

            // Return true if no unpaid invoices without RoyaltyFee or AdFund items found, else return false
            return !hasUnpaidInvoiceWithoutRoyaltyOrAdFundItems;
        }
        private InvoiceItemEditModel SalesDataLateFeeInvoiceItem(FranchiseeInvoice item, long currencyExchangeRateId, DateTime currenctdate, DateTime lateFeeStartDate, DateTime lateFeeEnddate)
        {
            var invoiceitemeditmodel = new LateFeeInvoiceItemEditModel();
            invoiceitemeditmodel.InvoiceId = item.InvoiceId;
            invoiceitemeditmodel.Amount = item.Franchisee.LateFee.SalesDataLateFee;
            invoiceitemeditmodel.ItemTypeId = (long)InvoiceItemType.LateFees;
            invoiceitemeditmodel.Description = "Sales Data Fee charged on " + currenctdate.Date.ToString("MM/dd/yyyy");

            invoiceitemeditmodel.LateFeeTypeId = (long)LateFeeType.SalesData;
            invoiceitemeditmodel.SalesAmount = item.Franchisee.LateFee.SalesDataLateFee;
            invoiceitemeditmodel.WaitPeriod = item.Franchisee.LateFee.SalesDataWaitPeriodInDays;
            invoiceitemeditmodel.StartDate = lateFeeStartDate;
            invoiceitemeditmodel.EndDate = lateFeeEnddate;
            invoiceitemeditmodel.Quantity = 1;
            invoiceitemeditmodel.Rate = item.Franchisee.LateFee.SalesDataLateFee;
            invoiceitemeditmodel.CurrencyExchangeRateId = currencyExchangeRateId;
            invoiceitemeditmodel.GeneratedOn = currenctdate;
            return invoiceitemeditmodel;
        }
        private void SaveSalesDataLateFeeInvoice(Franchisee franchisee, DateTime currenctdate, DateTime lateFeeStartdate, DateTime lateFeeEndDate)
        {
            try
            {
                var waitPeriod = franchisee.LateFee.SalesDataWaitPeriodInDays > 0 ? franchisee.LateFee.SalesDataWaitPeriodInDays : _settings.DefaultSalesDataLateFeeWaitPeriod;

                if (lateFeeEndDate.Date > _settings.LateFeeStartDate.Date && currenctdate > lateFeeEndDate.AddDays(waitPeriod))
                {
                    if (IsNewInvoiceCreated(franchisee))
                    {
                        SaveSalesDataLateFeeNewInvoice(franchisee, currenctdate, lateFeeStartdate, lateFeeEndDate);
                    }
                    else
                    {
                        AddSalesDataLateFeeInvoiceItem(franchisee, currenctdate, lateFeeStartdate, lateFeeEndDate);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error In Applying Late Fee On Sales Data- {0}", ex));
                return;
            }
        }
        private void SaveSalesDataLateFeeNewInvoice(Franchisee franchisee, DateTime currenctdate, DateTime lateFeeStartdate, DateTime lateFeeEndDate)
        {
            try
            {
                var currencyExchangeRate = GetCurrencyExchangeRate(franchisee, currenctdate);

                var invoiceModel = GetInvoiceModel(currenctdate, franchisee.LateFee.SalesDataWaitPeriodInDays);
                var invoiceitemeditmodel = new LateFeeInvoiceItemEditModel
                {
                    Amount = franchisee.LateFee.SalesDataLateFee,
                    ItemTypeId = (long)InvoiceItemType.LateFees,
                    Description = "Sales Data Fee charged on " + currenctdate.Date.ToString("MM/dd/yyyy"),

                    LateFeeTypeId = (long)LateFeeType.SalesData,
                    SalesAmount = franchisee.LateFee.SalesDataLateFee,
                    WaitPeriod = franchisee.LateFee.SalesDataWaitPeriodInDays,
                    StartDate = lateFeeStartdate,
                    EndDate = lateFeeEndDate,
                    Quantity = 1,
                    Rate = franchisee.LateFee.SalesDataLateFee,
                    CurrencyExchangeRateId = currencyExchangeRate.Id,
                    GeneratedOn = currenctdate
                };
                _logService.Info(string.Format("Start Creating New Invoice of Sales Data Late Fee {0}  ", franchisee.Id));

                _unitOfWork.StartTransaction();

                invoiceModel.InvoiceItems.Add(invoiceitemeditmodel);
                var invoicedomain = _invoicefactory.CreateDomain(invoiceModel);
                _invoiceRepository.Save(invoicedomain);

                var invoiceItemDomain = invoicedomain.InvoiceItems.FirstOrDefault();
                var franchiseeInvoice = _franchiseeInvoiceFactory.CreateDomain(franchisee.Id, invoicedomain.Id, null, null);
                _franchiseeInvoiceRepository.Save(franchiseeInvoice);

                if (invoiceItemDomain == null)
                    _logService.Info(string.Format("Can't find invoiceItem for Invoice# {0}  ", invoicedomain.Id));
                if (invoiceItemDomain != null)
                    _lateFeeNotificationService.CreateLateFeeNotification(invoiceItemDomain, franchisee.Organization.Id, currenctdate);


                _unitOfWork.SaveChanges();

                _logService.Info(string.Format("End Creating New Invoice of Sales Data Late Fee {0}  ", franchisee.Id));

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }
        }
        private void AddSalesDataLateFeeInvoiceItem(Franchisee franchisee, DateTime currenctdate, DateTime lateFeeStartdate, DateTime lateFeeEndDate)
        {
            try
            {
                var openInvoice = franchisee.FranchiseeInvoices.Where(x => x.Invoice.StatusId == (long)InvoiceStatus.Unpaid).OrderByDescending(x => x.Invoice.DueDate).FirstOrDefault();
                if (openInvoice != null)
                //&& !openInvoice.Invoice.InvoiceItems.Any(x => x.ItemTypeId == (long)InvoiceItemType.LateFees && x.LateFeeInvoiceItem.LateFeeTypeId == (long)LateFeeType.SalesData))
                {
                    _logService.Info(string.Format("Start Adding Sales Data Late Fee Item {0}  ", franchisee.Id));

                    _unitOfWork.StartTransaction();

                    var currencyExchengeRate = GetCurrencyExchangeRate(franchisee, currenctdate);

                    var invoiceitemeditmodel = SalesDataLateFeeInvoiceItem(openInvoice, currencyExchengeRate.Id, currenctdate, lateFeeStartdate, lateFeeEndDate);
                    invoiceitemeditmodel.CurrencyExchangeRateId = currencyExchengeRate.Id;
                    var invoiceitemdomain = _invoiceItemfactory.CreateDomain(invoiceitemeditmodel);
                    _invoiceItemRepository.Save(invoiceitemdomain);

                    _lateFeeNotificationService.CreateLateFeeNotification(invoiceitemdomain, franchisee.Organization.Id, currenctdate);

                    _unitOfWork.SaveChanges();

                    _logService.Info(string.Format("End Adding Sales Data Late Fee Item {0}  ", franchisee.Id));
                }

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }
        }
        private CurrencyExchangeRate GetCurrencyExchangeRate(Franchisee franchisee, DateTime currentdate)
        {
            long countryId = franchisee.Organization.Address != null ? franchisee.Organization.Address.First().CountryId : 0;
            var currencyExchangeRate = new CurrencyExchangeRate();
            if (countryId > 0)
            {
                currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId && x.DateTime.Year == currentdate.Year && x.DateTime.Month == currentdate.Month
                                        && x.DateTime.Day == currentdate.Day).OrderByDescending(y => y.DateTime).FirstOrDefault();

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
