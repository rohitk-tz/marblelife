using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Organizations.ViewModel;
using System.Collections.Generic;

namespace Core.Organizations.ViewModels
{
    [NoValidatorRequired]
    public class FranchiseeListModel
    {
        public IEnumerable<FranchiseeViewModel> Collection { get; set; }
        public FranchiseeListFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
