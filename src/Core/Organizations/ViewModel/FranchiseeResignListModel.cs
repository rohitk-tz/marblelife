using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Organizations.ViewModels;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
   public class FranchiseeResignListModel
    {
        public List<FranchiseeModel> Collection { get; set; }
        public FranchiseeListFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
