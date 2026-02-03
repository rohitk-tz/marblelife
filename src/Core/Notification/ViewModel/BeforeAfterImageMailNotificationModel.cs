using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification.ViewModel
{
    public class BeforeAfterImageMailNotificationModel
    {
        public string FranchiseeName { get; set; }
        public string CustomerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
        public string FromMail { get; set; }
        public string Email { get; set; }
        public long? FranchiseeId { get; set; }
        public long? CustomerId { get; set; }
        public string AssigneePhone { get; set; }
        public string CCMail { get; set; }
        public string Code { get; set; }

        public string Url { get; set; }
        public string IsSigned { get; set; }
        public string InvoicesName { get; set; }
        public string InvoicesSignedBy { get; set; }
        public string AllInvoicesSigned { get; set; }
        public string AllInvoicesNotSigned { get; set; }
        public string SalesRepName { get; set; }
        public string SchedulerUrl { get; set; }
        public long? JobId { get; set; }
        public string Name { get; set; }
        public bool? IsFromUrl { get; set; }
        public bool? IsFromJob { get; set; }
        public string DoneFrom { get; set; }

        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }
        public string OfficeNumber { get; set; }

        public BeforeAfterImageMailNotificationModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }
}
