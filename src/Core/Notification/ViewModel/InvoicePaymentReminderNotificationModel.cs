using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Notification.ViewModel
{
    [NoValidatorRequired]
    public class InvoicePaymentReminderNotificationModel
    {
        public string FullName { get; set; }
        public string Franchisee { get; set; }
        public IList<PaymentViewModelForInvoice> InvoiceDetailList { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }
        public InvoicePaymentReminderNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
