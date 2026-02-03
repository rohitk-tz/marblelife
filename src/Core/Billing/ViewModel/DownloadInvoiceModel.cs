using CsvHelper.Configuration.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Billing.ViewModel
{
    public class DownloadInvoiceModel
    {
        [DisplayName("InvoiceId#")]
        public long ItemId { get; set; }
        public long InvoiceItem { get; set; }
        //[Format("MM\\/dd\\/yyyy")]
        //public DateTime StartDate { get; set; }
        [Format("MM\\/dd\\/yyyy")]
        public DateTime EndDate { get; set; }
        //[Format("MM\\/dd\\/yyyy")]
        //public DateTime DueDate { get; set; }
        //[Format("MM\\/dd\\/yyyy")]
        //public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Customer { get; set; }
        public string BillToLine1 { get; set; }
        public string BillToLine2 { get; set; }
        public string BillToLine3 { get; set; }
        public string BillToCity { get; set; }
        public string BillToState { get; set; }
        public string BillToPostalCode { get; set; }
        public string Terms { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Price { get; set; }
        [DisplayName("Adfund/Royality")]
        public string AdfundRoyalty { get;  set; }
    }
}
