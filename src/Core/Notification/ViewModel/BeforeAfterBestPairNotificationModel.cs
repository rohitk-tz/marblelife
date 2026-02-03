using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification.ViewModel
{
  public  class BeforeAfterBestPairNotificationModel
    {
        public string PersonName { get; set; }
        public string FranchiseeName { get; set; }
        public long? FranchiseeId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string DateTimes { get; set; }
        public string FromMail { get; set; }
        public string Email { get; set; }
        public string CCMail { get; set; }
        public string AssigneePhone { get; set; }
        public string NavigationUrl { get; set; }
        public string PhotoManagemenrURL { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public BeforeAfterBestPairNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }

    }
    
}
