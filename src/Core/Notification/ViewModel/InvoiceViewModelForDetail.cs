using Core.Application.Attribute;
using Core.Billing.ViewModel;
using System.Collections.Generic;

namespace Core.Notification.ViewModel
{
    [NoValidatorRequired]
    public class InvoiceViewModelForDetail
    {
        public long InvoiceId { get; set; }
        public string TotalPayment { get; set; }
        public string GeneratedOn { get; set; }
        public string DueDate { get; set; }
        public string  AdFund { get; set; }
        public string Royalty { get; set; }


        public ICollection<InvoiceItemEditModel> InvoiceItems { get; set; }
    }
}
