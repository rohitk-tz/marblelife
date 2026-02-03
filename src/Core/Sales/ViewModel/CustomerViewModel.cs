using Core.Application.Attribute;
using Core.Geo.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class CustomerViewModel
    {
        public long CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [DownloadField(Required = false)]
        public IList<string> Emails { get; set; }
        [DownloadField(false, true)]
        public AddressViewModel Address { get; set; }
        public string ContactPerson { get; set; }

        public string PhoneNumber { get; set; }
        public long? LastInvoiceId { get; set; }
        public string QbInvoiceId { get; set; }

        [DownloadField(CurrencyType = "$")]
        public decimal Amount { get; set; }
        public string MarketingClass { get; set; }
        

        [DownloadField(Required = false)]
        public decimal CurrencyRate { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string FranchiseeName { get; set; }
        public string CurrencyCode { get; set; }

        [DownloadField(Required = false)]
        public string ClassTypeId { get; set; }

        [DownloadField(Required = false)]
        public bool  IsSynced { get; set; }
        public DateTime? LastServicedDate { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal? TotalSales { get; set; }
        [DownloadField(Required = false)]
        public int? NoOfSales { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal? AvgSales { get; set; }
        public int status { get; set; }
        public long TotalNumberOfInvoices { get; set; }
    }
}
