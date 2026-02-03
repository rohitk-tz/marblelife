using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Dashboard.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeDirectoryListModel
    {
        public IEnumerable<FranchiseeDirectoryViewModel> Collection { get; set; }
    }
}
