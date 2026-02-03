using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.MarketingLead;
using Core.MarketingLead.ViewModel;
using Core.Users.Enum;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.MarketingLead.Controllers
{
    [AllowAnonymous]
    public class RoutingNumberController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IMarketingLeadsReportService _marketingLeadService;

        public RoutingNumberController(ISessionContext sessionContext, IMarketingLeadsReportService marketingLeadService)
        {
            _sessionContext = sessionContext;
            _marketingLeadService = marketingLeadService;
        }

        [HttpGet]
        public RoutingNumberListModel Get([FromUri]CallDetailFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _marketingLeadService.GetRoutingNumberList(filter, pageNumber, pageSize);
        }

        [HttpPost]
        public bool UpdateFranchisee([FromUri]long id, [FromBody]long? franchiseeId)
        {
            var result = _marketingLeadService.UpdateFranchisee(id, franchiseeId);
            if (!result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Error Updating Franchisee!");
                return false;
            }
            else
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Franchisee Updated!");
            return true;
        }

        [HttpPost]
        public HttpResponseMessage DownloadRoutingNumber([FromBody]CallDetailFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            string fileName;
            var result = _marketingLeadService.DownloadRoutingNumber(filter, out fileName);
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
                    FileName = "routingNumber.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public bool UpdateTag([FromUri]long id, [FromBody]long? tagId)
        {
            var result = _marketingLeadService.UpdateTag(id, tagId); 
            if (!result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Error Updating Tag!");
                return false;
            }
            else
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Tag Updated!");
            return true;
        }
    }
}