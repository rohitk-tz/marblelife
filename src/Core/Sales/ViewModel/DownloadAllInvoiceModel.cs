using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sales.ViewModel
{
   public class DownloadAllInvoiceModel
    {
        [DisplayName("Id")]
        public long Id { get; set; }
        [DisplayName("Invoice Id")]
        public long InvoiceId { get; set; }
        
        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }
        [DisplayName("Franchisee Name")]
        public string FranchiseeName { get; set; }

        [DisplayName("Address Line 1")]
        public string AddressLine1 { get; set; }
        [DisplayName("Address Line 2")]
        public string AddressLine2 { get; set; }
        [DisplayName("City")]
        public string City { get; set; }
        [DisplayName("State")]
        public string State { get; set; }
        [DisplayName("ZipCode")]
        public string ZipCode { get; set; }
        [DisplayName("Generated On")]
        public DateTime GeneratedOn { get; set; }
        [DisplayName("Service Name")]
        public string ServiceName { get; set; }

        [DisplayName("New Service")]
        public string NewService { get; set; }
        [DisplayName("Final Service")]
        public string FinalService { get; set; }

        [DisplayName("Marketing Class")]
        public string MarketingClass { get; set; }

        [DisplayName("New Class")]
        public string NewClass { get; set; }
        [DisplayName("Final Class")]
        public string FinalClass { get; set; }

       
        [DisplayName("Rate")]
        public decimal Rate { get; set; }
        [DisplayName("Quantity")]
        public decimal Quantity { get; set; }
        [DisplayName("Amount")]
        public decimal Amount { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("New Description")]
        public string NewDescription { get; set; }
        [DisplayName("Final Description")]
        public string FinalDescription { get; set; }

        [DisplayName("Item")]
        public string Item { get; set; }
        [DisplayName("Is Changed")]
        public int IsChanged { get; set; }


    }
}
