using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Notification.ViewModel
{
    [NoValidatorRequired]
    public class SalesdataUploadReminderNotificationModel
    {
        public string FullName { get; set; }
        public string Franchisee { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public IList<DateRangeViewModel> DateRange { get; set; }
        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }
        public EmailNotificationModelBase Base { get; private set; }

        public SalesdataUploadReminderNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
