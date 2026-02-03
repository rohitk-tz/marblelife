using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeAccountCreditListModel
    {
        public IEnumerable<FranchiseeAccountCreditViewModel> Collection { get; set; }
        public PagingModel PagingModel { get; set; }
        public AccountCreditListFilter Filter { get; set; }
    }
}
