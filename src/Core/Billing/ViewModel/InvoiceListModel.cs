using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Billing.ViewModel
{
    [NoValidatorRequired]
    public class InvoiceListModel
    {
        public IEnumerable<InvoiceViewModel> Collection { get; set; }
        public InvoiceListFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
        public decimal? TotalUnPaidAmount { get; set; }
        public string FranchiseeName { get; set; }
    }

    [NoValidatorRequired]
    public class InvoiceReconciliationNotesModel
    {
        public long Id { get; set; }
        public string ReconciliationNotes { get; set; }
    }
}
