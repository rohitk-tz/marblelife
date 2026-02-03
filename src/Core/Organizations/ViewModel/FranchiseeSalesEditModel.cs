using Core.Application.Attribute;

namespace Core.Organizations.ViewModels
{
    [NoValidatorRequired]
    public class FranchiseeSalesEditModel
    {
        public long Id { get; set; }
        public long FranchiseeId { get; set; }
        public long CustomerId { get; set; }
        public long InvoiceId { get; set; }
        public long CreditMemoId { get; set; }
        public decimal Amount { get; set; }
        public string QbInvoiceNumber { get; set; }
        public long SalesDataUploadId { get; set; }
    }
}
