using Core.Application.Attribute;
using Core.Geo.ViewModel;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;


namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class InvoiceDetailsForAttactmentViewModel
    {
        public long? InvoiceId { get; set; }
        public long AuditInvoiceId { get; set; }
        public long AnnualUploadId { get; set; }
        public string QBInvoiceNumber { get; set; }
        public string QBInvoiceNumbers { get; set; }
        public string Customer { get; set; }

        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string FranchiseeName { get; set; }
        public string PhoneNumbers { get; set; }
        public string FranchiseePhoneNumbers { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal GrandTotal { get; set; }
        public string GeneratedOn { get; set; }
        public string DueDate { get; set; }
        public decimal? SalesAmount { get; set; }
        public string Address { get; set; }
        public ICollection<FranchiseeSalesPaymentEditModel> Payments { get; set; }
        public ICollection<InvoiceItemEditModel> InvoiceItems { get; set; }
        public string CurrencyCode { get; set; }
        public string UploadEndDate { get; set; }
        public long StatusId { get; set; }
        public string InvoiceDate { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerEmails { get; set; }
        public decimal LoanAmount { get; set; }
        public string FranchiseeAddress { get; set; }
        public decimal ReportId { get; set; }
        public string ReportName { get; set; }
        public decimal RemainingLoanAmount { get; set; }
        public decimal CurrentLoanAmount { get; set; }
    }
}
