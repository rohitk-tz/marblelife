using Api.Areas.Application.Controller;
using Core.Application;
using Core.Notification.ViewModel;
using Core.Reports;
using Core.Reports.ViewModel;
using Core.Users.Enum;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Reports.Controllers
{
    public class ARReportController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IReportService _reportService;
        public ARReportController(ISessionContext sessionContext, IReportService reportService)
        {
            _sessionContext = sessionContext;
            _reportService = reportService;
        }

        [HttpPost]
        public ARReportListModel Get([FromBody] ArReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _reportService.GetARReportList(filter);
        }
    }
}