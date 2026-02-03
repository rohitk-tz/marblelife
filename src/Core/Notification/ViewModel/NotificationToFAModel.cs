using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification.ViewModel
{
    public class NotificationToFAModel
    {
        public string Email { get; set; }
        public string FranchiseeName { get; set; }
        public string Date { get; set; }
        public string FromEmail { get; set; }
        public long OrganizationId { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public string RenewableDate { get; set; }
        public decimal? RenewableFees { get; set; }
        public string PersonName { get; set; }
        public NotificationToFAModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
