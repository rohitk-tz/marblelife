using Api.Areas.Application.Controller;
using Core.Application;
using Core.Reports;
using Core.Reports.ViewModel;
using Core.Scheduler.ViewModel;
using Core.Users.Enum;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Reports.Controllers
{
    [AllowAnonymous]
    public class CustomerEmailReportController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly ICustomerEmailReportService _customerEmailReportService;

        public CustomerEmailReportController(ISessionContext sessionContext, ICustomerEmailReportService customerEmailReportService)
        {
            _sessionContext = sessionContext;
            _customerEmailReportService = customerEmailReportService;
        }

        [HttpGet]
        public CustomerEmailReportListModel Get([FromUri]CustomerEmailReportFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _customerEmailReportService.GetCustomerEmailReportList(filter);
        }

        public EmailChartDataListModel GetChartData([FromUri]long franchiseeId, [FromUri]DateTime startDate, [FromUri]DateTime endDate)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _customerEmailReportService.GetChartData(franchiseeId, startDate, endDate);
        }

        [HttpPost]
        public HttpResponseMessage DownloadEmailReport([FromBody]CustomerEmailReportFilter filter)
        {
            string fileName;
            var result = _customerEmailReportService.DownloadEmailReport(filter, out fileName);
            if (result)
            {
                var responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();

                responseStream.Position = 0;

                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "EmailReport.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public MailListModel GetFranchiseeWiseMails([FromBody]MailListFilter query)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                query.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive && query.FranchiseeId <= 1)
            {
                query.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _customerEmailReportService.GetFranchiseeWiseMail(query);
        }

        public ReviewChartDataListModel GetChartDataForReviews([FromUri] long franchiseeId, [FromUri] DateTime startDate, [FromUri] DateTime endDate)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _customerEmailReportService.GetChartDataForReview(franchiseeId, startDate, endDate);
        }

        [HttpGet]
        public ReviewCountModel GetReviewCounts([FromUri] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _customerEmailReportService.GetReviewCounts(franchiseeId);
        }
    }
}