using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Geo;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class InvoiceFactory : IInvoiceFactory
    {
        private readonly IPaymentFactory _paymentFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceItemFactory _invoiceItemFactory;
        private readonly IFranchiseeSalesPaymentService _franchiseeSalesPaymentService;
        private readonly IClock _clock;
        private readonly IAddressFactory _addressFactory;
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private readonly IRepository<FranchiseeLoanSchedule> _franchiseeLoanScheduleRepository;
        private readonly IRepository<OneTimeProjectFee> _oneTimeProjectFeeRepository;
        private readonly IRepository<ServiceFeeInvoiceItem> _serviceFeeInvoiceItem;
        public InvoiceFactory(IUnitOfWork unitOfWork, IPaymentFactory paymentFactory, IInvoiceItemFactory invoiceItemFactory,
            IFranchiseeSalesPaymentService franchiseeSalesPaymentService, IClock clock, IAddressFactory addressFactory)
        {
            _paymentFactory = paymentFactory;
            _invoiceItemFactory = invoiceItemFactory;
            _franchiseeSalesPaymentService = franchiseeSalesPaymentService;
            _clock = clock;
            _addressFactory = addressFactory;
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _franchiseeLoanScheduleRepository = unitOfWork.Repository<FranchiseeLoanSchedule>();
            _oneTimeProjectFeeRepository = unitOfWork.Repository<OneTimeProjectFee>();
            _serviceFeeInvoiceItem = unitOfWork.Repository<ServiceFeeInvoiceItem>();
        }

        public Invoice CreateDomain(InvoiceEditModel model)
        {
            return new Invoice()
            {
                IsNew = model.Id < 1,
                Id = model.Id,
                GeneratedOn = model.GeneratedOn,
                DueDate = model.DueDate,
                StatusId = model.StatusId,

                //  InvoicePayments = model.Payments != null ? new List<Payment>(model.Payments.Select(x => _paymentFactory.CreateDomain(x))) : null,
                InvoiceItems = model.InvoiceItems != null ? new List<InvoiceItem>(model.InvoiceItems.Select(x => _invoiceItemFactory.CreateDomain(x))) : null,
                DataRecorderMetaData = model.DataRecorderMetaData
            };
        }



        public InvoiceViewModel CreateViewModel(FranchiseeInvoice domain)
        {
            var metaDatadate = default(DateTime);
            const string Credit = "Account Credit";
            var currencyRate = domain.Invoice.InvoiceItems.Select(x => x.CurrencyExchangeRate).FirstOrDefault();
            var adFund = GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.AdFund);
            var royalty = GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.RoyaltyFee);
            var accountCredit = domain.Franchisee.FranchiseeAccountCredit != null ? domain.Franchisee.FranchiseeAccountCredit.Sum(x => x.RemainingAmount) : 0;
            var paymentMode = _franchiseeSalesPaymentService.GetPaymentInstrument(domain.Invoice.InvoicePayments);

            if (paymentMode == null && domain.Invoice.StatusId == (long)InvoiceStatus.Paid
                && domain.Invoice.InvoicePayments.Any(x => x.Payment.InstrumentTypeId == (long)InstrumentType.AccountCredit))
            {
                paymentMode = Credit;
            }
            var itemTypeId = default(long);
            if(domain.Invoice!= null && domain.Invoice.InvoiceItems.Count > 0)
            {
                itemTypeId = domain.Invoice.InvoiceItems.Select(x => x.ItemTypeId).First();
            }

            #region lateFee Info
            var royaltyLateFee = GetSumLateFeeBasedonItemType(domain.Invoice, LateFeeType.Royalty);
            var salesDtataLateFee = GetSumLateFeeBasedonItemType(domain.Invoice, LateFeeType.SalesData);
            var interestRate = GetSumInvoiceItembasedonItemType(domain.Invoice, InvoiceItemType.InterestRatePerAnnum);
            var fixedAccountingCharge = GetFixedAccountingChargebasedonItemType(domain.Invoice, InvoiceItemType.ServiceFee);
            var variableAccountingCharge = GetVariableAccountingChargebasedonItemType(domain.Invoice, InvoiceItemType.ServiceFee);
            var ISQFT = GetSumISQFTChargebasedonItemType(domain.Invoice, InvoiceItemType.LoanServiceFee);
            //var ISQFT1 = GetSumISQFTChargebasedonItemType(domain.Invoice, InvoiceItemType.LoanServiceFeeInterestRatePerAnnum);
            var LoanAndLoanInterestCharges = GetSumLoanAndLoanInterestChargebasedonItemType(domain.Invoice);
            var oneTimeCharges = GetOneTimeChargesbasedonItemType(domain.Invoice, InvoiceItemType.ServiceFee);
            var notes = GetOneTimeNote(domain.Invoice);
            var RecruitingFeeCharges = GetRecruitingFeeChargesbasedonItemType(domain.Invoice, InvoiceItemType.ServiceFee);
            var PayrollProcessingCharges = GetPayrollProcessingChargesbasedonItemType(domain.Invoice, InvoiceItemType.ServiceFee);
            var minRoaltyAmount = GetMinRoyalityAmount(domain.Invoice.InvoiceItems.ToList());
            var nationalCharges = GetNationalChargesbasedonItemType(domain.Invoice, InvoiceItemType.ServiceFee);
            var franchiseeEmailFeeCharges = GetFranchiseeEmailFeeChargesbasedonItemType(domain.Invoice, InvoiceItemType.FranchiseeTechMail);

            #endregion

            var startDate = _clock.UtcNow;
            var endDate = _clock.UtcNow;
            if (itemTypeId == (long)InvoiceItemType.LateFees)
            {
                metaDatadate = domain.Invoice.DataRecorderMetaData.DateCreated;
                startDate = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null).OrderByDescending(x => x.LateFeeInvoiceItem.EndDate).Select(x => x.LateFeeInvoiceItem.StartDate).FirstOrDefault();
                endDate = domain.Invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null).OrderByDescending(x => x.LateFeeInvoiceItem.EndDate).Select(x => x.LateFeeInvoiceItem.EndDate).FirstOrDefault();
            }
            else
            {
                startDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodStartDate : _clock.UtcNow;
                endDate = domain.SalesDataUpload != null ? domain.SalesDataUpload.PeriodEndDate : _clock.UtcNow;
            }
            var SECCostApplied = false;
            var WebSEOCharge = default(decimal);
            if (domain.Invoice.InvoiceItems.Count > 0)
            {
                SECCostApplied = domain.Invoice.InvoiceItems.Where(x => x.ServiceFeeInvoiceItem != null && x.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.SEOCharges).FirstOrDefault() != null ? true : false;
                if(SECCostApplied)
                {
                    WebSEOCharge = GetSumSEOChargebasedonItemType(domain.Invoice, InvoiceItemType.ServiceFee);
                }
            }

            var BackUpPhoneNumberChargestApplied = false;
            var BackUpPhoneNumberCharges = default(decimal);
            if (domain.Invoice.InvoiceItems.Count > 0)
            {
                BackUpPhoneNumberChargestApplied = domain.Invoice.InvoiceItems.Where(x => x.ServiceFeeInvoiceItem != null && x.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)Organizations.Enum.ServiceFeeType.PHONECALLCHARGES).FirstOrDefault() != null ? true : false;
                if (BackUpPhoneNumberChargestApplied)
                {
                    BackUpPhoneNumberCharges = GetSumBackUpPhoneNumberChargebasedonItemType(domain.Invoice, InvoiceItemType.ServiceFee);
                }
            }

            var a = new InvoiceViewModel
            {
                InvoiceId = domain.Invoice.Id,
                FranchiseeName = domain.Franchisee.Organization.Name,
                FranchiseeId = domain.Franchisee.Id,
                StartDate = startDate,
                EndDate = endDate,
                DueDate = domain.Invoice.DueDate,
                TotalSales = domain.SalesDataUpload != null ? domain.SalesDataUpload.TotalAmount : 0,
                PaidAmount = domain.SalesDataUpload != null ? domain.SalesDataUpload.PaidAmount : 0,
                AccruedAmount = domain.SalesDataUpload != null ? (domain.SalesDataUpload.TotalAmount) - (domain.SalesDataUpload.PaidAmount) : 0,
                PayableAmount = domain.Invoice.InvoiceItems.Count > 0 ? domain.Invoice.InvoiceItems.ToList().Sum(x => x.Amount) : default(decimal),
                Status = domain.Invoice.Lookup.Name != "$0-DUE" ? domain.Invoice.Lookup.Name : GetInvoiceStatus(domain.Invoice),
                StatusId = domain.Invoice.StatusId,
                CurrencyRate = currencyRate != null ? currencyRate.Rate : 1,
                CurrencyCode = domain.Franchisee.Currency,
                AdFund = adFund,
                Royalty = royalty,
                AccountTypeId = domain.Invoice.InvoiceItems.Count > 0 ? GetProfileType(domain.Invoice.InvoiceItems.Select(x => x.ItemTypeId).First()) : 0,
                AccountCredit = accountCredit,
                PaymentMode = paymentMode != null ? paymentMode : "N/A",
                InterestRate = interestRate,
                RoyaltyLateFee = royaltyLateFee,
                SalesDataLateFee = salesDtataLateFee,
                IsDownloaded = domain.IsDownloaded,
                PaymentDate = domain.Invoice.InvoicePayments.FirstOrDefault() != null ? domain.Invoice.InvoicePayments.FirstOrDefault().Payment.Date.ToShortDateString() : "N/A",
                MinRoyaltyAmount = minRoaltyAmount,
                UploadedOn = domain.SalesDataUpload != null ? domain.SalesDataUpload.DataRecorderMetaData.DateCreated.Date : metaDatadate,
                IsSEOCostApplied = SECCostApplied,
                FixedAccountingCharges = fixedAccountingCharge,
                VariableAccountingCharges = variableAccountingCharge,
                LoanAndLoanIntCharges = LoanAndLoanInterestCharges,
                ISQFTCharges = ISQFT,
                WebSEOCharges = WebSEOCharge,
                BackUpPhoneNumber = BackUpPhoneNumberCharges,
                OneTimeCharges = oneTimeCharges,
                RoyalityOrAddFund = GetInvoiceItemType(domain.Invoice),
                OneTimeNote = notes != null ? notes : null,
                RecruitingFee = RecruitingFeeCharges,
                PayrollProcessing = PayrollProcessingCharges,
                ReconciliationNotes = domain.Invoice.ReconciliationNotes,
                NationalCharges = nationalCharges,
                FranchiseeEmailFee = franchiseeEmailFeeCharges,
                CheckSum = adFund + royalty + minRoaltyAmount + royaltyLateFee + salesDtataLateFee + interestRate + fixedAccountingCharge + variableAccountingCharge + LoanAndLoanInterestCharges + ISQFT + WebSEOCharge + BackUpPhoneNumberCharges + oneTimeCharges + RecruitingFeeCharges + PayrollProcessingCharges
            };
            return a;
        }
        private long GetProfileType(long itemTypeId)
        {
            if (itemTypeId == (long)InvoiceItemType.RoyaltyFee)
            {
                return (long)AuthorizeNetAccountType.Royalty;
            }
            else if (itemTypeId == (long)InvoiceItemType.AdFund)
            {
                return (long)AuthorizeNetAccountType.AdFund;
            }
            else if (itemTypeId == (long)InvoiceItemType.LateFees)
            {
                return (long)AuthorizeNetAccountType.Royalty;
            }
            return 0;
        }

        public decimal GetSumInvoiceItembasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            IEnumerable<decimal> query;
            if (type == InvoiceItemType.RoyaltyFee)
            {
               query =  invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type && 
               !x.Description.Contains("Charged Minimum Royalty")).Select(x => x.Amount);
            }
            else
            {
                query = invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type).Select(x => x.Amount);
            }
            if (!query.Any())
                return 0;
            return query.Sum();
        }

        public decimal GetFixedAccountingChargebasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            var query = invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type && x.Description.Contains("for Book-keeping") && !(x.Description.Contains("for Book-keeping(var)"))).Select(x => x.Amount);
            if (!query.Any()) return 0;
            return query.Sum();
        }
        public decimal GetVariableAccountingChargebasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            var query = invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type && x.Description.Contains("for Book-keeping(var)")).Select(x => x.Amount);
            if (!query.Any()) return 0;
            return query.Sum();
        }
        public decimal GetSumISQFTChargebasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            var invoiceItemIds = invoice.InvoiceItems.Select(x => x.Id).ToList();
            var franchiseeLoanSchedule = _franchiseeLoanScheduleRepository.Table.Where(x => invoiceItemIds.Contains(x.InvoiceItemId.Value)).ToList();
            var query = default(decimal);
            var loanType = franchiseeLoanSchedule.ToList();

            foreach (var loan in loanType)
            { 
                if (loan != null && loan.FranchiseeLoan != null && loan.FranchiseeLoan.LoanTypeId != null && loan.FranchiseeLoan.LoanTypeId.Equals((long)LoanType.ISQFT))
                {
                    query += loan.PayableAmount;
                }
            }
            return query;
        }
        public decimal GetSumSEOChargebasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            var invoiceItemIds = invoice.InvoiceItems.Select(x => x.Id).ToList();
            var query = default(decimal);

            foreach (var ids in invoiceItemIds)
            {
                query += invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type && x.Description.Contains("for SEO Charges")).Sum(x => x.Amount);
                return query;
            }
            return 0;
        }
        public decimal GetSumBackUpPhoneNumberChargebasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            var invoiceItemIds = invoice.InvoiceItems.Select(x => x.Id).ToList();
            var query = default(decimal);

            foreach (var ids in invoiceItemIds)
            {
                query += invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type && x.Description.Contains("(See Phone data for call details)")).Sum(x => x.Amount);
                return query;
            }
            return 0;
        }
        public decimal GetSumLoanAndLoanInterestChargebasedonItemType(Invoice invoice)
        {
            var invoiceItemIds = invoice.InvoiceItems.Select(x => x.Id).ToList();
            var franchiseeLoanSchedule = _franchiseeLoanScheduleRepository.Table.Where(x => invoiceItemIds.Contains(x.InvoiceItemId.Value)).ToList();
            var loanType = franchiseeLoanSchedule.ToList();
            var query = default(decimal);

            foreach (var loan in loanType)
            {
                if (loan != null && loan.FranchiseeLoan != null && loan.FranchiseeLoan.LoanTypeId == null)
                {
                    query += loan.PayableAmount;
                }
                if (loan != null && loan.FranchiseeLoan != null && loan.FranchiseeLoan.LoanTypeId != null && !loan.FranchiseeLoan.LoanTypeId.Equals((long)LoanType.ISQFT))
                {
                    query += loan.PayableAmount;
                }
            }
            return query;
        }

        public decimal GetOneTimeChargesbasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            var invoiceItemIds = invoice.InvoiceItems.Select(x => x.Id).ToList();
            var query = default(decimal);

            foreach (var ids in invoiceItemIds)
            {
                query += invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type && x.Description.Contains("OneTime Project") || x.Description.Contains("for Back Up Service") || x.Description.Contains("for One time Charge")).Sum(x => x.Amount);
                return query;
            }
            return 0;
        }

        public string GetInvoiceItemType(Invoice invoice)
        {
            var isInvoiceAddFund = invoice.InvoiceItems.Any(x => x.ItemTypeId == (long)InvoiceItemType.AdFund);
            if (isInvoiceAddFund)
            {
                return "A";
            }
            else
                return "R";
        }
        public string GetOneTimeNote(Invoice invoice)
        {
            var invoiceItemIds = invoice.InvoiceItems.Select(y => y.Id).ToList();
            var servicefee = _serviceFeeInvoiceItem.Table.Where(x => x.ServiceFeeTypeId == (long)ServiceFeeType.OneTimeProject && invoiceItemIds.Contains(x.Id)).Select(z => z.Id).ToList();
            var note = _oneTimeProjectFeeRepository.Table.FirstOrDefault(x => servicefee.Contains((long)x.InvoiceItemId));
            return note != null ? note.Description : "";
            //return "A";
        }
        public decimal GetRecruitingFeeChargesbasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            var invoiceItemIds = invoice.InvoiceItems.Select(x => x.Id).ToList();
            var query = default(decimal);

            foreach (var ids in invoiceItemIds)
            {
                query += invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type && x.Description.Contains("for Recruiting")).Sum(x => x.Amount);
                return query;
            }
            return 0;
        }
        public decimal GetPayrollProcessingChargesbasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            var invoiceItemIds = invoice.InvoiceItems.Select(x => x.Id).ToList();
            var query = default(decimal);

            foreach (var ids in invoiceItemIds)
            {
                query += invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type && x.Description.Contains("for PayRoll Processing")).Sum(x => x.Amount);
                return query;
            }
            return 0;
        }
        public decimal GetNationalChargesbasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            var invoiceItemIds = invoice.InvoiceItems.Select(x => x.Id).ToList();
            var query = default(decimal);

            foreach (var ids in invoiceItemIds)
            {
                query += invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type && x.Description.Contains("for NationalCharge")).Sum(x => x.Amount);
                return query;
            }
            return 0;
        }
        public decimal GetFranchiseeEmailFeeChargesbasedonItemType(Invoice invoice, InvoiceItemType type)
        {
            var invoiceItemIds = invoice.InvoiceItems.Select(x => x.Id).ToList();
            var query = default(decimal);

            foreach (var ids in invoiceItemIds)
            {
                query += invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type && x.Description.Contains("Franchisee Tech Email Fees Charged")).Sum(x => x.Amount);
                return query;
            }
            return 0;
        }

        public decimal GetSumLateFeeBasedonItemType(Invoice invoice, LateFeeType type)
        {
            var query = invoice.InvoiceItems.Where(x => x.LateFeeInvoiceItem != null && x.LateFeeInvoiceItem.LateFeeTypeId == (long)type).Select(x => x.Amount);
            if (!query.Any()) return 0;
            return query.Sum();
        }
        public DownloadInvoiceModel CreateModel(long invoiceId, DateTime startdate, DateTime enddate, DateTime dueDate, Franchisee franchisee, InvoiceItem domain, string paymentMode)
        {
            var percentage = "0 %";
            var item = domain.Lookup.Name;
            //var date = transactionDate;

            if (domain.ItemTypeId == (long)InvoiceItemType.RoyaltyFee && domain.RoyaltyInvoiceItem != null)
            {
                if (domain.RoyaltyInvoiceItem.Percentage > 0)
                {
                    percentage = (Math.Truncate((decimal)domain.RoyaltyInvoiceItem.Percentage)).ToString();
                    item = "ML - Royalties " + percentage + " Royalty";
                }
                else
                {
                    item = "Minimums";
                }
            }
            if (domain.ItemTypeId == (long)InvoiceItemType.AdFund)
            {
                percentage = (Math.Truncate(domain.AdFundInvoiceItem.Percentage)).ToString();
                item = "ML - Royalties " + percentage + " Royalty";
            }
            if (domain.ItemTypeId == (long)InvoiceItemType.InterestRatePerAnnum)
            {
                item = domain.Lookup.Name;
            }
            if (domain.ItemTypeId == (long)InvoiceItemType.LateFees)
            {
                item = domain.Lookup.Name;
                //date = domain.LateFeeInvoiceItem.EndDate;
            }
            var formattedFranchiseeName = franchisee.Organization.Name.Replace('-', ' ');
            formattedFranchiseeName = formattedFranchiseeName.Replace("(", string.Empty).Replace(")", string.Empty);
            if (domain.ItemTypeId == (long)InvoiceItemType.ServiceFee && domain.ServiceFeeInvoiceItem != null)
            {
                item = domain.ServiceFeeInvoiceItem.ServiceFee.Alias;
                if (domain.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges)
                {
                    item = item + ": " + formattedFranchiseeName;
                }
            }
            
            if ((domain.ItemTypeId == (long)InvoiceItemType.LoanServiceFee || domain.ItemTypeId == (long)InvoiceItemType.LoanServiceFeeInterestRatePerAnnum) && domain.ServiceFeeInvoiceItem != null)
            {
                if (item.Contains("LoanServiceFee") || item.Contains("LoanInterestRatePerAnnum"))
                {
                    //item = domain.ServiceFeeInvoiceItem.ServiceFee.Alias;
                    item = item + ": " + formattedFranchiseeName;
                }
            }

            var address = franchisee.Organization.Address.FirstOrDefault();
            return new DownloadInvoiceModel
            {
                ItemId = invoiceId,
                InvoiceItem = domain.Id,
                Customer = franchisee.Organization.Name,
                //StartDate = startdate,
                EndDate = enddate,
                //DueDate = dueDate,
                //PaymentDate = date,
                PaymentMethod = paymentMode,
                Terms = "30 D",
                Item = item,
                Description = domain.Description,
                Quantity = domain.Quantity,
                TotalAmount = domain.Rate,
                Price = domain.Amount,
                BillToLine1 = franchisee.Organization.Name,
                BillToLine2 = address != null ? address.AddressLine1 : null,
                BillToLine3 = address != null ? address.AddressLine2 : null,
                BillToCity = address != null ? (address.City == null ? address.CityName : address.City.Name) : null,
                BillToState = address != null ? (address.State == null ? address.StateName : address.State.Name) : null,
                BillToPostalCode = address != null ? (address.Zip == null ? address.ZipCode : address.Zip.Code) : null,
            };
        }

        private decimal GetMinRoyalityAmount(List<InvoiceItem> invoiceItems)
        {
            var invoiceItem = invoiceItems.Where(x => x.Description.Contains("Charged Minimum Royalty")).FirstOrDefault();
            if (invoiceItem != null)
            {
                return invoiceItem.Amount;
            }
            return default(decimal);
        }

        private string GetInvoiceStatus(Invoice invoice)
        {
            var paidAmount = invoice.InvoicePayments.Count() == 0 ? 0 : invoice.InvoicePayments.Sum(x => x.Payment.Amount);
            var amountToPay = invoice.InvoiceItems.Sum(x => x.Amount);
            if ((paidAmount == amountToPay || paidAmount > amountToPay) && amountToPay > 0)
            {
                return "Paid";
            }
            else if (paidAmount < amountToPay)
            {
                return "Unpaid";
            }
            return "$0-DUE";

        }


        private string GetStatus(string status, decimal? totalSales, string paymentModel)
        {
            if (totalSales != 0 && paymentModel != "N/A")
            {
                return "Paid";
            }
            else if (totalSales != 0 && paymentModel == "N/A")
            {
                return "Unpaid";
            }
            else if (totalSales == 0 && paymentModel == "N/A")
            {
                return "$0-DUE";
            }
            else
            {
                return "Paid";
            }
        }
    }
}
