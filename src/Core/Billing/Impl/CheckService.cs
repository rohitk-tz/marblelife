using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Notification;
using Core.Organizations.Domain;
using System;
using System.Linq;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class CheckService : ICheckService
    {
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<Check> _checkRepository;
        private readonly ICheckFactory _checkFactory;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IPaymentFactory _paymentFactory;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IInvoicePaymentService _invoicePaymentService;
        private readonly IRepository<CheckPayment> _checkPaymentRepository;
        private readonly IPaymentReminderPollingAgent _paymentReminderPollingAgent;
        private readonly IPaymentService _paymentService;
        private readonly ILogService _logService;

        public CheckService(IUnitOfWork unitOfWork, ICheckFactory checkFactory, IPaymentFactory paymentFactory, IInvoicePaymentService invoicePaymentService,
            IPaymentReminderPollingAgent paymentReminderPollingAgent, IPaymentService paymentService, ILogService logService)
        {
            _checkRepository = unitOfWork.Repository<Check>();
            _checkFactory = checkFactory;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _paymentRepository = unitOfWork.Repository<Payment>();
            _paymentFactory = paymentFactory;
            _invoiceRepository = unitOfWork.Repository<Invoice>();
            _invoicePaymentService = invoicePaymentService;
            _checkPaymentRepository = unitOfWork.Repository<CheckPayment>();
            _paymentReminderPollingAgent = paymentReminderPollingAgent;
            _paymentService = paymentService;
            _logService = logService;

        }
        public ProcessorResponse SaveCheck(CheckPaymentEditModel model, long franchiseeId, long invoiceId)
        {
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            if (franchisee == null)
            {
                _logService.Error("Franchisee does not exist.");
                return new ProcessorResponse
                {
                    Message = "Franchisee does not exist.",
                    ProcessorResult = ProcessorResponseResult.Fail,
                    RawResponse = "Franchisee does not exist."
                };
            }
            model.InvoiceId = invoiceId;
            ProcessorResponse response = null;
            if (model.ProfileTypeId == (long)AuthorizeNetAccountType.AdFund)
            {
                response = SaveCheckInfo((long)AuthorizeNetAccountType.AdFund, model, response, franchiseeId, invoiceId);
            }
            else if (model.ProfileTypeId != (long)AuthorizeNetAccountType.AdFund)
            {
                if (model.IsLoanOverPayment && model.OverPaymentAmount > 0)
                {
                    _paymentService.CreateOverPaymentInvoiceItem(model.OverPaymentAmount, franchiseeId, invoiceId);
                }
                response = SaveCheckInfo((long)AuthorizeNetAccountType.Royalty, model, response, franchiseeId, invoiceId);
            }
            return response;
        }

        private ProcessorResponse SaveCheckInfo(long accountTypeId, CheckPaymentEditModel model, ProcessorResponse response, long franchiseeId, long invoiceId)
        {
            Check check = null;
            try
            {
                var invoice = _invoiceRepository.Get(model.InvoiceId);
                var chargedAmount = invoice.InvoiceItems.Sum(x => x.Amount);

                var amount = _paymentService.AccountCreditPayment(chargedAmount, invoice, franchiseeId);
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
                if (amount > 0)
                {
                    chargedAmount = amount;
                    check = _checkFactory.CreateDomain(model);
                    _checkRepository.Save(check);

                    model.CheckId = check.Id;

                    var instrumentTypeId = (long)InstrumentType.Check;

                    var currencyexchangerate = _invoiceRepository.Get(invoiceId).InvoiceItems.Select(x => x.CurrencyExchangeRate).First();
                    var payment = _paymentFactory.CreatePaymentDomain(chargedAmount, instrumentTypeId, currencyexchangerate.Id);
                    _paymentRepository.Save(payment);

                    invoice.StatusId = (long)InvoiceStatus.Paid;
                    _invoiceRepository.Save(invoice);

                    _invoicePaymentService.Save(invoice.Id, payment.Id);

                    var checkPayment = _checkFactory.CreateCheckPayment(check, payment);
                    _checkPaymentRepository.Save(checkPayment);

                    // Payment confirmation Notification
                    _paymentReminderPollingAgent.CreatePaymentConfirmationNotification(invoice, payment, franchiseeId);
                    _logService.Info(string.Format("Payment of Invoice# {0} for Franchisee# {1} is done via check#", invoiceId, franchiseeId, check.Id));

                    return new ProcessorResponse
                    {
                        Message = "Payment is done.",
                        ProcessorResult = ProcessorResponseResult.Accepted,
                        RawResponse = "Payment is done."
                    };
                }
                return response;
            }
            catch (Exception ex)
            {
                _logService.Error(ex);
                return new ProcessorResponse
                {
                    Message = "There are some problem in processing Check.",
                    ProcessorResult = ProcessorResponseResult.Fail,
                    RawResponse = "There are some problem in processing Check."
                };
            }
        }
    }
}
