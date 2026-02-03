using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.Billing.Enum;
using Core.MarketingLead.Domain;
using Core.MarketingLead.ViewModel;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Organizations.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeServiceFeeFactory : IFranchiseeServiceFeeFactory
    {
        private readonly IRepository<Lookup> _lookupRepository;
        private readonly IRepository<OneTimeProjectFee> _oneTimeProjectFeeRepository;
        private readonly IClock _clock;
        private readonly ISettings _settings;
        private readonly IRepository<FranchiseeServiceFee> _franchiseeServiceFeeRepository;

        public FranchiseeServiceFeeFactory(IUnitOfWork unitOfWork, IClock clock, ISettings settings)
        {
            _clock = clock;
            _lookupRepository = unitOfWork.Repository<Lookup>();
            _oneTimeProjectFeeRepository = unitOfWork.Repository<OneTimeProjectFee>();
            _settings = settings;
            _franchiseeServiceFeeRepository = unitOfWork.Repository<FranchiseeServiceFee>();
        }

        public ICollection<FranchiseeServiceFeeEditModel> CreateEditModel(IEnumerable<FranchiseeServiceFee> serviceFeeList)
        {
            if (serviceFeeList == null || serviceFeeList.Count() < 1)
                return PrepareListModel();

            var serviceTypes = GetServiceTypeIds();
            var list = new List<FranchiseeServiceFeeEditModel>();

            foreach (var type in serviceTypes)
            {
                var model = new FranchiseeServiceFeeEditModel { TypeId = type.Id, ServiceName = type.Name };
                foreach (var fee in serviceFeeList)
                {
                    if (type.Id == fee.ServiceFeeTypeId)
                    {
                        CreateModel(fee, model);
                    }
                }
                list.Add(model);
            }
            return list;
        }

        public ICollection<FranchiseeServiceFeeEditModel> CreateViewModel(IEnumerable<OneTimeProjectFee> projectFeeList)
        {
            var list = new List<FranchiseeServiceFeeEditModel>();
            if (!projectFeeList.Any())
                return list;

            foreach (var otp in projectFeeList)
            {
                var model = new FranchiseeServiceFeeEditModel
                {
                    Id = otp.Id,
                    Amount = otp.Amount,
                    Description = otp.Description,
                    FranchiseeId = otp.FranchiseeId,
                    InvoiceItemId = otp.InvoiceItemId,
                    CreatedOn = otp.DateCreated,
                    CurrencyExchangeRateId = otp.CurrencyExchangeRateId,
                    CurrencyRate = otp.CurrencyExchangeRate.Rate,
                    CurrencyCode = otp.Franchisee.Currency
                };
                list.Add(model);
            }
            return list;
        }

        public ICollection<FranchiseeServiceFeeEditModel> CreateViewModel(IEnumerable<FranchiseeLoan> loanList)
        {
            var list = new List<FranchiseeServiceFeeEditModel>();
            if (!loanList.Any())
                return list;

            foreach (var loan in loanList)
            {
                var loanPayment = loan.FranchiseeLoanSchedule.Any(x => x.PayableAmount > 0 && x.InvoiceItemId == null
                                                                && !x.IsPrePaid);

                var model = new FranchiseeServiceFeeEditModel
                {
                    Id = loan.Id,
                    Amount = loan.Amount,
                    Description = loan.Description,
                    FranchiseeId = loan.FranchiseeId,
                    Duration = loan.Duration,
                    Percentage = loan.InterestratePerAnum,
                    CreatedOn = loan.DateCreated,
                    CurrencyCode = loan.Franchisee.Currency,
                    CurrencyRate = loan.CurrencyExchangeRate.Rate,
                    LoanSchedule = loan.FranchiseeLoanSchedule.Where(x => !x.IsPrePaid).Select(x => CreateLoanSchedule(x)).ToList(),
                    IsRoyality = loan.IsRoyality.Value ? true : false,
                    IsAdfund = loan.IsRoyality.Value ? false : true,
                    LoanAdjustment = loan.IsRoyality.Value ? "Royalty" : "Adfund",
                    IsEditable = false,
                    LoanAdjustmentId = loan.IsRoyality.Value ? "1" : "0",
                    IsEditing = false,
                    IsLoanCompletetd = loanPayment,
                    IsLoanPayed = loan.IsCompleted,
                    StartDate = loan.StartDate,
                    IsCompleted = loan.IsCompleted,
                    LoanScheduleList = loan.FranchiseeLoanSchedule.Where(x => !x.IsPrePaid).Select(x => CreateLoanSchedule(x)).ToList(),
                    LoanType = loan.LoanType != null ? loan.LoanType.Name : "",
                    LoanTypeId = loan.LoanTypeId
                };
                list.Add(model);
            }
            return list;
        }

        private LoanScheduleViewModel CreateLoanSchedule(FranchiseeLoanSchedule schedule)
        {
            var model = new LoanScheduleViewModel
            {
                LoanTerm = schedule.LoanTerm,
                LoanId = schedule.LoanId,
                DueDate = schedule.DueDate,
                Balance = schedule.Balance,
                Interest = schedule.Interest,
                Principal = schedule.Principal,
                PayableAmount = schedule.PayableAmount,
                OverPayment = schedule.OverPaidAmount,
                InvoiceItemId = schedule.InvoiceItemId + (schedule.InvoiceItemId > 0 ? ", " : " ") + schedule.InterestAmountInvoiceItemId,
                TotalPrincipal = schedule.TotalPrincipal,
                CurrencyExchangeRateId = schedule.FranchiseeLoan != null ? schedule.FranchiseeLoan.CurrencyExchangeRateId : 1,
                CurrencyRate = schedule.FranchiseeLoan != null ? schedule.FranchiseeLoan.CurrencyExchangeRate.Rate : 1,
                LoanAdjustment = schedule.IsRoyality ? "Royalty" : "Adfund",
                IsCompleted = schedule.FranchiseeLoan != null ? schedule.FranchiseeLoan.IsCompleted : false,
                Id = schedule.Id
            };
            return model;
        }

        private ICollection<FranchiseeServiceFeeEditModel> PrepareListModel()
        {
            var serviceTypes = GetServiceTypeIds();
            IList<FranchiseeServiceFeeEditModel> list = new List<FranchiseeServiceFeeEditModel>();
            foreach (var item in serviceTypes)
            {
                var model = new FranchiseeServiceFeeEditModel { TypeId = item.Id, ServiceName = item.Name };
                list.Add(model);
            }
            return list;
        }

        private FranchiseeServiceFeeEditModel CreateModel(FranchiseeServiceFee domain, FranchiseeServiceFeeEditModel model)
        {
            model.Id = domain.Id;
            model.Amount = domain.Amount;
            model.FrequencyId = domain.FrequencyId;
            model.FranchiseeId = domain.FranchiseeId;
            model.IsApplicable = domain.IsActive;
            model.Percentage = domain.Percentage;
            return model;
        }

        private ICollection<Lookup> GetServiceTypeIds()
        {
            return _lookupRepository.Table.Where(x => x.LookupTypeId == (long)LookupTypes.ServiceFeeType
                                                    && x.Id != (long)ServiceFeeType.Loan
                                                    && x.Id != (long)ServiceFeeType.OneTimeProject
                                                    && x.Id != (long)ServiceFeeType.InterestAmount
                                                    && x.Id != (long)ServiceFeeType.FRANCHISEETECHMAIL
                                                    && x.Id != (long)ServiceFeeType.VarBookkeeping
                                                    && x.Name != "SEO Charges").OrderBy(x => x.Id).ToList();
        }

        public ICollection<FranchiseeServiceFee> CreateDomain(ICollection<FranchiseeServiceFeeEditModel> serviceFeeList, Franchisee franchisee)
        {
            var feeInDb = new List<FranchiseeServiceFee>();
            foreach (var serviceFee in serviceFeeList.Where(x => x.Amount > 0 || x.Percentage > 0))
            {
                serviceFee.FranchiseeId = franchisee.Id;
                var domain = CreateDomainForServiceFee(serviceFee);
                feeInDb.Add(domain);

                if (domain.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping)
                {
                    var VarBookkeeepingService = new FranchiseeServiceFeeEditModel
                    {
                        Amount = serviceFee.Amount,
                        Percentage = serviceFee.Percentage,
                        FrequencyId = serviceFee.FrequencyId,
                        FranchiseeId = serviceFee.FranchiseeId,
                        IsApplicable = true,
                        TypeId = (long)ServiceFeeType.VarBookkeeping
                    };
                    var varBookkeepingDomain = CreateDomainForServiceFee(VarBookkeeepingService);

                    var indbVarBookkeeping = _franchiseeServiceFeeRepository.Table.Where(x => x.FranchiseeId == serviceFee.FranchiseeId
                                                && x.ServiceFeeTypeId == (long)ServiceFeeType.VarBookkeeping).FirstOrDefault();
                    if (indbVarBookkeeping != null)
                    {
                        varBookkeepingDomain.Id = indbVarBookkeeping.Id;
                        varBookkeepingDomain.IsNew = false;
                    }
                    feeInDb.Add(varBookkeepingDomain);
                }
            }
            return feeInDb;
        }

        public OneTimeProjectFee CreateOneTimeProject(FranchiseeServiceFeeEditModel serviceFee)
        {
            var domain = new OneTimeProjectFee
            {
                Amount = serviceFee.Amount,
                Description = serviceFee.Description,
                FranchiseeId = serviceFee.FranchiseeId,
                InvoiceItemId = null,
                DateCreated = _clock.UtcNow,
                CurrencyExchangeRateId = serviceFee.CurrencyExchangeRateId,
                IsNew = true
            };
            return domain;
        }

        public FranchiseeLoan CreateFranchiseeLoan(FranchiseeServiceFeeEditModel serviceFee)
        {
            var domain = new FranchiseeLoan
            {
                Amount = serviceFee.Amount,
                Description = serviceFee.Description,
                FranchiseeId = serviceFee.FranchiseeId,
                DateCreated = _clock.UtcNow,
                Duration = serviceFee.Duration.Value,
                InterestratePerAnum = serviceFee.Percentage.Value,
                CurrencyExchangeRateId = serviceFee.CurrencyExchangeRateId,
                IsNew = true,
                IsRoyality = serviceFee.IsRoyality,
                IsCompleted = false,
                StartDate = serviceFee.StartDate,
                LoanTypeId = serviceFee.LoanTypeId
            };
            return domain;
        }

        public FranchiseeServiceFee CreateDomainForServiceFee(FranchiseeServiceFeeEditModel model)
        {
            var domain = new FranchiseeServiceFee
            {
                Id = model.Id,
                Amount = model.Amount,
                FranchiseeId = model.FranchiseeId,
                FrequencyId = model.FrequencyId,
                IsActive = model.IsApplicable,
                Percentage = model.Percentage ?? 0,
                ServiceFeeTypeId = model.TypeId,
                IsNew = model.Id <= 0,

            };
            if (domain.ServiceFeeTypeId == (long)ServiceFeeType.OneTimeProject)
                domain.Amount = _settings.DefaultOneTimeProjectAmount;
            return domain;
        }

        private FranchiseeServiceFee CreateServiceFeeDoamin(long franchiseeId, decimal amount, long serviceTypeId, decimal percentage, bool isActive, int? duration, FranchiseeServiceFee inDBServiceFee)
        {
            return new FranchiseeServiceFee
            {
                Id = inDBServiceFee != null ? inDBServiceFee.Id : 0,
                Amount = amount,
                FranchiseeId = franchiseeId,
                Percentage = percentage,
                ServiceFeeTypeId = serviceTypeId,
                IsNew = inDBServiceFee == null ? true : false,
                IsActive = isActive
            };
        }

        public FranchiseeLoanSchedule CreateDomain(AmortPaymentSchedule schedule)
        {
            var Domain = new FranchiseeLoanSchedule
            {
                Id = schedule.Id,
                LoanId = schedule.LoanId,
                LoanTerm = schedule.TermNumber,
                Balance = Convert.ToDecimal(schedule.Balance),
                DueDate = schedule.Date,
                Interest = Convert.ToDecimal(schedule.Interest),
                IsPrePaid = schedule.ScheduledPayment > 0 ? false : true,
                InvoiceItemId = null,
                PayableAmount = Convert.ToDecimal(schedule.ScheduledPayment),
                Principal = Convert.ToDecimal(schedule.Principal),
                TotalPrincipal = Convert.ToDecimal(schedule.Totalprincipal),
                OverPaidAmount = 0,
                IsNew = schedule.Id <= 0,

            };
            return Domain;
        }

        public ICollection<FranchiseeDocumentEditModel> CreateEditModelForDocument(IEnumerable<FranchiseeServiceFee> serviceFeeList)
        {
            return null;
        }

        public LoanAdjustmentAudit CreateDomain(FranchiseeChangeServiceFee franchiseeChange, FranchiseeLoan franchiseeLoan)
        {
            var Domain = new LoanAdjustmentAudit
            {
                AfterLoanAdjustment = franchiseeChange.IsRoyality.GetValueOrDefault(),
                BeforeLoanAdjustment = franchiseeLoan.IsRoyality.GetValueOrDefault(),
                CreatedOn = _clock.ToUtc(DateTime.Now),
                LoanId = franchiseeChange.LoanId,
                UserId = franchiseeChange.UserId,
                IsNew = true,

            };
            return Domain;
        }

        public FranchiseeLoanViewModel CreateViewModel(FranchiseeLoanSchedule loanSchedule)
        {

            var model = new FranchiseeLoanViewModel
            {
                Balance = loanSchedule.Balance.ToString(),
                DueDate = loanSchedule.DueDate.ToShortDateString(),
                Interest = loanSchedule.Interest.ToString(),
                InterestAmountInvoiceItem = loanSchedule.InterestAmountInvoiceItemId.ToString(),
                OverPaidAmount = loanSchedule.OverPaidAmount.ToString(),
                InvoiceItemId = loanSchedule.InvoiceItemId.ToString(),
                LoanId = loanSchedule.LoanId,
                LoanTerm = loanSchedule.LoanTerm.ToString(),
                Principal = loanSchedule.Principal.ToString(),
                RoyalityAdfund = loanSchedule.IsRoyality ? "Royalty" : "Adfund",
                TotalPrincipal = loanSchedule.TotalPrincipal.ToString(),
                PayableAmount = loanSchedule.PayableAmount.ToString(),
                LoanType = loanSchedule.FranchiseeLoan.LoanType != null ? loanSchedule.FranchiseeLoan.LoanType.Name : ""
            };
            return model;
        }

        public FranchiseeServiceFee CreateDomainForServiceFeeForCalls(Phonechargesfee model)
        {
            var domain = new FranchiseeServiceFee
            {
                Id = model.Id,
                Amount = model.Amount,
                FranchiseeId = model.FranchiseeId,
                FrequencyId = null,
                IsActive = true,
                Percentage = 0,
                ServiceFeeTypeId = (long)ServiceFeeType.PHONECALLCHARGES,
                IsNew = model.Id <= 0,

            };
            return domain;
        }

        public AutomationClassMarketingLead CreateViewModel(MarketingLeadCallDetailV3 domain, MarketingLeadCallDetail domain2)
        {
            return new AutomationClassMarketingLead()
            {
                Id = domain != null && domain.MarketingLeadCallDetail != null ? domain.MarketingLeadCallDetail.Id : 0,
                Ani = domain2 != null ? domain2.SessionId : "",
                DateOfCall = domain2 != null ? domain2.DateAdded.Value : default(DateTime),
                InvoiceId = domain2 != null ? domain2.InvoiceId : default(long),
                DataFromNewAPI = domain2 != null ? domain2.IsFromNewAPI : default(bool),
                DataFromInvoca = domain2 != null ? domain2.IsFromInvoca : default(bool),
                PhoneLabel = domain2 != null ? domain2.PhoneLabel : "",
                CallTypeId = domain2 != null ? domain2.CallTypeId : default(long),
                TransferToNumber = domain2 != null ? domain2.TransferToNumber : "",
                CallType = domain2 != null ? domain2.CallType.Name : "",
                TransferType = domain2 != null ? domain2.CallTransferType : "",
                Route = domain != null ? domain.CallflowSourceRoute : "",
                Destination = (domain != null && domain.CallflowDestination != null) ? domain.CallflowDestination : "No Destination",
                ZipCode = domain != null ? domain.CallflowEnteredZip : "",
                CallDuration = domain2 != null ? domain2.CallDuration : 0,
                CallerId = domain2 != null ? domain2.CallerId : "",
                CallFlowState = domain != null ? domain.CallflowState : ""
            };
        }
    }
}
