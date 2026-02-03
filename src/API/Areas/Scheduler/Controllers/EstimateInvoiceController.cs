using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Sales.ViewModel;
using Core.Scheduler.ViewModel;
using Core.Scheduler;
using System.Web.Http;
using System.Collections.Generic;
using Core.Users.Enum;

namespace API.Areas.Scheduler.Controllers
{
    public class EstimateInvoiceController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionContext _sessionContext;
        private readonly IEstimateInvoiceServices _estimateInvoiceService;

        public EstimateInvoiceController(ISessionContext sessionContext, IUnitOfWork unitOfWork, IEstimateInvoiceServices estimateInvoiceService)
        {
            _unitOfWork = unitOfWork;
            _sessionContext = sessionContext;
            _estimateInvoiceService = estimateInvoiceService;
        }
        [HttpPost]
        public EstimateInvoiceViewModel GetEstimateInvoice([FromBody] InvoiceEstimateFilterModel model)
        {
            var userId= _sessionContext.UserSession.UserId;
            var roleId = _sessionContext.UserSession.RoleId;
            return _estimateInvoiceService.GetEstimateInvoice(model, userId,roleId);
        }

        [HttpPost]
        public bool SaveEstimateInvoice(EstimateInvoiceEditModel model)
        {
            model.UserId = _sessionContext.UserSession.UserId;
            return _estimateInvoiceService.SaveEstimateInvoice(model);
        }

        [HttpPost]
        public int MailToCustomer(SelectInvoicesViewModel model)
        {
            model.LoggedInUserId = _sessionContext.UserSession.UserId;
            var result = _estimateInvoiceService.SendMailToCustomer(model.SchedulerId!=null? model.SchedulerId: model.JobSchedulerId, model.ServiceInvoice, "cutomer_invoice.cshtml", model.UserId, model);
            if (result == 1)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Mail Sent Successfully!");
                return 1;
            }
            else if (result == 0)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Error in Sending Mail!");
                return 0;
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Please Save the Invoice before sending the email");
                return -1;
            }
        }

        public string UploadInvoicesZipFile(SelectInvoicesViewModel model)
        {
            return _estimateInvoiceService.UploadInvoicesZipFile(model.SchedulerId.Value, model.ServiceInvoice);
        }

        public string UploadInvoicesCustomerZipFile(SelectInvoicesViewModel model)
        {
            return _estimateInvoiceService.UploadInvoicesCustomerZipFile(model.SchedulerId.Value, model.ServiceInvoice);
        }


        [HttpPost]
        public bool SaveCustomerSignature(CustomersignatureViewModel model)
        {
            model.UserId = _sessionContext.UserSession.UserId;
            return _estimateInvoiceService.SaveCustomerSignature(model);
        }
        public string UploadSignedInvoicesZipFile(JobInvoiceDownloadViewModel model)
        {
            return _estimateInvoiceService.UploadSignedInvoicesZipFile(model);
        }
        [HttpPost]
        public bool AddInvoiceToEstimate([FromUri] long schedulerId)
        {
            return _estimateInvoiceService.AddInvoiceToEstimate(schedulerId, false);
        }
        [HttpPost]
        public bool SendFeedBackMailToCustomer(SelectInvoicesViewModel model)
        {
            return _estimateInvoiceService.SendFeedBackMailToCustomer(model);
        }

        [HttpPost]
        public bool CustomerIsAvailableOrNot(SelectInvoicesViewModel model)
        {
            return _estimateInvoiceService.CustomerIsAvailableOrNot(model);
        }
        [HttpPost]
        public EstimateInvoiceServiceGetServiceResultModel GetServiceTypeId(EstimateInvoiceServiceGetServiceModel model)
        {
            return _estimateInvoiceService.GetServiceTypeId(model);
        }
    }

}