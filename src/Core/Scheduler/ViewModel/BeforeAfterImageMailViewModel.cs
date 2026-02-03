using Core.Application.ViewModel;
using Core.Notification.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
   public class BeforeAfterImageMailViewModel
    {
        public string DoneFrom { get; set; }
        public string Body { get; set; }
        public List<FileModel> FileModel { get; set; }
        public string FranchiseeName { get; set; }
        public string CustomerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string EmailId { get; set; }
        public string FromMail { get; set; }
        public long? FranchiseeId { get; set; }
        public long? CustomerId { get; set; }

        public string CCMail { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public string IsSigned { get; set; }
        public string BackColor { get; set; }
        public string CcEmail { get; set; }
        public string SchedulerEmail { get; set; }
        public string FromEmail { get; set; }

        public string SignedInvoicesName { get; set; }
        public string UnsignedInvoicesName { get; set; }
        public string SalesRepName { get; set; }

        public bool AllInvoicesSigned { get; set; }
        public bool IsFromURL { get; set; }
        public bool MailToSalesRep { get; set; }
        public long? SchedulerId { get; set; }
        public string SchedulerUrl { get; set; }
        public long? JobId { get; set; }
        public string Name { get; set; }
        public bool? IsFromUrl { get; set; }
        public bool? IsFromJob { get; set; }
        public string OfficeNumber { get; set; }
        public long ToEmailId { get; set; }
    }
}
