using Core.Application;
using Core.Application.Attribute;
using Core.Users.Domain;
using Core.Users.ViewModels;
using System;
using System.Text;

namespace Core.Users.Impl
{
    [DefaultImplementation]
    public class UserLoginFactory : IUserLoginFactory
    {
        private readonly ICryptographyOneWayHashService _cryptographyService;
        private readonly ISettings _settings;

        public UserLoginFactory(ICryptographyOneWayHashService cryptographyService, ISettings settings)
        {
            _cryptographyService = cryptographyService;
            _settings = settings;
        }

        public UserLogin CreateDomain(UserLoginEditModel model, Person person, UserLogin inPersistence)
        {
            var userLogin = new UserLogin()
            {
                Person = person
            };

            if (inPersistence != null && inPersistence.Id > 0)
            {
                userLogin = inPersistence;
            }

            if (model.UserName != null)
                userLogin.UserName = model.UserName.ToLower();

            if (model.Id < 1 || model.ChangePassword)
            {
                if (model.SendUserLoginViaEmail)
                {
                    model.Password = GetRandomPassword();
                }

                var hash = _cryptographyService.CreateHash(model.Password);
                userLogin.Password = hash.HashedText;
                userLogin.Salt = hash.Salt;
            }

            userLogin.IsNew = model.Id <= 0;
            userLogin.IsLocked = false;
            return userLogin;
        }

        private string GetRandomPassword() 
        {
            int size = _settings.PasswordLength;
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char pass;
            for (int i = 1; i < size + 1; i++)
            {
                pass = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(pass);
            }
            return builder.ToString().ToLower();
        }

        public UserLoginEditModel CreateEditModel(UserLogin userLogin)
        {
            if (userLogin.Id <= 0)
            {
                return new UserLoginEditModel { Id = userLogin.Id, UserName = userLogin.UserName, SendUserLoginViaEmail = true, IsLocked = false, ChangePassword = userLogin.Id <= 0 };
            }
            else
            {
                return new UserLoginEditModel { Id = userLogin.Id, UserName = userLogin.UserName, SendUserLoginViaEmail = false, IsLocked = false, ChangePassword = userLogin.Id <= 0 };
            }
        }

        public UserLogin CreateResetPasswordDomain(ChangePasswordEditModel model, UserLogin inPersistence)
        {
            var userLogin = inPersistence;

            SetPassword(model.Password, userLogin);

            return userLogin;
        }

        private void SetPassword(string text, UserLogin userLogin)
        {
            var hash = _cryptographyService.CreateHash(text.Trim());
            userLogin.Password = hash.HashedText;
            userLogin.Salt = hash.Salt;
        }

        public UserLogin CreateDomain(OrganizationOwnerEditModel organizationOwner, string email)
        {
            var userLogin = new UserLogin();

            if (organizationOwner.SendUserLoginViaEmail == true)
            {
                organizationOwner.Password = GetRandomPassword();
            }
            userLogin.Password = organizationOwner.Password;

            var hash = _cryptographyService.CreateHash(userLogin.Password);
            userLogin.Password = hash.HashedText;
            userLogin.Salt = hash.Salt;

            userLogin.Id = organizationOwner.OwnerId;
            userLogin.UserName = email;
            userLogin.IsNew = organizationOwner.OwnerId <= 0;
            userLogin.IsLocked = false;
            userLogin.LoginAttemptCount = 0;
            return userLogin;
        }
    }
}
