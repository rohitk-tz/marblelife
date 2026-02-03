using Core.Application.Attribute;
using System;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class AccountCreditListFilter
    {
        public string Text { get; set; }
        public long FranchiseeId { get; set; }
        public string CustomerName { get; set; }
        public string QbInvoiceNumber { get; set; }
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
    }
}
