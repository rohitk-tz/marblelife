using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Organizations.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeLoanViewModel
    {
        [DisplayName("Term#")]
        public string LoanTerm { get; set; }
        [DownloadField(Required = false)]
        public long? LoanId { get; set; }
        [DisplayName("DueDate")]
        public string DueDate { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Balance")]
        public string Balance { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Interest Amount")]
        public string Interest { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Principal Amount")]
        public string Principal { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Payable Amount")]
        public string PayableAmount { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Over Paid Amount")]
        public string OverPaidAmount { get; set; }
        [DisplayName("InvoiceItem #")]
        public string InvoiceItemId { get; set; }
        [DisplayName("Interest Amount InvoiceItem")]
        public string InterestAmountInvoiceItem { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Total Principal")]
        public string TotalPrincipal { get; set; }
        [DisplayName("Loan Adjustment")]
        public string RoyalityAdfund { get; set; }

        [DisplayName("Loan Type")]
        public string LoanType { get; set; }

    }
}
