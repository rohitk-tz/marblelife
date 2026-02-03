using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Organizations.ViewModel;
using Core.Sales;
using Core.Sales.ViewModel;
using Core.Users.Enum;
using System.Web.Http;

namespace API.Areas.Payment.Controllers
{
    public class FranchiseeAccountCreditController : BaseController
    {
        private readonly IAccountCreditService _accountCreditService;
        private ISessionContext _sessionContext;

        public FranchiseeAccountCreditController(ISessionContext sessionContext, IAccountCreditService accountCreditService)
        {
            _sessionContext = sessionContext;
            _accountCreditService = accountCreditService;
        }
        [HttpGet]
        public FranchiseeAccountCreditList GetFranchiseeAccountCredit(long franchiseeId, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _accountCreditService.GetFranchiseeAccountCredit(franchiseeId, pageNumber, pageSize);
        }

        [HttpPost]
        public bool SaveAccountCredit(FranchiseeAccountCreditEditModel accountCredit, [FromUri]long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Can't create Account Credit.");
                return false;
            }
            var organizationRoleUserId = _sessionContext.UserSession.OrganizationRoleUserId;
            _accountCreditService.SaveAccountCredit(accountCredit, franchiseeId, organizationRoleUserId);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Account Credit Created successfully.");
            return true;
        }

        [HttpGet]
        public bool DeleteAccountCredit([FromUri]long accountCreditId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                ResponseModel.SetSuccessMessage("You don't have access to delete the AccountCredit.");
                return false;
            }
            var result = _accountCreditService.DeleteAccountCredit(accountCreditId);
            if (result)
            {
                ResponseModel.SetSuccessMessage("AccountCredit has been deleted successfully.");
                return true;
            }
            else
            {
                ResponseModel.SetSuccessMessage("can't delete AccountCredit.");
                return false;
            }
        }

        [HttpGet]
        public bool RemoveAccountCredit([FromUri]long accountCreditId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                ResponseModel.SetSuccessMessage("You don't have access to Remove AccountCredit.");
                return false;
            }
            var result = _accountCreditService.RemoveAccountCredit(accountCreditId);
            if (result)
            {
                ResponseModel.SetSuccessMessage("Credit Amount has been removed successfully.");
                return true;
            }
            else
            {
                ResponseModel.SetSuccessMessage("can't remove Credit amount.");
                return false;
            }
        }

        [HttpGet]
        public FranchiseeAccountCreditViewModel GetCreditForInvoice([FromUri]long franchiseeId, [FromUri]long invoiceId)
        {
          return _accountCreditService.GetCreditForInvoice(franchiseeId,invoiceId);
        }

    }
}