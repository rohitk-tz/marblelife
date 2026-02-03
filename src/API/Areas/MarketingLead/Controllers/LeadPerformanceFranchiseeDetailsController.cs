using Api.Areas.Application.Controller;
using Core.Application;
using Core.MarketingLead.ViewModel;
using Core.Organizations;
using Core.Users.Enum;
using System.Web.Http;


namespace API.Areas.MarketingLead.Controllers
{
  
    public class LeadPerformanceFranchiseeDetailsController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly ILeadPerformanceFranchiseeDetailsService _leadPerformanceFranchiseeDetailsService;
        
        public LeadPerformanceFranchiseeDetailsController(ISessionContext sessionContext, ILeadPerformanceFranchiseeDetailsService leadPerformanceFranchiseeDetailsService)
        {
            _sessionContext = sessionContext;
            _leadPerformanceFranchiseeDetailsService = leadPerformanceFranchiseeDetailsService;
        }

        [HttpPost]
        public LeadPerformanceListModel GetPerformanceHistry([FromBody] LeadPerformanceFranchiseeFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _leadPerformanceFranchiseeDetailsService.GetFranchiseePerformance(filter);
        }

        [HttpPost]
        public LeadPerformanceNationalListModel GetPerformanceReportNational([FromBody] LeadPerformanceFranchiseeFilter filter)
        {
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                filter.OrganizationRoleUserId = _sessionContext.UserSession.OrganizationRoleUserId;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FranchiseeAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _leadPerformanceFranchiseeDetailsService.GetLeadPerformanceNationalList(filter);
        }
        [HttpPost]
        public LeadFranchiseeNationallListModel GetSeoAndPpcNational([FromBody] LeadPerformanceFranchiseeFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _leadPerformanceFranchiseeDetailsService.GetSeoAndPpcNational(filter);
        }
        [HttpPost]
        public LeadFranchiseeLocalListModel GetSeoAndPpcLocal([FromBody] LeadPerformanceFranchiseeFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _leadPerformanceFranchiseeDetailsService.GetSeoAndPpcLocal(filter.FranchiseeId.GetValueOrDefault(),null);
        }
    }
}