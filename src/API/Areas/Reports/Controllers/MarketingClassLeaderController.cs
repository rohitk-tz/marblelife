using Api.Areas.Application.Controller;
using Core.Application;
using Core.Reports;
using Core.Reports.ViewModel;
using Core.Users.Enum;
using System.Web.Http;

namespace API.Areas.Reports.Controllers
{
    public class MarketingClassLeaderController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IReportService _reportService; 

        public MarketingClassLeaderController(ISessionContext sessionContext, IReportService reportService)
        {
            _sessionContext = sessionContext;
            _reportService = reportService;
        }

        [HttpGet]
        public TopLeadersListModel Get([FromUri]TopLeadersFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.LoggedInFranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _reportService.GetClassLeaderList(filter); 
        }
    }
}