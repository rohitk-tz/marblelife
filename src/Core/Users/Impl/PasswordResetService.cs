using Core.Application;
using Core.Application.Attribute;
using Core.Application.Exceptions;
using Core.Notification;
using Core.Users.Domain;
using Core.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users.Impl
{
    [DefaultImplementation]
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IRepository<Person> _personRepository;
        private readonly ISettings _settings;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly UserLoginFactory _userLoginFactory;
        private readonly IClock _clock;

        public PasswordResetService(IUnitOfWork unitOfWork, ISettings settings, UserLoginFactory userLoginFactory, IClock clock, IUserNotificationModelFactory userNotificationModelFactory)
        {
            _personRepository = unitOfWork.Repository<Person>();
            _settings = settings;
            _userNotificationModelFactory = userNotificationModelFactory;
            _userLoginRepository = unitOfWork.Repository<UserLogin>(); ;
            _userLoginFactory = userLoginFactory;
            _clock = clock;
        }


        public bool SendPasswordLink(string email)
        {
            var person = _personRepository.Get(x => x.Email == email.ToLower().Trim());
            if (person != null)
            {
                if (string.IsNullOrEmpty(person.Email))
                {
                    throw new InvalidDataProvidedException("Email id does not exists. Please update your email or contact your administrator.");
                }
                var resetToken = Guid.NewGuid().ToString();
                var userLogin = person.UserLogin;
                userLogin.ResetToken = resetToken;
                userLogin.ResetTokenIssueDate = _clock.UtcNow;

                var resetLink = _settings.SiteRootUrl + "#/password/reset/" + resetToken;
                _userNotificationModelFactory.CreateForgetPasswordNotification(resetLink, person);

                person.UserLogin = userLogin;
                _personRepository.Save(person);
            }
            else
                throw new InvalidDataProvidedException("E-mail does not exist.");

            return true;
        }


        public bool ResetPassword(ChangePasswordEditModel model)
        {
            if (string.IsNullOrEmpty(model.Key)) throw new InvalidDataProvidedException("Key is empty.");

            var userlogin = (from p in _userLoginRepository.Table where p.ResetToken == model.Key && p.ResetTokenIssueDate != null select p).FirstOrDefault();

            if (userlogin != null)
            {
                if ((_clock.UtcNow - Convert.ToDateTime(userlogin.ResetTokenIssueDate.ToString())).TotalMinutes >= 1440)
                    throw new InvalidDataProvidedException("Link has been expired.");
                var userLogin = _userLoginFactory.CreateResetPasswordDomain(model, userlogin);
                userLogin.ResetTokenIssueDate = null;
                userLogin.ResetToken = null;

                var person = _personRepository.Get(userLogin.Id);
                person.UserLogin = userLogin;
                _personRepository.Save(person);
            }
            else
            {
                return false;
            }
            //throw new InvalidDataProvidedException("User does not exist or link has been expired");
            return true;
        }

        public bool ResetPasswordExpire(ChangePasswordEditModel model)
        {
            bool result = true;
            if (string.IsNullOrEmpty(model.Key)) throw new InvalidDataException("Key is empty.");
            var userlogin = (from p in _userLoginRepository.Table where p.ResetToken == model.Key && p.ResetTokenIssueDate != null select p).FirstOrDefault();
            if (userlogin != null)
            {
                if ((_clock.UtcNow - Convert.ToDateTime(userlogin.ResetTokenIssueDate.ToString())).TotalMinutes >= 1440)
                {
                    result = false;
                    // throw new InvalidDataException("Link has been expired.");
                }
            }
            else
            {
                result = false;
                // throw new InvalidDataException("User does not exist or link has been expired");
            }
            return result;
        }
    }
}
