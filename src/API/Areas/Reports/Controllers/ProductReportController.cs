using Api.Areas.Application.Controller;
using Core.Application;
using Core.Reports;
using Core.Reports.Impl;
using Core.Reports.ViewModel;
using Core.Users.Enum;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Reports.Controllers
{
    public class ProductReportController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IProductReportService _productReportService;

        public ProductReportController(ISessionContext sessionContext, IProductReportService productReportService)
        {
            _sessionContext = sessionContext;
            _productReportService = productReportService;
        }

        [HttpGet]
        public ProductChannelReportListModel Get([FromUri]ProductReportListFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _productReportService.GetReport(filter, pageNumber, pageSize);
        }

        [HttpPost]
        public HttpResponseMessage DownloadReport([FromBody]ProductReportListFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }

            string fileName;
            var result = _productReportService.DownloadReport(filter, out fileName);
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
                    FileName = "productReport.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public ProductReportChartListModel GetChartData([FromBody]ProductReportListFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _productReportService.GenerateChartData(filter);
        }

    }
}