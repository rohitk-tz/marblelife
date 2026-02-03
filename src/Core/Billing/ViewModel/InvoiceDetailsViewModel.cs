using Core.Application.Attribute;
using Core.Geo.ViewModel;
using Core.Organizations.ViewModel;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class InvoiceDetailsViewModel
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
        public IEnumerable<PhoneEditModel> PhoneNumbers { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime GeneratedOn { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? SalesAmount { get; set; }
        public AddressViewModel Address { get; set; }
        public ICollection<FranchiseeSalesPaymentEditModel> Payments { get; set; }
        public ICollection<InvoiceItemEditModel> InvoiceItems { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime UploadEndDate { get; set; }
        public long StatusId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<EmailEditModel> CustomerEmails { get; set; }
        public decimal LoanAmount { get; set; }
        public AddressViewModel FranchiseeAddress { get; set; }
        public decimal ReportId { get; set; }
        public string ReportName { get; set; }
        public decimal RemainingLoanAmount { get; set; }
        public decimal CurrentLoanAmount { get; set; }
        public string FranchiseePhone { get; set; }
        public SumByCategory SumByCategory { get; set; }
        public InvoiceDetailsViewModel()
        {
            PhoneNumbers = new List<PhoneEditModel>();
        }
    }
}
