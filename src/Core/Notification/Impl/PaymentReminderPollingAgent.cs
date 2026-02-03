using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Notification.Domain;
using Core.Organizations.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class PaymentReminderPollingAgent : IPaymentReminderPollingAgent
    {
        private readonly IRepository<PaymentMailReminder> _paymentMailReminderRepository;
        private readonly IRepository<NotificationType> _notificationTypeRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IClock _clock;
        private readonly ISettings _settings;
        private IUnitOfWork _unitOfWork;
        private ILogService _logService;

        public PaymentReminderPollingAgent(IUnitOfWork unitOfWork, IUserNotificationModelFactory userNotificationModelFactory, IClock clock,
            ISettings settings, ILogService logService)
        {
            _userNotificationModelFactory = userNotificationModelFactory;
            _paymentMailReminderRepository = unitOfWork.Repository<PaymentMailReminder>();
            _notificationTypeRepository = unitOfWork.Repository<NotificationType>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _clock = clock;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _logService = logService;
        }
        public void CreateNotificationReminderForPayment()
        {

            if (!IsNotificationQueuingEnabled())
            {
                _logService.Debug("Payment Reminder Notification Queuing is disabled");
                return;
            }
            var franchiseeWithUnpaidInvoices = _franchiseeRepository.Table.Where(x => x.FranchiseeInvoices.Any(y => y.Invoice.Lookup.Id == (long)InvoiceStatus.Unpaid)
                                                && x.Organization.IsActive).ToArray();

            if (franchiseeWithUnpaidInvoices == null || franchiseeWithUnpaidInvoices.Count() < 1)
            {
                _logService.Debug("No records found.");
                return;
            }
            try
            {
                foreach (var item in franchiseeWithUnpaidInvoices)
                {
                    _unitOfWork.StartTransaction();
                    List<FranchiseeInvoice> franchiseeInvoiceList = new List<FranchiseeInvoice>();

                    var unpaidInvoiceList = item.FranchiseeInvoices.Where(x => x.Invoice.StatusId == (long)InvoiceStatus.Unpaid).ToList();


                    foreach (var franchiseeInvoice in unpaidInvoiceList)
                    {
                        if (_paymentMailReminderRepository.Table.Any(x => x.InvoiceId == franchiseeInvoice.InvoiceId && x.Date == _clock.UtcNow.Date))
                        {
                            _logService.Debug("Reminder already send.");
                            continue;
                        }

                        var currentDate = _clock.UtcNow;
                        var dueDate = franchiseeInvoice.Invoice.DueDate;
                        if (currentDate >= dueDate)
                        {
                            franchiseeInvoiceList.Add(franchiseeInvoice);
                        }
                    }
                    if (franchiseeInvoiceList.Any())
                    {
                        _userNotificationModelFactory.CreatePaymentReminderNotification(franchiseeInvoiceList, item);

                        foreach (var invoice in franchiseeInvoiceList)
                        {
                            var domain = CreateDomain(invoice);
                            _paymentMailReminderRepository.Save(domain);
                        }
                    }
                    _unitOfWork.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }
        }

        private PaymentMailReminder CreateDomain(FranchiseeInvoice doman)
        {
            return new PaymentMailReminder
            {
                IsNew = true,
                FranchiseeId = doman.FranchiseeId,
                InvoiceId = doman.InvoiceId,
                Date = _clock.UtcNow.Date
            };
        }
        private bool IsNotificationQueuingEnabled()
        {
            var notificationtype = _notificationTypeRepository.Get(x => x.Id == (long)Enum.NotificationTypes.PaymentReminder);
            if (notificationtype.IsQueuingEnabled && notificationtype.IsServiceEnabled)
            {
                return true;
            }
            return false;
        }

        public void CreatePaymentConfirmationNotification(Invoice invoice, Payment payment, long organizationId)
        {
            if (invoice == null || payment == null)
            {
                _logService.Debug("No records found.");
                return;
            }
            _userNotificationModelFactory.CreatePaymentConfirmationNotification(invoice, payment, organizationId);
        }

        public void CreateLoanCompletionNotification(FranchiseeLoan loanSchedule)
        {
            _userNotificationModelFactory.CreateLoanCompletionNotification(loanSchedule);
        }
    }
}
