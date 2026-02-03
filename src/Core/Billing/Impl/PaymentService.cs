using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Notification;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Sales;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class PaymentService : IPaymentService
    {
        private readonly IChargeCardPaymentService _chargeCardPaymentService;
        private readonly IChargeCardProfileService _chargeCardProfileService;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IPaymentFactory _paymentFactory;
        private readonly IECheckPaymentService _eCheckPaymentService;
        private readonly IInvoicePaymentService _invoicePaymentService;
        private readonly IChargeCardService _chargeCardService;
        private readonly IPaymentReminderPollingAgent _paymentReminderPollingAgent;
        private readonly IRepository<FranchiseeAccountCredit> _franchiseeAccountCreditRepository;
        private readonly IAccountCreditFactory _accountCreditFactory;
        private readonly IRepository<AccountCreditPayment> _accountCreditPaymentRepository;
        private readonly ILogService _logService;
        private readonly IFranchiseeServiceFeeService _franchiseeServicefeeService;
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private readonly IRepository<FranchiseeLoanSchedule> _franchiseeLoanScheduleRepository;
        private readonly IRepository<FranchiseeLoan> _franchiseeLoanRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly ISettings _settings;

        public PaymentService(IUnitOfWork unitOfWork, IChargeCardPaymentService chargeCardPaymentService, IChargeCardProfileService chargeCardProfileService,
             IPaymentFactory paymentFactory, IECheckPaymentService eCheckPaymentService, IInvoicePaymentService invoicePaymentService,
             IChargeCardService chargeCardService, IPaymentReminderPollingAgent paymentReminderPollingAgent, IAccountCreditFactory accountCreditfactory,
             ILogService logService, IFranchiseeServiceFeeService franchiseeServiceFeeService, IClock clock, ISettings settings)
        {
            _unitOfWork = unitOfWork;
            _chargeCardPaymentService = chargeCardPaymentService;
            _chargeCardProfileService = chargeCardProfileService;
            _invoiceRepository = unitOfWork.Repository<Invoice>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _paymentRepository = unitOfWork.Repository<Payment>();
            _paymentFactory = paymentFactory;
            _eCheckPaymentService = eCheckPaymentService;
            _invoicePaymentService = invoicePaymentService;
            _chargeCardService = chargeCardService;
            _paymentReminderPollingAgent = paymentReminderPollingAgent;
            _franchiseeAccountCreditRepository = unitOfWork.Repository<FranchiseeAccountCredit>();
            _accountCreditFactory = accountCreditfactory;
            _accountCreditPaymentRepository = unitOfWork.Repository<AccountCreditPayment>();
            _logService = logService;
            _franchiseeServicefeeService = franchiseeServiceFeeService;
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _franchiseeLoanScheduleRepository = unitOfWork.Repository<FranchiseeLoanSchedule>();
            _clock = clock;
            _settings = settings;
            _franchiseeLoanRepository = unitOfWork.Repository<FranchiseeLoan>();
            _invoiceItemRepository= unitOfWork.Repository<InvoiceItem>();
        }

        public ProcessorResponse MakePaymentByNewChargeCard(ChargeCardPaymentEditModel model, long franchiseeId, long invoiceId)
        {
            ProcessorResponse response = null;
            _logService.Info(string.Format("Start payment of Invoice# {0} of Franchisee# {1}", invoiceId, franchiseeId));
            response = GetFranchiseeAndInvoiceDetail(franchiseeId, invoiceId);

            if (response.ProcessorResult == ProcessorResponseResult.Fail)
            {
                return response;
            }
            try
            {
                if (model.IsLoanOverPayment && model.OverPaymentAmount > 0)
                {
                    CreateOverPaymentInvoiceItem(model.OverPaymentAmount, franchiseeId, invoiceId);
                }
                else
                {

                    CheckingAndMailSendingForLoan(invoiceId);
                }
                var invoice = _invoiceRepository.Get(invoiceId);
                model.InvoiceId = invoiceId;
                model.Amount = invoice.InvoiceItems.Sum(x => x.Amount);

                var amount = AccountCreditPayment(model.Amount, invoice, franchiseeId);
                if (amount <= 0)
                {
                    _logService.Info(string.Format("Payment of Invoice# {0} for Franchisee# {1} is done Via Account Credit", invoiceId, franchiseeId));
                    response = new ProcessorResponse
                    {
                        Message = "The amount is Paid Via Account Credit.",
                        ProcessorResult = ProcessorResponseResult.Accepted,
                    };
                    return response;
                }

                decimal creditCardCharge = 0, chargedAmount = model.Amount;
                var instrumentTypeId = (long)InstrumentType.ChargeCard;
                model.InstrumentTypeId = instrumentTypeId;

                if (amount > 0)
                {
                    model.Amount = amount;
                    if (model.ProfileTypeId <= 0)
                        model.ProfileTypeId = (long)AuthorizeNetAccountType.Royalty;

                    response = _chargeCardPaymentService.ChargeCardPayment(model, franchiseeId, out creditCardCharge, out chargedAmount);
                    model.ProcessorResponse = response;

                    if (response == null || response.ProcessorResult != ProcessorResponseResult.Accepted)
                    {
                        throw new Exception(response.Message);
                    }
                    var paymentsResponse = response.ProcessorResult;
                    var paymentTransId = response.RawResponse;

                    if (response.ProcessorResult == ProcessorResponseResult.Accepted && model.SaveOnFile == false)
                    {
                        var chargeCard = _chargeCardService.Save(model.ChargeCardEditModel);
                        response.InstrumentId = chargeCard.Id;
                    }
                    else if (response.ProcessorResult == ProcessorResponseResult.Accepted && model.SaveOnFile == true)
                    {
                        response = _chargeCardProfileService.CreateProfile(model.ChargeCardEditModel, franchiseeId);
                    }
                    if (response == null || response.ProcessorResult == ProcessorResponseResult.Fail)
                    {
                        _logService.Error("Some error accured, start rollback payment.");
                        if (paymentsResponse == ProcessorResponseResult.Accepted)
                            _chargeCardPaymentService.RollbackPayment(model.ProfileTypeId, paymentTransId);
                        return response;
                    }
                    try
                    {
                        var currencyexchangerate = _invoiceRepository.Get(invoiceId).InvoiceItems.Select(x => x.CurrencyExchangeRate).First();
                        var payment = _paymentFactory.CreatePaymentDomain(model.Amount, instrumentTypeId, currencyexchangerate.Id);
                        _paymentRepository.Save(payment);

                        invoice.StatusId = (long)InvoiceStatus.Paid;
                        _invoiceRepository.Save(invoice);

                        _invoicePaymentService.Save(invoice.Id, payment.Id);

                        _chargeCardPaymentService.Save(response, payment);

                        //   Payment confirmation Notification
                        _paymentReminderPollingAgent.CreatePaymentConfirmationNotification(invoice, payment, franchiseeId);
                    }

                    catch (Exception ex)
                    {
                        //Mark Credit card payment void if some exception occurs
                        _logService.Error(ex);
                        _unitOfWork.Rollback();
                        _logService.Error(string.Format("Some error accured, start rollback payment."));
                        if (response.ProcessorResult == ProcessorResponseResult.Accepted)
                            _chargeCardPaymentService.RollbackPayment(model.ProfileTypeId, paymentTransId);

                        throw ex;
                    }
                    if (response.ProcessorResult == ProcessorResponseResult.Accepted)
                    {
                        CreditFranchiseeAccount(model.OverPaymentAmount, franchiseeId, invoiceId);
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw ex;
            }
            return response;
        }


        private void CheckingAndMailSendingForLoan(long? invoiceId)
        {
            // Checking for Loan Completion If Loan is Completed then we are sending Mails to Franchisee
            var franchiseeInvoice = _franchiseeInvoiceRepository.Get(x => x.InvoiceId == invoiceId);
            if (franchiseeInvoice.Invoice.InvoiceItems.Any(x => x.ServiceFeeInvoiceItem != null
                                        && x.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.Loan))
            {
                var termId = default(long);
                var loanInvoiceItem = franchiseeInvoice.Invoice.InvoiceItems.Where(x => x.ServiceFeeInvoiceItem != null
                                        && x.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.Loan).FirstOrDefault();
                var currentLoanSchedule = loanInvoiceItem != null ? _franchiseeLoanScheduleRepository.Table.Where(x => x.InvoiceItemId == loanInvoiceItem.Id).FirstOrDefault() : null;
                if (currentLoanSchedule != null)
                {
                    if (currentLoanSchedule.Balance == 0 && currentLoanSchedule.FranchiseeLoan != null)
                    {
                        SavingFranchiseeLoan(currentLoanSchedule.FranchiseeLoan);
                        SendMailForLoanCompletion(currentLoanSchedule.FranchiseeLoan);
                    }


                }

            }
        }
        private void SavingFranchiseeLoan(FranchiseeLoan franchiseeLoan)
        {
            franchiseeLoan.IsCompleted = true;
            franchiseeLoan.IsNew = false;
            _franchiseeLoanRepository.Save(franchiseeLoan);
        }
        public ProcessorResponse MakePaymentOnFileChargeCard(InstrumentOnFileEditModel model, long franchiseeId, long invoiceId)
        {
            ProcessorResponse paymentResponse = null;

            paymentResponse = GetFranchiseeAndInvoiceDetail(franchiseeId, invoiceId);

            if (paymentResponse.ProcessorResult == ProcessorResponseResult.Fail)
            {
                return paymentResponse;
            }
            try
            {
                if (model.IsLoanOverPayment && model.OverPaymentAmount > 0)
                {
                    CreateOverPaymentInvoiceItem(model.OverPaymentAmount, franchiseeId, invoiceId);
                }
                else
                {

                    CheckingAndMailSendingForLoan(invoiceId);
                }

                var invoice = _invoiceRepository.Get(invoiceId);
                model.Amount = invoice.InvoiceItems.Sum(x => x.Amount);
                model.InvoiceId = invoiceId;

                var amount = AccountCreditPayment(model.Amount, invoice, franchiseeId);
                if (amount <= 0)
                {
                    _logService.Info(string.Format("Payment of Invoice# {0} for Franchisee# {1} is done Via Account Credit", invoiceId, franchiseeId));

                    paymentResponse = new ProcessorResponse
                    {
                        Message = "The amount is Paid Via Account Credit.",
                        ProcessorResult = ProcessorResponseResult.Accepted,
                    };
                    return paymentResponse;
                }

                decimal chargedAmount = model.Amount;
                var instrumentTypeId = (long)InstrumentType.ChargeCard;

                if (amount > 0)
                {
                    model.Amount = amount;
                    if (model.ProfileTypeId <= 0)
                        model.ProfileTypeId = (long)AuthorizeNetAccountType.Royalty;

                    paymentResponse = _chargeCardPaymentService.ChargeCardOnFile(model as InstrumentOnFileEditModel, paymentResponse, franchiseeId);
                    model.ProcessorResponse = paymentResponse;
                    paymentResponse.InstrumentId = model.InstrumentId;

                    if (paymentResponse == null || paymentResponse.ProcessorResult != ProcessorResponseResult.Accepted)
                    {
                        throw new Exception(paymentResponse.Message);
                    }
                    try
                    {
                        var currencyexchangerate = _invoiceRepository.Get(invoiceId).InvoiceItems.Select(x => x.CurrencyExchangeRate).First();
                        var payment = _paymentFactory.CreatePaymentDomain(model.Amount, instrumentTypeId, currencyexchangerate.Id);
                        _paymentRepository.Save(payment);

                        invoice.StatusId = (long)InvoiceStatus.Paid;
                        _invoiceRepository.Save(invoice);

                        _invoicePaymentService.Save(invoice.Id, payment.Id);

                        _chargeCardPaymentService.Save(paymentResponse, payment);

                        // Payment confirmation Notification
                        _paymentReminderPollingAgent.CreatePaymentConfirmationNotification(invoice, payment, franchiseeId);

                    }
                    catch (Exception ex)
                    {
                        _logService.Error("Some error accured, start rollback payment.");
                        //Mark Credit card payment void if some exception occurs
                        if (paymentResponse.ProcessorResult == ProcessorResponseResult.Accepted)
                            _chargeCardPaymentService.RollbackPayment(model.ProfileTypeId, paymentResponse.RawResponse);

                        throw ex;
                    }
                    if (paymentResponse.ProcessorResult == ProcessorResponseResult.Accepted)
                    {
                        CreditFranchiseeAccount(model.OverPaymentAmount, franchiseeId, invoiceId);
                    }
                    return paymentResponse;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return paymentResponse;
        }

        public ProcessorResponse MakePaymentByECheck(ECheckEditModel model, long franchiseeId, long invoiceId)
        {
            ProcessorResponse paymentResponse = null;

            paymentResponse = GetFranchiseeAndInvoiceDetail(franchiseeId, invoiceId);

            if (paymentResponse.ProcessorResult == ProcessorResponseResult.Fail)
            {
                return paymentResponse;
            }

            try
            {
                if (model.IsLoanOverPayment && model.OverPaymentAmount > 0)
                {
                    CreateOverPaymentInvoiceItem(model.OverPaymentAmount, franchiseeId, invoiceId);
                }
                else
                {

                    CheckingAndMailSendingForLoan(invoiceId);
                }

                var invoice = _invoiceRepository.Get(invoiceId);
                model.Amount = invoice.InvoiceItems.Sum(x => x.Amount);
                model.InvoiceId = invoiceId;

                var amount = AccountCreditPayment(model.Amount, invoice, franchiseeId);
                if (amount <= 0)
                {
                    _logService.Info(string.Format("Payment of Invoice# {0} for Franchisee# {1} is done Via Account Credit", invoiceId, franchiseeId));
                    paymentResponse = new ProcessorResponse
                    {
                        Message = "The amount is Paid Via Account Credit.",
                        ProcessorResult = ProcessorResponseResult.Accepted,
                    };
                    return paymentResponse;
                }

                if (amount > 0)
                {
                    model.Amount = amount;
                    if (model.ProfileTypeId <= 0)
                        model.ProfileTypeId = (long)AuthorizeNetAccountType.Royalty;

                    var instrumentTypeId = model.InstrumentTypeId;
                    paymentResponse = _eCheckPaymentService.MakeECheckPayment(model, franchiseeId);
                    model.ProcessorResponse = paymentResponse;
                    model.InstrumentId = paymentResponse.InstrumentId;

                    if (paymentResponse == null || paymentResponse.ProcessorResult != ProcessorResponseResult.Accepted)
                    {
                        throw new Exception(paymentResponse.Message);
                    }
                    try
                    {
                        var currencyexchangerate = _invoiceRepository.Get(invoiceId).InvoiceItems.Select(x => x.CurrencyExchangeRate).First();
                        var payment = _paymentFactory.CreatePaymentDomain(model.Amount, instrumentTypeId, currencyexchangerate.Id);
                        _paymentRepository.Save(payment);

                        invoice.StatusId = (long)InvoiceStatus.Paid;
                        _invoiceRepository.Save(invoice);

                        _invoicePaymentService.Save(invoice.Id, payment.Id);

                        _eCheckPaymentService.Save(model, payment);

                        // Payment confirmation Notification
                        _paymentReminderPollingAgent.CreatePaymentConfirmationNotification(invoice, payment, franchiseeId);
                    }
                    catch (Exception ex)
                    {
                        _logService.Error("Some error accured, start rollback payment.");
                        if (paymentResponse.ProcessorResult == ProcessorResponseResult.Accepted)
                            _chargeCardPaymentService.RollbackPayment(model.ProfileTypeId, paymentResponse.RawResponse);

                        throw ex;
                    }
                    if (paymentResponse.ProcessorResult == ProcessorResponseResult.Accepted)
                    {
                        CreditFranchiseeAccount(model.OverPaymentAmount, franchiseeId, invoiceId);
                    }
                    return paymentResponse;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return paymentResponse;
        }

        private ProcessorResponse GetFranchiseeAndInvoiceDetail(long franchiseeId, long invoiceId)
        {
            var franchisee = _franchiseeRepository.Fetch(x => x.Id == franchiseeId).FirstOrDefault();

            if (franchisee == null)
            {
                return new ProcessorResponse
                {
                    Message = "Franchisee does not exist.",
                    ProcessorResult = ProcessorResponseResult.Fail,
                    RawResponse = "Franchisee does not exist."
                };
            }

            if (IsInvoiceAlreadyPaid(invoiceId))
            {
                return new ProcessorResponse
                {
                    Message = "Payment for this invoice has already been received or is in progress.",
                    ProcessorResult = ProcessorResponseResult.Fail,
                    RawResponse = "Payment for this invoice has already been received or is in progress."
                };
            }
            else return new ProcessorResponse
            {
                ProcessorResult = ProcessorResponseResult.Accepted,
            };
        }

        public bool IsInvoiceAlreadyPaid(long invoiceId)
        {
            var invoice = _invoiceRepository.Get(invoiceId);
            var invoiceIntems = _invoiceItemRepository.Table.Where(x => x.InvoiceId == invoiceId).ToList();
            decimal totalAmount = 0;
            if (invoiceIntems.Count() > 0)
            {
                totalAmount = invoiceIntems.Sum(x => x.Amount);
            }

            if (invoice.StatusId == (long)InvoiceStatus.Paid && totalAmount>0 && invoice.InvoicePayments.Count() >0)
                return true;

            var invoiceAmount = invoice.InvoiceItems.Sum(x => x.Amount);
            decimal paidAmount = 0;

            return paidAmount >= invoiceAmount;
        }

        public decimal AccountCreditPayment(decimal amount, Invoice invoice, long franchiseeId)
        {
            var invoiceTypeId = invoice.InvoiceItems.Any(i => i.AdFundInvoiceItem != null) ? (long)AccountCreditType.AdFund : (long)AccountCreditType.Royalty;
            var accountCreditList = _franchiseeAccountCreditRepository.Table.Where(x => x.FranchiseeId == franchiseeId
                                     && (x.CreditTypeId == invoiceTypeId || x.CreditTypeId == (long)AccountCreditType.AllSalesCredit)
                                     && x.RemainingAmount > 0).ToList();

            if (!accountCreditList.Any())
                return amount;
            var creditAmount = accountCreditList.Sum(x => x.RemainingAmount);

            if (creditAmount > 0)
            {
                var paymentAmount = SaveAccountCredit(invoice, accountCreditList);
                return paymentAmount;
            }
            else return amount;
        }

        private decimal SaveAccountCredit(Invoice invoice, IList<FranchiseeAccountCredit> accountCreditList)
        {
            var invoiceAmount = invoice.InvoiceItems.Sum(x => x.Amount);
            var instrumentTypeId = (long)InstrumentType.AccountCredit;
            decimal amountToPayViaCredit = 0;

            var totalCreditAmount = accountCreditList.Sum(x => x.RemainingAmount);

            if (totalCreditAmount == invoiceAmount || totalCreditAmount > invoiceAmount)
            {
                amountToPayViaCredit = invoiceAmount;
            }
            else if (totalCreditAmount < invoiceAmount)
            {
                amountToPayViaCredit = totalCreditAmount;
            }

            var currencyexchangerate = invoice.InvoiceItems.Select(x => x.CurrencyExchangeRate).First();
            var payment = _paymentFactory.CreatePaymentDomain(amountToPayViaCredit, instrumentTypeId, currencyexchangerate.Id);
            _paymentRepository.Save(payment);

            _invoicePaymentService.Save(invoice.Id, payment.Id);

            foreach (var item in accountCreditList)
            {
                decimal amountToPay = 0;
                if (item.RemainingAmount > 0 && amountToPayViaCredit > 0)
                {
                    if ((item.RemainingAmount == amountToPayViaCredit || item.RemainingAmount > amountToPayViaCredit))
                    {
                        amountToPay = amountToPayViaCredit;
                    }
                    else if (item.RemainingAmount < amountToPayViaCredit)
                    {
                        amountToPay = item.RemainingAmount;
                    }
                    var accountCreditPayment = _accountCreditFactory.CreateDomain(payment, item, amountToPay);
                    _accountCreditPaymentRepository.Save(accountCreditPayment);

                    item.RemainingAmount = item.RemainingAmount - accountCreditPayment.Amount;
                    invoiceAmount = invoiceAmount - accountCreditPayment.Amount;
                    amountToPayViaCredit = amountToPayViaCredit - accountCreditPayment.Amount;
                }

                invoice.StatusId = invoiceAmount > 0 ? (long)InvoiceStatus.PartialPaid : (long)InvoiceStatus.Paid;
                _invoiceRepository.Save(invoice);
                item.IsNew = false;
                _franchiseeAccountCreditRepository.Save(item);

                //payment confirmation notification ----- to do
            }
            return invoiceAmount;
        }

        public ProcessorResponse AdjustAccountCredit(long franchiseeId, long invoiceId)
        {
            ProcessorResponse response = new ProcessorResponse { };
            if (invoiceId > 0)
            {
                var invoice = _invoiceRepository.Get(invoiceId);
                var amountToPay = invoice.InvoiceItems.Sum(x => x.Amount);
                var amount = AccountCreditPayment(amountToPay, invoice, franchiseeId);
                if (amount <= 0)
                {
                    _logService.Error(string.Format("The amount is Paid Via Account Credit for Invoice# {0}", invoiceId));
                    response.Message = "The amount is Paid Via Account Credit.";
                    response.ProcessorResult = ProcessorResponseResult.Accepted;
                }
            }
            else
            {
                _logService.Error(string.Format("some error accured while Account Credit adjustment for Invoice# {0}", invoiceId));
                response.Message = "Some error accured.";
                response.ProcessorResult = ProcessorResponseResult.Fail;
            }
            return response;
        }

        private static string Log(string message)
        {
            return DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss:fff tt") + " \t\t " + message + "\r\n\r\n";
        }

        public void CreateOverPaymentInvoiceItem(decimal amount, long franchiseeId, long invoiceId)
        {
            try
            {
                var franchiseeInvoice = _franchiseeInvoiceRepository.Get(x => x.InvoiceId == invoiceId);
                var loanInvoiceItem = franchiseeInvoice.Invoice.InvoiceItems.Where(x => x.ServiceFeeInvoiceItem != null
                                            && x.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.Loan).FirstOrDefault();

                var currencyRate = franchiseeInvoice.Invoice.InvoiceItems.Where(x => x.CurrencyExchangeRate != null).FirstOrDefault().CurrencyExchangeRate.Rate;
                var amountInDefaultCurrency = amount.ToDefaultCurrency(currencyRate);

                var loanSchedule = loanInvoiceItem != null ? _franchiseeLoanScheduleRepository.Get(x => x.InvoiceItemId == loanInvoiceItem.Id) : null;
                if (franchiseeInvoice != null && loanSchedule != null)
                {
                    var serviceFee = franchiseeInvoice.Franchisee.FranchiseeServiceFee.Where(x => x.ServiceFeeTypeId == (long)ServiceFeeType.Loan).FirstOrDefault();
                    serviceFee.Percentage = 0;
                    var qty = 1;
                    var rate = amountInDefaultCurrency;
                    var model = _franchiseeServicefeeService.CreateModel(serviceFee, franchiseeInvoice, amountInDefaultCurrency, rate, qty);
                    var invoiceItemid = _franchiseeServicefeeService.Save(model, serviceFee, invoiceId);
                    loanSchedule.OverPaidAmount = amountInDefaultCurrency;
                    loanSchedule.Balance = loanSchedule.Balance - amountInDefaultCurrency;
                    loanSchedule.TotalPrincipal = loanSchedule.TotalPrincipal + amountInDefaultCurrency;
                    loanSchedule.Principal = loanSchedule.Principal + amountInDefaultCurrency;
                    loanSchedule.CalculateReschedule = true;
                    _franchiseeLoanScheduleRepository.Save(loanSchedule);

                    if (loanSchedule.Balance <= 0 && loanSchedule.FranchiseeLoan != null)
                    {
                        loanSchedule.FranchiseeLoan.IsCompleted = true;
                        loanSchedule.FranchiseeLoan.IsNew = false;
                        _franchiseeLoanRepository.Save(loanSchedule.FranchiseeLoan);
                        SendMailForLoanCompletion(loanSchedule.FranchiseeLoan);
                    }
                }
            }
            catch (Exception ex)
            {
                //_logService.Error(ex);
                throw ex;
            }
        }


        public void CreditFranchiseeAccount(decimal amount, long franchiseeId, long invoiceId)
        {
            try
            {
                var franchiseeInvoice = _franchiseeInvoiceRepository.Get(x => x.InvoiceId == invoiceId);
                var loanInvoiceItem = franchiseeInvoice.Invoice.InvoiceItems.Where(x => x.ServiceFeeInvoiceItem != null
                                            && x.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.Loan).FirstOrDefault();

                var currencyRate = franchiseeInvoice.Invoice.InvoiceItems.Where(x => x.CurrencyExchangeRate != null).FirstOrDefault().CurrencyExchangeRate.Rate;
                var amountInDefaultCurrency = amount.ToDefaultCurrency(currencyRate);

                var loanSchedule = loanInvoiceItem != null ? _franchiseeLoanScheduleRepository.Get(x => x.InvoiceItemId == loanInvoiceItem.Id) : null;
                if (franchiseeInvoice != null && loanSchedule != null)
                {

                    if (loanSchedule.Balance <= 0 && loanSchedule.FranchiseeLoan != null)
                    {
                        loanSchedule.FranchiseeLoan.IsCompleted = true;
                        loanSchedule.FranchiseeLoan.IsNew = false;
                        _franchiseeLoanRepository.Save(loanSchedule.FranchiseeLoan);
                    }
                    if (loanSchedule.Balance < 0)
                    {
                        SaveAccountCreditForLessBalance(loanSchedule);
                    }
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw ex;
            }
        }
        public void SendMailForLoanCompletion(FranchiseeLoan loanSchedule)
        {
            _paymentReminderPollingAgent.CreateLoanCompletionNotification(loanSchedule);
        }
        public void SaveAccountCreditForLessBalance(FranchiseeLoanSchedule loanSchedule)
        {
            var description = "Creding Amount For Loan Id " + loanSchedule.LoanId + " for amount " + loanSchedule.FranchiseeLoan.Amount + " having description " + loanSchedule.FranchiseeLoan.Description;

            var franchiseeCreditAmount = new FranchiseeAccountCredit
            {
                Amount = loanSchedule.Balance * -1,
                Description = description,
                CreditedOn = _clock.ToUtc(DateTime.Now),
                FranchiseeId = loanSchedule.FranchiseeLoan != null ? loanSchedule.FranchiseeLoan.FranchiseeId : default(long),
                IsNew = true,
                CreditTypeId = (long)AccountCreditType.AllSalesCredit,
                CurrencyExchangeRateId = loanSchedule.FranchiseeLoan != null ? loanSchedule.FranchiseeLoan.CurrencyExchangeRateId : default(long),
                RemainingAmount = loanSchedule.Balance * -1

            };
            _franchiseeAccountCreditRepository.Save(franchiseeCreditAmount);
        }
    }
}
