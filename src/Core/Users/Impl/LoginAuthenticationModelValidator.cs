using Core.Users.ViewModels;
using Core.Application;
using Core.Organizations.Domain;
using Core.Application.ViewModel;
using FluentValidation;
using Core.Application.Attribute;
using Core.Application.ValueType;
using Core.Localization.Validations;
using Core.Users.Domain;
using System.Linq;
using Core.Scheduler.Domain;

namespace Core.Users.Impl
{
    [DefaultImplementation(Interface = typeof(IValidator<LoginAuthenticationModel>))]

    public class LoginAuthenticationModelValidator : AbstractValidator<LoginAuthenticationModel>, ILoginAuthenticationModelValidator
    {
        private readonly IUserLoginService _userLoginService;
        private readonly ICryptographyOneWayHashService _cryptographyService;

        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;

        public LoginAuthenticationModelValidator(IUserLoginService userLoginService, ICryptographyOneWayHashService cryptographyService, IUnitOfWork unitOfWork)
        {
            _userLoginService = userLoginService;
            _cryptographyService = cryptographyService;
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();

            RuleFor(x => x.Username).NotNull().WithMessage("Username cannot be null").NotEmpty().WithMessage("Username cannot be empty");
            RuleFor(x => x.Password).NotNull().WithMessage("Password cannot be null").NotEmpty().WithMessage("Password cannot be empty");
        }

        public bool IsValid(LoginAuthenticationModel model)
        {
            var userLogin = _userLoginService.GetbyUserName(model.Username);

            if (userLogin == null)
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.InvalidCredentials);
                return false;
            }

            if (userLogin.IsLocked)
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.AccountLocked);
                return false;
            }

            var oru = _organizationRoleUserRepository.Fetch(x => x.UserId == userLogin.Id && x.IsDefault).ToList();

            if (!oru.Any(x => x.IsActive))
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.UserDeactivated);
                return false;
            }

            var isValid = _cryptographyService.Validate(model.Password, new SecureHash(userLogin.Password, userLogin.Salt));

            if (isValid)
            {
                _userLoginService.UpdateforValidAttempt(userLogin);
                return true;
            }

            userLogin = _userLoginService.UpdateforInvalidAttempt(userLogin);

            if (userLogin.IsLocked)
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.AccountLocked);
            }
            else if (userLogin.LoginAttemptCount == (UserLogin.MaxAttempts - 2))
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.InvalidCredentialsLastTwoAttempts);
            }
            else
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.InvalidCredentials);
            }

            return false;
        }

        public bool IsValidForReviewAPI(LoginAuthenticationModel model)
        {
            var userLogin = _userLoginService.GetbyUserName(model.Username);

            if (userLogin == null)
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.InvalidCredentials);
                return false;
            }


            var oru = _organizationRoleUserRepository.Fetch(x => x.UserId == userLogin.Id && x.IsDefault).ToList();

            if (!oru.Any(x => x.IsActive))
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.UserDeactivated);
                return false;
            }

            var isValid = _cryptographyService.Validate(model.Password, new SecureHash(userLogin.Password, userLogin.Salt));

            if (isValid)
            {
               // _userLoginService.UpdateforValidAttempt(userLogin);
                return true;
            }

            userLogin = _userLoginService.UpdateforInvalidAttempt(userLogin);

            if (userLogin.IsLocked)
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.AccountLocked);
            }
            else if (userLogin.LoginAttemptCount == (UserLogin.MaxAttempts - 2))
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.InvalidCredentialsLastTwoAttempts);
            }
            else
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.InvalidCredentials);
            }

            return false;
        }


        public bool IsValidForCustomer(LoginCustomerAuthenticationModel model)
        {
            var customerLogin = _userLoginService.GetbyCode(model.Code);

            if (customerLogin == null)
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.InvalidCodeForCustomer);
                return false;
            }

            if (customerLogin.IsActive==false)
            {
                model.Message = FeedbackMessageModel.CreateErrorMessage(Login.CodeExpired);
                return false;
            }
            return true;
        }
        public CustomerSignatureInfo GetCustomerSignatureInfo(LoginCustomerAuthenticationModel model)
        {
            var customerLogin = _userLoginService.GetbyCode(model.Code);

            return customerLogin;
        }
    }
}
