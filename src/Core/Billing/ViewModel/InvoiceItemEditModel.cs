using System;

namespace Core.Billing.ViewModel
{
    public class InvoiceItemEditModel
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public long? ItemId { get; set; }
        public long ItemTypeId { get; set; }

        public string Description { get; set; }
        public string Item { get; set; }
        public string ItemOriginal { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public long CurrencyExchangeRateId { get; set; }
        public decimal CurrencyRate { get; set; }

        public string OneTimeProjectDescription { get; set; }
        public string LoneDescription { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? DebitAmount { get; set; }
    }
    public class RoyaltyInvoiceItemEditModel : InvoiceItemEditModel
    {
        public decimal? Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal SalesAmount { get; set; }
    }

    public class AdFundInvoiceItemEditModel : InvoiceItemEditModel
    {
        public decimal Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal SalesAmount { get; set; }
    }

    public class LateFeeInvoiceItemEditModel : InvoiceItemEditModel
    {
        public long? LateFeeTypeId { get; set; }
        public int WaitPeriod { get; set; }
        public decimal SalesAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedOn { get; set; }

    }
    public class InterestRateInvoiceItemEditModel : InvoiceItemEditModel
    {
        public int WaitPeriod { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
    public class ServiceFeeInvoiceItemEditModel : InvoiceItemEditModel
    {
        public long ServiceFeeTypeId { get; set; }
        public string ServiceFeeType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Percentage { get; set; }
    }
}
