using Core.Organizations.ViewModels;
using Core.Users.ViewModels;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    public class FranchiseeDetailsModel
    {
        public FranchiseeEditModel Franchisee { get; set; }
        public IList<UserEditModel> users { get; set; }
    }
}
