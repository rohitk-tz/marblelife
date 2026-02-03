using Core.Application.Attribute;
using System;
using System.ComponentModel;

namespace Core.Sales.ViewModel
{
    public class Auditaddress
    {

        [DownloadField(Required = false)]
        public double TotalSum { get; set; }
        [DownloadField(Required = false)]
        public long Id { get; set; }

        [DownloadField(Required = false)]
        public long AuditInvoiceId { get; set; }

        [DownloadField(Required = false)]
        public long AnnualUploadId { get; set; }

        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }

        [DisplayName("Qb Invoice")]
        public string QbInvoice { get; set; }

        [DownloadField(Required = false)]
        public decimal WeeklyRoyality { get; set; }

        [DownloadField(Required = false)]
        public decimal AnnualRoyality { get; set; }

        [DisplayName("Invoice Id")]
        public long? InvoiceId { get; set; }

        [DownloadField(Required = false)]
        public DateTime InvoiceDate { get; set; }

        [DisplayName("Invoice Date")]
        public string InvoiceDateForExcel { get; set; }
        [DownloadField(Required = false)]
        public decimal AnnualSalesAmount { get; set; }
        [DownloadField(Required = false)]
        public decimal AnnualPaidAmount { get; set; }

        [DownloadField(Required = false)]
        public decimal WeeklySalesAmount { get; set; }

        [DownloadField(Required = false)]
        public decimal WeeklyPaidAmount { get; set; }

        [DisplayName("Annual Sales Amount")]
        [DownloadField(CurrencyType = "$")]
        public string AnnualSalesAmountForExcel { get; set; }

        [DisplayName("Annual Paid Amount")]
        [DownloadField(CurrencyType = "$")]
        public string AnnualPaidAmountForExcel { get; set; }

        [DisplayName("Weekly Sales Amount")]
        [DownloadField(CurrencyType = "$")]
        public string WeeklySalesAmountForExcel { get; set; }

        [DisplayName("Weekly Paid Amount")]
        [DownloadField(CurrencyType = "$")]
        public string WeeklyPaidAmountForExcel { get; set; }

        [DownloadField(Required = false)]
        public string FranchiseeName { get; set; }

        [DownloadField(Required = false)]
        public long FranchiseeId { get; set; }

        [DownloadField(Required = false)]
        public string CurrencyCode { get; set; }

        [DownloadField(Required = false)]
        public decimal CurrencyRate { get; set; }

        [DisplayName("Annual Report Status")]
        public string ReportTypeDescription { get; set; }
        [DownloadField(Required = false)]
        [DisplayName("")]
        public string Blank { get; set; }

        [DownloadField(Required = false)]
        
        public decimal AnnuallyDifference { get; set; }
        [DownloadField(Required = false)]
        
        public decimal WeeklyDifference { get; set; }

        [DownloadField(Required = false)]
     
        public decimal AnnuallySalesDifference { get; set; }

        [DownloadField(Required = false)]
        public decimal AnnuallyPaidDifference { get; set; }

        [DisplayName("A-B")]
        [DownloadField(CurrencyType = "$")]
        public string AnnuallyDifferenceForExcel { get; set; }
        [DisplayName("C-D")]
        [DownloadField(CurrencyType = "$")]
        public string WeeklyDifferenceForExcel { get; set; }

        [DisplayName("A-C")]
        [DownloadField(CurrencyType = "$")]
        public string AnnuallySalesDifferenceForExcel { get; set; }
        [DisplayName("B-D")]
        [DownloadField(CurrencyType = "$")]
        public string AnnuallyPaidDifferenceForExcel { get; set; }

        [DownloadField(Required = false)]
        public long GroupTypeId { get; set; }
        [DownloadField(Required = false)]
        public long ReportTypeId { get; set; }

    }
}
