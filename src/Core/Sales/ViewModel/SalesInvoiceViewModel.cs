using Core.Application.Attribute;
using Core.Geo.ViewModel;
using System;

namespace Core.Sales.ViewModel
{
    public class SalesInvoiceViewModel
    {
        public long InvoiceId { get; set; }
        public string Franchisee { get; set; }

        public string CustomerName { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string QBInvoiceNumber { get; set; }

        [DownloadField(false, true)]
        public AddressViewModel Address { get; set; }

        public string MarketingClass { get; set; }
        public string Service { get; set; }
        public long InvoiceItemId { get; set; }
        public string Description { get; set; }

        [DownloadField(CurrencyType = "$")]
        public decimal Amount { get; set; }
        public string NewClass { get; set; }
        public string NewService { get; set; }
    }
}
