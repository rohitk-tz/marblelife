using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Sales.ViewModel;
using Core.Scheduler;
using Core.Scheduler.ViewModel;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace API.Areas.Scheduler.Controllers
{
    public class GeoCodeController : BaseController
    {
        private readonly IGeoCodeService _geoCodeService;
        private readonly ISessionContext _sessionContext;

        public GeoCodeController(IGeoCodeService geoCodeService, ISessionContext sessionContext)
        {
            _geoCodeService = geoCodeService;
            _sessionContext = sessionContext;
        }

        [HttpPost]
        public bool Upload([FromBody]CustomerFileUploadCreateModel model)
        {
            _geoCodeService.SaveFile(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("The list has been uploaded successfully, data will be updated shortly.");
            return true;
        }

        [HttpGet]
        public ZipDataUploadListModel GetList([FromUri]ZipDataListFilter filter, [FromUri]int pageNumber, [FromUri]int pageSize) 
        {
            return _geoCodeService.GetZipList(filter, pageNumber, pageSize);
        }

        [HttpPost]
        public HttpResponseMessage DownloadAllGeoCodeFile([FromBody]ZipDataListFilter filter)
        {
            string fileName;
            var result = _geoCodeService.CreateExcelForAllFiles(filter, out fileName);
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
                    FileName = "Geo_Code-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx",                };
                return response;
            }
            else return null;
        }

        [HttpPost]
        public ZipDataInfoListModel GetGeoCodeInfo([FromBody]ZipCodeInfoListFilter filter)
        {
            filter.RoleId = _sessionContext.UserSession.RoleId;
            return _geoCodeService.GetZipInfo(filter);
        }

        [HttpGet]
        public ZipDataInfoViewModel GetGeoInfo([FromUri] long franchiseeId)
        {
            return _geoCodeService.GetZipInfoScheduler(franchiseeId);
        }

        [HttpGet]
        public GeoCodeDataInfoViewModel GetGeoInfoByZipCode([FromUri] string zipCode, [FromUri] string state, [FromUri] long? franchiseeId, [FromUri] long? countryId)
        {
            if (state == "default") state = "";
            zipCode = zipCode.TrimStart(new Char[] { '0' });
           
            return _geoCodeService.GetZipInfoByZipCode(zipCode,state, franchiseeId, countryId);
        }

        [HttpPost]
        public bool SaveGeoCodeNotes([FromBody]ZipDataUploadViewModel filter)
        {
            var result= _geoCodeService.SaveGeoCodeNotes(filter);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Note Added!");
            }
            return result;
        }
        [HttpPost]
        public bool DeleteRecord(long batchId)
        {
            var result = _geoCodeService.DeleteGeoCodeRecord(batchId);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Geo Code Record Deleted Successfully!");
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Error in Deleting Record!");
            }
            return true;
        }
        [HttpPost]
        public bool ReparseFile(long batchId)
        {
            var result = _geoCodeService.ReparseFile(batchId);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Geo Code Reparse Successfully!");
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Error in Reparsing File!");
            }
            return true;
        }
    }
}