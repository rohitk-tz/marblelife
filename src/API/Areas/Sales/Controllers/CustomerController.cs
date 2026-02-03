using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Sales;
using Core.Sales.ViewModel;
using Core.Users.Enum;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Sales.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        private ISessionContext _sessionContext;

        public CustomerController(ICustomerService customerService, ISessionContext sessionContext)
        {
            _customerService = customerService;
            _sessionContext = sessionContext;
        }

        [HttpGet]
        public CustomerEditModel Get(long id)
        {
            return _customerService.Get(id);
        }
        [HttpPost]
        public bool SaveCustomer([FromBody]CustomerEditModel model)
        {
            _customerService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Customer updated successfully.");
            return true;
        }
        [HttpGet]
        public CustomerListModel Get([FromUri]CustomerListFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _customerService.GetCustomers(filter, pageNumber, pageSize);
        }
        [HttpPost]
        public HttpResponseMessage DownloadCustomerFile([FromBody]CustomerListFilter filter)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin)
            {
                filter.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            string fileName;
            var result = _customerService.DownloadCustomerFile(filter, out fileName);
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
                    FileName = "Customer.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public bool Upload([FromBody]CustomerFileUploadCreateModel model)
        {
            _customerService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Customer list has been uploaded successfully, customer will update shortly.");
            return true;
        }

        [HttpPost]
        public bool UpdateMarketingClass([FromUri]long id, [FromBody]long classTypeId)
        {
            var result = _customerService.UpdateMarketingClass(id, classTypeId);
            if (!result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Can't Update Marketing Class.");
                return false;
            }
            else
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Marketing class Updated.");
            return true;
        }
    }
}