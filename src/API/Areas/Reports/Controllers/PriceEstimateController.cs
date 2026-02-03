using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Reports;
using Core.Reports.ViewModel;
using Core.Users.Enum;
using System.Web.Http;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace API.Areas.Reports.Controllers
{
    public class PriceEstimateController : BaseController
    {
        // GET: Reports/PriceEstimate
        private readonly ISessionContext _sessionContext;
        private readonly IReportService _reportService;

        public PriceEstimateController(ISessionContext sessionContext, IReportService reportService)
        {
            _sessionContext = sessionContext;
            _reportService = reportService;
        }

        [HttpPost]
        public PriceEstimatePageViewModel GetPriceEstimateCollection(PriceEstimateGetModel model)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.GetPriceEstimateList(model, userId, roleUserId); 
        }
        [HttpPost]
        public PriceEstimateViewModel GetPriceEstimate(PriceEstimateGetModel model)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.GetPriceEstimate(model, userId, roleUserId); 
        }
        [HttpPost]
        public bool SaveBulkCorporatePriceEstimate(PriceEstimateSaveCorporatePriceModel model)
        {
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.SaveBulkCorporatePriceEstimate(model, roleUserId);
        }
        [HttpPost]
        public bool BulkUpdateCorporatePrice(PriceEstimateBulkUpdateModel model)
        {
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.BulkUpdateCorporatePrice(model, roleUserId);
        }
        [HttpPost]
        public bool SavePriceEstimateFranchiseeWise(PriceEstimateSaveModel model)
        {
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.SavePriceEstimateFranchiseeWise(model, roleUserId);
        }
        [HttpPost]
        public bool BulkUpdatePriceEstimate(PriceEstimateBulkUpdateModel model)
        {
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.SavePriceEstimateBulkUpdate(model, roleUserId);
        }

        [HttpPost]
        public PriceEstimatePageViewModel GetPriceEstimateCollectionPerFranchisee(PriceEstimateGetModel model)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.GetPriceEstimateCollectionPerFranchisee(model, userId, roleUserId);
        }
        [HttpPost]
        public ShiftChargesViewModel GetShiftCharges()
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.GetShiftCharges(userId, roleUserId);
        }
        
        [HttpPost]
        public bool SaveShiftCharges(ShiftChargesSaveModel model)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.SaveShiftCharges(model, userId, roleUserId);
        }

        [HttpPost]
        public ReplacementChargesViewModel GetReplacementCharges()
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.GetReplacementCharges(userId, roleUserId);
        }
        [HttpPost]
        public bool SaveReplacementCharges(ReplacementChargesSaveModel model)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.SaveReplacementCharges(model, userId, roleUserId);
        }

        [HttpPost]
        public MaintenanceChargesViewModel GetMaintenanceCharges()
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.GetMaintenanceCharges(userId, roleUserId);
        }

        [HttpPost]
        public bool SaveMaintenanceCharges(MaintenanceChargesSaveModel model)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.SaveMaintenanceCharges(model, userId, roleUserId);
        }
        [HttpPost]
        public FloorGrindingAdjustmentViewModel GetFloorGrindingAdjustment()
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.GetFloorGrindingAdjustment(userId, roleUserId);
        }        
        [HttpPost]
        public bool SaveFloorGrindingAdjustmentNote(FloorGrindingAdjustNoteSaveModel model)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.SaveFloorGrindingAdjustmentNote(userId, roleUserId, model);
        }
        [HttpPost]
        public SeoHistryViewModel GetSeoHistry(SeoHistryModel model)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.GetSeoHistry(model);
        }
        [HttpPost]
        public bool SaveSeoNotes(SeoNotesModel model)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            return _reportService.SaveSeoNotes(model);
        }

        [HttpPost]
        public HttpResponseMessage DownloadPriceEstimateDataFile([FromBody] PriceEstimateGetModel model)
        {
            string fileName;
            var userId = _sessionContext.UserSession.UserId;
            var roleUserId = _sessionContext.UserSession.RoleId;
            var result = _reportService.DownloadPriceEstimateDataFile(model, userId, roleUserId, out fileName);
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
                    FileName = "PriceEstimateData.xlsx",
                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public bool Upload([FromBody] PriceEstimateExcelUploadModel model)
        {
            model.UserId = _sessionContext.UserSession.UserId;
            model.RoleUserId = _sessionContext.UserSession.RoleId;
            _reportService.SaveFile(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("The list has been uploaded successfully, data will be updated shortly.");
            return true;
        }

        [HttpGet]
        public PriceEstimateDataUploadListModel GetList([FromUri] PriceEstimateDataListFilter filter, [FromUri] int pageNumber, [FromUri] int pageSize)
        {
            var userId = _sessionContext.UserSession.OrganizationRoleUserId;
            return _reportService.GetPriceEstimateUploadList(filter, pageNumber, pageSize, userId);
        }
        
        [HttpPost]
        public bool SaveServiceTagNotes(PriceEstimateServiceTagNotesModel filter)
        {
            var userId = _sessionContext.UserSession.OrganizationRoleUserId;
            return _reportService.SaveServiceTagNotes(filter, userId);
        }
        
        [HttpPost]
        public PriceEstimateServiceTagNotesGetModel GetServiceTagNotes(PriceEstimateServiceTagNotesGetModel filter)
        {
            var userId = _sessionContext.UserSession.OrganizationRoleUserId;
            return _reportService.GetServiceTagNotes(filter);
        }
    }
}