using Core.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users
{
    public interface IPasswordResetService
    {
        bool SendPasswordLink(string email);
        bool ResetPassword(ChangePasswordEditModel model);
        bool ResetPasswordExpire(ChangePasswordEditModel model);
    }
}
