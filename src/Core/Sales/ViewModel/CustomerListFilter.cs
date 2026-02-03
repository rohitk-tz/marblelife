using Core.Application.Attribute;
using System;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class CustomerListFilter
    {
        public string Text { get; set; }
        public long FranchiseeId { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string ReceiveNotification { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string LastName { get; set; }
        public string AdvancedSearchBy { get; set; }
        public string AdvancedText { get; set; }

    }
}
