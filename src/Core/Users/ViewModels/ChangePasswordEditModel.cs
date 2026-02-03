using Core.Application.Attribute;

namespace Core.Users.ViewModels
{
    [NoValidatorRequired]
    public class ChangePasswordEditModel
    {
        public string Key { get; set; }

        public string Password { get; set; }
    }
}
