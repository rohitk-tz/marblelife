
using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Reports;
using Core.Reports.ViewModel;
using Core.Users.Enum;
using System.Web.Http;

namespace API.Areas.Reports.Controllers
{
    public class MLFSReportController : BaseController
    {
        private readonly ISessionContext _sessionContext;
        private readonly IMlfsReport _iMlfsReportService;
        public MLFSReportController(ISessionContext sessionContext, IMlfsReport iMlfsReportService)
        {
            _sessionContext = sessionContext;
            _iMlfsReportService = iMlfsReportService;
        }
        [HttpPost]
        public MLFSReportListModel GetReportForPurchase([FromBody] MLFSReportListFilter filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;
            return _iMlfsReportService.GetReportForPurchase(filter);
        }
        [HttpPost]
        public MLFSReportListModel GetReportForSales([FromBody] MLFSReportListFilter filter)
        {
            filter.UserId = _sessionContext.UserSession.UserId;
            return _iMlfsReportService.GetReportForSale(filter);
        }

        [HttpPost]
        public MLFSReportConfigurationListModel GetMLFSConfigurationData([FromBody] MLFSConfigurationFilter filter)
        {
                filter.UserId = _sessionContext.UserSession.UserId;
            return _iMlfsReportService.GetMLFSConfiguration(filter);
        }
        [HttpPost]
        public bool SaveMLFSConfigurationData([FromBody] MLFSEditModel filter)
        {
                filter.UserId = _sessionContext.UserSession.UserId;
            var isSaved= _iMlfsReportService.SaveMLFSConfiguration(filter);
            if (isSaved)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Value(s) saved successfully!!");
                return true;
            }
            else
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Error in Saving Value(s)!!");
            return false;
        }
    }
}