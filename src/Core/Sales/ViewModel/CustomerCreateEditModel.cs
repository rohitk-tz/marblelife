using Core.Application.ViewModel;
using Core.Geo.ViewModel;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    public class CustomerCreateEditModel : EditModelBase
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string ContactPerson { get; set; }

        public ICollection<CustomerEmail> CustomerEmails { get; set; }

        public string Phone { get; set; }

        public long? MarketingClassId { get; set; }


        public long? SubMarketingClassId { get; set; }

        public AddressEditModel Address { get; set; }

        public DateTime? DateCreated { get; set; }

        public bool ReceiveNotification { get; set; }
        public decimal? TotalSales { get; set; }
        public int? NoOfSales { get; set; }
        public decimal? AvgSales { get; set; }

        public int Status { get; set; }

        public long? LastInvoiceId { get; set; }
        public string QbInvoiceId { get; set; }


        public CustomerCreateEditModel()
        {
            CustomerEmails = new List<CustomerEmail>();
        }
    }
}
