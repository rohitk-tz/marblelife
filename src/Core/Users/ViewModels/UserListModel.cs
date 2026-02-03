using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Users.ViewModels
{
    [NoValidatorRequired]
    public class UserListModel
    {
        public IEnumerable<UserViewModel> Collection { get; set; }
        public UserListFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }
}
