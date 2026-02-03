using Api.Areas.Application.Controller;
using Core.Application;
using Core.Billing.ViewModel;
using Core.Dashboard;
using Core.Dashboard.Impl;
using Core.Dashboard.ViewModel;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace API.Areas.Dashboard.Controllers
{
    [AllowAnonymous]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;
        private ISessionContext _sessionContext;

        public DashboardController(ISessionContext sessionContext, IDashboardService dashboardService)
        {
            _sessionContext = sessionContext;
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public FranchiseeDirectoryListModel GetFranchiseeDirectoryList()
        {
            return _dashboardService.GetFranchiseeDirectoryList();
        }

        [HttpGet]
        public SalesSummaryListModel GetSalesSummary(long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId; ;
            }
            return _dashboardService.GetSalesSummary(franchiseeId);
        }

        [HttpGet]
        public RecentInvoiceListModel GetRecentInvoices(long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId; ;
            }
            return _dashboardService.GetRecentInvoices(franchiseeId);
        }

        public IList<SalesRepLeaderboardViewModel> GetSalesRepLeaderBoard(long franchiseeId, [FromUri]DateTime startDate, [FromUri]DateTime endDate)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId; ;
            }
            return _dashboardService.GetSalesRepLeaderboard(franchiseeId, startDate, endDate);
        }

        public IList<FranchiseeLeaderboardViewModel> GetFranchiseeLeaderboard(long franchiseeId, [FromUri]DateTime startDate, [FromUri]DateTime endDate)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId; ;
            }
            return _dashboardService.GetFranchiseeLeaderboard(franchiseeId, startDate, endDate);
        }

        [HttpGet]
        public List<ChartDataModel> GetRevenue(long franchiseeId, [FromUri] DateTime startDate, [FromUri]DateTime endDate)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId; ;
            }
            return _dashboardService.GetChartData(franchiseeId, startDate, endDate);
        }
        [HttpGet]
        public List<ChartDataModel> GetRevenueBasedOnService(long franchiseeId, [FromUri] DateTime startDate, [FromUri]DateTime endDate)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId; ;
            }
            return _dashboardService.GetChartDataForService(franchiseeId, startDate, endDate);
        }

        [HttpGet]
        public CustomerCountViewModel GetCustomerCount(long franchiseeId, [FromUri] DateTime startDate, [FromUri]DateTime endDate)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId; ;
            }
            return _dashboardService.GetCustomerCount(franchiseeId, startDate, endDate);
        }

        [HttpGet]
        public IEnumerable<StartEndDateViewModel> GetPendingUploadRange(long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _dashboardService.GetPendingUploadRange(franchiseeId);
        }

        [HttpGet]
        public RecentInvoiceListModel GetUnpaidInvoices(long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId; ;
            }
            return _dashboardService.GetUnpaidInvoices(franchiseeId);
        }

        [HttpGet]
        public AnnualUploadResponseModel GetAnnualUploadResponse(long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _dashboardService.GetAnnualUploadResponse(franchiseeId);
        }
        [HttpGet]
        public DocumentSummaryListModel GetDocumentSummary(long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId; ;
            }
            return _dashboardService.GetDocumentsSummary(franchiseeId);
        }
        [HttpGet]
        public DocumentPendingListModel GetPendingDocument(long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId; ;
            }
            return _dashboardService.GetDocumentsPending(franchiseeId);
        }
        [HttpGet]
        public FranchiseeDirectoryListModel GetFranchiseeDirectoryListForSuperAdmin(string franchiseeName)
        {
            if (franchiseeName == "INVALID") franchiseeName = "";
            return _dashboardService.GetFranchiseeDirectoryListForSuperAdmin(franchiseeName);
        }

        [HttpGet]
        public string RedirectionToBulkPhotoUpload()
        {
            var token = _sessionContext.Token;
            return _dashboardService.Redirection(token);
        }
    }
}