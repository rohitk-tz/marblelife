using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Billing.ViewModel
{
    public class InvoiceEditModel: EditModelBase
    {
        public long Id { get; set; }
        public DateTime GeneratedOn { get; set; }
        public DateTime DueDate { get; set; }
        public long? InvoiceId { get; set; }
        public long AnnualSalesDataUploadId { get; set; } 
        public long StatusId { get; set; }
        public  IList<FranchiseeSalesPaymentEditModel> Payments { get; set; }
        public  IList<InvoiceItemEditModel> InvoiceItems { get; set; }

        public InvoiceEditModel()
        {
            Payments = new List<FranchiseeSalesPaymentEditModel>();
            InvoiceItems = new List<InvoiceItemEditModel>();
        }
    }

}
