using Core.Application;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
namespace Api.Impl
{

    public class FileUploadHelper
    {
        System.Drawing.Image imgBef;
        System.Drawing.Image _imgR;
        private readonly IClock _clock;
        private readonly IFileService _fileService;

        public FileUploadHelper()
        {
        }

        public FileUploadHelper(IClock clock, IFileService fileService)
        {
            _clock = clock;
            _fileService = fileService;
        }
        public async Task<FileModel> UploadGeoCodeFile(HttpRequestMessage request)
        {
            try
            {
                if (!request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                var rootPath = MediaLocationHelper.GetZipMediaLocation().Path;

                var streamProvider = new MultipartFormDataStreamProvider(rootPath);
                await request.Content.ReadAsMultipartAsync(streamProvider);

                var model = GetFileModelForUploadedFile(streamProvider);

                //model.ContentType = request.Content.Headers.ContentType.MediaType;


                return model;
            }
            catch (Exception exception)
            {
                throw new HttpException((Int32)HttpStatusCode.InternalServerError, exception.Message);
            }
        }
        public async Task<FileModel> Upload(HttpRequestMessage request)
        {
            try
            {
                if (!request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                var rootPath = MediaLocationHelper.GetSalesMediaLocation().Path;

                var streamProvider = new MultipartFormDataStreamProvider(rootPath);
                await request.Content.ReadAsMultipartAsync(streamProvider);

                var model = GetFileModelForUploadedFile(streamProvider);

                //model.ContentType = request.Content.Headers.ContentType.MediaType;


                return model;
            }
            catch (Exception exception)
            {
                throw new HttpException((Int32)HttpStatusCode.InternalServerError, exception.Message);
            }
        }



        public async Task<FileModel> UploadForBeforeAfter(HttpRequestMessage request)
        {
            try
            {
                if (!request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                var rootPath = MediaLocationHelper.GetSalesMediaLocation().Path;

                var streamProvider = new MultipartFormDataStreamProvider(rootPath);
                await request.Content.ReadAsMultipartAsync(streamProvider);

                var model = GetFileModelForUploadedFile(streamProvider);

                //model.ContentType = request.Content.Headers.ContentType.MediaType;

                //imgBef = System.Drawing.Image.FromFile(rootPath + model.Name);
                //_imgR = Resize(imgBef, 600, 400, true);

                return model;
            }
            catch (Exception exception)
            {
                throw new HttpException((Int32)HttpStatusCode.InternalServerError, exception.Message);
            }
        }

        public async Task<FileModel> UploadFile(HttpRequestMessage request)
        {
            try
            {
                if (!request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                var rootPath = MediaLocationHelper.GetTempImageLocation().Path;

                var streamProvider = new MultipartFormDataStreamProvider(rootPath);
                await request.Content.ReadAsMultipartAsync(streamProvider);

                var model = GetFileModelForUploadedFile(streamProvider);

                //model.ContentType = request.Content.Headers.ContentType.MediaType;


                return model;
            }
            catch (Exception exception)
            {
                throw new HttpException((Int32)HttpStatusCode.InternalServerError, exception.Message);
            }
        }

        private FileModel GetFileModelForUploadedFile(MultipartFormDataStreamProvider streamProvider)
        {

            var localFileName = streamProvider.FileData[0].LocalFileName;

            var fileName = streamProvider.FileData[0].Headers.ContentDisposition.FileName.Replace("\"", "");

            var contentType = streamProvider.FileData[0].Headers.ContentType == null ? "" : streamProvider.FileData[0].Headers.ContentType.MediaType;

            return new FileModel
            {
                Name = Path.GetFileName(localFileName),
                Caption = Path.GetFileNameWithoutExtension(fileName),
                RelativeLocation = Path.GetDirectoryName(localFileName).ToRelativePath(),
                MimeType = string.IsNullOrEmpty(contentType) ? System.Net.Mime.MediaTypeNames.Text.Plain : contentType,
                Size = new FileInfo(localFileName).Length,
                Extension = Path.GetExtension(fileName)
            };
        }


        public async Task<FileModel> UploadnvoiceFile(HttpRequestMessage request)
        {
            try
            {
                if (!request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                var rootPath = MediaLocationHelper.GetZipMediaLocation().Path;

                var streamProvider = new MultipartFormDataStreamProvider(rootPath);
                await request.Content.ReadAsMultipartAsync(streamProvider);

                var model = GetFileModelForUploadedFile(streamProvider);

                //model.ContentType = request.Content.Headers.ContentType.MediaType;


                return model;
            }
            catch (Exception exception)
            {
                throw new HttpException((Int32)HttpStatusCode.InternalServerError, exception.Message);
            }
        }

        public async Task<FileModel> UploadCustomer(HttpRequestMessage request)
        {
            try
            {
                if (!request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                var rootPath = MediaLocationHelper.GetACustomerInvoiceLocation().Path;

                var streamProvider = new MultipartFormDataStreamProvider(rootPath);
                await request.Content.ReadAsMultipartAsync(streamProvider);

                var model = GetFileModelForUploadedFile(streamProvider);

                //model.ContentType = request.Content.Headers.ContentType.MediaType;


                return model;
            }
            catch (Exception exception)
            {
                throw new HttpException((Int32)HttpStatusCode.InternalServerError, exception.Message);
            }
        }

    }
}