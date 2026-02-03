using Core.Application.ViewModel;

namespace Core.Users.ViewModels
{
    public class UserLoginEditModel : EditModelBase
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public bool ChangePassword { get; set; }
        
        public string Password { get; set; }

        public bool SendUserLoginViaEmail { get; set; }
        public string ConfirmPassword { get; set; }

        public bool IsLocked { get; set; }

    }
}
