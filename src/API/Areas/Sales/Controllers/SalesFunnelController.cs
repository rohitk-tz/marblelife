using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Sales;
using Core.Sales.Impl;
using Core.Sales.ViewModel;
using Core.Users.Enum;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Sales.Controllers
{
    public class SalesFunnelController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly ISalesFunnelNationalService _salesfUnnelNationalService;
        // GET: Sales/SalesFunnel
        public SalesFunnelController(ISessionContext sessionContext, SalesFunnelNationalService salesfUnnelNationalService)
        {
            _sessionContext = sessionContext;
            _salesfUnnelNationalService = salesfUnnelNationalService;
        }

        [HttpGet]
        public SalesFunnelNationalListModel Get([FromUri]SalesFunnelNationalListFilter filter)
        {
            //if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            //{
                //filter.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId;
                filter.UserId = _sessionContext.UserSession.UserId;
                filter.RoleId = _sessionContext.UserSession.RoleId;
            //}
            return _salesfUnnelNationalService.GetSalesFunnelNationalList(filter);
        }
        [HttpPost]
        public HttpResponseMessage DownloadFunelNationalFile([FromBody]SalesFunnelNationalListFilter filter)
        {
            string fileName;
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                if (_sessionContext.UserSession.LoggedInOrganizationId != null)
                    filter.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId;
                else
                    filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
                filter.UserId = _sessionContext.UserSession.UserId;
            filter.RoleId = _sessionContext.UserSession.RoleId;
            }
            var result = _salesfUnnelNationalService.CreateExcelForNatioanlFunnel(filter, out fileName);
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
                    FileName = "QB_Invoice_Payment.xlsx",
                };

                return response;
            }
            else return null;

        }

        [HttpGet]
        public SalesFunnelNationalListModel GetSalesFunnelLocal([FromUri]SalesFunnelNationalListFilter filter)
        {
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FranchiseeAdmin || _sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                if(_sessionContext.UserSession.LoggedInOrganizationId!=null)
                filter.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId;
                else
                    filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _salesfUnnelNationalService.GetSalesFunnelLocalList(filter);
        }

        [HttpPost]
        public HttpResponseMessage DownloadFunelLocalFile([FromBody]SalesFunnelNationalListFilter filter)
        {
            string fileName;
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive || _sessionContext.UserSession.RoleId == (long)RoleType.FranchiseeAdmin)
            {
                if (_sessionContext.UserSession.LoggedInOrganizationId != null)
                    filter.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId;
                else
                    filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            var result = _salesfUnnelNationalService.CreateExcelForLocalFunnel(filter, out fileName);
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
                    FileName = "QB_Invoice_Payment.xlsx",
                };

                return response;
            }
            else return null;

        }
        [HttpPost]
        public SalesFunnelLocalGraphListModel GetChartData([FromBody]SalesFunnelNationalListFilter filter)
        {
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FranchiseeAdmin || _sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                if (_sessionContext.UserSession.LoggedInOrganizationId != null)
                    filter.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId;
                else
                    filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _salesfUnnelNationalService.GenerateChartData(filter);
        }
    }
}