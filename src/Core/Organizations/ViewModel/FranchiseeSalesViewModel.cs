using Core.Application.Attribute;
using Core.Geo.ViewModel;
using System;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeSalesViewModel
    {
        [DownloadField(Required = false)]
        public long Id { get; set; }

        public long? InvoiceId { get; set; }

        public string CustomerName { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string QbInvoiceNumber { get; set; }
        [DownloadField(false, true)]
        public AddressViewModel Address { get; set; }

        [DownloadField(CurrencyType = "$")]
        public decimal TotalAmount { get; set; }

        [DownloadField(CurrencyType = "$")]
        public decimal PaidAmount { get; set; }

        [DownloadField(Required = false)]
        public decimal? AccruedAmount { get; set; }

        public string MarketingClass { get; set; }

        public string FranchiseeName { get; set; }

        [DownloadField(Required = false)]
        public long FranchiseeId { get; set; }

        [DownloadField(Required = false)]
        public bool IsInvoice { get; set; }

        public string CurrencyCode { get; set; }

        [DownloadField(Required = false)]
        public decimal CurrencyRate { get; set; }
        [DownloadField(Required = false)]
        public bool IsFeedbackRequestSent { get; set; }
        [DownloadField(Required = false)]
        public bool IsFeedbackResponseReceived { get; set; }
        [DownloadField(Required = false)]
        public long ResponseId { get; set; }
        [DownloadField(Required = false)]
        public bool IsSystemGenerated { get; set; }
        [DownloadField(Required = false)]
        public string ServiceList { get; set; }
    }
}
