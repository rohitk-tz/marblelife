using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Organizations.ViewModel;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Core.MarketingLead.Domain;
using System.Globalization;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeServiceFeeService : IFranchiseeServiceFeeService
    {
        private readonly IClock _clock;
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceItemFactory _invoiceItemFactory;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<OneTimeProjectFee> _oneTimeProjectFeeRepository;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly IRepository<FranchiseeServiceFee> _franchiseeServiceFeeRepository;
        private readonly IFranchiseeServiceFeeFactory _franchiseeServiceFeeFactory;
        private readonly IRepository<FranchiseeLoan> _franchiseeLoanRepository;
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<FranchiseeLoanSchedule> _franchiseeLoanScheduleRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<LoanAdjustmentAudit> _loanAdjustmentAuditRepository;
        private readonly IRepository<Application.Domain.File> _fileRepository;
        private readonly IRepository<FranchiseeAccountCredit> _franchiseeAccountCreditRepository;
        private readonly IFileService _fileService;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IRepository<OnetimeprojectfeeAddFundRoyality> _onetimeprojectfeeAddFundRoyality;
        private readonly IRepository<FranchiseeDurationNotesHistry> _franchiseeDurationNotesHistryRepository;
        private IRepository<Phonechargesfee> _phonechargesfeeRepository;
        public FranchiseeServiceFeeService(IClock clock, IUnitOfWork unitOfWork, ILogService logService, IInvoiceItemFactory invoiceItemFactory,
            IFranchiseeServiceFeeFactory franchiseeServiceFeeFactory, IFileService fileService, IExcelFileCreator excelFileCreator)
        {
            _clock = clock;
            _logService = logService;
            _unitOfWork = unitOfWork;
            _invoiceItemFactory = invoiceItemFactory;
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _oneTimeProjectFeeRepository = unitOfWork.Repository<OneTimeProjectFee>();
            _franchiseeServiceFeeRepository = unitOfWork.Repository<FranchiseeServiceFee>();
            _franchiseeServiceFeeFactory = franchiseeServiceFeeFactory;
            _franchiseeLoanRepository = unitOfWork.Repository<FranchiseeLoan>();
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _franchiseeSalesPaymentRepository = unitOfWork.Repository<FranchiseeSalesPayment>();
            _paymentRepository = unitOfWork.Repository<Payment>();
            _franchiseeLoanScheduleRepository = unitOfWork.Repository<FranchiseeLoanSchedule>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _loanAdjustmentAuditRepository = unitOfWork.Repository<LoanAdjustmentAudit>();
            _fileRepository = unitOfWork.Repository<Application.Domain.File>();
            _fileService = fileService;
            _excelFileCreator = excelFileCreator;
            _onetimeprojectfeeAddFundRoyality = unitOfWork.Repository<OnetimeprojectfeeAddFundRoyality>();
            _franchiseeAccountCreditRepository = unitOfWork.Repository<FranchiseeAccountCredit>();
            _franchiseeDurationNotesHistryRepository = unitOfWork.Repository<FranchiseeDurationNotesHistry>();
            _phonechargesfeeRepository = unitOfWork.Repository<Phonechargesfee>();
        }

        public void SaveServiceFeeItem(FranchiseeInvoice franchiseeInvoice)
        {
            var currentDate = _clock.UtcNow;
            var feeProfile = franchiseeInvoice.Franchisee.FeeProfile;
            foreach (var serviceFee in franchiseeInvoice.Franchisee.FranchiseeServiceFee.Where(sf => sf.IsActive))
            {
                if (serviceFee.ServiceFeeTypeId == (long)ServiceFeeType.PayrollProcessing && serviceFee.Amount > 0)
                {
                    SavePayRollInvoiceItem(serviceFee, franchiseeInvoice, feeProfile);
                }
                if (serviceFee.ServiceFeeTypeId == (long)ServiceFeeType.Recruiting && serviceFee.Amount > 0)
                {
                    SaveRecruitingInvoiceItem(serviceFee, franchiseeInvoice, feeProfile);
                }
                if (serviceFee.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping && (serviceFee.Amount > 0 || serviceFee.Percentage > 0)
                    && franchiseeInvoice.SalesDataUpload.PaidAmount > 0)
                {
                    SaveBookKeepingInvoiceItem(serviceFee, franchiseeInvoice, feeProfile);
                }
                if (serviceFee.ServiceFeeTypeId == (long)ServiceFeeType.NationalCharge && serviceFee.Percentage > 0
                     && franchiseeInvoice.SalesDataUpload.PaidAmount > 0)
                {
                    SaveNationalCharge(serviceFee, franchiseeInvoice);
                }
                //if (serviceFee.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges
                //     && franchiseeInvoice.SalesDataUpload.PaidAmount > 0)
                //{
                //    SaveSeoCharge(serviceFee, franchiseeInvoice, false);
                //}
            }
            //var isSeoChargePresent = franchiseeInvoice.Franchisee.FranchiseeServiceFee.Any(sf => !sf.IsActive && sf.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges);
            //if (!isSeoChargePresent)
            //{
            //    var franchiseeLastMonthCharges = _franchiseeServiceFeeRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges && !x.IsActive);
            //    if (franchiseeLastMonthCharges != null)
            //        SaveSeoCharge(franchiseeLastMonthCharges, franchiseeInvoice, true);
            //}
        }

        public void SaveServiceFeeItemForSeo(FranchiseeInvoice franchiseeInvoice)
        {
            List<int> FranchiseeList = new List<int>() {3, 5, 8, 10, 11, 14, 16, 17, 18, 19, 21, 22, 
                23, 30, 31, 38, 39, 40, 42, 45, 47, 48, 50, 52, 57, 62, 64, 76, 78, 79, 82, 85, 86, 
                87, 90, 92, 95, 98, 100 };
            var seoChargesApplied = false;
            foreach (var serviceFee in franchiseeInvoice.Franchisee.FranchiseeServiceFee.Where(sf => sf.IsActive))
            {
                if (serviceFee.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges)
                {
                    SaveSeoCharge(serviceFee, franchiseeInvoice, false);
                    seoChargesApplied = true;
                }
            }
            var isSeoChargePresent = franchiseeInvoice.Franchisee.FranchiseeServiceFee.Any(sf => !sf.IsActive && sf.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges);
            if (isSeoChargePresent && !seoChargesApplied)
            {
                var franchiseeLastMonthCharges = _franchiseeServiceFeeRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges && !x.IsActive && x.FranchiseeId== franchiseeInvoice.FranchiseeId);
                if (franchiseeLastMonthCharges != null)
                    SaveSeoCharge(franchiseeLastMonthCharges, franchiseeInvoice, true);
            }
        }

        public void SaveNationalCharge(FranchiseeServiceFee serviceFee, FranchiseeInvoice frInvoice)
        {
            var nationalpercentage = serviceFee.Percentage;
            var nationalChargeList = _franchiseeSalesPaymentRepository.Table.Where(x => x.SalesDataUploadId == frInvoice.SalesDataUploadId
                                   && x.FranchiseeSales != null && x.FranchiseeSales.ClassTypeId == (long)MarketingClassType.National).ToList();

            var nationalChargeAmount = nationalChargeList.Any() ? nationalChargeList.Sum(x => x.Payment.Amount) : 0;
            if (nationalChargeAmount > 0)
            {
                var amount = (nationalChargeAmount * nationalpercentage) / 100;
                var qty = 1;
                var rate = nationalChargeAmount;
                var model = CreateModel(serviceFee, frInvoice, amount.Value, rate, qty);
                model.Description = "Sales - Franchise Services:  National Acct Sales " + model.Description;
                Save(model, serviceFee, frInvoice.InvoiceId);
            }
        }
        private static int GetWeekNumberOfMonth(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            return (date - firstMonthMonday).Days / 7 + 1;
        }
        public bool SaveSeoCharge(FranchiseeServiceFee serviceFee, FranchiseeInvoice frInvoice, bool isFromOtherMonth)
        {
            List<long> FranchiseeList = new List<long>() {3, 5, 8, 10, 11, 14, 16, 17, 18, 19, 21, 22,
                23, 30, 31, 38, 39, 40, 42, 47, 48, 52, 57, 64, 75, 76, 78, 79, 82, 85, 86,
                87, 90, 92, 95, 98, 100 };

            var FirstDayOfFeb = new DateTime(2023, 2, 1);
            var LastDayOfFeb = new DateTime(2023, 2, 28);

            var franchiseeInvoice = _franchiseeInvoiceRepository.Table.Where(x => x.FranchiseeId == frInvoice.FranchiseeId).ToList();

            var invoiceIdsForAFranchisee = franchiseeInvoice != null ? franchiseeInvoice.Select(x => x.InvoiceId).ToList()
                : new List<long>();

            var sEOAppliedInFeb = invoiceIdsForAFranchisee != null ? _invoiceItemRepository.Table.Where(x => x.ItemTypeId == (long)InvoiceItemType.ServiceFee
             && x.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges
             && x.ServiceFeeInvoiceItem.StartDate <= LastDayOfFeb
             && x.ServiceFeeInvoiceItem.StartDate >= FirstDayOfFeb
             && invoiceIdsForAFranchisee.Contains(x.InvoiceId)).ToList() : new List<InvoiceItem>();

            var isSeoAppliedInFeb = (sEOAppliedInFeb!=null && sEOAppliedInFeb.Count() > 0) ? true : false;

            var isSalesUploadFrequencyMonthly = serviceFee.Franchisee.FeeProfile.PaymentFrequencyId == (long)PaymentFrequency.Monthly;
            int weekNum = GetWeekNumberOfMonth(frInvoice.SalesDataUpload.PeriodStartDate);
            var WeekInDb = serviceFee.FrequencyId == (long)PaymentFrequency.FirstWeek ? 1 : 2;
            var isSeoActive = serviceFee.Franchisee != null ? serviceFee.Franchisee.IsSEOActive : false;
            var isSEOApplied = false;
            if (weekNum == WeekInDb && isSeoActive)
            {
                isSEOApplied = true;
                var amount = serviceFee.Amount;
                var qty = 1;
                var rate = serviceFee.Percentage;
                serviceFee.SaveDateForSeoCost = _clock.UtcNow;
                serviceFee.IsActive = false;
                serviceFee.IsNew = false;
                _franchiseeServiceFeeRepository.Save(serviceFee);
                var model = CreateModel(serviceFee, frInvoice, amount, rate.GetValueOrDefault(), qty);

                Save(model, serviceFee, frInvoice.InvoiceId);
                if (isFromOtherMonth)
                {
                    var franchiseServiceFee = new FranchiseeServiceFee()
                    {
                        FranchiseeId = serviceFee.FranchiseeId,
                        ServiceFeeTypeId = (long)ServiceFeeType.SEOCharges,
                        Amount = Convert.ToDecimal(serviceFee.Amount),
                        Percentage = Convert.ToDecimal(0.00),
                        FrequencyId = serviceFee.FrequencyId,
                        IsActive = false,
                        IsNew = true,
                        SaveDateForSeoCost = _clock.ToUtc(DateTime.Now),
                        InvoiceDateForSeoCost = _clock.ToUtc(DateTime.Now)
                    };
                    _franchiseeServiceFeeRepository.Save(franchiseServiceFee);
                }
            }
            else if(!isSeoAppliedInFeb && !isSEOApplied && FranchiseeList.Contains(serviceFee.FranchiseeId) &&
                frInvoice.SalesDataUpload.PeriodStartDate == new DateTime(2023,2,20) &&
                frInvoice.SalesDataUpload.PeriodEndDate == new DateTime(2023,2,26))
            {
                var amount = serviceFee.Amount;
                var qty = 1;
                var rate = serviceFee.Percentage;
                serviceFee.SaveDateForSeoCost = _clock.UtcNow;
                serviceFee.IsActive = false;
                serviceFee.IsNew = false;
                _franchiseeServiceFeeRepository.Save(serviceFee);
                var model = CreateModel(serviceFee, frInvoice, amount, rate.GetValueOrDefault(), qty);

                Save(model, serviceFee, frInvoice.InvoiceId);
                if (isFromOtherMonth)
                {
                    var franchiseServiceFee = new FranchiseeServiceFee()
                    {
                        FranchiseeId = serviceFee.FranchiseeId,
                        ServiceFeeTypeId = (long)ServiceFeeType.SEOCharges,
                        Amount = Convert.ToDecimal(serviceFee.Amount),
                        Percentage = Convert.ToDecimal(0.00),
                        FrequencyId = serviceFee.FrequencyId,
                        IsActive = false,
                        IsNew = true,
                        SaveDateForSeoCost = _clock.ToUtc(DateTime.Now),
                        InvoiceDateForSeoCost = _clock.ToUtc(DateTime.Now)
                    };
                    _franchiseeServiceFeeRepository.Save(franchiseServiceFee);
                }
            }
            else if(isSalesUploadFrequencyMonthly && isSeoActive)
            {
                var amount = serviceFee.Amount;
                var qty = 1;
                var rate = serviceFee.Percentage;
                serviceFee.SaveDateForSeoCost = _clock.UtcNow;
                serviceFee.IsActive = false;
                serviceFee.IsNew = false;
                _franchiseeServiceFeeRepository.Save(serviceFee);
                var model = CreateModel(serviceFee, frInvoice, amount, rate.GetValueOrDefault(), qty);

                Save(model, serviceFee, frInvoice.InvoiceId);
                if (isFromOtherMonth)
                {
                    var franchiseServiceFee = new FranchiseeServiceFee()
                    {
                        FranchiseeId = serviceFee.FranchiseeId,
                        ServiceFeeTypeId = (long)ServiceFeeType.SEOCharges,
                        Amount = Convert.ToDecimal(serviceFee.Amount),
                        Percentage = Convert.ToDecimal(0.00),
                        FrequencyId = serviceFee.FrequencyId,
                        IsActive = false,
                        IsNew = true,
                        SaveDateForSeoCost = _clock.ToUtc(DateTime.Now),
                        InvoiceDateForSeoCost = _clock.ToUtc(DateTime.Now)
                    };
                    _franchiseeServiceFeeRepository.Save(franchiseServiceFee);
                }
            }


            return true;
        }

        public void SaveOneTimeProjectFee(FranchiseeInvoice frInvoice, IEnumerable<OneTimeProjectFee> feeList)
        {
            var serviceFee = frInvoice.Franchisee.FranchiseeServiceFee.Where(x => x.ServiceFeeTypeId == (long)ServiceFeeType.OneTimeProject).FirstOrDefault();
            if (serviceFee != null)
            {
                foreach (var fee in feeList)
                {
                    decimal amount = fee.Amount;
                    int qty = 1;
                    decimal rate = serviceFee.Amount;
                    var startDate = frInvoice.SalesDataUpload.PeriodStartDate;
                    var endDate = frInvoice.SalesDataUpload.PeriodEndDate;

                    var model = CreateModel(serviceFee, frInvoice, amount, rate, qty);
                    var invoiceItemId = Save(model, serviceFee, frInvoice.InvoiceId);

                    fee.InvoiceItemId = invoiceItemId;
                    _oneTimeProjectFeeRepository.Save(fee);
                }
            }
        }

        public void SaveLoanInvoiceItem(FranchiseeInvoice frInvoice, FranchiseeLoanSchedule loan)
        {
            var serviceFee = frInvoice.Franchisee.FranchiseeServiceFee.Where(x => x.ServiceFeeTypeId == (long)ServiceFeeType.Loan).FirstOrDefault();
            var interestAmountFee = frInvoice.Franchisee.FranchiseeServiceFee.Where(x => x.ServiceFeeTypeId == (long)ServiceFeeType.InterestAmount).FirstOrDefault();
            if (serviceFee != null)
            {
                //var isRoyality = true;
                var amount = loan.Principal;
                var qty = 1;
                var rate = loan.FranchiseeLoan.InterestratePerAnum;
                var model = CreateModel(serviceFee, frInvoice, amount, rate, qty, loan.DueDate, loan.DueDate);
                var description = GetLoanDescription(loan.FranchiseeLoan);
                model.Description = description + " " + model.Description;
                model.ItemTypeId = (long)InvoiceItemType.LoanServiceFee;
                var invoiceItemId = Save(model, serviceFee, frInvoice.InvoiceId);
                loan.InvoiceItemId = invoiceItemId;

                //if (interestAmountFee != null && loan.Interest > 0)
                if (interestAmountFee != null)
                {
                    var interestAmount = loan.Interest;
                    var quantity = 1;
                    var interestRate = loan.FranchiseeLoan.InterestratePerAnum;
                    var interestAmountModel = CreateModel(interestAmountFee, frInvoice, interestAmount, interestRate, quantity, loan.DueDate, loan.DueDate);
                    interestAmountModel.ItemTypeId = (long)InvoiceItemType.LoanServiceFeeInterestRatePerAnnum;
                    interestAmountModel.Description = description + " " + interestAmountModel.Description;
                    var itemId = Save(interestAmountModel, interestAmountFee, frInvoice.InvoiceId);
                    loan.InterestAmountInvoiceItemId = itemId;
                    loan.IsRoyality = loan.FranchiseeLoan.IsRoyality.Value;
                }
                _franchiseeLoanScheduleRepository.Save(loan);
            }
        }

        private void SaveBookKeepingInvoiceItem(FranchiseeServiceFee serviceFee, FranchiseeInvoice invoice, FeeProfile feeProfile)
        {
            decimal amount = serviceFee.Amount;
            int qty = 1;
            decimal rate = serviceFee.Amount;
            decimal percentage = serviceFee.Percentage.Value;

            var startDate = invoice.SalesDataUpload.PeriodStartDate;
            var endDate = invoice.SalesDataUpload.PeriodEndDate;
            var daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            var lastDayOfMonth = new DateTime(startDate.Year, startDate.Month, daysInMonth);
            var firstDatyOfMonth = new DateTime(startDate.Year, startDate.Month, 1);
            var fixedAmount = amount;
            var variableAmount = invoice.SalesDataUpload.PaidAmount != null ? Math.Round((invoice.SalesDataUpload.PaidAmount.Value * percentage) / 100, 2) : 0;

            if (startDate.Date <= lastDayOfMonth && endDate.Date >= lastDayOfMonth && fixedAmount > 0)
            {
                var model = CreateModel(serviceFee, invoice, fixedAmount, rate, qty);
                model.Description = "Sales - Franchise Services: Accounting Services " + model.Description;
                Save(model, serviceFee, invoice.InvoiceId);
            }

            if (feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Monthly && (startDate.Date <= lastDayOfMonth && endDate.Date >= lastDayOfMonth)
                && variableAmount > 0)
            {
                SaveVariableFee(serviceFee.FranchiseeId, invoice, variableAmount, rate, qty);
            }

            else if (feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Weekly)
            {
                if (serviceFee.FrequencyId == (long)PaymentFrequency.Weekly && variableAmount > 0)
                {
                    SaveVariableFee(serviceFee.FranchiseeId, invoice, variableAmount, rate, qty);
                }

                if (serviceFee.FrequencyId == (long)PaymentFrequency.Monthly && (startDate.Date <= lastDayOfMonth && endDate.Date >= lastDayOfMonth))
                {
                    var paymentIds = _franchiseeSalesPaymentRepository.Table.Where(x => x.FranchiseeSales.FranchiseeId == invoice.FranchiseeId
                                            && x.Payment != null && (x.Payment.Date >= firstDatyOfMonth && x.Payment.Date <= lastDayOfMonth)
                                            && x.Payment.PaymentItems.Any(ii => ii.ItemTypeId == (long)InvoiceItemType.Service
                                            || ii.ItemTypeId == (long)InvoiceItemType.Discount))
                                           .Select(x => x.PaymentId).Distinct();

                    var paymentAmounts = _paymentRepository.Table.Where(x => paymentIds.Contains(x.Id)).Select(m => m.Amount);
                    var totalSales = paymentAmounts.Count() > 0 ? paymentAmounts.Sum() : 0;
                    variableAmount = Math.Round((totalSales * percentage) / 100, 2);

                    if (variableAmount > 0)
                    {
                        rate = totalSales;
                        SaveVariableFee(serviceFee.FranchiseeId, invoice, variableAmount, rate, qty);
                    }
                }
            }
        }

        private void SaveVariableFee(long franchiseeId, FranchiseeInvoice invoice, decimal variableAmount, decimal rate, int qty)
        {
            var varServiceFee = _franchiseeServiceFeeRepository.Get(x => x.FranchiseeId == franchiseeId
                                      && x.ServiceFeeTypeId == (long)ServiceFeeType.VarBookkeeping);
            if (varServiceFee != null)
            {
                var model = CreateModel(varServiceFee, invoice, variableAmount, rate, qty);
                model.Description = "Sales - Franchise Services: Accounting Services " + model.Description;
                Save(model, varServiceFee, invoice.InvoiceId);
            }
        }

        private void SaveRecruitingInvoiceItem(FranchiseeServiceFee serviceFee, FranchiseeInvoice invoice, FeeProfile feeProfile)
        {
            decimal amount = serviceFee.Amount;
            int qty = 1;
            decimal rate = serviceFee.Amount;
            var startDate = invoice.SalesDataUpload.PeriodStartDate;
            var endDate = invoice.SalesDataUpload.PeriodEndDate;

            var daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            var lastDayOfMonth = new DateTime(startDate.Year, startDate.Month, daysInMonth);

            if (startDate.Date <= lastDayOfMonth && endDate.Date >= lastDayOfMonth)
            {
                //var personToBeCharged = _organizationRoleUserRepository.Table.Where(x => x.Person != null && x.Person.UserLogin != null
                //                        && x.Person.IsRecruitmentFeeApplicable && x.IsActive
                //                        && !x.Person.UserLogin.IsLocked
                //                        && x.OrganizationId == invoice.FranchiseeId
                //                        && (x.RoleId == (long)RoleType.SalesRep || x.RoleId == (long)RoleType.Technician))
                //                        .Select(y => y.UserId).Distinct().ToList();
                //if (personToBeCharged.Count() > 0)
                //{
                //amount = amount;
                qty = 1;

                var model = CreateModel(serviceFee, invoice, amount, rate, qty);
                model.Description = "Sales - Franchise Services:  HR Recruitment Fees " + model.Description;
                Save(model, serviceFee, invoice.InvoiceId);
                //}
            }
        }

        private void SavePayRollInvoiceItem(FranchiseeServiceFee serviceFee, FranchiseeInvoice invoice, FeeProfile feeProfile)
        {
            decimal amount = serviceFee.Amount;
            int qty = 1;
            decimal rate = serviceFee.Amount;
            var startDate = invoice.SalesDataUpload.PeriodStartDate;
            var endDate = invoice.SalesDataUpload.PeriodEndDate;

            const int midDayOfMonth = 15;
            var daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            var lastDayOfMonth = new DateTime(startDate.Year, startDate.Month, daysInMonth);

            if (feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Monthly && startDate.Date <= lastDayOfMonth && endDate.Date >= lastDayOfMonth)
            {
                var model = CreateModel(serviceFee, invoice, amount, rate, qty);
                model.Description = "Sales - Franchise Services:  Payroll Processing " + model.Description;
                Save(model, serviceFee, invoice.InvoiceId);
            }

            else if (feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Weekly)
            {
                var fixedAmount = amount;
                if (serviceFee.FrequencyId == (long)PaymentFrequency.Monthly && (startDate.Date <= lastDayOfMonth && endDate.Date >= lastDayOfMonth))
                {
                    var model = CreateModel(serviceFee, invoice, amount, rate, qty);
                    model.Description = "Sales - Franchise Services:  Payroll Processing " + model.Description;
                    Save(model, serviceFee, invoice.InvoiceId);
                }
                if (serviceFee.FrequencyId == (long)PaymentFrequency.TwiceAMonth &&
                    ((startDate.Day <= midDayOfMonth && endDate.Day >= midDayOfMonth) || (startDate.Date <= lastDayOfMonth && endDate.Date >= lastDayOfMonth)))
                {
                    var model = CreateModel(serviceFee, invoice, amount / 2, rate, qty);
                    model.Description = "Sales - Franchise Services:  Payroll Processing " + model.Description;
                    Save(model, serviceFee, invoice.InvoiceId);
                }
            }
        }

        public long Save(InvoiceItemEditModel model, FranchiseeServiceFee serviceFee, long invoiceId)
        {
            _logService.Info(string.Format("Start Adding Service Fee {0}  {1} - {2} ", serviceFee.Id, serviceFee.FranchiseeId, invoiceId));

            _unitOfWork.StartTransaction();

            var domain = _invoiceItemFactory.CreateDomain(model);
            _invoiceItemRepository.Save(domain);

            _unitOfWork.SaveChanges();
            _logService.Info(string.Format("End Adding Late Fee {0}  {1} - {2} ", serviceFee.Id, serviceFee.FranchiseeId, invoiceId));
            return domain.Id;
        }

        public InvoiceItemEditModel CreateModel(FranchiseeServiceFee serviceFee, FranchiseeInvoice invoice, decimal amount, decimal rate, int qty,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            var currencyExchangeRate = GetCurrencyExchangeRate(invoice.Franchisee, _clock.UtcNow);
            var salesDataUpload = invoice != null ? invoice.SalesDataUpload : new SalesDataUpload();
            var month = string.Empty;
            var year = string.Empty;
            if (salesDataUpload != null || salesDataUpload != default(SalesDataUpload))
            {
                month = salesDataUpload.PeriodStartDate.ToString("MMMM");
                year = salesDataUpload.PeriodStartDate.ToString("yyyy");
            }
            var model = new ServiceFeeInvoiceItemEditModel
            {
                ItemTypeId = (long)InvoiceItemType.ServiceFee,
                InvoiceId = invoice.InvoiceId,
                Quantity = qty,
                Rate = rate,
                Description = GetDescriptionForServiceFee(serviceFee, month, year),
                Amount = amount,
                CurrencyExchangeRateId = currencyExchangeRate.Id,
                ServiceFeeTypeId = serviceFee.ServiceFeeTypeId,
                StartDate = startDate != null ? startDate.Value : invoice.SalesDataUpload.PeriodStartDate,
                Percentage = serviceFee.Percentage,
                EndDate = endDate != null ? endDate.Value : invoice.SalesDataUpload.PeriodEndDate
            };
            return model;
        }

        private CurrencyExchangeRate GetCurrencyExchangeRate(Franchisee franchisee, DateTime date)
        {
            long countryId = franchisee.Organization.Address != null ? franchisee.Organization.Address.First().CountryId : 0;

            var currencyExchangeRate = new CurrencyExchangeRate();

            currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId && x.DateTime.Year == date.Year
                                    && x.DateTime.Month == date.Month
                                    && x.DateTime.Day == date.Day).OrderByDescending(y => y.DateTime).FirstOrDefault();

            if (currencyExchangeRate == null)
                currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId).OrderByDescending(y => y.DateTime).First();
            return currencyExchangeRate;
        }

        public DeleteInvoiceResponseModel Delete(long id, long typeId)
        {
            var response = new DeleteInvoiceResponseModel();
            if (typeId == (long)ServiceFeeType.OneTimeProject)
            {
                var otpFee = _oneTimeProjectFeeRepository.Get(id);
                if (otpFee == null || otpFee.InvoiceItemId != null)
                {
                    response.Response = "Can't delete OneTime Project Fee.";
                    return response;
                }
                var allOTPFee = _oneTimeProjectFeeRepository.Fetch(x => x.FranchiseeId == otpFee.FranchiseeId);
                if (allOTPFee.Count() == 1)
                    response.IsLastItem = true;
                _oneTimeProjectFeeRepository.Delete(id);
                response.IsSuccess = true;
                response.Response = "OneTime Project Fee Has been deleted successfully";
            }
            else if (typeId == (long)ServiceFeeType.Loan)
            {
                var loan = _franchiseeLoanRepository.Get(id);
                if (loan == null)
                {
                    response.Response = "Can't find Loan Record.";
                    return response;
                }
                var allLoans = _franchiseeLoanRepository.Fetch(x => x.FranchiseeId == loan.FranchiseeId);
                if (allLoans.Count() == 1)
                    response.IsLastItem = true;

                var franchiseeLoanSchedule = loan.FranchiseeLoanSchedule;
                if (franchiseeLoanSchedule.Any(x => x.InvoiceItemId != null))
                {
                    response.Response = "Can't delete Loan Record, as One or more installments are already paid!";
                    return response;
                }
                else
                {
                    var scheduleIds = franchiseeLoanSchedule.Select(x => x.Id).ToList();
                    _franchiseeLoanScheduleRepository.Delete(x => scheduleIds.Contains(x.Id));
                }
                _franchiseeLoanRepository.Delete(id);
                response.IsSuccess = true;
                response.Response = "Loan Record has been deleted successfully!";
            }
            return response;
        }

        public FranchiseeServiceFeeListsModel GetOneTimeFeeList(long franchiseeId)
        {
            var isRoyality = true;
            var oneTimeProjectFeeList = _oneTimeProjectFeeRepository.Fetch(x => x.FranchiseeId == franchiseeId && x.Amount > 0);
            var result = _franchiseeServiceFeeFactory.CreateViewModel(oneTimeProjectFeeList).ToList();
            var onetimeprojectfeeAddFundRoyality = _onetimeprojectfeeAddFundRoyality.Table.FirstOrDefault(x => x.FranchiseeId == franchiseeId);
            if (onetimeprojectfeeAddFundRoyality != null)
            {
                isRoyality = onetimeprojectfeeAddFundRoyality.IsInRoyality;
            }
            var listModel = new FranchiseeServiceFeeListsModel
            {
                Collection = result,
                IsRoyality = isRoyality
            };
            return listModel;
        }

        public FranchiseeServiceFeeListsModel GetLoanList(long franchiseeId)
        {
            var loanList = _franchiseeLoanRepository.Fetch(x => x.FranchiseeId == franchiseeId && x.Amount > 0);
            var result = _franchiseeServiceFeeFactory.CreateViewModel(loanList);
            var totalUnPaidAmount = GetUnPaidAmountForLoan(result.Where(x => x.IsCompleted == false).Select(x => x.LoanScheduleList).ToList(), result.FirstOrDefault());
            var balanceLeft = new BalanceLeft();

            return new FranchiseeServiceFeeListsModel()
            {
                Balance = totalUnPaidAmount,
                Collection = result.ToList(),
                AmountPaid = GetPaidAmountForLoan(result.Select(x => x.LoanSchedule).ToList(), result.FirstOrDefault())
            };
        }

        public void Save(FranchiseeServiceFeeEditModel model)
        {
            CreateFranchiseeServiceFee(model);
            var franchisee = _franchiseeRepository.Get(model.FranchiseeId);
            var currencyExchangeRate = GetCurrencyExchangeRate(franchisee, _clock.UtcNow);
            model.CurrencyExchangeRateId = currencyExchangeRate.Id;

            if (model.TypeId == (long)ServiceFeeType.OneTimeProject)
            {
                var domain = _franchiseeServiceFeeFactory.CreateOneTimeProject(model);
                _oneTimeProjectFeeRepository.Save(domain);
            }
            else if (model.TypeId == (long)ServiceFeeType.Loan)
            {
                var domain = _franchiseeServiceFeeFactory.CreateFranchiseeLoan(model);
                var list = CalculateLoanSchedule(domain);
                domain.FranchiseeLoanSchedule = new List<FranchiseeLoanSchedule>();
                foreach (var item in list)
                {
                    var schedule = _franchiseeServiceFeeFactory.CreateDomain(item);
                    schedule.IsRoyality = model.IsRoyality.Value;
                    domain.IsRoyality = model.IsRoyality;
                    domain.FranchiseeLoanSchedule.Add(schedule);
                }
                _franchiseeLoanRepository.Save(domain);
            }
        }

        private ICollection<AmortPaymentSchedule> CalculateLoanSchedule(FranchiseeLoan loan, int term = 0)
        {
            DateTime payDate = loan.StartDate.GetValueOrDefault().AddMonths(-1);
            //DateTime payDate = _clock.UtcNow;
            double interestRate = 0;
            double loanAmount = Convert.ToDouble(loan.Amount);
            int amortizationTerm = loan.Duration;
            double currentBalance;
            int startTerm = 0;
            double cummulativePrincipal = 0;

            currentBalance = loanAmount;
            interestRate = Convert.ToDouble(loan.InterestratePerAnum) * 0.01;

            double monthlyPayment = 0;
            if (interestRate > 0)
                monthlyPayment = ((interestRate / 12) / (1 - (Math.Pow((1 + (interestRate / 12)), -(amortizationTerm))))) * loanAmount;
            else
                monthlyPayment = loanAmount / amortizationTerm;
            monthlyPayment = Math.Round(monthlyPayment, 2);
            if (term != 0)
            {
                startTerm = term;
            }
            var amortPaymentList = CalculateLoanTerms(startTerm, amortizationTerm, currentBalance, interestRate, payDate, monthlyPayment, cummulativePrincipal);
            return amortPaymentList;
        }




        public List<AmortPaymentSchedule> CalculateLoanTerms(int startTerm, int amortizationTerm, double currentBalance, double interestRate, DateTime payDate, double monthlyPayment,
            double cummulativePrincipal, bool isFromLastLoan = false)
        {
            double monthlyInterest = 0;
            double cummulativeInterest = 0;
            double monthlyPrincipal = 0;
            if (startTerm == 0 && isFromLastLoan)
            {
                cummulativePrincipal = 0;
                payDate = payDate.AddMonths(-1);
            }
            if (isFromLastLoan)
            {
                cummulativePrincipal = 0;
            }
            List<AmortPaymentSchedule> amortPaymentList = new List<AmortPaymentSchedule>();

            for (int j = startTerm; j < amortizationTerm; j++)
            {
                monthlyInterest = currentBalance * interestRate / 12;
                monthlyPrincipal = monthlyPayment - monthlyInterest;
                currentBalance = currentBalance - monthlyPrincipal;

                if (j == amortizationTerm - 1 || currentBalance <= 0)
                {
                    // Adjust the last payment to make sure the final balance is 0
                    monthlyPayment += currentBalance;
                    monthlyPrincipal = (monthlyPayment - monthlyInterest);
                    currentBalance = 0;
                }

                // Reset Date
                payDate = payDate.AddMonths(1);
                // Add to cummulative totals
                cummulativeInterest += monthlyInterest;
                cummulativePrincipal += monthlyPrincipal;

                amortPaymentList.Add
                    (new AmortPaymentSchedule
                    {
                        TermNumber = j + 1,
                        Date = payDate,
                        ScheduledPayment = Math.Round(monthlyPayment, 2),
                        Interest = Math.Round(monthlyInterest, 2),
                        Balance = Math.Round(currentBalance, 2),
                        TotalInterest = Math.Round(cummulativeInterest, 2),
                        Totalprincipal = Math.Round(cummulativePrincipal, 2),
                        Principal = Math.Round(monthlyPrincipal, 2),
                    }); ;
            }
            return amortPaymentList;
        }

        private void CreateFranchiseeServiceFee(FranchiseeServiceFeeEditModel model)
        {
            var serviceFee = _franchiseeServiceFeeRepository.Get(x => x.FranchiseeId == model.FranchiseeId && x.ServiceFeeTypeId == model.TypeId);
            if (serviceFee == null)
            {
                model.IsApplicable = true;
                var domain = _franchiseeServiceFeeFactory.CreateDomainForServiceFee(model);
                _franchiseeServiceFeeRepository.Save(domain);
            }
            if (model.TypeId == (long)ServiceFeeType.Loan && (_franchiseeServiceFeeRepository.Get(x => x.FranchiseeId == model.FranchiseeId
                                                            && x.ServiceFeeTypeId == (long)ServiceFeeType.InterestAmount) == null))
            {
                model.IsApplicable = true;
                model.TypeId = (long)ServiceFeeType.InterestAmount;
                var interestAmountFee = _franchiseeServiceFeeFactory.CreateDomainForServiceFee(model);
                _franchiseeServiceFeeRepository.Save(interestAmountFee);
                model.TypeId = (long)ServiceFeeType.Loan;
            }
        }
        public bool SaveLoanAdjustmentType(FranchiseeChangeServiceFee model)
        {
            try
            {
                var loanList = _franchiseeLoanRepository.Table.FirstOrDefault(x => x.Id == model.LoanId);

                SaveFranchiseeLoanAudit(model, loanList);

                loanList.IsRoyality = model.IsRoyality;
                _franchiseeLoanRepository.Save(loanList);
                var loanSchedules = _franchiseeLoanScheduleRepository.Table.Where(x => x.LoanId == model.LoanId && !x.IsPrePaid && x.InvoiceItemId == null).ToList();
                foreach (var loanSchedule in loanSchedules)
                {
                    loanSchedule.IsRoyality = model.IsRoyality.Value;
                    loanSchedule.IsNew = false;
                    _franchiseeLoanScheduleRepository.Save(loanSchedule);
                }

                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        private bool SaveFranchiseeLoanAudit(FranchiseeChangeServiceFee model, FranchiseeLoan franchiseeLoan)
        {
            var loanId = franchiseeLoan.Id;
            try
            {
                var domain = _franchiseeServiceFeeFactory.CreateDomain(model, franchiseeLoan);
                _loanAdjustmentAuditRepository.Save(domain);
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error {{0}} For Updating Loan Id: {{1}}", ex.Message, loanId));
                return false;
            }
        }


        public bool GetFranchiseeRoyality(long? franchiseeId)
        {
            try
            {
                var franchisee = _franchiseeRepository.Table.FirstOrDefault(x => x.Id == franchiseeId);
                return franchisee.IsRoyality;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        public FranchiseeTeamImageViewModel GetFranchiseeTeamImage(long? franchiseeId)
        {
            try
            {
                string relativeLocation = (MediaLocationHelper.GetTempImageLocation().Path) + "\\";
                var franchisee = _franchiseeRepository.Table.FirstOrDefault(x => x.Id == franchiseeId);

                return new FranchiseeTeamImageViewModel
                {
                    FileId = franchisee != null ? franchisee.FileId : default(long?),
                    FranchiseeTeamImage = (franchisee != null && franchisee.FileId != null && franchisee.File != null) ? (relativeLocation + franchisee.File.Name) : "",
                    ImageName = (franchisee != null && franchisee.FileId != null && franchisee.File != null) ? (franchisee.File.Name) : "",
                    FranchiseeName = (franchisee != null && franchisee.Organization != null) ? franchisee.Organization.Name : ""
                };
            }
            catch (Exception e1)
            {
                return new FranchiseeTeamImageViewModel
                {
                    FileId = default(long?),
                    FranchiseeTeamImage = ""
                };
            }
        }

        public bool SaveFranchiseeTeamImage(FranchiseeTeamImageEditModel franchiseeTeamImage)
        {
            try
            {
                var franchiseeDomain = _franchiseeRepository.Get(franchiseeTeamImage.FranchiseeId.GetValueOrDefault());
                if (franchiseeTeamImage.FileUploadModel != null && franchiseeTeamImage.FileUploadModel.FileList.Count() > 0)
                {
                    long fileId = SavesImage(franchiseeTeamImage.FileUploadModel);
                    franchiseeDomain.FileId = fileId;
                    franchiseeDomain.IsNew = false;
                    _franchiseeRepository.Save(franchiseeDomain);
                    return true;
                }
                else
                {
                    franchiseeDomain.FileId = null;
                    franchiseeDomain.IsNew = false;
                    _franchiseeRepository.Save(franchiseeDomain);

                    return true;
                }
            }

            catch (Exception e1)
            {
                return false;
            }
        }
        public long SavesImage(FileUploadModel model)
        {
            foreach (var fileModel in model.FileList)
            {
                try
                {
                    if (fileModel.Id > 0)
                    {
                        var fileRepository = _fileRepository.Get(fileModel.Id);
                        fileRepository.IsNew = false;
                        fileRepository.css = model.css;
                        _fileRepository.Save(fileRepository);
                        continue;
                    }
                    var path = MediaLocationHelper.FilePath(fileModel.RelativeLocation, fileModel.Name).ToFullPath();
                    var destination = MediaLocationHelper.GetTempImageLocation();
                    var destFileName = string.Format((fileModel.Caption.Length <= 20) ? fileModel.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)
                                        : fileModel.Caption.Substring(0, 20) + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow));
                    var fileName = _fileService.MoveFile(path, destination, destFileName, fileModel.Extension);
                    fileModel.Name = destFileName + fileModel.Extension;

                    fileModel.RelativeLocation = MediaLocationHelper.GetTempImageLocation().Path;

                    string folderName = Path.GetFileName(fileModel.RelativeLocation);
                    fileModel.css = model.css;
                    fileModel.RelativeLocation = "\\" + folderName;
                    var file = _fileService.SaveModel(fileModel);

                    return file.Id;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return default(long);
        }

        public bool DownloadFranchiseeLoan(long? franchiseeLoanId, out string fileName)
        {
            fileName = string.Empty;
            var reportCollection = new List<FranchiseeLoanViewModel>();
            IEnumerable<FranchiseeLoanSchedule> reportList = GetFranchiseeLoan(franchiseeLoanId).ToList();
            foreach (var item in reportList)
            {
                var model = _franchiseeServiceFeeFactory.CreateViewModel(item);
                reportCollection.Add(model);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/franchiseeLoan-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }


        public List<FranchiseeLoanSchedule> GetFranchiseeLoan(long? franchiseeLoanId)
        {
            return _franchiseeLoanScheduleRepository.Table.Where(x => x.LoanId == franchiseeLoanId && !x.IsPrePaid).ToList();
        }
        private BalanceLeft GetUnPaidAmountForLoan(List<List<LoanScheduleViewModel>> loanScheduleList, FranchiseeServiceFeeEditModel editModel)
        {
            var balanceLeft = new BalanceLeft();

            foreach (var loanSchedule in loanScheduleList)
            {
                var alreadParsed = false;
                var loanDomain = loanSchedule.OrderBy(x => x.Id).FirstOrDefault();
                var loansWithoutInvoice = loanSchedule.Where(x => x.InvoiceItemId == " ").ToList();
                balanceLeft.Balance += loansWithoutInvoice.Sum(x => x.PayableAmount);
                balanceLeft.CurrencyCode = editModel != null ? editModel.CurrencyCode : "";
                balanceLeft.CurrencyRate = editModel != null ? editModel.CurrencyRate : default(decimal);
                //if (loanDomain != null && loanDomain.InvoiceItemId == " ")
                //{
                //    alreadParsed = true;
                //    balanceLeft.Balance += loanDomain.PayableAmount;
                //    balanceLeft.CurrencyCode = editModel != null ? editModel.CurrencyCode : "";
                //    balanceLeft.CurrencyRate = editModel != null ? editModel.CurrencyRate : default(decimal);
                //}
                //var loanWithoutInvoiceItem = loanSchedule.OrderByDescending(x => x.Id).FirstOrDefault(x => x.InvoiceItemId != " ");
                //if (loanWithoutInvoiceItem != null && !alreadParsed)
                //{
                //    balanceLeft.Balance += loanWithoutInvoiceItem.PayableAmount;
                //    balanceLeft.CurrencyCode = editModel != null ? editModel.CurrencyCode : "";
                //    balanceLeft.CurrencyRate = editModel != null ? editModel.CurrencyRate : default(decimal);
                //}
                //balanceLeft.Balance = (balanceLeft.Balance - loanSchedule.Sum(x => x.OverPayment));
            }

            return balanceLeft;
        }


        private BalanceLeft GetPaidAmountForLoan(List<ICollection<LoanScheduleViewModel>> loanScheduleList, FranchiseeServiceFeeEditModel editModel)
        {
            var balanceAlreadyPaid = new BalanceLeft();
            foreach (var loanSchedule in loanScheduleList)
            {
                var alreadyPaidAmountList = loanSchedule.Where(x => x.InvoiceItemId != " ").ToList();
                if (alreadyPaidAmountList.Count() > 0)
                {
                    balanceAlreadyPaid.Balance += (alreadyPaidAmountList.Sum(x => x.PayableAmount));
                    balanceAlreadyPaid.AmountActuallyPaid += (alreadyPaidAmountList.Sum(x => x.PayableAmount));
                    balanceAlreadyPaid.CurrencyCode = editModel != null ? editModel.CurrencyCode : "";
                    balanceAlreadyPaid.CurrencyRate = editModel != null ? editModel.CurrencyRate : default(decimal);
                }
                balanceAlreadyPaid.Balance += loanSchedule.Sum(x => x.OverPayment);
                balanceAlreadyPaid.OverPaidAmount += loanSchedule.Sum(x => x.OverPayment);
            }

            return balanceAlreadyPaid;
        }

        public bool SavePrePayLoan(FranchiseePrePayLoanFeeEditModel model)
        {
            var lastLoanSchedulerList = new List<FranchiseeLoanSchedule>();
            var franchiseeLoan = _franchiseeLoanScheduleRepository.Table.Where(x => x.LoanId == model.Id).ToList();
            var lastLoanScheduler = franchiseeLoan.OrderByDescending(x => x.Id).FirstOrDefault(x => x.InvoiceItemId != null);
            var term = default(int);
            var isOverPaid = false;
            var isOverPaidLoan = false;
            if (lastLoanScheduler != default(FranchiseeLoanSchedule))
            {
                var amountInDefaultCurrency = model.PrePayAmount.ToDefaultCurrency(lastLoanScheduler.FranchiseeLoan.CurrencyExchangeRate.Rate);

                if (lastLoanScheduler.OverPaidAmount == default(decimal))
                {
                    isOverPaid = true;
                    lastLoanScheduler.OverPaidAmount = amountInDefaultCurrency;
                    lastLoanScheduler.FranchiseeLoan.LoanTypeId = model.LoanTypeId;
                }
                else
                {
                    isOverPaid = true;
                    lastLoanScheduler.OverPaidAmount = amountInDefaultCurrency + lastLoanScheduler.OverPaidAmount;
                    lastLoanScheduler.FranchiseeLoan.LoanTypeId = model.LoanTypeId;
                }

                if (amountInDefaultCurrency >= lastLoanScheduler.Balance)
                {
                    AccountToBeCredited(((amountInDefaultCurrency) - (lastLoanScheduler.Balance)), lastLoanScheduler.FranchiseeLoan);
                    amountInDefaultCurrency = lastLoanScheduler.Balance;
                    lastLoanScheduler.FranchiseeLoan.IsCompleted = true;
                    isOverPaidLoan = true;
                    lastLoanScheduler.PayableAmount = 0;
                    lastLoanScheduler.Balance = 0;
                }
                lastLoanScheduler.Balance = lastLoanScheduler.Balance - amountInDefaultCurrency;

                _franchiseeLoanScheduleRepository.Save(lastLoanScheduler);

                term = lastLoanScheduler.LoanTerm;
                lastLoanSchedulerList.Add(lastLoanScheduler);
            }
            else
            {
                lastLoanScheduler = franchiseeLoan.OrderBy(x => x.Id).FirstOrDefault();
                term = 0;
                var amountInDefaultCurrency = model.PrePayAmount.ToDefaultCurrency(lastLoanScheduler.FranchiseeLoan.CurrencyExchangeRate.Rate);

                if (lastLoanScheduler.OverPaidAmount == default(decimal))
                {
                    lastLoanScheduler.OverPaidAmount = amountInDefaultCurrency;
                    if (amountInDefaultCurrency >= lastLoanScheduler.FranchiseeLoan.Amount)
                    {
                        AccountToBeCredited(((amountInDefaultCurrency) - (lastLoanScheduler.FranchiseeLoan.Amount)), lastLoanScheduler.FranchiseeLoan);
                        amountInDefaultCurrency = lastLoanScheduler.Balance;
                        lastLoanScheduler.FranchiseeLoan.IsCompleted = true;
                        lastLoanScheduler.FranchiseeLoan.LoanTypeId = model.LoanTypeId;
                        isOverPaidLoan = true;
                        lastLoanScheduler.PayableAmount = 0;
                        lastLoanScheduler.Balance = 0;
                    }
                    else
                    {
                        lastLoanScheduler.Balance = lastLoanScheduler.FranchiseeLoan.Amount - amountInDefaultCurrency;
                    }

                }
                else
                {
                    var overPaidAmount = amountInDefaultCurrency;

                    if ((amountInDefaultCurrency + lastLoanScheduler.OverPaidAmount) >= lastLoanScheduler.FranchiseeLoan.Amount)
                    {

                        AccountToBeCredited(((amountInDefaultCurrency + lastLoanScheduler.OverPaidAmount) - (lastLoanScheduler.FranchiseeLoan.Amount)), lastLoanScheduler.FranchiseeLoan);
                        amountInDefaultCurrency = lastLoanScheduler.Balance;
                        lastLoanScheduler.FranchiseeLoan.IsCompleted = true;
                        lastLoanScheduler.PayableAmount = 0;
                        lastLoanScheduler.Balance = 0;
                        lastLoanScheduler.FranchiseeLoan.LoanTypeId = model.LoanTypeId;
                        isOverPaidLoan = true;
                    }
                    else
                    {
                        lastLoanScheduler.Balance = lastLoanScheduler.FranchiseeLoan.Amount - (amountInDefaultCurrency + lastLoanScheduler.OverPaidAmount);
                    }
                    lastLoanScheduler.OverPaidAmount = overPaidAmount + lastLoanScheduler.OverPaidAmount;
                }

                lastLoanScheduler.FranchiseeLoan.LoanTypeId = model.LoanTypeId;
                _franchiseeLoanScheduleRepository.Save(lastLoanScheduler);
                lastLoanSchedulerList.Add(lastLoanScheduler);
            }
            if (!isOverPaidLoan)
            {
                CheckingForNotOverPaidLoan(lastLoanSchedulerList, term, isOverPaid);
            }
            else
            {
                foreach (var loan in franchiseeLoan.Where(x => x.LoanTerm > lastLoanScheduler.LoanTerm))
                {
                    loan.IsPrePaid = true;
                    _franchiseeLoanScheduleRepository.Save(loan);
                }
            }
            _unitOfWork.SaveChanges();
            return true;
        }

        private void AccountToBeCredited(decimal amountToBeCredited, FranchiseeLoan franchiseeLoan)
        {
            var franchiseeCredit = new FranchiseeAccountCredit()
            {
                Amount = amountToBeCredited,
                CreditedOn = DateTime.UtcNow,
                CreditTypeId = franchiseeLoan.IsRoyality.GetValueOrDefault() ? 162 : 161,
                Description = "Adding OverPaid Amount Of $" + amountToBeCredited + " for LoanId " + franchiseeLoan.Id + " having description " + franchiseeLoan.Description,
                FranchiseeId = franchiseeLoan.FranchiseeId,
                CurrencyExchangeRateId = franchiseeLoan.CurrencyExchangeRateId,
                IsNew = true,
                RemainingAmount = 0
            };
            _franchiseeAccountCreditRepository.Save(franchiseeCredit);
        }

        private void CheckingForNotOverPaidLoan(List<FranchiseeLoanSchedule> loanScheduleList, int term, bool isOverPaid)
        {
            foreach (var loanSchedule in loanScheduleList)
            {
                try
                {
                    //Reschedule loan
                    var loan = loanSchedule.FranchiseeLoan;
                    var newSchedules = CalculateLoanTerms(term, loan.Duration,
                       Convert.ToDouble(loanSchedule.Balance),
                        Convert.ToDouble(loan.InterestratePerAnum) * 0.01, loanSchedule.DueDate, Convert.ToDouble(loanSchedule.PayableAmount),
                        Convert.ToDouble(loanSchedule.TotalPrincipal), true);

                    //update schedule
                    var isFirst = 0;
                    foreach (var item in newSchedules)
                    {
                        item.LoanId = loan.Id;
                        var indbLoanSchedule = loan.FranchiseeLoanSchedule.Where(x => x.LoanId == loan.Id && x.LoanTerm == item.TermNumber).FirstOrDefault();
                        if (indbLoanSchedule != null)
                            item.Id = indbLoanSchedule.Id;
                        var schedule = _franchiseeServiceFeeFactory.CreateDomain(item);
                        schedule.IsRoyality = indbLoanSchedule.IsRoyality;
                        if (loanSchedule.Balance < 0)
                        {
                            schedule.IsPrePaid = true;
                        }
                        if (item.ScheduledPayment == 0)
                        {
                            schedule.IsPrePaid = true;
                        }
                        if (isFirst == 0)
                        {
                            schedule.OverPaidAmount = loanSchedule.OverPaidAmount;
                            ++isFirst;
                            isOverPaid = false;
                        }
                        _franchiseeLoanScheduleRepository.Save(schedule);
                    }

                    loanSchedule.CalculateReschedule = false;
                    _franchiseeLoanScheduleRepository.Save(loanSchedule);
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logService.Error(ex.StackTrace);
                }
            }
        }

        public bool SaveFranchisseeNotes(FranchiseeNotesDurationViewModel model)
        {
            try
            {
                var franchisee = _franchiseeRepository.Table.FirstOrDefault(x => x.Id == model.FranchiseeId);
                var franchisseeNotes = new FranchiseeDurationNotesHistry()
                {
                    DataRecorderMetaData = new DataRecorderMetaData(),
                    Description = model.Description,
                    FranchiseeId = model.FranchiseeId,
                    StatusId = (model.RoleId == ((long)RoleType.SuperAdmin) || model.RoleId == ((long)RoleType.FrontOfficeExecutive)) ? (long)AuditActionType.Approved : (long)AuditActionType.Pending,
                    TypeId = model.TypeId,
                    UserId = model.UserId,
                    IsNew = true,
                    Duration = (model.TypeId == (long)FranchiseeNotesEnum.FRANCHISEEDURATION) ? model.Duration : default(long?),
                    RoleId = model.RoleId,
                    ApprovedById = null
                };
                if (model.TypeId == (long)FranchiseeNotesEnum.NOTESFROMCALLCENTER || model.TypeId == (long)FranchiseeNotesEnum.NOTESFROMOWNER)
                {

                    if (model.TypeId == (long)FranchiseeNotesEnum.NOTESFROMCALLCENTER)
                    {
                        franchisee.NotesFromCallCenter = model.Description;
                    }
                    else
                    {
                        franchisee.NotesFromOwner = model.Description;
                    }
                    _franchiseeRepository.Save(franchisee);
                }
                else if ((model.RoleId == ((long)RoleType.SuperAdmin) || (model.RoleId == ((long)RoleType.FrontOfficeExecutive))) && model.TypeId == (long)FranchiseeNotesEnum.FRANCHISEEDURATION)
                {
                    franchisee.Duration = model.Duration;
                    _franchiseeRepository.Save(franchisee);
                }
                _franchiseeDurationNotesHistryRepository.Save(franchisseeNotes);
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        public DurationApprovalListModel GetDurationApprovalList(long franchiseeId, long? roleId)
        {
            var durationApprovalList = _franchiseeDurationNotesHistryRepository.Table.Where(x => x.FranchiseeId == franchiseeId).OrderByDescending(x => x.DataRecorderMetaData.DateCreated).ToList();

            var durationApprovalViewModel = durationApprovalList.Select(x => CreateApprovalViewModel(x)).ToList();
            return new DurationApprovalListModel()
            {
                Collection = durationApprovalViewModel
            };
        }

        private DurationApprovalViewModel CreateApprovalViewModel(FranchiseeDurationNotesHistry domain)
        {
            return new DurationApprovalViewModel()
            {
                Id = domain.Id,
                AddDate = domain.DataRecorderMetaData.DateCreated,
                Duration = domain.Duration,
                FranchiseeName = domain.Franchisee.Organization.Name,
                UserName = domain.Person.Name.FirstName + " " + domain.Person.Name.LastName,
                StatusId = domain.StatusId,
                ApprovedByUserName = domain.ApprovedBy != null ? domain.ApprovedBy.FirstName + " " + domain.ApprovedBy.LastName : "",
                AddDateTime = domain.DataRecorderMetaData.DateModified
            };
        }

        public bool ChangeDurationStatus(DurationApprovalFilterModel model)
        {
            try
            {
                var franchiseeDurationModel = _franchiseeDurationNotesHistryRepository.Get(model.Id);
                franchiseeDurationModel.StatusId = model.StatusId;
                franchiseeDurationModel.ApprovedById = model.UserId;
                _franchiseeDurationNotesHistryRepository.Save(franchiseeDurationModel);
                var franchisee = _franchiseeRepository.Get(franchiseeDurationModel.FranchiseeId);
                franchisee.Duration = franchiseeDurationModel.Duration;
                _franchiseeRepository.Save(franchisee);
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        private string GetLoanDescription(FranchiseeLoan franchiseeLoan)
        {
            if (franchiseeLoan.LoanTypeId == (long?)(LoanType.Geofence))
            {
                return "GEOFencing";
            }
            else if (franchiseeLoan.LoanTypeId == (long?)(LoanType.ISQFT))
            {
                return "ISQFT-Construct Connect";
            }
            else if (franchiseeLoan.LoanTypeId == (long?)(LoanType.Other))
            {
                return "Uncategorized Income";
            }
            else if (franchiseeLoan.LoanTypeId == (long?)(LoanType.SurgicalStrike))
            {
                return "SURGICAL STRIKE Marble Home Lst";
            }
            return "";
        }

        public void SavePhoneChargeFess(FranchiseeInvoice frInvoice, IEnumerable<Phonechargesfee> feeList)
        {
            var serviceFee = frInvoice.Franchisee.FranchiseeServiceFee.Where(x => x.ServiceFeeTypeId == (long)ServiceFeeType.PHONECALLCHARGES).FirstOrDefault();
            if (serviceFee != null)
            {
                foreach (var fee in feeList)
                {
                    decimal amount = fee.Amount;
                    int qty = 1;
                    decimal rate = serviceFee.Amount;
                    var startDate = frInvoice.SalesDataUpload.PeriodStartDate;
                    var endDate = frInvoice.SalesDataUpload.PeriodEndDate;

                    var model = CreateModelForPhone(serviceFee, frInvoice, amount, rate, qty, fee);
                    var invoiceItemId = Save(model, serviceFee, frInvoice.InvoiceId);

                    fee.InvoiceItemId = invoiceItemId;
                    fee.IsInvoiceInQueue = false;
                    fee.IsInvoiceGenerated = true;
                    _phonechargesfeeRepository.Save(fee);
                }
            }
        }

        public InvoiceItemEditModel CreateModelForPhone(FranchiseeServiceFee serviceFee, FranchiseeInvoice invoice, decimal amount, decimal rate, int qty, Phonechargesfee phoneCharges,
           DateTime? startDate = null, DateTime? endDate = null)
        {
            var currencyExchangeRate = GetCurrencyExchangeRate(invoice.Franchisee, _clock.UtcNow);

            if (phoneCharges != null)
            {
                startDate = new DateTime(phoneCharges.DateCreated.Year, phoneCharges.DateCreated.Month, 1);
                endDate = new DateTime(phoneCharges.DateCreated.Year, phoneCharges.DateCreated.Month, DateTime.DaysInMonth(phoneCharges.DateCreated.Year, phoneCharges.DateCreated.Month));
            }
            var model = new ServiceFeeInvoiceItemEditModel
            {
                ItemTypeId = (long)InvoiceItemType.ServiceFee,
                InvoiceId = invoice.InvoiceId,
                Quantity = qty,
                Rate = rate,
                Description = phoneCharges.Franchiseetechmailservice.DateForCharges.GetValueOrDefault().Year + "-" + phoneCharges.Franchiseetechmailservice.DateForCharges.GetValueOrDefault().ToString("MMMM") + " Back Up Charges on " + phoneCharges.Franchiseetechmailservice.CallCount + "*" + phoneCharges.Franchiseetechmailservice.ChargesForPhone + " = " + phoneCharges.Amount + " (See Phone data for call details)",
                Amount = amount,
                CurrencyExchangeRateId = currencyExchangeRate.Id,
                ServiceFeeTypeId = serviceFee.ServiceFeeTypeId,
                StartDate = startDate.Value != null ? startDate.Value : invoice.SalesDataUpload.PeriodStartDate,
                Percentage = serviceFee.Percentage,
                EndDate = endDate.Value != null ? endDate.Value : invoice.SalesDataUpload.PeriodEndDate
            };
            return model;
        }

        public string GetDescriptionForServiceFee(FranchiseeServiceFee serviceFee, string month, string year)
        {
            var description = default(string);
            if (serviceFee.ServiceFeeTypeId == (long)ServiceFeeType.OneTimeProject)
            {
                description = "Service Fee Charged For " + month + ", " + year + " for " + "One time Charge";
            }
            else
            {
                description = "Service Fee Charged For " + month + ", " + year + " for " + serviceFee.ServiceFeeType.Name;
            }
            return description;
        }
    }
}
