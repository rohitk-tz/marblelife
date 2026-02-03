using Api.Areas.Application.Controller;
using Api.Impl;
using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace API.Areas.Application.Controller
{
    public class FileController : BaseController
    {
        private readonly IRepository<Core.Application.Domain.File> _fileRepository;
        private readonly IFileService _fileService;
        private readonly IClock _clock;
        private const string SampleFile = "SampleFile";

        public FileController(IUnitOfWork unitOfWork, IFileService fileService, IClock clock)
        {
            _fileRepository = unitOfWork.Repository<Core.Application.Domain.File>();
            _fileService = fileService;
            _clock = clock;
        }
        [HttpPost]
        public async Task<FileModel> UploadZip()
        {
            return await (new FileUploadHelper()).UploadGeoCodeFile(Request);
        }
        [HttpPost]
        public async Task<FileModel> Upload()
        {
            return await (new FileUploadHelper()).Upload(Request);
        }
        [HttpPost]
        public async Task<FileModel> UploadForBeforeAfter()
        {
            return await (new FileUploadHelper()).UploadForBeforeAfter(Request);
        }

        [HttpPost]
        public async Task<FileModel> UploadFile()
        {
            return await (new FileUploadHelper(_clock, _fileService)).UploadFile(Request);
        }

        [AllowAnonymous]
        public HttpResponseMessage Get([FromUri] string url)
        {
            var responseStream = new MemoryStream();
            Stream fileStream = File.Open(url.ToPath(), FileMode.Open);
            fileStream.CopyTo(responseStream);
            fileStream.Close();

            responseStream.Position = 0;

            var response = Request.CreateResponse();
            response.Content = new StreamContent(responseStream);

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet);

            return response;
        }

        [AllowAnonymous]
        public string GetById([FromUri] long id)
        {
            var file = _fileRepository.Get(id);
            if (file != null)
            {
                StreamReader myFile = new StreamReader((file.RelativeLocation + @"\" + file.Name).ToFullPath());
                var content = myFile.ReadToEnd();
                myFile.Close();
                return content;
            }
            return null;
        }

        [AllowAnonymous]
        public HttpResponseMessage GetFile([FromUri] long id)
        {

            var file = _fileRepository.Get(id);
            if (file != null)
            {
                string name = ApplicationManager.Settings.SiteRootUrl;
                var responseStream = new MemoryStream();

                Stream fileStream = File.Open((file.RelativeLocation + @"\" + file.Name).ToFullPath().ToPath(), FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();
                fileStream.Dispose();
                responseStream.Position = 0;

                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet);
                response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + System.Uri.EscapeDataString(file.Name));
                return response;
            }
            return null;
        }

        [AllowAnonymous]
        public HttpResponseMessage GetExcelFileById(long id)
        {
            var file = _fileRepository.Get(id);
            if (file != null)
            {
                var filePath = (file.RelativeLocation + @"\" + file.Name).ToPath().ToFullPath();
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    var responseStream = new MemoryStream();
                    Stream fileStream = File.Open(filePath, FileMode.Open);
                    fileStream.CopyTo(responseStream);
                    fileStream.Close();

                    responseStream.Position = 0;


                    var response = Request.CreateResponse();
                    response.Content = new StreamContent(responseStream);
                    response.Content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = file.Name,
                    };

                    return response;

                }
            }
            return null;
        }

        [AllowAnonymous]
        public HttpResponseMessage GetExcelFileByName()
        {
            var loc = Assembly.GetExecutingAssembly().CodeBase;
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/bin") + "\\Sample_salesData.xlsx"; //  Path.GetDirectoryName(loc).Replace("file://", "") + "\\Sample_salesData.xlsx"; //System.Web.Hosting.HostingEnvironment.MapPath(@"~/Areas/SampleFile/Sample_salesData.xlsx");

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                var responseStream = new MemoryStream();
                Stream fileStream = File.Open(filePath, FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();

                responseStream.Position = 0;

                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Sample_salesData.xlsx",
                };

                return response;
            }
            return null;
        }

        [AllowAnonymous]
        public string GetFileUrl()
        {

            return ApplicationManager.Settings.SiteRootUrl; ;
        }


        [AllowAnonymous]
        public async Task<FileModel> UploadedExceltoImage()
        {

            return await (new FileUploadHelper(_clock, _fileService)).UploadFile(Request);
        }


        [AllowAnonymous]
        public HttpResponseMessage GetFileForBeforeAfter([FromUri] long id)
        {

            var file = _fileRepository.Get(id);
            if (file != null)
            {
                string name = ApplicationManager.Settings.SiteRootUrl;
                var responseStream = new MemoryStream();

                Stream fileStream = File.Open((file.RelativeLocation + @"\" + file.Name), FileMode.Open);
                fileStream.CopyTo(responseStream);
                fileStream.Close();
                fileStream.Dispose();
                responseStream.Position = 0;

                var response = Request.CreateResponse();
                response.Content = new StreamContent(responseStream);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet);
                response.Content.Headers.Add("Content-Disposition", "attachment; filename=" + System.Uri.EscapeDataString(file.Name));
                return response;
            }
            return null;
        }


        [HttpPost]
        public string UploadBeforeAfterZipFile([FromBody] FileModels fileModels)
        {
            fileModels.FileModel = fileModels.FileModel.Distinct().ToList();
            try
            {
                using (var zip = new ZipFile())
                {
                    foreach (var id in fileModels.FileModel)
                    {
                        var file = _fileRepository.Get(id);
                        if (file != null)
                        {
                            string filePath = file.RelativeLocation + @"\" + file.Name;
                            zip.AddFile(filePath,"");
                        }
                    }
                    var fileName2 = "Before_After-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                    var fileName = "Before_After-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".zip";
                    var rootPath = MediaLocationHelper.GetZipImageLocation().Path + "/" + fileName;
                    zip.Save(rootPath);

                    return fileName2;
                }
            }
            catch (Exception e1)
            {
                return "";
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage DownloadZipFile([FromUri] string fileId)
        {
            fileId = fileId + ".zip";
            var response = Request.CreateResponse();
            try
            {
                using (var zip = new ZipFile())
                {
                    int a = 0;
                    var rootPath = MediaLocationHelper.GetZipImageLocation().Path + "/" + fileId;
                    //zip.Save(memoryStream);
                    var responseStream = new MemoryStream();
                    Stream fileStream = File.Open(rootPath, FileMode.Open);
                    fileStream.CopyTo(responseStream);
                    fileStream.Close();

                    responseStream.Position = 0;


                    response.Content = new StreamContent(responseStream);

                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet);

                    return response;
                }
            }
            catch (Exception e1)
            {
                return response;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage DownloadCustomerInvoice([FromUri] string fileName)
        {
            fileName = fileName + ".zip";
            var response = Request.CreateResponse();
            try
            {
                using (var zip = new ZipFile())
                {
                    int a = 0;
                    var rootPath = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "/" + fileName;
                    //zip.Save(memoryStream);
                    var responseStream = new MemoryStream();
                    Stream fileStream = File.Open(rootPath, FileMode.Open);
                    fileStream.CopyTo(responseStream);
                    fileStream.Close();

                    responseStream.Position = 0;


                    response.Content = new StreamContent(responseStream);

                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet);

                    return response;
                }
            }
            catch (Exception e1)
            {
                return response;
            }
        }

        [AllowAnonymous]
        public HttpResponseMessage GetBeforeAfter([FromUri] long? id)
        {
            var file = _fileRepository.Get(id.Value);
            var responseStream = new MemoryStream();
            Stream fileStream = File.Open(file.RelativeLocation + @"\" + file.Name, FileMode.Open);
            fileStream.CopyTo(responseStream);
            fileStream.Close();

            responseStream.Position = 0;

            var response = Request.CreateResponse();
            response.Content = new StreamContent(responseStream);

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet);

            return response;
        }
        [NoValidatorRequired]
        public class FileModels
        {
            public List<long> FileModel { get; set; }
        }

        [HttpPost]
        public async Task<FileModel> UploadnvoiceFile()
        {
            return await (new FileUploadHelper()).UploadnvoiceFile(Request);
        }

        [HttpPost]
        public async Task<FileModel> UploadDyamicFile()
        {
            return await (new FileUploadHelper()).UploadCustomer(Request);
        }
    }
}
