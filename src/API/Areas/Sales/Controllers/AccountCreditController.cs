using Api.Areas.Application.Controller;
using Core.Application;
using Core.Sales;
using Core.Sales.ViewModel;
using Core.Users.Enum;
using System.Web.Http;

namespace API.Areas.Sales.Controllers
{
    public class AccountCreditController : BaseController
    {
        private readonly IAccountCreditService _accountCreditService;
        private readonly ISessionContext _sessionContext;

        public AccountCreditController(IAccountCreditService accountCreditService, ISessionContext sessionContext)
        {
            _accountCreditService = accountCreditService;
            _sessionContext = sessionContext;
        }

        [HttpGet]
        public AccountCreditListModel Get([FromUri]AccountCreditListFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _accountCreditService.Get(filter, pageNumber, pageSize);
        }

        [HttpGet]
        public FranchiseeAccountCreditListModel GetAccountCreditList([FromUri]AccountCreditListFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _accountCreditService.GetAccountCreditList(filter, pageNumber, pageSize);
        }
    }
}