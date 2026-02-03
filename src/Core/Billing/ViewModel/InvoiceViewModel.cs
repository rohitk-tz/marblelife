using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class InvoiceViewModel
    {
        public long InvoiceId { get; set; }
        public string RoyalityOrAddFund { get; set; }
        public string Status { get; set; }
        public string FranchiseeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? UploadedOn { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentMode { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal? TotalSales { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal? AccruedAmount { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal? PaidAmount { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal? AccountCredit { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal AdFund { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal Royalty { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal MinRoyaltyAmount { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal RoyaltyLateFee { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal SalesDataLateFee { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal InterestRate { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal FixedAccountingCharges { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal VariableAccountingCharges { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal LoanAndLoanIntCharges { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal ISQFTCharges { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal WebSEOCharges { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal BackUpPhoneNumber { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal OneTimeCharges { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal RecruitingFee { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal PayrollProcessing { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal NationalCharges { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal FranchiseeEmailFee { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal? PayableAmount { get; set; }
        public bool? IsSEOCostApplied { get; set; }
        [DownloadField(CurrencyType = "$")]
        public decimal CheckSum { get; set; }
        public string OneTimeNote { get; set; }
        public string ReconciliationNotes { get; set; }

        //Not Include in Excel download
        [DownloadField(Required = false)]
        public long FranchiseeId { get; set; }
       
        [DownloadField(Required = false)]
        public long StatusId { get; set; }
        [DownloadField(Required = false)]
        public string CurrencyCode { get; set; }
        [DownloadField(Required = false)]
        public decimal CurrencyRate { get; set; }
       
        [DownloadField(Required = false)]
        public long AccountTypeId { get; set; }

        [DownloadField(Required = false)]
        public bool IsDownloaded { get; set; }
    }
}
