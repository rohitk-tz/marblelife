using Api.Areas.Application.Controller;
using Core.Application;
using Core.Review;
using Core.Review.ViewModel;
using Core.Users.Enum;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Reports.Controllers
{
    [AllowAnonymous]
    public class CustomerFeedbackReportController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly ICustomerFeedbackReportService _customerFeedbackReportService;

        public CustomerFeedbackReportController(ISessionContext sessionContext, ICustomerFeedbackReportService customerFeedbackReportService)
        {
            _sessionContext = sessionContext;
            _customerFeedbackReportService = customerFeedbackReportService;
        }

        [HttpGet]
        public CustomerFeedbackReportListModel Get([FromUri]CustomerFeedbackReportFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _customerFeedbackReportService.GetCustomerFeedbackList(filter, pageNumber, pageSize);
        }

        [HttpGet]
        public CustomerFeedbackReportViewModel GetFeedbackDetail([FromUri]string responseId)
        {
            var splittedValue = responseId.Split('_');
            var isFromNewSystemValue = splittedValue[1]=="1"?true:false;
            var isFromSystemTableValue = splittedValue[2] == "1" ? true : false;
            return _customerFeedbackReportService.GetCustomerFeedbackDetail(long.Parse(splittedValue[0]), isFromNewSystemValue, isFromSystemTableValue);
        }

        [HttpPost]
        public HttpResponseMessage DownloadFeedbackReport([FromBody]CustomerFeedbackReportFilter filter)
        {
            string fileName;
            var result = _customerFeedbackReportService.DownloadFeedbackReport(filter, out fileName);
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
                    FileName = "FeedbackReport.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public bool ManageCustomerFeedbackStatus(bool isAccept, long customerId, long id, string fromTable)
        {
            var res = _customerFeedbackReportService.ManageCustomerFeedbackStatus(isAccept, customerId, id, fromTable);
            return true;
        }

    }
}