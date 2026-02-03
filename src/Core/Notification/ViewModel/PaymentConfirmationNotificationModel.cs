using Core.Application.Attribute;
using Core.Billing.ViewModel;
using System.Collections.Generic;

namespace Core.Notification.ViewModel
{
    [NoValidatorRequired]
    public class PaymentConfirmationNotificationModel
    {
        public string FullName { get; set; }
        public string Franchisee { get; set; }
        public long InvoiceId { get; set; }
        public string Amount { get; set; }
        public string PaymentDate { get; set; }
        public string GeneratedOn { get; set; }
        public string DueDate { get; set; }
        public string AdFund { get; set; }
        public string Royalty { get; set; }
        public ICollection<FranchiseeSalesPaymentEditModel> Payments { get; set; }
        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public PaymentConfirmationNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
