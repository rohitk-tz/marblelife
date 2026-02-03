using System.Web;
using System.Web.Http;
using Core.Users;
using Core.Application;
using Core.Users.ViewModels;
using Core.Application.Exceptions;
using Api.Areas.Application.Controller;
using Api.Impl;
using Core.Notification;

namespace Api.Areas.Users.Controller
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        private readonly IUserLoginService _userLoginService;
        private readonly ILoginAuthenticationModelValidator _loginAuthenticationModelValidator;
        private readonly ISessionFactory _sessionFactory;
        private readonly ISessionContext _sessionContext;
        private readonly IPasswordResetService _passwordResetService;
        public LoginController()
        { }

        public LoginController(IUserLoginService userLoginService, ILoginAuthenticationModelValidator loginAuthenticationModelValidator, ISessionFactory sessionFactory, ISessionContext sessionContext, IPasswordResetService passwordResetService)
        {
            _userLoginService = userLoginService;
            _loginAuthenticationModelValidator = loginAuthenticationModelValidator;
            _sessionFactory = sessionFactory;
            _sessionContext = sessionContext;
            _passwordResetService = passwordResetService;
        }
        [HttpPost]
        public string Post([FromBody] LoginAuthenticationModel model)
        {
            var isValid = _loginAuthenticationModelValidator.IsValid(model);

            if (!isValid)
            {
                PostResponseModel.Message = model.Message;
                return null;
            }

            return SessionHelper.CreateNewSession(
                ((HttpContextBase)ControllerContext.Request.Properties["MS_HttpContext"]).Request,
                _userLoginService.GetbyUserName(model.Username));
        }


        [HttpGet]
        public UserSessionModel Identity(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new InvalidDataProvidedException("Blank token reported.");
            var model = _sessionFactory.GetActiveSessionModel(sessionId);
            return model;
        }

        [HttpGet]
        public bool Logout()
        {
            if (_sessionContext.UserSession == null)
            {
                return true;
            }
            SessionHelper.DestroySession(_sessionContext.Token);
            return true;
        }

        [HttpPost]
        public bool SendPasswordLink([FromUri] string email)
        {
            bool result = _passwordResetService.SendPasswordLink(email);
            if (result)
                ResponseModel.SetSuccessMessage("Link has been sent successfully. Please check your email.");
            return result;
        }

        [HttpPost]
        public bool ResetPassword(ChangePasswordEditModel model)
        {
            bool result = _passwordResetService.ResetPassword(model);
            if (result)
                ResponseModel.SetSuccessMessage("Your password reset successfully.");
            return result;
        }

        [HttpPost]
        public bool ResetPasswordExpire(ChangePasswordEditModel model)
        {
            bool result = _passwordResetService.ResetPasswordExpire(model);
            return result;
        }

        [HttpPost]
        public string CustomerLogin([FromBody] LoginCustomerAuthenticationModel model)
        {
            //return "";
            var isValid = _loginAuthenticationModelValidator.IsValidForCustomer(model);

            if (!isValid)
            {
                PostResponseModel.Message = model.Message;
                return null;
            }
            var customerSignatureInfo = _loginAuthenticationModelValidator.GetCustomerSignatureInfo(model);

            var request = ((HttpContextBase)ControllerContext.Request.Properties["MS_HttpContext"]).Request;
            return SessionHelper.CreateNewSessionForCustomer(
                ((HttpContextBase)ControllerContext.Request.Properties["MS_HttpContext"]).Request,
                customerSignatureInfo.EstimateInvoice.CustomerId.GetValueOrDefault(), customerSignatureInfo.EstimateInvoice.InvoiceCustomerId.GetValueOrDefault(), customerSignatureInfo.EstimateInvoiceId.GetValueOrDefault(), customerSignatureInfo.TypeId.GetValueOrDefault(), customerSignatureInfo.Code);
        }
    }
}
