using Core.Application.Attribute;
using System.Collections.Generic;
using System.Web;

namespace Core.Notification.ViewModel
{
    [NoValidatorRequired]
    public class InvoiceDetailNotificationViewModel
    {
        public string FullName { get; set; }
        public string Franchisee { get; set; }
        public IList<InvoiceViewModelForDetail> InvoiceDetailList { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string DueDate { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }

        public InvoiceDetailNotificationViewModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
