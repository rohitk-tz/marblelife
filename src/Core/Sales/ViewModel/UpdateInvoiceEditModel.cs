using Core.Application.ViewModel;
using System;

namespace Core.Sales.ViewModel
{
   public class UpdateInvoiceEditModel : EditModelBase
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string ServiceName { get; set; }
        public string MarketingClass { get; set; }
        public decimal Rate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string FileId { get; set; }
        public string Description { get; set; }
        public string NewDescription { get; set; }
        public string FinalDescription { get; set; }
        public string NewService { get; set; }
        public string FinalClass { get; set; }
        public string NewClass { get; set; }
        public string FinalService { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int IsChanged { get; set; }
        public string Item { get; set; }
    }
}
